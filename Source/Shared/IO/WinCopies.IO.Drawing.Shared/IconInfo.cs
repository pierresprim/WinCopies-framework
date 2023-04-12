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
using System.Drawing;

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Shell.ComponentSources.Bitmap;
using WinCopies.PropertySystem;
#endregion WinCopies

namespace WinCopies.IO.ObjectModel
{
    public interface IIconInfo : IBrowsableObjectInfo<object, Icon, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>
    {
        // Left empty.
    }

    public class IconInfo : BrowsableObjectInfo<IBrowsableObjectInfo, object, Icon, object, IEnumerableSelectorDictionary<object, IBrowsableObjectInfo>, object>, IIconInfo
    {
        private Icon _icon;
        private IBrowsableObjectInfo _parent;
        private IBitmapSourceProvider _bitmapSourceProvider;

        protected override IItemSourcesProvider<object>
#if CS8
            ?
#endif
            ItemSourcesGenericOverride => null;

        public override string
#if CS8
                ?
#endif
                Protocol => null;

        protected override bool IsLocalRootOverride => false;

        public override string Name => LocalizedName;

        protected override IBrowsableObjectInfo ParentGenericOverride => _parent;

        protected override Icon InnerObjectGenericOverride => _icon;

        protected override object
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

        protected override bool IsRecursivelyBrowsableOverride => false;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this, new BrowsableObjectInfoIconBitmapSources(InnerObjectGeneric), true)
#if !CS8
            )
#endif
            ;

        protected override string ItemTypeNameOverride => "Icon";

        protected override string DescriptionOverride => "Icon";

        protected override bool IsSpecialItemOverride => false;

        public override string LocalizedName { get; }

        public IconInfo(in IIconImageInfo parent, in string name, in Icon icon) : base($"{parent.Path}\\{name}", parent.ClientVersion)
        {
            LocalizedName = name;
            _icon = icon;
            _parent = parent;
        }

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(Parent.Parent.Parent);

            return arrayBuilder;
        }

        protected override void DisposeUnmanaged()
        {
            _bitmapSourceProvider.Dispose();
            _bitmapSourceProvider = null;

            _parent = null;

            base.DisposeUnmanaged();
        }

        protected override void DisposeManaged()
        {
            _icon = null;

            base.DisposeManaged();
        }

        protected override IEnumerableSelectorDictionary<object, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;
    }
}
