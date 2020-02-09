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

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WinCopies.IO.FileProcesses
{

    // todo: to put in WinCopies.IO

    public enum GenericAccessRights : uint

    {

        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000

    }

    public enum FileFlags : uint

    {

        /// <summary>
        /// <para>The file is being opened or created for a backup or restore operation. The system ensures that the calling process overrides file security checks when the process has SE_BACKUP_NAME and SE_RESTORE_NAME privileges. For more information, see the 'Changing Privileges in a Token' article at this page: https://msdn.microsoft.com/b8e47d04-07c1-4d57-8209-6b0c397476e5 .</para>
        ///
        /// <para>You must set this flag to obtain a handle to a directory. A directory handle can be passed to some functions instead of a file handle. For more information, see the Remarks section.</para>
        /// </summary>
        FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,

        /// <summary>
        /// <para>The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated handles.</para>
        /// <para>If there are existing open handles to a file, the call fails unless they were all opened with the FILE_SHARE_DELETE share mode.</para>
        ///
        /// <para>Subsequent open requests for the file fail, unless the FILE_SHARE_DELETE share mode is specified.</para>
        /// </summary>
        FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,

        /// <summary>
        /// <para>The file or device is being opened with no system caching for data reads and writes. This flag does not affect hard disk caching or memory mapped files.</para>
        ///
        /// <para>There are strict requirements for successfully working with files opened with CreateFile using the FILE_FLAG_NO_BUFFERING flag, for details see the 'File Buffering' article at this page: https://msdn.microsoft.com/ae1e5d0f-9b55-4aae-8402-b9c8e33d9363 .</para>
        /// </summary>
        FILE_FLAG_NO_BUFFERING = 0x20000000,

        /// <summary>
        /// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. This flag is for use by remote storage systems.
        /// </summary>
        FILE_FLAG_OPEN_NO_RECALL = 0x00100000,

        /// <summary>
        /// <para>Normal reparse point ( https://msdn.microsoft.com/3abb3a08-9a00-43eb-9792-82eab1a25f06 ) processing will not occur; CreateFile will attempt to open the reparse point. When a file is opened, a file handle is returned, whether or not the filter that controls the reparse point is operational.</para>
        ///
        /// <para>This flag cannot be used with the <see cref="FileMode.Create"/> flag.</para>
        ///
        /// <para>If the file is not a reparse point, then this flag is ignored.</para>
        ///
        /// <para>For more information, see the Remarks section.</para>
        /// </summary>
        FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x0100000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SESSION_AWARE = 0x00800000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
        FILE_FLAG_WRITE_THROUGH = 0x80000000

    }

    public enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    } 

    public class UnmanagedFileLoader
    {

        public const short INVALID_HANDLE_VALUE = -1;

        // Use interop to call the CreateFile function.
        // For more information about CreateFile,
        // see the unmanaged MSDN reference library.

        /// <summary>
        /// <para>Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe. The function returns a handle that can be used to access the file or device for various types of I/O depending on the file or device and the flags and attributes specified.</para>
        ///
        /// <para>To perform this operation as a transacted operation, which results in a handle that can be used for transacted I/O, use the CreateFileTransacted function.</para>
        /// </summary>
        /// <param name="lpFileName"><para>The name of the file or device to be created or opened. You may use either forward slashes (/) or backslashes (\) in this name.</para>
        ///
        /// <para>In the ANSI version of this function, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\?" to the path.For more information, see the 'Naming Files, Paths, and Namespaces' article at this page: https://msdn.microsoft.com/121cd5b2-e6fd-4eb4-99b4-b652d27b53e8 .</para>
        ///
        /// <para>For information on special device names, see the 'Defining an MS-DOS Device Name' article at this page: https://msdn.microsoft.com/7d802e9f-dc09-4e3d-b064-e9b57af396e2 .</para>
        ///
        /// <para>To create a file stream, specify the name of the file, a colon, and then the name of the stream.For more information, see the 'File Streams' article at this page: https://msdn.microsoft.com/41dda6f1-a6d1-4e76-94f3-a72f9e491bee .</para>
        /// <para>Tip Starting with Windows 10, version 1607, for the unicode version of this function (CreateFileW), you can opt-in to remove the MAX_PATH limitation without prepending "\\?\". See the "Maximum Path Length Limitation" section of Naming Files, Paths, and Namespaces for details.</para></param>
        /// <param name="dwDesiredAccess"><para>The requested access to the file or device, which can be summarized as read, write, both or neither zero).</para>
        ///
        /// <para>The most commonly used values are <see cref="GenericAccessRights.GENERIC_READ"/>, <see cref="GenericAccessRights.GENERIC_WRITE"/>, or both (<see cref="GenericAccessRights.GENERIC_READ"/> | <see cref="GenericAccessRights.GENERIC_WRITE"/>). For more information, see the 'Generic Access Rights' article at this page: https://msdn.microsoft.com/e18cede9-9bf7-4866-850b-5d7fa43a5b0f , the 'File Security and Access Rights' article at this page: https://msdn.microsoft.com/991d7d94-fae7-406f-b2e3-dee811279366 , the 'File Access Rights Constants' article at this page: https://msdn.microsoft.com/c534e853-b61f-414d-befe-8d3c4bf08d22 , and the 'ACCESS_MASK' article at this page: https://msdn.microsoft.com/f115ee54-3333-4109-8004-d71904a7a943 .</para>
        ///
        /// <para>If this parameter is zero, the application can query certain metadata such as file, directory, or device attributes without accessing that file or device, even if <see cref="GenericAccessRights.GENERIC_READ"/> access would have been denied.</para>
        ///
        /// <para>You cannot request an access mode that conflicts with the sharing mode that is specified by the dwShareMode parameter in an open request that already has an open handle.</para>
        ///
        /// <para>For more information, see the Remarks section of this topic and the 'Creating and Opening Files' article at this page: https://msdn.microsoft.com/f115ee54-3333-4109-8004-d71904a7a943 .</para></param>
        /// <param name="dwShareMode"><para>The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none (refer to the following table). Access requests to attributes or extended attributes are not affected by this flag.</para>
        ///
        /// <para>If this parameter is zero and CreateFile succeeds, the file or device cannot be shared and cannot be opened again until the handle to the file or device is closed.For more information, see the Remarks section.</para>
        ///
        /// <para>You cannot request a sharing mode that conflicts with the access mode that is specified in an existing request that has an open handle. CreateFile would fail and the <see cref="Marshal.GetLastWin32Error"/> function would return <see cref="WinCopies.IO.FilesProcesses.NativeWin32FilesProcessesErrorCodes.ERROR_SHARING_VIOLATION"/>.</para>
        ///
        /// <para>To enable a process to share a file or device while another process has the file or device open, use a compatible combination of one or more of the following values. For more information about valid combinations of this parameter with the dwDesiredAccess parameter, see the 'Creating and Opening Files' article at this page: https://msdn.microsoft.com/094cac29-c66d-409e-8928-878dc693d393 .</para>
        ///
        /// <para>Note The sharing options for each open handle remain in effect until that handle is closed, regardless of process context.</para></param>
        /// <param name="lpSecurityAttributes"><para>A pointer to a SECURITY_ATTRIBUTES structure that contains two separate but related data members: an optional security descriptor, and a Boolean value that determines whether the returned handle can be inherited by child processes.</para>
        ///
        /// <para>This parameter can be NULL.</para>
        ///
        /// <para>If this parameter is NULL, the handle returned by CreateFile cannot be inherited by any child processes the application may create and the file or device associated with the returned handle gets a default security descriptor.</para>
        ///
        /// <para>The lpSecurityDescriptor member of the structure specifies a SECURITY_DESCRIPTOR for a file or device. If this member is NULL, the file or device associated with the returned handle is assigned a default security descriptor.</para>
        ///
        /// <para>CreateFile ignores the lpSecurityDescriptor member when opening an existing file or device, but continues to use the bInheritHandle member.</para>
        ///
        /// <para>The bInheritHandlemember of the structure specifies whether the returned handle can be inherited.</para>
        ///
        /// <para>For more information, see the Remarks section.</para></param>
        /// <param name="dwCreationDisposition"><para>An action to take on a file or device that exists or does not exist.</para>
        ///
        /// <para>For devices other than files, this parameter is usually set to <see cref="FileMode.Open"/>.</para>
        ///
        /// <para>For more information, see the Remarks section.</para>
        ///
        /// <para>This parameter must be one of the following values, which cannot be combined:</para></param>
        /// <param name="dwFlagsAndAttributes"><para>The file or device attributes and flags, <see cref="FileAttributes.Normal"/> being the most common default value for files.</para>
        ///
        /// <para>This parameter can include any combination of the available file attributes (FileAttributes.*). All other file attributes override <see cref="FileAttributes.Normal"/>.</para>
        ///
        /// <para>This parameter can also contain combinations of flags (FILE_FLAG_) for control of file or device caching behavior, access modes, and other special-purpose flags. These combine with any <see cref="FileAttributes"/> values.</para>
        ///
        /// <para>This parameter can also contain Security Quality of Service (SQOS) information by specifying the SECURITY_SQOS_PRESENT flag. Additional SQOS-related flags information is presented in the table following the attributes and flags tables.</para>
        /// 
        /// <para>Note When CreateFile opens an existing file, it generally combines the file flags with the file attributes of the existing file, and ignores any file attributes supplied as part of dwFlagsAndAttributes.Special cases are detailed in Creating and Opening Files.</para>
        ///
        /// <para>Some of the following file attributes and flags may only apply to files and not necessarily all other types of devices that CreateFile can open. For additional information, see the Remarks section of this topic and 'Creating and the Opening Files' article at this page: https://msdn.microsoft.com/094cac29-c66d-409e-8928-878dc693d393 .</para>
        ///
        /// <para>For more advanced access to file attributes, see the 'SetFileAttributes' article at this page: https://msdn.microsoft.com/3d5400c3-555f-44fc-9470-52a36d04d90b . For a complete list of all file attributes with their values and descriptions, see the 'File Attribute Constants' article at this page: https://msdn.microsoft.com/ed9a73d2-7fb6-4fb7-97f6-4dbf89e2f156 .</para>
        /// 
        /// <para>The dwFlagsAndAttributes parameter can also specify SQOS information. For more information, see the 'Impersonation Levels' article at this page: https://msdn.microsoft.com/ae152dbf-44f0-417f-a85e-09bf60dcfcb0 . When the calling application specifies the SECURITY_SQOS_PRESENT flag as part of dwFlagsAndAttributes, it can also contain one or more of the following values.</para></param>
        /// <param name="hTemplateFile"><para>A valid handle to a template file with the GENERIC_READ access right. The template file supplies file attributes and extended attributes for the file that is being created.</para>
        ///
        /// <para>This parameter can be NULL.</para>
        ///
        /// <para>When opening an existing file, CreateFile ignores this parameter.</para>
        ///
        /// <para>When opening a new encrypted file, the file inherits the discretionary access control list from its parent directory.For additional information, see File Encryption.</para></param>
        /// <returns><para>If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.</para>
        ///
        /// <para>If the function fails, the return value is <see cref="INVALID_HANDLE_VALUE"/>. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);



        private SafeFileHandle handleValue = null;


        public UnmanagedFileLoader(string Path) => Load(Path);


        public void Load(string Path)
        {
            
            if (Path == null || Path.Length == 0)

                throw new ArgumentNullException("Path");

            // Try to open the file.
            handleValue = CreateFile(Path, (uint) GenericAccessRights. GENERIC_READ, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            // If the handle is invalid,
            // get the last Win32 error 
            // and throw a Win32Exception.
            if (handleValue.IsInvalid)

                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        }

        public SafeFileHandle Handle =>

                // If the handle is valid,
                // return it.
                !handleValue.IsInvalid ? handleValue : null;

    }

}
