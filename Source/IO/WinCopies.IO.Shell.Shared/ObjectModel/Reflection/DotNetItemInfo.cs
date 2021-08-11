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

using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using WinCopies.Collections.Generic;
using WinCopies.IO.Process;
using WinCopies.IO.Reflection.PropertySystem;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        protected class BrowsableObjectInfoBitmapSources : BrowsableObjectInfoBitmapSources<DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>>
        {
            protected sealed override BitmapSource SmallBitmapSourceOverride => InnerObject.TryGetBitmapSource(SmallIconSize);

            protected sealed override BitmapSource MediumBitmapSourceOverride => InnerObject.TryGetBitmapSource(MediumIconSize);

            protected sealed override BitmapSource LargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(LargeIconSize);

            protected sealed override BitmapSource ExtraLargeBitmapSourceOverride => InnerObject.TryGetBitmapSource(ExtraLargeIconSize);

            public BrowsableObjectInfoBitmapSources(in DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> dotNetItemInfo) : base(dotNetItemInfo) { /* Left empty. */ }
        }

        private static ArrayBuilder<IBrowsableObjectInfo> _defaultRootItems;
        private IBrowsableObjectInfo _parent;
        private IDotNetAssemblyInfo _parentAssembly;
        private IBrowsableObjectInfoBitmapSources _bitmapSources;

        #region Properties
        //public override Predicate<TPredicateTypeParameter> RootItemsPredicate => item => false;

        //public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => null;

        /// <summary>
        /// Gets the name of this <see cref="IDotNetItemInfo"/>.
        /// </summary>
        public sealed override string Name { get; }

        /// <summary>
        /// Gets the same value as <see cref="Name"/>.
        /// </summary>
        public sealed override string LocalizedName => Name;

        /// <summary>
        /// Gets the <see cref="NotApplicable"/> value.
        /// </summary>
        protected override string DescriptionOverride => NotApplicable;

        /// <summary>
        /// Gets the <see langword="false"/> value.
        /// </summary>
        protected override bool IsSpecialItemOverride => false;

        protected sealed override IBrowsableObjectInfo ParentOverride => _parent;

        public IDotNetAssemblyInfo ParentDotNetAssemblyInfo => _parentAssembly;

        protected sealed override bool IsRecursivelyBrowsableOverride => IsBrowsable;

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

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath> BrowsabilityPathsOverride => null;

        protected override System.Collections.Generic.IEnumerable<IProcessInfo> CustomProcessesOverride => null;
        #endregion

        protected DotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, parent.ClientVersion)
        {
            Debug.Assert(!(IsNullEmptyOrWhiteSpace(Path) || IsNullEmptyOrWhiteSpace(name)));

            ThrowIfNull(parent, nameof(parent));

            Name = name;

            _parent = parent;

            // todo: provide two constructors:

            _parentAssembly = parent is IDotNetItemInfo dotNetItemInfo ? dotNetItemInfo.ParentDotNetAssemblyInfo ?? throw new ArgumentException(string.Format(PropertyCannotBeNullOnObject, nameof(IDotNetItemInfo.ParentDotNetAssemblyInfo), nameof(dotNetItemInfo))) : parent is IDotNetAssemblyInfo dotNetAssemblyInfo ? dotNetAssemblyInfo : throw new ArgumentException(ParentMustBeAnIDotNetAssemblyInfoOrAnIDotNetItemInfoBase, nameof(parent));
        }

        protected abstract BitmapSource TryGetBitmapSource(in int size);

        protected override ArrayBuilder<IBrowsableObjectInfo> GetRootItemsOverride() => _defaultRootItems
#if CS8
            ??=
#else
            ?? (_defaultRootItems =
#endif
            FileSystemObjectInfo.GetRootItems()
#if !CS8
            )
#endif
            ;

        protected override System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> GetSubRootItemsOverride() => null;

        protected override void DisposeUnmanaged()
        {
            _parent = null;
            _parentAssembly = null;

            _bitmapSources = null;

            base.DisposeUnmanaged();
        }
    }

    public abstract class BrowsableDotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        protected sealed override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected BrowsableDotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, name, parent)
        {
            // Left empty.
        }

        protected override BitmapSource TryGetBitmapSource(in int size) => Shell.ObjectModel.BrowsableObjectInfo.TryGetBitmapSource(FolderIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);
    }
}
