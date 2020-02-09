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

using System.IO;

namespace WinCopies.IO.FileProcesses
{

    // todo:

    /// <summary>
    /// Représente les erreurs qui se produisent lorsque la destination d'un répertoire est égale à sa source lors d'un processus de copie ou de déplacement.
    /// </summary>
    public class DestinationEqualsSourceException : IOException
    {

        /// <summary>
        /// Répertoire de destination.
        /// </summary>
        public string Dest_Path { get; } = "";

        /// <summary>
        /// Le répertoire initial représentant le conflit.
        /// </summary>
        public string Path_With_Conflict { get; } = "";

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DestinationEqualsSourceException"/>.
        /// </summary>
        /// <param name="message">Message décrivant l'exception.</param>
        /// <param name="dest_path">Répertoire de destination.</param>
        /// <param name="path_with_conflict">Répertoire initial représentant le conflit.</param>
        public DestinationEqualsSourceException(string message, string dest_path, string path_with_conflict)
            : base(message)
        {

            Dest_Path = dest_path;

            Path_With_Conflict = path_with_conflict;

        }

    }

}
