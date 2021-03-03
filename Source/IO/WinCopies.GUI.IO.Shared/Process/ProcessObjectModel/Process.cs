///* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;

//using WinCopies.Util.Data;

//using static WinCopies.Util.Desktop.ThrowHelper;

//using Size = WinCopies.IO.Size;

//#if !WinCopies3
//using WinCopies.Util.DotNetFix;

//using static WinCopies.Util.Util;
//#else
//using WinCopies.DotNetFix;

//using static WinCopies.ThrowHelper;
//#endif

//namespace WinCopies.GUI.IO.Process
//{
//#if DEBUG
//    /// <summary>
//    /// The base class for WinCopies IO processes.
//    /// </summary>
//    /// <typeparam name="TCollection">The type of the loaded paths collection.</typeparam>
//    /// <typeparam name="TReadOnlyCollection">The type of the loaded paths read-only collection.</typeparam>
//    /// <typeparam name="TErrorPathCollection">The type of the error paths collection.</typeparam>
//    /// <typeparam name="TReadOnlyErrorPathCollection">The type of the error paths read-only collection.</typeparam>
//    /// <typeparam name="TSimulationParameters">The type of the simulation parameters.</typeparam>
//#else
//    /// <summary>
//    /// The base class for WinCopies IO processes.
//    /// </summary>
//    /// <typeparam name="TCollection">The type of the loaded paths collection.</typeparam>
//    /// <typeparam name="TReadOnlyCollection">The type of the loaded paths read-only collection.</typeparam>
//    /// <typeparam name="TErrorPathCollection">The type of the error paths collection.</typeparam>
//    /// <typeparam name="TReadOnlyErrorPathCollection">The type of the error paths read-only collection.</typeparam>
//#endif
//    public abstract class ProcessBase<TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//         , TSimulationParameters
//#endif
//        >
//        : ViewModelBase

//        where TCollection : IProcessCollection
//        where TReadOnlyCollection : IReadOnlyProcessCollection
//        where TErrorPathCollection : IProcessErrorPathCollection
//        where TReadOnlyErrorPathCollection : IReadOnlyProcessErrorPathCollection
//#if DEBUG
//        where TSimulationParameters : ProcessSimulationParameters
//#endif
//    {
//        #region Private fields
//        private Size _initialSize;
//        private
//#if !WinCopies3
//int
//#else
//            uint
//#endif
//            _initialItemCount;
//        private bool _completed = false;
//        private bool _pathsLoaded = false;
//        private ProcessError _error;
//        private int _progressPercentage = 0;
//        private IPathInfo _currentPath;
//        private readonly TErrorPathCollection _errorPaths;
//        #endregion

//        #region Properties
//        protected internal TCollection _Paths { get; }

//        /// <summary>
//        /// Gets the paths that have been loaded.
//        /// </summary>
//        public TReadOnlyCollection Paths { get; }

//        public TReadOnlyErrorPathCollection ErrorPaths { get; }

//        /// <summary>
//        /// Gets the inner background worker.
//        /// </summary>
//        protected PausableBackgroundWorker BackgroundWorker { get; } = new PausableBackgroundWorker();

//        /// <summary>
//        /// Gets or sets (protected) the initial total item size.
//        /// </summary>
//        public Size InitialItemSize
//        {
//            get => _initialSize;

//            private set
//            {
//                _initialSize = value;

//                OnPropertyChanged(nameof(InitialItemSize));
//            }
//        }

//        /// <summary>
//        /// Gets or sets (protected) the initial total item count.
//        /// </summary>
//        public
//#if !WinCopies3
//            int
//#else
//            uint
//#endif
//            InitialItemCount
//        {
//            get => _initialItemCount;

//            private set
//            {
//                _initialItemCount = value;

//                OnPropertyChanged(nameof(InitialItemCount));
//            }
//        }

//        /// <summary>
//        /// Gets a value that indicates whether the process has completed.
//        /// </summary>
//        public bool IsCompleted
//        {
//            get => _completed;

//            private set
//            {
//                _completed = value;

//                OnPropertyChanged(nameof(IsCompleted));
//            }
//        }

//        /// <summary>
//        /// Gets the source root path.
//        /// </summary>
//        public string SourcePath { get; }

//        /// <summary>
//        /// Gets a value that indicates whether all the paths and subpaths are loaded.
//        /// </summary>
//        public bool ArePathsLoaded
//        {
//            get => _pathsLoaded;

//            private set
//            {
//                _pathsLoaded = value;

//                OnPropertyChanged(nameof(ArePathsLoaded));
//            }
//        }

//        /// <summary>
//        /// Gets the global process error, if any.
//        /// </summary>
//        public ProcessError Error
//        {
//            get => _error;

//            private set
//            {
//                if (value != _error)
//                {
//                    _error = value;

//                    OnPropertyChanged(nameof(Error));
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the current processed <see cref="IPathInfo"/>.
//        /// </summary>
//        public IPathInfo CurrentPath
//        {
//            get => _currentPath;

//            protected set
//            {
//                if (value != _currentPath)
//                {
//                    _currentPath = value;

//                    OnPropertyChanged(nameof(CurrentPath));
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the progress percentage of the current process.
//        /// </summary>
//        public int ProgressPercentage
//        {
//            get => _progressPercentage;

//            private set
//            {
//                if (value != _progressPercentage)
//                {
//                    _progressPercentage = value;

//                    OnPropertyChanged(nameof(ProgressPercentage));
//                }
//            }
//        }

//#if DEBUG
//        public TSimulationParameters SimulationParameters { get; }
//#endif
//        #endregion

//        protected ProcessBase(in string sourcePath, in TCollection pathCollection, in TReadOnlyCollection readOnlyPathCollection, in TErrorPathCollection errorPathCollection, TReadOnlyErrorPathCollection readOnlyErrorPathCollection
//#if DEBUG
//             , in TSimulationParameters simulationParameters
//#endif
//            )
//        {
//            ThrowIfNullEmptyOrWhiteSpace(sourcePath, nameof(sourcePath));
//#if !WinCopies3
//            ThrowIfNull(pathCollection, nameof(pathCollection));
//            ThrowIfNull(readOnlyPathCollection, nameof(readOnlyPathCollection));
//            ThrowIfNull(errorPathCollection, nameof(errorPathCollection));
//            ThrowIfNull(readOnlyErrorPathCollection, nameof(readOnlyErrorPathCollection));
//#else
//            void throwIfNull(object obj, string argumentName)
//            {
//                if (obj == null) throw GetArgumentNullException(argumentName);
//            }

//            throwIfNull(pathCollection, nameof(pathCollection));
//            throwIfNull(readOnlyPathCollection, nameof(readOnlyPathCollection));
//            throwIfNull(errorPathCollection, nameof(errorPathCollection));
//            throwIfNull(readOnlyErrorPathCollection, nameof(readOnlyErrorPathCollection));
//#endif

//            SourcePath = sourcePath;

//            _Paths = pathCollection;

//            Paths = readOnlyPathCollection;

//            _errorPaths = errorPathCollection;

//            ErrorPaths = readOnlyErrorPathCollection;



//            BackgroundWorker.DoWork += BackgroundWorker_DoWork;

//            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

//            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;



//#if DEBUG
//            SimulationParameters = simulationParameters;
//#endif
//        }

//        #region Methods
//        #region Helpers
//        /// <summary>
//        /// Throws an <see cref="InvalidOperationException"/> if the <see cref="IsCompleted"/> property is <see langword="true"/>.
//        /// </summary>
//        protected void ThrowIfCompleted()
//        {
//            if (IsCompleted)

//                throw new InvalidOperationException("The process has already been completed.");
//        }

//        protected void ThrowIfIsBusy()
//        {
//            if (BackgroundWorker.IsBusy)

//                throw GetBackgroundWorkerIsBusyException();
//        }

//        public string[] PathsToStringArray()
//        {
//            string[] paths = new string[_Paths.Count];

//            int i = 0;

//            foreach (string path in _Paths.Select(_path => _path.Path))

//                paths[i++] = path;

//            return paths;
//        }

//        protected bool TryReportProgress(int progressPercentage)
//        {
//            if (WorkerReportsProgress)
//            {
//                ReportProgress(progressPercentage);

//                return true;
//            }

//            return false;
//        }

//        #region Checks
//        protected virtual bool CheckIfEnoughSpace()
//        {
//            if (Paths.Size.ValueInBytes.IsNaN)
//            {
//                Error = ProcessError.NotEnoughSpace;

//                return false;
//            }

//            return true;
//        }

//        protected internal virtual bool CheckIfDriveIsReady()
//        {
//            string drive = System.IO.Path.GetPathRoot(SourcePath);

//            bool _return(in ProcessError error, in bool value)
//            {
//                Debug.Assert(value == (error == ProcessError.None));

//                Error = error;

//                return value;
//            }

//            if (
//#if DEBUG
//                    SimulationParameters?.SourcePathRootExists ??
//#endif
//                    System.IO.Directory.Exists(drive))
//            {
//                var driveInfo = new DriveInfo(drive);

//                if (
//#if DEBUG
//                    SimulationParameters?.SourceDriveReady ??
//#endif
//                    driveInfo.IsReady) return _return(ProcessError.None, true);
//            }

//            return _return(ProcessError.DriveNotReady, false);
//        }

//        /// <summary>
//        /// Checks if a cancellation or a pause is pending. See Remarks section.
//        /// </summary>
//        /// <returns>A value indicating whether a cancellation or a pause is pending.</returns>
//        /// <remarks><para>This method checks if a cancellation is pending, then, if not, checks if a pause is pending. These checks are performed by calling the <see cref="OnCancellationPending"/> and <see cref="OnPausePending"/> methods.</para>
//        /// <para>Each of these methods is called only if the corresponding -Pending property is <see langword="true"/>. If one of these methods is called, the <see cref="Error"/> property is assigned to the value returned by the one of these method that was called.</para>
//        /// <para>If one of these method is called, the other one is not.</para></remarks>
//        protected internal virtual bool CheckIfPauseOrCancellationPending()
//        {
//            bool _return(in ProcessError error) => (Error = error) != ProcessError.None;

//            if (CancellationPending)

//                return _return(OnCancellationPending());

//            else if (PausePending)

//                return _return(OnPausePending());

//            return false;
//        }
//        #endregion
//        #endregion

//        /// <summary>
//        /// Returns <see cref="ProcessError.AbortedByUser"/>.
//        /// </summary>
//        /// <returns>The <see cref="ProcessError.AbortedByUser"/> error code.</returns>
//        protected virtual ProcessError OnPausePending() => ProcessError.AbortedByUser;

//        /// <summary>
//        /// Clears <see cref="_Paths"/>, sets <see cref="IsCompleted"/> to <see langword="true"/> and returns <see cref="ProcessError.AbortedByUser"/>.
//        /// </summary>
//        /// <returns>The <see cref="ProcessError.AbortedByUser"/> error code.</returns>
//        protected virtual ProcessError OnCancellationPending()
//        {
//            _Paths.Clear();

//            IsCompleted = true;

//            return ProcessError.AbortedByUser;
//        }

//        /// <summary>
//        /// Starts the paths load and process operations. This method reset the <see cref="Error"/> property to <see cref="ProcessError.None"/>.
//        /// </summary>
//        /// <param name="e">The event args of the event that was raised originally.</param>
//        /// <exception cref="InvalidOperationException"><see cref="IsCompleted"/> is <see langword="true"/>.</exception>
//        protected virtual void OnDoWork(DoWorkEventArgs e)
//        {
//            OnPropertyChanged(nameof(IsBusy));

//            ThrowIfCompleted();

//            Error = ProcessError.None;

//            LoadPaths(e);

//            _DoWork(e);
//        }

//        /// <summary>
//        /// Calls <see cref="OnLoadPaths(DoWorkEventArgs)"/> and initializes the current process to process the loaded paths if <see cref="OnLoadPaths(DoWorkEventArgs)"/> returned a success error code. The <see cref="Error"/> property is assigned to the error code returned by <see cref="OnLoadPaths(DoWorkEventArgs)"/> anyway.
//        /// </summary>
//        /// <param name="e">The event args of the event that was raised originally.</param>
//        protected void LoadPaths(DoWorkEventArgs e)
//        {
//            if ((Error = OnLoadPaths(e)) == ProcessError.None)
//            {
//                InitialItemSize = Paths.Size;

//                InitialItemCount = _Paths.Count;

//                ArePathsLoaded = true;
//            }
//        }

//        /// <summary>
//        /// Adds the value of <see cref="CurrentPath"/> to <see cref="ErrorPaths"/> and removes it from <see cref="_Paths"/>.
//        /// </summary>
//        /// <param name="error">The error associated to the value of <see cref="CurrentPath"/>.</param>
//        protected void RemoveErrorPath(ProcessError error)
//        {
//            _errorPaths.Add(new ErrorPathInfo(CurrentPath, error));

//            _ = _Paths.Remove();
//        }

//        /// <summary>
//        /// When overridden in a derived class, loads the paths to process.
//        /// </summary>
//        /// <param name="e">The event args of the event that was raised originally.</param>
//        /// <returns>An error code for the current operation.</returns>
//        protected abstract ProcessError OnLoadPaths(DoWorkEventArgs e);

//        protected void _DoWork(DoWorkEventArgs e)
//        {
//            Error = OnProcessDoWork(e);

//            CurrentPath = null;

//            if (Error == ProcessError.None && _errorPaths.Count == 0)

//                IsCompleted = true;
//        }

//        protected abstract ProcessError OnProcessDoWork(DoWorkEventArgs e);
//        #endregion

//        #region BackgroundWorker implementation
//        #region Public properties
//        /// <summary>
//        /// Gets or sets a value that indicates whether the process supports cancellation.
//        /// </summary>
//        /// <exception cref="InvalidOperationException">When setting: The process is busy.</exception>
//        public bool WorkerSupportsCancellation
//        {
//            get => BackgroundWorker.WorkerSupportsCancellation; set
//            {
//                if (IsBusy)

//                    ThrowBackgroundWorkerIsBusyException();

//                if (value != BackgroundWorker.WorkerSupportsCancellation)
//                {
//                    BackgroundWorker.WorkerSupportsCancellation = value;

//                    OnPropertyChanged(nameof(WorkerSupportsCancellation));
//                }
//            }
//        }

//        /// <summary>
//        /// Gets or sets a value that indicates whether the process supports pausing.
//        /// </summary>
//        /// <exception cref="InvalidOperationException">When setting: The process is busy.</exception>
//        public bool WorkerSupportsPausing
//        {
//            get => BackgroundWorker.WorkerSupportsPausing;

//            set
//            {
//                bool oldValue = BackgroundWorker.WorkerSupportsPausing;

//                BackgroundWorker.WorkerSupportsPausing = value;

//                if (value != oldValue) // We make this test after trying to update the inner BackgroundWorker property because this property checks if the BackgroundWorker is busy before updating the underlying value. Because this check has to be performed even if the new value is the same as the old one, in order to let the user know even in this case if there is a bug, and because this check is performed in the inner BackgroundWorker property, to make the check of this line here makes possible to let the user know if there is a bug in all cases, without performing the is-busy check twice.

//                    OnPropertyChanged(nameof(WorkerSupportsPausing));
//            }
//        }

//        /// <summary>
//        /// Gets or sets a value that indicates whether the process reports progress.
//        /// </summary>
//        /// <exception cref="InvalidOperationException">When setting: The process is busy.</exception>
//        public bool WorkerReportsProgress
//        {
//            get => BackgroundWorker.WorkerReportsProgress; set
//            {
//                if (IsBusy)

//                    ThrowBackgroundWorkerIsBusyException();

//                if (value != BackgroundWorker.WorkerReportsProgress)
//                {
//                    BackgroundWorker.WorkerReportsProgress = value;

//                    OnPropertyChanged(nameof(WorkerReportsProgress));
//                }
//            }
//        }

//        /// <summary>
//        /// Gets a value that indicates whether the process is busy.
//        /// </summary>
//        public bool IsBusy => BackgroundWorker.IsBusy;

//        /// <summary>
//        /// Gets a value that indicates whether a cancellation is pending.
//        /// </summary>
//        public bool CancellationPending => BackgroundWorker.CancellationPending;

//        /// <summary>
//        /// Gets a value that indicates whether a pause is pending.
//        /// </summary>
//        public bool PausePending => BackgroundWorker.PausePending;
//        #endregion

//        #region Events
//        public event ProgressChangedEventHandler ProgressChanged;

//        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
//        #endregion

//        #region Methods
//        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) => OnDoWork(e);

//        /// <summary>
//        /// Starts the process at background.
//        /// </summary>
//        /// <exception cref="InvalidOperationException"><see cref="IsBusy"/> is <see langword="true"/>.</exception>
//        public void RunWorkerAsync() => BackgroundWorker.RunWorkerAsync();

//        /// <summary>
//        /// Starts the process at background.
//        /// </summary>
//        /// <param name="argument">A parameter for use by the process.</param>
//        /// <exception cref="InvalidOperationException"><see cref="IsBusy"/> is <see langword="true"/>.</exception>
//        public void RunWorkerAsync(object argument) => BackgroundWorker.RunWorkerAsync(argument);



//        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) => OnProgressChanged(e);

//        /// <summary>
//        /// Raises the <see cref="ProgressChanged"/> event of the <see cref="BackgroundWorker"/> property.
//        /// </summary>
//        /// <param name="progressPercentage">The percentage, from 0 to 100, of the background operation that is complete.</param>
//        /// <exception cref="InvalidOperationException">The <see cref="WorkerReportsProgress"/> property is set to <see langword="false"/>.</exception>
//        protected void ReportProgress(int progressPercentage) => BackgroundWorker.ReportProgress(progressPercentage);

//        /// <summary>
//        /// Raises the <see cref="ProgressChanged"/> event of the <see cref="BackgroundWorker"/> property.
//        /// </summary>
//        /// <param name="progressPercentage">The percentage, from 0 to 100, of the background operation that is complete.</param>
//        /// <param name="userState">The state object passed to <see cref="RunWorkerAsync(object)"/>.</param>
//        /// <exception cref="InvalidOperationException">The <see cref="WorkerReportsProgress"/> property is set to <see langword="false"/>.</exception>
//        protected void ReportProgress(int progressPercentage, object userState) => BackgroundWorker.ReportProgress(progressPercentage, userState);

//        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
//        {
//            ProgressPercentage = (e ?? throw GetArgumentNullException(nameof(e))).ProgressPercentage;

//            ProgressChanged?.Invoke(this, e);
//        }



//        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted(e);

//        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
//        {
//            OnPropertyChanged(nameof(IsBusy));

//            RunWorkerCompleted?.Invoke(this, e);
//        }

//        public void PauseAsync() => OnPauseAsync();

//        protected virtual void OnPauseAsync()
//        {
//            BackgroundWorker.PauseAsync();

//            OnPropertyChanged(nameof(PausePending));
//        }

//        public void CancelAsync() => OnCancelAsync();

//        protected virtual void OnCancelAsync()
//        {
//            BackgroundWorker.CancelAsync();

//            OnPropertyChanged(nameof(CancellationPending));
//        }
//        #endregion
//        #endregion
//    }

//    public abstract class Process<T, TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//        , TSimulationParameters
//#endif
//        > : ProcessBase<TCollection, TReadOnlyCollection, TErrorPathCollection, TReadOnlyErrorPathCollection
//#if DEBUG
//         , TSimulationParameters
//#endif
//            >, IProcess

//        where TCollection : IProcessCollection
//        where TReadOnlyCollection : IReadOnlyProcessCollection
//        where TErrorPathCollection : IProcessErrorPathCollection
//        where TReadOnlyErrorPathCollection : IReadOnlyProcessErrorPathCollection
//#if DEBUG
//         where TSimulationParameters : ProcessSimulationParameters
//#endif
//        where T : WinCopies.IO.IPathInfo
//    {

//        /// <summary>
//        /// Gets the <see cref="PathCollection{T}"/> from which to load the paths.
//        /// </summary>
//        protected internal PathCollection<T> PathCollection { get; }

//        IReadOnlyProcessCollection IProcess.Paths => Paths;

//        IReadOnlyProcessErrorPathCollection IProcess.ErrorPaths => ErrorPaths;

//        ProcessSimulationParameters IProcess.SimulationParameters => SimulationParameters;

//        ///// <summary>
//        ///// Initializes a new instance of the <see cref="Process"/> class.
//        ///// </summary>
//        protected Process(in PathCollection<T> paths, in TCollection pathCollection, in TReadOnlyCollection readOnlyPathCollection, in TErrorPathCollection errorPathCollection, TReadOnlyErrorPathCollection readOnlyErrorPathCollection
//#if DEBUG
//            , in TSimulationParameters simulationParameters
//#endif
//            ) : base(GetSourcePathFromPathCollection(paths), pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
//#if DEBUG
//                 , simulationParameters
//#endif
//                ) => PathCollection = paths;

//        #region Methods
//        /// <summary>
//        /// Returns the source path of a given <see cref="PathCollection{T}"/>.
//        /// </summary>
//        /// <param name="paths">The path collection from which to get the source path.</param>
//        /// <returns>The source path of <paramref name="paths"/>.</returns>
//        /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
//        public static string GetSourcePathFromPathCollection(in PathCollection<T> paths) => (paths ?? throw GetArgumentNullException(nameof(paths))).Path;

//        //protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

//        //protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        #endregion
//    }
//}
