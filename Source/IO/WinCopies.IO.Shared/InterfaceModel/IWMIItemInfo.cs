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
 * along with the WinCopies Framework.If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Management;

namespace WinCopies.IO
{
    public interface IWMIItemInfoProperties
    {
        bool IsRootNode { get; }

        WMIItemType WMIItemType { get; }
    }

    namespace ObjectModel
    {
        public interface IWMIItemInfo : IWMIItemInfoProperties, IBrowsableObjectInfo, IEncapsulatorBrowsableObjectInfo<ManagementBaseObject>
        {
            IEnumerable<IBrowsableObjectInfo> GetItems(IWMIItemInfoFactory factory, Predicate<ManagementBaseObject> predicate, bool catchExceptionsDuringEnumeration);
        }

        public interface IWMIItemInfo<T> : IWMIItemInfo, IBrowsableObjectInfo<T, ManagementBaseObject> where T : IWMIItemInfoProperties
        {
            // Left empty.
        }
    }
}
