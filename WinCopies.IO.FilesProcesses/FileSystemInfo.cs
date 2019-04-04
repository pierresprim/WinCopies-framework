﻿using System;
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
        public FileTypes FileType { get; set; } = FileTypes.None;

        public Exceptions _exception = Exceptions.None;

        /// <summary>
        /// Gets, if any, the exceptions occured with this <see cref="FileSystemInfo"/> when processing.
        /// </summary>
        public Exceptions Exception => _exception;

        /// <summary>
        /// Gets or sets a value that indicates what the file system process has to do for this file when an error has occured.
        /// </summary>
        public HowToRetry HowToRetryToProcess { get; set; } = HowToRetry.None;

        public FileSystemInfo(string path, FileTypes fileType) => Init(path, fileType);

        private void Init(string path, FileTypes fileType)

        {

            if (fileType != FileTypes.Folder && fileType != FileTypes.Drive && fileType != FileTypes.File) throw new ArgumentException("fileType must be Folder, Drive or File.");

            switch (fileType) 

            {

                case FileTypes.Folder:
                case FileTypes.Drive:

                    FileSystemInfoProperties = new DirectoryInfo(path);

                    break;

                case FileTypes.File:

                    FileSystemInfoProperties = new FileInfo(path);

                    break;

            }

            FileType = fileType;

        }

        public FileSystemInfo(System.IO.FileSystemInfo fileSystemInfo, FileTypes fileType)

        {

            if (fileType != FileTypes.Folder && fileType != FileTypes.Drive && fileType != FileTypes.File) throw new ArgumentException("fileType must be Folder, Drive or File.");

            if (((fileType == FileTypes.Folder || fileType == FileTypes.Drive) && fileSystemInfo.GetType() == typeof(FileInfo)) || (fileType == FileTypes.File && fileSystemInfo.GetType() == typeof(DirectoryInfo)))

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