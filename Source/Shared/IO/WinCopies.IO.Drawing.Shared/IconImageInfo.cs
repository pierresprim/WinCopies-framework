/* Copyright © Pierre Sprimont, 2021
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

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.IO.Shell;
using WinCopies.IO.Shell.ComponentSources.Bitmap;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel
{
    public interface IIconImageInfo : IBrowsableObjectInfo<object, IconImage, IconImageInfoItemProvider, IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>, IconImageInfoItemProvider>
    {
        // Left empty.
    }

    public class IconImageInfo : BrowsableObjectInfo<IBrowsableObjectInfo, IIconImageInfoProperties, IconImage, IconImageInfoItemProvider, IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>, IconImageInfoItemProvider>, IIconImageInfo
    {
        private class ItemSource : ItemSourceBase4<IIconImageInfo, IconImageInfoItemProvider, IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo>, IconImageInfoItemProvider>
        {
            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public override bool IsPaginationSupported => false;

            public ItemSource(in IIconImageInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<IconImageInfoItemProvider> GetItemProviders()
            {
                IIconImageInfo browsableObjectInfo = BrowsableObjectInfo;
                IconImage innerObject = browsableObjectInfo.InnerObject;

                return new IconImageInfoItemProvider[] { new IconImageInfoItemProvider("Image", innerObject.Image, browsableObjectInfo), new IconImageInfoItemProvider("Mask", innerObject.Mask, browsableObjectInfo), new IconImageInfoItemProvider("Icon", innerObject.Icon, browsableObjectInfo) };
            }

            protected override System.Collections.Generic.IEnumerable<IconImageInfoItemProvider> GetItemProviders(Predicate<IconImageInfoItemProvider> predicate) => predicate == null ? GetItemProviders() : GetItemProviders().WherePredicate(predicate);

            internal new System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
                ?
#endif
                GetItems() => base.GetItems();
        }

        private readonly ItemSource _itemSource;
        private ISingleIconInfo _parent;
        private IBitmapSourceProvider _bitmapSourceProvider;
        private IconImage _iconImage;

        protected override IItemSourcesProvider<IconImageInfoItemProvider> ItemSourcesGenericOverride { get; }

        protected override bool IsLocalRootOverride => false;

        protected override IconImage InnerObjectGenericOverride => _iconImage;

        protected override IIconImageInfoProperties
#if CS8
                ?
#endif
                ObjectPropertiesGenericOverride => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
                ?
#endif
                BrowsabilityPathsOverride => null;

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
                ?
#endif
                ObjectPropertySystemOverride => null;

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override bool IsRecursivelyBrowsableOverride => true;

        protected override IBrowsableObjectInfo ParentGenericOverride => _parent;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this, new BrowsableObjectInfoIconBitmapSources(InnerObjectGeneric.Icon), true)
#if !CS8
            )
#endif
            ;

        protected override string ItemTypeNameOverride => "Icon image";

        protected override string DescriptionOverride => WinCopies.Consts.NotApplicable;

        protected override bool IsSpecialItemOverride => false;

        public override string LocalizedName => $"{InnerObjectGeneric.Size} {ObjectPropertiesGeneric.FriendlyBitDepth}";

        public override string Name => LocalizedName;

        public static IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new IconImageInfoSelectorDictionary();

        public IconImageInfo(in IconImage icon, in ISingleIconInfo parent) : base($"{(parent ?? throw GetArgumentNullException(nameof(parent))).Path}\\{(icon ?? throw GetArgumentNullException(nameof(icon))).Size} - {icon.IconImageFormat}", parent.ClientVersion)
        {
            _parent = parent;

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(_itemSource = new ItemSource(this));
        }

        protected override IEnumerableSelectorDictionary<IconImageInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetSubRootItemsOverride() => _itemSource.GetItems();

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(Parent.Parent);

            return arrayBuilder;
        }

        protected override void DisposeManaged()
        {
            _iconImage = null;

            base.DisposeManaged();
        }

        protected override void DisposeUnmanaged()
        {
            _bitmapSourceProvider.Dispose();
            _bitmapSourceProvider = null;

            _parent = null;

            base.DisposeUnmanaged();
        }
    }
}
