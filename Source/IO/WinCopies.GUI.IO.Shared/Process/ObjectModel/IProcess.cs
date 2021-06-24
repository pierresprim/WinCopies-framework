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
    public interface IProcessActions : DotNetFix.IDisposable
    {
        Action Run { get; }

        Action Pause { get; }

        Action Cancel { get; }

        Action<bool> TryAgain { get; }

        Action<bool> Ignore { get; }
    }

    public sealed class ProcessActions : IProcessActions
    {
        private IProcess _process;

        private IProcess Process => IsDisposed ? throw ThrowHelper.GetExceptionForDispose(false) : _process;

        public bool IsDisposed => _process == null;

        public Action Run => Process.RunWorkerAsync;

        public Action Pause { get; }

        public Action Cancel => Process.CancelAsync;

        public Action<bool> TryAgain => _TryAgain;

        public Action<bool> Ignore => _Ignore;

        public ProcessActions(in IProcess process) => _process = process;

        public void Dispose() => _process = null;

        private void _TryAgain(bool parameter)
        {
            if (parameter)

                _ = Process.Start();

            else

                _ = Process.RetryFirst();
        }

        private void _Ignore(bool parameter)
        {
            if (parameter)

                _ = Process.Ignore();

            else

                _ = Process.IgnoreFirst();
        }

        ~ProcessActions() => Dispose();
    }

    public interface IProcess : WinCopies.IO.Process.ObjectModel.IProcess, IBackgroundWorker, INotifyPropertyChanged
    {
        #region Properties
        bool IsCompleted { get; }

        bool IsPaused { get; }

        IPathCommon CurrentPath { get; }

        sbyte CurrentPathProgressPercentage { get; }

        uint ProgressPercentage { get; }

        IProcessActions ProcessActions { get; }
        #endregion

        void RunWorkerAsync();
    }
}
