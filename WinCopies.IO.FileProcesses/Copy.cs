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

using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace WinCopies.IO.FileProcesses
{

    /// <summary>
    /// A class that provides static methods to copy files. The doc is from the following Microsoft's doc page: https://docs.microsoft.com/en-us/windows/desktop/api/winbase/nc-winbase-lpprogress_routine.
    /// </summary>
    public static class Copy
    {

        //TODO:

        //public static void Copy_Drives_Content(PathInfoBase path, string formated_Path, string formated_Copy_Path_Address, string dest_Path_3)
        //{



        //    System.IO.Directory.CreateDirectory(dest_Path_3);



        //} // end function ( void )

        //TODO:

        //public static void Copy_Folders(bool Is_A_File_Moving, PathInfoBase path, List<string> folder_Path, string formated_Path, string formated_Copy_Path_Address, string dest_Path_2, ref string dest_Path_3)
        //{
        //    //System.Windows.Forms.MessageBox.Show(dest_Path_3 );
        //    System.IO.Directory.CreateDirectory(dest_Path_3);

        //    if (Is_A_File_Moving)
        //    {

        //        folder_Path.Add(path.Path);          //      if (folder_Path.Count == 0) folder_Path.Add(path.Path);

        //        //        else
        //        //        {
        //        //      if (folder_Path[folder_Path.Count - 1].Length < path.Path.Length)
        //        //    {

        //        //    if (folder_Path[folder_Path.Count - 1] + path.Path.Substring(folder_Path[folder_Path.Count - 1].Length) == path.Path)

        //        //       folder_Path.Add(path.Path);

        //        //   else folder_Path.Clear();

        //        //   }

        //        //  else folder_Path.Clear();

        //        //   }

        //    }



        //} // end function ( void )

        //TODO: ? - erreur quand le répertoire de destination existe déjà (avec un fichier dedans) lors de la copie avec le répetoire de destination identique au répertoire de départ + '\' + qqch

        public static (Exceptions ex, int file, string fileName) Copy_Files(string oldFile, string newFile, bool Is_A_File_Moving, /* ref FileStream fs, ref FileStream fs2, */ bool overwrite, CopyProgressRoutine progressCallback)
        {

            Exceptions ex = Exceptions.None;

            //fs = null;

            //fs2 = null;

            // TODO :

            Directory.CreateDirectory(new FileInfo(newFile).DirectoryName);

            //TODO: ?

            // CopyProgressRoutine cpr = progressCallback;

            #region Comments
            // {

            // System.Windows.Forms.MessageBox.Show($"CopyFileEx : CopyProgressRoutine: {TotalFileSize.ToString()} {TotalBytesTransferred.ToString()}"

            // + $" { StreamSize.ToString()} {StreamBytesTransferred.ToString()} {dwStreamNumber.ToString()}"

            // + " " + dwCallbackReason.ToString());

            //Current_Copied_SizeProperty += fs.Position - Current_File_Copied_SizeProperty;



            //Current_File_Copied_SizeProperty = fs.Position;

            //return CopyProgressResult.PROGRESS_CONTINUE;

            // System.Windows.Forms.MessageBox.Show("");

            // };

            #endregion

            // Exceptions ex = Exceptions.None;

            // int error_Code = 0;

            bool pbCancel = false;

            // TODO : meilleure valeur ? --> no buffuring ?

            // TODO : rationnaliser des processus tels que celui-ci dans le processus général de la copie

            CopyFileFlags copyFileFlag = CopyFileFlags.NoBuffering | CopyFileFlags.Restartable;

            // if (overwrite) copyFileFlag |= CopyFileFlags.COPY_FILE_OPEN_SOURCE_FOR_WRITE;

            /*else*/

            if (overwrite) copyFileFlag |= CopyFileFlags.FailIfExists;

#if DEBUG

            Debug.WriteLine(Is_A_File_Moving);

            Debug.WriteLine(copyFileFlag.ToString());

#endif 

            //TODO:

            if (Is_A_File_Moving)
            {

                if (System.IO.Path.GetPathRoot(oldFile) != System.IO.Path.GetPathRoot(newFile))
                {

                    //fs = System.IO.File.OpenRead(newFile);

                    //fs2 = System.IO.File.OpenWrite(oldFile + @"\\" + dest_Path_2);

                    //fs.CopyTo(fs2);

                    (int file, string fileName) = CopyFile(oldFile, newFile, progressCallback, IntPtr.Zero, ref pbCancel, copyFileFlag);

                    // todo: to implement handlers for the other exceptions

                    //if (ex == Exceptions.None)

                    //{

                    try

                    {

                        File.Delete(oldFile);

                    }

                    catch (DirectoryNotFoundException)
                    {

                        ex = Exceptions.PathNotFound;

                    }

                    catch (PathTooLongException)

                    {

                        ex = Exceptions.FileNameTooLong;

                    }

                    catch (IOException)

                    {

                        ex = Exceptions.FileCheckedOut;

                    }

                    catch (UnauthorizedAccessException)

                    {

                        ex = Exceptions.AccessDenied;

                    }

                    // Assuming that when using this function, oldFile and newFile are both really files -- and not folders --, if ex has not the None value,
                    // that means that an exception has occurred. So, we need to try to remove the new file created, assuming this can cause a lot of waste time,
                    // but for logical reasons, we need to do not confuse the user. However, if the removal of the new file created and copied fail as well,
                    // we don't continue the process, also for logical reasons, because this process does not track this kind of exceptions.

                    if (ex != Exceptions.None)

                        try

                        {

                            File.Delete(newFile);

                        }

                        catch { }

                    //}

                    return (ex, file, fileName);

                }


                else
                {

                    // todo : deux fonctions distinctes afin que celle-ci puisse retourner une valeur ? - retourner une valeur ou déclencher une exception lors d'un problème avec la copie ? - voir des apis et les utiliser aussi directement aussi pour un déplacement

                    //    System.Windows.Forms.MessageBox.Show("a1");

                    //Exceptions ex = Exceptions.None;

                    int file = -1;

                    try
                    {

                        File.Move(oldFile, newFile);

                    }

                    catch (DirectoryNotFoundException)
                    {

                        ex = Exceptions.PathNotFound;

                        file = File.Exists(oldFile) ? 0 : 1;

                    }

                    catch (PathTooLongException)

                    {

                        ex = Exceptions.FileNameTooLong;

                        file = 1;

                    }

                    catch (IOException)

                    {

                        // This exception is more important than the other, so it is tested first.

                        if (!File.Exists(oldFile) && !Directory.Exists(oldFile))
                        {
                            ex = Exceptions.FileNotFound;

                            file = 0;
                        }

                        // This api does not tell us where the exception has appeared: on the old file or the new file, so we'll manage this with just an else:

                        else /* if (System.IO.File.Exists(newFile) || System.IO.Directory.Exists(newFile)) */
                        {
                            ex = Exceptions.FileAlreadyExists;

                            file = 1;

                        }

                    }

                    catch (UnauthorizedAccessException)

                    {

                        ex = Exceptions.AccessDenied;

                        file = File.Exists(oldFile) ? 0 : 1;

                    }

                    //   System.Windows.Forms.MessageBox.Show("b2");

                    return (ex, file, file == -1 ? null : (file == 0 ? oldFile : newFile));

                }

            }

            else
            {

                (int file, string fileName) = CopyFile(oldFile, newFile, progressCallback, IntPtr.Zero, ref pbCancel, copyFileFlag);

#if DEBUG

                //if (ex == Exceptions.None)

                //    Debug.WriteLine($"Fichier copié avec succès : {oldFile}");

                //else

                //    Debug.WriteLine($"Erreur lors de la copie du fichier : {oldFile}");

                // TODO : supprimer la référence à system.windows.forms

                Debug.WriteLine(copyFileFlag.ToString());

#endif 

                return ( ex, file, fileName);

            }



        } // end function ( void )

        public static (int file, string fileName) CopyFile(string lpExistingFileName, string lpNewFileName,
   CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref bool pbCancel,
   CopyFileFlags dwCopyFlags)

        {
            // ErrorCodes error_Code = 0;

            int file = -1;

            FileOperation.CopyFile(lpExistingFileName, lpNewFileName, lpProgressRoutine, lpData, ref pbCancel, dwCopyFlags);

            #region Comments

            //if (!copy_result)
            //{

            // error_Code = (ErrorCodes)Marshal.GetLastWin32Error();

            //#if DEBUG
            //            Debug.WriteLine("error_Code: " + error_Code.ToString());
            //#endif

            //if (error_Code != 0)

            //    // todo: to implement handlers for the other error codes 

            //    switch (error_Code)

            //    {

            //        case ErrorCodes.ERROR_FILE_NOT_FOUND:

            //            ex = Exceptions.FileNotFound;

            //            break;

            //        case ErrorCodes.ERROR_PATH_NOT_FOUND:

            //            ex = Exceptions.PathNotFound;

            //            break;

            //        case ErrorCodes.ERROR_ACCESS_DENIED:

            //            if (Directory.Exists(lpNewFileName))
            //                ex = Exceptions.FileAlreadyExists;

            //            else

            //                ex = Exceptions.AccessDenied;

            //            break;

            //        case ErrorCodes.ERROR_CURRENT_DIRECTORY:

            //            ex = Exceptions.DirectoryCannotBeRemoved;

            //            break;

            //        case ErrorCodes.ERROR_WRITE_PROTECT:

            //            ex = Exceptions.WriteProtected;

            //            break;

            //        case ErrorCodes.ERROR_INVALID_DRIVE:
            //        case ErrorCodes.ERROR_BAD_UNIT:
            //        case ErrorCodes.ERROR_SEEK:
            //        case ErrorCodes.ERROR_NOT_DOS_DISK:
            //        case ErrorCodes.ERROR_SECTOR_NOT_FOUND:
            //        case ErrorCodes.ERROR_WRITE_FAULT:
            //        case ErrorCodes.ERROR_READ_FAULT:
            //        case ErrorCodes.ERROR_WRONG_DISK:

            //            ex = Exceptions.ExceptionOnDeviceUnit;

            //            break;

            //        case ErrorCodes.ERROR_NOT_READY:

            //            ex = Exceptions.DiskNotReady;

            //            break;


            //        case ErrorCodes.ERROR_ALREADY_EXISTS:
            //        case ErrorCodes.ERROR_FILE_EXISTS:

            //            ex = Exceptions.FileAlreadyExists;

            //            break;

            //        case ErrorCodes.ERROR_BUFFER_OVERFLOW:
            //        case ErrorCodes.ERROR_FILENAME_EXCED_RANGE:

            //            ex = Exceptions.FileNameTooLong;

            //            break;

            //        case ErrorCodes.ERROR_DISK_FULL:
            //        case ErrorCodes.ERROR_HANDLE_DISK_FULL:

            //            ex = Exceptions.DiskFull;

            //            break;

            //        case ErrorCodes.ERROR_INVALID_NAME:
            //        case ErrorCodes.ERROR_DIRECTORY:

            //            ex = Exceptions.InvalidName;

            //            break;

            //        case ErrorCodes.ERROR_FILE_CHECKED_OUT:
            //        case ErrorCodes.ERROR_SHARING_VIOLATION:
            //        case ErrorCodes.ERROR_LOCK_VIOLATION:

            //            ex = Exceptions.FileCheckedOut;

            //            break;

            //        case ErrorCodes.ERROR_FILE_TOO_LARGE:

            //            ex = Exceptions.FileTooLarge;

            //            break;

            //        case ErrorCodes.ERROR_DISK_TOO_FRAGMENTED:

            //            ex = Exceptions.DiskTooFragmented;

            //            break;

            //        case ErrorCodes.ERROR_DELETE_PENDING:

            //            ex = Exceptions.DeletePending;

            //            break;

            //        case ErrorCodes.ERROR_NOT_ALLOWED_ON_SYSTEM_FILE:

            //            ex = Exceptions.NotAllowedOnSystemFile;

            //            break;

            //        case ErrorCodes.ERROR_REQUEST_ABORTED:

            //            ex = Exceptions.RequestCancelled;

            //            break;

            //        default:

            //            ex = Exceptions.Unknown;

            //            break;

            //    }

            #endregion

            bool handleValid = false;

            try
            {

                UnmanagedFileLoader loader = new UnmanagedFileLoader(lpExistingFileName);

                handleValid = loader.Handle != null;

                loader.Handle.Close();

                loader.Handle.Dispose();

            }

#if DEBUG

            catch (Exception _ex)

            {

                Debug.WriteLine(string.Format("Unable to create file handle : {0} ({1}). Exception message : {2}", lpExistingFileName, _ex.GetType().ToString(), _ex.Message));

                throw;

            }

#else

                finally 

                {

#endif

            file = handleValid ? 1 : 0;

#if !DEBUG

                }

#endif

            return (file, file == -1 ? null : (file == 0 ? lpExistingFileName : lpNewFileName));

        }

} // end class

} // end namespace