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

using System;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

namespace WinCopies.IO.Selectors
{
    public class DotNetAssemblyInfoSelectorDictionary : BrowsableObjectInfoSelectorDictionary<DotNetAssemblyInfoItemProvider>
    {
        public static IBrowsableObjectInfo Convert(DotNetAssemblyInfoItemProvider item)
        {
            if (item.TypeInfoItemProvider != null)

                return new DotNetTypeInfo(item.TypeInfoItemProvider.Type, item.TypeInfoItemProvider.ItemType, true, item.Parent);

            if (item.NamespaceName != null)

                return new DotNetNamespaceInfo(item.NamespaceName,  );

            return item.PortableDevice == null
                ? throw new ArgumentException("The given item provider or its current configuration is not supported.")
                : new PortableDeviceInfo(item.PortableDevice, item.ClientVersion.Value);
        }

        protected override Converter<DotNetAssemblyInfoItemProvider, IBrowsableObjectInfo> DefaultSelectorOverride => Convert;

        public DotNetAssemblyInfoSelectorDictionary() { /* Left empty. */ }
    }
}
