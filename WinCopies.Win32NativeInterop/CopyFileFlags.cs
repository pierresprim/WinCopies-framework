﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.Win32NativeInterop
{
    /// <summary>
    /// Flags that specify how the file is to be copied.
    /// </summary>
    [System.Flags]
    public enum CopyFileFlags : uint
    {

        /// <summary>
        /// The copy operation fails immediately if the target file already exists.
        /// </summary>
        COPY_FILE_FAIL_IF_EXISTS = 0x00000001,

        /// <summary>
        /// Progress of the copy is tracked in the target file in case the copy fails. The failed copy can be restarted at a later time by specifying the same values for lpExistingFileName and lpNewFileName as those used in the call that failed. This can significantly slow down the copy operation as the new file may be flushed multiple times during the copy operation.
        /// </summary>
        COPY_FILE_RESTARTABLE = 0x00000002,

        /// <summary>
        /// The file is copied and the original file is opened for write access.
        /// </summary>
        COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,

        /// <summary>
        /// An attempt to copy an encrypted file will succeed even if the destination copy cannot be encrypted.
        /// </summary>
        COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,

        /// <summary>
        /// If the source file is a symbolic link, the destination file is also a symbolic link pointing to the same file that the source symbolic link is pointing to.
        ///
        /// Windows Server 2003 and Windows XP:  This value is not supported.
        /// </summary>
        COPY_FILE_COPY_SYMLINK = 0x00000800, //NT 6.0+

        /// <summary>
        /// The copy operation is performed using unbuffered I/O, bypassing system I/O cache resources. Recommended for very large file transfers.
        ///
        /// Windows Server 2003 and Windows XP:  This value is not supported.
        /// </summary>
        COPY_FILE_NO_BUFFERING = 0x00001000 //NT 6.0+
    }
}
