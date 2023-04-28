/* Copyright © Pierre Sprimont, 2022
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

using WinCopies.Collections;

namespace WinCopies.Installer
{
    public interface IProcessDataViewModel : IProcessData, INotifyPropertyChanged, ICommand
    {
        string StepName { get; }

        /// <summary>
        /// Indicates both the current step (4 first bits) and the total number of steps (4 last bits).
        /// </summary>
        byte StepData { get; }

        byte OverallProgress { get; }

        byte CurrentItemProgress { get; }

        string Log { get; }

        void AddLogMessage(string message);

        void Start();
    }

    public partial class ProcessPageViewModel
    {
        protected partial class ProcessDataViewModel : InstallerPageDataViewModel<IProcessData>, IProcessDataViewModel
        {
            private byte _overallProgress;
            private byte _currentItemProgress;
            private string _log;
            private bool _canExecute = true;
            private string _stepName;
            private byte _stepData;

            public event EventHandler CanExecuteChanged;

            protected BackgroundWorker InnerBackgroundWorker { get; }

            protected ProcessPageViewModel ProcessPage { get; }

            /*public string
#if CS8
                ?
#endif
                ValidationDirectory => ModelGeneric.ValidationDirectory;*/

            public string StepName { get => _stepName; private set => UpdateValue(ref _stepName, value, nameof(StepName)); }

            public byte StepData { get => _stepData; private set => UpdateValue(ref _stepData, value, nameof(StepData)); }

            public byte OverallProgress { get => _overallProgress; protected set => UpdateValue(ref _overallProgress, value, nameof(OverallProgress)); }

            public byte CurrentItemProgress { get => _currentItemProgress; protected set => UpdateValue(ref _currentItemProgress, value, nameof(CurrentItemProgress)); }

            public string Log { get => _log; private set => UpdateValue(ref _log, value, nameof(Log)); }

            public IEnumerable<KeyValuePair<string, string>>
#if CS8
                ?
#endif
            Resources => ModelGeneric.Resources;

            public string
#if CS8
                ?
#endif
            RelativeDirectory => ModelGeneric.RelativeDirectory;

            public string
#if CS8
                ?
#endif
            OldRelativeDirectory => ModelGeneric.OldRelativeDirectory;

            public ProcessDataViewModel(in ProcessPageViewModel processPage) : base(processPage.ModelGeneric.Data, processPage.Installer)
            {
                ProcessPage = processPage;
                InnerBackgroundWorker = new BackgroundWorker(this);
            }

            private void IncrementStep() => StepData = (byte)((((_stepData >> 4) + 1) << 4) | ((byte)(_stepData << 4) >> 4));

            private static void ValidateStep(in byte b)
            {
                if (b > 0b1111)

                    throw new ArgumentOutOfRangeException(nameof(b));
            }

            private void SetTotalSteps(in byte b)
            {
                ValidateStep(b);

                StepData = b;
            }

            public System.IO.Stream GetWriter(string path) => ModelGeneric.GetWriter(path);

            public void DeleteOldFiles(Action<string> logger) => ModelGeneric.DeleteOldFiles(logger);

            public void AddLogMessage(string message)
            {
                if (_log == null)

                    Log = message;

                else
                {
                    var sb = new StringBuilder();

                    void append(in string msg) => sb.Append(msg);

                    append(_log);

                    if (!_log.EndsWith('\n'))

                        _ = sb.Append('\n');

                    append(message);

                    Log = sb.ToString();
                }
            }

            public void Start() => InnerBackgroundWorker.RunWorkerAsync();

            public bool CanExecute(object parameter) => _canExecute;
            public void Execute(object parameter)
            {
                Start();

                _canExecute = false;

                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable>
#if CS8
                ?
#endif
                GetTemporaryFiles(Func<string, bool, bool> onError) => ModelGeneric.GetTemporaryFiles(onError);

            public byte[] GetValidationData(System.IO.Stream stream) => ModelGeneric.GetValidationData(stream);

            /*public Func<System.IO.Stream>
#if CS8
                ?
#endif
                GetLocalValidationStream(string file) => ModelGeneric.GetLocalValidationStream(file);
            public System.IO.Stream
#if CS8
                ?
#endif
                GetRemoteValidationStream(string file, out ulong? length) => ModelGeneric.GetRemoteValidationStream(file, out length);
            public byte[]
#if CS8
                ?
#endif
                GetValidationData(System.IO.Stream stream) => ModelGeneric.GetValidationData(stream);*/

            public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => ModelGeneric.GetEnumerator();

#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }
    }
}
