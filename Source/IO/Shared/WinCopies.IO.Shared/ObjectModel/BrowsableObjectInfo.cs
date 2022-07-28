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

#region Usings
#region System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Linq;
using WinCopies.PropertySystem;
using WinCopies.Util;
using WinCopies.Util.Commands.Primitives;
#endregion WinCopies

using static WinCopies.ThrowHelper;
#endregion Usings

namespace WinCopies.IO
{
    public static class BrowsableObjectInfoHelper
    {
        public static bool IsBrowsable(this IBrowsableObjectInfo browsableObjectInfo) => BrowsableObjectInfo.IsBrowsableObject(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo)));

        public static IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetDefaultItems(this IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo.ItemSources?.Default.AsFromType<IEnumerable<IBrowsableObjectInfo>>();

        public static IEnumerator<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetDefaultItemsEnumerator(this IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo.GetDefaultItems()?.GetEnumerator();

        public static IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetDefaultItems<T>(this IBrowsableObjectInfo2<T> browsableObjectInfo, in Predicate<T> predicate) => browsableObjectInfo.ItemSources?.Default.GetItems(predicate);

        public static IEnumerator<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetDefaultItemsEnumerator<T>(this IBrowsableObjectInfo2<T> browsableObjectInfo, in Predicate<T> predicate) => browsableObjectInfo.GetDefaultItems(predicate)?.GetEnumerator();
    }

    public class BrowsableObjectInfoCallbackArgs
    {
        public string Path { get; }

        public IBrowsableObjectInfo BrowsableObjectInfo { get; }

        public BrowsableObjectInfoCallbackReason CallbackReason { get; }

        public BrowsableObjectInfoCallbackArgs(in string path, in IBrowsableObjectInfo browsableObjectInfo, in BrowsableObjectInfoCallbackReason callbackReason)
        {
            Path = path;

            BrowsableObjectInfo = browsableObjectInfo;

            CallbackReason = callbackReason;
        }
    }

    public class Dictionary<T> : IPropertySystemCollection<PropertyId, ShellPropertyGroup> where T : IReadOnlyDictionary<PropertyId, IProperty>, IIndexableR<IProperty>, IReadOnlyList<KeyValuePair<PropertyId, IProperty>>
    {
        protected T InnerDictionary { get; }

        public IEnumerable<PropertyId> Keys => InnerDictionary.Keys;

        public IEnumerable<IProperty> Values => InnerDictionary.Values;

        public int Count => InnerDictionary.Count;

        public bool SupportsReversedEnumeration => true;

        public IProperty this[PropertyId key] => InnerDictionary[key];

        public IProperty this[int index] => InnerDictionary.AsFromType<IIndexableR<IProperty>>()[index];

        public Dictionary(T innerDictionary) => InnerDictionary = innerDictionary;

        public bool ContainsKey(PropertyId key) => InnerDictionary.ContainsKey(key);
        public bool TryGetValue(PropertyId key,
#if CS8
            [MaybeNullWhen(false)]
#endif
            out IProperty value) => InnerDictionary.TryGetValue(key, out value);

        private IEnumeratorInfo<KeyValuePair<PropertyId, IProperty>> GetEnumeratorInfo(in IEnumerator<KeyValuePair<PropertyId, IProperty>> enumerator) => enumerator is IEnumeratorInfo<KeyValuePair<PropertyId, IProperty>> enumeratorInfo ? enumeratorInfo : new EnumeratorInfo<KeyValuePair<PropertyId, IProperty>>(enumerator);

        public IEnumeratorInfo<KeyValuePair<PropertyId, IProperty>> GetEnumerator() => GetEnumeratorInfo(InnerDictionary.GetEnumerator());
        public IEnumeratorInfo<KeyValuePair<PropertyId, IProperty>> GetReversedEnumerator() => GetEnumeratorInfo(InnerDictionary is Collections.Extensions.Generic.IEnumerable<KeyValuePair<PropertyId, IProperty>> dictionary ? dictionary.GetReversedEnumerator() : new Collections.DotNetFix.Generic.ArrayEnumerator<KeyValuePair<PropertyId, IProperty>>(InnerDictionary, true));
        IEnumerator<IProperty> IEnumerable<IProperty>.GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#if !CS8
        IEnumerator<KeyValuePair<PropertyId, IProperty>> Collections.Extensions.Generic.IEnumerable<KeyValuePair<PropertyId, IProperty>>.GetReversedEnumerator() => GetReversedEnumerator();
        IEnumerator<KeyValuePair<PropertyId, IProperty>> IEnumerable<KeyValuePair<PropertyId, IProperty>>.GetEnumerator() => GetEnumerator();
        IEnumerator Collections.Enumeration.IEnumerable.GetReversedEnumerator() => GetReversedEnumerator();
#endif
    }

    public class Dictionary : Dictionary<IReadOnlyIndexableDictionary<PropertyId, IProperty>>
    {
        public Dictionary(in IReadOnlyIndexableDictionary<PropertyId, IProperty> innerDictionary) : base(innerDictionary) { /* Left empty. */ }
    }

    namespace ObjectModel
    {
        /// <summary>
        /// The base class for all IO browsable objects of the WinCopies framework.
        /// </summary>
        [DebuggerDisplay("{Name}")]
        public abstract class BrowsableObjectInfo : BrowsableObjectInfoBase, IBrowsableObjectInfo
        {
            #region Consts
            public const ushort SmallIconSize = 16;
            public const ushort MediumIconSize = 48;
            public const ushort LargeIconSize = 128;
            public const ushort ExtraLargeIconSize = 256;

            public const int FileIcon = 0;
            public const int ComputerIcon = 15;
            public const int FolderIcon = 3;
            #endregion

            private BrowsableObjectInfoCallbackQueue
#if CS8
                ?
#endif
                _callbackQueue;
            private IBitmapSourcesLinker
#if CS8
                ?
#endif
                _bitmapSources;

            #region Properties
            IBitmapSources
#if CS8
                ?
#endif
                IBrowsableObjectInfo.BitmapSources => BitmapSources;

            IBrowsableObjectInfo Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>.Value => this;

            #region Static Properties
            public static ClientVersion DefaultClientVersion { get; } = new ClientVersion((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName());

            public static ISelectorDictionary<BrowsableObjectInfoURL3, IBrowsableObjectInfo> DefaultBrowsableObjectInfoSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<BrowsableObjectInfoURL3, IBrowsableObjectInfo>();

            public static ISelectorDictionary<ProcessFactorySelectorDictionaryParameters, IProcess> DefaultProcessSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<ProcessFactorySelectorDictionaryParameters, IProcess>();

            //public static Action RegisterDefaultSelectors { get; private set; } = () =>
            //{
            //    DotNetAssemblyInfo.RegisterSelectors();

            //    RegisterDefaultSelectors = EmptyVoid;
            //};
            #endregion

            #region Protected Properties
            /// <summary>
            /// When overridden in a derived class, gets the item sources for this <see cref="BrowsableObjectInfo"/>.
            /// </summary>
            protected abstract IItemSourcesProvider
#if CS8
                ?
#endif
                ItemSourcesOverride
            { get; }

            protected abstract IBrowsableObjectInfo
#if CS8
                ?
#endif
                ParentOverride
            { get; }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether this <see cref="BrowsableObjectInfo"/> can be monitored. If this value is <see langword="true"/>, a callback can be passed to the current item so that it will call it when changes are made on its context, i.e. when a new item is added or when properties have been updated.
            /// </summary>
            /// <seealso cref="IsMonitoring"/>
            /// <seealso cref="StartMonitoringOverride"/>
            /// <seealso cref="StopMonitoringOverride"/>
            protected virtual bool IsMonitoringSupportedOverride { get; }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether this <see cref="BrowsableObjectInfo"/> is the root of its context.
            /// </summary>
            protected abstract bool IsLocalRootOverride { get; }

            protected abstract IBitmapSourceProvider BitmapSourceProviderOverride { get; }

            protected abstract IBrowsabilityOptions BrowsabilityOverride { get; }

            protected abstract System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
            ?
#endif
            BrowsabilityPathsOverride
            { get; }

            protected abstract string
#if CS8
            ?
#endif
            DescriptionOverride
            { get; }

            /// <summary>
            /// When overridden in a derived class, gets the underlying object of this abstraction class.
            /// </summary>
            protected abstract object
#if CS8
                ?
#endif
                InnerObjectOverride
            { get; }

            protected abstract bool IsRecursivelyBrowsableOverride { get; }

            protected abstract bool IsSpecialItemOverride { get; }

            protected abstract string ItemTypeNameOverride { get; }

            protected abstract object
#if CS8
                ?
#endif
                ObjectPropertiesOverride
            { get; }

            protected abstract IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride
            { get; }

            protected virtual IBrowsableObjectInfoContextCommandEnumerable
#if CS8
                ?
#endif
                ContextCommandsOverride
            { get; }

            protected virtual IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
                ?
#endif
                ConnectionParametersOverride
            { get; }
            #endregion

            #region Public Properties
            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public IItemSourcesProvider
#if CS8
                ?
#endif
                ItemSources => GetValueIfNotDisposed(() => ItemSourcesOverride);

            public System.Collections.Generic.IReadOnlyDictionary<string, ConnectionParameter>
#if CS8
                ?
#endif
                ConnectionParameters => GetValueIfNotDisposed(() => ConnectionParametersOverride);

            /// <inheritdoc/>
            public bool IsMonitoring { get; private set; }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public bool IsMonitoringSupported => GetValueIfNotDisposed(() => IsMonitoringSupportedOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public bool IsLocalRoot => GetValueIfNotDisposed(() => IsLocalRootOverride);

            public IBitmapSourceProvider BitmapSourceProvider => GetValueIfNotDisposed(() => BitmapSourceProviderOverride);

            public IBitmapSourcesLinker
#if CS8
                ?
#endif
                BitmapSources => GetValueIfNotDisposed(() => _bitmapSources ?? (BitmapSourceProviderOverride == null ? null : (_bitmapSources = new BitmapSourcesLinker(BitmapSourceProviderOverride))));

            public IBrowsabilityOptions Browsability => GetValueIfNotDisposed(() => BrowsabilityOverride);

            public System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths => GetValueIfNotDisposed(() => BrowsabilityPathsOverride);

            public ClientVersion ClientVersion { get; }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public string
#if CS8
                ?
#endif
                Description => GetValueIfNotDisposed(() => DescriptionOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public object InnerObject => GetValueIfNotDisposed(() => InnerObjectOverride);

            public bool IsBrowsable => GetValueIfNotDisposed(() => IsBrowsableObject(this));

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public bool IsRecursivelyBrowsable => GetValueIfNotDisposed(() => IsRecursivelyBrowsableOverride);

            public bool IsSpecialItem => GetValueIfNotDisposed(() => IsSpecialItemOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public string ItemTypeName => GetValueIfNotDisposed(() => ItemTypeNameOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public object ObjectProperties => GetValueIfNotDisposed(() => ObjectPropertiesOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystem => GetValueIfNotDisposed(() => ObjectPropertySystemOverride);

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public IBrowsableObjectInfo Parent => GetValueIfNotDisposed(() => ParentOverride);

            public IBrowsableObjectInfoContextCommandEnumerable
#if CS8
                ?
#endif
                ContextCommands => GetValueIfNotDisposed(ContextCommandsOverride);

            /// <inheritdoc/>
            public virtual string
#if CS8
                ?
#endif
                Protocol => null;

            /// <inheritdoc/>
            public virtual string URI => Path;

            public virtual DisplayStyle DisplayStyle => DisplayStyle.Size3;
            #endregion
            #endregion

            // /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>
            /// <summary>
            /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
            /// </summary>
            /// <param name="path">The path of the new item.</param>
            protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path) => ClientVersion = clientVersion;

            #region Methods
            #region Static Methods
            #region Drawing
            #region Icons
            public static Icon
#if CS8
                ?
#endif
                TryGetIcon(in Icon[] icons, in System.Drawing.Size size) => icons?.TryGetIcon(size, 64, true, true);

            public static Icon
#if CS8
                ?
#endif
                TryGetIcon(in Icon icon, in System.Drawing.Size size)
            {
                Icon[]
#if CS8
                ?
#endif
                icons = icon?.Split();

                return icons == null ? null : TryGetIcon(icons, size);
            }

            public static Icon
#if CS8
                ?
#endif
                TryGetIcon(in Icon[] icons, in ushort size) => TryGetIcon(icons, new System.Drawing.Size(size, size));

            public static Icon
#if CS8
                ?
#endif
                TryGetIcon(in Icon icon, in ushort size) => TryGetIcon(icon, new System.Drawing.Size(size, size));
            #endregion

            #region BitmapSources
            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in Icon
#if CS8
                ?
#endif
                icon) => icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in Icon[] icons, in System.Drawing.Size size) => TryGetBitmapSource(TryGetIcon(icons, size));

            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in Icon icon, in System.Drawing.Size size) => TryGetBitmapSource(TryGetIcon(icon, size));

            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in Icon[] icons, in ushort size) => TryGetBitmapSource(TryGetIcon(icons, size));

            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in Icon icon, in ushort size) => TryGetBitmapSource(TryGetIcon(icon, size));
            #endregion
            #endregion

            public static void PromptPathNotFound(in string path) => MessageBox.Show($"Could not find the path:\n{path}\nor the given path is not handled by any installed plugin. It may does not exist or the protocol may be wrong.", "Path not found", MessageBoxButton.OK, MessageBoxImage.Error);

            public static IBrowsableObjectInfo
#if CS8
                ?
#endif
                Create(in string text, in bool prompt, in bool rethrow)
            {
                IBrowsableObjectInfo
#if CS8
                    ?
#endif
                    result = null;

                try
                {
                    result = DefaultBrowsableObjectInfoSelectorDictionary.Select(new BrowsableObjectInfoURL3(new BrowsableObjectInfoURL2(text), GetDefaultClientVersion()));
                }

                catch
                {
                    if (rethrow)

                        throw;
                }

                finally
                {
                    if (result == null && prompt)

                        PromptPathNotFound(text);
                }

                return result;
            }

            internal static bool IsBrowsableObject(in IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo.Browsability != null && (browsableObjectInfo.Browsability.Browsability == IO.Browsability.BrowsableByDefault || browsableObjectInfo.Browsability.Browsability == IO.Browsability.Browsable);

            public static ClientVersion GetDefaultClientVersion()
            {
                AssemblyName assemblyName = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName();

                Version
#if CS8
                ?
#endif
                assemblyVersion = assemblyName.Version;

                // todo: use the new constructor

                return new ClientVersion(assemblyName.Name, (uint)assemblyVersion.Major, (uint)assemblyVersion.Minor, (uint)assemblyVersion.Revision);
            }

            public static bool Predicate(in BrowsableObjectInfoURL3 item, in Type t) => WinCopies.Extensions.UtilHelpers.ContainsFieldValue(t, null, item.URL.Protocol);

            public static bool Predicate(in ProcessFactorySelectorDictionaryParameters item, in Type t) => WinCopies.Extensions.UtilHelpers.ContainsFieldValue(t, null, item.ProcessParameters.Guid.ToString());
            #endregion

            #region Protected Methods
            protected virtual IContextMenu
#if CS8
                ?
#endif
                GetContextMenuOverride(bool extendedVerbs) => null;

            protected virtual IContextMenu
#if CS8
                ?
#endif
                GetContextMenuOverride(IEnumerable<IBrowsableObjectInfo> children, bool extendedVerbs) => null;

            protected virtual void RaiseCallbacksOverride(BrowsableObjectInfoCallbackArgs a) => _callbackQueue?.RaiseCallbacks(a);

            protected void RaiseCallbacks(in BrowsableObjectInfoCallbackArgs a)
            {
                ThrowIfDisposed(this);

                RaiseCallbacksOverride(a);
            }

#if CS8
            [return: NotNullIfNotNull("value")]
#endif
            protected T
#if CS9
                ?
#endif
                GetValueIfNotDisposed<T>(in T
#if CS9
                ?
#endif
                value) => GetOrThrowIfDisposed(this, value);

            protected T
#if CS9
                ?
#endif
                GetValueIfNotDisposed<T>(in Func<T
#if CS9
                ?
#endif
                > func) => GetOrThrowIfDisposed(this, func);

            protected abstract ArrayBuilder<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetRootItemsOverride();

            protected abstract System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride();

            protected virtual void StartMonitoringOverride() => throw new NotSupportedException();

            protected virtual void StopMonitoringOverride() => throw new NotSupportedException();

            protected virtual System.Collections.Generic.IEnumerable<ICommand>
#if CS8
                ?
#endif
                GetCommandsOverride(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                items = null) => null;
            #endregion

            #region Public Methods
            public static Icon
#if CS8
                ?
#endif
                TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => TryGetIcon(new IconExtractor(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), dll)).GetIcon(iconIndex), size);

            public static BitmapSource
#if CS8
                ?
#endif
                TryGetBitmapSource(in int iconIndex, in string dllName, in int size)
            {
                using
#if !CS8
            (
#endif
                Icon
#if CS8
                ?
#endif
                icon = TryGetIcon(iconIndex, dllName, new System.Drawing.Size(size, size))
#if CS8
            ;
#else
            )
#endif
                return TryGetBitmapSource(icon);
            }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public IContextMenu
#if CS8
                ?
#endif
                GetContextMenu(bool extendedVerbs) => GetValueIfNotDisposed(() => GetContextMenuOverride(extendedVerbs));

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public IContextMenu
#if CS8
                ?
#endif
                GetContextMenu(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> children, bool extendedVerbs) => GetValueIfNotDisposed(() => GetContextMenuOverride(children, extendedVerbs));

            public System.Collections.Generic.IEnumerable<ICommand>
#if CS8
                ?
#endif
                GetCommands(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                items) => GetValueIfNotDisposed(() => GetCommandsOverride(items));

            public IBrowsableObjectInfoCallback RegisterCallback(Action<BrowsableObjectInfoCallbackArgs> action)
            {
                ThrowIfDisposed(this);

                ThrowIfNull(action, nameof(action));

                return (_callbackQueue
#if CS8
                ??=
#else
                ?? (_callbackQueue =
#endif
                new BrowsableObjectInfoCallbackQueue()
#if !CS8
                )
#endif
                ).Enqueue(action);
            }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public void StartMonitoring()
            {
                if (IsMonitoringSupported && !IsMonitoring)
                {
                    IsMonitoring = true;

                    StartMonitoringOverride();
                }
            }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">The current <see cref="BrowsableObjectInfo"/> is disposed.</exception>
            public void StopMonitoring()
            {
                if (GetValueIfNotDisposed(IsMonitoring))
                {
                    StopMonitoringOverride();

                    IsMonitoring = false;
                }
            }
            #endregion

            #region Interface Implementations
            #region Enumeration
            protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetDefaultEnumerable() => BrowsableObjectInfoHelper.GetDefaultItems(this);
            protected System.Collections.Generic.IEnumerator<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetDefaultEnumerator() => BrowsableObjectInfoHelper.GetDefaultItemsEnumerator(this);

            public ArrayBuilder<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetRootItems() => GetValueIfNotDisposed(GetRootItemsOverride);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetSubRootItems() => GetValueIfNotDisposed(GetSubRootItemsOverride);

            System.Collections.Generic.IEnumerator<Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>>
#if CS8
                ?
#endif
                IRecursiveEnumerableProviderEnumerable<IBrowsableObjectInfo>.GetRecursiveEnumerator() => GetDefaultEnumerator();

            System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => GetDefaultEnumerator();

#if !(WinCopies3 && CS8)
            IEnumerator IEnumerable.GetEnumerator() => GetDefaultEnumerator();
#endif

            RecursiveEnumeratorAbstract<IBrowsableObjectInfo> IRecursiveEnumerable<IBrowsableObjectInfo>.GetEnumerator() => IsRecursivelyBrowsable ? new RecursiveEnumerator<IBrowsableObjectInfo>(this, RecursiveEnumerationOrder.ParentThenChildren) : throw new NotSupportedException("The current BrowsableObjectInfo does not support recursive browsing.");
            #endregion

            #region IDisposable
            /// <summary>
            /// Gets a value that indicates whether the current object is disposed.
            /// </summary>
            public bool IsDisposed { get; internal set; }

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                DisposeManaged();

                DisposeUnmanaged();

                GC.SuppressFinalize(this);
            }

            protected virtual void DisposeManaged() => IsDisposed = true;

            protected virtual void DisposeUnmanaged()
            {
                if (IsMonitoring)

                    StopMonitoringOverride();

                _bitmapSources = null;
            }

            //if (ItemsLoader != null)

            //{

            //    if (ItemsLoader.IsBusy)

            //        ItemsLoader.Cancel();

            //    // ItemsLoader.Path = null;

            //}

            //if (disposing)

            //    _parent = null;
            #endregion
            #endregion Interface Implementations
            #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            ~BrowsableObjectInfo() => DisposeUnmanaged();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        public abstract class BrowsableObjectInfo2 : BrowsableObjectInfo
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            protected BrowsableObjectInfo2(in string path, in ClientVersion clientVersion, in IItemSourcesProvider itemSources) : base(path, clientVersion) => ItemSourcesOverride = itemSources;
        }

        public abstract class BrowsableObjectInfo5<T> : BrowsableObjectInfo, IBrowsableObjectInfo3<T> where T : IBrowsableObjectInfo
        {
            #region Properties
            protected abstract T
#if CS9
                ?
#endif
                ParentGenericOverride
            { get; }

            protected sealed override IBrowsableObjectInfo
#if CS8
                ?
#endif
                ParentOverride => ParentGenericOverride;

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="BrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            public T
#if CS9
                ?
#endif
                ParentGeneric => GetValueIfNotDisposed(() => ParentGenericOverride);

            T
#if CS9
                ?
#endif
                IBrowsableObjectInfo3<T>.Parent => ParentGeneric;
            #endregion

            #region Constructors
            /*/// <summary>
            /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
            /// </summary>
            /// <param name="path">The path of the new item.</param>
            /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>*/
            protected BrowsableObjectInfo5(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
            #endregion
        }

        public abstract class BrowsableObjectInfo<TParent, TObjectProperties> : BrowsableObjectInfo5<TParent>, IBrowsableObjectInfo<TParent, TObjectProperties> where TParent : IBrowsableObjectInfo
        {
            #region Properties
            protected abstract TObjectProperties
#if CS9
                ?
#endif
                ObjectPropertiesGenericOverride
            { get; }

            public TObjectProperties
#if CS9
                ?
#endif
                ObjectPropertiesGeneric => ObjectPropertiesGenericOverride;

            protected sealed override object
#if CS8
                ?
#endif
                ObjectPropertiesOverride => ObjectPropertiesGenericOverride;

            TObjectProperties IBrowsableObjectInfo<TObjectProperties>.ObjectProperties => ObjectPropertiesGeneric;
            #endregion

            #region Constructors
            /*/// <summary>
            /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
            /// </summary>
            /// <param name="path">The path of the new item.</param>
            /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>*/
            protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
            #endregion
        }

        public abstract class BrowsableObjectInfo2<T> : BrowsableObjectInfo, IEncapsulatorBrowsableObjectInfo<T>
        {
            #region Properties
            protected abstract T InnerObjectGenericOverride { get; }

            public T InnerObjectGeneric => GetValueIfNotDisposed(() => InnerObjectGenericOverride);

            T IEncapsulatorBrowsableObjectInfo<T>.InnerObject => InnerObjectGenericOverride;

            protected sealed override object InnerObjectOverride => InnerObjectGenericOverride;
            #endregion

            protected BrowsableObjectInfo2(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
        }

        public abstract class BrowsableObjectInfo3<T> : BrowsableObjectInfo2<T>
        {
            protected override T InnerObjectGenericOverride { get; }

            protected BrowsableObjectInfo3(in T innerObject, in string path, in ClientVersion clientVersion) : base(path, clientVersion) => InnerObjectGenericOverride = innerObject
#if CS8
                ??
#else
                == null ?
#endif
                throw GetArgumentNullException(nameof(innerObject))
#if !CS8
                : innerObject
#endif
                ;
        }

        public abstract class BrowsableObjectInfo4<T> : BrowsableObjectInfo3<T>
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            protected BrowsableObjectInfo4(in T innerObject, in string path, in ClientVersion clientVersion, in Func<IEnumerable<IBrowsableObjectInfo>> func) : base(innerObject, path, clientVersion) => ItemSourcesOverride = new ItemSourcesProvider(func);
        }

        public abstract class BrowsableObjectInfo<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TParent, TObjectProperties>, IBrowsableObjectInfo<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TParent : IBrowsableObjectInfo where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            #region Properties
            protected abstract TInnerObject
#if CS9
                ?
#endif
                InnerObjectGenericOverride
            { get; }

            public TInnerObject InnerObjectGeneric => GetValueIfNotDisposed(() => InnerObjectGenericOverride);

            TInnerObject IEncapsulatorBrowsableObjectInfo<TInnerObject>.InnerObject => InnerObjectGenericOverride;

            protected sealed override object InnerObjectOverride => InnerObjectGenericOverride;

            protected abstract IItemSourcesProvider<TPredicateTypeParameter>
#if CS8
                ?
#endif
                ItemSourcesGenericOverride
            { get; }

            protected sealed override IItemSourcesProvider
#if CS8
                ?
#endif
                ItemSourcesOverride => ItemSourcesGenericOverride;

            public IItemSourcesProvider<TPredicateTypeParameter>
#if CS8
                ?
#endif
                ItemSourcesGeneric => GetValueIfNotDisposed(() => ItemSourcesGenericOverride);

            IItemSourcesProvider<TPredicateTypeParameter>
#if CS8
                ?
#endif
                IBrowsableObjectInfo2<TPredicateTypeParameter>.ItemSources => ItemSourcesGeneric;

            // public abstract Predicate<TPredicateTypeParameter> RootItemsPredicate { get; }
            #endregion

            /// <summary>
            /// When called from a derived class, initializes a new instance of the <see cref="BrowsableObjectInfo{TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> class with a custom <see cref="ClientVersion"/>.
            /// </summary>
            /// <param name="path">The path of the new <see cref="BrowsableObjectInfo{TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.</param>
            /// <param name="clientVersion">A custom <see cref="ClientVersion"/>. This parameter can be null for non-file system and portable devices-related types.</param>
            protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }

            #region Methods
            protected abstract TSelectorDictionary GetSelectorDictionaryOverride();

            public TSelectorDictionary GetSelectorDictionary() => GetValueIfNotDisposed(GetSelectorDictionaryOverride);
            #endregion
        }

        public abstract class BrowsableObjectInfo2<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TParent : IBrowsableObjectInfo where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            protected override TInnerObject InnerObjectGenericOverride { get; }

            protected BrowsableObjectInfo2(in TInnerObject innerObject, in string path, in ClientVersion clientVersion) : base(path, clientVersion) => InnerObjectGenericOverride = innerObject;
        }

        public abstract class BrowsableObjectInfo3<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo2<TParent, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TParent : IBrowsableObjectInfo where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            protected override IItemSourcesProvider<TPredicateTypeParameter> ItemSourcesGenericOverride { get; }

            protected BrowsableObjectInfo3(in TInnerObject innerObject, in string path, in ClientVersion clientVersion, in Converter<Predicate<TPredicateTypeParameter>, IEnumerable<TDictionaryItems>> converter) : base(innerObject, path, clientVersion) => ItemSourcesGenericOverride = new ItemSourcesProvider<TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>(this, converter);
        }

        public abstract class ProtocolInfo : BrowsableObjectInfo
        {
            public override string LocalizedName => Name;

            public override string Name { get; }

            protected override bool IsLocalRootOverride => false;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => null;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

            protected override string
#if CS8
                ?
#endif
                DescriptionOverride => null;

            protected override object
#if CS8
                ?
#endif
                InnerObjectOverride => null;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Protocol";

            protected override object
#if CS8
                ?
#endif
                ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

            protected override IBrowsableObjectInfo ParentOverride { get; }

            public override string Protocol => "protocol";

            public ProtocolInfo(string protocol, in IBrowsableObjectInfo parent, in ClientVersion clientVersion) : base($"{parent.Path}{System.IO.Path.DirectorySeparatorChar}{UtilHelpers.Update(ref protocol, null)}", clientVersion)
            {
                Name = protocol;

                ParentOverride = parent;
            }

            protected override ArrayBuilder<IBrowsableObjectInfo>
#if CS9
                ?
#endif
                GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS9
                ?
#endif
                GetSubRootItemsOverride() => null;
        }

        public abstract class AppBrowsableObjectInfo<T> : BrowsableObjectInfo3<T>
        {
            public sealed override string Protocol => "about";

            public override string URI => Path;

            protected AppBrowsableObjectInfo(in T innerObject, in string path, in ClientVersion clientVersion) : base(innerObject, path, clientVersion) { /* Left empty. */ }
        }

        public abstract class PluginInfo<T> : BrowsableObjectInfo3<T>, IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin> where T : IBrowsableObjectInfoPlugin
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            public override string LocalizedName => GetName(InnerObjectGenericOverride.GetType().Assembly);

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => false;

            protected override IBitmapSourceProvider BitmapSourceProviderOverride => InnerObjectGenericOverride.BitmapSourceProvider ?? new BitmapSourceProvider(new Shell.ComponentSources.Bitmap.BitmapSources(new Shell.ComponentSources.Bitmap.BitmapSourcesStruct(0, "cabview")), null, null, true);

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override IEnumerable<IBrowsabilityPath>
#if CS8
                ?
#endif
                BrowsabilityPathsOverride => null;

            protected override string
#if CS8
            ?
#endif
            DescriptionOverride => InnerObjectGeneric.GetType().Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description;

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Plug-in Start Page";

            protected override object
#if CS8
                ?
#endif
                ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

            public override string Protocol => "plugin";

            public override string URI { get; }

            IBrowsableObjectInfoPlugin IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>.InnerObject => InnerObjectGeneric;

            private PluginInfo(in T plugin, in ClientVersion clientVersion, Assembly assembly) : base(plugin, GetName(assembly), clientVersion)
            {
                ItemSourcesOverride = new ItemSourcesProvider(() => InnerObjectGenericOverride.GetStartPages(ClientVersion).AppendValues(InnerObjectGenericOverride.GetProtocols(this, ClientVersion)));

                URI = GetURI(assembly);
            }

            protected PluginInfo(in T plugin, in ClientVersion clientVersion) : this(plugin, clientVersion, (plugin
#if CS8
                ??
#else
                == null ?
#endif
                throw GetArgumentNullException(nameof(plugin))
#if !CS8
                : plugin
#endif
                ).GetType().Assembly)
            { /* Left empty. */ }

            public static string GetName(in Assembly assembly)
            {
                AssemblyName name = (assembly ?? throw GetArgumentNullException(nameof(assembly))).GetName();

                return name.Name ?? name.FullName;
            }

            public static string GetURI(in Assembly assembly) => assembly.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value ?? GetName(assembly);

            protected override ArrayBuilder<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetRootItemsOverride() => null;

            protected override IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetSubRootItemsOverride() => null;
        }

        public abstract class BrowsableObjectInfoStartPage<T> : AppBrowsableObjectInfo<IEnumerable<T>> where T : IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>
        {
            protected override IItemSourcesProvider ItemSourcesOverride { get; }

            public override string LocalizedName => "Start Here";

            public override string Name => LocalizedName;

            protected override bool IsLocalRootOverride => true;

            protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

            protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
                ?
#endif
                BrowsabilityPathsOverride => null;

            protected override string DescriptionOverride => "This is the start page of the Explorer window. Here you can find all the root browsable items from the plug-ins you have installed.";

            protected override bool IsRecursivelyBrowsableOverride => true;

            protected override bool IsSpecialItemOverride => false;

            protected override string ItemTypeNameOverride => "Start Page";

            protected override object
#if CS8
                ?
#endif
                ObjectPropertiesOverride => null;

            protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

            protected override IBrowsableObjectInfo
#if CS8
                ?
#endif
                ParentOverride => null;

            protected BrowsableObjectInfoStartPage(in System.Collections.Generic.IEnumerable<T> plugins, in ClientVersion clientVersion) : base(plugins, "start", clientVersion) => ItemSourcesOverride = new ItemSourcesProvider(InnerObjectGenericOverride.Cast<IBrowsableObjectInfo>);

            protected override ArrayBuilder<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetRootItemsOverride() => null;

            protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetSubRootItemsOverride() => null;
        }
    }
}
