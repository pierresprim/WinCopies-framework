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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDevice, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IPortableDeviceInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        private const int PortableDeviceIcon = 42;
        private const string PortableDeviceIconDllName = "imageres.dll";

        #region Properties
        public sealed override IPortableDevice EncapsulatedObjectGeneric { get; }

        public override bool IsSpecialItem => false;

        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);

        public override bool IsBrowsable => true;

        public override string ItemTypeName => "Portable device";

        public override string Description => "N/A";

        public override IBrowsableObjectInfo Parent => ShellObjectInfo.From(ShellObject.FromParsingName(KnownFolders.Computer.ParsingName), ClientVersion);

        public override string LocalizedName => Name;

        public override string Name => EncapsulatedObjectGeneric.DeviceFriendlyName;
        #endregion // Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : base((portableDevice ?? throw GetArgumentNullException(nameof(portableDevice))).DeviceFriendlyName, clientVersion) => EncapsulatedObjectGeneric = portableDevice;

        private BitmapSource TryGetBitmapSource(in int size)
        {
            using
#if NETFRAMEWORK
            (
#endif

            Icon icon = TryGetIcon(PortableDeviceIcon, PortableDeviceIconDllName, new System.Drawing.Size(size, size))

#if NETFRAMEWORK
            )
#else
                ;
#endif

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }

    public class PortableDeviceInfo : PortableDeviceInfo<IPortableDeviceInfoProperties, IPortableDeviceObject, IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceInfo
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new PortableDeviceInfoSelectorDictionary();

        public override bool IsBrowsableByDefault => true;

        public sealed override IPortableDeviceInfoProperties ObjectPropertiesGeneric { get; }

        public override IPropertySystemCollection ObjectPropertySystem => null;
        #endregion // Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in PortableDeviceOpeningOptions openingOptions, in ClientVersion clientVersion) : base(portableDevice, clientVersion) => ObjectPropertiesGeneric = new PortableDeviceInfoProperties<IPortableDeviceInfo>(this, openingOptions);

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : this(portableDevice, new PortableDeviceOpeningOptions(GenericRights.Read, FileShareOptions.Read, true), clientVersion)
        {
            // Left empty.
        }

        #region Methods
        public override IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders(Predicate<IPortableDeviceObject> predicate)
        {
            if (!EncapsulatedObjectGeneric.IsOpen)

                EncapsulatedObjectGeneric.Open(ClientVersion, ObjectPropertiesGeneric.OpeningOptions);

            return (predicate == null ? EncapsulatedObjectGeneric : EncapsulatedObjectGeneric.WherePredicate(predicate)).SelectConverter(item=>new PortableDeviceObjectInfoItemProvider(item, this, ClientVersion));
        }

        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        #endregion // GetItems
        #endregion // Methods
    }
}
