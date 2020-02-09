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

namespace WinCopies.IO.FileProcesses
{

    /// <summary>
    /// Énumération servant à décrire le type de processus de fichiers en cours
    /// </summary>
    public enum ActionType
    {

        /// <summary>
        /// The process is not determinated.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The process is a copy.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The process is a file move.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The process is a file move to the Windows Recycle bin.
        /// </summary>
        Recycling = 3,

        // /// <summary>
        // /// Le processus de fichiers est une suppression. Si le processus marqué comme Deletion plutôt que comme Moving_To_Recycle_Bin ( valeur 2 ), cela signifie que les éléments marqués à supprimer sont effacés définitivement du système.
        // /// 
        // /// Néanmoins, une récupération peut être possible dans certains cas.
        // /// </summary>

        /// <summary>
        /// The process is a deletion.
        /// </summary>
        Deletion = 4,

        // /// <summary>
        // /// Le processus est une recherche de fichiers et dossiers dont certaines caractéristiques correspondent à celles passées au processus de recherche et définient généralement par l'utilisateur.
        // /// </summary>

        // /// <summary>
        // /// The process is a file search.
        // /// </summary>
        // Search = 5

    }

}
