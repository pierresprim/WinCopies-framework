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
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Selectors
{
    public class PortableDeviceInfoSelectorDictionary : EnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(PortableDeviceObjectInfoItemProvider item) => new PortableDeviceObjectInfo((item ?? throw GetArgumentNullException(nameof(item))).PortableDeviceObject, item.ParentPortableDevice, item.ClientVersion);

        protected override Converter<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public PortableDeviceInfoSelectorDictionary() { /* Left empty. */ }
    }

    public class PortableDeviceObjectInfoSelectorDictionary : EnumerableSelectorDictionary<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo>
    {
        public static IBrowsableObjectInfo Convert(PortableDeviceObjectInfoItemProvider item) => new PortableDeviceObjectInfo((item ?? throw GetArgumentNullException(nameof(item))).PortableDeviceObject, item.ParentPortableDeviceObject, item.ClientVersion);

        protected override Converter<PortableDeviceObjectInfoItemProvider, IBrowsableObjectInfo> DefaultActionOverride => Convert;

        public PortableDeviceObjectInfoSelectorDictionary() { /* Left empty. */ }
    }
}
