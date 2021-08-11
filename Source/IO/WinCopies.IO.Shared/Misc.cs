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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Desktop;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process.ObjectModel;

using static WinCopies.ThrowHelper;
using System.IO;

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

        public interface IProcessFactoryProcessInfo
        {
            bool UserConfirmationRequired { get; }

            string GetUserConfirmationText();

            bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public interface IDirectProcessFactoryProcessInfo : IProcessFactoryProcessInfo
        {
            IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

            IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public interface IRunnableProcessFactoryProcessInfo : IProcessFactoryProcessInfo
        {
            bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

            IProcessParameters GetProcessParameters(uint count);

            IProcessParameters TryGetProcessParameters(uint count);
        }

        public static class ProcessFactoryProcessInfo
        {
            public static Exception GetProcessParametersGenerationException() => new InvalidOperationException("An unknown error occurred during process parameters generation.");
        }

        public abstract class ProcessFactoryProcessInfo<T> : IProcessFactoryProcessInfo where T : IBrowsableObjectInfo
        {
            protected T Path { get; private set; }

            protected ProcessFactoryProcessInfo(in T path) => Path = path;

            public abstract bool UserConfirmationRequired { get; }

            public abstract string GetUserConfirmationText();

            protected abstract bool CanRun(System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> enumerator);

            public virtual bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths)
            {
                var enumerator = new EmptyCheckEnumerator<IBrowsableObjectInfo>((paths ?? throw GetArgumentNullException(nameof(paths))).GetEnumerator());

                return enumerator.HasItems && CanRun(enumerator);
            }

            protected virtual void Dispose() => Path = default;

            ~ProcessFactoryProcessInfo() => Dispose();
        }

        public abstract class DirectProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IDirectProcessFactoryProcessInfo where T : IBrowsableObjectInfo
        {
            protected DirectProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

            public virtual IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => TryGetProcessParameters(paths) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

            public abstract IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
        }

        public abstract class RunnableProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IRunnableProcessFactoryProcessInfo where T : IBrowsableObjectInfo
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

            IRunnableProcessFactoryProcessInfo Copy { get; }

            IRunnableProcessFactoryProcessInfo Cut { get; }

            IDirectProcessFactoryProcessInfo Recycling { get; }

            IDirectProcessFactoryProcessInfo Deletion { get; }

            bool CanPaste(uint count);

            IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters);

            IProcess TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters);
        }

        public static class ProcessFactory
        {
            public static IDirectProcessFactoryProcessInfo DefaultProcessInfo { get; } = new _DefaultProcessFactory.DefaultDirectProcessInfo();

            public static IRunnableProcessFactoryProcessInfo DefaultRunnableProcessFactoryProcessInfo { get; } = new _DefaultProcessFactory.DefaultRunnableProcessFactoryProcessInfo();

            private class _DefaultProcessFactory : IProcessFactory
            {
                internal class DefaultProcessInfo : IProcessFactoryProcessInfo
                {
                    bool IProcessFactoryProcessInfo.UserConfirmationRequired => false;

                    string IProcessFactoryProcessInfo.GetUserConfirmationText() => null;

                    bool IProcessFactoryProcessInfo.CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => false;
                }

                internal class DefaultDirectProcessInfo : DefaultProcessInfo, IDirectProcessFactoryProcessInfo
                {
                    IProcessParameters IDirectProcessFactoryProcessInfo.GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => throw new NotSupportedException();

                    IProcessParameters IDirectProcessFactoryProcessInfo.TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => null;
                }

                internal class DefaultRunnableProcessFactoryProcessInfo : DefaultProcessInfo, IRunnableProcessFactoryProcessInfo
                {
                    IProcessParameters IRunnableProcessFactoryProcessInfo.GetProcessParameters(uint count) => throw new NotSupportedException();

                    IProcessParameters IRunnableProcessFactoryProcessInfo.TryGetProcessParameters(uint count) => null;

                    void IRunnableProcessFactoryProcessInfo.Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => throw new NotSupportedException();

                    bool IRunnableProcessFactoryProcessInfo.TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => false;
                }

                bool DotNetFix.IDisposable.IsDisposed => false;

                IProcessCommands IProcessFactory.NewItemProcessCommands => null;

                IRunnableProcessFactoryProcessInfo IProcessFactory.Copy => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

                IRunnableProcessFactoryProcessInfo IProcessFactory.Cut => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

                IDirectProcessFactoryProcessInfo IProcessFactory.Recycling => ProcessFactory.DefaultProcessInfo;

                IDirectProcessFactoryProcessInfo IProcessFactory.Deletion => ProcessFactory.DefaultProcessInfo;

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

    /// <summary>
    /// Represents a <see cref="BitmapSource"/>s provider for a GUI.
    /// </summary>
    public interface IBrowsableObjectInfoBitmapSources : DotNetFix.IDisposable
    {
        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource SmallBitmapSource { get; }

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource MediumBitmapSource { get; }

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource LargeBitmapSource { get; }

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource ExtraLargeBitmapSource { get; }
    }

    public abstract class BrowsableObjectInfoBitmapSources<T> : IBrowsableObjectInfoBitmapSources
    {
        private T _innerObject;

        public T InnerObject => IsDisposed ? throw GetExceptionForDispose(false) : _innerObject;

        public bool IsDisposed { get; private set; }

        protected abstract BitmapSource SmallBitmapSourceOverride { get; }

        protected abstract BitmapSource MediumBitmapSourceOverride { get; }

        protected abstract BitmapSource LargeBitmapSourceOverride { get; }

        protected abstract BitmapSource ExtraLargeBitmapSourceOverride { get; }



        public BitmapSource SmallBitmapSource => GetValueIfNotDisposed(() => SmallBitmapSourceOverride);

        public BitmapSource MediumBitmapSource => GetValueIfNotDisposed(() => MediumBitmapSourceOverride);

        public BitmapSource LargeBitmapSource => GetValueIfNotDisposed(() => LargeBitmapSourceOverride);

        public BitmapSource ExtraLargeBitmapSource => GetValueIfNotDisposed(() => ExtraLargeBitmapSourceOverride);

        public BrowsableObjectInfoBitmapSources(in T browsableObjectInfo) => _innerObject = browsableObjectInfo;

        protected T GetValueIfNotDisposed<T>(in Func<T> func) => IsDisposed ? throw GetExceptionForDispose(false) : func();

        protected virtual void Dispose(bool disposing)
        {
            _innerObject = default;

            IsDisposed = true;
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~BrowsableObjectInfoBitmapSources() => Dispose(false);
    }

    public class BrowsableObjectInfoBitmapBitmapSources : BrowsableObjectInfoBitmapSources<Bitmap>
    {
        protected override BitmapSource SmallBitmapSourceOverride => InnerObject.ToImageSource();

        protected override BitmapSource MediumBitmapSourceOverride => InnerObject.ToImageSource();

        protected override BitmapSource LargeBitmapSourceOverride => InnerObject.ToImageSource();

        protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.ToImageSource();

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
        private Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>> _list = new Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>();

        public BrowsableObjectInfoCallback Enqueue(in Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> action)
        {
            Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>.LinkedListNode node = (Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>.LinkedListNode)_list.AddLast(action);

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
