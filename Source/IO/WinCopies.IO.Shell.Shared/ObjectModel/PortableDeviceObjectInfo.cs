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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.PropertySystem;

using System;
using System.Windows.Media.Imaging;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common;

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

        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => PortableDeviceObjectInfo.DefaultCustomProcessesSelectorDictionary.Select(this);

        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

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

        protected override string ItemTypeNameOverride => FileSystemObjectInfo.GetItemTypeName(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType);

        protected override string DescriptionOverride => UtilHelpers.NotApplicable;

        protected override IBrowsableObjectInfo ParentOverride => _parent;

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
        private PortableDeviceObjectInfo(in string path, in IPortableDeviceObject portableDeviceObject, in ClientVersion clientVersion) : base(path, clientVersion) => _portableDeviceObject = portableDeviceObject;

        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : this(GetPath(portableDeviceObject, parentPortableDevice), portableDeviceObject, clientVersion) => _parent = parentPortableDevice;

        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : this(GetPath(portableDeviceObject, parent), portableDeviceObject, clientVersion) => _parent = parent;
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

        private static string GetPath(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice)
        {
            ThrowIfNull(portableDeviceObject, nameof(portableDeviceObject));
            ThrowIfNull(parentPortableDevice, nameof(parentPortableDevice));

            return $"{parentPortableDevice.Path}{IO.Path.PathSeparator}{portableDeviceObject.Name}";
        }

        private static string GetPath(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent)
        {
            ThrowIfNull(portableDeviceObject, nameof(portableDeviceObject));
            ThrowIfNull(parent, nameof(parent));

            return $"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}{IO.Path.PathSeparator}{(portableDeviceObject ?? throw GetArgumentNullException(nameof(portableDeviceObject))).Name}";
        }
    }

    public class PortableDeviceObjectInfo : PortableDeviceObjectInfo<IFileSystemObjectInfoProperties, IPortableDeviceObject, IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceObjectInfo
    {
        private static readonly BrowsabilityPathStack<IPortableDeviceObjectInfo> __browsabilityPathStack = new BrowsabilityPathStack<IPortableDeviceObjectInfo>();

        private IFileSystemObjectInfoProperties _objectProperties;

        #region Properties
        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        public static IBrowsabilityPathStack<IPortableDeviceObjectInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

        public static ISelectorDictionary<IPortableDeviceObjectInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IPortableDeviceObjectInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        public static IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new PortableDeviceObjectInfoSelectorDictionary();

        protected sealed override IFileSystemObjectInfoProperties ObjectPropertiesGenericOverride => IsDisposed ? throw GetExceptionForDispose(false) : _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null; // TODO
        #endregion

        #region Constructors
        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : base(portableDeviceObject, parentPortableDevice, clientVersion) => _objectProperties = GetProperties();

        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : base(portableDeviceObject, parent, clientVersion) => _objectProperties = GetProperties();
        #endregion

        #region Methods
        protected override IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        private IFileSystemObjectInfoProperties GetProperties() => new PortableDeviceObjectInfoProperties<IPortableDeviceObjectInfoBase>(this);

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _objectProperties = null;
        }

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders(Predicate<IPortableDeviceObject> predicate) => InnerObject is IEnumerablePortableDeviceObject enumerablePortableDeviceObject
                ? (predicate == null
                    ? enumerablePortableDeviceObject
                    : enumerablePortableDeviceObject.WherePredicate(predicate)).SelectConverter(item => new PortableDeviceObjectInfoItemProvider(item, this, ClientVersion))
                : GetEmptyEnumerable();

        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        #endregion
        #endregion
    }
}
