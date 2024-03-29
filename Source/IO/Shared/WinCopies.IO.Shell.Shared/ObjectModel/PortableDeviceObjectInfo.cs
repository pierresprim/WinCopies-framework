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

#region WAPICP
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.PropertySystem;
#endregion

using System;

#region WinCopies
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion

using static Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common;

using static System.IO.Path;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDeviceObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        #region Fields
        private IPortableDeviceObject _portableDeviceObject;
        private IBrowsableObjectInfo _parent;
        private IBrowsabilityOptions _browsability;
        private bool _isNameLoaded;
        private string _name;
        private bool? _isSpecialItem;
        #endregion

        #region Properties
        protected override bool IsLocalRootOverride => false;

        protected sealed override IPortableDeviceObject InnerObjectGenericOverride => _portableDeviceObject;

        protected override bool IsSpecialItemOverride
        {
            get
            {
                if (_isSpecialItem.HasValue)

                    return _isSpecialItem.Value;

                bool result = (InnerObjectGeneric.Properties.TryGetValue(IsSystem, out Property value) && value.TryGetValue(out bool _value) && _value) || (InnerObjectGeneric.Properties.TryGetValue(IsHidden, out Property __value) && __value.TryGetValue(out bool ___value) && ___value);

                _isSpecialItem = result;

                return result;
            }
        }

        protected override IBrowsabilityOptions BrowsabilityOverride => _browsability
#if CS8
            ??=
#else
            ?? (_browsability =
#endif
            InnerObject is IEnumerablePortableDeviceObject ? BrowsabilityOptions.BrowsableByDefault : BrowsabilityOptions.NotBrowsable
#if !CS8
            )
#endif
            ;

        protected override string ItemTypeNameOverride => FileSystemObjectInfo.GetItemTypeName(GetExtension(Path), ObjectPropertiesGeneric.FileType);

        protected override string DescriptionOverride => WinCopies.Consts.
#if WinCopies4
            Common.
#endif
            NotApplicable;

        protected override IBrowsableObjectInfo ParentGenericOverride => _parent;

        public override string LocalizedName => Name;

        public override string Name
        {
            get
            {
                if (_isNameLoaded)

                    return _name;

                _name = InnerObjectGeneric.Name;

                _isNameLoaded = true;

                return _name;
            }
        }
        #endregion Properties

        #region Constructors
        private protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IBrowsableObjectInfo parent, in string parentParamName, in ClientVersion clientVersion) : base(GetPath(portableDeviceObject, parent, parentParamName), clientVersion)
        {
            _portableDeviceObject = portableDeviceObject;
            _parent = parent;
        }

        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : this(portableDeviceObject, parentPortableDevice, nameof(parentPortableDevice), clientVersion) { /* Left empty. */ }

        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : this(portableDeviceObject, parent, nameof(parent), clientVersion) => _parent = parent;
        #endregion

        protected override void DisposeUnmanaged()
        {
            _parent = null;

            base.DisposeUnmanaged();
        }

        protected override void DisposeManaged()
        {
            InnerObjectGeneric.Dispose();

            _portableDeviceObject = null;

            base.DisposeManaged();
        }

        private static string GetPath(in IPortableDeviceObject portableDeviceObject, in IBrowsableObjectInfo parent, in string parentParamName) => $"{(parent ?? throw new ArgumentNullException(parentParamName)).Path}{DirectorySeparatorChar}{(portableDeviceObject ?? throw new ArgumentNullException(nameof(portableDeviceObject))).Name}";
    }

    public class PortableDeviceObjectInfo : PortableDeviceObjectInfo<IFileSystemObjectInfoProperties, IPortableDeviceObject, IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceObjectInfo
    {
        public class ItemSource : ItemSourceBase3<IPortableDeviceObjectInfo, IPortableDeviceObject, IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>, PortableDeviceObjectInfoItemProvider>
        {
            private IProcessSettings _processSettings;

            public override bool IsPaginationSupported => false;

            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => _processSettings;

            public override bool IsDisposed => _processSettings == null;

            public ItemSource(in IPortableDeviceObjectInfo browsableObjectInfo) : base(browsableObjectInfo) => _processSettings = new ProcessSettings(null, DefaultCustomProcessesSelectorDictionary.Select(browsableObjectInfo));

            protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders(Predicate<IPortableDeviceObject>
#if CS8
                ?
#endif
                predicate) => BrowsableObjectInfo.InnerObject is IEnumerablePortableDeviceObject enumerablePortableDeviceObject
                    ? (predicate == null
                        ? enumerablePortableDeviceObject
                        : enumerablePortableDeviceObject.WherePredicate(predicate)).SelectConverter(item => new PortableDeviceObjectInfoItemProvider(item, BrowsableObjectInfo, BrowsableObjectInfo.ClientVersion))
                    : null;

            protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider>
#if CS8
                ?
#endif
                GetItemProviders() => GetItemProviders(null);

            protected override void DisposeManaged() => _processSettings = null;
        }

        private static IItemSourcesProvider<IPortableDeviceObject> _itemSourcesOverride;
        private static readonly BrowsabilityPathStack<IPortableDeviceObjectInfo> __browsabilityPathStack = new BrowsabilityPathStack<IPortableDeviceObjectInfo>();
        private IFileSystemObjectInfoProperties _objectProperties;

        #region Properties
        protected override IItemSourcesProvider<IPortableDeviceObject> ItemSourcesGenericOverride => _itemSourcesOverride;

        public override string Protocol => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        public static IBrowsabilityPathStack<IPortableDeviceObjectInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

        public static ISelectorDictionary<IPortableDeviceObjectInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IPortableDeviceObjectInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        public static IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new PortableDeviceObjectInfoSelectorDictionary();

        protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => IsDisposed ? throw GetExceptionForDispose(false) : _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null; // TODO
        #endregion Properties

        #region Constructors
        private PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IBrowsableObjectInfo parent, in string parentParamName, in ClientVersion clientVersion) : base(portableDeviceObject, parent, parentParamName, clientVersion)
        {
            _objectProperties = new PortableDeviceObjectInfoProperties<IPortableDeviceObjectInfoBase>(this);
            _itemSourcesOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : this(portableDeviceObject, parentPortableDevice, nameof(parentPortableDevice), clientVersion) { /* Left empty. */ }

        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : this(portableDeviceObject, parent, nameof(parent), clientVersion) { /* Left empty. */ }
        #endregion Constructors

        #region Methods
        public override IBrowsableObjectInfo Clone() => new PortableDeviceObjectInfo(InnerObjectGeneric, ParentGenericOverride, "parent", ClientVersion);

        protected override IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _objectProperties = null;
        }
        #endregion Methods
    }
}
