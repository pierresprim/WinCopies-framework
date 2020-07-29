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
using System.IO;

using WinCopies.Collections.DotNetFix;

using static WinCopies.Util.Desktop.ThrowHelper;
using static WinCopies.Util.Util;

namespace WinCopies.GUI.IO.Process
{
    public class DuplicateFindingReadOnlyObservableQueueCollection : ReadOnlyObservableQueueCollection<IPathInfo>
    {
        protected internal new ObservableQueueCollection<IPathInfo> InnerQueueCollection => base.InnerQueueCollection;

        public DuplicateFindingReadOnlyObservableQueueCollection(ObservableQueueCollection<IPathInfo> queueCollection) : base(queueCollection) { }
    }

    public class DuplicateFinding : Process<WinCopies.IO.IPathInfo, ProcessLinkedCollection, ReadOnlyProcessLinkedCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , ProcessSimulationParameters
#endif
        >
    {
        #region Private fields
        private int _bufferLength;
        private readonly ObservableQueueCollection<DuplicateFindingReadOnlyObservableQueueCollection> _checkedPaths = new ObservableQueueCollection<DuplicateFindingReadOnlyObservableQueueCollection>();
        #endregion

        #region Public properties
        public int BufferLength { get => _bufferLength; set => _bufferLength = BackgroundWorker.IsBusy ? throw GetBackgroundWorkerIsBusyException() : value < 0 ? throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be less than zero.") : value; }

        public DuplicateFindingOptions Options { get; }

        public ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableQueueCollection> CheckedPaths { get; }
        #endregion

        public DuplicateFinding(DuplicateFindingOptions options)
        {
            CheckedPaths = new ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableQueueCollection>(_checkedPaths);

            Options = options ?? throw GetArgumentNullException(nameof(Options));
        }

        protected override ProcessError OnProcessDoWork(DoWorkEventArgs e)
        {
            DuplicateFindingIgnoreOptions ignoreOptions = Options.IgnoreOptions;

            bool checkIgnoring(IPathInfo _path)
            {
                DuplicateFindingIgnoreValues ignoreValues = ignoreOptions.IgnoreValues;

                if (ignoreValues != DuplicateFindingIgnoreValues.None)
                {
                    var fileInfo = new FileInfo(_path.Path);

                    if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.SystemFiles) && fileInfo.Attributes.HasFlag(FileAttributes.System)) || (ignoreValues.HasFlag(DuplicateFindingIgnoreValues.HiddenFiles) && fileInfo.Attributes.HasFlag(FileAttributes.Hidden))) return true;
                }

                if (!_path.Size.Value.ValueInBytes.IsNaN)
                {
                    ulong size = _path.Size.Value.ValueInBytes.Value;

                    if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.ZeroByteFiles) && size == 0)
                        || (ignoreOptions.FileSizeUnder.HasValue && size < ignoreOptions.FileSizeUnder)
                        || (ignoreOptions.FileSizeOver.HasValue && size > ignoreOptions.FileSizeOver)) return true;
                }

                #region Array checks

                if (ignoreOptions.Paths != null && ignoreOptions.Paths.Count > 0)

                    foreach (string __path in ignoreOptions.Paths)

                        if (_path.Path.StartsWith(__path)) return true;

                string value;

                if (ignoreOptions.FileNames != null && ignoreOptions.FileNames.Count > 0)
                {
                    value = System.IO.Path.GetFileNameWithoutExtension(_path.Path);

                    foreach (string fileName in ignoreOptions.FileNames)

                        if (value == fileName) return true;
                }

                if (ignoreOptions.FileExtensions != null && ignoreOptions.FileExtensions.Count > 0)
                {
                    value = System.IO.Path.GetExtension(_path.Path);

                    foreach (string extension in ignoreOptions.FileExtensions)

                        if (value == extension) return true;
                }

                #endregion

                return false;
            }

            while (_Paths.Count > 0)
            {
                foreach (IPathInfo path in _Paths)
                {
                    if (CheckIfPauseOrCancellationPending())

                        return Error;

                    if (ignoreOptions != null && checkIgnoring(path))

                        break;
                }

                _ = _Paths.Dequeue();
            }
        }
    }
}
