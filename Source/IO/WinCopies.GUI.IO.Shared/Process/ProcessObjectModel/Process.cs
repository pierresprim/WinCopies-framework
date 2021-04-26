//* Copyright © Pierre Sprimont, 2021
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.ComponentModel;
using System.Threading;

using WinCopies.DotNetFix;
using WinCopies.IO;
using WinCopies.IO.Process;
using WinCopies.Util;

namespace WinCopies.GUI.IO.Process
{
    public class Process : PausableBackgroundWorker, IProcess
    {
        #region Fields
        private bool _isCompleted;
        private WinCopies.IO.Process.ObjectModel.IProcess _process;
        private int _progress;
        private IPathCommon _currentPath;
        private bool _arePathsLoaded;
        private bool _isPaused;
        #endregion Fields

        #region Properties
        protected WinCopies.IO.Process.ObjectModel.IProcess InnerProcess => this.GetIfNotDisposed(_process);

        public IProcessErrorFactoryBase Factory => InnerProcess.Factory;

        public IProcessEventDelegates ProcessEventDelegates => InnerProcess.ProcessEventDelegates;

        public bool ArePathsLoaded { get => _arePathsLoaded; private set => UpdateValue(ref _arePathsLoaded, value, nameof(ArePathsLoaded)); }

        public bool IsDisposed { get; private set; }

        public string Name => InnerProcess.Name;

        public IPathCommon SourcePath => InnerProcess.SourcePath;

        public IProcessErrorFactoryData ProcessErrorFactoryData => InnerProcess.ProcessErrorFactoryData;

        public IProcessError Error => InnerProcess.Error;

        public Size InitialTotalSize => InnerProcess.InitialTotalSize;

        public uint InitialItemCount => InnerProcess.InitialItemCount;

        public Size ActualRemainingSize => InnerProcess.ActualRemainingSize;

        public ProcessTypes<WinCopies.IO.IPathInfo>.IProcessQueue Paths => InnerProcess.Paths;

        public ProcessTypes<IProcessErrorItem>.IProcessQueue ErrorPaths => InnerProcess.ErrorPaths;

        public bool IsCompleted { get => _isCompleted; private set => UpdateValue(ref _isCompleted, value, nameof(IsCompleted)); }

        // public ApartmentState ApartmentState { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool IsCancelled => Error.Error == ProcessErrorFactoryData.CancelledByUserError && !_isPaused;

        public bool IsPaused { get => !IsBusy && _isPaused; private set => UpdateValue(ref _isPaused, value, nameof(IsPaused)); }

        public int Progress { get => _progress; private set { UpdateValue(ref _progress, value, nameof(Progress)); OnPropertyChanged(nameof(ProgressPercentage)); } }

        public int ProgressPercentage => Progress;

        public IPathCommon CurrentPath { get => _currentPath; private set => UpdateValue(ref _currentPath, value, nameof(CurrentPath)); }

        Action IProcess.PauseAction => PauseAsync;

#if !CS8
        Action IProcess.RunAction => RunWorkerAsync;

        Action IProcess.CancelAction => CancelAsync;
#endif

        ApartmentState IBackgroundWorker.ApartmentState { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public string Guid => InnerProcess.Guid;
        #endregion Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public Process(in WinCopies.IO.Process.ObjectModel.IProcess process)
        {
            _process = process;
            _process.AddPropertyChangedDelegate(OnPropertyChanged);
            _process.ProcessEventDelegates.AddCommonDelegate(new QueryDelegateDelegate<IProcessProgressDelegateParameter, bool>(UpdateProgress, (value, previousResult) => UpdateProgress(value)));
            _process.ProcessEventDelegates.AddProgressDelegate(path => CurrentPath = path);

            DoWork += Process_DoWork;

            RunWorkerCompleted += Process_RunWorkerCompleted;
        }

        #region Methods
        public void AddPropertyChangedDelegate(Action<string> action) => InnerProcess.AddPropertyChangedDelegate(action);

        public void RemovePropertyChangedDelegate(Action<string> action) => InnerProcess.RemovePropertyChangedDelegate(action);

        private void Process_DoWork(object sender, DoWorkEventArgs e) => OnDoWork(e);

        private void Process_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) => OnRunWorkerCompleted();

        private bool UpdateProgress(IProcessProgressDelegateParameter value)
        {
            Progress = value.Progress;

            if (PausePending)
            {
                _isPaused = true;

                return false;
            }

            return !CancellationPending;
        }

        protected virtual void OnDoWork(in DoWorkEventArgs e)
        {
            IsCompleted = false;
            IsPaused = false;

            if (ArePathsLoaded)

                _ = InnerProcess.Start();

            else
            {
                _ = LoadPaths();

                ArePathsLoaded = InnerProcess.ArePathsLoaded;

                if (InnerProcess.ArePathsLoaded)

                    _ = InnerProcess.Start();
            }
        }

        protected virtual void OnRunWorkerCompleted() => IsCompleted = true;

        protected virtual void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        protected virtual void OnPropertyChanged(in PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected virtual void UpdateValue<T>(ref T value, in T newValue, PropertyChangedEventArgs e) => UtilHelpers.UpdateValue(ref value, newValue, () => OnPropertyChanged(e));

        protected virtual void UpdateValue<T>(ref T value, in T newValue, string propertyName) => UtilHelpers.UpdateValue(ref value, newValue, () => OnPropertyChanged(propertyName));

        public bool LoadPaths() => InnerProcess.LoadPaths();

        public bool Start() => InnerProcess.Start();

        public void Reset() => InnerProcess.Reset();

        protected virtual void DisposeManaged()
        {
            _process.Dispose();
            _process = null;

            IsDisposed = true;
        }

        protected virtual void DisposeUnmanaged() { /* Left empty. */ }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)

                DisposeManaged();

            DisposeUnmanaged();
        }

        private static void ThrowNotSupportedException() => throw new NotSupportedException("This operation is not supported for this type of BackgroundWorker.");

        void IBackgroundWorker.Cancel() => ThrowNotSupportedException();

        void IBackgroundWorker.Cancel(object stateInfo) => ThrowNotSupportedException();

        void IBackgroundWorker.CancelAsync(object stateInfo) => CancelAsync();

        void IBackgroundWorker.Suspend() => ThrowNotSupportedException();

        void IBackgroundWorker.Resume() => ThrowNotSupportedException();
        #endregion Methods
    }
}
