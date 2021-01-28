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

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.PropertySystem
{
    /// <summary>
    /// The Windows registry item type.
    /// </summary>
    public enum RegistryItemType
    {
        /// <summary>
        /// The current instance represents the Windows registry root node.
        /// </summary>
        Root,

        /// <summary>
        /// The current instance represents a Windows registry key.
        /// </summary>
        Key,

        /// <summary>
        /// The current instance represents a Windows registry value.
        /// </summary>
        Value
    }

    public interface IRegistryItemInfoProperties
    {
        RegistryItemType RegistryItemType { get; }
    }

    public class RegistryItemInfoProperties<T> : BrowsableObjectInfoProperties<T>, IRegistryItemInfoProperties where T : IRegistryItemInfoBase
    {
        public RegistryItemType RegistryItemType { get; }

        public RegistryItemInfoProperties(in T browsableObjectInfo, in RegistryItemType registryItemType) : base(browsableObjectInfo) => RegistryItemType = registryItemType;
    }
}
