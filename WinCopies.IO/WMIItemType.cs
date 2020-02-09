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

namespace WinCopies.IO

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

    /// <summary>
    /// Determines the WMI items to load.
    /// </summary>
    [Flags]
    public enum WMIItemTypes
    {

        /// <summary>
        /// Do not load any items.
        /// </summary>
        None = 0,

        /// <summary>
        /// Load the namespace items.
        /// </summary>
        Namespace = 1,

        /// <summary>
        /// Load the class items.
        /// </summary>
        Class = 2,

        /// <summary>
        /// Load the instance items.
        /// </summary>
        Instance = 4

    }

}