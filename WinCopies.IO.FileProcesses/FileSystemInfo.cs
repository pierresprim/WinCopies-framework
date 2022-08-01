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
using System.IO;

namespace WinCopies.IO.FileProcesses
{

    /// <summary>
    /// Provides information on file system items for file system processes.
    /// </summary>
    public class FileSystemInfo
    {

        /// <summary>
        /// Gets information for both <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> objects.
        /// </summary>
        public System.IO.FileSystemInfo FileSystemInfoProperties { get; private set; } = null;

        /// <summary>
        /// Gets the file type of this <see cref="FileSystemInfo"/> item.
        /// </summary>
        public FileType FileType { get; set; } = FileType.None;

        internal Exceptions _exception = Exceptions.None;

        /// <summary>
        /// Gets, if any, the exceptions occured with this <see cref="FileSystemInfo"/> when processing.
        /// </summary>
        public Exceptions Exception => _exception;

        /// <summary>
        /// Gets or sets a value that indicates what the file system process has to do for this file when an error has occured.
        /// </summary>
        public HowToRetry HowToRetryToProcess { get; set; } = HowToRetry.None;

        public FileSystemInfo(string path, FileType fileType) => Init(path, fileType);

        private void Init(string path, FileType fileType)

        {

            if (fileType != FileType.Folder && fileType != FileType.Drive && fileType != FileType.File) throw new ArgumentException("fileType must be Folder, Drive or File.");

            switch (fileType) 

            {

                case FileType.Folder:
                case FileType.Drive:

                    FileSystemInfoProperties = new DirectoryInfo(path);

                    break;

                case FileType.File:

                    FileSystemInfoProperties = new FileInfo(path);

                    break;

            }

            FileType = fileType;

        }

        public FileSystemInfo(System.IO.FileSystemInfo fileSystemInfo, FileType fileType)

        {

            if (fileType != FileType.Folder && fileType != FileType.Drive && fileType != FileType.File) throw new ArgumentException("fileType must be Folder, Drive or File.");

            if (((fileType == FileType.Folder || fileType == FileType.Drive) && fileSystemInfo.GetType() == typeof(FileInfo)) || (fileType == FileType.File && fileSystemInfo.GetType() == typeof(DirectoryInfo)))

                throw new ArgumentException("fileType must correspond with the type of fileSystemInfo.");

            FileSystemInfoProperties = fileSystemInfo;

            FileType = fileType;

        }

        //public FileSystemInfo(string path, FileTypes fileType)

        //{

        //    // FileTypes fileType = FileTypes.None;

        //    if (fileType == FileTypes.Folder || fileType == FileTypes.Drive)

        //    {

        //        var d = new DirectoryInfo(path);

        //        // if (d.Root.FullName == path) fileType = FileTypes.Drive;

        //        // else fileType = FileTypes.Folder;

        //        FileSystemInfoProperties = d;

        //    }

        //    else

        //        // fileType = FileTypes.File;

        //        FileSystemInfoProperties = new FileInfo(path); 

        //    Init(path, fileType);

        //}

    }

}
