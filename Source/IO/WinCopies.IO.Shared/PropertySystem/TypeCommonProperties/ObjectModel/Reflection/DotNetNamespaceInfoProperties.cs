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

using WinCopies.IO.ObjectModel.Reflection;

namespace WinCopies.IO.Reflection.PropertySystem
{
    public interface IDotNetNamespaceInfoProperties : IDotNetItemInfoProperties
    {
        bool IsRootNamespace { get; }
    }

    public class DotNetNamespaceInfoProperties<T> : DotNetItemInfoProperties<T>, IDotNetNamespaceInfoProperties where T : IDotNetNamespaceInfoBase
    {
        public bool IsRootNamespace => BrowsableObjectInfo.IsRootNamespace;

        public DotNetNamespaceInfoProperties(T browsableObjectInfo) : base(browsableObjectInfo)
        {
            // Left empty.
        }
    }
}
