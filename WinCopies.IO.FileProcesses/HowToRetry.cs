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

// todo : orthographe de 'occured' ? + autres fichiers

namespace WinCopies.IO.FileProcesses
{
    public enum HowToRetry
    {

        // todo : ou none ? - autre place/valeur dans l'enum ? - ou autre nom ?

            /// <summary>
            /// Does not retry.
            /// </summary>
        None = 0,

        /// <summary>
        /// Ignores the current file and pass to the next item.
        /// </summary>
        Ignore = 1, 

        /// <summary>
        /// Retry without any modification on the process for the current file.
        /// </summary>
        Retry = 2,

        /// <summary>
        /// Renames the current file.
        /// </summary>
        Rename = 3,

        /// <summary>
        /// Replaces the file.
        /// </summary>
        Replace = 4, 

        Cancel = 5

    }
}
