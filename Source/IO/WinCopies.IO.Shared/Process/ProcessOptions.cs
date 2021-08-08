/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.IO.Process.ObjectModel;

namespace WinCopies.IO.Process
{
    public class ProcessOptionsBase<T>
    {
        public Predicate<T> PathLoadedDelegate { get; }

        public bool ClearOnError { get; }

        public ProcessOptionsBase(in Predicate<T> pathLoadedDelegate, in bool clearOnError)
        {
            PathLoadedDelegate = pathLoadedDelegate;

            ClearOnError = clearOnError;
        }
    }

    public class ProcessOptionsCommon<T> : ProcessOptionsBase<T>
    {
        private IProcess _process;

        public IProcess Process { get => _process; internal set => _process = _process == null ? value : throw new InvalidOperationException("Another process is already registered."); }

        public ProcessOptionsCommon(in Predicate<T> pathsLoadedDelegate, in bool clearOnError) : base(pathsLoadedDelegate, clearOnError)
        {
            // Left empty.
        }

        protected virtual bool UpdateValue<TValue>(ref TValue value, in TValue newValue) => Process.Status == ProcessStatus.Running
                ? throw new InvalidOperationException("The associated process is running.")
                : Temp.Temp.UpdateValue(ref value, newValue);

        internal void Dispose() => Process = default;
    }

    public class CopyOptions<T> : ProcessOptionsCommon<T>
    {
        private bool _autoRenameFiles;
        private int _bufferLength;
        private bool _ignoreFolderFileNameConflicts;

        /// <summary>
        /// Gets a value that indicates whether files are automatically renamed when they conflict with existing paths.
        /// </summary>
        public bool AutoRenameFiles { get => _autoRenameFiles; set => UpdateValue(ref _autoRenameFiles, value); }

        public int BufferLength { get => _bufferLength; set => UpdateValue(ref _bufferLength, value < 0 ? throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} cannot be less than zero.") : value); }

        public bool IgnoreFolderFileNameConflicts { get => _ignoreFolderFileNameConflicts; set => UpdateValue(ref _ignoreFolderFileNameConflicts, value); }

        public bool Move { get; }

        public CopyOptions(in Predicate<T> pathLoadedDelegate, in bool clearOnError, in bool move = false) : base(pathLoadedDelegate, clearOnError) => Move = move;

        public static CopyOptions<T> GetInstance(in Predicate<T> pathLoadedDelegate, in bool clearOnError, in bool move = false) => new CopyOptions<T>(pathLoadedDelegate, clearOnError, move);
    }

    public class DeletionOptions<T> : ProcessOptionsCommon<T>
    {
        public bool Recycle { get; }

        public DeletionOptions(in Predicate<T> pathLoadedDelegate, in bool clearOnError, in bool recycle = true) : base(pathLoadedDelegate, clearOnError) => Recycle = recycle;

        public static DeletionOptions<T> GetInstance(in Predicate<T> pathLoadedDelegate, in bool clearOnError, in bool recycle = false) => new DeletionOptions<T>(pathLoadedDelegate, clearOnError, recycle);
    }
}
