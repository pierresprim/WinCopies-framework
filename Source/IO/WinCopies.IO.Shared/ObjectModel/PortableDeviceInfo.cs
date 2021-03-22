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
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDevice, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IPortableDeviceInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        private const int PortableDeviceIcon = 42;
        private const string PortableDeviceIconDllName = "imageres.dll";
        private IPortableDevice _portableDevice;

        #region Properties
        public override IProcessFactory ProcessFactory => IO.ProcessFactory.DefaultProcessFactory;

        public sealed override IPortableDevice InnerObjectGeneric => IsDisposed ? throw GetExceptionForDispose(false) : _portableDevice;

        public override bool IsSpecialItem => false;

        public override BitmapSource SmallBitmapSource => TryGetBitmapSource(SmallIconSize);

        public override BitmapSource MediumBitmapSource => TryGetBitmapSource(MediumIconSize);

        public override BitmapSource LargeBitmapSource => TryGetBitmapSource(LargeIconSize);

        public override BitmapSource ExtraLargeBitmapSource => TryGetBitmapSource(ExtraLargeIconSize);

        public override IBrowsabilityOptions Browsability => BrowsabilityOptions.BrowsableByDefault;

        public override string ItemTypeName => Properties.Resources.PortableDevice;

        public override string Description => UtilHelpers.NotApplicable;

        public IProcessPathCollectionFactory ShellProcessPathCollectionFactory { get; }

        public override IBrowsableObjectInfo Parent => ShellObjectInfo.From(ShellObjectFactory.Create(KnownFolders.Computer.ParsingName), ShellProcessPathCollectionFactory, ClientVersion);

        public override string LocalizedName => Name;

        public override string Name => InnerObjectGeneric.DeviceFriendlyName;
        #endregion Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion) : base((portableDevice ?? throw GetArgumentNullException(nameof(portableDevice))).DeviceFriendlyName, null, clientVersion)
        {
            _portableDevice = portableDevice;

            ShellProcessPathCollectionFactory = shellProcessPathCollectionFactory;
        }

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

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _portableDevice.Dispose();

            _portableDevice = null;
        }
    }

    public class PortableDeviceInfo : PortableDeviceInfo<IPortableDeviceInfoProperties, IPortableDeviceObject, IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceInfo
    {
        private IPortableDeviceInfoProperties _objectProperties;

        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new PortableDeviceInfoSelectorDictionary();

        public sealed override IPortableDeviceInfoProperties ObjectPropertiesGeneric => IsDisposed ? throw GetExceptionForDispose(false) : _objectProperties;

        public override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystem => null;
        #endregion // Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in PortableDeviceOpeningOptions openingOptions, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion) : base(portableDevice, shellProcessPathCollectionFactory, clientVersion) => _objectProperties = new PortableDeviceInfoProperties<IPortableDeviceInfo>(this, openingOptions);

        public PortableDeviceInfo(in IPortableDevice portableDevice, in IProcessPathCollectionFactory shellProcessPathCollectionFactory, in ClientVersion clientVersion) : this(portableDevice, new PortableDeviceOpeningOptions(GenericRights.Read, FileShareOptions.Read, true), shellProcessPathCollectionFactory, clientVersion)
        {
            // Left empty.
        }

        #region Methods
        public override IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _objectProperties = null;
        }

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders(Predicate<IPortableDeviceObject> predicate)
        {
            if (!InnerObjectGeneric.IsOpen)

                InnerObjectGeneric.Open(ClientVersion, ObjectPropertiesGeneric.OpeningOptions);

            return (predicate == null ? InnerObjectGeneric : InnerObjectGeneric.WherePredicate(predicate)).SelectConverter(item => new PortableDeviceObjectInfoItemProvider(item, this, ClientVersion));
        }

        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        #endregion // GetItems
        #endregion // Methods
    }
}
