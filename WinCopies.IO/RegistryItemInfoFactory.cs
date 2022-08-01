/* Copyright © Pierre Sprimont, 2019
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

using Microsoft.Win32;
using System;
using WinCopies.Util;

namespace WinCopies.IO
{

    public class RegistryItemInfoFactory : BrowsableObjectInfoFactory, IRegistryItemInfoFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryItemInfoFactory"/> class.
        /// </summary>
        public RegistryItemInfoFactory() : base() { }

        protected override BrowsableObjectInfoFactory DeepCloneOverride() => new RegistryItemInfoFactory();

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo() => new RegistryItemInfo();

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(RegistryKey registryKey, DeepClone<RegistryKey> registryKeyDelegate) => new RegistryItemInfo(registryKey, registryKeyDelegate);

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string registryKeyPath) => new RegistryItemInfo(registryKeyPath);

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(RegistryKey registryKey, DeepClone<RegistryKey> registryKeyDelegate, string valueName) => new RegistryItemInfo(registryKey, registryKeyDelegate, valueName);

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string registryKeyPath, string valueName) => new RegistryItemInfo(registryKeyPath, valueName);

    }

}
