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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using WinCopies.Util;
using BackgroundWorker = WinCopies.Util.BackgroundWorker;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace WinCopies.IO.FileProcesses
{

    //TODO : 'abstract' utile ?

    //TODO : revoir les commentaires xml pour cette classe

    /// <summary>
    /// Provides the base class for all processes manager in the WinCopies framework. This is an abstract class.
    /// </summary>
    public abstract class Process : IBackgroundWorker, INotifyPropertyChanged
    {

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

        // public string SourcePath { get => FilesInfoLoader.SourcePath; } 

        // public Size TotalSize => _filesInfoLoader.totalSize;  

        // private ActionType _actionType = ActionType.Unknown; 

        #region BackgroundWorker implementation

        private readonly BackgroundWorker _bgWorker = new BackgroundWorker();



        /// <summary>
        /// Gets the <see cref="System.Threading.ApartmentState"/> of this thread.
        /// </summary>
        public ApartmentState ApartmentState { get => _bgWorker.ApartmentState; }

        /// <summary>
        /// Gets a value that indicates if the thread must try to cancel before finished the background tasks.
        /// </summary>
        public bool CancellationPending { get => _bgWorker.CancellationPending; }

        /// <summary>
        /// Gets a value that indicates if the thread is busy.
        /// </summary>
        public bool IsBusy { get => _bgWorker.IsBusy; }

        /// <summary>
        /// Gets a value that indicates if the working is cancelled.
        /// </summary>
        public bool IsCancelled { get => _bgWorker.IsCancelled; }

        /// <summary>
        /// Gets the current progress of the working in percent.
        /// </summary>
        public int Progress { get => _bgWorker.Progress; }

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

        protected void Pause()

        {

            PausePending = false;

            IsPaused = true;

            _bgWorker.Suspend();

        }

        public void Resume()

        {

            PausePending = false;

            IsPaused = false;

            if (_bgWorker.IsBusy)

                _bgWorker.Resume();

            else

                _bgWorker.RunWorkerAsync();

        }

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

        /// <summary>
        /// Gets the type of this process.
        /// </summary>
        public abstract ActionType ActionType { get; }    // { get => _actionType; private    set => OnPropertyChanged(nameof(ActionType), nameof(_actionType), value);    } 

        #region Exceptions manager



        //TODO :



        // /// <summary>
        // /// Liste de toutes les exceptions s'étant déroulées dans le processus.
        // /// </summary>
        // public List<int> List_Of_Total_Exceptions { get; set; }



        // TODO : tout en public ? -- List<int> ?



        //        private Controls.ReadOnlyObservableCollection<int> _paths_With_Same_Name___Paths_Info_Read_Only = null;

        ///// <summary>
        ///// Liste des répertoires avec le même nom.
        ///// </summary>
        //public ReadOnlyObservableCollection<int> Paths_With_Same_Name___Paths_Info
        //{

        //    get
        //    {

        //        return _paths_With_Same_Name___Paths_Info_Read_Only.ReadOnlyObservableCollection;

        //    }

        //}



        // /// <summary>
        // /// Les exceptions pour lesquelles il faut relancer la copie. Certaines exceptions peuvent être évitées en définnissant des valeurs adéquates dans chaque instance de WinCopies.IO.PathInfo liée à un fichier présentant une exception.
        // /// </summary>
        // public Types_Of_Exceptions_Enum Types_Of_Exceptions_To_Retry { get; set; }

        // private Types_Of_Exceptions_Enum Types_Of_Exceptions_Occurred_Property { get; set; }

        #endregion

        // TODO : ?

        // private string _Volume_Label = string.Empty;

        // public string Volume_Label { get; }

        private int _startItemIndex = 0;

        // /// <summary>
        // /// Endroit à partir duquel la copie ou le déplacement doivent  (re) commencer dans la liste des répertoires à copier ou déplacer.
        // /// </summary>

        public int StartItemIndex { get => _startItemIndex; set => OnPropertyChanged(nameof(StartItemIndex), nameof(_startItemIndex), value, typeof(Process)); }

        // private FilesInfoLoader _filesInfoLoader = null;

        // TODO : utile en tant que champ/propriété ? meilleur nom ? 

        private FilesInfoLoader _FilesInfoLoader = null;

        /// <summary>
        /// Gets the loader of the files and folders for this process.
        /// </summary>
        public FilesInfoLoader FilesInfoLoader
        {
            get => _FilesInfoLoader; set
            {

                // FilesInfoLoader.RunWorkerCompleted += FilesInfoLoader_RunWorkerCompleted;

                // FilesInfoLoader.PathsInfo = pathsInfo;

                OnPropertyChanged(nameof(FilesInfoLoader), nameof(_FilesInfoLoader), value, typeof(Process));

                value.ActionType = ActionType.Copy;
            }
        }

        private bool _exceptionsOccurred = false;

        public bool ExceptionsOccurred { get => _exceptionsOccurred; protected set => OnPropertyChanged(nameof(ExceptionsOccurred), nameof(_exceptionsOccurred), value, typeof(Process)); }

        private readonly Exceptions _exceptionsToRetry = FileProcesses.Exceptions.None;

        public Exceptions ExceptionsToRetry { get => _exceptionsToRetry; set { OnPropertyChanged(nameof(ExceptionsToRetry), nameof(_exceptionsToRetry), value, typeof(Process)); } }

        private readonly HowToRetry _howToRetryWhenExceptionOccured = HowToRetry.None;

        /// <summary>
        /// Gets or sets a value that indicates how to retry to copy the file system objects.
        /// </summary>
        /// <remarks>If this property is set, this property have to be set to null individually for all the path items.</remarks>
        public HowToRetry HowToRetryWhenExceptionOccured
        {

            get => _howToRetryWhenExceptionOccured;

            set => OnPropertyChangedWhenNotBusy(nameof(HowToRetryWhenExceptionOccured), nameof(_howToRetryWhenExceptionOccured), value, typeof(CopyProcessInfo));

        }

        private bool _isPaused = false;

        public bool IsPaused { get => _isPaused; private set => OnPropertyChanged(nameof(IsPaused), nameof(_isPaused), value, typeof(Process)); }

        private bool _pausePending = false;

        public bool PausePending { get => _pausePending; private set => OnPropertyChanged(nameof(PausePending), nameof(_pausePending), value, typeof(Process)); }

        private FileSystemInfo _pausedFile = null;

        /// <summary>
        /// If a copy is paused during copying, this property gets the file paused, otherwise it returns null.
        /// </summary>
        public FileSystemInfo PausedFile { get => _pausedFile; protected set => OnPropertyChanged(nameof(PausedFile), nameof(_pausedFile), value, typeof(Process)); }

        private int _pausedIndex = -1;

        public int PausedIndex { get => _pausedIndex; protected set => OnPropertyChanged(nameof(PausedIndex), nameof(_pausedIndex), value, typeof(CopyProcessInfo)); }

        private System.Collections.ObjectModel.ObservableCollection<FileSystemInfo> _pausedFiles = null;

        protected System.Collections.ObjectModel.ObservableCollection<FileSystemInfo> _PausedFiles

        {

            get => _pausedFiles;

            set

            {

                _pausedFiles = value;

                PausedFiles = new System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo>(value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PausedFiles)));

            }

        }

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo> PausedFiles { get; private set; } = null;

        protected System.Collections.ObjectModel.ObservableCollection<FileSystemInfo> ExceptionsProtected { get; } = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

        /// <summary>
        /// Gets a <see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{FileSystemInfo}"/> which represents the files for which a <see cref="WinCopies.IO.FileProcesses.Exceptions"/> exception has occurred.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo> Exceptions { get; private set; } = null;

        private bool _stopProcessOnPause = true;

        /// <summary>
        /// Gets or sets a value that indicates if the process will stop on pause request.
        /// </summary>
        public bool StopProcessOnPause { get => _stopProcessOnPause; set => OnPropertyChanged(nameof(StopProcessOnPause), nameof(_stopProcessOnPause), value, typeof(CopyProcessInfo)); }

        public event PropertyChangedEventHandler PropertyChanged;

        //public event EventHandler<CopyProgressChangedEventArgs> ProgressChanged;

        //  public Process() => SetProperties(new FilesInfoLoader()); // { PathsInfo = null; }

        // public Process() => SetProperties();
        // {



        // _pathsInfo = new List<PathInfoBase>(pathsInfo);

        // _pathsInfoReadOnly = new ReadOnlyCollection<PathInfoBase>(_pathsInfo);



        // _filesInfoLoader.PathsInfo = pathsInfo;

        // _filesInfoLoader.ActionType = ActionType;





        // }

        // TODO : vraiment utile ? 

        /// <summary>
        /// Initializes a new instance of the <see cref="Process"/> class.
        /// </summary>
        public Process() => SetProperties();

        private void SetProperties()
        {

            // FilesInfoLoader = filesInfoLoader;

            Exceptions = new System.Collections.ObjectModel.ReadOnlyObservableCollection<FileSystemInfo>(ExceptionsProtected);

            WorkerReportsProgress = true;

            WorkerSupportsCancellation = true;

            _PausedFiles = new System.Collections.ObjectModel.ObservableCollection<FileSystemInfo>();

            _bgWorker.DoWork += (object sender, DoWorkEventArgs e) => DoWork?.Invoke(this, e);

            DoWork += (object sender, DoWorkEventArgs e) =>
            {

                OnDoWork(e);
            };

            _bgWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => ProgressChanged?.Invoke(sender, e);

            _bgWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted(e);

            _bgWorker.Disposed += (object sender, EventArgs e) => Disposed?.Invoke(sender, e);

        }

        // todo : copy tout-court pour le nom ?

        /// <summary>
        /// Starts the process.
        /// </summary>
        public void StartProcess() => StartProcess(false);

        /// <summary>
        /// Starts the process with the possibility to copy only the first item.
        /// </summary>
        /// <param name="onlyFirstFile">Defines whether to only process the first item</param>
        /// <exception cref="NullReferenceException">Exception thrown when the <see cref="FilesInfoLoader"/> property is null.</exception>
        /// <exception cref="Exception">Exception thrown when the load files info module is running or has not ran yet.</exception>
        /// <exception cref="ArgumentException">Exception thrown when onlyFirstFile is set to <see langword="true"/> and <see cref="ExceptionsToRetry"/> is set to <see cref="FileProcesses.Exceptions.None"/> or <see cref="HowToRetryWhenExceptionOccured"/> is set to <see cref="HowToRetry.Cancel"/>.</exception>
        public void StartProcess(bool onlyFirstFile)
        {

            //            CancellationPending = false;

            if (FilesInfoLoader == null)

                throw new NullReferenceException(nameof(FilesInfoLoader));

            if (FilesInfoLoader.IsBusy)

                throw new Exception(Generic.LoadFilesInfoModuleIsRunning);

            if (!FilesInfoLoader.IsLoaded)

                throw new Exception(Generic.LoadFilesInfoModuleHasNotRanYet);

            if (onlyFirstFile && (ExceptionsToRetry == FileProcesses.Exceptions.None || HowToRetryWhenExceptionOccured == HowToRetry.Cancel))

                throw new ArgumentException(string.Format(Generic.IncompatibleValues, nameof(onlyFirstFile), nameof(ExceptionsToRetry)));

            /*if (CopyFiles == null)*/ //CopyFiles = new CopyFiles(FilesInfoLoader.pathsLoaded, DestPath);

            //TODO:

            _bgWorker.RunWorkerAsync(onlyFirstFile);

        }

        public void Suspend() => PausePending = true;

        protected abstract void OnDoWork(DoWorkEventArgs e);

        private void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)

        {

            OnPropertyChanged(nameof(IsBusy), true, false);

            RunWorkerCompleted?.Invoke(this, e);

        }

        //TODO : vraiment utile ?

        //public void Initialize(bool reset_Paths_Info_Properties = false)
        //{

        //CancellationPendingProperty = false;

        //    if (reset_Paths_Info_Properties) { _totalSize = 0; }

        //}

        //public void ReportProgress(CopyProgressChangedEventArgs eventArgs)
        //{
        //    ProgressChanged?.Invoke(this, eventArgs);
        //}

        // public void FilesInfoLoader() => _filesInfoLoader.LoadAsync();
    }

}
