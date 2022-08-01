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

using System;
using System.Management;
using WinCopies.Util;

namespace WinCopies.IO
{

    public class WMIItemInfoFactory : BrowsableObjectInfoFactory, IWMIItemInfoFactory // where TItems : BrowsableObjectInfo, IWMIItemInfo
    {

        /// <summary>
        /// Gets a value that indicates whether this object needs to reconstruct objects on deep cloning.
        /// </summary>
        public override bool NeedsObjectsOrValuesReconstruction => !(Options is null);

        protected override BrowsableObjectInfoFactory DeepCloneOverride() => new WMIItemInfoFactory((WMIItemInfoFactoryOptions)Options?.DeepClone());

        private WMIItemInfoFactoryOptions _options;

        public WMIItemInfoFactoryOptions Options
        {

            get => _options; set
            {

                // ThrowOnInvalidPropertySet(this);

                _options.Factory = null;

                value.Factory = this;

                _options = value;

            }

        }

        IWMIItemInfoFactoryOptions IWMIItemInfoFactory.Options => _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfoFactory"/> class.
        /// </summary>
        public WMIItemInfoFactory() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIItemInfoFactory"/> class using custom options.
        /// </summary>
        public WMIItemInfoFactory(WMIItemInfoFactoryOptions options) : base() => _options = options;

        /// <summary>
        /// Gets a new instance of the <see cref="WMIItemInfo"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="WMIItemInfo"/> class.</returns>
        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo() => new WMIItemInfo();

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType) => GetBrowsableObjectInfo(path, wmiItemType, new ManagementObject(new ManagementScope(path, _options?.ConnectionOptions is null ? null : WMIItemInfo.DefaultConnectionOptionsDeepClone(_options?.ConnectionOptions, null)), new ManagementPath(path), _options?.ObjectGetOptions is null ? null : WMIItemInfo.DefaultObjectGetOptionsDeepClone(_options?.ObjectGetOptions)), _managementObject => _managementObject is ManagementClass managementClass ? WMIItemInfo.DefaultManagementClassDeepCloneDelegate(managementClass, null) : _managementObject is ManagementObject __managementObject ? WMIItemInfo.DefaultManagementObjectDeepClone(__managementObject, null) : throw new ArgumentException("The given object must be a ManagementClass or a ManagementObject.", "managementObject"));

        public virtual IBrowsableObjectInfo GetBrowsableObjectInfo(string path, WMIItemType wmiItemType, ManagementBaseObject managementObject, DeepClone<ManagementBaseObject> managementObjectDelegate) => new WMIItemInfo(path, wmiItemType, managementObject, managementObjectDelegate);
    }
}
