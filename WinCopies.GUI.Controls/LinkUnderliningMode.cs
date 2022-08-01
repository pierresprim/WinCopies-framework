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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Values that describe how to underline the links.
    /// </summary>
    public enum LinkUnderliningMode 
    {

        /// <summary>
        /// No underline.
        /// </summary>
        None = 0,

        /// <summary>
        /// Underline only when the mouse is over the link, or when the link is focused.
        /// </summary>
        UnderlineWhenMouseOverOrFocused = 1,

        /// <summary>
        /// Underline only when the mouse is NOT over the link and when the link is NOT focused.
        /// </summary>
        UnderlineWhenNotMouseOverNorFocused = 2,

        /// <summary>
        /// Always underline.
        /// </summary>
        AlwaysUnderline = 3

    }
}
