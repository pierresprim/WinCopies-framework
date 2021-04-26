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

using System.Drawing;
using System.Windows.Media.Imaging;

using WinCopies.IO.PropertySystem;

namespace WinCopies.IO
{
    // public interface IFileSystemObjectInfoFactory : IBrowsableObjectInfoFactory { }

    namespace ObjectModel
    {
        public interface IFileSystemObjectInfo : IBrowsableObjectInfo
        {
            Icon TryGetIcon(in int size);

            BitmapSource TryGetBitmapSource(in int size);
        }

        public interface IFileSystemObjectInfo<T> : IFileSystemObjectInfo, IBrowsableObjectInfo<T> where T : IFileSystemObjectInfoProperties
        {
            // Left empty.
        }

        public interface IFileSystemObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IFileSystemObjectInfo<TObjectProperties>, IBrowsableObjectInfo<TObjectProperties, TInnerObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<TDictionaryItems, IBrowsableObjectInfo>
        {
            // Left empty.
        }
    }
}
