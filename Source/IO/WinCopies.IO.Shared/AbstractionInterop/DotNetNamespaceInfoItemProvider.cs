/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.IO.ObjectModel;
using WinCopies.IO.Reflection;

namespace WinCopies.IO.AbstractionInterop.Reflection
{
    public class DotNetAssemblyInfoItemProvider
    {
        public string NamespaceName { get; }

        public TypeInfoItemProviderClass TypeInfoItemProvider { get; }

        public bool IsRootItem { get; }

        public IBrowsableObjectInfo Parent { get; }

        public class TypeInfoItemProviderClass
        {
            public TypeInfo Type { get; }

            public DotNetItemType ItemType { get; }

            public TypeInfoItemProviderClass(in TypeInfo type, DotNetItemType itemType)
            {
                Type = type;

                ItemType = itemType;
            }
        }

        private DotNetAssemblyInfoItemProvider(in bool isRootItem, in IBrowsableObjectInfo parent)
        {
            IsRootItem = isRootItem;

            Parent = parent;
        }

        public DotNetAssemblyInfoItemProvider(in string _namespace, in IBrowsableObjectInfo parent) : this(false, parent) => NamespaceName = _namespace;

        public DotNetAssemblyInfoItemProvider(in TypeInfoItemProviderClass typeInfoItemProvider, in IBrowsableObjectInfo parent) : this(false, parent) => TypeInfoItemProvider = typeInfoItemProvider;

        public DotNetAssemblyInfoItemProvider(in IBrowsableObjectInfo parent) : this(true, parent)
        {
            // Left empty.
        }
    }
}
