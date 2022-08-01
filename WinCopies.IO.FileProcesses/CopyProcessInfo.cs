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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using WinCopies.Util;
using static WinCopies.IO.FileProcesses.Copy;
using Microsoft.WindowsAPICodePack.Shell;

namespace WinCopies.IO.FileProcesses
{

    //TODO : supprimer les 'OK' des commentaires et revoir l'ensemble des commentaires

    //TODO : rationnaliser les classes pour les différents types de processus ?

    //TODO : d'autres éléments à mettre en abstract ou pas dans la classe de base (Process) ?

    //TODO : revérifier les commentaires xml pour cette classe !

    //TODO : classer les propriétés et les champs par ordre alphabétique



    /// <summary>
    /// Provides instance methods and properties to copy files and folders. This class inherits of <see cref="Process"/>.
    /// </summary>
    public class CopyProcessInfo : Process
    {

        private readonly ActionType _actionType = ActionType.Unknown;

        /// <summary>
        /// Gets the type of this process.
        /// </summary>
        public override ActionType ActionType => _actionType;

        private void SetActionType(ActionType value) => OnPropertyChanged(nameof(ActionType), nameof(_actionType), value, typeof(CopyProcessInfo));

        private readonly long _copiedFiles = 0;

        /// <summary>
        /// Gets the number of copied files.
        /// </summary>
        public long CopiedFiles
        {

            get => _copiedFiles;

            private set => OnPropertyChanged(nameof(CopiedFiles), nameof(_copiedFiles), value, typeof(CopyProcessInfo));

        }

        private readonly bool _overwrite = false;

        public bool Overwrite
        {

            get => _overwrite;

            set => OnPropertyChangedWhenNotBusy(nameof(Overwrite), nameof(_overwrite), value, typeof(CopyProcessInfo));

        }

        //TODO : 'internal' ? 'private' ?

        // public bool _Is_A_File_Moving = false;

        // todo : exception : 'exceptions' ? - nullable

        private readonly bool _isAFileMove = false;

        //TODO : 'OK' : - simplifier l'utilisation ? - nom / nomenclature du nom ? - voir également commenatire CopyFiles.cs

        // /// <summary>
        // /// Obtient ou défini une valeur qui indique si ce module effectue un déplacement de fichier. True pour déplacer directement les fichiers si le déplacement a lieu sur le même lecteur ou supprimer les originaux si le déplacement s'effectue sur plusieurs lecteurs différents. False pour copier à chaque fois tous les fichiers et conserver les originaux.
        // /// <br /><br />
        // /// Remarque : Renvoi null ou Nothing en VB si le module CopyFiles n'est pas instancié !
        // /// <br /><br />
        // /// Remarque : Si la valeur passée pour l'assignation est null ou Nothing en VB la propriété sera finalement assignée avec false pour ne pas créer une erreur de conversion .
        // /// </summary>

        /// <summary>
        /// Gets or sets a value that indicates if this module represents a file moving. True for moving directly the selected file system objects if the moving is running on the same drive for both the source and destination paths or deleting the original file system objects if the moving is running on multiple drives. False for copying all the file system objects each time and keep the original file system objects.
        /// </summary>
        public bool IsAFileMove
        {

            get => _isAFileMove; set

            {
                OnPropertyChangedWhenNotBusy(nameof(IsAFileMove), nameof(_isAFileMove), value, typeof(CopyProcessInfo)); SetActionType(value ? ActionType.Move : ActionType.Copy);

            }

        }

        //TODO : 'internal' - 'private' ?

        // public string _destPath = "";

        //TODO : ?



        private readonly FileSystemInfo _currentCopiedFile = null;

        // /// <summary>
        // /// Le fichier qui est en cours de copie ou de actuellement. S'il s'agit d'un déplacement et que le fichier est déplacé sur un autre lecteur de celui d'origine, il est d'abord copié, puis supprimé de son emplacement d'origine. Certains noms de fichiers ainsi que tous les noms de dossiers accessibles via cette propriété ne devraient donc pas y rester logntemps avant de laisser leur place aux éléments suivants si le processus est exécuté à une vitesse moyenne. Cette propriété est en lecture seule.
        // /// </summary>

        /// <summary>
        /// Gets the file which is being copied or moved.
        /// </summary>
        public FileSystemInfo CurrentCopiedFile
        {

            get => _currentCopiedFile;

            private set => OnPropertyChanged(nameof(CurrentCopiedFile), nameof(_currentCopiedFile), value, typeof(CopyProcessInfo));

        }

        //TODO : ?

        // /// <summary>
        // /// OK - Également accessible depuis le module CopyFiles de ce module-ci quand celui-ci est instancié. Emplacement où sont stockés les fichiers copiés
        // /// <br />
        // /// Remarque : Renvoi null ou Nothing en VB si le module CopyFiles n'est pas instancié !
        // /// </summary>

        /// <summary>
        /// Gets the destination path to copy or move the files.
        /// </summary>
        public string DestPath { get; } = null;

        private readonly Size _current_Copied_Size = new Size(0, SizeUnit.Byte);

        private readonly Size _current_File_Copied_Size = new Size(0, SizeUnit.Byte);

        /// <summary>
        /// Gets the total copied size.
        /// </summary>
        public Size CurrentCopiedSize { get => _current_Copied_Size; private set => OnPropertyChanged(nameof(CurrentCopiedSize), nameof(_current_Copied_Size), value, typeof(CopyProcessInfo)); }

        /// <summary>
        /// Gets the current file copied size.
        /// </summary>
        public Size CurrentFileCopiedSize { get => _current_File_Copied_Size; private set => OnPropertyChanged(nameof(CurrentFileCopiedSize), nameof(_current_File_Copied_Size), value, typeof(CopyProcessInfo)); }

        // todo : utiliser des observable collections "traditionnelles" ou bien des read-only wrapper, afin de respecter la logique

        // #region Exceptions collections

        private string _destPausedFilePath = null;

        public string DestPausedFilePath { get => _destPausedFilePath; private set => OnPropertyChanged(nameof(DestPausedFilePath), nameof(_destPausedFilePath), value, typeof(CopyProcessInfo)); }

        /// <summary>
        /// Initializes a new instance of <see cref="CopyProcessInfo"/> which will keep in memory the source files and folders.
        /// </summary>
        /// <param name="destPath">Destination folder path for copying the source files and folders</param>
        /// <param name="isAFileMove">True if the process is a file moving</param>
        // /// <param name="filesInfoLoader">Files info loader for this <see cref="CopyProcessInfo"/>.</param>
        public CopyProcessInfo(string destPath, bool isAFileMove) : base()
        {

            DestPath = destPath;

            IsAFileMove = isAFileMove;

        }

        //private void On_exceptions_CollectionItemRemovedAt(int index)

        //{

        //    ExceptionsProtected.RemoveAt(index);

        //    ExceptionsProtected.Sort(new FileSystemInfoComparer());

        //}



        // TODO : ou bien plutôt mettre directement cela dans le BGWorker

        //TODO: problème de back-slash - private ? vraiment utile en tant que méthode à part ? déplacer la conditionnelle pour ne pas la ré-itérer à chaque fois ?

        public string GetCopyPath(string oldFile, bool renamePath)
        {

            string path = DestPath + oldFile.Substring(System.IO.Path.GetDirectoryName(FilesInfoLoader.Paths[0].FileSystemInfoProperties.FullName).Length);

#if DEBUG 

            Debug.WriteLine($"DestPath : {path}, oldFile : {oldFile}");
            Debug.WriteLine(oldFile);
            Debug.WriteLine(FilesInfoLoader.Paths[0].FileSystemInfoProperties.FullName);
            Debug.WriteLine(FilesInfoLoader.PathsLoaded[0].FileSystemInfoProperties.FullName);

#endif

            return renamePath ? Path.RenamePathWithAutomaticNumber(oldFile, System.IO.Path.GetDirectoryName(path)) : path;

        }

        //private void FilesInfoLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) =>

        //    // todo : si le disque du répertoire de destination a un espace de stockage insuffisant, mettre une exception.

        //    FilesInfoLoaded(this, new EventArgs());

        //private void CopyFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{

        //    CopyProgressChangedEventArgs copyProgressChangedEventArgs = new CopyProgressChangedEventArgs((int)(CopyFiles.copiedFiles / PathsToCopy.Count * 100));

        //    ReportProgress(copyProgressChangedEventArgs);

        //}

        protected override void OnDoWork(DoWorkEventArgs e)
        {

            OnPropertyChanged(nameof(IsBusy), false, true);

            // #if DEBUG 
            //             MessageBox.Show("a");
            // #endif

            // int i = 0;

            // string formated_Path = "";

            // string formated_Copy_Path_Address = "";

            // string dest_Path_2 = "";

            // string dest_Path_3 = "";    



            bool onlyFirstFile = (bool)e.Argument;



            if (PausedFiles.Count == 0 && HowToRetryWhenExceptionOccured == HowToRetry.Cancel)

            {

                FileSystemInfo item = null;

                void copy()

                {

                    item = ExceptionsProtected[0];

                    item._exception = FileProcesses.Exceptions.None;

                    CopiedFiles += 1;

                    if (item.FileSystemInfoProperties.GetType() == typeof(FileInfo))

                        CurrentCopiedSize += ((FileInfo)item.FileSystemInfoProperties).Length;

                    ExceptionsProtected.RemoveAt(0);

                }

                if (onlyFirstFile)

                    copy();

                else

                    while (ExceptionsProtected.Count > 0)

                        copy();

                ExceptionsOccurred = ExceptionsProtected.Count == 0;

                return;

            }



            // List<string> folder_Path = new List<string>(); // Liste des répertoires à supprimer lors du déplacement une fois celui-ci effectué (déplacements uniquement).

            void getStartAndLength(Exceptions flag, out int _start, out int _length)

            {

                if (flag == FileProcesses.Exceptions.None)

                {

                    _start = StartItemIndex;

                    _length = FilesInfoLoader.PathsLoaded.Count;

                }

                else

                {

                    _start = ExceptionsProtected.IndexOf(ExceptionsProtected.First((FileSystemInfo f) => f.Exception == flag));

                    _length = onlyFirstFile ? 1 : ExceptionsProtected.IndexOf(ExceptionsProtected.Last((FileSystemInfo f) => f.Exception == flag)) + 1;

                }

            }

            int start;

            int length;

            if (PausedFiles.Count > 0)

                this_all_Files_DoWork(PausedIndex, PausedFiles.Count, _PausedFiles, _PausedFiles == ExceptionsProtected, true);

            else if (ExceptionsToRetry == FileProcesses.Exceptions.None)
            {

                getStartAndLength(FileProcesses.Exceptions.None, out start, out length);

                this_all_Files_DoWork(start, length, FilesInfoLoader.PathsLoaded);

            }

            else

            {

                Type type = ExceptionsToRetry.GetType();

                Enum enumValue = null;



                foreach (string s in type.GetEnumNames())

                {

                    enumValue = (Enum)Enum.Parse(type, s);



                    if (enumValue.GetNumValue(s).Equals(0)) continue;



                    if (ExceptionsToRetry.HasFlag(enumValue))

                    {

                        getStartAndLength((Exceptions)enumValue, out start, out length);

                        this_all_Files_DoWork(start, length, Exceptions, true);

                        ExceptionsOccurred = ExceptionsProtected.Count > 0; // If we are in a re-try process, we check if there are still paths in the exceptions list.

                    }

                }

            }

            // else if (ExceptionsToRetry.HasFlag(Exceptions.None))

            // }

        }

        // todo : nom + le 'i' ? + déplacer les répétitions dans une méthode : avoir un seul do work au final, une méthode pour récupérer les noms des fichiers et passer les collections des fichiers à copier en paramètres

        //private void this_MultipleEtcDoWork(int i)
        //{



        //}

        // ne pas pouvoir ré-appeler après une première exécution sans une remise à 0 des propriétés

        // ou bien alors remettre à 0 les collections sur les exceptions

        // todo : 'items' : files_To_Copy - paths ? 

        private void this_all_Files_DoWork(int start, int length, IList<FileSystemInfo> items, bool isARetry = false, bool isResuming = false)
        {

            Exceptions copyFile(FileSystemInfo _path, string destFilePath, int currentIndex, bool overwrite)

            {

                Exceptions ex = Copy_Files(_path.FileSystemInfoProperties.FullName, destFilePath, IsAFileMove, overwrite,
                            (
           long totalFileSize,
           long totalBytesTransferred,
           long streamSize,
           long streamBytesTransferred,
           uint dwStreamNumber,
           CopyProgressCallbackReason dwCallbackReason,
           IntPtr hSourceFile,
           IntPtr hDestinationFile,
           IntPtr lpData) =>
                            copyProgressCallback(_path, ref destFilePath, items, ref currentIndex, ref totalFileSize, ref totalBytesTransferred, ref streamSize, ref streamBytesTransferred, ref dwStreamNumber, dwCallbackReason, ref hSourceFile, ref hDestinationFile, ref lpData)).ex;

                if (ex != FileProcesses.Exceptions.None)

                    onException(_path, ex, 0);

                if (PausePending)

                    return ex;

                //if (ex == FileProcesses.Exceptions.None)

                //{

                //TODO: ? - Si c'est un déplacement sur le même lecteur, plutôt procéder par nbre de fichiers déplacés / s.

                if (IsAFileMove && System.IO.Path.GetPathRoot(FilesInfoLoader.SourcePath) == System.IO.Path.GetPathRoot(DestPath))
                {

                    long fileLength = ((FileInfo)_path.FileSystemInfoProperties).Length;

                    CurrentFileCopiedSize = (Size)fileLength;

                    CurrentCopiedSize += fileLength;

                }

                //}

                return ex ;

            }



            CopyProgressResult copyProgressCallback(FileSystemInfo currentPath,
                       ref string destFilePath,
                        IList<FileSystemInfo> currentFiles,
                        ref int currentIndex,
                        ref long TotalFileSize,
                      ref long TotalBytesTransferred,
                      ref long StreamSize,
                      ref long StreamBytesTransferred,
                      ref uint dwStreamNumber,
                      CopyProgressCallbackReason dwCallbackReason,
                      ref IntPtr hSourceFile,
                      ref IntPtr hDestinationFile,
                      ref IntPtr lpData)
            {

                // TODO : meilleure gestion ?

                if (dwStreamNumber > 1) return CopyProgressResult.Quiet;

                //TODO: 'TotalFileSize' : utiliser plutôt path.Length ? renommer Path.Length en Path.Size ?

                Size oldCurrent_File_Copied_Size = _current_File_Copied_Size;

                CurrentFileCopiedSize = (Size)StreamBytesTransferred;

                CurrentCopiedSize += StreamBytesTransferred - oldCurrent_File_Copied_Size;

#if DEBUG
                if (CurrentCopiedSize >= FilesInfoLoader.TotalSize) Debug.WriteLine(_copiedFiles.ToString() + " " + CurrentCopiedSize.ToString() + " " + FilesInfoLoader.TotalSize.ToString());
#endif

                // var fi = new FileInfo(currentPath.FileSystemInfoProperties.FullName);



                //TODO:

                ReportProgress(0);

                if (PausePending)

                {

                    PausedFile = currentPath;

                    DestPausedFilePath = destFilePath;

                    _PausedFiles = isARetry ? (System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>)currentFiles : new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>(currentFiles);

                    PausedIndex = currentIndex;



                    if (StopProcessOnPause) return CopyProgressResult.Stop;



                    Pause();

                }

                return CancellationPending ? CopyProgressResult.Cancel : CopyProgressResult.Continue;

            }



            #region pre-processing actions



            DriveInfo driveInfo = null;



            void onException(FileSystemInfo _path, Exceptions exception, int progress)

            {

                ExceptionsOccurred = true;

                if (_path._exception != exception)

                {

                    _path._exception = exception;

                    if (!isARetry)

                        ExceptionsProtected.Add(_path);

                }

                ReportProgress(progress);

            }



            FileSystemInfo path = null;

            int i = start;

            try

            {

                if (isResuming && PausedFile != null)

                {

                    Exceptions ex = copyFile(PausedFile, DestPausedFilePath, start, _overwrite || (isARetry && (PausedFile.HowToRetryToProcess == HowToRetry.Replace || HowToRetryWhenExceptionOccured == HowToRetry.Replace)));

                    if (PausePending)

                        return;

                    PausedFile = null;

                    DestPausedFilePath = null;

                    _PausedFiles = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

                    PausedIndex = -1;

                }

                else if (!isResuming)

                {

                    if (length == 1 && HowToRetryWhenExceptionOccured == HowToRetry.None && items[start].HowToRetryToProcess == HowToRetry.None)

                        return;



                    ExceptionsOccurred = false;



                    // if (isARetry && HowToRetryWhenExceptionOccured == HowToRetry.None) return;



                    // We check if the destination drive has enough space on disk if the destination drives are different or if the current process is a copy. 

                    driveInfo = new DriveInfo(System.IO.Path.GetPathRoot(DestPath));

                    if ((new DriveInfo(System.IO.Path.GetPathRoot(FilesInfoLoader.SourcePath)).RootDirectory != driveInfo.RootDirectory || !_isAFileMove) && driveInfo.AvailableFreeSpace <= FilesInfoLoader.TotalSize)

                    {

                        driveInfo = null;

                        ExceptionsOccurred = true;

                        foreach (FileSystemInfo _path in items)

                        {

                            _path._exception = FileProcesses.Exceptions.NotEnoughSpaceOnDisk;

                            ExceptionsProtected.Add(_path);

                        }

                        ExceptionsProtected.Sort(new FileSystemInfoComparer());

                        return;

                    }

                    else driveInfo = null;



                }

                #endregion



                // _length = onlyFirstFile ? 1 : items.Count;

                // TODO : for ? -- attention alors aux incrémentations manuelles / autre système ? 

                while (i < length && !CancellationPending)
                {

                    #region item pre-processing actions

                    path = items[i];

#if DEBUG

                    Debug.WriteLine($"Fichier à copier : {path.FileSystemInfoProperties.FullName}");

#endif 

                    string destFilePath = "";



                    bool rename = path.Exception == FileProcesses.Exceptions.FileAlreadyExists && (path.HowToRetryToProcess == HowToRetry.Rename || HowToRetryWhenExceptionOccured == HowToRetry.Rename);

                    destFilePath = GetCopyPath(path.FileSystemInfoProperties.FullName, rename);

                    if (DestPath.Length > path.FileSystemInfoProperties.FullName.Length && DestPath.Substring(path.FileSystemInfoProperties.FullName.Length).Contains("\\"))

                    {

                        onException(path, FileProcesses.Exceptions.DestPathIsASubdirectory, i / length * 100);

                        return;

                    }

                    if (PausePending)

                    {

                        PausedFile = path;

                        DestPausedFilePath = destFilePath;

                        _PausedFiles = isARetry ? (System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>)items : new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>(items);

                        PausedIndex = i;



                        if (StopProcessOnPause) return;

                        Pause();

                    }

                    if (isARetry && path.HowToRetryToProcess != HowToRetry.Retry && HowToRetryWhenExceptionOccured != HowToRetry.Retry && path.HowToRetryToProcess != HowToRetry.Rename && HowToRetryWhenExceptionOccured != HowToRetry.Rename && path.HowToRetryToProcess != HowToRetry.Replace && HowToRetryWhenExceptionOccured != HowToRetry.Replace)

                    {

                        if (path.HowToRetryToProcess != HowToRetry.None || HowToRetryWhenExceptionOccured != HowToRetry.None)

                        {

                            ExceptionsProtected.RemoveAt(i);

                            // items.RemoveAt(i);    

                            // todo : HowToRetry ...

                            //if (path.HowToRetryToProcess == HowToRetry.None)

                            //{

                            //    onException(path.Exception);

                            //    continue;

                            //}

                            if (path.HowToRetryToProcess == HowToRetry.Ignore || HowToRetryWhenExceptionOccured == HowToRetry.Ignore)

                            {

                                path._exception = FileProcesses.Exceptions.None;

                                CopiedFiles += 1;

                                if (path.FileSystemInfoProperties.GetType() == typeof(FileInfo))

                                    CurrentCopiedSize += ((FileInfo)path.FileSystemInfoProperties).Length;

                            }

                        }

                        // i++;

                        length--;

                        ReportProgress(0);

                        ExceptionsOccurred = i == length && ExceptionsProtected.Count > 0;

                        continue;

                    }



                    CurrentFileCopiedSize = (Size)0;

                    CurrentCopiedFile = path;

#if DEBUG

                    Debug.WriteLine($"Fichier de destination : {destFilePath + " " + path.FileSystemInfoProperties.FullName}");

#endif

                    #endregion

                    if (path.FileType == FileType.Drive || path.FileType == FileType.Folder)
#if DEBUG
                    {

                        Console.WriteLine($"Création du répertoire : {destFilePath}");
#endif

                        Directory.CreateDirectory(destFilePath);

#if DEBUG
                    }
#endif

                    else if (path.FileType == FileType.File)
                    {

#if DEBUG
                        Debug.WriteLine($"Copie du fichier : de {path.FileSystemInfoProperties.FullName} vers {destFilePath}");
#endif

                        Exceptions ex = copyFile(path, destFilePath, i, _overwrite || (isARetry && (path.HowToRetryToProcess == HowToRetry.Replace || HowToRetryWhenExceptionOccured == HowToRetry.Replace)));

                        if (PausePending) return;

                        if (ex == FileProcesses.Exceptions.None)

                            CopiedFiles += 1;

                        else

                            return;

                    }

                    //else if (ex == Exceptions.FileAlreadyExists)

                    //{

                    //    _file_Already_Exists.Add(path);

                    //}

                    //else if (ex == Exceptions.DirectoryNotFound)

                    //{

                    //    _directory_Not_Found.Add(path);

                    //}



                    //TODO:

                    // if (start_Item_Index + 1 == pathsToCopy.Count) start_Item_Index = 0; else start_Item_Index += 1;

                    //TODO:

                    ReportProgress(i / length * 100);

                    if (isARetry)

                    {

                        ExceptionsProtected.RemoveAt(0);

                        length--;

                    }

                    else

                        i++;

                }

            }

            catch (DirectoryNotFoundException)
            {

                onException(path, FileProcesses.Exceptions.PathNotFound, i / length * 100);

            }
            catch (PathTooLongException)
            {

                onException(path, FileProcesses.Exceptions.FileNameTooLong, i / length * 100);

            }
            // catch (IOException) { ex = FileProcesses.Exceptions.FileAlreadyExists; } 
            catch (UnauthorizedAccessException)
            {

                onException(path, FileProcesses.Exceptions.AccessDenied, i / length * 100);

            }

            catch (IOException)

            {

                if (new DriveInfo(System.IO.Path.GetPathRoot(FilesInfoLoader.SourcePath)).IsReady || new DriveInfo(System.IO.Path.GetPathRoot(DestPath)).IsReady)

                {

                    ExceptionsOccurred = true;

                    ExceptionsProtected.Clear();

                    foreach (FileSystemInfo _path in items)

                    {

                        _path._exception = FileProcesses.Exceptions.DiskNotReady;

                        ExceptionsProtected.Add(_path);

                    }

                    // break;

                }

                else

                    onException(path, FileProcesses.Exceptions.Unknown, i / length * 100);

            }

            // todo : maybe more functional with a dictionary which would create a collection corresponding to an exception when it'd be needed (this case would mean no sorting needed)

            ExceptionsProtected.Sort(new FileSystemInfoComparer());




#if DEBUG
            Debug.WriteLine("azertyuiop : " + Exceptions.Count((FileSystemInfo f) => { return f.Exception == FileProcesses.Exceptions.FileAlreadyExists; }).ToString());
#endif

        }

        //TODO : ?

        // /// <summary>
        // /// Libère toutes les ressources utilisées par le composant CopyFiles s'il est différent de null ou Nothing en Visual Basic.
        // /// </summary>

        // public void Dispose()
        // {

        //     if (CopyFiles != null) CopyFiles.Dispose();

        // }

    }

    public class FileSystemInfoComparer : IComparer<FileSystemInfo>

    {

        StringComparer sc = StringComparer.Create(CultureInfo.CurrentCulture, true);

        public int Compare(FileSystemInfo x, FileSystemInfo y)
        {

            if (x.Exception < y.Exception) return -1;

            else if (x.Exception == y.Exception) return 0;

            else return 1;

        }

    }

}
