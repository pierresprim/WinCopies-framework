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

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.PropertySystem
{
    /// <summary>
    /// The WMI item type.
    /// </summary>
    public enum WMIItemType
    {
        /// <summary>
        /// The WMI item is a namespace.
        /// </summary>
        Namespace,

        /// <summary>
        /// The WMI item is a class.
        /// </summary>
        Class,

        /// <summary>
        /// The WMI item is an instance.
        /// </summary>
        Instance
    }

    public interface IWMIItemInfoProperties
    {
        WMIItemType ItemType { get; }

        bool IsRootNode { get; }

        IWMIItemInfoOptions Options { get; }
    }

    public class WMIItemInfoProperties : BrowsableObjectInfoProperties<IWMIItemInfoBase>, IWMIItemInfoProperties
    {
        public WMIItemType ItemType { get; }

        public bool IsRootNode { get; }

        public IWMIItemInfoOptions Options { get; }

        public WMIItemInfoProperties(in IWMIItemInfoBase browsableObjectInfo, in WMIItemType itemType, in bool isRootNode, in IWMIItemInfoOptions options) : base(browsableObjectInfo)
        {
            ItemType = itemType;

            IsRootNode = isRootNode;

            Options = options;
        }
    }
}
