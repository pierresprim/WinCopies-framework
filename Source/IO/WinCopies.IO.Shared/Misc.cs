/* Copyright © Pierre Sprimont, 2020
*
* This file is part of the WinCopies Framework.
*
* The WinCopies Framework is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* The WinCopies Framework is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Desktop;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process.ObjectModel;

using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;
using System.Collections.Generic;

namespace WinCopies.IO
{
    public enum FileSystemEntryEnumerationOrder : byte
    {
        /// <summary>
        /// Does not sort items.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enumerates files then directories.
        /// </summary>
        FilesThenDirectories = 1,

        /// <summary>
        /// Enumerates directories then files.
        /// </summary>
        DirectoriesThenFiles = 2
    }

    public struct ClientVersion
    {
        public string ClientName { get; }
        public uint MajorVersion { get; }
        public uint MinorVersion { get; }
        public uint Revision { get; }

        public ClientVersion(in string clientName, in uint majorVersion, in uint minorVersion, in uint revision)
        {
            ClientName = clientName;

            MajorVersion = majorVersion;

            MinorVersion = minorVersion;

            Revision = revision;
        }

        public ClientVersion(in string clientName, in Version version) : this(clientName, (uint)version.Major, (uint)version.Minor, (uint)version.Revision)
        {
            // Left empty.
        }

        public ClientVersion(in AssemblyName assemblyName) : this(assemblyName.Name, assemblyName.Version)
        {
            // Left empty.
        }

        public override bool Equals(object obj) => obj is ClientVersion _obj ? _obj.ClientName == ClientName && _obj.MajorVersion == MajorVersion && _obj.MinorVersion == MinorVersion && _obj.Revision == Revision : false;

        public override int GetHashCode() => ClientName.GetHashCode() ^ MajorVersion.GetHashCode() ^ MinorVersion.GetHashCode() ^ Revision.GetHashCode();

        public static bool operator ==(ClientVersion left, ClientVersion right) => left.Equals(right);

        public static bool operator !=(ClientVersion left, ClientVersion right) => !(left == right);
    }

    public interface IRecursiveEnumerable<T> : Collections.Generic.IRecursiveEnumerable<T>
    {
        new RecursiveEnumeratorAbstract<T> GetEnumerator();
    }

    /// <summary>
    /// Indicates the browsing ability for an <see cref="IBrowsableObjectInfo"/>.
    /// </summary>
    public enum Browsability : byte
    {
        /// <summary>
        /// The item is not browsable.
        /// </summary>
        NotBrowsable = 0,

        /// <summary>
        /// The item is browsable by default.
        /// </summary>
        BrowsableByDefault = 1,

        /// <summary>
        /// The item is browsable but browsing should not be the default action to perform on this item.
        /// </summary>
        Browsable = 2,

        /// <summary>
        /// The item redirects to an other item, with this item browsable.
        /// </summary>
        RedirectsToBrowsableItem = 3
    }

    public interface IBrowsabilityPathBase
    {
        string Name { get; }
    }

    public interface IBrowsabilityPath : IBrowsabilityPathBase
    {
        IBrowsableObjectInfo GetPath();

        bool IsValid();
    }

    public interface IBrowsabilityPath<in T> : IBrowsabilityPathBase where T : IBrowsableObjectInfo
    {
        IBrowsableObjectInfo GetPath(T browsableObjectInfo);

        bool IsValidFor(T browsableObjectInfo);
    }

    public sealed class BrowsabilityPath<T> : IBrowsabilityPath, DotNetFix.IDisposable where T : IBrowsableObjectInfo
    {
        private IBrowsabilityPath<T> _browsabilityPath;
        private T _browsableObjectInfo;

        public bool IsDisposed { get; private set; }

        private IBrowsabilityPath<T> _BrowsabilityPath => IsDisposed ? throw GetExceptionForDispose(false) : _browsabilityPath;

        string IBrowsabilityPathBase.Name => _BrowsabilityPath.Name;

        public BrowsabilityPath(in IBrowsabilityPath<T> browsabilityPath, in T browsableObjectInfo)
        {
            _browsabilityPath = browsabilityPath;

            _browsableObjectInfo = browsableObjectInfo;
        }

        IBrowsableObjectInfo IBrowsabilityPath.GetPath() => _BrowsabilityPath.GetPath(_browsableObjectInfo);

        bool IBrowsabilityPath.IsValid() => _BrowsabilityPath.IsValidFor(_browsableObjectInfo);

        public void Dispose()
        {
            if (IsDisposed)

                return;

            _browsabilityPath = null;

            _browsableObjectInfo = default;

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        ~BrowsabilityPath() => Dispose();
    }

    public interface IBrowsabilityPathStack<T> where T : IBrowsableObjectInfo
    {
        void Push(IBrowsabilityPath<T> browsabilityPath);
    }

    public sealed class BrowsabilityPathStack<T> : IBrowsabilityPathStack<T> where T : IBrowsableObjectInfo
    {
        private IEnumerableStack<IBrowsabilityPath<T>> _stack;

        public BrowsabilityPathStack(in IEnumerableStack<IBrowsabilityPath<T>> stack) => _stack = stack;

        public BrowsabilityPathStack() : this(new EnumerableStack<IBrowsabilityPath<T>>()) { /* Left empty. */ }

        public System.Collections.Generic.IEnumerable<IBrowsabilityPath> GetBrowsabilityPaths(T browsableObjectInfo) => _stack.Select(item => new BrowsabilityPath<T>(item, browsableObjectInfo));

        public void Push(IBrowsabilityPath<T> browsabilityPath) => _stack.Push(browsabilityPath);

        public WriteOnlyBrowsabilityPathStack<T> AsWriteOnly() => new WriteOnlyBrowsabilityPathStack<T>(_stack);
    }

    public sealed class WriteOnlyBrowsabilityPathStack<T> : IBrowsabilityPathStack<T> where T : IBrowsableObjectInfo
    {
        private IEnumerableStack<IBrowsabilityPath<T>> _stack;

        public WriteOnlyBrowsabilityPathStack(in IEnumerableStack<IBrowsabilityPath<T>> stack) => _stack = stack;

        public void Push(IBrowsabilityPath<T> browsabilityPath) => _stack.Push(browsabilityPath);
    }

    public interface IBrowsabilityOptions
    {
        Browsability Browsability { get; }

        IBrowsableObjectInfo RedirectToBrowsableItem();
    }

    public static class BrowsabilityOptions
    {
        private class _Browsability : IBrowsabilityOptions
        {
            public Browsability Browsability { get; }

            internal _Browsability(Browsability browsability) => Browsability = browsability;

            IBrowsableObjectInfo IBrowsabilityOptions.RedirectToBrowsableItem() => null;
        }

        public static IBrowsabilityOptions NotBrowsable { get; } = new _Browsability(Browsability.NotBrowsable);

        public static IBrowsabilityOptions BrowsableByDefault { get; } = new _Browsability(Browsability.BrowsableByDefault);

        public static IBrowsabilityOptions Browsable { get; } = new _Browsability(Browsability.Browsable);

        public static bool IsBrowsable(this Browsability browsability)
#if CS8
            => browsability switch
            {
#if CS9
                Browsability.BrowsableByDefault or Browsability.Browsable => true,
#else
                Browsability.BrowsableByDefault => true,
                Browsability.Browsable => true,
#endif
                _ => false,
            };
#else
        {
            switch (browsability)
            {
                case Browsability.BrowsableByDefault:
                case Browsability.Browsable:

                    return true;
            }

            return false;
        }
#endif
    }

    namespace Process
    {
        public interface IProcessParameters
        {
            Guid Guid { get; }

            System.Collections.Generic.IEnumerable<string> Parameters { get; }
        }

        public class ProcessParameters : IProcessParameters
        {
            public Guid Guid { get; }

            public System.Collections.Generic.IEnumerable<string> Parameters { get; }

            public ProcessParameters(Guid guid, System.Collections.Generic.IEnumerable<string> parameters)
            {
                Guid = guid;

                Parameters = parameters;
            }

            public ProcessParameters(string guid, System.Collections.Generic.IEnumerable<string> parameters) : this(new Guid(guid), parameters) { /* Left empty. */ }
        }

        [Flags]
        public enum ProcessValidityScopeFlags
        {
            Global = 1,

            Local = 2
        }

        public interface IProcessInfo
        {
            string GroupName { get; }

            string Name { get; }

            bool CanRun(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

            IProcessParameters GetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

            IProcessParameters TryGetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public interface IProcessCommands : DotNetFix.IDisposable
        {
            string Name { get; }

            string Caption { get; }

            bool CanCreateNewItem();

            bool TryCreateNewItem(string name, out IProcessParameters result);

            IProcessParameters CreateNewItem(string name);
        }

        public interface IProcessInfoBase
        {
            bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public interface IProcessFactoryProcessInfo : IProcessInfoBase
        {
            bool UserConfirmationRequired { get; }

            string GetUserConfirmationText();
        }

        public interface IDirectProcessInfo : IProcessFactoryProcessInfo
        {
            IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

            IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public interface IRunnableProcessInfo : IProcessFactoryProcessInfo
        {
            bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            IProcessParameters GetProcessParameters(uint count);

            IProcessParameters TryGetProcessParameters(uint count);
        }

        public interface IDragDropProcessInfo : IProcessInfoBase
        {
            bool CanRun(IDictionary<string, object> data);

            IDictionary<string, object> TryGetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects);

            IDictionary<string, object> GetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects);

            IProcessParameters TryGetProcessParameters(IDictionary<string, object> data);

            IProcessParameters GetProcessParameters(IDictionary<string, object> data);
        }

        public static class ProcessFactoryProcessInfo
        {
            private static Exception GetParametersGenerationException(in bool process) => new InvalidOperationException($"An unknown error occurred during {(process ? "process " : null)}parameters generation.");

            public static Exception GetProcessParametersGenerationException() => GetParametersGenerationException(true);

            public static Exception GetParametersGenerationException() => GetParametersGenerationException(false);
        }

        public abstract class ProcessFactoryProcessInfoBase<T>
        {
            protected T Path { get; private set; }

            protected ProcessFactoryProcessInfoBase(in T path) => Path = path;

            protected virtual void Dispose() => Path = default;

            ~ProcessFactoryProcessInfoBase() => Dispose();
        }

        public abstract class ProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfoBase<T>, IProcessFactoryProcessInfo where T : IBrowsableObjectInfo
        {
            public abstract bool UserConfirmationRequired { get; }

            protected ProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

            public abstract string GetUserConfirmationText();

            protected abstract bool CanRun(EmptyCheckEnumerator<IBrowsableObjectInfo> enumerator);

            public virtual bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => CanRun(new EmptyCheckEnumerator<IBrowsableObjectInfo>((paths ?? throw GetArgumentNullException(nameof(paths))).GetEnumerator()));
        }

        public abstract class DirectProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IDirectProcessInfo where T : IBrowsableObjectInfo
        {
            protected DirectProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

            public virtual IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => TryGetProcessParameters(paths) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

            public abstract IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public abstract class RunnableProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IRunnableProcessInfo where T : IBrowsableObjectInfo
        {
            protected RunnableProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

            public abstract void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            public abstract bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            public virtual IProcessParameters GetProcessParameters(uint count) => TryGetProcessParameters(count) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

            public abstract IProcessParameters TryGetProcessParameters(uint count);
        }

        public interface IProcessFactory : DotNetFix.IDisposable
        {
            IProcessCommands NewItemProcessCommands { get; }

            IRunnableProcessInfo Copy { get; }

            IRunnableProcessInfo Cut { get; }

            IDirectProcessInfo Recycling { get; }

            IDirectProcessInfo Deletion { get; }

            IDirectProcessInfo Clearing { get; }

            IDragDropProcessInfo DragDrop { get; }

            bool CanPaste(uint count);

            IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters);

            IProcess TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters);
        }

        public static class ProcessFactory
        {
            public static IDirectProcessInfo DefaultProcessInfo { get; } = new _DefaultProcessFactory.DefaultDirectProcessInfo();

            public static IRunnableProcessInfo DefaultRunnableProcessFactoryProcessInfo { get; } = new _DefaultProcessFactory.DefaultRunnableProcessFactoryProcessInfo();

            public static IDragDropProcessInfo DefaultDragDropProcessInfo { get; } = new _DefaultProcessFactory.DragDropProcessInfo();

            private class _DefaultProcessFactory : IProcessFactory
            {
                internal class DefaultProcessInfoBase : IProcessInfoBase
                {
                    bool IProcessInfoBase.CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => false;
                }

                internal class DefaultProcessInfo : DefaultProcessInfoBase, IProcessFactoryProcessInfo
                {
                    bool IProcessFactoryProcessInfo.UserConfirmationRequired => false;

                    string IProcessFactoryProcessInfo.GetUserConfirmationText() => null;
                }

                internal class DefaultDirectProcessInfo : DefaultProcessInfo, IDirectProcessInfo
                {
                    IProcessParameters IDirectProcessInfo.GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => throw new NotSupportedException();

                    IProcessParameters IDirectProcessInfo.TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => null;
                }

                internal class DefaultRunnableProcessFactoryProcessInfo : DefaultProcessInfo, IRunnableProcessInfo
                {
                    IProcessParameters IRunnableProcessInfo.GetProcessParameters(uint count) => throw new NotSupportedException();

                    IProcessParameters IRunnableProcessInfo.TryGetProcessParameters(uint count) => null;

                    void IRunnableProcessInfo.Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => throw new NotSupportedException();

                    bool IRunnableProcessInfo.TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => false;
                }

                internal class DragDropProcessInfo : DefaultProcessInfoBase, IDragDropProcessInfo
                {
                    bool IDragDropProcessInfo.CanRun(IDictionary<string, object> data) => false;

                    IDictionary<string, object> IDragDropProcessInfo.GetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects) => throw new NotSupportedException();

                    IProcessParameters IDragDropProcessInfo.GetProcessParameters(IDictionary<string, object> data) => throw new NotSupportedException();

                    IDictionary<string, object> IDragDropProcessInfo.TryGetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects)
                    {
                        dragDropEffects = DragDropEffects.None;

                        return null;
                    }

                    IProcessParameters IDragDropProcessInfo.TryGetProcessParameters(IDictionary<string, object> data) => null;
                }

                bool DotNetFix.IDisposable.IsDisposed => false;

                IProcessCommands IProcessFactory.NewItemProcessCommands => null;

                IRunnableProcessInfo IProcessFactory.Copy => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

                IRunnableProcessInfo IProcessFactory.Cut => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

                IDirectProcessInfo IProcessFactory.Recycling => ProcessFactory.DefaultProcessInfo;

                IDirectProcessInfo IProcessFactory.Deletion => ProcessFactory.DefaultProcessInfo;

                IDirectProcessInfo IProcessFactory.Clearing => ProcessFactory.DefaultProcessInfo;

                IDragDropProcessInfo IProcessFactory.DragDrop => ProcessFactory.DefaultDragDropProcessInfo;

                bool IProcessFactory.CanPaste(uint count) => false;

                IProcess IProcessFactory.GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => throw new NotSupportedException();

                IProcess IProcessFactory.TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => null;

                void System.IDisposable.Dispose() { /* Left empty. */ }
            }

            public static IProcessFactory DefaultProcessFactory { get; } = new _DefaultProcessFactory();
        }

        public interface IProcessPathCollectionFactory
        {
            ProcessTypes<T>.IProcessQueue GetProcessCollection<T>() where T : IPathInfo;

            IProcessLinkedList<TItems, TError, TErrorItems, TAction> GetProcessLinkedList<TItems, TError, TErrorItems, TAction>() where TItems : IPathInfo where TErrorItems : IProcessErrorItem<TItems, TError, TAction>;
        }
    }

    public interface IBitmapSourceProvider : WinCopies.DotNetFix.IDisposable
    {
        IBitmapSources Default { get; }

        IBitmapSources Sources { get; }
    }

    public abstract class BitmapSourceProviderAbstract : IBitmapSourceProvider
    {
        protected abstract IBitmapSources DefaultOverride { get; }

        protected abstract IBitmapSources SourcesOverride { get; }

        public IBitmapSources Default => GetOrThrowIfDisposed(this, DefaultOverride);

        public IBitmapSources Sources => GetOrThrowIfDisposed(this, SourcesOverride);

        protected abstract bool DisposeBitmapSources { get; }

        public bool IsDisposed { get; private set; }

        protected virtual void DisposeManaged() => IsDisposed = true;

        protected virtual void DisposeUnmanaged()
        {
            if (DisposeBitmapSources)
            {
                DefaultOverride?.Dispose();

                SourcesOverride?.Dispose();
            }
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            DisposeManaged();

            DisposeUnmanaged();

            GC.SuppressFinalize(this);
        }

        ~BitmapSourceProviderAbstract() => DisposeUnmanaged();
    }

    public class BitmapSourceProvider : BitmapSourceProviderAbstract
    {
        private IBitmapSources _defaultOverride;
        private IBitmapSources _sourcesOverride;

        protected override IBitmapSources DefaultOverride => _defaultOverride;

        protected override IBitmapSources SourcesOverride => _sourcesOverride;

        protected override bool DisposeBitmapSources { get; }

        public BitmapSourceProvider(in IBitmapSources defaultBitmapSources, in IBitmapSources bitmapSources, in bool disposeBitmapSources)
        {
            _defaultOverride = defaultBitmapSources;

            _sourcesOverride = bitmapSources;

            DisposeBitmapSources = disposeBitmapSources;
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            _defaultOverride = null;
            _sourcesOverride = null;
        }
    }

    /// <summary>
    /// Represents a <see cref="BitmapSource"/>s provider for a GUI.
    /// </summary>
    public interface IBitmapSources : DotNetFix.IDisposable
    {
        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Small { get; }

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Medium { get; }

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Large { get; }

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource ExtraLarge { get; }
    }

    public abstract class BitmapSources : IBitmapSources
    {
        public bool IsDisposed { get; private set; }

        protected abstract BitmapSource SmallOverride { get; }

        protected abstract BitmapSource MediumOverride { get; }

        protected abstract BitmapSource LargeOverride { get; }

        protected abstract BitmapSource ExtraLargeOverride { get; }



        public BitmapSource Small => GetValueIfNotDisposed(() => SmallOverride);

        public BitmapSource Medium => GetValueIfNotDisposed(() => MediumOverride);

        public BitmapSource Large => GetValueIfNotDisposed(() => LargeOverride);

        public BitmapSource ExtraLarge => GetValueIfNotDisposed(() => ExtraLargeOverride);

        protected TParam GetValueIfNotDisposed<TParam>(in Func<TParam> func) => IsDisposed ? throw GetExceptionForDispose(false) : func();

        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~BitmapSources() => Dispose(false);
    }

    public abstract class BitmapSources<T> : BitmapSources
    {
        private T _innerObject;

        public T InnerObject => IsDisposed ? throw GetExceptionForDispose(false) : _innerObject;

        public BitmapSources(in T browsableObjectInfo) => _innerObject = browsableObjectInfo;

        protected override void Dispose(bool disposing)
        {
            _innerObject = default;

            base.Dispose(disposing);
        }
    }

    public interface IBitmapSourcesLinker : IBitmapSources
    {
        void LoadSmall();

        void LoadMedium();

        void LoadLarge();

        void LoadExtraLarge();

        void Load();

        void OnSmallLoaded();

        void OnMediumLoaded();

        void OnLargeLoaded();

        void OnExtraLargeLoaded();

        void OnBitmapSourcesLoaded();
    }

    public class BitmapSourcesLinker : BitmapSources<IBitmapSourceProvider>, IBitmapSourcesLinker, INotifyPropertyChanged
    {
        private const BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

        private BitmapSource _small;
        private BitmapSource _medium;
        private BitmapSource _large;
        private BitmapSource _extraLarge;

        protected override BitmapSource SmallOverride => _small;

        protected override BitmapSource MediumOverride => _medium;

        protected override BitmapSource LargeOverride => _large;

        protected override BitmapSource ExtraLargeOverride => _extraLarge;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSourcesLinker(in IBitmapSourceProvider bitmapSourceProvider) : base(bitmapSourceProvider)
        {
            UpdateBitmapSource(ref _small, bitmapSourceProvider.Default.Small);
            UpdateBitmapSource(ref _medium, bitmapSourceProvider.Default.Medium);
            UpdateBitmapSource(ref _large, bitmapSourceProvider.Default.Large);
            UpdateBitmapSource(ref _extraLarge, bitmapSourceProvider.Default.ExtraLarge);
        }

        protected void UpdateBitmapSource(ref BitmapSource value, in BitmapSource newValue) => (value = newValue).Freeze();

        protected virtual void RaisePropertyChangedEvent(in PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected virtual void RaisePropertyChangedEvent(in string propertyName) => RaisePropertyChangedEvent(new PropertyChangedEventArgs(propertyName));

        public virtual void OnSmallLoaded() => RaisePropertyChangedEvent(nameof(Small));

        public virtual void OnMediumLoaded() => RaisePropertyChangedEvent(nameof(Medium));

        public virtual void OnLargeLoaded() => RaisePropertyChangedEvent(nameof(Large));

        public virtual void OnExtraLargeLoaded() => RaisePropertyChangedEvent(nameof(ExtraLarge));

        public virtual void OnBitmapSourcesLoaded() => InvokeMethods(name => name != nameof(OnBitmapSourcesLoaded) && name.StartsWith("On") && name.EndsWith("Loaded"));

        public void LoadSmall() => UpdateBitmapSource(ref _small, InnerObject.Sources.Small);

        public void LoadMedium() => UpdateBitmapSource(ref _medium, InnerObject.Sources.Medium);

        public void LoadLarge() => UpdateBitmapSource(ref _large, InnerObject.Sources.Large);

        public void LoadExtraLarge() => UpdateBitmapSource(ref _extraLarge, InnerObject.Sources.ExtraLarge);

        public virtual void Load() => InvokeMethods(name => name.Length > nameof(Load).Length && name.StartsWith(nameof(Load)));

        private void InvokeMethods(Predicate<string> predicate)
        {
            System.Collections.Generic.IEnumerable<MethodInfo> methods = GetType().GetMethods(_flags).Where(m => predicate(m.Name) && m.GetParameters().Length == 0);

            foreach (MethodInfo method in methods)

                _ = method.Invoke(this, null);
        }
    }

    public class BrowsableObjectInfoBitmapBitmapSources : BitmapSources<Bitmap>
    {
        protected override BitmapSource SmallOverride => InnerObject.ToImageSource();

        protected override BitmapSource MediumOverride => InnerObject.ToImageSource();

        protected override BitmapSource LargeOverride => InnerObject.ToImageSource();

        protected override BitmapSource ExtraLargeOverride => InnerObject.ToImageSource();

        public BrowsableObjectInfoBitmapBitmapSources(in Bitmap bitmap) : base(bitmap) { /* Left empty. */ }
    }

    public enum BrowsableObjectInfoCallbackReason
    {
        Added,

        Updated,

        Remove
    }

    internal class BrowsableObjectInfoCallbackQueue : System.IDisposable
    {
        private Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>> _list = new
#if !CS9
            Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>
#endif
            ();

        public BrowsableObjectInfoCallback Enqueue(in Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> action)
        {
            Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>.LinkedListNode node = _list.AddLast(action);

            return new BrowsableObjectInfoCallback(() => _list.Remove(node));
        }

        public void RaiseCallbacks(IBrowsableObjectInfo browsableObjectInfo, BrowsableObjectInfoCallbackReason callbackReason)
        {
            foreach (Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> action in _list)

                action(browsableObjectInfo, callbackReason);
        }

        public void Dispose()
        {
            _list.Clear();

            _list = null;
        }
    }

    internal class BrowsableObjectInfoCallback : DotNetFix.IDisposable
    {
        private Action _action;

        public bool IsDisposed => _action == null;

        public BrowsableObjectInfoCallback(in Action action) => _action = action;

        public void Dispose()
        {
            if (_action != null)
            {
                _action();

                _action = null;

                GC.SuppressFinalize(this);
            }
        }

        ~BrowsableObjectInfoCallback() => Dispose();
    }
}
