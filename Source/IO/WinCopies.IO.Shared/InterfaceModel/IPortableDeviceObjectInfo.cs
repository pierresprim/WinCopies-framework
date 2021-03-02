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

using Microsoft.WindowsAPICodePack.PortableDevices;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Selectors;

namespace WinCopies.IO.ObjectModel
{
    public interface IPortableDeviceObjectInfoBase : IFileSystemObjectInfo, IEncapsulatorBrowsableObjectInfo<IPortableDeviceObject>
    {
        // Left empty.
    }

    public interface IPortableDeviceObjectInfo<TObjectProperties, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> : IPortableDeviceObjectInfoBase, IFileSystemObjectInfo<TObjectProperties, IPortableDeviceObject, TPredicateTypeParameter, TSelectorDictionary, TDictionaryItems> where TObjectProperties : IFileSystemObjectInfoProperties where TSelectorDictionary:IBrowsableObjectInfoSelectorDictionary<TDictionaryItems>
    {
        // Left empty.
    }

    public interface IPortableDeviceObjectInfo: IPortableDeviceObjectInfo<IFileSystemObjectInfoProperties, IPortableDeviceObject, IBrowsableObjectInfoSelectorDictionary<PortableDeviceObjectInfoItemProvider>, PortableDeviceObjectInfoItemProvider>
    {
        // Left empty.
    }
}
