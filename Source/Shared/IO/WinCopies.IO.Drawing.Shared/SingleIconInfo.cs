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

#region System
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.GUI.Drawing;
using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ComponentSources.Item;
using WinCopies.IO.Process;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;
using WinCopies.Linq;
using WinCopies.PropertySystem;
#endregion WinCopies

namespace WinCopies.IO.ObjectModel
{
    public interface ISingleIconInfo : IBrowsableObjectInfo<object, SingleIcon, SingleIconInfoItemProvider, IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>, SingleIconInfoItemProvider>
    {
        // Left empty.
    }

    public class SingleIconInfo : BrowsableObjectInfo<IBrowsableObjectInfo, object, SingleIcon, SingleIconInfoItemProvider, IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>, SingleIconInfoItemProvider>, ISingleIconInfo
    {
        #region Subtypes
        protected class BrowsableObjectInfoBitmapSources : BitmapSources<SingleIcon>
        {
            public static BitmapSource TryGetBitmapSource(in Icon icon, in ushort size) => BrowsableObjectInfo.TryGetBitmapSource(TryGetIcon(icon, size));

            protected override BitmapSource SmallOverride => TryGetBitmapSource(InnerObject.Icon, SmallIconSize);

            protected override BitmapSource MediumOverride => TryGetBitmapSource(InnerObject.Icon, MediumIconSize);

            protected override BitmapSource LargeOverride => TryGetBitmapSource(InnerObject.Icon, LargeIconSize);

            protected override BitmapSource ExtraLargeOverride => TryGetBitmapSource(InnerObject.Icon, ExtraLargeIconSize);

            public BrowsableObjectInfoBitmapSources(in SingleIcon singleIcon) : base(singleIcon) { /* Left empty. */ }
        }

        public class ItemSource : ItemSourceBase4<ISingleIconInfo, SingleIconInfoItemProvider, IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo>, SingleIconInfoItemProvider>
        {
            public override bool IsPaginationSupported => false;

            protected override IProcessSettings
#if CS8
                ?
#endif
                ProcessSettingsOverride => null;

            public ItemSource(in ISingleIconInfo browsableObjectInfo) : base(browsableObjectInfo) { /* Left empty. */ }

            protected override System.Collections.Generic.IEnumerable<SingleIconInfoItemProvider> GetItemProviders() => BrowsableObjectInfo.InnerObject.Select(icon => new SingleIconInfoItemProvider(icon, BrowsableObjectInfo));

            protected override System.Collections.Generic.IEnumerable<SingleIconInfoItemProvider> GetItemProviders(Predicate<SingleIconInfoItemProvider> predicate) => predicate == null ? GetItemProviders() : GetItemProviders().WherePredicate(predicate);
        }
        #endregion

        private static readonly WriteOnlyBrowsabilityPathStack<ISingleIconInfo> _browsabilityPathStack = __browsabilityPathStack.AsWriteOnly();
        public static IBrowsabilityPathStack<ISingleIconInfo> BrowsabilityPathStack => _browsabilityPathStack;
        private static readonly BrowsabilityPathStack<ISingleIconInfo> __browsabilityPathStack = new
#if !CS9
                BrowsabilityPathStack<ISingleIconInfo>
#endif
                ();

        private SingleIcon _singleIcon;
        private IBrowsableObjectInfo _parent;
        private IBitmapSourceProvider _bitmapSourceProvider;

        protected override IItemSourcesProvider<SingleIconInfoItemProvider>
#if CS8
            ?
#endif
            ItemSourcesGenericOverride
        { get; }

        protected override bool IsLocalRootOverride => false;

        protected override SingleIcon InnerObjectGenericOverride => _singleIcon;

        protected override object
#if CS8
            ?
#endif
            ObjectPropertiesGenericOverride => null;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => __browsabilityPathStack.GetBrowsabilityPaths(this);

        protected override IPropertySystemCollection<PropertyId, ShellPropertyGroup>
#if CS8
            ?
#endif
            ObjectPropertySystemOverride => null;

        protected override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override bool IsRecursivelyBrowsableOverride => true;

        protected override IBrowsableObjectInfo ParentGenericOverride => _parent
#if CS8
            ??=
#else
            ?? (_parent =
#endif
            new MultiIconInfo(ShellObjectInfo.From(Path.Remove(Path.LastIndexOf("\\"))))
#if !CS8
            )
#endif
            ;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this, new BrowsableObjectInfoBitmapSources(InnerObjectGeneric), true)
#if !CS8
            )
#endif
            ;

        protected override string ItemTypeNameOverride => "Icon";

        protected override string DescriptionOverride => WinCopies.Consts.NotApplicable;

        protected override bool IsSpecialItemOverride => false;

        public override string LocalizedName => GetValueIfNotDisposed(_singleIcon).Name;

        public override string Name => LocalizedName;

        public static IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo> DefaultItemSelectorDictionary { get; } = new SingleIconInfoSelectorDictionary();

        public override string
#if CS8
            ?
#endif
            Protocol => null;

        public SingleIconInfo(in string parentPath, in ClientVersion clientVersion, in SingleIcon icon) : base($"{parentPath}\\{icon.Name}", clientVersion)
        {
            _singleIcon = icon;

            ItemSourcesGenericOverride = ItemSourcesProvider.Construct(new ItemSource(this));
        }

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride()
        {
            var arrayBuilder = new ArrayBuilder<IBrowsableObjectInfo>();

            _ = arrayBuilder.AddLast(Parent);

            return arrayBuilder;
        }

        protected override IEnumerableSelectorDictionary<SingleIconInfoItemProvider, IBrowsableObjectInfo> GetSelectorDictionaryOverride() => DefaultItemSelectorDictionary;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => GetDefaultEnumerable();

        protected override void DisposeUnmanaged()
        {
            _singleIcon = null;
            _parent = null;

            base.DisposeUnmanaged();
        }

        protected override void DisposeManaged()
        {
            _bitmapSourceProvider = null;

            base.DisposeManaged();
        }
    }
}
