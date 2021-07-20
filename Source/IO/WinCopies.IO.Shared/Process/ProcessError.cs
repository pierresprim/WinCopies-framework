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

namespace WinCopies.IO.Process
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

        WrongStatus = 2,

        /// <summary>
        /// The process was aborted by user.
        /// </summary>
        CancelledByUser = 3,

        /// <summary>
        /// One part or all of the source or destination path was not found.
        /// </summary>
        PathNotFound = 4,

        /// <summary>
        /// The source or destination drive is not ready.
        /// </summary>
        DriveNotReady = 5,

        /// <summary>
        /// The source or destination path is read-protected.
        /// </summary>
        ReadProtection = 6,

        /// <summary>
        /// The destination path is read-protected.
        /// </summary>
        DestinationReadProtection = 7,

        /// <summary>
        /// The destination path is write-protected.
        /// </summary>
        WriteProtection = 8,

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
        /// The file could not be renamed.
        /// </summary>
        FileRenamingFailed = 12,

        /// <summary>
        /// The source and destination relative paths are equal.
        /// </summary>
        SourceAndDestPathAreEqual = 13,

        /// <summary>
        /// The destination path is a sub-path of the source path.
        /// </summary>
        DestPathIsASubPath = 14,

        /// <summary>
        /// An unknown disk error occurred.
        /// </summary>
        DiskError = 15,

        EncryptionFailed = 16,

        DirectoryNotEmpty=17,

        ItemIsNotDirectory=18,

        SharingViolation=19,

        FileReadOnly=20,

        AccessDenied=21
    }
}
