﻿/* Copyright © Pierre Sprimont, 2020
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
using System.Collections;
using System.Reflection;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.PropertySystem;
using WinCopies.Util.Commands.Primitives;

using static WinCopies.ThrowHelper;
using static WinCopies.Collections.Util;

namespace WinCopies.IO
{
    public static class BrowsableObjectInfoHelper
    {
        public static bool IsBrowsable(this IBrowsableObjectInfo browsableObjectInfo) => BrowsableObjectInfo._IsBrowsable(browsableObjectInfo ?? throw GetArgumentNullException(nameof(browsableObjectInfo)));
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

    namespace ObjectModel
    {
        /// <summary>
        /// The base class for all IO browsable objects of the WinCopies framework.
        /// </summary>
        public abstract class BrowsableObjectInfo : BrowsableObjectInfoBase, IBrowsableObjectInfo
        {
            protected static void EmptyVoid() { /* Left empty. */ }

            #region Consts
            public const ushort SmallIconSize = 16;
            public const ushort MediumIconSize = 48;
            public const ushort LargeIconSize = 128;
            public const ushort ExtraLargeIconSize = 256;

            public const int FileIcon = 0;
            public const int ComputerIcon = 15;
            public const int FolderIcon = 3;
            #endregion

            private BrowsableObjectInfoCallbackQueue _callbackQueue;
            private IBitmapSourcesLinker _bitmapSources;

            #region Properties
            IBitmapSources IBrowsableObjectInfo.BitmapSources => BitmapSources;

            #region Static Properties
            public static ISelectorDictionary<ProcessFactorySelectorDictionaryParameters, IProcess> DefaultProcessSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<ProcessFactorySelectorDictionaryParameters, IProcess>();

            //public static Action RegisterDefaultSelectors { get; private set; } = () =>
            //{
            //    DotNetAssemblyInfo.RegisterSelectors();

            //    RegisterDefaultSelectors = EmptyVoid;
            //};
            #endregion

            #region Protected Properties
            protected virtual bool IsMonitoringSupportedOverride { get; } = false;

            protected abstract bool IsLocalRootOverride { get; }

            protected abstract IBitmapSourceProvider BitmapSourceProviderOverride { get; }

            protected abstract IBrowsabilityOptions BrowsabilityOverride { get; }

            protected abstract System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride { get; }

            protected abstract System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride { get; }

            protected abstract string DescriptionOverride { get; }

            protected abstract object InnerObjectOverride { get; }

            protected abstract bool IsRecursivelyBrowsableOverride { get; }

            protected abstract bool IsSpecialItemOverride { get; }

            protected abstract string ItemTypeNameOverride { get; }

            protected abstract object ObjectPropertiesOverride { get; }

            protected abstract IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride { get; }

            protected abstract IBrowsableObjectInfo ParentOverride { get; }

            protected abstract IProcessFactory ProcessFactoryOverride { get; }
            #endregion

            #region Public Properties
            public bool IsMonitoring { get; private set; }

            public bool IsMonitoringSupported => GetValueIfNotDisposed(() => IsMonitoringSupportedOverride);

            public bool IsLocalRoot => GetValueIfNotDisposed(() => IsLocalRootOverride);

            public IBitmapSourceProvider BitmapSourceProvider => GetValueIfNotDisposed(() => BitmapSourceProviderOverride);

            public IBitmapSourcesLinker BitmapSources => GetValueIfNotDisposed(() => _bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BitmapSourcesLinker(BitmapSourceProviderOverride)
#if !CS8
            )
#endif
            );

            public IBrowsabilityOptions Browsability => GetValueIfNotDisposed(() => BrowsabilityOverride);

            public System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPaths => GetValueIfNotDisposed(() => BrowsabilityPathsOverride);

            public ClientVersion ClientVersion { get; }

            public System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcesses => GetValueIfNotDisposed(() => CustomProcessesOverride);

            public string Description => GetValueIfNotDisposed(() => DescriptionOverride);

            public object InnerObject => GetValueIfNotDisposed(() => InnerObjectOverride);

            public bool IsBrowsable => GetValueIfNotDisposed(() => _IsBrowsable(this));

            public bool IsRecursivelyBrowsable => GetValueIfNotDisposed(() => IsRecursivelyBrowsableOverride);

            public bool IsSpecialItem => GetValueIfNotDisposed(() => IsSpecialItemOverride);

            public string ItemTypeName => GetValueIfNotDisposed(() => ItemTypeNameOverride);

            public object ObjectProperties => GetValueIfNotDisposed(() => ObjectPropertiesOverride);

            public IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => GetValueIfNotDisposed(() => ObjectPropertySystemOverride);

            /// <summary>
            /// Gets the <see cref="IBrowsableObjectInfo"/> parent of this <see cref="BrowsableObjectInfo"/>. Returns <see langword="null"/> if this object is the root object of a hierarchy.
            /// </summary>
            public IBrowsableObjectInfo Parent => GetValueIfNotDisposed(() => ParentOverride);

            public IProcessFactory ProcessFactory => GetValueIfNotDisposed(() => ProcessFactoryOverride);

            IBrowsableObjectInfo Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>.Value => this;
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
            internal static bool _IsBrowsable(in IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo.Browsability != null && (browsableObjectInfo.Browsability.Browsability == IO.Browsability.BrowsableByDefault || browsableObjectInfo.Browsability.Browsability == IO.Browsability.Browsable);

            public static ClientVersion GetDefaultClientVersion()
            {
                AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

                Version assemblyVersion = assemblyName.Version;

                // todo: use the new constructor

                return new ClientVersion(assemblyName.Name, (uint)assemblyVersion.Major, (uint)assemblyVersion.Minor, (uint)assemblyVersion.Revision);
            }

            public static bool Predicate(in ProcessFactorySelectorDictionaryParameters item, in Type t) => WinCopies.Extensions.UtilHelpers.ContainsFieldValue(t, null, item.ProcessParameters.Guid.ToString());
            #endregion

            #region Protected Methods
            protected virtual void RaiseCallbacksOverride(BrowsableObjectInfoCallbackArgs a) => _callbackQueue?.RaiseCallbacks(a);

            protected void RaiseCallbacks(in BrowsableObjectInfoCallbackArgs a)
            {
                ThrowIfDisposed(this);

                RaiseCallbacksOverride(a);
            }

            protected T GetValueIfNotDisposed<T>(in T value) => IsDisposed ? throw GetExceptionForDispose(false) : value;

            protected T GetValueIfNotDisposed<T>(in Func<T> value) => IsDisposed ? throw GetExceptionForDispose(false) : value();

            /// <summary>
            /// When overridden in a derived class, returns the items of this <see cref="BrowsableObjectInfo"/>.
            /// </summary>
            /// <returns>An <see cref="System.Collections.Generic.IEnumerable{IBrowsableObjectInfo}"/> that enumerates through the items of this <see cref="BrowsableObjectInfo"/>.</returns>
            protected abstract System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride();

            protected abstract ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride();

            protected abstract System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride();

            protected virtual void StartMonitoringOverride() => throw new NotSupportedException();

            protected virtual void StopMonitoringOverride() => throw new NotSupportedException();

            protected virtual System.Collections.Generic.IEnumerable<ICommand> GetCommandsOverride(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => null;
            #endregion

            #region Public Methods
            public System.Collections.Generic.IEnumerable<ICommand> GetCommands(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => GetValueIfNotDisposed(() => GetCommandsOverride(items));

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

            public void StartMonitoring()
            {
                if (IsMonitoringSupported && !IsMonitoring)
                {
                    IsMonitoring = true;

                    StartMonitoringOverride();
                }
            }

            public void StopMonitoring()
            {
                if (GetValueIfNotDisposed(IsMonitoring))
                {
                    StopMonitoringOverride();

                    IsMonitoring = false;
                }
            }

            public ArrayBuilder<IBrowsableObjectInfo> GetRootItems() => GetValueIfNotDisposed(GetRootItemsOverride);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems() => GetValueIfNotDisposed(GetItemsOverride);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItems() => GetValueIfNotDisposed(GetSubRootItemsOverride);
            #endregion

            #region Enumeration
            System.Collections.Generic.IEnumerator<Collections.Generic.IRecursiveEnumerable<IBrowsableObjectInfo>> IRecursiveEnumerableProviderEnumerable<IBrowsableObjectInfo>.GetRecursiveEnumerator() => GetItems().GetEnumerator();

            RecursiveEnumeratorAbstract<IBrowsableObjectInfo> IRecursiveEnumerable<IBrowsableObjectInfo>.GetEnumerator() => IsRecursivelyBrowsable ? new RecursiveEnumerator<IBrowsableObjectInfo>(this, RecursiveEnumerationOrder.ParentThenChildren) : throw new NotSupportedException("The current BrowsableObjectInfo does not support recursive browsing.");

            System.Collections.Generic.IEnumerator<IBrowsableObjectInfo> System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>.GetEnumerator() => GetItems().GetEnumerator();

            IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetItems().GetEnumerator();
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

            /// <summary>
            /// Sets <see cref="IsDisposed"/> to <see langword="true"/>. This method is called from the <see cref="Dispose()"/> method.
            /// </summary>
            protected virtual void DisposeManaged() => IsDisposed = true;

            /// <summary>
            /// Not used in this class.
            /// </summary>
            /// <seealso cref="Dispose"/>
            /// <seealso cref="DisposeManaged"/>
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
            #endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            ~BrowsableObjectInfo() => DisposeUnmanaged();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        public abstract class BrowsableObjectInfo<T> : BrowsableObjectInfo, IBrowsableObjectInfo<T>
        {
            #region Properties
            protected abstract T ObjectPropertiesGenericOverride { get; }

            public T ObjectPropertiesGeneric => ObjectPropertiesGenericOverride;

            protected sealed override object ObjectPropertiesOverride => ObjectPropertiesGenericOverride;

            T IBrowsableObjectInfo<T>.ObjectProperties => ObjectPropertiesGeneric;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="BrowsableObjectInfo"/> class.
            /// </summary>
            /// <param name="path">The path of the new item.</param>
            /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used to initialize new <see cref="PortableDeviceInfo"/>s and <see cref="PortableDeviceObjectInfo"/>s.</param>
            protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
            #endregion
        }

        public abstract class BrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties>, IBrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            #region Properties
            protected abstract TInnerObject InnerObjectGenericOverride { get; }

            public TInnerObject InnerObjectGeneric => GetValueIfNotDisposed(() => InnerObjectGenericOverride);

            TInnerObject IEncapsulatorBrowsableObjectInfo<TInnerObject>.InnerObject => InnerObjectGenericOverride;

            protected sealed override object InnerObjectOverride => InnerObjectGenericOverride;

            // public abstract Predicate<TPredicateTypeParameter> RootItemsPredicate { get; }
            #endregion

            /// <summary>
            /// When called from a derived class, initializes a new instance of the <see cref="BrowsableObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> class with a custom <see cref="ClientVersion"/>.
            /// </summary>
            /// <param name="path">The path of the new <see cref="BrowsableObjectInfo{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.</param>
            /// <param name="clientVersion">A custom <see cref="ClientVersion"/>. This parameter can be null for non-file system and portable devices-related types.</param>
            protected BrowsableObjectInfo(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }

            #region Methods
            public static System.Collections.Generic.IEnumerable<TDictionaryItems> GetEmptyEnumerable() => GetEmptyEnumerable<TDictionaryItems>();

            #region Protected Methods
            protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders();

            protected abstract System.Collections.Generic.IEnumerable<TDictionaryItems> GetItemProviders(Predicate<TPredicateTypeParameter> predicate);

            protected sealed override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItemsOverride() => GetItems(GetItemProviders());

            protected System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(System.Collections.Generic.IEnumerable<TDictionaryItems> items) => GetSelectorDictionary().Select(items);

            protected abstract TSelectorDictionary GetSelectorDictionaryOverride();
            #endregion

            #region Public Methods
            public TSelectorDictionary GetSelectorDictionary() => GetValueIfNotDisposed(GetSelectorDictionaryOverride);

            public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<TPredicateTypeParameter> predicate) => GetValueIfNotDisposed(() => predicate == null ? GetItems(GetItemProviders(item => true)) : GetItems(GetItemProviders(predicate)));
            #endregion
            #endregion
        }
    }
}
