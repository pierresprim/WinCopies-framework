﻿using System;

// todo : meilleur nom ? - FileAlreadyExists <=> -WithSameName ? Uniformiser le nom pour ces variables ? : Multiple_Files_With_Same_Name_And_Destination

namespace WinCopies.IO.FileProcesses
{

    /// <summary>
    /// Exception types can occur when file system objects processes.
    /// </summary>
    [Flags]
    public enum Exceptions
    {

        /// <summary>
        /// The process has completed successfully.
        /// </summary>
        None = 0,

        /// <summary>
        /// The process has encoutered an unknown exception.
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// The file couldn't be found.
        /// </summary>
        FileNotFound = 2,

        /// <summary>
        /// The path couldn't be found.
        /// </summary>
        PathNotFound = 4,

        AccessDenied = 8,

        DirectoryCannotBeRemoved = 16,

        WriteProtect = 32,

        ExceptionOnDeviceUnit = 64,

        DiskNotReady = 128,

        /// <summary>
        /// A file or a folder already exists with the same name.
        /// </summary>
        FileAlreadyExists = 256,

        // /// <summary>
        // /// The directory couldn't be found.
        // /// </summary>
        // DirectoryNotFound = 4,

        /// <summary>
        /// The file name is too long.
        /// </summary>
        FileNameTooLong = 512,

        /// <summary>
        /// The disk is full.
        /// </summary>
        DiskFull = 1024,

        InvalidName = 2048,

        FileCheckedOut = 4096,

        FileTooLarge = 8192,

        DiskTooFragmented = 16384,

        DeletePending = 32768,

        NotAllowedOnSystemFile = 65536,

        DestPathIsASubdirectory = 131072,

        NotEnoughSpaceOnDisk = 262144, 

        RequestCancelled = 524288






    }
}