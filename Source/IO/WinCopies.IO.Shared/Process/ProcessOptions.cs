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

namespace WinCopies.IO.Process
{
    public class ProcessOptions<T>
    {
        public Predicate<T> PathLoadedDelegate { get; }

        public bool ClearOnError { get; }

        public ProcessOptions(in Predicate<T> pathLoadedDelegate, in bool clearOnError)
        {
            PathLoadedDelegate = pathLoadedDelegate;

            ClearOnError = clearOnError;
        }
    }

    public class CopyProcessOptions<T> : ProcessOptions<T>
    {
        private int _bufferLength;

        /// <summary>
        /// Gets a value that indicates whether files are automatically renamed when they conflict with existing paths.
        /// </summary>
        public bool AutoRenameFiles { get; set; }

        public int BufferLength
        {
            get => _bufferLength;

            set
            {
                if (value < 0 ? throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} cannot be less than zero.") : value != _bufferLength)

                    _bufferLength = value;
            }
        }

        public bool IgnoreFolderFileNameConflicts { get; set; }

        public bool Move { get; }

        public CopyProcessOptions(in Predicate<T> pathLoadedDelegate, in bool clearOnError, in bool move = false) : base(pathLoadedDelegate, clearOnError) => Move = move;
    }
}
