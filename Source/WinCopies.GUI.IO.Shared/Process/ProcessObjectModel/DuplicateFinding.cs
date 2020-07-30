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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using WinCopies.Collections.DotNetFix;
using WinCopies.IO;

using static WinCopies.Util.Desktop.ThrowHelper;
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

        CheckingForPathsToIgnore = 1,

        CheckingForDuplicates = 2,

        DuplicatesDeletion = 3
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

        public DuplicateFindingDoWorkEventArgs(Func<DuplicateFindingDoWorkEventArgs, ProcessError> func, object argument) : base(argument) => Func = () => func(this);

        public DuplicateFindingDoWorkEventArgs(Func<DuplicateFindingDoWorkEventArgs, ProcessError> func, DoWorkEventArgs e) : base(e.Argument)
        {
            Cancel = e.Cancel;

            Result = e.Result;

            Func = () => func(this);
        }
    }

    public class DuplicateFinding : Process<WinCopies.IO.IPathInfo, ProcessLinkedCollection, ReadOnlyProcessLinkedCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , ProcessSimulationParameters
#endif
        >
    {
        #region Private fields
        private int _bufferLength;
        private DuplicateFindingStep _step = 0;
        private readonly ObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection> _checkedPaths = new ObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection>();
        #endregion

        #region Public properties
        public int BufferLength { get => _bufferLength; set => _bufferLength = BackgroundWorker.IsBusy ? throw GetBackgroundWorkerIsBusyException() : value < 0 ? throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be less than zero.") : value; }

        public DuplicateFindingOptions Options { get; }

        public DuplicateFindingStep Step
        {
            get => _step;

            set
            {
                _step = value;

                OnPropertyChanged(nameof(Step));
            }
        }

        public ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection> CheckedPaths { get; }
        #endregion

        public DuplicateFinding(DuplicateFindingOptions options)
        {
            CheckedPaths = new ReadOnlyObservableQueueCollection<DuplicateFindingReadOnlyObservableLinkedCollection>(_checkedPaths);

            Options = options ?? throw GetArgumentNullException(nameof(Options));
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (_checkedPaths.Count == 0)

                base.OnDoWork(new DuplicateFindingDoWorkEventArgs(_e => OnCheckForDuplicates(_e), e));

            else

                _DoWork(new DuplicateFindingDoWorkEventArgs(_e => OnDeleteDuplicates(_e), e));

            Step = DuplicateFindingStep.None;
        }

        protected virtual ProcessError OnCheckForDuplicates(DoWorkEventArgs e)
        {
            DuplicateFindingIgnoreOptions ignoreOptions = Options.IgnoreOptions;

            LinkedListNode<IPathInfo> node;

            bool checkIgnoring()
            {
                DuplicateFindingIgnoreValues ignoreValues = ignoreOptions.IgnoreValues;

                if (ignoreValues != DuplicateFindingIgnoreValues.None)
                {
                    var fileInfo = new FileInfo(CurrentPath.Path);

                    if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.SystemFiles) && fileInfo.Attributes.HasFlag(FileAttributes.System)) || (ignoreValues.HasFlag(DuplicateFindingIgnoreValues.HiddenFiles) && fileInfo.Attributes.HasFlag(FileAttributes.Hidden))) return true;
                }

                if (!CurrentPath.Size.Value.ValueInBytes.IsNaN)
                {
                    ulong size = CurrentPath.Size.Value.ValueInBytes.Value;

                    if ((ignoreValues.HasFlag(DuplicateFindingIgnoreValues.ZeroByteFiles) && size == 0)
                        || (ignoreOptions.FileSizeUnder.HasValue && size < ignoreOptions.FileSizeUnder)
                        || (ignoreOptions.FileSizeOver.HasValue && size > ignoreOptions.FileSizeOver)) return true;
                }

                #region Array checks

                if (ignoreOptions.Paths != null && ignoreOptions.Paths.Count > 0)

                    foreach (string _path in ignoreOptions.Paths)

                        if (CurrentPath.Path.StartsWith(_path)) return true;

                string value;

                if (ignoreOptions.FileNames != null && ignoreOptions.FileNames.Count > 0)
                {
                    value = System.IO.Path.GetFileNameWithoutExtension(CurrentPath.Path);

                    foreach (string fileName in ignoreOptions.FileNames)

                        if (value == fileName) return true;
                }

                if (ignoreOptions.FileExtensions != null && ignoreOptions.FileExtensions.Count > 0)
                {
                    value = System.IO.Path.GetExtension(CurrentPath.Path);

                    foreach (string extension in ignoreOptions.FileExtensions)

                        if (value == extension) return true;
                }

                #endregion

                return false;
            }

            void _checkIgnoring()
            {
                CurrentPath = node.Value;

                if (CheckIfPauseOrCancellationPending())

                    return;

                if (ignoreOptions != null && checkIgnoring())

                    _Paths.Remove(node);
            }

            Step = DuplicateFindingStep.CheckingForPathsToIgnore;

            if (_Paths.Count < 2)
            {
                _Paths.Clear();

                return ProcessError.None;
            }

            node = _Paths.First;

            _checkIgnoring();

            if (Error != ProcessError.None)

                return Error;

            while (node.Next != null)
            {
                node = node.Next;

                _checkIgnoring();

                if (Error != ProcessError.None)

                    return Error;
            }

            Step = DuplicateFindingStep.CheckingForDuplicates;

            if (_Paths.Count < 2)
            {
                _Paths.Clear();

                return ProcessError.None;
            }

            LinkedListNode<IPathInfo> nodeToCompare;

            IPathInfo pathToCompare;

            DuplicateFindingMatchingOptions matchingOptions = Options.MatchingOptions;

            var duplicates = new WinCopies.Collections.DotNetFix.LinkedList<IDuplicateFindingPathInfo>();

            bool isDuplicate = false;

            bool hasDuplicates = false;

            bool? result;

            void check()
            {
                bool? checkSize()
                {
                    if (matchingOptions.ContentMatchingOption == DuplicateFindingContentMatchingOption.Size)

                        return CurrentPath.Size.HasValue && pathToCompare.Size.HasValue && CurrentPath.Size.Value == pathToCompare.Size.Value;

                    else return null;
                }

                bool? checkContent()
                {
                    if (matchingOptions.ContentMatchingOption == DuplicateFindingContentMatchingOption.Content)

                        return _checkContent();

                    else return null;
                }

                bool _check(bool? value)
                {
                    if ((result = value).HasValue)

                        if (result.Value)

                            isDuplicate = true;

                        else

                            return false;

                    return true;
                }

                CurrentPath = node.Value;

                pathToCompare = nodeToCompare.Value;

                if (_check(checkSize()) && _check(checkContent()) && isDuplicate)
                {
                    isDuplicate = false;

                    _ = duplicates.AddLast(new DuplicateFindingPathInfo(pathToCompare));

                    hasDuplicates = true;
                }
            }

            void _check()
            {
                if (duplicates.Count > 0)

                    duplicates = new WinCopies.Collections.DotNetFix.LinkedList<IDuplicateFindingPathInfo>();

                nodeToCompare = node.Next;

                check();

                while (nodeToCompare.Next != null)
                {
                    nodeToCompare = nodeToCompare.Next;

                    check();
                }

                if (hasDuplicates)
                {
                    hasDuplicates = false;

                    _ = duplicates.AddFirst(new DuplicateFindingPathInfo(CurrentPath));

                    _checkedPaths.Enqueue(new DuplicateFindingReadOnlyObservableLinkedCollection(new ObservableLinkedCollection<IDuplicateFindingPathInfo>(duplicates)));
                }
            }

            node = _Paths.First;

            _check();

            while (node.Next != null)
            {
                node = node.Next;

                _check();
            }
        }

        protected virtual ProcessError OnDeleteDuplicates(DoWorkEventArgs e)
        {

        }

        protected override ProcessError OnProcessDoWork(DoWorkEventArgs e) => e is DuplicateFindingDoWorkEventArgs _e ? _e.Func() : ProcessError.None;
    }
}
