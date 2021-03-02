/* Copyright © Pierre Sprimont, 2020
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

using System.ComponentModel;

namespace WinCopies.GUI.IO.Process
{
    public interface IProcess // : IPausableBackgroundWorker
    {
        #region Properties
        IReadOnlyProcessCollection Paths { get; }

        IReadOnlyProcessErrorPathCollection ErrorPaths { get; }

        WinCopies.IO.Size InitialItemSize { get; }

#if !WinCopies3
int 
#else
        uint
#endif
            InitialItemCount { get; }

        bool IsCompleted { get; }

        string SourcePath { get; }

        bool ArePathsLoaded { get; }

        bool WorkerSupportsCancellation { get; set; }

        bool WorkerSupportsPausing { get; set; }

        bool WorkerReportsProgress { get; set; }

        bool IsBusy { get; }

        bool CancellationPending { get; }

        bool PausePending { get; }

        ProcessError Error { get; }

        IPathInfo CurrentPath { get; }

        int ProgressPercentage { get; }

#if DEBUG
        ProcessSimulationParameters SimulationParameters { get; }
#endif
#endregion

#region Events
        event ProgressChangedEventHandler ProgressChanged;

        event RunWorkerCompletedEventHandler RunWorkerCompleted;
#endregion

#region Methods
        string[] PathsToStringArray();

        void RunWorkerAsync();

        void RunWorkerAsync(object argument);

        void PauseAsync();

        void CancelAsync();
#endregion
    }
}
