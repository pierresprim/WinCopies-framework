/* Copyright © Pierre Sprimont, 2019
 *
 * This file is part of the WinCopies Windows API Code Pack.
 *
 * This part of the WinCopies Windows API Code Pack is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This part of the WinCopies Windows API Code Pack is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Windows API Code Pack.  If not, see <https://www.gnu.org/licenses/>. */

#if CS6
using Microsoft.WindowsAPICodePack.COMNative.Shell;
using Microsoft.WindowsAPICodePack.COMNative.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

using static Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell;

using static WinCopies.
#if WinCopies3
    ThrowHelper;

using WinCopies.Collections.DotNetFix.Generic;
#else
    Util.Util;

using System.Collections.Generic;
#endif

using static Microsoft.WindowsAPICodePack.Win32Native.CoreErrorHelper;
using static Microsoft.WindowsAPICodePack.Shell.TEMP.CoreErrorHelper;

using static System.Runtime.InteropServices.UnmanagedType;

using FileAttributes = Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes;

namespace Microsoft.WindowsAPICodePack.Shell.TEMP
{
    [Flags]
    public enum TransferSourceFlags : uint
    {
        /// <summary>
        /// Fail if the destination already exists, unless <see cref="OverwriteExist"/> is specified. This is a default behavior.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// See <see cref="Normal"/>.
        /// </summary>
        FailExist = Normal,

        /// <summary>
        /// Rename with auto-name generation if the destination already exists.
        /// </summary>
        RenameExist = 0x1,

        /// <summary>
        /// Overwrite or merge with the destination.
        /// </summary>
        OverwriteExist = 0x2,

        /// <summary>
        /// Allow creation of a decrypted destination.
        /// </summary>
        AllowDecryption = 0x4,

        /// <summary>
        /// No discretionary access control list (DACL), system access control list (SACL), or owner.
        /// </summary>
        NoSecurity = 0x8,

        /// <summary>
        /// Copy the creation time as part of the copy. This can be useful for a move operation that is being used as a copy and delete operation (<see cref="MoveAsCopyDelete"/>).
        /// </summary>
        CopyCreationTime = 0x10,

        /// <summary>
        /// Copy the last write time as part of the copy.
        /// </summary>
        CopyWriteTime = 0x20,

        /// <summary>
        /// Assign write, read, and delete permissions as share mode.
        /// </summary>
        UseFullAccess = 0x40,

        /// <summary>
        /// Recycle on file delete, if possible.
        /// </summary>
        DeleteRecycleIfPossible = 0x80,

        /// <summary>
        /// Hard link to the desired source (not required). This avoids a normal copy operation.
        /// </summary>
        CopyHardLink = 0x100,

        /// <summary>
        /// Copy the localized name.
        /// </summary>
        CopyLocalizedName = 0x200,

        /// <summary>
        /// Move as a copy and delete operation.
        /// </summary>
        MoveAsCopyDelete = 0x400,

        /// <summary>
        /// Suspend Shell events.
        /// </summary>
        SuspendShellEvents = 0x800
    }

    [ComImport,
       Guid(NativeAPI.Guids.Shell.IFileOperation),
       InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOperation
    {
        /// <summary>
        /// Enables a handler to provide status and error information for all operations.
        /// </summary>
        /// <param name="pfops">An <see cref="IFileOperationProgressSink"/> object to be used for progress status and error notifications.</param>
        /// <param name="pdwCookie">When this method returns, this parameter points to a returned token that uniquely identifies this connection. The calling application uses this token later to delete the connection by passing it to <see cref="Unadvise"/>. If the call to <see cref="Advise"/> fails, this value is meaningless.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>Several individual methods have the ability to declare their own progress sinks, which are redundant to the one set here. They are used when you only want to be given progress and error information for a specific operation.</remarks>
        HResult Advise(IFileOperationProgressSink pfops, out uint pdwCookie);

        /// <summary>
        /// Terminates an advisory connection previously established through <see cref="Advise"/>.
        /// </summary>
        /// <param name="dwCookie">The connection token that identifies the connection to delete. This value was originally retrieved by <see cref="Advise"/> when the connection was made.</param>
        /// <returns>Any value other than those listed here indicate a failure.
        /// | Return code              | Description                                                  |
        /// |--------------------------+--------------------------------------------------------------|
        /// | <see cref="HResult.Ok"/> | The connection was terminated successfully.                  |
        /// | CONNECT_E_NOCONNECTION   | The value in dwCookie does not represent a valid connection. |</returns>
        HResult Unadvise(uint dwCookie);

        /// <summary>
        /// Sets parameters for the current operation.
        /// </summary>
        /// <param name="dwOperationFlags">Flags that control the file operation. FOF flags are defined in Shellapi.h and FOFX flags are defined in Shobjidl.h. Note : If this method is not called, the default value used by the operation is <see cref=" ShellOperationFlags.FOF_ALLOWUNDO"/> | <see cref="ShellOperationFlags.FOF_NOCONFIRMMKDIR"/>.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>Set these flags before you call <see cref="PerformOperations"/> to define the parameters for whatever operations are being performed, such as copy, delete, or rename.</remarks>
        HResult SetOperationFlags(ShellOperationFlags dwOperationFlags);

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="pszMessage">The window title.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        HResult SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);

        /// <summary>
        /// Specifies a dialog box used to display the progress of the operation.
        /// </summary>
        /// <param name="popd">An <see cref="IOperationsProgressDialog"/> object that represents the dialog box.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        HResult SetProgressDialog(IOperationsProgressDialog popd);

        /// <summary>
        /// Declares a set of properties and values to be set on an item or items.
        /// </summary>
        /// <param name="pproparray">An <see cref="IPropertyChangeArray"/>, which accesses a collection of <see cref="IPropertyChange"/> objects that specify the properties to be set and their new values.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not set the new property values, it merely declares them. To set property values on an item or a group of items, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="SetProperties"/> to declare the specific properties to be set and their new values.</li>
        /// <li>Call <see cref="ApplyPropertiesToItem"/> or <see cref="ApplyPropertiesToItems"/> to declare the item or items whose properties are to be set.</li>
        /// <li>Call <see cref="PerformOperations"/> to apply the properties to the item or items.</li></ol></remarks>
        HResult SetProperties(IPropertyChangeArray pproparray);

        /// <summary>
        /// Sets the parent or owner window for progress and dialog windows.
        /// </summary>
        /// <param name="hwndOwner">A handle to the owner window of the operation. This window will receive error messages.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        HResult SetOwnerWindow(IntPtr hwndOwner);

        /// <summary>
        /// Declares a single item whose property values are to be set.
        /// </summary>
        /// <param name="psiItem">Pointer to the item to receive the new property values.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not apply the properties to the item, it merely declares the item. To set property values on an item, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="SetProperties"/> to declare the specific properties to be set and their new values.</li>
        /// <li>Call <see cref="ApplyPropertiesToItem"/> to declare the item whose properties are to be set.</li>
        /// <li>Call <see cref="PerformOperations"/> to apply the properties to the item.</li></ol></remarks>
        HResult ApplyPropertiesToItem(IShellItem psiItem);

        /// <summary>
        /// Declares a set of items for which to apply a common set of property values.
        /// </summary>
        /// <param name="punkItems">Pointer to the IUnknown of the <see cref="IShellItemArray"/>, <see cref="IDataObject"/>, or <see cref="IEnumShellItems"/> object which represents the group of items. You can also point to an <see cref="IPersistIDList"/> object to represent a single item, effectively accomplishing the same function as <see cref="ApplyPropertiesToItem"/>.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not apply the properties to the items, it merely declares the items. To set property values on a group of items, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="SetProperties"/> to declare the specific properties to be set and their new values.</li>
        /// <li>Call <see cref="ApplyPropertiesToItems"/> to declare the items whose property values are to be set.</li>
        /// <li>Call <see cref="PerformOperations"/> to apply the properties to the items.</li></ol></remarks>
        HResult ApplyPropertiesToItems([MarshalAs(UnmanagedType.Interface)] object punkItems);

        /// <summary>
        /// Declares a single item that is to be given a new display name.
        /// </summary>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="pszNewName">Pointer to the new <a href="https://msdn.microsoft.com/9b159be9-3797-4c8b-90f8-9d3b3a3afb71">display name</a> of the item. This is a null-terminated, Unicode string.</param>
        /// <param name="pfopsItem">Pointer to an <see cref="IFileOperationProgressSink"/> object to be used for status and failure notifications. If you call <see cref="Advise"/> for the overall operation, progress status and error notifications for the rename operation are included there, so set this parameter to <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not rename the item, it merely declares the item to be renamed. To rename an object, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="RenameItem"/> to declare the new name.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the rename operation.</li></ol></remarks>
        HResult RenameItem(IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [MarshalAs(UnmanagedType.LPWStr)] IFileOperationProgressSink pfopsItem);

        /// <summary>
        /// Declares a set of items that are to be given a new display name. All items are given the same name.
        /// </summary>
        /// <param name="pUnkItems">Pointer to the <see cref="UnmanagedType.IUnknown"/> of the <see cref="IShellItemArray"/>, <see cref="IDataObject"/>, or <see cref="IEnumShellItems"/> object which represents the group of items to be renamed. You can also point to an <see cref="IPersistIDList"/> object to represent a single item, effectively accomplishing the same function as <see cref="RenameItem"/>.</param>
        /// <param name="pszNewName">Pointer to the new display name of the items. This is a null-terminated, Unicode string.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks><para>If more than one of the items in the collection at pUnkItems is in the same folder, the renamed files are appended with a number in parentheses to differentiate them, for instance newfile(1).txt, newfile(2).txt, and newfile(3).txt.</para>
        /// <para>This method does not rename the items, it merely declares the items to be renamed.To rename a group of objects, you must make at least the sequence of calls detailed here:</para>
        /// <ol><li>Call <see cref="RenameItems"/> to declare the source files or folders and the new name.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the rename operation.</li></ol></remarks>
        HResult RenameItems([MarshalAs(UnmanagedType.Interface)] object pUnkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);

        /// <summary>
        /// Declares a single item that is to be moved to a specified destination.
        /// </summary>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the moved item.</param>
        /// <param name="pszNewName">Pointer to a new name for the item in its new location. This is a null-terminated Unicode string and can be <see langword="null"/>. If <see langword="null"/>, the name of the destination item is the same as the source.</param>
        /// <param name="pfopsItem">Pointer to an <see cref="IFileOperationProgressSink"/> object to be used for progress status and error notifications for this specific move operation. If you call <see cref="Advise"/> for the overall operation, progress status and error notifications for the move operation are included there, so set this parameter to <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not move the item, it merely declares the item to be moved. To move an object, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="MoveItem"/> to declare the source item, destination folder, and destination name.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the move operation.</li></ol></remarks>
        HResult MoveItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IFileOperationProgressSink pfopsItem);

        /// <summary>
        /// Declares a set of items that are to be moved to a specified destination.
        /// </summary>
        /// <param name="punkItems">Pointer to the IUnknown of the <see cref="IShellItemArray"/>, <see cref="IDataObject"/>, or <see cref="IEnumShellItems"/> object which represents the group of items to be moved. You can also point to an <see cref="IPersistIDList"/> object to represent a single item, effectively accomplishing the same function as IFileOperation::MoveItem.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the moved items.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not move the items, it merely declares the items to be moved. To move a group of items, you must make at least the sequence of calls detailed here:
        /// <ul><li>Call <see cref="MoveItems"/> to declare the source files or folders and the destination folder.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the move operation.</li></ul></remarks>
        HResult MoveItems([MarshalAs(UnmanagedType.Interface)] object punkItems, IShellItem psiDestinationFolder);

        /// <summary>
        /// Declares a single item that is to be copied to a specified destination.
        /// </summary>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the copy of the item.</param>
        /// <param name="pszCopyName">Pointer to a new name for the item after it has been copied. This is a null-terminated Unicode string and can be <see langword="null"/>. If <see langword="null"/>, the name of the destination item is the same as the source.</param>
        /// <param name="pfopsItem">Pointer to an <see cref="IFileOperationProgressSink"/> object to be used for progress status and error notifications for this specific copy operation. If you call <see cref="Advise"/> for the overall operation, progress status and error notifications for the copy operation are included there, so set this parameter to <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not copy the item, it merely declares the item to be copied. To copy an object, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="CopyItem"/> to declare the source item, destination folder, and destination name.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the copy operation.</li></ol></remarks>
        HResult CopyItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem);

        /// <summary>
        /// Declares a set of items that are to be copied to a specified destination.
        /// </summary>
        /// <param name="punkItems">Pointer to the IUnknown of the <see cref="IShellItemArray"/>, <see cref="IDataObject"/>, or <see cref="IEnumShellItems"/> object which represents the group of items to be copied. You can also point to an <see cref="IPersistIDList"/> object to represent a single item, effectively accomplishing the same function as IFileOperation::CopyItem.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the copy of the items.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not copy the items, it merely declares the items to be copied. To copy a group of items, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="CopyItems"/> to declare the source items and the destination folder.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the copy operation.</li></ol></remarks>
        HResult CopyItems([MarshalAs(UnmanagedType.Interface)] object punkItems, IShellItem psiDestinationFolder);

        /// <summary>
        /// Declares a single item that is to be deleted.
        /// </summary>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item to be deleted.</param>
        /// <param name="pfopsItem">Pointer to an <see cref="IFileOperationProgressSink"/> object to be used for progress status and error notifications for this specific delete operation. If you call <see cref="Advise"/> for the overall operation, progress status and error notifications for the delete operation are included there, so set this parameter to <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not delete the item, it merely declares the item to be deleted. To delete an item, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="DeleteItem"/> to declare the file or folder to be deleted.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the delete operation.</li></ol></remarks>
        HResult DeleteItem(IShellItem psiItem, IFileOperationProgressSink pfopsItem);

        /// <summary>
        /// Declares a set of items that are to be deleted.
        /// </summary>
        /// <param name="punkItems">Pointer to the IUnknown of the <see cref="IShellItemArray"/>, <see cref="IDataObject"/>, or <see cref="IEnumShellItems"/> object which represents the group of items to be deleted. You can also point to an <see cref="IPersistIDList"/> object to represent a single item, effectively accomplishing the same function as IFileOperation::DeleteItem.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not delete the items, it merely declares the items to be deleted. To delete a group of items, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="DeleteItems"/> to declare the files or folders to be deleted.</li>
        /// <li>Call <see cref="PerformOperations"/> to begin the delete operation.</li></ol></remarks>
        HResult DeleteItems([MarshalAs(UnmanagedType.Interface)] object punkItems);

        /// <summary>
        /// Declares a new item that is to be created in a specified location.
        /// </summary>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder that will contain the new item.</param>
        /// <param name="dwFileAttributes">A bitwise value that specifies the file system attributes for the file or folder. See <see cref="File.GetAttributes(string)"/> for possible values.</param>
        /// <param name="pszName">Pointer to the file name of the new item, for instance <b>Newfile.txt</b>. This is a null-terminated, Unicode string.</param>
        /// <param name="pszTemplateName">Pointer to the name of the template file (for example Excel9.xls) that the new item is based on, stored in one of the following locations:
        /// <ol><li>CSIDL_COMMON_TEMPLATES.The default path for this folder is %ALLUSERSPROFILE%\Templates.</li>
        /// <li>CSIDL_TEMPLATES.The default path for this folder is %USERPROFILE%\Templates.</li>
        /// <li>%SystemRoot%\shellnew</li></ol>
        /// <para>This is a null-terminated, Unicode string used to specify an existing file of the same type as the new file, containing the minimal content that an application wants to include in any new file.</para>
        /// <para>This parameter is normally <see langword="null"/> to specify a new, blank file.</para></param>
        /// <param name="pfopsItem">Pointer to an <see cref="IFileOperationProgressSink"/> object to be used for status and failure notifications. If you call <see cref="Advise"/> for the overall operation, progress status and error notifications for the creation operation are included there, so set this parameter to <see langword="null"/>.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method does not create the new item, it merely declares the item to be created. To create a new item, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="NewItem"/> to declare the specifics of the new file or folder.</li>
        /// <li>Call <see cref="PerformOperations"/> to create the new item.</li></ol></remarks>
        HResult NewItem(IShellItem psiDestinationFolder, FileAttributes dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, IFileOperationProgressSink pfopsItem);

        /// <summary>
        /// Executes all selected operations.
        /// </summary>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. Note that if the operation was canceled by the user, this method can still return a success code. Use the <see cref="GetAnyOperationsAborted"/> method to determine if this was the case.</returns>
        /// <remarks>This method is called last to execute those actions that have been specified earlier by calling their individual methods. For instance, <see cref="RenameItem"/> does not rename the item, it simply sets the parameters. The actual renaming is done when you call <see cref="PerformOperations"/>.</remarks>
        HResult PerformOperations();

        /// <summary>
        /// Gets a value that states whether any file operations initiated by a call to <see cref="PerformOperations"/> were stopped before they were complete. The operations could be stopped either by user action or silently by the system.
        /// </summary>
        /// <param name="pfAnyOperationsAborted">When this method returns, points to <see langword="true"/> if any file operations were aborted before they were complete; otherwise, <see langword="false"/>.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks><para>Call this method after <see cref="PerformOperations"/> returns.</para>
        /// <para>You should call <see cref="GetAnyOperationsAborted"/> regardless of whether <see cref="PerformOperations"/> returned a success or failure code.A success code can be returned even if the operation was stopped by the user or the system.</para>
        /// <para>This method provides the same functionality as the fAnyOperationsAborted member of the SHFILEOPSTRUCT structure used by the legacy function SHFileOperation.</para></remarks>
        HResult GetAnyOperationsAborted(out bool pfAnyOperationsAborted);
    }

    /// <summary>
    /// Exposes methods that provide a rich notification system used by callers of <see cref="IFileOperation"/> to monitor the details of the operations they are performing through that interface.
    /// </summary>
    [ComImport,
        Guid("04B0F1A7-9490-44BC-96E1-4296A31252E2"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOperationProgressSink
    {
        /// <summary>
        /// Performs caller-implemented actions before any specific file operations are performed.
        /// </summary>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>StartOperations is the first of the <see cref="IFileOperationProgressSink"/> methods to be called after <see cref="IFileOperation.PerformOperations"/>. It can be used to perform any setup or initialization that you require before the file operations begin.</remarks>
#if WinCopies3
        HResult
#else
        void
#endif
            StartOperations();

        /// <summary>
        /// Performs caller-implemented actions after the last operation performed by the call to <see cref="IFileOperation"/> is complete.
        /// </summary>
        /// <param name="hr">The return value of the final operation. Note that this is not the <see cref="HResult"/> returned by one of the <see cref="IFileOperation"/> methods, which simply queue the operations. Instead, this is the result of the actual operation, such as copy, delete, or move.</param>
        /// <returns>Not used.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             FinishOperations([In] HResult hr);

        /// <summary>
        /// Performs caller-implemented actions before the rename process for each item begins.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that control the operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item to be renamed.</param>
        /// <param name="pszNewName">Pointer to the new <a href="https://msdn.microsoft.com/9b159be9-3797-4c8b-90f8-9d3b3a3afb71">display name</a> of the item. This is a null-terminated, Unicode string.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, the rename operation and all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PreRenameItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
        dwFlags, [In] IShellItem psiItem, [In, MarshalAs(LPWStr)] string pszNewName);

        /// <summary>
        /// Performs caller-implemented actions after the rename process for each item is complete.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that were used during the rename operation. Some values can be set or changed during the rename operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item before it was renamed.</param>
        /// <param name="pszNewName">Pointer to the new <a href="https://msdn.microsoft.com/9b159be9-3797-4c8b-90f8-9d3b3a3afb71">display name</a> of the item. This is a null-terminated, Unicode string. Note that this might not be the name that you asked for, given collisions and other naming rules.</param>
        /// <param name="hrRename">The return value of the rename operation. Note that this is not the HRESULT returned by <see cref="IFileOperation.RenameItem"/>, which simply queues the rename operation. Instead, this is the result of the actual rename operation.</param>
        /// <param name="psiNewlyCreated">Pointer to an <see cref="IShellItem"/> that represents the item with its new name.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PostRenameItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, IShellItem psiItem, [In, MarshalAs(LPWStr)] string pszNewName, [In] HResult hrRename, [In] IShellItem psiNewlyCreated);

        /// <summary>
        /// Performs caller-implemented actions before the move process for each item begins.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that control the operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item to be moved.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the moved item.</param>
        /// <param name="pszNewName">Pointer to a new name for the item in its new location. This is a null-terminated Unicode string and can be <see langword="null"/>. If <see langword="null"/>, the name of the destination item is the same as the source.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, the move operation and all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PreMoveItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName);

        /// <summary>
        /// Performs caller-implemented actions after the move process for each item is complete.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that were used during the move operation. Some values can be set or changed during the move operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder that contains the moved item.</param>
        /// <param name="pszNewName">Pointer to the name that was given to the item after it was moved. This is a null-terminated Unicode string. Note that this might not be the name that you asked for, given collisions and other naming rules.</param>
        /// <param name="hrMove">The return value of the move operation. Note that this is not the HRESULT returned by <see cref="IFileOperation.MoveItem"/>, which simply queues the move operation. Instead, this is the result of the actual move.</param>
        /// <param name="psiNewlyCreated">Pointer to an <see cref="IShellItem"/> that represents the moved item in its new location.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PostMoveItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName, [In] HResult hrMove, [In] IShellItem psiNewlyCreated);

        /// <summary>
        /// Performs caller-implemented actions before the copy process for each item begins.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that control the operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to contain the copy of the item.</param>
        /// <param name="pszNewName">Pointer to a new name for the item after it has been copied. This is a null-terminated Unicode string and can be <see langword="null"/>. If <see langword="null"/>, the name of the destination item is the same as the source.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, the copy operation and all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PreCopyItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName);

        /// <summary>
        /// Performs caller-implemented actions after the copy process for each item is complete.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that were used during the copy operation. Some values can be set or changed during the copy operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the source item.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to which the item was copied.</param>
        /// <param name="pszNewName">Pointer to the new name that was given to the item after it was copied. This is a null-terminated Unicode string. Note that this might not be the name that you asked for, given collisions and other naming rules.</param>
        /// <param name="hrCopy">The return value of the copy operation. Note that this is not the <see cref="HResult"/> returned by <see cref="IFileOperation.CopyItem"/>, which simply queues the copy operation. Instead, this is the result of the actual copy.</param>
        /// <param name="psiNewlyCreated">Pointer to an <see cref="IShellItem"/> that represents the new copy of the item.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PostCopyItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName, [In] HResult hrCopy, [In] IShellItem psiNewlyCreated);

        /// <summary>
        /// Performs caller-implemented actions before the delete process for each item begins.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that control the operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item to be deleted.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, the delete operation and all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PreDeleteItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem);

        /// <summary>
        /// Performs caller-implemented actions after the delete process for each item is complete.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that were used during the delete operation. Some values can be set or changed during the delete operation.</param>
        /// <param name="psiItem">Pointer to an <see cref="IShellItem"/> that specifies the item that was deleted.</param>
        /// <param name="hrDelete">The return value of the delete operation. Note that this is not the <see cref="HResult"/> returned by <see cref="IFileOperation.DeleteItem"/>, which simply queues the delete operation. Instead, this is the result of the actual deletion.</param>
        /// <param name="psiNewlyCreated">A pointer to an <see cref="IShellItem"/> that specifies the deleted item, now in the Recycle Bin. If the item was fully deleted, this value is <see langword="null"/>.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PostDeleteItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiItem, [In] HResult hrDelete, [In] IShellItem psiNewlyCreated);

        /// <summary>
        /// Performs caller-implemented actions before the process to create a new item begins.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that control the operation.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder that will contain the new item.</param>
        /// <param name="pszNewName">Pointer to the file name of the new item, for instance <b>Newfile.txt</b>. This is a null-terminated, Unicode string.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, this operation and all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PreNewItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName);

        /// <summary>
        /// Performs caller-implemented actions after the new item is created.
        /// </summary>
        /// <param name="dwFlags">Bitwise value that contains flags that were used during the creation operation. Some values can be set or changed during the creation operation.</param>
        /// <param name="psiDestinationFolder">Pointer to an <see cref="IShellItem"/> that specifies the destination folder to which the new item was added.</param>
        /// <param name="pszNewName">Pointer to the file name of the new item, for instance <b>Newfile.txt</b>. This is a null-terminated, Unicode string.</param>
        /// <param name="pszTemplateName">Pointer to the name of the template file (for example <b>Excel9.xls</b>) that the new item is based on, stored in one of the following locations:
        /// <ol><li>CSIDL_COMMON_TEMPLATES. The default path for this folder is %ALLUSERSPROFILE%\Templates.</li>
        /// <li>CSIDL_TEMPLATES. The default path for this folder is %USERPROFILE%\Templates.</li>
        /// <li>%SystemRoot%\shellnew</li></ol>
        /// <para>This is a null-terminated, Unicode string used to specify an existing file of the same type as the new file, containing the minimal content that an application wants to include in any new file.</para>
        /// <para>This parameter is normally <see langword="null"/> to specify a new, blank file.</para></param>
        /// <param name="dwFileAttributes">The file attributes applied to the new item.</param>
        /// <param name="hrNew">The return value of the creation operation. Note that this is not the <see cref="HResult"/> returned by <see cref="IFileOperation.NewItem"/>, which simply queues the creation operation. Instead, this is the result of the actual creation.</param>
        /// <param name="psiNewItem">Pointer to an <see cref="IShellItem"/> that represents the new item.</param>
        /// <returns>Returns <see cref="HResult.Ok"/> if successful, or an error value otherwise. In the case of an error value, all subsequent operations pending from the call to <see cref="IFileOperation"/> are canceled.</returns>

#if WinCopies3
        HResult
#else
        void
#endif
             PostNewItem([In, MarshalAs(U4)]
#if WinCopies3
             TransferSourceFlags
#else
uint
#endif
       dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(LPWStr)] string pszNewName, [In, MarshalAs(LPWStr)] string pszTemplateName, [In, MarshalAs(U4)] FileAttributes dwFileAttributes, [In] HResult hrNew, [In] IShellItem psiNewItem);

        /// <summary>
        /// Provides an estimate of the total amount of work currently done in relation to the total amount of work.
        /// </summary>
        /// <param name="iWorkTotal">An estimate of the amount of work to be completed.</param>
        /// <param name="iWorkSoFar">The portion of iWorkTotal that has been completed so far.</param>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>The iWorkTotal and iWorkSoFar values are "points" or estimates of the amount of work to be done, and how much is completed. They are not specified in any particular units, but should be roughly proportional to how much time the total process takes. For example, to copy one small file might be considered two points, and a large file might be considered ten points. If a process is performing an operation that copies five small files and one large file, and the process has completed four of the small files, iWorkSoFar would be eight points (4 x 2 = 8) and iWorkTotal would be twenty points (5 x 2 + 10 = 20), so the estimate would be 8 of 20 points (or 40%) complete.</remarks>

#if WinCopies3
        HResult
#else
        void
#endif
             UpdateProgress([In, MarshalAs(U4)] uint iWorkTotal, [In, MarshalAs(U4)] uint iWorkSoFar);

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method should return <see cref="HResult.Ok"/> rather than <see cref="HResult.NotImplemented"/>.</remarks>

#if WinCopies3
        HResult
#else
        void
#endif
             ResetTimer();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method should return <see cref="HResult.Ok"/> rather than <see cref="HResult.NotImplemented"/>.</remarks>

#if WinCopies3
        HResult
#else
        void
#endif
             PauseTimer();

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>If this method succeeds, it returns <see cref="HResult.Ok"/>. Otherwise, it returns an <see cref="HResult"/> error code.</returns>
        /// <remarks>This method should return <see cref="HResult.Ok"/> rather than <see cref="HResult.NotImplemented"/>.</remarks>

#if WinCopies3
        HResult
#else
        void
#endif
             ResumeTimer();
    }

    public static class CoreErrorHelper
    {
        public static void ThrowExceptionForHR(in HResult hresult) => Marshal.ThrowExceptionForHR((int)hresult);

        public static Exception GetExceptionForHR(in HResult hResult) => Marshal.GetExceptionForHR((int)hResult);

        public static Exception GetExceptionForHR(in HResult hResult, in IntPtr errorInfo) => Marshal.GetExceptionForHR((int)hResult, errorInfo);

        public static HResult GetHRForException(in Exception ex) => (HResult)Marshal.GetHRForException(ex);

        public static T GetIfSucceeded<T>(in T value, in HResult hResult) => Succeeded(hResult) ? value : throw GetExceptionForHR(hResult);

        // TODO: replace by WinCopies.FuncOut:

        public delegate HResult FuncOut<T>(out T param);

        public static T GetIfSucceeded<T>(in FuncOut<T> func)
        {
            HResult hr = (func ?? throw GetArgumentNullException(nameof(func)))(out T param);

            return GetIfSucceeded(param, hr);
        }
    }

    /// <summary>
    /// Contains information about a file object.
    /// </summary>
    /// <remarks>This structure is used with the <see cref="FileOperation.GetFileInfo(in string, in FileAttributes, in GetFileInfoOptions)"/> function.</remarks>
    public struct FileInfo :
#if WinCopies3
        WinCopies.DotNetFix
#else
        System
#endif
        .IDisposable
    {
        /// <summary>
        /// Gets or sets the icon that represents the file. When the <see cref="Dispose"/> method of this struct is called, that method calls the <see cref="Dispose"/> method on the current <see cref="Icon"/>.
        /// </summary>
        public Icon Icon { get; private set; }

        /// <summary>
        /// Gets or sets the index of the icon image within the system image list.
        /// </summary>
        public int IconIndex { get; }

        /// <summary>
        /// Gets or sets an array of values that indicates the attributes of the file object. For information about these values, see the <see cref="IShellFolder.GetAttributesOf"/> method.
        /// </summary>
        public ShellFileGetAttributesOptions Attributes { get; }

        /// <summary>
        /// Gets or sets a string that contains the name of the file as it appears in the Windows Shell, or the path and file name of the file that contains the icon representing the file.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets or sets a string that describes the type of file.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfo"/> structure.
        /// </summary>
        /// <param name="icon">The icon of the file represented by this structure.</param>
        /// <param name="iconIndex">The icon index of the retrieved icon.</param>
        /// <param name="attributes">The attributes of the file represented by this structure.</param>
        /// <param name="displayName">The display name of the file represented by this structure.</param>
        /// <param name="typeName">The type name of the file represented by this structure.</param>
        public FileInfo(
#if WinCopies3
            in
#endif
            Icon icon,
#if WinCopies3
            in
#endif
            int iconIndex,
#if WinCopies3
            in
#endif
            ShellFileGetAttributesOptions attributes,
#if WinCopies3
            in
#endif
            string displayName,
#if WinCopies3
            in
#endif
            string typeName)
        {
            Icon = icon;

            IconIndex = iconIndex;

            Attributes = attributes;

            DisplayName = displayName;

            TypeName = typeName;
        }

#if WinCopies3
        public bool IsDisposed => Icon == null;

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Icon.Dispose();
            Icon = null;

            DisplayName = null;

            TypeName = null;
        }
#else
        /// <summary>
        /// Calls the <see cref="Icon.Dispose"/> method from the <see cref="Icon"/> property.
        /// </summary>
        public void Dispose() => Icon.Dispose();
#endif
    }

    /// <summary>
    /// Provides methods to perform file system operations.
    /// </summary>
    public class FileOperation : WinCopies.DotNetFix.IDisposable
    {
        private readonly IFileOperation _fileOperation;
        private readonly
#if WinCopies3
            ILinkedList
#else
List
#endif
            <uint> _cookies = new
#if WinCopies3
            WinCopies.Collections.DotNetFix.Generic.LinkedList
#else
List
#endif
            <uint>();

        public
#if WinCopies3
            IReadOnlyLinkedList
#else
            System.Collections.ObjectModel.ReadOnlyCollection
#endif
            <uint> Cookies
        { get; }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOperation"/> class.
        /// </summary>
        public FileOperation()
        {
            _fileOperation = (IFileOperation)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(NativeAPI.Guids.Shell.FileOperation)));

            Cookies = new
#if WinCopies3
                WinCopies.Collections.DotNetFix.Generic.ReadOnlyLinkedList
#else
System.Collections.ObjectModel.ReadOnlyCollection
#endif
                <uint>(_cookies);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            foreach (uint cookie in _cookies)

                Unadvise(cookie);

            _ = Marshal.FinalReleaseComObject(_fileOperation);

            IsDisposed = true;
        }

        ~FileOperation() => Dispose(false);

#if WinCopies3
        public HResult TryAdvise(in IFileOperationProgressSinkProvider fileOperationProgressSinkProvider, out uint cookie) => TryAdvise(fileOperationProgressSinkProvider?.GetFileOperationProgressSink(), out cookie);
#endif

        public HResult TryAdvise(in
#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfops, out uint cookie)
        {
            if (pfops == null) throw new ArgumentNullException(nameof(pfops));

            if (IsDisposed) throw new ObjectDisposedException(nameof(FileOperation));

            HResult hr = _fileOperation.Advise(pfops
#if !WinCopies3
                ?.FileOperationProgressSink
#endif
                , out cookie);

            if (Succeeded(hr))

                _cookies.Add(cookie);

            return hr;
        }

#if WinCopies3
        public uint Advise(in IFileOperationProgressSinkProvider fileOperationProgressSinkProvider) => Advise(fileOperationProgressSinkProvider?.GetFileOperationProgressSink());
#endif

        /// <summary>
        /// Enables a handler to provide status and error information for all operations.
        /// </summary>
        /// <param name="pfops">An <see cref="IFileOperationProgressSink"/> object to be used for progress status and error notifications.</param>
        /// <returns>A returned token that uniquely identifies this connection. The calling application uses this token later to delete the connection by passing it to <see cref="Unadvise"/>.</returns>
        /// <remarks>Several individual methods have the ability to declare their own progress sinks, which are redundant to the one set here. They are used when you only want to be given progress and error information for a specific operation.</remarks>
        /// <exception cref="ArgumentNullException">Exception thrown when a parameter is null.</exception>
        /// <exception cref="ObjectDisposedException">Exception thrown when this object is disposed.</exception>
        /// <exception cref="Win32Exception">Exception thrown when this method fails because of an error in the Win32 COM API implementation.</exception>
        public uint Advise(
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfops)
        {
            ThrowExceptionForHR(TryAdvise(pfops, out uint cookie));

            return cookie;
        }

        public HResult TryUnadvise(in uint cookie)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(FileOperation));

            HResult hr = _fileOperation.Unadvise(cookie);

            if (Succeeded(hr))

                _ = _cookies.Remove(cookie);

            return hr;
        }

        /// <summary>
        /// Terminates an advisory connection previously established through <see cref="Advise"/>.
        /// </summary>
        /// <param name="dwCookie">The connection token that identifies the connection to delete. This value was originally retrieved by <see cref="Advise"/> when the connection was made.</param>
        /// <exception cref="ObjectDisposedException">Exception thrown when this object is disposed.</exception>
        /// <exception cref="Win32Exception">Exception thrown when this method fails because of an error in the Win32 COM API implementation.</exception>
        public void Unadvise(
#if WinCopies3
            in
#endif
            uint dwCookie) => ThrowExceptionForHR(TryUnadvise(dwCookie));

        private HResult GetHResult(in Func<HResult> func) => IsDisposed ? throw new ObjectDisposedException(nameof(FileOperation)) : func();

        public HResult TrySetOperationFlags(ShellOperationFlags flags) => GetHResult(() =>
        {
            switch (flags)
            {
                case ShellOperationFlags.AddUndoRecord:
                case ShellOperationFlags.RecycleOnDelete:

                    CoreHelpers.ThrowIfNotWin8();

                    break;

                case ShellOperationFlags.CopyAsDownload:
                case ShellOperationFlags.DoNotDisplayLocations:

                    CoreHelpers.ThrowIfNotWin7();

                    break;

                case ShellOperationFlags.RequireElevation:

                    CoreHelpers.ThrowIfNotVista();

                    break;
            }

            return _fileOperation.SetOperationFlags(flags);
        });

        /// <summary>
        /// Sets parameters for the current operation.
        /// </summary>
        /// <param name="flags">Flags that control the file operation. FOF flags are defined in Shellapi.h and FOFX flags are defined in Shobjidl.h. Note : If this method is not called, the default value used by the operation is <see cref=" ShellOperationFlags.FOF_ALLOWUNDO"/> | <see cref="ShellOperationFlags.FOF_NOCONFIRMMKDIR"/>.</param>
        /// <remarks>Set these flags before you call <see cref="PerformOperations"/> to define the parameters for whatever operations are being performed, such as copy, delete, or rename.</remarks>
        /// <exception cref="ObjectDisposedException">Exception thrown when this object is disposed.</exception>
        /// <exception cref="PlatformNotSupportedException">Exception thrown when a requested flag is not supported by the current platform.</exception>
        /// <exception cref="Win32Exception">Exception thrown when this method fails because of an error in the Win32 COM API implementation.</exception>
        public void SetOperationFlags(
#if WinCopies3
            in
#endif
            ShellOperationFlags flags) => ThrowExceptionForHR(TrySetOperationFlags(flags));

        public HResult TrySetProgressDialog(IOperationsProgressDialog progressDialog)
        {
            ThrowIfNull(progressDialog, nameof(progressDialog));

            return GetHResult(() => _fileOperation.SetProgressDialog(progressDialog));
        }

        /// <summary>
        /// Specifies a dialog box used to display the progress of the operation.
        /// </summary>
        /// <param name="popd">An <see cref="IOperationsProgressDialog"/> object that represents the dialog box.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when a parameter is null.</exception>
        /// <exception cref="Win32Exception">Exception thrown when this method fails because of an error in the Win32 COM API implementation.</exception>
        public void SetProgressDialog(
#if WinCopies3
            in
#endif
            IOperationsProgressDialog popd) => ThrowExceptionForHR(TrySetProgressDialog(popd));

        public HResult TrySetProperties(IPropertyChangeArray pproparray)
        {
            ThrowIfNull(pproparray, nameof(pproparray));

            return GetHResult(() => _fileOperation.SetProperties(pproparray));
        }

        /// <summary>
        /// Declares a set of properties and values to be set on an item or items.
        /// </summary>
        /// <param name="pproparray">An <see cref="IPropertyChangeArray"/>, which accesses a collection of <see cref="IPropertyChange"/> objects that specify the properties to be set and their new values.</param>
        /// <remarks>This method does not set the new property values, it merely declares them. To set property values on an item or a group of items, you must make at least the sequence of calls detailed here:
        /// <ol><li>Call <see cref="SetProperties"/> to declare the specific properties to be set and their new values.</li>
        /// <li>Call <see cref="ApplyPropertiesToItem(ShellObject)"/> or <see cref="ApplyPropertiesToItem(IShellItem)"/> to declare the item or items whose properties are to be set.</li>
        /// <li>Call <see cref="PerformOperations"/> to apply the properties to the item or items.</li></ol></remarks>
        /// <exception cref="ArgumentNullException">Exception thrown when a parameter is null.</exception>
        /// <exception cref="ObjectDisposedException">Exception thrown when this object is disposed.</exception>
        /// <exception cref="Win32Exception">Exception thrown when this method fails because of an error in the Win32 COM API implementation.</exception>
        public void SetProperties(
#if WinCopies3
            in
#endif
            IPropertyChangeArray pproparray) => ThrowExceptionForHR(TrySetProperties(pproparray));

        public HResult TrySetOwnerWindow(IntPtr ptr) => GetHResult(() => _fileOperation.SetOwnerWindow(ptr));

        public void SetOwnerWindow(
#if WinCopies3
            in
#endif
            IntPtr hwndOwner) => ThrowExceptionForHR(TrySetOwnerWindow(hwndOwner));

        public HResult TrySetOwnerWindow(in Form window) => TrySetOwnerWindow(window.Handle);

        public void SetOwnerWindow(
#if WinCopies3
            in
#endif
            Form window) => SetOwnerWindow(window.Handle);

        public HResult TrySetOwnerWindow(in Window window) => TrySetOwnerWindow(new WindowInteropHelper(window).Handle);

        public void SetOwnerWindow(
#if WinCopies3
            in
#endif
            Window window) => SetOwnerWindow(new WindowInteropHelper(window).Handle);

        public HResult TryApplyPropertiesToItem(IShellItem item) => GetHResult(() => _fileOperation.ApplyPropertiesToItem(item));

        public void ApplyPropertiesToItem(
#if WinCopies3
            in
#endif
            IShellItem psiItem) => ThrowExceptionForHR(TryApplyPropertiesToItem(psiItem));

        private static IShellItem GetShellItem(in ShellObject shellObject) => (IShellItem) typeof(ShellObject).GetProperty("NativeShellItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(shellObject);

        public void ApplyPropertiesToItem(
#if WinCopies3
            in
#endif
            ShellObject shellObject) => ApplyPropertiesToItem(GetShellItem(shellObject));

#if WinCopies3
        public HResult TryRenameItem(in IShellItem item, in string newName, in IFileOperationProgressSinkProvider provider) => TryRenameItem(item, newName, provider?.GetFileOperationProgressSink());

        public HResult TryRenameItem(in ShellObject item, in string newName, in IFileOperationProgressSinkProvider provider) => TryRenameItem(GetShellItem(item), newName, provider?.GetFileOperationProgressSink());

        public void RenameItem(in IShellItem psiItem, in string pszNewName, in IFileOperationProgressSinkProvider pfopsItem) => RenameItem(psiItem, pszNewName, pfopsItem?.GetFileOperationProgressSink());

        public void RenameItem(in ShellObject shellObject, in string newName, in IFileOperationProgressSinkProvider pfopsItem) => RenameItem(GetShellItem(shellObject), newName, pfopsItem.GetFileOperationProgressSink());
#endif

        public HResult TryRenameItem(IShellItem item, string newName,
#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            fileOperationProgressSink) => GetHResult(() => _fileOperation.RenameItem(item, newName, fileOperationProgressSink
#if !WinCopies3
                ?.FileOperationProgressSink
#endif
                ));

        public HResult TryRenameItem(in ShellObject item, in string newName, in
#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            fileOperationProgressSink) => TryRenameItem(GetShellItem(item), newName, fileOperationProgressSink);

        public void RenameItem(
#if WinCopies3
            in
#endif
            IShellItem psiItem,
#if WinCopies3
            in
#endif
            string pszNewName,
#if WinCopies3
            in
#endif

#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
         pfopsItem) => ThrowExceptionForHR(TryRenameItem(psiItem, pszNewName, pfopsItem));

        public void RenameItem(
#if WinCopies3
            in
#endif
            ShellObject shellObject,
#if WinCopies3
            in
#endif
            string newName,
#if WinCopies3
            in
#endif

#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
         pfopsItem) => RenameItem(GetShellItem(shellObject), newName, pfopsItem);

#if WinCopies3
        public HResult TryMoveItem(in IShellItem item, in IShellItem destinationFolder, in string newName, in IFileOperationProgressSinkProvider provider) => TryMoveItem(item, destinationFolder, newName, provider?.GetFileOperationProgressSink());

        public HResult TryMoveItem(in ShellObject item, in ShellObject destinationFolder, in string newName, in IFileOperationProgressSinkProvider provider) => TryMoveItem(GetShellItem(item), GetShellItem(destinationFolder), newName, provider?.GetFileOperationProgressSink());
#endif

        public HResult TryMoveItem(IShellItem item, IShellItem destinationFolder, string newName, IFileOperationProgressSink fileOperationProgressSink) => GetHResult(() => _fileOperation.MoveItem(item, destinationFolder, newName, fileOperationProgressSink
#if !WinCopies3
                ?.FileOperationProgressSink
#endif
                ));

        public HResult TryMoveItem(in ShellObject item, in ShellObject destinationFolder, in string newName, in IFileOperationProgressSink fileOperationProgressSink) => TryMoveItem(GetShellItem(item), GetShellItem(destinationFolder), newName, fileOperationProgressSink);

        public void MoveItem(in IShellItem item, in IShellItem destinationFolder, in string newName, in IFileOperationProgressSinkProvider provider) => MoveItem(item, destinationFolder, newName, provider?.GetFileOperationProgressSink());

        public void MoveItem(in ShellObject item, in ShellObject destinationFolder, in string newName, IFileOperationProgressSinkProvider provider) => MoveItem(GetShellItem(item), GetShellItem(destinationFolder), newName, provider?.GetFileOperationProgressSink());

        public void MoveItem(
#if WinCopies3
            in
#endif
            IShellItem psiItem,
#if WinCopies3
            in
#endif
            IShellItem psiDestinationFolder,
#if WinCopies3
            in
#endif
            string pszNewName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfopsItem) => ThrowExceptionForHR(TryMoveItem(psiItem, psiDestinationFolder, pszNewName, pfopsItem));

        public void MoveItem(
#if WinCopies3
            in
#endif
            ShellObject shellObject,
#if WinCopies3
            in
#endif
            ShellObject destinationShellObject,
#if WinCopies3
            in
#endif
            string newName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
        pfopsItem) => MoveItem(GetShellItem(shellObject), GetShellItem(destinationShellObject), newName, pfopsItem);

        public HResult TryCopyItem(IShellItem item, IShellItem destinationFolder, string copyName,
#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
       fileOperationProgressSink) => GetHResult(() => _fileOperation.CopyItem(item, destinationFolder, copyName, fileOperationProgressSink
#if !WinCopies3
            ?.FileOperationProgressSink
#endif
            ));

        public HResult TryCopyItem(in ShellObject item, in ShellObject destinationFolder, in string copyName, in
#if WinCopies3
            IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            fileOperationProgressSink) => TryCopyItem(GetShellItem(item), GetShellItem(destinationFolder), copyName, fileOperationProgressSink);

#if WinCopies3
        public HResult TryCopyItem(in IShellItem item, in IShellItem destinationFolder, in string copyName, in IFileOperationProgressSinkProvider provider) => TryCopyItem(item, destinationFolder, copyName, provider?.GetFileOperationProgressSink());

        public HResult TryCopyItem(in ShellObject item, in ShellObject destinationFolder, in string copyName, in IFileOperationProgressSinkProvider provider) => TryCopyItem(GetShellItem(item), GetShellItem(destinationFolder), copyName, provider?.GetFileOperationProgressSink());

        public void CopyItem(in IShellItem item, in IShellItem destinationFolder, in string copyName, in IFileOperationProgressSinkProvider provider) => CopyItem(item, destinationFolder, copyName, provider?.GetFileOperationProgressSink());

        public void CopyItem(in ShellObject item, in ShellObject destinationFolder, in string copyName, in IFileOperationProgressSinkProvider provider) => CopyItem(GetShellItem(item), GetShellItem(destinationFolder), copyName, provider?.GetFileOperationProgressSink());
#endif

        public void CopyItem(
#if WinCopies3
            in
#endif
            IShellItem psiItem,
#if WinCopies3
            in
#endif
            IShellItem psiDestinationFolder,
#if WinCopies3
            in
#endif
            string pszCopyName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
             pfopsItem) => ThrowExceptionForHR(TryCopyItem(psiItem, psiDestinationFolder, pszCopyName, pfopsItem));

        public void CopyItem(
#if WinCopies3
            in
#endif
            ShellObject shellObject,
#if WinCopies3
            in
#endif
            ShellObject destinationFolder,
#if WinCopies3
            in
#endif
            string copyName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
             pfopsItem) => CopyItem(GetShellItem(shellObject), GetShellItem(destinationFolder), copyName, pfopsItem);

        public HResult TryDeleteItem(IShellItem item, IFileOperationProgressSink fileOperationProgressSink) => GetHResult(() => _fileOperation.DeleteItem(item, fileOperationProgressSink
#if !WinCopies3
            ?.FileOperationProgressSink
#endif
            ));

        public HResult TryDeleteItem(in ShellObject item, in IFileOperationProgressSink fileOperationProgressSink) => TryDeleteItem(GetShellItem(item), fileOperationProgressSink);

#if WinCopies3
        public HResult TryDeleteItem(in IShellItem item, in IFileOperationProgressSinkProvider provider) => TryDeleteItem(item, provider?.GetFileOperationProgressSink());

        public HResult TryDeleteItem(in ShellObject item, in IFileOperationProgressSinkProvider provider) => TryDeleteItem(GetShellItem(item), provider?.GetFileOperationProgressSink());

        public void DeleteItem(in IShellItem item, in IFileOperationProgressSinkProvider provider) => DeleteItem(item, provider?.GetFileOperationProgressSink());

        public void DeleteItem(in ShellObject item, in IFileOperationProgressSinkProvider provider) => DeleteItem(GetShellItem(item), provider?.GetFileOperationProgressSink());
#endif

        public void DeleteItem(
#if WinCopies3
            in
#endif
            IShellItem psiItem,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfopsItem) => ThrowExceptionForHR(TryDeleteItem(psiItem, pfopsItem));

        public void DeleteItem(
#if WinCopies3
            in
#endif
            ShellObject shellObject,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfopsItem) => DeleteItem(GetShellItem(shellObject), pfopsItem);

        public HResult TryNewItem(IShellItem destinationFolder, FileAttributes fileAttributes, string name, string templateName, IFileOperationProgressSink fileOperationProgressSink) => GetHResult(() => _fileOperation.NewItem(destinationFolder, fileAttributes, name, templateName, fileOperationProgressSink
#if !WinCopies3
            ?.FileOperationProgressSink
#endif
            ));

        public HResult TryNewItem(in ShellObject destinationFolder, in FileAttributes fileAttributes, in string name, in string templateName, in IFileOperationProgressSink fileOperationProgressSink) => TryNewItem(GetShellItem(destinationFolder), fileAttributes, name, templateName, fileOperationProgressSink);

#if WinCopies3
        public HResult TryNewItem(in IShellItem destinationFolder, in FileAttributes fileAttributes, in string name, in string templateName, in IFileOperationProgressSinkProvider fileOperationProgressSink) => TryNewItem(destinationFolder, fileAttributes, name, templateName, fileOperationProgressSink?.GetFileOperationProgressSink());

        public HResult TryNewItem(in ShellObject destinationFolder, in FileAttributes fileAttributes, in string name, in string templateName, in IFileOperationProgressSinkProvider fileOperationProgressSink) => TryNewItem(GetShellItem(destinationFolder), fileAttributes, name, templateName, fileOperationProgressSink?.GetFileOperationProgressSink());

        public void NewItem(in IShellItem destinationFolder, in FileAttributes fileAttributes, in string name, in string templateName, in IFileOperationProgressSinkProvider provider) => NewItem(destinationFolder, fileAttributes, name, templateName, provider.GetFileOperationProgressSink());

        public void NewItem(in ShellObject destinationFolder, in FileAttributes fileAttributes, in string name, in string templateName, in IFileOperationProgressSinkProvider provider) => NewItem(GetShellItem(destinationFolder), fileAttributes, name, templateName, provider.GetFileOperationProgressSink());
#endif

        public void NewItem(
#if WinCopies3
            in
#endif
            IShellItem psiDestinationFolder,
#if WinCopies3
            in
#endif
            FileAttributes dwFileAttributes,
#if WinCopies3
            in
#endif
            string pszName,
#if WinCopies3
            in
#endif
            string pszTemplateName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfopsItem) => ThrowExceptionForHR(TryNewItem(psiDestinationFolder, dwFileAttributes, pszName, pszTemplateName, pfopsItem));

        public void NewItem(
#if WinCopies3
            in
#endif
            ShellObject destinationFolder,
#if WinCopies3
            in
#endif
            FileAttributes fileAttributes,
#if WinCopies3
            in
#endif
            string name,
#if WinCopies3
            in
#endif
            string templateName,
#if WinCopies3
            in IFileOperationProgressSink
#else
            FileOperationProgressSink
#endif
            pfopsItem) => NewItem(GetShellItem(destinationFolder), fileAttributes, name, templateName, pfopsItem);

        public HResult TryPerformOperations() => GetHResult(() => _fileOperation.PerformOperations());

        public void PerformOperations() => ThrowExceptionForHR(TryPerformOperations());

        public HResult TryGetAnyOperationsAborted(out bool anyOperationsAborted) => IsDisposed
                ? throw new ObjectDisposedException(nameof(FileOperation))
                : _fileOperation.GetAnyOperationsAborted(out anyOperationsAborted);

        public bool GetAnyOperationsAborted() => GetIfSucceeded<bool>(TryGetAnyOperationsAborted);

        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="path"><para>A string of maximum length <see cref="MaxPath"/> that contains the path and file name. Both absolute and relative paths are valid.</para>
        /// <para>If the <b>uFlags</b> parameter includes the <see cref="GetFileInfoOptions.PIDL"/> flag, this parameter must be the address of an ITEMIDLIST(PIDL) structure that contains the list of item identifiers that uniquely identifies the file within the Shell's namespace. The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.</para>
        /// <para>If the <b>uFlags</b> parameter includes the <see cref="GetFileInfoOptions.UseFileAttributes"/> flag, this parameter does not have to be a valid file name. The function will proceed as if the file exists with the specified name and with the file attributes passed in the <b>dwFileAttributes</b> parameter. This allows you to obtain information about a file type by passing just the extension for <b>pszPath</b> and passing <see cref="FileAttributes.Normal"/> in <b>dwFileAttributes</b>.</para>
        /// <para>This string can use either short (the 8.3 form) or long file names.</para></param>
        /// <param name="fileAttributes">A combination of one or more <see cref="FileAttributes"/> flags. If <b>uFlags</b> does not include the <see cref="GetFileInfoOptions.UseFileAttributes"/> flag, this parameter is ignored.</param>
        /// <param name="options">The flags that specify the file information to retrieve. This parameter can be a combination of the values of the <see cref="GetFileInfoOptions"/> enum.</param>
        /// <returns>A <see cref="FileInfo"/> structure that contains the file information.</returns>
        public static FileInfo GetFileInfo(
#if WinCopies3
            in
#endif
            string path,
#if WinCopies3
            in
#endif
            FileAttributes fileAttributes,
#if WinCopies3
            in
#endif
            GetFileInfoOptions options)
        {
            var psfi = new SHFILEINFO();

            HResult hr = SHGetFileInfo(path, fileAttributes, ref psfi, (uint)Marshal.SizeOf(psfi), options);

            if (!Succeeded(hr))

                Marshal.ThrowExceptionForHR((int)hr);

            Icon icon;

            if (psfi.hIcon == IntPtr.Zero)

                icon = null;

            else
            {
                icon = (Icon)Icon.FromHandle(psfi.hIcon).Clone();

                _ = Core.DestroyIcon(psfi.hIcon);
            }

            return new FileInfo(icon, psfi.iIcon, psfi.dwAttributes, psfi.szDisplayName, psfi.szTypeName);
        }

        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root and a value that indicates the exe type.
        /// </summary>
        /// <param name="path"><para>A string of maximum length <see cref="MaxPath"/> that contains the path and file name. Both absolute and relative paths are valid.</para>
        /// <para>If the <b>uFlags</b> parameter includes the <see cref="GetFileInfoOptions.PIDL"/> flag, this parameter must be the address of an ITEMIDLIST(PIDL) structure that contains the list of item identifiers that uniquely identifies the file within the Shell's namespace. The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.</para>
        /// <para>If the <b>uFlags</b> parameter includes the <see cref="GetFileInfoOptions.UseFileAttributes"/> flag, this parameter does not have to be a valid file name. The function will proceed as if the file exists with the specified name and with the file attributes passed in the <b>dwFileAttributes</b> parameter. This allows you to obtain information about a file type by passing just the extension for <b>pszPath</b> and passing <see cref="FileAttributes.Normal"/> in <b>dwFileAttributes</b>.</para>
        /// <para>This string can use either short (the 8.3 form) or long file names.</para></param>
        /// <param name="fileAttributes">A combination of one or more <see cref="FileAttributes"/> flags. If <b>uFlags</b> does not include the <see cref="GetFileInfoOptions.UseFileAttributes"/> flag, this parameter is ignored.</param>
        /// <param name="options">The flags that specify the file information to retrieve. This parameter can be a combination of the values of the <see cref="GetFileInfoOptions"/> enum.</param>
        /// <param name="exeType">The exe type. In order to this method retrieves the exe type, you need to use the <see cref="GetFileInfoOptions.ExeType"/> flag in the <b>options</b> parameter.</param>
        /// <returns>A <see cref="FileInfo"/> structure that contains the file information.</returns>
        public static FileInfo GetFileInfo(
#if WinCopies3
            in
#endif
            string path,
#if WinCopies3
            in
#endif
            FileAttributes fileAttributes,
#if WinCopies3
            in
#endif
            GetFileInfoOptions options, out int exeType)
        {
            var psfi = new SHFILEINFO();

            HResult hr = SHGetFileInfo(path, fileAttributes, ref psfi, (uint)Marshal.SizeOf(psfi), options);

            if (!Succeeded(hr))

                Marshal.ThrowExceptionForHR((int)hr);

            exeType = options.HasFlag(GetFileInfoOptions.ExeType) ? (int)hr : 0;

            Icon icon;

            if (psfi.hIcon == IntPtr.Zero)

                icon = null;

            else
            {
                icon = (Icon)Icon.FromHandle(psfi.hIcon).Clone();

                _ = Core.DestroyIcon(psfi.hIcon);
            }

            return new FileInfo(icon, psfi.iIcon, psfi.dwAttributes, psfi.szDisplayName, psfi.szTypeName);
        }

        public static void CopyFile(
#if WinCopies3
            in
#endif
            string sourceFileName,
#if WinCopies3
            in
#endif
            string newFileName,
#if WinCopies3
            in
#endif
            CopyProgressRoutine progressRoutine,
#if WinCopies3
            in
#endif
            IntPtr data, ref bool cancel,
#if WinCopies3
            in
#endif
            CopyFileFlags copyFlags)
        {
            if (copyFlags == CopyFileFlags.CopySymLink || copyFlags == CopyFileFlags.NoBuffering)

                CoreHelpers.ThrowIfNotVista();

            if (!CopyFileEx(sourceFileName, newFileName, progressRoutine, data, ref cancel, copyFlags))

                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        /// <summary>
        /// Queries the size and the number of items in the Recycle Bin.
        /// </summary>
        /// <param name="drivePath">The path to the Recycle Bin drive. Leave this parameter null or empty if you want to get the info for the Recycle Bins for all drives.</param>
        /// <param name="recycleBinInfo">The out Recycle Bin info</param>
        /// <returns><see langword="true"/> if the drive supports the Recycle Bin, otherwise <see langword="false"/>.</returns>
        /// <exception cref="Win32Exception">Exception thrown if a Win32 exception has occurred during thr process.</exception>
        public static bool QueryRecycleBinInfo(
#if WinCopies3
            in
#endif
            string drivePath, out RecycleBinInfo recycleBinInfo)
        {
            var rbInfo = new SHQUERYRBINFO
#if CS7
            {
                cbSize = Marshal.SizeOf<SHQUERYRBINFO>()
            };
#else
                ();

            rbInfo.cbSize = Marshal.SizeOf(rbInfo);
#endif

            HResult hr = SHQueryRecycleBin(drivePath, ref rbInfo);

            if (hr == HResult.Ok)
            {
                recycleBinInfo = new RecycleBinInfo(rbInfo.i64Size, rbInfo.i64NumItems);

                return true;
            }

            else if (hr == HResult.Fail)
            {
                recycleBinInfo = default;

                return false;
            }

            else

                Marshal.ThrowExceptionForHR((int)hr);

            recycleBinInfo = default;

            return false;
        }

        /// <summary>
        /// Empties the Recycle Bin.
        /// </summary>
        /// <param name="windowHandle">A handle to the owner window</param>
        /// <param name="drivePath">The path to the Recycle Bin drive. Leave this parameter null or empty if you want to get the info for the Recycle Bins for all drives.</param>
        /// <param name="emptyRecycleBinFlags">Flags to describe the behavior for the process.</param>
        /// <returns><see langword="true"/> if the drive supports the Recycle Bin, otherwise <see langword="false"/>.</returns>
        /// <exception cref="Win32Exception">Exception thrown if a Win32 exception has occurred during thr process.</exception>
        public static bool EmptyRecycleBin(
#if WinCopies3
            in
#endif
            IntPtr windowHandle,
#if WinCopies3
            in
#endif
            string drivePath,
#if WinCopies3
            in
#endif
            EmptyRecycleBinFlags emptyRecycleBinFlags)
        {
            HResult hr = SHEmptyRecycleBin(windowHandle, drivePath, emptyRecycleBinFlags);

            if (hr == HResult.Ok) return true;

            else if (hr == HResult.Fail) return false;

            else Marshal.ThrowExceptionForHR((int)hr);

            return false;
        }

        /// <summary>
        /// Empties the Recycle Bin.
        /// </summary>
        /// <param name="window">The owner window</param>
        /// <param name="drivePath">The path to the Recycle Bin drive. Leave this parameter null or empty if you want to get the info for the Recycle Bins for all drives.</param>
        /// <param name="emptyRecycleBinFlags">Flags to describe the behavior for the process.</param>
        /// <returns><see langword="true"/> if the drive supports the Recycle Bin, otherwise <see langword="false"/>.</returns>
        /// <exception cref="Win32Exception">Exception thrown if a Win32 exception has occurred during thr process.</exception>
        public static bool EmptyRecycleBin(
#if WinCopies3
            in
#endif
            Form window,
#if WinCopies3
            in
#endif
            string drivePath,
#if WinCopies3
            in
#endif
            EmptyRecycleBinFlags emptyRecycleBinFlags)
        {
            HResult hr = SHEmptyRecycleBin(window.Handle, drivePath, emptyRecycleBinFlags);

            if (hr == HResult.Ok) return true;

            else if (hr == HResult.Fail) return false;

            else Marshal.ThrowExceptionForHR((int)hr);

            return false;
        }

        /// <summary>
        /// Empties the Recycle Bin.
        /// </summary>
        /// <param name="window">The owner window</param>
        /// <param name="drivePath">The path to the Recycle Bin drive. Leave this parameter null or empty if you want to get the info for the Recycle Bins for all drives.</param>
        /// <param name="emptyRecycleBinFlags">Flags to describe the behavior for the process.</param>
        /// <returns><see langword="true"/> if the drive supports the Recycle Bin, otherwise <see langword="false"/>.</returns>
        /// <exception cref="Win32Exception">Exception thrown if a Win32 exception has occurred during thr process.</exception>
        public static bool EmptyRecycleBin(
#if WinCopies3
            in
#endif
            Window window,
#if WinCopies3
            in
#endif
            string drivePath,
#if WinCopies3
            in
#endif
            EmptyRecycleBinFlags emptyRecycleBinFlags)
        {
            HResult hr = SHEmptyRecycleBin(new WindowInteropHelper(window).Handle, drivePath, emptyRecycleBinFlags);

            switch (hr)
            {
                case HResult.Ok:

                    return true;

                case HResult.Fail:

                    return false;
            }

            Marshal.ThrowExceptionForHR((int)hr);

            return false;
        }
    }

    public struct RecycleBinInfo
    {
        public long Size { get; }

        public long NumItems { get; }

        internal RecycleBinInfo(long size, long numItems)
        {
            Size = size;

            NumItems = numItems;
        }
    }

#if WinCopies3
    public delegate HResult PreNewItem(TransferSourceFlags dwFlags, IShellItem psiItem, string pszNewName);

    public delegate HResult PostNewItem(TransferSourceFlags dwFlags, IShellItem psiDestinationFolder, string pszNewName, string pszTemplateName, Microsoft.WindowsAPICodePack.Win32Native.Shell.FileAttributes dwFileAttributes, HResult hrNew, IShellItem psiNewItem);

    public delegate HResult PostRename(TransferSourceFlags dwFlags, IShellItem psiItem, string pszNewName, HResult hrRename, IShellItem psiNewlyCreated);

    public delegate HResult PreCopy(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName);

    public delegate HResult PostCopy(TransferSourceFlags dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName, HResult hrCopy, IShellItem psiNewlyCreated);

    public delegate HResult PreDelete(TransferSourceFlags dwFlags, IShellItem psiItem);

    public delegate HResult PostDelete(TransferSourceFlags dwFlags, IShellItem psiItem, HResult hrDelete, IShellItem psiNewlyCreated);

    public delegate HResult UpdateProgress(uint iWorkTotal, uint iWorkSoFar);

    public interface IFileOperationProgressSinkProvider
    {
        IFileOperationProgressSink GetFileOperationProgressSink();
    }
#endif

    public abstract class FileOperationProgressSinkAbstract : IFileOperationProgressSink, IFileOperationProgressSinkProvider
    {
        public abstract HResult StartOperations();
        public abstract HResult FinishOperations([In] HResult hr);
        public abstract HResult PreRenameItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
        public abstract HResult PostRenameItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, IShellItem psiItem, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrRename, [In] IShellItem psiNewlyCreated);
        public abstract HResult PreMoveItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
        public abstract HResult PostMoveItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrMove, [In] IShellItem psiNewlyCreated);
        public abstract HResult PreCopyItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
        public abstract HResult PostCopyItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrCopy, [In] IShellItem psiNewlyCreated);
        public abstract HResult PreDeleteItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem);
        public abstract HResult PostDeleteItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] HResult hrDelete, [In] IShellItem psiNewlyCreated);
        public abstract HResult PreNewItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
        public abstract HResult PostNewItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In, MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, [In, MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes, [In] HResult hrNew, [In] IShellItem psiNewItem);
        public abstract HResult UpdateProgress([In, MarshalAs(UnmanagedType.U4)] uint iWorkTotal, [In, MarshalAs(UnmanagedType.U4)] uint iWorkSoFar);
        public abstract HResult ResetTimer();
        public abstract HResult PauseTimer();
        public abstract HResult ResumeTimer();

        IFileOperationProgressSink IFileOperationProgressSinkProvider.GetFileOperationProgressSink() => this;
    }

    public class FileOperationProgressSink : WinCopies.DotNetFix.IDisposable
#if WinCopies3
        , IFileOperationProgressSinkProvider
#endif
    {
        protected
#if !WinCopies3
internal
#endif
            FileOperationProgressSinkDefault InnerFileOperationProgressSink
        { get; private set; }

        public FileOperationProgressSink() => InnerFileOperationProgressSink = new FileOperationProgressSinkDefault(this);

        /// <summary>
        /// Gets or sets a delegate to call in the <see cref="FileOperation"/> class when file operations are starting.
        /// </summary>
        public
#if WinCopies3
            Func<HResult>
#else
Action
#endif
            StartOperations
        { get; set; }

        public
#if WinCopies3
            Func<HResult, HResult>
#else
Action
#endif
            FinishOperations
        { get; set; }

        public
#if WinCopies3
            PreNewItem
#else
            Action<uint, IShellItem, string>
#endif
          PreRenameItem
        { get; set; }

        public
#if WinCopies3
            PostRename
#else
            Action<uint, IShellItem, string, IShellItem>
#endif
        PostRenameItem
        { get; set; }

        public
#if WinCopies3
            PreCopy
#else
Action<uint, IShellItem, IShellItem, string>
#endif
            PreMoveItem
        { get; set; }

        public
#if WinCopies3
            PostCopy
#else
            Action<uint, IShellItem, IShellItem, string, IShellItem>
#endif
            PostMoveItem
        { get; set; }

        public
#if WinCopies3
            PreCopy
#else
            Action<uint, IShellItem, IShellItem, string>
#endif
            PreCopyItem
        { get; set; }

        public
#if WinCopies3
            PostCopy
#else
Action<uint, IShellItem, IShellItem, string, IShellItem>
#endif
            PostCopyItem
        { get; set; }

        public
#if WinCopies3
            PreDelete
#else
            Action<uint, IShellItem>
#endif
            PreDeleteItem
        { get; set; }

        public
#if WinCopies3
            PostDelete
#else
Action<uint, IShellItem, IShellItem>
#endif
            PostDeleteItem
        { get; set; }

        public
#if WinCopies3
            PreNewItem
#else
Action<uint, IShellItem, string>
#endif
            PreNewItem
        { get; set; }

        public
#if WinCopies3
            PostNewItem
#else
            Action<uint, IShellItem, string, string, FileAttributes, IShellItem>
#endif
            PostNewItem
        { get; set; }

        public
#if WinCopies3
            UpdateProgress
#else
            Action<uint, uint>
#endif
            UpdateProgress
        { get; set; }

        public
#if WinCopies3
            Func<HResult>
#else
Action
#endif
            ResetTimer
        { get; set; }

        public
#if WinCopies3
            Func<HResult>
#else
Action
#endif
       PauseTimer
        { get; set; }

        public
#if WinCopies3
            Func<HResult>
#else
Action
#endif
       ResumeTimer
        { get; set; }

        public IFileOperationProgressSink GetFileOperationProgressSink() => InnerFileOperationProgressSink;

        #region IDisposable Support
        public bool IsDisposed => InnerFileOperationProgressSink == null;

        protected virtual void Dispose(bool disposing)
        {
            // if (!disposedValue)
            // {
            if (disposing)

                foreach (System.Reflection.PropertyInfo prop in GetType().GetProperties())

                    prop.SetValue(this, null);

            InnerFileOperationProgressSink = null;

            // disposedValue = true;
            // }
        }

        ~FileOperationProgressSink() => Dispose(false);

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class FileOperationProgressSinkDefault : FileOperationProgressSinkAbstract
    {
        protected FileOperationProgressSink FileOperationProgressSink { get; }

        public FileOperationProgressSinkDefault(FileOperationProgressSink fileOperationProgressSink) => FileOperationProgressSink = fileOperationProgressSink;

        // todo: replace by a call to WinCopies.Util.GetValue.

        private static HResult GetHResult(in HResult? value) => value ?? HResult.Ok;
        private static HResult GetHResult(Func<HResult> value) => value == null ? HResult.Ok : value();
        private static HResult GetHResult<T>(in Func<T, HResult> value, in T param) => value == null ? HResult.Ok : value(param);

        public override HResult StartOperations() => GetHResult(FileOperationProgressSink.StartOperations);

        public override HResult FinishOperations([In] HResult hr) => GetHResult(FileOperationProgressSink.FinishOperations, hr);

        public override HResult PreRenameItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName) => GetHResult(FileOperationProgressSink.PreRenameItem?.Invoke(dwFlags, psiItem, pszNewName));

        public override HResult PostRenameItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, IShellItem psiItem, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrRename, [In] IShellItem psiNewlyCreated) => GetHResult(FileOperationProgressSink.PostRenameItem?.Invoke(dwFlags, psiItem, pszNewName, hrRename, psiNewlyCreated));

        public override HResult PreMoveItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName) => GetHResult(FileOperationProgressSink.PreMoveItem?.Invoke(dwFlags, psiItem, psiDestinationFolder, pszNewName));

        public override HResult PostMoveItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrMove, [In] IShellItem psiNewlyCreated) => GetHResult(FileOperationProgressSink.PostMoveItem?.Invoke(dwFlags, psiItem, psiDestinationFolder, pszNewName, hrMove, psiNewlyCreated));

        public override HResult PreCopyItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName) => GetHResult(FileOperationProgressSink.PreCopyItem?.Invoke(dwFlags, psiItem, psiDestinationFolder, pszNewName));

        public override HResult PostCopyItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In] HResult hrCopy, [In] IShellItem psiNewlyCreated) => GetHResult(FileOperationProgressSink.PostCopyItem?.Invoke(dwFlags, psiItem, psiDestinationFolder, pszNewName, hrCopy, psiNewlyCreated));

        public override HResult PreDeleteItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem) => GetHResult(FileOperationProgressSink.PreDeleteItem?.Invoke(dwFlags, psiItem));

        public override HResult PostDeleteItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiItem, [In] HResult hrDelete, [In] IShellItem psiNewlyCreated) => GetHResult(FileOperationProgressSink.PostDeleteItem(dwFlags, psiItem, hrDelete, psiNewlyCreated));

        public override HResult PreNewItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName) => GetHResult(FileOperationProgressSink.PreNewItem?.Invoke(dwFlags, psiDestinationFolder, pszNewName));

        public override HResult PostNewItem([In, MarshalAs(UnmanagedType.U4)] TransferSourceFlags dwFlags, [In] IShellItem psiDestinationFolder, [In, MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [In, MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, [In, MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes, [In] HResult hrNew, [In] IShellItem psiNewItem) => GetHResult(FileOperationProgressSink.PostNewItem?.Invoke(dwFlags, psiDestinationFolder, pszNewName, pszTemplateName, dwFileAttributes, hrNew, psiNewItem));

        public override HResult UpdateProgress([In, MarshalAs(UnmanagedType.U4)] uint iWorkTotal, [In, MarshalAs(UnmanagedType.U4)] uint iWorkSoFar) => GetHResult(FileOperationProgressSink.UpdateProgress?.Invoke(iWorkTotal, iWorkSoFar));

        public override HResult ResetTimer() => GetHResult(FileOperationProgressSink.ResetTimer);

        public override HResult PauseTimer() => GetHResult(FileOperationProgressSink.PauseTimer);

        public override HResult ResumeTimer() => GetHResult(FileOperationProgressSink.ResumeTimer);
    }
}
#endif
