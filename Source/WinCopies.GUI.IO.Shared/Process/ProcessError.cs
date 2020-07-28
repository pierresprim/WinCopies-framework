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

namespace WinCopies.GUI.IO.Process
{
    public enum ProcessError : byte
    {
        /// <summary>
        /// No error occurred.
        /// </summary>
        None = 0,

        /// <summary>
        /// An unknown error occurred.
        /// </summary>
        UnknownError = 1,

        /// <summary>
        /// The process was aborted by user.
        /// </summary>
        AbortedByUser = 2,

        /// <summary>
        /// One part or all of the source or destination path was not found.
        /// </summary>
        PathNotFound = 3,

        /// <summary>
        /// The source or destination drive is not ready.
        /// </summary>
        DriveNotReady = 4,

        /// <summary>
        /// The source path is read-protected.
        /// </summary>
        ReadProtection = 5,

        /// <summary>
        /// The destination path is read-protected.
        /// </summary>
        DestinationReadProtection = 6,

        /// <summary>
        /// The destination path is write-protected.
        /// </summary>
        WriteProtection = 7,

        /// <summary>
        /// The source or destination path cannot be accessed.
        /// </summary>
        AccessDenied = 8,

        /// <summary>
        /// The destination path is too long.
        /// </summary>
        PathTooLong = 9,

        /// <summary>
        /// The destination disk has not enough space.
        /// </summary>
        NotEnoughSpace = 10,

        /// <summary>
        /// A file or folder already exists with the same name.
        /// </summary>
        FileSystemEntryAlreadyExists = 11,

        /// <summary>
        /// A folder already exists with the same name.
        /// </summary>
        FolderAlreadyExists = 12,

        /// <summary>
        /// The file could not be renamed.
        /// </summary>
        FileRenamingFailed = 13,

        /// <summary>
        /// The source and destination relative paths are equal.
        /// </summary>
        SourceAndDestPathAreEqual = 14,

        /// <summary>
        /// The destination path is a sub-path of the source path.
        /// </summary>
        DestPathIsASubPath = 15,

        /// <summary>
        /// An unknown disk error occurred.
        /// </summary>
        DiskError = 16,

        EncryptionFailed = 17
    }
}
