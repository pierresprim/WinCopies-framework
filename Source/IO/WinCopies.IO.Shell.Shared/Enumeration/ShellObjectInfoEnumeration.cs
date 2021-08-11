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

using Microsoft.WindowsAPICodePack.COMNative.PortableDevices.PropertySystem;
using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Diagnostics;
using System.Linq;

using WinCopies.IO.AbstractionInterop;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Shell;
using WinCopies.Linq;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

#if WinCopies3
using static WinCopies.ThrowHelper;
#else
using System.Collections.Generic;

using WinCopies.Util;
#endif

namespace WinCopies.IO.Enumeration
{
    public static class ShellObjectInfoEnumeration
    {
        public static System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> From(ShellObjectInfo shellObjectInfo, Predicate<ShellObjectInfoEnumeratorStruct> func)
        {
            ThrowIfNull(shellObjectInfo, nameof(shellObjectInfo));

            System.Collections.Generic.IEnumerable<ShellObject> shellObjects;
            System.Collections.Generic.IEnumerable<IPortableDevice> portableDevices;

            shellObjects = (System.Collections.Generic.IEnumerable<ShellObject>)shellObjectInfo.InnerObjectGeneric;

            System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> getShellObjects(in System.Collections.Generic.IEnumerable<IPortableDevice> _portableDevices) => GetShellObjects(shellObjects, _portableDevices,  shellObjectInfo.ClientVersion, func);

            if (shellObjectInfo.InnerObjectGeneric.ParsingName == Computer.ParsingName)
            {
                var portableDeviceManager = new PortableDeviceManager();

                portableDeviceManager.GetDevices();

                portableDevices = portableDeviceManager.PortableDevices;

                //System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> getShellItems() =>
                // if (shellObjects == null) return GetPortableDevices(portableDevices, shellObjects, clientVersion, func);

                /*else*/
                //;

                ShellObjectInfoItemProvider getNewShellObjectInfoItemProvider(in NonShellObjectRootItemType nonShellObjectRootItemType) => new ShellObjectInfoItemProvider(nonShellObjectRootItemType,  shellObjectInfo.ClientVersion);

                return (portableDevices == null
                        ? getShellObjects(null)
                        : getShellObjects(portableDevices).AppendValues(GetPortableDevices(portableDevices, shellObjects,  shellObjectInfo.ClientVersion, func)))
                        .AppendValues(new ShellObjectInfoItemProvider(ShellObjectFactory.Create(RecycleBin.ParsingName),  shellObjectInfo.ClientVersion),
                        getNewShellObjectInfoItemProvider(NonShellObjectRootItemType.Registry),
                        getNewShellObjectInfoItemProvider(NonShellObjectRootItemType.WMI));
            }

            else return getShellObjects(null);
        }

        private static System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetShellObjects(in System.Collections.Generic.IEnumerable<ShellObject> shellObjects, System.Collections.Generic.IEnumerable<IPortableDevice> portableDevices,  ClientVersion clientVersion, Predicate<ShellObjectInfoEnumeratorStruct> func) => shellObjects.Where(item =>
       {
           if (portableDevices != null)

               foreach (IPortableDevice portableDevice in portableDevices)

                   if (item.ParsingName.EndsWith(portableDevice.DeviceId, StringComparison.InvariantCultureIgnoreCase))

                       return false;

           return func(new ShellObjectInfoEnumeratorStruct(item));

       }).Select(shellObject => new ShellObjectInfoItemProvider(shellObject,  clientVersion));

        private static bool PortableDevicePredicate(in IPortableDevice portableDevice, in ClientVersion clientVersion, in Predicate<ShellObjectInfoEnumeratorStruct> func)
        {
            Debug.Assert(portableDevice != null);
            Debug.Assert(func != null);

            //if (portableDevice.IsOpen)
            try
            {
                portableDevice.Open(clientVersion.ToPortableDeviceClientVersion(), new PortableDeviceOpeningOptions(Microsoft.WindowsAPICodePack.Win32Native.GenericRights.Read, Microsoft.WindowsAPICodePack.Win32Native.FileShareOptions.Read, false));

                ////#if DEBUG 

                ////                Queue<PropertyKey> queue = new Queue<PropertyKey>();

                ////                foreach (var key in portableDevice.Properties.Keys)

                ////                    queue.Enqueue(key);

                ////                string id=portableDevice.DeviceId;

                ////                if (count++>0&&portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Category, out Property _value))
                ////                {
                ////                    object ____value = _value.GetValue(out _);

                ////                    string guid = ((Guid)____value).ToString();

                ////                    string guid1 = Microsoft.WindowsAPICodePack.PortableDevices.Guids.PropertySystem.FunctionalCategory.Device;

                ////                    bool bool1 = guid.ToLower() == guid1.ToLower();

                ////                    string guid2 = Microsoft.WindowsAPICodePack.PortableDevices.Guids.PropertySystem.FunctionalCategory.Storage;

                ////                    bool bool2 = guid.ToLower() == guid2.ToLower();
                ////                }

                ////                bool value1 = portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Device.Type, out _value);

                ////                object ___value = _value.GetValue(out _);

                ////                bool value2 = (DeviceTypeValues)___value == DeviceTypeValues.Generic;

                ////                bool __value = !(value1 && value2);

                ////#endif

                return !(portableDevice.Properties.TryGetValue(Microsoft.WindowsAPICodePack.PortableDevices.PropertySystem.Properties.Device.Type, out Property value) && (DeviceTypeValues)value.GetValue(out Type _) == DeviceTypeValues.Generic) && func(new ShellObjectInfoEnumeratorStruct(portableDevice));
            }

            //throw new InvalidOperationException("The current portable device is not open.");

            catch (Exception)
            {
                return true;
            }
        }

        private static System.Collections.Generic.IEnumerable<ShellObjectInfoItemProvider> GetPortableDevices(in System.Collections.Generic.IEnumerable<IPortableDevice> portableDevices, System.Collections.Generic.IEnumerable<ShellObject> shellObjects,  ClientVersion clientVersion, Predicate<ShellObjectInfoEnumeratorStruct> func) => portableDevices.Where(item =>
{
    if (shellObjects != null)
    {
        foreach (ShellObject shellObject in shellObjects)

            if (shellObject.ParsingName.EndsWith(item.DeviceId, StringComparison.InvariantCultureIgnoreCase))

                return PortableDevicePredicate(item, clientVersion, func);

        return false;
    }

    return PortableDevicePredicate(item, clientVersion, func);

}).Select(portableDevice => new ShellObjectInfoItemProvider(portableDevice,  clientVersion));
    }
}
