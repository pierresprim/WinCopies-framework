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
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell;
using WinCopies.Linq;
using WinCopies.PropertySystem;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public abstract class PortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, IPortableDevice, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IPortableDeviceInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        protected class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>>
        {
            protected override BitmapSource SmallBitmapSourceOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

            protected override BitmapSource MediumBitmapSourceOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

            protected override BitmapSource LargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

            protected override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

            public BrowsableObjectInfoBitmapSources(in IPortableDeviceInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> portableDeviceInfo) : base(portableDeviceInfo) { /* Left empty. */ }
        }

        private const int PortableDeviceIcon = 42;
        private const string PortableDeviceIconDllName = "imageres.dll";
        private IPortableDevice _portableDevice;
        private IBrowsableObjectInfoBitmapSources _bitmapSources;

        #region Properties
        protected override IBrowsableObjectInfoBitmapSources BitmapSourcesOverride => _bitmapSources
#if CS8
            ??=
#else
            ?? (_bitmapSources =
#endif
            new BrowsableObjectInfoBitmapSources(this)
#if !CS8
            )
#endif
            ;

        protected override IProcessFactory ProcessFactoryOverride => Process.ProcessFactory.DefaultProcessFactory;

        protected sealed override IPortableDevice InnerObjectGenericOverride => _portableDevice;

        protected override bool IsSpecialItemOverride => false;

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override string ItemTypeNameOverride => Shell.Properties.Resources.PortableDevice;

        protected override string DescriptionOverride => UtilHelpers.NotApplicable;

        protected override IBrowsableObjectInfo ParentOverride => ShellObjectInfo.From(ShellObjectFactory.Create(KnownFolders.Computer.ParsingName), ClientVersion);

        public override string LocalizedName => Name;

        public override string Name => InnerObjectGeneric.DeviceFriendlyName;

        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => PortableDeviceInfo.DefaultCustomProcessesSelectorDictionary.Select(this);
        #endregion Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : base((portableDevice ?? throw GetArgumentNullException(nameof(portableDevice))).DeviceFriendlyName, clientVersion) => _portableDevice = portableDevice;

        private BitmapSource TryGetBitmapSource(in int size)
        {
            using
#if NETFRAMEWORK
            (
#endif

            Icon icon = Shell.ObjectModel.BrowsableObjectInfo.TryGetIcon(PortableDeviceIcon, PortableDeviceIconDllName, new System.Drawing.Size(size, size))

#if NETFRAMEWORK
            )
#else
                ;
#endif

            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        protected override void DisposeUnmanaged()
        {
            if (_bitmapSources != null)
            {
                _bitmapSources.Dispose();
                _bitmapSources = null;
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
        private static readonly BrowsabilityPathStack<IPortableDeviceInfo> __browsabilityPathStack = new BrowsabilityPathStack<IPortableDeviceInfo>();

        private IPortableDeviceInfoProperties _objectProperties;

        #region Properties
        public static ISelectorDictionary<IPortableDeviceInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>> DefaultCustomProcessesSelectorDictionary { get; } = new DefaultNullableValueSelectorDictionary<IPortableDeviceInfoBase, System.Collections.Generic.IEnumerable<IProcessInfo>>();

        public static IBrowsabilityPathStack<IPortableDeviceInfo> BrowsabilityPathStack { get; } = __browsabilityPathStack.AsWriteOnly();

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        public static IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new PortableDeviceInfoSelectorDictionary();

        protected sealed override IPortableDeviceInfoProperties ObjectPropertiesGenericOverride => _objectProperties;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup> ObjectPropertySystemOverride => null;
        #endregion // Properties

        public PortableDeviceInfo(in IPortableDevice portableDevice, in PortableDeviceOpeningOptions openingOptions, in ClientVersion clientVersion) : base(portableDevice, clientVersion) => _objectProperties = new PortableDeviceInfoProperties<IPortableDeviceInfo>(this, openingOptions);

        public PortableDeviceInfo(in IPortableDevice portableDevice, in ClientVersion clientVersion) : this(portableDevice, new PortableDeviceOpeningOptions(GenericRights.Read, FileShareOptions.Read, true), clientVersion)
        {
            // Left empty.
        }

        #region Methods
        protected override IEnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override void DisposeManaged()
        {
            _objectProperties = null;

            base.DisposeManaged();
        }

        #region GetItems
        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders(Predicate<IPortableDeviceObject> predicate)
        {
            if (!InnerObjectGeneric.IsOpen)

                InnerObjectGeneric.Open(ClientVersion.ToPortableDeviceClientVersion(), ObjectPropertiesGeneric.OpeningOptions);

            return (predicate == null ? InnerObjectGeneric : InnerObjectGeneric.WherePredicate(predicate)).SelectConverter(item => new PortableDeviceObjectInfoItemProvider(item, this, ClientVersion));
        }

        protected override System.Collections.Generic.IEnumerable<PortableDeviceObjectInfoItemProvider> GetItemProviders() => GetItemProviders(null);
        #endregion GetItems
        #endregion Methods
    }
}
