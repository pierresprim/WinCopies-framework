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

using System;
using System.ComponentModel;

using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{
    public enum ProcessStatus : sbyte
    {
        None,

        InProgress,

        Succeeded,

        CancelledByUser,

        Error
    }

    public interface IProcess : WinCopies.IO.Process.ObjectModel.IProcess, IBackgroundWorker, INotifyPropertyChanged
    {
        #region Properties
        ProcessStatus Status { get; }

        bool IsCompleted { get; }

        bool IsPaused { get; }

        IPathCommon CurrentPath { get; }

        int ProgressPercentage { get; }

        Action RunAction
#if CS8
            => RunWorkerAsync;
#else
            { get; }
#endif

        Action PauseAction { get; }

        Action CancelAction
#if CS8
            => CancelAsync;
#else
            { get; }
#endif
        #endregion

        void RunWorkerAsync();
    }
}
