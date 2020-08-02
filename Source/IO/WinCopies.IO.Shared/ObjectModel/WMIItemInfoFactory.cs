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

using System.Management;

namespace WinCopies.IO.ObjectModel
{
    public interface IWMIItemInfoFactory
    {
        IWMIItemInfoFactoryOptions Options { get; }

        /// <summary>
        /// Gets a new instance of the <see cref="IBrowsableObjectInfo"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="IBrowsableObjectInfo"/> class.</returns>
        IBrowsableObjectInfo GetBrowsableObjectInfo();

        IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType, ManagementBaseObject managementObject);

        IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType);
    }

    public class WMIItemInfoFactory : IWMIItemInfoFactory 
    {
        public WMIItemInfoFactoryOptions Options { get; set; }

        IWMIItemInfoFactoryOptions IWMIItemInfoFactory.Options => Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfoFactory"/> class.
        /// </summary>
        public WMIItemInfoFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfoFactory"/> class using custom options.
        /// </summary>
        public WMIItemInfoFactory(WMIItemInfoFactoryOptions options) : base() => Options = options;

        /// <summary>
        /// Gets a new instance of the <see cref="WMIItemInfo"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="WMIItemInfo"/> class.</returns>
        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo() => new WMIItemInfo();

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType) => GetBrowsableObjectInfo(path, wmiItemType, new ManagementObject(new ManagementScope(path, Options?.ConnectionOptions is null ? null : Options?.ConnectionOptions), new ManagementPath(path), Options?.ObjectGetOptions is null ? null : Options?.ObjectGetOptions));

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType, ManagementBaseObject managementObject) => new WMIItemInfo(path, wmiItemType, managementObject);
    }

    public interface IWMIItemInfoFactoryOptions
    {
        ConnectionOptions ConnectionOptions { get; set; }

        ObjectGetOptions ObjectGetOptions { get; set; }

        EnumerationOptions EnumerationOptions { get; set; }
    }

    public class WMIItemInfoFactoryOptions : IWMIItemInfoFactoryOptions
    {
        public ConnectionOptions ConnectionOptions { get; set; }

        public ObjectGetOptions ObjectGetOptions { get; set; }

        public EnumerationOptions EnumerationOptions { get; set; }

        public WMIItemInfoFactoryOptions() { }

        public WMIItemInfoFactoryOptions(ConnectionOptions connectionOptions, ObjectGetOptions objectGetOptions, EnumerationOptions enumerationOptions)
        {
            ConnectionOptions = connectionOptions;

            ObjectGetOptions = objectGetOptions;

            EnumerationOptions = enumerationOptions;
        }
    }
}
