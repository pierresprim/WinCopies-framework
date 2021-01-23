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

using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;

using static WinCopies.UtilHelpers;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public abstract class DotNetItemInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : BrowsableObjectInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IDotNetItemInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        #region Properties
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
        public override string Description => NotApplicable;

        /// <summary>
        /// Gets the <see langword="false"/> value.
        /// </summary>
        public override bool IsSpecialItem => false;

        public sealed override IBrowsableObjectInfo Parent { get; }

        public IDotNetAssemblyInfo ParentDotNetAssemblyInfo { get; }

        public sealed override bool IsRecursivelyBrowsable => IsBrowsable;

        #region BitmapSources
        protected abstract BitmapSource TryGetBitmapSource(in int size);

        private BitmapSource _smallBitmapSource;
        private BitmapSource _mediumBitmapSource;
        private BitmapSource _largeBitmapSource;
        private BitmapSource _extraLargeBitmapSource;

        public sealed override BitmapSource SmallBitmapSource => _smallBitmapSource
#if CS7
            ?? (_smallBitmapSource =
#else
            ??=
#endif
            TryGetBitmapSource(SmallIconSize)
#if CS7
            )
#endif
            ;

        public sealed override BitmapSource MediumBitmapSource => _mediumBitmapSource
#if CS7
            ?? (_mediumBitmapSource =
#else
            ??=
#endif
            TryGetBitmapSource(MediumIconSize)
#if CS7
            )
#endif
            ;

        public sealed override BitmapSource LargeBitmapSource => _largeBitmapSource
#if CS7
            ?? (_largeBitmapSource =
#else
            ??=
#endif
            TryGetBitmapSource(LargeIconSize)
#if CS7
            )
#endif
            ;

        public sealed override BitmapSource ExtraLargeBitmapSource => _extraLargeBitmapSource
#if CS7
            ?? (_extraLargeBitmapSource =
#else
            ??=
#endif
            TryGetBitmapSource(ExtraLargeIconSize)
#if CS7
            )
#endif
            ;
        #endregion
        #endregion

        protected DotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, parent.ClientVersion)
        {
            Debug.Assert(!(IsNullEmptyOrWhiteSpace(Path) || IsNullEmptyOrWhiteSpace(name)));

            ThrowIfNull(parent, nameof(parent));

            Name = name;

            Parent = parent;

            // todo: provide two constructors:

            ParentDotNetAssemblyInfo = parent is IDotNetItemInfoBase dotNetItemInfo ? dotNetItemInfo.ParentDotNetAssemblyInfo ?? throw new ArgumentException($"{nameof(dotNetItemInfo)} has no parent IDotNetAssemblyInfo.") : parent is IDotNetAssemblyInfo dotNetAssemblyInfo ? dotNetAssemblyInfo : throw new ArgumentException($"{nameof(parent)} must be an {nameof(IDotNetAssemblyInfo)} or an {nameof(IDotNetItemInfoBase)}.", nameof(parent));
        }
    }

    public abstract class BrowsableDotNetItemInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : DotNetItemInfo<TObjectProperties, TEncapsulatedObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        public sealed override bool IsBrowsable => true;

        protected BrowsableDotNetItemInfo(in string path, in string name, in IBrowsableObjectInfo parent) : base(path, name, parent)
        {
            // Left empty.
        }

        protected sealed override BitmapSource TryGetBitmapSource(in int size) => TryGetBitmapSource(FolderIcon, Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Shell32, size);
    }
}
