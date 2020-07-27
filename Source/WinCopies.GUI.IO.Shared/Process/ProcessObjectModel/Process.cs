﻿/* Copyright © Pierre Sprimont, 2020
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
using System.ComponentModel;
using System.IO;
using System.Linq;

using WinCopies.Collections.DotNetFix;
using WinCopies.Util.Data;
using WinCopies.Util.DotNetFix;

using static WinCopies.Util.Util;
using static WinCopies.Util.Desktop.ThrowHelper;

using Size = WinCopies.IO.Size;
using System.Diagnostics;

namespace WinCopies.GUI.IO.Process
{
    public abstract class ProcessBase
#if DEBUG
         <TSimulationParameters>
#endif
        : ViewModelBase
#if DEBUG
        where TSimulationParameters : ProcessSimulationParameters
#endif
    {
        #region Private fields

        private Size _initialSize;
        private int _initialItemCount;
        private bool _completed = false;
        private bool _pathsLoaded = false;
        private readonly ObservableQueueCollection<IErrorPathInfo> _errorPaths = new ObservableQueueCollection<IErrorPathInfo>();
        private int _progressPercentage = 0;
        private IPathInfo _currentPath;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the inner background worker.
        /// </summary>
        protected PausableBackgroundWorker BackgroundWorker { get; } = new PausableBackgroundWorker();

        /// <summary>
        /// Gets or sets (protected) the initial total item size.
        /// </summary>
        public Size InitialItemSize
        {
            get => _initialSize;

            private set
            {
                _initialSize = value;

                OnPropertyChanged(nameof(InitialItemSize));
            }
        }

        /// <summary>
        /// Gets or sets (protected) the initial total item count.
        /// </summary>
        public int InitialItemCount
        {
            get => _initialItemCount;

            private set
            {
                _initialItemCount = value;

                OnPropertyChanged(nameof(InitialItemCount));
            }
        }

        protected internal ObservableQueueCollection<IPathInfo> _Paths { get; } = new ObservableQueueCollection<IPathInfo>();

        /// <summary>
        /// Gets the paths that have been loaded.
        /// </summary>
        public ProcessQueueCollection Paths { get; }

        /// <summary>
        /// Gets a value that indicates whether the process has completed.
        /// </summary>
        public bool IsCompleted
        {
            get => _completed;

            private set
            {
                _completed = value;

                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        /// <summary>
        /// Gets the source root path.
        /// </summary>
        public string SourcePath { get; }

        /// <summary>
        /// Gets a value that indicates whether all the paths and subpaths are loaded.
        /// </summary>
        public bool ArePathsLoaded
        {
            get => _pathsLoaded;

            private set
            {
                _pathsLoaded = value;

                OnPropertyChanged(nameof(ArePathsLoaded));
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the process supports cancellation.
        /// </summary>
        public bool WorkerSupportsCancellation
        {
            get => BackgroundWorker.WorkerSupportsCancellation; set
            {
                if (IsBusy)

                    ThrowBackgroundWorkerIsBusyException();

                if (value != BackgroundWorker.WorkerSupportsCancellation)
                {
                    BackgroundWorker.WorkerSupportsCancellation = value;

                    OnPropertyChanged(nameof(WorkerSupportsCancellation));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the process reports progress.
        /// </summary>
        public bool WorkerReportsProgress
        {
            get => BackgroundWorker.WorkerReportsProgress; set
            {
                if (IsBusy)

                    ThrowBackgroundWorkerIsBusyException();

                if (value != BackgroundWorker.WorkerReportsProgress)
                {
                    BackgroundWorker.WorkerReportsProgress = value;

                    OnPropertyChanged(nameof(WorkerReportsProgress));
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the process is busy.
        /// </summary>
        public bool IsBusy => BackgroundWorker.IsBusy;

        /// <summary>
        /// Gets a value that indicates whether a cancellation is pending.
        /// </summary>
        public bool CancellationPending => BackgroundWorker.CancellationPending;

        /// <summary>
        /// Gets or sets a value that indicates whether the process supports pausing.
        /// </summary>
        public bool WorkerSupportsPausing
        {
            get => BackgroundWorker.WorkerSupportsPausing; set
            {
                bool oldValue = BackgroundWorker.WorkerSupportsPausing;

                BackgroundWorker.WorkerSupportsPausing = value;

                if (value != oldValue) // We make this test after trying to update the inner BackgroundWorker property because this property checks if the BackgroundWorker is busy before updating the underlying value. Because this check has to be performed even if the new value is the same as the old one, in order to let the user know even in this case if there is a bug, and because this check is performed in the inner BackgroundWorker property, to make the check of this line here makes possible to let the user know if there is a bug in all cases, without performing the is-busy check twice.

                    OnPropertyChanged(nameof(WorkerSupportsPausing));
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a pause is pending.
        /// </summary>
        public bool PausePending => BackgroundWorker.PausePending;

        public ReadOnlyObservableQueueCollection<IErrorPathInfo> ErrorPaths { get; }

        /// <summary>
        /// Gets the global process error, if any.
        /// </summary>
        public ProcessError Error { get; private set; }

        /// <summary>
        /// Gets the current processed <see cref="IPathInfo"/>.
        /// </summary>
        public IPathInfo CurrentPath
        {
            get => _currentPath; protected set
            {
                if (value != _currentPath)
                {
                    _currentPath = value;

                    OnPropertyChanged(nameof(CurrentPath));
                }
            }
        }

        /// <summary>
        /// Gets the progress percentage of the current process.
        /// </summary>
        public int ProgressPercentage
        {
            get => _progressPercentage;

            private set
            {
                if (value != _progressPercentage)
                {
                    _progressPercentage = value;

                    OnPropertyChanged(nameof(ProgressPercentage));
                }
            }
        }

#if DEBUG
        public TSimulationParameters SimulationParameters { get; }
#endif

        #endregion

        #region Events

        public event DoWorkEventHandler DoWork;

        public event ProgressChangedEventHandler ProgressChanged;

        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        #endregion

        protected ProcessBase(in string sourcePath
#if DEBUG
             , in TSimulationParameters simulationParameters
#endif
            )
        {
            SourcePath = sourcePath;

            Paths = new ProcessQueueCollection(_Paths);

            ErrorPaths = new ReadOnlyObservableQueueCollection<IErrorPathInfo>(_errorPaths);



            BackgroundWorker.DoWork += BackgroundWorker_DoWork;

            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;



#if DEBUG
            SimulationParameters = simulationParameters;
#endif
        }

        #region Methods

        #region BackgroundWorker implementation

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) => OnDoWork(e);

        public void RunWorkerAsync() => BackgroundWorker.RunWorkerAsync();

        public void RunWorkerAsync(object argument) => BackgroundWorker.RunWorkerAsync(argument);



        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) => OnProgressChanged(e);

        protected void ReportProgress(int progressPercentage) => BackgroundWorker.ReportProgress(progressPercentage);

        protected void ReportProgress(int progressPercentage, object userState) => BackgroundWorker.ReportProgress(progressPercentage, userState);

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressPercentage = (e ?? throw GetArgumentNullException(nameof(e))).ProgressPercentage;

            ProgressChanged?.Invoke(this, e);
        }



        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted(e);

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            OnPropertyChanged(nameof(IsBusy));

            RunWorkerCompleted?.Invoke(this, e);
        }

        public void PauseAsync()
        {
            BackgroundWorker.PauseAsync();

            OnPropertyChanged(nameof(PausePending));
        }

        public void CancelAsync() => OnCancelAsync();

        protected virtual void OnCancelAsync()
        {
            BackgroundWorker.CancelAsync();

            OnPropertyChanged(nameof(CancellationPending));
        }

        #endregion

        #region Helpers

        protected void ThrowIfCompleted()
        {
            if (IsCompleted)

                throw new InvalidOperationException("The process has already been completed.");
        }

        public string[] PathsToStringArray()
        {
            string[] paths = new string[_Paths.Count];

            int i = 0;

            foreach (string path in _Paths.Select(_path => _path.Path))

                paths[i++] = path;

            return paths;
        }

        protected bool TryReportProgress(int progressPercentage)
        {
            if (WorkerReportsProgress)
            {
                ReportProgress(progressPercentage);

                return true;
            }

            return false;
        }

        #region Checks

        protected virtual bool CheckIfEnoughSpace()
        {
            if (Paths.Size.ValueInBytes.IsNaN)
            {
                Error = ProcessError.NotEnoughSpace;

                return false;
            }

            return true;
        }

        protected internal virtual bool CheckIfDriveIsReady()
        {
            string drive = System.IO.Path.GetPathRoot(SourcePath);

            bool _return(in ProcessError error, in bool value)
            {
                Debug.Assert(value == (error == ProcessError.None));

                Error = error;

                return value;
            }

            if (
#if DEBUG
                    SimulationParameters?.SourcePathRootExists ??
#endif
                    System.IO.Directory.Exists(drive))
            {
                var driveInfo = new DriveInfo(drive);

                if (
#if DEBUG
                    SimulationParameters?.SourceDriveReady ??
#endif
                    driveInfo.IsReady) return _return(ProcessError.None, true);
            }

            return _return(ProcessError.DriveNotReady, false);
        }

        protected internal virtual bool CheckIfPauseOrCancellationPending()
        {
            if (PausePending)

                Error = OnPausePending();

            if (CancellationPending)

                Error = OnCancellationPending();

            return Error != ProcessError.None;
        }

        #endregion

        #endregion

        protected virtual ProcessError OnPausePending() => ProcessError.AbortedByUser;

        protected virtual ProcessError OnCancellationPending()
        {
            _Paths.Clear();

            IsCompleted = true;

            return ProcessError.AbortedByUser;
        }

        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            OnPropertyChanged(nameof(IsBusy));

            DoWork?.Invoke(this, e);

            ThrowIfCompleted();

            LoadPaths(e);

            _DoWork(e);
        }

        protected void LoadPaths(DoWorkEventArgs e)
        {
            if ((Error = OnLoadPaths(e)) == ProcessError.None)
            {
                InitialItemSize = Paths.Size;

                InitialItemCount = _Paths.Count;

                ArePathsLoaded = true;
            }
        }

        protected void DequeueErrorPath(ProcessError error)
        {
            _errorPaths.Enqueue(new ErrorPathInfo(CurrentPath, error));

            _ = _Paths.Dequeue();
        }

        protected abstract ProcessError OnLoadPaths(DoWorkEventArgs e);

        protected void _DoWork(DoWorkEventArgs e)
        {
            Error = OnProcessDoWork(e);

            CurrentPath = null;

            if (Error == ProcessError.None && _errorPaths.Count == 0)

                IsCompleted = true;
        }

        protected abstract ProcessError OnProcessDoWork(DoWorkEventArgs e);

        #endregion
    }

    public abstract class Process<T
#if DEBUG
        , TSimulationParameters
#endif
        > : ProcessBase
#if DEBUG
         <TSimulationParameters> where TSimulationParameters : ProcessSimulationParameters
#endif
        where T : WinCopies.IO.IPathInfo
    {

        protected internal PathCollection<T> PathCollection { get; }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="Process"/> class.
        ///// </summary>
        protected Process(in PathCollection<T> paths
#if DEBUG
            , in TSimulationParameters simulationParameters
#endif
            ) : base(
#if DEBUG
             GetSourcePathFromPathCollection(paths),

                   simulationParameters
#endif
                )

         => PathCollection = paths;

        #region Methods

        public static string GetSourcePathFromPathCollection(in PathCollection<T> paths) => (paths ?? throw GetArgumentNullException(nameof(paths))).Path;

        //protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        //protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
