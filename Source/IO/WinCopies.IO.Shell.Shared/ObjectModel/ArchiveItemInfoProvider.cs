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

using WinCopies.IO.PropertySystem;

namespace WinCopies.IO.ObjectModel
{
    /// <summary>
    /// The base class for archive item info provider objects.
    /// </summary>
    public abstract class ArchiveItemInfoProvider<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : FileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems>, IArchiveItemInfoProvider<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
    {
        /// <summary>
        /// The parent <see cref="IShellObjectInfoBase"/> of this item. For <see cref="ShellObjectInfo"/> items, this property returns the current object.
        /// </summary>
        public abstract IShellObjectInfoBase ArchiveShellObject { get; }

        IShellObjectInfoBase IArchiveItemInfoProvider.ArchiveShellObject => ArchiveShellObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveItemInfoProvider{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/> class.
        /// </summary>
        /// <param name="path">The path of this <see cref="ArchiveItemInfoProvider{TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems}"/>.</param>
        /// <param name="clientVersion">The <see cref="ClientVersion"/> that will be used for <see cref="PortableDeviceInfo"/> and <see cref="PortableDeviceObjectInfo"/> initialization.</param>
        protected ArchiveItemInfoProvider(in string path, in ClientVersion clientVersion) : base(path, clientVersion) { /* Left empty. */ }
    }
}
