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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using WinCopies.Controls;
using WinCopies.Util;
using BackgroundWorker = WinCopies.Util.BackgroundWorker;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace WinCopies.IO.FileProcesses
{

    // /// <summary>
    // /// Classe héritée de <see cref="WinCopies.Util.BackgroundWorker"/> qui expose des méthodes d'instance pour effectuer une action de recherche de dossiers et fichiers créés sur le disque en tant qu'éléments enfants des dossiers mentionnés dans la propriété <see cref="PathsInfo"/>.
    // /// </summary>

    /// <summary>
    /// Provides instance methods and properties for a file search process. This class inherits from <see cref="WinCopies.Util.BackgroundWorker"/>.
    /// </summary>
    public class FilesInfoLoader : IBackgroundWorker, INotifyPropertyChanged
    {

        #region BackgroundWorker implementation

        protected readonly BackgroundWorker _bgWorker = new BackgroundWorker();



        /// <summary>
        /// Gets the <see cref="System.Threading.ApartmentState"/> of this thread.
        /// </summary>
        public ApartmentState ApartmentState => _bgWorker.ApartmentState;

        /// <summary>
        /// Gets a value that indicates if the thread must try to cancel before finished the background tasks.
        /// </summary>
        public bool CancellationPending => _bgWorker.CancellationPending;

        /// <summary>
        /// Gets a value that indicates if the thread is busy.
        /// </summary>
        public bool IsBusy => _bgWorker.IsBusy;

        /// <summary>
        /// Gets a value that indicates if the working is cancelled.
        /// </summary>
        public bool IsCancelled => _bgWorker.IsCancelled;

        /// <summary>
        /// Gets the current progress of the working in percent.
        /// </summary>
        public int Progress => _bgWorker.Progress;

        public ISite Site { get => _bgWorker.Site; set => _bgWorker.Site = value; }

        /// <summary>
        /// Gets or sets a value that indicates if the thread can notify of the progress.
        /// </summary>
        public bool WorkerReportsProgress
        {
            get => _bgWorker.WorkerReportsProgress; set

            {

                if (value != _bgWorker.WorkerReportsProgress)

                {

                    object previousValue = _bgWorker.WorkerReportsProgress;

                    _bgWorker.WorkerReportsProgress = value;

                    OnPropertyChanged(nameof(WorkerReportsProgress), previousValue, value);

                }

            }

        }

        /// <summary>
        /// Gets or sets a value that indicates if the thread supports the cancellation.
        /// </summary>
        public bool WorkerSupportsCancellation
        {

            get => _bgWorker.WorkerSupportsCancellation; set

            {

                if (value != _bgWorker.WorkerSupportsCancellation)

                {

                    object previousValue = _bgWorker.WorkerSupportsCancellation;

                    _bgWorker.WorkerSupportsCancellation = value;

                    OnPropertyChanged(nameof(WorkerSupportsCancellation), previousValue, value);

                }

            }

        }

        /// <summary>
        /// Cancels the working asynchronously.
        /// </summary>
        public void CancelAsync() => _bgWorker.CancelAsync();

        /// <summary>
        /// Cancels the working.
        /// </summary>
        public void Cancel() => _bgWorker.Cancel();

        public void Dispose() => _bgWorker.Dispose();

        public void Suspend() => _bgWorker.Suspend();

        public void Resume() => _bgWorker.Resume();

        /// <summary>
        /// Notifies of the progress.
        /// </summary>
        /// <param name="percentProgress">
        /// Progress percentage.
        /// </param>
        public void ReportProgress(int percentProgress) => _bgWorker.ReportProgress(percentProgress);

        /// <summary>
        /// Notifies of the progress.
        /// </summary>
        /// <param name="percentProgress">
        /// Progress percentage.
        /// </param>
        /// <param name="userState">
        /// User object.
        /// </param>
        public void ReportProgress(int percentProgress, object userState) => _bgWorker.ReportProgress(percentProgress, userState);

        /// <summary>
        /// <para>This event is called when the background thread starts. Put your background working code here.</para>
        /// <para>The event handler is running in the background thread.</para>
        /// </summary>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// <para>This event is called when the background thread reports progress.</para>
        /// <para>The event handler is running in the main thread.</para>
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// <para>This event is called when the background thread has finished working.</para>
        /// <para>The event handler is running in the background thread.</para>
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        public event EventHandler Disposed;

        #endregion

        // todo:

        /// <summary>
        /// Gets the source path for all the files and folders in this process.
        /// </summary>
        public string SourcePath => Paths[0].FileSystemInfoProperties.FullName.EndsWith(":")
                                    || Paths[0].FileSystemInfoProperties.FullName.EndsWith(":\\")
                                    || Paths[0].FileSystemInfoProperties.FullName.EndsWith(":\\\\")
                    ? new DriveInfo(Paths[0].FileSystemInfoProperties.FullName).VolumeLabel
                    : System.IO.Path.GetDirectoryName(Paths[0].FileSystemInfoProperties.FullName);

        //TODO: Remplacer par un BG Worker personnalisé ? - OK.
        //public List<string> Hidden_Folders_With_Subpaths = new List<string>();

        private readonly ActionType actionType = ActionType.Unknown;

        /// <summary>
        /// Gets or sets the <see cref="FileProcesses. ActionType"/> of this <see cref="FilesInfoLoader"/>.
        /// </summary>
        public ActionType ActionType
        {

            get => actionType;

            set => OnPropertyChangedWhenNotBusy(nameof(ActionType), nameof(actionType), value, typeof(FilesInfoLoader));

        }

        //private readonly Search_Terms_Properties search_Terms = null;

        //public Search_Terms_Properties Search_Terms
        //{

        //    get => search_Terms;

        //    set => OnPropertyChangedWhenNotBusy(nameof(Search_Terms), nameof(search_Terms), value, typeof(FilesInfoLoader), true);

        //}

        //private bool loadOnlyItemsWithSearchTermsForAllActions = false;

        //public bool LoadOnlyItemsWithSearchTermsForAllActions { get => loadOnlyItemsWithSearchTermsForAllActions; set => OnPropertyChanged(nameof(LoadOnlyItemsWithSearchTermsForAllActions), nameof(loadOnlyItemsWithSearchTermsForAllActions), value, typeof(FilesInfoLoader)); }

        // TODO : avec un setter ? gérer les exceptions pour différents répertoires racines

        // private IList<FileSystemInfo> paths = null;

        /// <summary>
        /// Gets the paths to browse.
        /// </summary>
        public IList<FileSystemInfo> Paths { get; private set; } = null;

        // private FileSystemInfo _FileSystemInfoThatIsLoading = null;

        // TODO : vraiment utile ?

        //TODO : utile ?

        // public ObservableCollection<FileSystemInfo> _pathsLoaded = null;

        private System.Collections.ObjectModel.ObservableCollection<FileSystemInfo> _pathsLoaded { get; /*private*/ set; } = null;

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo> PathsLoaded { get; private set; } = null;

        // Structure : avoir des nombres plus réduits et adapter les unités

        private readonly Size _totalSize = new Size(0, SizeUnit.Byte);

        public Size TotalSize
        {

            get => _totalSize;

            private set => OnPropertyChanged(nameof(TotalSize), nameof(_totalSize), value, typeof(FilesInfoLoader));

        }

        // TODO : vraiment utile ?

        // private long totalFolders_ = 0;

        // private long _totalFolders = 0;

        // public long TotalFolders { get=>_totalFolders; private set=>OnPropertyChanged(nameof(TotalFolders), nameof(_totalFolders), value, typeof(FilesInfoLoader)); } 

        private readonly bool _IsLoaded = false;

        public bool IsLoaded { get => _IsLoaded; private set => OnPropertyChanged(nameof(IsLoaded), nameof(_IsLoaded), value, typeof(FilesInfoLoader)); }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

        }

        protected void OnPropertyChangedWhenNotBusy(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            (bool propertyChanged, object oldValue) = WinCopies.Util.Util.SetPropertyWhenNotBusy(this, propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

        }

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // /// <summary>
        // /// Événement déclenché quand la recherche est terminée.
        // /// </summary>
        // public event EventHandler<FileInfoLoadedEventArgs> FileInfoLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesInfoLoader"/> class.
        /// </summary>
        public FilesInfoLoader() => SetProperties();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesInfoLoader"/> class using custom parameters.
        /// </summary>
        /// <param name="paths">The paths to browse.</param>
        /// <param name="actionType">The <see cref="WinCopies.IO.FileProcesses. ActionType"/> to set this <see cref="FilesInfoLoader"/> for.</param>
        public FilesInfoLoader(IList<FileSystemInfo> paths, ActionType actionType)
        {

            SetProperties();

            Paths = paths;

            ActionType = actionType;

        } // end FilesInfoLoader

        private void SetProperties()
        {
            //TODO:
            /*ActionType = ActionType.Unknown;

            Search_Terms = null;

            LoadOnlyItemsWithSearchTermsForAllActions = false;*/



            WorkerReportsProgress = true;

            WorkerSupportsCancellation = true;



            _bgWorker.DoWork += FilesInfoLoader_DoWork;

            _bgWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressChanged?.Invoke(sender, e);

            _bgWorker.RunWorkerCompleted += FilesInfoLoader_RunWorkerCompleted;

            _bgWorker.Disposed += (object sender, EventArgs e) => Disposed?.Invoke(sender, e);

            _pathsLoaded = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

            PathsLoaded = new System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo>(_pathsLoaded);

            OnPropertyChanged(nameof(PathsLoaded), null, PathsLoaded);

        } // end void



        // protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        public void LoadAsync() => _bgWorker.RunWorkerAsync();



        private void FilesInfoLoader_DoWork(object sender, DoWorkEventArgs e)

        {

            Load();

            DoWork?.Invoke(sender, e);

        }

        private void FilesInfoLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)

        {

            IsLoaded = true;

            RunWorkerCompleted?.Invoke(sender, e);

        }



        /// <summary>
        /// Loads the items.
        /// </summary>
        public void Load()
        {

            // _pathsLoaded = new ObservableCollection<FileSystemInfo>();

#if DEBUG 

            Debug.WriteLine("Début du chargement");

#endif

            //string parent_Path = "";

            //if (((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Parent != null) parent_Path = ((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Parent.FullName;


            //    else
            //    {

            //        if (new DriveInfo(((System.IO.DirectoryInfo)path.FileSystemInfoProperties).Root.FullName).VolumeLabel == string.Empty)
            //        {

            //            parent_Path = ((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName;

            //            parent_Path = parent_Path.Substring(0, parent_Path.Length - 2);

            //        } // end if

            //        else

            //            parent_Path = new DriveInfo(((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName).VolumeLabel;


            //    } // end if



            if (actionType == ActionType.Recycling)

                _pathsLoaded = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>(Paths);

            else

                foreach (FileSystemInfo path in Paths)

#if DEBUG
                {

                    Debug.WriteLine("FilesInfoLoader log: " + path.FileSystemInfoProperties.FullName);

#endif

                    // parent_Path = "";



                    switch (path.FileType)
                    {

                        case FileType.Folder:
                        case FileType.Drive:

                            // TODO : vraiment utile ?

                            // ReportProgress(0);

                            // TotalFolders += 1;

                            List<int> pathsIndexes = new List<int>();

                            Type t = path.FileSystemInfoProperties.GetType();

                            DirectoryInfo directoriesInfo = (DirectoryInfo)path.FileSystemInfoProperties;

#if DEBUG

                            Debug.WriteLine("FilesInfoLoader log: " + (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/).ToString());

                            // Console.WriteLine("FilesInfoLoader log: "+ActionType.ToString() + " " + path.FileSystemInfoProperties.FullName + " " + path.FileType.ToString() + " " + LoadOnlyItemsWithSearchTermsForAllActions.ToString() + " " + Search_Terms.ToString());

#endif

                            if (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
                            {

                                _pathsLoaded.Add(path);

                                ReportProgress(0);

                                // System.Windows.Forms.MessageBox.Show("a" + IO.Path.Return_A_Path_With_One_Backslash_Per_Path(path.FileSystemInfoProperties.FullName) + "a" + " " + "b" + IO.Path.Return_A_Path_With_One_Backslash_Per_Path(new System.IO.DirectoryInfo(path.FileSystemInfoProperties.FullName).Root.FullName) + "b" + " " + (IO.Path.Return_A_Path_With_One_Backslash_Per_Path(path.FileSystemInfoProperties.FullName) != IO.Path.Return_A_Path_With_One_Backslash_Per_Path(new System.IO.DirectoryInfo(path.FileSystemInfoProperties.FullName).Root.FullName)).ToString());
                                //if (path.FileSystemInfoProperties.FullName != ((DirectoryInfo)path.FileSystemInfoProperties).Root.FullName)

                                //    if (directoriesInfo.Attributes.HasFlag(FileAttributes.Hidden) && (directoriesInfo.GetDirectories().Length > 0 || directoriesInfo.GetFiles().Length > 0))

                                //        Hidden_Folders_With_Subpaths.Add(path.FileSystemInfoProperties.FullName);

                                //else
                                //{

                                //    _pathsLoaded.Add(new FileSystemInfo(path.FileSystemInfoProperties, FileTypes.Drive));

                                //    ReportProgress(0);

                                //}


                                //FileSystemInfoLoaded = pathsLoaded[pathsLoaded.Count - 1];

                                // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSystemInfoThatIsLoading)));

                                //ReportProgress(0);

                            } // end if

                            //TODO : vraiment utile ?

                            // ReportProgress(0);

                            try
                            {

                                foreach (FileInfo file in directoriesInfo.GetFiles())

                                {

                                    //if (SearchMethods.AddFile(file, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
                                    //{

                                    _pathsLoaded.Add(new FileSystemInfo(file, FileType.File));

                                    TotalSize += file.Length;

                                    ReportProgress(0);

                                    //} // end if

                                } // next file

                            } // end try

                            catch (Exception)
                            { }

                            int pathSubdirectoriesCount = 0;

                            try
                            {

                                pathSubdirectoriesCount = ((DirectoryInfo)path.FileSystemInfoProperties).GetDirectories().Length;

                            } // end try

                            catch (Exception)
                            { }

                            pathsIndexes.Add(0);

                            int findIndex = 0;

                            while (pathsIndexes[0] < pathSubdirectoriesCount)
                            {

                                try
                                {

                                    DirectoryInfo[] directories = directoriesInfo.GetDirectories();

                                    while (directories.Length > 0)
                                    {



                                        DirectoryInfo directory = directories[pathsIndexes[findIndex]];

                                        directories = directory.GetDirectories();



                                        //if (directory.Attributes.HasFlag(FileAttributes.Hidden) && (directory.GetDirectories().Length > 0 || directory.GetFiles().Length > 0))

                                        //    Hidden_Folders_With_Subpaths.Add(directory.FullName);




                                        if (ActionType != ActionType.Deletion /*&& SearchMethods.AddFile(directory, FileType.Folder, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
                                        {

                                            _pathsLoaded.Add(new FileSystemInfo(directory, FileType.Folder));

                                            ReportProgress(0);

                                        } // end if



                                        foreach (FileInfo file in directory.GetFiles())

                                        {

                                            //if (SearchMethods.AddFile(file, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
                                            //{

                                            _pathsLoaded.Add(new FileSystemInfo(file, FileType.File));

                                            TotalSize += file.Length;

                                            ReportProgress(0);

                                            //} // end if

                                        } // next file

                                        if (ActionType == ActionType.Deletion /*&& SearchMethods.AddFile(directory, FileType.Folder, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
                                        {

                                            _pathsLoaded.Add(new FileSystemInfo(directory, FileType.Folder));

                                            ReportProgress(0);

                                            //FileSystemInfoLoaded = pathsLoaded[pathsLoaded.Count - 1];

                                            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileSystemInfoThatIsLoading)));

                                            //ReportProgress(0);

                                        } // end if

                                        pathsIndexes.Add(0);

                                        findIndex++;

                                    } // end while

                                } // end try

                                catch (Exception)
                                { findIndex++; }

                                do
                                {



                                    directoriesInfo = directoriesInfo.Parent;

                                    findIndex--;

                                    pathsIndexes[findIndex] += 1;



                                    for (int i = findIndex + 1; i < pathsIndexes.Count; i++)

                                        pathsIndexes.RemoveAt(findIndex + 1);



                                } while (directoriesInfo.GetDirectories().Length == pathsIndexes[findIndex] && pathsIndexes[0] != pathSubdirectoriesCount);

                            } // end while

                            if (ActionType == ActionType.Deletion /*&& SearchMethods.AddFile(path.FileSystemInfoProperties, path.FileType, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms)*/)
                            {

                                _pathsLoaded.Add(path);

                                ReportProgress(0);

                            } // end if

                            break;

                        case FileType.File:

                            try
                            {

#if DEBUG
                                Debug.WriteLine("FilesInfoLoader log: " + path.FileSystemInfoProperties.FullName + " (1)");
#endif

                                //if (SearchMethods.AddFile(path.FileSystemInfoProperties, FileType.File, ActionType, LoadOnlyItemsWithSearchTermsForAllActions, Search_Terms))
                                //{

#if DEBUG
                                Debug.WriteLine("FilesInfoLoader log: " + path.FileSystemInfoProperties.FullName + " (2)");
#endif 

                                _pathsLoaded.Add(path);

                                TotalSize += ((FileInfo)path.FileSystemInfoProperties).Length;

                                ReportProgress(0);

                                //} // end if

                            } // end try

                            catch (Exception) { }



                            break;

                    } // end switch
                      //System.Windows.Forms.MessageBox.Show(path.FileSystemInfoProperties.FullName);
#if DEBUG
                } // next
#endif

        } // end void

    } // end class

} // end namespace
