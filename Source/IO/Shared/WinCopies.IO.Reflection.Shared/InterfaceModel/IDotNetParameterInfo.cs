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

using System.Reflection;

using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Reflection.PropertySystem;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetParameterInfoBase : IDotNetItemInfo<ParameterInfo>
    {
        // Left empty.
    }

    public interface IDotNetParameterInfo<TObjectProperties, TSelectorDictionary> : IDotNetParameterInfoBase, IDotNetItemInfo<TObjectProperties, ParameterInfo, CustomAttributeData, TSelectorDictionary, DotNetParameterInfoItemProvider> where TObjectProperties : IDotNetParameterInfoProperties where TSelectorDictionary : IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>
    {
        // Left empty.
    }

    public interface IDotNetParameterInfo : IDotNetParameterInfo<IDotNetParameterInfoProperties, IEnumerableSelectorDictionary<DotNetParameterInfoItemProvider, IBrowsableObjectInfo>>
    {
        // Left empty.
    }
}
