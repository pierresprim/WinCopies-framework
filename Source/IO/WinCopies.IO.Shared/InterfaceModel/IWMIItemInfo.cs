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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework. If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Management;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

namespace WinCopies.IO.ObjectModel
{
    public interface IWMIItemInfoBase : IBrowsableObjectInfo, IEncapsulatorBrowsableObjectInfo<ManagementBaseObject>
    {
        // Left empty.
    }

    public interface IWMIItemInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IWMIItemInfoBase, IBrowsableObjectInfo<TObjectProperties, ManagementBaseObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IWMIItemInfoProperties where TSelectorDictionary : IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        // Left empty.
    }

    public interface IWMIItemInfo : IWMIItemInfo<IWMIItemInfoProperties, ManagementBaseObject, IBrowsableObjectInfoSelectorDictionary<WMIItemInfoItemProvider>, WMIItemInfoItemProvider>
    {
        IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ManagementBaseObject> predicate);
    }
}
