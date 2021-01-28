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

using System.Reflection;

using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Reflection.PropertySystem;
using WinCopies.IO.Selectors;

namespace WinCopies.IO
{
    namespace ObjectModel.Reflection
    {
        public interface IDotNetTypeInfoBase : IDotNetItemInfo<TypeInfo>
        {
            // Left empty.
        }

        public interface IDotNetTypeInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IDotNetTypeInfoBase, IDotNetItemInfo<TObjectProperties, TypeInfo, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IDotNetTypeInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    namespace ObjectModel.Reflection
    {
        public interface IDotNetTypeInfo : IDotNetTypeInfoProperties, IDotNetItemInfo, IEncapsulatorBrowsableObjectInfo<TypeInfo>
    namespace ObjectModel.Reflection
    {
        public interface IDotNetTypeInfo : IDotNetTypeInfoProperties, IDotNetItemInfo, IEncapsulatorBrowsableObjectInfo<TypeInfo>
        {
            // Left empty.
        }

        public interface IDotNetTypeInfo : IDotNetTypeInfo<IDotNetTypeInfoProperties, DotNetTypeInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetTypeInfoItemProvider>, DotNetTypeInfoItemProvider>
        {
            // Left empty.
        }
    }
}
