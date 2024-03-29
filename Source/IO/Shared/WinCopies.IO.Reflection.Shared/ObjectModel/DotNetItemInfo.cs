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

#region Usings
#region System
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
#endregion System

#region WinCopies
using WinCopies.Collections.Generic;
using WinCopies.IO.Process;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Shell;
#endregion WinCopies

#region Static Usings
using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;
using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using WinCopies.IO.ComponentSources.Bitmap;
#endregion Static Usings
#endregion Usings

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<IBrowsableObjectInfo, TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        private static ArrayBuilder<IBrowsableObjectInfo> _defaultRootItems;
        private IBrowsableObjectInfo _parent;
        private IBitmapSourceProvider _bitmapSourceProvider;

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
        protected override string DescriptionOverride => WinCopies.Consts.NotApplicable;

        /// <summary>
        /// Gets the <see langword="false"/> value.
        /// </summary>
        protected override bool IsSpecialItemOverride => false;

        protected sealed override IBrowsableObjectInfo ParentGenericOverride => _parent;

        public IDotNetAssemblyInfo ParentDotNetAssemblyInfo { get; private set; }

        protected sealed override bool IsRecursivelyBrowsableOverride => IsBrowsable;

        protected virtual IBitmapSources BitmapSources => Icons.File.Instance;

        protected override IBitmapSourceProvider BitmapSourceProviderOverride => _bitmapSourceProvider
#if CS8
            ??=
#else
            ?? (_bitmapSourceProvider =
#endif
            new Shell.ComponentSources.Bitmap.BitmapSourceProvider(this, BitmapSources, true)
#if !CS8
            )
#endif
            ;

        protected override System.Collections.Generic.IEnumerable<IBrowsabilityPath>
#if CS8
            ?
#endif
            BrowsabilityPathsOverride => null;
        #endregion

        protected DotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, parent.ClientVersion)
        {
            Debug.Assert(!(IsNullEmptyOrWhiteSpace(Path) || IsNullEmptyOrWhiteSpace(name)));

            ThrowIfNull(parent, nameof(parent));

            Name = name;

            _parent = parent;

            // todo: provide two constructors:

            ParentDotNetAssemblyInfo = parent is IDotNetItemInfo dotNetItemInfo ? dotNetItemInfo.ParentDotNetAssemblyInfo ?? throw new ArgumentException(string.Format(PropertyCannotBeNullOnObject, nameof(IDotNetItemInfo.ParentDotNetAssemblyInfo), nameof(dotNetItemInfo))) : parent is IDotNetAssemblyInfo dotNetAssemblyInfo ? dotNetAssemblyInfo : throw new ArgumentException(ParentMustBeAnIDotNetAssemblyInfoOrAnIDotNetItemInfoBase, nameof(parent));
        }

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
            ParentDotNetAssemblyInfo = null;

            _bitmapSourceProvider.Dispose();
            _bitmapSourceProvider = null;

            base.DisposeUnmanaged();
        }
    }

    public abstract class BrowsableDotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        protected sealed override IBrowsabilityOptions BrowsabilityOverride => BrowsabilityOptions.BrowsableByDefault;

        protected override IBitmapSources BitmapSources => Icons.Folder.Instance;

        protected BrowsableDotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, name, parent) { /* Left empty. */ }
    }
}
