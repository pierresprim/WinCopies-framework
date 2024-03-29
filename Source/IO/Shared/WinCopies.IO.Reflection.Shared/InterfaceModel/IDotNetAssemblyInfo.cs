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
using System.Collections.Generic;
using System.Reflection;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public interface IDotNetAssemblyInfo : IBrowsableObjectInfo<Shell.PropertySystem.IFileSystemObjectInfoProperties, Assembly, DotNetNamespaceInfoItemProvider, IEnumerableSelectorDictionary<DotNetNamespaceInfoItemProvider, IBrowsableObjectInfo>, DotNetNamespaceInfoItemProvider>, IShellObjectInfo<IFileSystemObjectInfoProperties, ShellObjectInfoEnumeratorStruct, IEnumerableSelectorDictionary<ShellObjectInfoItemProvider, IBrowsableObjectInfo>, ShellObjectInfoItemProvider>
    {
        IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func);
    }

    //public interface IDotNetAssemblyInfo<T> : IDotNetAssemblyInfo, IBrowsableObjectInfo<T, Assembly>, IShellObjectInfoBase<T> where T : IFileSystemObjectInfoProperties
    //{
    //    // Left empty.
    //}
}
