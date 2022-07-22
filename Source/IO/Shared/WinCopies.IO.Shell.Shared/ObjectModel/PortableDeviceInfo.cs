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
#region WAPICP
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Win32Native;
#endregion WAPICP

using System;

#region WinCopies
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.ThrowHelper;
#endregion Usings

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDevice, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IPortableDeviceInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private IPortableDevice _portableDevice;
        private IBitmapSourceProvider _bitmapSourceProvider;

        #region Properties
        public override string Protocol => "mtp";

        protected override bool IsLocalRootOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            FileSystemObjectInfo.GetDefaultBitmapSourcesProvider(this, new Shell.ComponentSources.Bitmap.BitmapSources(new Shell.ComponentSources.Bitmap.BitmapSourcesStruct(38, "imageres.dll")))
#if !CS8
            )
#endif
            ;

        protected sealed override IPortableDevice InnerObjectGenericOverride => _portableDevice;

        protected override bool IsSpecialItemOverride => false;

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.PortableDevice;

        protected override string DescriptionOverride => WinCopies.Consts.NotApplicable;

        protected override IBrowsableObjectInfo ParentGenericOverride => ShellObjectInfo.From(ShellObjectFactory.Create(KnownFolders.Computer.ParsingName), ClientVersion);

        public override string LocalizedName => Name;

        public override string Name => InnerObjectGeneric.DeviceFriendlyName;
        #endregion Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : base((portableDevice ?? throw GetArgumentNullException(nameof(portableDevice))).DeviceFriendlyName, clientVersion) => _portableDevice = portableDevice;

        protected override void DisposeUnmanaged()
        {
            if (_bitmapSourceProvider != null)
            {
                _bitmapSourceProvider.Dispose();
                _bitmapSourceProvider = null;
            }

            base.DisposeUnmanaged();
        }

        protected override void DisposeManaged()
        {
            _portableDevice.Dispose();
            _portableDevice = null;

            base.DisposeManaged();
        }
    }

    public class PortableDeviceInfo : PortableDeviceInfo<IPortableDeviceInfoProperties, IPortableDeviceObject, IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>, PortableDeviceObjectInfoItemProvider>, IPortableDeviceInfo
    {
        public class ItemSource : ItemSourceBase4<IPortableDeviceInfo, IPortableDeviceObject, IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>, PortableDeviceObjectInfoItemProvider>
        {
            protected override IProcessSettings ProcessSettingsOverride { get; } 

            public override bool IsPaginationSupported => false;

            public ItemSource(in IPortableDeviceInfo browsableObjectInfo) : base(browsableObjectInfo) => ProcessSettingsOverride = new ProcessSettings(null, DefaultCustomProcessesSelectorDictionary.Select(BrowsableObjectInfo));

            protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders(Predicate<IPortableDeviceObject>
#if CS8
                ?
#endif
                predicate)
            {
                if (!BrowsableObjectInfo.InnerObject.IsOpen)

                    BrowsableObjectInfo.InnerObject.Open(BrowsableObjectInfo.ClientVersion.ToPortableDeviceClientVersion(), BrowsableObjectInfo.ObjectProperties.OpeningOptions);

                return (predicate == null ? BrowsableObjectInfo.InnerObject : BrowsableObjectInfo.InnerObject.WherePredicate(predicate)).SelectConverter(item => new PortableDeviceObjectInfoItemProvider(item, BrowsableObjectInfo, BrowsableObjectInfo.ClientVersion));
            }

            protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        }

        private static readonly BrowsabilityPathStack<IPortableDeviceInfo> _browsabilityPathStack = new BrowsabilityPathStack<IPortableDeviceInfo>();
        private IPortableDeviceInfoProperties _objectProperties;

        #region Properties
        protected override IItemSourcesProvider<IPortableDeviceObject> ItemSourcesGenericOverride { get; }

        public static ISelectorDictionary<IPortableDeviceInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IPortableDeviceInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        public static IBrowsabilityPathStack<IPortableDeviceInfo> BrowsabilityPathStack { get; } = _browsabilityPathStack.AsWriteOnly();

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => _browsabilityPathStack.GetBrowsabilityPaths(this);

        public static IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new PortableDeviceInfoSelectorDictionary();

        protected sealed override IPortableDeviceInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;
        #endregion Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in PortableDeviceOpeningOptions openingOptions, in ClientVersion clientVersion) : base(portableDevice, clientVersion)
        {
            _objectProperties = new PortableDeviceInfoProperties<IPortableDeviceInfo>(this, openingOptions);
            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : this(portableDevice, new PortableDeviceOpeningOptions(GenericRights.Read, FileShareOptions.Read, true), clientVersion) { /* Left empty. */ }

        protected override IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override void DisposeManaged()
        {
            _objectProperties = null;

            base.DisposeManaged();
        }
    }
}
