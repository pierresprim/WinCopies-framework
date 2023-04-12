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

#if DUPLICATEFINDING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using WinCopies.Collections.DotNetFix;
using WinCopies.IO;
using WinCopies.Util;

using static WinCopies.Util.Util;

namespace WinCopies.GUI.IO.Process
{
    public class DuplicateFindingReadOnlyObservableLinkedCollection : ReadOnlyObservableLinkedCollection<IDuplicateFindingPathInfo>
    {
        protected internal new ObservableLinkedCollection<IDuplicateFindingPathInfo> InnerLinkedCollection => base.InnerLinkedCollection;

        public DuplicateFindingReadOnlyObservableLinkedCollection(ObservableLinkedCollection<IDuplicateFindingPathInfo> linkedCollection) : base(linkedCollection) { }
    }

    public enum DuplicateFindingStep : uint
    {
        None = 0,

        CheckForPathsToIgnore = 1,

        CheckForDuplicates = 2
    }

    public interface IDuplicateFindingPathInfo : IPathInfo
    {
        bool Delete { get; }
    }

    public class DuplicateFindingPathInfo : IDuplicateFindingPathInfo, INotifyPropertyChanged
    {
        public IPathInfo PathInfo { get; }

        public bool Delete { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DuplicateFindingPathInfo(in IPathInfo pathInfo) => PathInfo = pathInfo;

        protected virtual void OnPropertyChanged(in PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected virtual void OnPropertyChanged(in string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

#region IPathInfo implementation
        Size? IPathInfo.Size => PathInfo.Size;

        string WinCopies.IO.IPathInfo.Path => PathInfo.Path;

        bool WinCopies.IO.IPathInfo.IsDirectory => PathInfo.IsDirectory;
#endregion
    }

    public class DuplicateFindingDoWorkEventArgs : DoWorkEventArgs
    {
        public Func<ProcessError> Func { get; }

        public DuplicateFindingDoWorkEventArgs(Func<DuplicateFindingDoWorkEventArgs, ProcessError> func, object argument) : base(argument)
        {
            ThrowIfNull(func, nameof(func));

            Func = () => func(this);
        }

        public DuplicateFindingDoWorkEventArgs(Func<DuplicateFindingDoWorkEventArgs, ProcessError> func, DoWorkEventArgs e) : this(func, (e ?? throw GetArgumentNullException(nameof(e))).Argument)
        {
            Cancel = e.Cancel;

            Result = e.Result;
        }
    }

    public class DuplicateFinding : Process<WinCopies.IO.IPathInfo, ProcessLinkedCollection, ReadOnlyProcessLinkedCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , ProcessSimulationParameters
#endif
        >
    {
#region Private fields
        private int _bufferLength = 4096;
        private DuplicateFindingStep _step = 0;
        private IPathInfo _currentDuplicateCheckPath;
        private byte _currentDuplicateCheckPathProgressPercentage;
        //private bool _ignoreOnError = true;
        private readonly ObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection> _checkedPaths = new ObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection>();
#endregion

#region Public properties
        /// <summary>
        /// Gets or sets the buffer length for content duplicate checking.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting: The given value is less than or equal to zero.</exception>
        /// <exception cref="InvalidOperationException">The process is busy.</exception>
        public int BufferLength
        {
            get => _bufferLength;

            set
            {
                ThrowIfIsBusy();

                if (value <= 0)

                    throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be less than or equal to zero.");

                if (value != _bufferLength)
                {
                    _bufferLength = value;

                    OnPropertyChanged(nameof(BufferLength));
                }
            }
        }

        public DuplicateFindingOptions Options { get; }

        public DuplicateFindingStep Step
        {
            get => _step;

            private set
            {
                if (value != _step) // We need to make this check in case of resuming.
                {
                    _step = value;

                    OnPropertyChanged(nameof(Step));
                }
            }
        }

        public IPathInfo CurrentDuplicateCheckPath
        {
            get => _currentDuplicateCheckPath;

            private set
            {
                if (value != _currentDuplicateCheckPath)
                {
                    _currentDuplicateCheckPath = value;

                    OnPropertyChanged(nameof(CurrentDuplicateCheckPath));
                }
            }
        }

        public byte CurrentDuplicateCheckPathProgressPercentage
        {
            get => _currentDuplicateCheckPathProgressPercentage;

            private set
            {
                _currentDuplicateCheckPathProgressPercentage = value;

                OnPropertyChanged(nameof(CurrentDuplicateCheckPathProgressPercentage));
            }
        }

        //public bool IgnoreOnError
        //{
        //    get => _ignoreOnError;

        //    set
        //    {
        //        ThrowIfIsBusy();

        //        if (value != _ignoreOnError)
        //        {
        //            _ignoreOnError = value;

        //            OnPropertyChanged(nameof(IgnoreOnError));
        //        }
        //    }
        //}

        public ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection> CheckedPaths { get; }
#endregion

        public DuplicateFinding(DuplicateFindingOptions options)
        {
            CheckedPaths = new ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection>(_checkedPaths);

            Options = options ?? throw GetArgumentNullException(nameof(Options));
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            switch (_step)
            {
                case DuplicateFindingStep.None:

                    base.OnDoWork(new DuplicateFindingDoWorkEventArgs(_e => OnCheckForDuplicates(_e), e));

                    break;

                case DuplicateFindingStep.CheckForPathsToIgnore:

                    ThrowIfCompleted();

                    _DoWork(new DuplicateFindingDoWorkEventArgs(_e => OnCheckForPathsToIgnore(_e), e));

                    break;

                case DuplicateFindingStep.CheckForDuplicates:

                    ThrowIfCompleted();

                    _DoWork(new DuplicateFindingDoWorkEventArgs(_e => OnDuplicateCheck(_e), e));

                    break;
            }
        }

        /// <summary>
        /// Determines whether the current processed path should be ignored.
        /// </summary>
        /// <param name="e">The event args for this method.</param>
        /// <param name="result">A value that indicates whether the current processes path should be ignored.</param>
        /// <returns>A <see cref="ProcessError"/> error code.</returns>
        protected virtual ProcessError OnCheckIgnoring(DoWorkEventArgs e, out bool result)
        {
            DuplicateFindingIgnoreOptions ignoreOptions = Options.IgnoreOptions;

            DuplicateFindingIgnoreValues ignoreValues = ignoreOptions.IgnoreValues;

            ProcessError error = ProcessError.None;

            if (ignoreValues != DuplicateFindingIgnoreValues.None)
            {
                if ((error = ProcessHelper.TryGetFileInfo(CurrentPath.Path, out FileInfo fileInfo)) == ProcessError.None)

                    if (fileInfo.Exists)
                    {
                        if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.SystemFiles) && fileInfo.Attributes.HasFlag(FileAttributes.System)) || (ignoreValues.HasFlag(DuplicateFindingIgnoreValues.HiddenFiles) && fileInfo.Attributes.HasFlag(FileAttributes.Hidden)))
                        {
                            result = true;

                            return error;
                        }
                    }

                    else
                    {
                        RemoveErrorPath(ProcessError.PathNotFound);

                        result = true;

                        return ProcessError.None;
                    }

                else
                {
                    RemoveErrorPath(error);

                    result = true;

                    return ProcessError.None;
                }
            }

            if (!CurrentPath.Size.Value.ValueInBytes.IsNaN)
            {
                ulong size = CurrentPath.Size.Value.ValueInBytes.Value;

                if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.ZeroByteFiles) && size == 0)
                    || (ignoreOptions.FileSizeUnder.HasValue && size < ignoreOptions.FileSizeUnder)
                    || (ignoreOptions.FileSizeOver.HasValue && size > ignoreOptions.FileSizeOver))
                {
                    result = true;

                    return error;
                }
            }

#region Array checks

            if (ignoreOptions.Paths != null && ignoreOptions.Paths.Count > 0)

                foreach (string _path in ignoreOptions.Paths)

                    if (CurrentPath.Path.StartsWith(_path, StringComparison.CurrentCulture))
                    {
                        result = true;

                        return error;
                    }

            string value;

            if (ignoreOptions.FileNames != null && ignoreOptions.FileNames.Count > 0)
            {
                value = System.IO.Path.GetFileNameWithoutExtension(CurrentPath.Path);

                foreach (string fileName in ignoreOptions.FileNames)

                    if (value == fileName)
                    {
                        result = true;

                        return error;
                    }
            }

            if (ignoreOptions.FileExtensions != null && ignoreOptions.FileExtensions.Count > 0)
            {
                value = System.IO.Path.GetExtension(CurrentPath.Path);

                foreach (string extension in ignoreOptions.FileExtensions)

                    if (value == extension)
                    {
                        result = true;

                        return error;
                    }
            }

#endregion

            result = false;

            return error;
        }

        /// <summary>
        /// Checks if _Paths count is less than two. If _Paths count is equal to one, _Paths is cleared. Otherwise, nothing is done.
        /// </summary>
        /// <returns><see langword="true"/> if _Paths count is less than two; otherwise <see langword="false"/>.</returns>
        protected bool CheckIfTooLowPathCount()
        {
            if (_Paths.Count == 0)

                return true;

            if (_Paths.Count == 1)
            {
                _Paths.Clear();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for paths to ignore. If a path is ignored, it is directly removed from the loaded paths collection.
        /// </summary>
        /// <param name="e">The event args for this method.</param>
        /// <returns>A <see cref="ProcessError"/> error code.</returns>
        protected virtual ProcessError OnCheckForPathsToIgnore(in DoWorkEventArgs e)
        {
            Step = DuplicateFindingStep.CheckForPathsToIgnore;

            LinkedListNode<IPathInfo> node;
            ProcessError error = ProcessError.None;

            if (Options.IgnoreOptions != null)
            {
                bool ignore;

                void _checkIgnoring(DoWorkEventArgs _e)
                {
                    CurrentPath = node.Value;

                    if (CheckIfPauseOrCancellationPending())

                        return;

                    if ((error = OnCheckIgnoring(_e, out ignore)) == ProcessError.None && ignore)

                        _Paths.Remove(node);
                }

                if (CheckIfTooLowPathCount())

                    return ProcessError.None;

                ProcessError? pProcessError = null;

                bool checkIfError()
                {
                    bool _checkIfError(in ProcessError _processError)
                    {
                        if (_processError != ProcessError.None)
                        {
                            pProcessError = _processError;

                            return true;
                        }

                        return false;
                    }

                    return _checkIfError(Error) || _checkIfError(error);
                }

                node = _Paths.First;

                _checkIgnoring(e);

                if (checkIfError())

                    return pProcessError.Value;

                while (node.Next != null)
                {
                    node = node.Next;

                    _checkIgnoring(e);

                    if (checkIfError())

                        return pProcessError.Value;
                }
            }

            return ProcessError.None;
        }

        protected virtual ProcessError OnDuplicateCheck(DoWorkEventArgs e)
        {
            Step = DuplicateFindingStep.CheckForDuplicates;

            if (CheckIfTooLowPathCount())

                return ProcessError.None;

            LinkedListNode<IPathInfo> node;
            LinkedListNode<IPathInfo> nodeToCompare;

            //IPathInfo pathToCompare;

            DuplicateFindingMatchingOptions matchingOptions = Options.MatchingOptions;

            var duplicates = new WinCopies.Collections.DotNetFix.LinkedList<IDuplicateFindingPathInfo>();

            bool isDuplicate = false;

            bool hasDuplicates = false;

            var errorPaths = new Queue<IErrorPathInfo>();

            // bool? result;

            bool updateCurrentPathAndCheckIfErrorPath()
            {
                CurrentPath = node.Value;

                if (CurrentPath == errorPaths.Peek().PathInfo)
                {
                    RemoveErrorPath(errorPaths.Dequeue().Error);

                    return true;
                }

                return false;
            }

            bool _check()
            {
                bool? checkSize()
                {
                    if (matchingOptions.ContentMatchingOption == DuplicateFindingContentMatchingOption.Size)

                        return CurrentPath.Size.HasValue && _currentDuplicateCheckPath.Size.HasValue && CurrentPath.Size.Value == _currentDuplicateCheckPath.Size.Value;

                    else return null;
                }

                bool? checkName()
                {
                    if (matchingOptions.Name)

                        return System.IO.Path.GetFileName(CurrentPath.Path) == System.IO.Path.GetFileName(_currentDuplicateCheckPath.Path);

                    return null;
                }

                Result checkModifiedDate()
                {
                    if (matchingOptions.ModifiedDate)
                    {
                        ProcessError error = ProcessHelper.TryGetFileInfo(CurrentPath.Path, out FileInfo currentPathFileInfo);

                        if (error == ProcessError.None)

                            if ((error = ProcessHelper.TryGetFileInfo(_currentDuplicateCheckPath.Path, out FileInfo fileToCompareFileInfo)) == ProcessError.None)

                                return currentPathFileInfo.LastWriteTime == fileToCompareFileInfo.LastWriteTime ? Result.True : Result.False;

                            else
                            {
                                errorPaths.Enqueue(new ErrorPathInfo(_currentDuplicateCheckPath, error));

                                return Result.Error;
                            }

                        else
                        {
                            RemoveErrorPath(error);

                            return Result.Error;
                        }
                    }

                    else

                        return Result.None;
                }

                Result checkContent()
                {
                    if (matchingOptions.ContentMatchingOption == DuplicateFindingContentMatchingOption.Content)
                    {
                        FileStream getFileStream(in string path, Action<ProcessError> actionOnError)
                        {
                            try
                            {
                                return ProcessHelper.GetFileStream(path, _bufferLength);
                            }

                            catch (System.IO.IOException ex) when (ex.Is(false, typeof(System.IO.FileNotFoundException), typeof(System.IO.DirectoryNotFoundException)))
                            {
                                actionOnError(ProcessError.PathNotFound);
                            }

                            catch (System.IO.PathTooLongException)
                            {
                                actionOnError(ProcessError.PathTooLong);
                            }

                            catch (System.IO.IOException)
                            {
                                actionOnError(ProcessError.UnknownError);
                            }

                            catch (Exception ex) when (ex.Is(false, typeof(System.UnauthorizedAccessException), typeof(System.Security.SecurityException)))
                            {
                                actionOnError(ProcessError.ReadProtection);
                            }

                            return null;
                        }

                        FileStream left = getFileStream(CurrentPath.Path, errorValue => RemoveErrorPath(errorValue));

                        if (left == null)

                            return Result.Error;

                        FileStream right = getFileStream(_currentDuplicateCheckPath.Path, errorValue => errorPaths.Enqueue(new ErrorPathInfo(_currentDuplicateCheckPath, errorValue)));

                        if (right == null)

                            return Result.Error;

                        bool? result = WinCopies.IO.File.IsDuplicate(left, right, _bufferLength, () => CheckIfPauseOrCancellationPending());

                        return result.HasValue ? result.Value.ToResultEnum() : Result.Canceled;
                    }

                    else return Result.None;
                }

                bool _checkResult(in bool? value)
                {
                    if (value.HasValue)

                        if (value.Value)

                            isDuplicate = true;

                        else

                            return false;

                    return true;
                }

                bool? _checkResultEnumResult(in Result value)
#if CS7
                {
                    switch (value)
                    {
                        case Result.None:

                            return _checkResult(null);

                        case Result.Error:

                            return false;

                        case Result.True:

                            return _checkResult(true);

                        case Result.False:

                            return _checkResult(false);

                        case Result.Canceled:

                            return null;

                        default: // We should not reach this code path.

                            throw new WinCopies.Util.InvalidEnumArgumentException(nameof(value), value);
                    }
                }
#else
                    => value switch
                    {
                        Result.None => _checkResult(null),
                        Result.Error => false,
                        Result.True => _checkResult(true),
                        Result.False => _checkResult(false),
                        Result.Canceled => null,
                        // We should not reach this code path.
                        _ => throw new Util.InvalidEnumArgumentException(nameof(value), value)
                    };
#endif

                if (updateCurrentPathAndCheckIfErrorPath())

                    return true;

                CurrentDuplicateCheckPath = nodeToCompare.Value;

                if (_checkResult(checkSize()) && _checkResult(checkName()) && _checkResultEnumResult(checkModifiedDate()).Value)
                {
                    bool? checkContentResult = _checkResultEnumResult(checkContent());

                    if (checkContentResult.HasValue)
                    {
                        if (checkContentResult.Value && isDuplicate)
                        {
                            isDuplicate = false;

                            _ = duplicates.AddLast(new DuplicateFindingPathInfo(_currentDuplicateCheckPath));

                            hasDuplicates = true;
                        }
                    }

                    else

                        return false;
                }

                return true;
            }

            bool check()
            {
                if (node.Next != null)
                {
                    nodeToCompare = node.Next;

                    if (!_check())

                        return false;

                    while (nodeToCompare.Next != null)
                    {
                        if (CheckIfPauseOrCancellationPending())

                            return false;

                        nodeToCompare = nodeToCompare.Next;

                        if (!_check())

                            return false;
                    }

                    if (hasDuplicates)
                    {
                        hasDuplicates = false;

                        _ = duplicates.AddFirst(new DuplicateFindingPathInfo(CurrentPath));

                        _checkedPaths.Enqueue(new DuplicateFindingReadOnlyObservableLinkedCollection(new ObservableLinkedCollection<IDuplicateFindingPathInfo>(duplicates)));

                        duplicates = new WinCopies.Collections.DotNetFix.LinkedList<IDuplicateFindingPathInfo>();
                    }

                    _Paths.RemoveFirst();
                }

                else if (!updateCurrentPathAndCheckIfErrorPath())

                    _Paths.RemoveFirst();

                return true;
            }

            node = _Paths.First;

            if (check())
            {
                while (node.Next != null)
                {
                    if (CheckIfPauseOrCancellationPending())

                        return Error;

                    node = node.Next;

                    if (!check())

                        return Error;
                }

                return ProcessError.None;
            }

            return Error;
        }

        protected virtual ProcessError OnCheckForDuplicates(DoWorkEventArgs e)
        {
            var funcs = new Queue<Func<ProcessError>>();
            funcs.Enqueue(() => OnCheckForPathsToIgnore(e));
            funcs.Enqueue(() => OnDuplicateCheck(e));

            var error = ProcessError.None;

            IEnumerator<Func<ProcessError>> enumerator = funcs.GetEnumerator();

            funcs = null;

            while (enumerator.MoveNext() && (error = enumerator.Current()) == ProcessError.None)
            {
                // Left empty.
            }

            return error;
        }

        protected override ProcessError OnProcessDoWork(DoWorkEventArgs e) => e is DuplicateFindingDoWorkEventArgs _e ? _e.Func() : ProcessError.None;
    }
}

#endif
