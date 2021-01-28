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
using System.Linq;
using System.Windows.Media.Imaging;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;

using static Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Legacy.Object.Common;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDeviceObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Private fields
        private bool? _isBrowsable;
        private bool _isNameLoaded;
        private string _name;
        #endregion

        #region Properties
        public sealed override IPortableDeviceObject InnerObjectGeneric { get; }

        private bool? _isSpecialItem;

        public override bool IsSpecialItem
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

        #region BitmapSources
        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);
        #endregion

        public override bool IsBrowsable
        {
            get
            {
                if (_isBrowsable.HasValue)

                    return _isBrowsable.Value;

                bool result = InnerObject is IEnumerablePortableDeviceObject;

                _isBrowsable = result;

                return result;
            }
        }

        public override string ItemTypeName => FileSystemObjectInfo.GetItemTypeName(System.IO.Path.GetExtension(Path), ObjectPropertiesGeneric.FileType);

        public override string Description => UtilHelpers.NotApplicable;

        public override IBrowsableObjectInfo Parent { get; }

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
        #endregion

        #region Constructors
        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : this(GetPath(portableDeviceObject, parentPortableDevice), portableDeviceObject, clientVersion) => Parent = parentPortableDevice;

        protected PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : this(GetPath(portableDeviceObject, parent), portableDeviceObject, clientVersion) => Parent = parent;

        private PortableDeviceObjectInfo(in string path, in IPortableDeviceObject portableDeviceObject, in ClientVersion clientVersion) : base(path, clientVersion) => InnerObjectGeneric = portableDeviceObject;
        #endregion

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

    public class PortableDeviceObjectInfo : PortableDeviceObjectInfo<IFileSystemObjectInfoProperties, IPortableDeviceObject, IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceObjectInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new PortableDeviceObjectInfoSelectorDictionary();

        public sealed override IFileSystemObjectInfoProperties ObjectPropertiesGeneric { get; }

        public override bool IsBrowsableByDefault => true;

        public override IPropertySystemCollection ObjectPropertySystem => null; // TODO
        #endregion

        #region Constructors
        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceInfoBase parentPortableDevice, in ClientVersion clientVersion) : base(portableDeviceObject, parentPortableDevice, clientVersion) => ObjectPropertiesGeneric = GetProperties();

        internal PortableDeviceObjectInfo(in IPortableDeviceObject portableDeviceObject, in IPortableDeviceObjectInfoBase parent, in ClientVersion clientVersion) : base(portableDeviceObject, parent, clientVersion) => ObjectPropertiesGeneric = GetProperties();
        #endregion

        #region Methods
        public override IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        private IFileSystemObjectInfoProperties GetProperties() => new PortableDeviceObjectInfoProperties<IPortableDeviceObjectInfoBase>(this);

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
