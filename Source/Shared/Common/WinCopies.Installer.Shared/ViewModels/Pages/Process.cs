using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections;
using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.Installer.Error;

using ICustomStream = WinCopies.DotNetFix.IWriterStream;

namespace WinCopies.Installer
{
    public interface IInstallerStream : ICustomStream
    {
        Action GetDeleter();
    }

    public class InstallerStream : DotNetFix.FileStream, IInstallerStream
    {
        public InstallerStream(in string path) : base(path, GetStream(path)) { /* Left empty. */ }

        protected static Action GetAction(string path) => () => System.IO.File.Delete(path);

        protected static FileStream GetStream(in string path)
        {
            string
#if CS8
                ?
#endif
                directory = System.IO.Path.GetDirectoryName(path);

            if (!(directory == null || System.IO.Directory.Exists(directory)))

                System.IO.Directory.CreateDirectory(directory);

            return new FileStream(path, FileMode.CreateNew);
        }

        public Action GetDeleter() => GetAction(Path);
    }

    public class ProcessPageViewModel : CommonPageViewModel2<IProcessPage, IEndPage, IProcessData>, IProcessPage
    {
        protected class ProcessDataViewModel : InstallerPageDataViewModel<IProcessData>, IProcessDataViewModel
        {
            protected class BackgroundWorker : System.ComponentModel.BackgroundWorker
            {
                protected ProcessDataViewModel ProcessData { get; }

                public BackgroundWorker(in ProcessDataViewModel processData)
                {
                    ProcessData = processData;
                    WorkerReportsProgress = true;
                }

                protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
                {
                    base.OnRunWorkerCompleted(e);

                    ProcessData.ProcessPage.MarkAsCompleted();

                    CommandManager.InvalidateRequerySuggested();
                }

                protected override void OnProgressChanged(ProgressChangedEventArgs e)
                {
                    IProcessDataViewModel processData = ProcessData;

                    processData.OverallProgress = (byte)e.ProgressPercentage;
                    processData.CurrentItemProgress = (byte)e.UserState;

                    base.OnProgressChanged(e);
                }

                protected override void OnDoWork(DoWorkEventArgs e)
                {
                    void reportProgress(in byte percentProgress) => ReportProgress(percentProgress, percentProgress);

                    reportProgress(0);

                    ulong total = 0;

                    foreach (KeyValuePair<string, File> _stream in ProcessData)

                        total += (ulong)_stream.Value.Stream.Length;

                    void log(in string message)
                    {
                        ProcessData.AddLogMessage(message);
                        System.Console.WriteLine(message);
                    }

                    Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items = Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack();

                    byte bools = 0;

                    bool isOK() => bools.GetBit(0);
                    void ok(in bool value) => UtilHelpers.SetBit(ref bools, 0, value);

                    bool hasError() => bools.GetBit(1);
                    void error(in bool value) => bools.SetBit(1, value);

                    void setError(in Error _error) => ProcessData.Installer.Model.Error = _error;

                    void trySetError()
                    {
                        if (ProcessData.Installer.Error == Succeeded)

                            error(true);
                    }

                    void onSucceeded()
                    {
                        if (hasError())
                        {
                            error(false);

                            setError(RecoveredError);
                        }
                    }

                    bool doInnerWork<T>(in string enter, in string errorMessage, in string errorCaption, in string exit, in IEnumerable<KeyValuePair<string, T>> _items, in ActionIn<KeyValuePair<string, T>> action)
                    {
                        void onError() => setError(FatalError);
                        void logError(in string
#if CS8
                            ?
#endif
                            msg, in string _msg) => log($"An {msg}error occurred. Error message: {_msg}");

                        foreach (KeyValuePair<string, T> item in _items)

                            do
                            {
                                log($"{enter} {item.Key}...");

                                ok(false);

                                try
                                {
                                    action(item);

                                    onSucceeded();
                                }

                                catch (IOException ex)
                                {
                                    string message = ex.Message;

                                    logError("I/O ", message);

                                    switch (MessageBox.Show($"Could not {errorMessage} the file '{item.Key}'.\nMessage: {message}\nClick 'Yes' to retry, 'No' to skip this file or 'Cancel' to quit the installation.", $"File {errorCaption} Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes))
                                    {
                                        case MessageBoxResult.Yes:

                                            ok(true);

                                            trySetError();

                                            break;

                                        case MessageBoxResult.No:

                                            setError(NotRecoveredError);

                                            break;

                                        case MessageBoxResult.Cancel:

                                            onError();

                                            return false;
                                    }
                                }

                                catch (Exception ex)
                                {
                                    logError(null, ex.Message);

                                    onError();

                                    return false;
                                }
                            }

                            while (isOK());

                        log(exit);

                        return true;
                    }

                    uint count = 0;
                    ulong totalWritten = 0;
                    int currentTotalWritten = 0;
                    string location;
                    string path;
                    string relativePath;
                    string tmp;
                    IEnumerable<KeyValuePair<string, string>>
#if CS8
                        ?
#endif
                        resources = ProcessData.Resources;

                    int setPercentProgress() => currentTotalWritten = (int)((float)(totalWritten += count) / total * 100);

                    bool doWork()
                    {
                        location = ProcessData.Installer.Location;

                        try
                        {
                            ProcessData.DeleteOldFiles(message => log(message));
                        }
                        catch (Exception ex)
                        {
                            log($"Error: {ex.Message}");

                            return false;
                        }

                        return doInnerWork("Extracting", "copy", "Extraction", "Files extracted.", ProcessData, (in KeyValuePair<string, File> item) =>
                            {
                                const ushort MAX_LENGTH = 4096;
                                const byte COMPLETED = 100;
                                System.IO.Stream stream;
                                var buffer = new byte[MAX_LENGTH];
                                long currentTotal;
                                ulong length;

                                currentTotal = 0;
                                length = (ulong)(stream = item.Value.Stream).Length;
                                relativePath = item.Key;

                                if (resources != null)
                                {
                                    string replace(in string text) => text.Replace("_u", "_").Replace("_b", "\\").Replace("_d", ".");

                                    foreach (KeyValuePair<string, string> resource in resources)

                                        if (replace(relativePath).StartsWith(tmp = replace(resource.Key) + '.'))
                                        {
                                            relativePath = $"{resource.Value}\\{relativePath.Substring(tmp.Length)}";

                                            break;
                                        }
                                }

                                using
#if !CS8
                                (
#endif
                                    IInstallerStream writer = ProcessData.GetWriter(path = Path.Combine(location, relativePath))
#if CS8
                                    ;
#else
                                )
#endif
                                {
                                    while ((count = (uint)stream.Read(buffer, 0, 4096)) > 0)
                                    {
                                        writer.Write(buffer, 0, count);

                                        ReportProgress(setPercentProgress(), (byte)(((float)(currentTotal += count)) / length * 100));
                                    }

                                    ReportProgress(currentTotalWritten, COMPLETED);

                                    items.Push(UtilHelpers.GetKeyValuePair(item.Key, writer.GetDeleter()));
                                }

                                log($"Extracted {path}");
                            });
                    }

                    void doExtraWork(in (string enter, string exit, byte progress, Action action) vars)
                    {
                        log(vars.enter);

                        vars.action();

                        base.OnDoWork(e);

                        log(vars.exit);

                        reportProgress(vars.progress);
                    }

                    doExtraWork(doWork() ? ("Starting custom actions...", "Custom actions completed.\nCompleted.", 100, () =>
                    {
                        string
#if CS8
                            ?
#endif
                             errorMessage;
                        string
#if CS8
                            ?
#endif
                            _log;

                        foreach (IOption option in ProcessData.ModelGeneric.Installer.Options.Where(optionGroup => optionGroup.IsChecked == true).SelectMany().Where(option => option.IsChecked))

                            do
                            {
                                errorMessage = null;
                                _log = null;

                                try
                                {
                                    ok(option.Action(out _log));
                                }

                                catch (Exception ex)
                                {
                                    ok(false);
                                    errorMessage = ex.Message;
                                }

                                log($"Custom Action: {option.Name} -- {_log ?? "<No log could be retrieved or provided.>"}");

                                if (isOK())
                                {
                                    onSucceeded();

                                    break;
                                }

                                MessageBoxResult result = MessageBox.Show($"Could not perform the action '{option.Name}'.\nMessage: {errorMessage ?? "<No error message.>"}\nClick 'Yes' to retry, 'No' to skip this action, 'Cancel' to skip all custom actions.", "Custom Action Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

                                switch (result)
                                {
                                    case MessageBoxResult.Yes:

                                        ok(true);

                                        trySetError();

                                        break;

                                    case MessageBoxResult.No:

                                        setError(NotRecoveredError);

                                        break;

                                    case MessageBoxResult.Cancel:

                                        setError(FatalError);

                                        return;
                                }
                            }
                            while (isOK());
                    }
                    ) :
#if !CS9
                    (ValueTuple<string, string, byte, Action>)
#endif
                    ("Cancelling...", "All files deleted.", 0, () => setError(doInnerWork("Deleting", "delete", "Deletion", "Cancelled.", items, (in KeyValuePair<string, Action> stream) =>
                        {
                            stream.Value();

                            setPercentProgress();
                        }) ? FatalError : SuperFatalError)
                    ));
                }
            }

            private byte _overallProgress;
            private byte _currentItemProgress;
            private string _log;
            private bool _canExecute = true;

            public event EventHandler CanExecuteChanged;

            protected BackgroundWorker InnerBackgroundWorker { get; }

            protected ProcessPageViewModel ProcessPage { get; }

            public byte OverallProgress { get => _overallProgress; set => UpdateValue(ref _overallProgress, value, nameof(OverallProgress)); }

            public byte CurrentItemProgress { get => _currentItemProgress; set => UpdateValue(ref _currentItemProgress, value, nameof(CurrentItemProgress)); }

            public string Log { get => _log; set => UpdateValue(ref _log, value, nameof(Log)); }

            public IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources => ModelGeneric.Resources;

            public ProcessDataViewModel(in ProcessPageViewModel processPage) : base(processPage.ModelGeneric.Data, processPage.Installer)
            {
                ProcessPage = processPage;
                InnerBackgroundWorker = new BackgroundWorker(this);
            }

            public IInstallerStream GetWriter(string path) => ModelGeneric.GetWriter(path);

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

            public IEnumerator<KeyValuePair<string, File>> GetEnumerator() => ModelGeneric.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public sealed override bool CanBrowseBack => false;

        internal ProcessPageViewModel(in IProcessPage installerPage, in InstallerViewModel installer) : base(installerPage, installer) => MarkAsBusy();

        public override IProcessData GetData() => new ProcessDataViewModel(this);

        public sealed override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();
        public override IInstallerPageViewModel GetNext() => new EndPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
