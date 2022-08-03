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
using static WinCopies.UtilHelpers;

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

            if (!(directory == null || Directory.Exists(directory)))

                Directory.CreateDirectory(directory);

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

                    void log(in string message)
                    {
                        ProcessData.AddLogMessage(message);
                        System.Console.WriteLine(message);
                    }

                    byte bools = 0;

                    bool isOK() => bools.GetBit(0);
                    void ok(in bool value) => SetBit(ref bools, 0, value);

                    bool hasError() => bools.GetBit(1);
                    void error(in bool value) => bools.SetBit(1, value);

                    void setError(in Error _error) => ProcessData.Installer.Model.Error = _error;

                    void trySetError()
                    {
                        if (ProcessData.Installer.Error <= RecoveredError)

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

                    ulong total = 0;

                    Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack tmpFiles = Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack();

                    void process(in Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items, in Action<Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack> action)
                    {
                        total = 0;
                        bools = 0;

                        reportProgress(0);

                        foreach (KeyValuePair<string, IFile> _stream in ProcessData)

                            total += (ulong)_stream.Value.Stream.Length;

                        action(items);
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
                        {
                            if (ProcessData.Installer.Error >= FatalError)
                            {
                                log("Fatal error during file enumeration.");

                                return false;
                            }

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
                        }

                        log(exit);

                        return true;
                    }

                    string path = null;
                    string relativePath = null;
                    string tmp;
                    string location = ProcessData.Installer.Location;
                    IEnumerable<KeyValuePair<string, string>>
#if CS8
                        ?
#endif
                        resources;
                    uint count = 0;
                    ulong totalWritten = 0;
                    int currentTotalWritten = 0;

                    int setPercentProgress() => currentTotalWritten = (int)((float)(totalWritten += count) / total * 100);

                    bool doCopy<T>(in string destination, in IFileEnumerableBase current, in KeyValuePair<string, T> item, in Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items) where T : IFile
                    {
                        long currentTotal = 0;
                        const byte COMPLETED = 100;
                        const ushort MAX_LENGTH = 4096;
                        System.IO.Stream stream;
                        var buffer = new byte[MAX_LENGTH];
                        ulong length = (ulong)(stream = item.Value.Stream).Length;

                        using
#if !CS8
                        (
#endif
                            IInstallerStream writer = current.GetWriter(path = Path.Combine(destination, relativePath))
#if CS8
                            ;
#else
                        )
#endif
                        {
                            items.Push(GetKeyValuePair(item.Key, writer.GetDeleter()));

                            while ((count = (uint)stream.Read(buffer, 0, 4096)) > 0)
                            {
                                writer.Write(buffer, 0, count);

                                ReportProgress(setPercentProgress(), (byte)(((float)(currentTotal += count)) / length * 100));
                            }

                            if (validate())

                                ReportProgress(currentTotalWritten, COMPLETED);

                            else

                                msgbox
                        }
                    }

                    bool copy<T>(in string destination, in IFileEnumerableBase current, in KeyValuePair<string, T> item, in Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items, in FuncIn<string, IFileEnumerableBase, KeyValuePair<string, T>, Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack, bool> action) where T : IFile
                    {
                        count = 0;
                        totalWritten = 0;
                        currentTotalWritten = 0;
                        relativePath = item.Key;

                        if ((resources = current.Resources) != null)
                        {
                            string replace(in string text) => text.Replace("_u", "_").Replace("_b", "\\").Replace("_d", ".");

                            foreach (KeyValuePair<string, string> resource in resources)

                                if (replace(relativePath).StartsWith(tmp = replace(resource.Key) + '.'))
                                {
                                    relativePath = $"{resource.Value}\\{relativePath.Substring(tmp.Length)}";

                                    break;
                                }
                        }

                        action(destination, current, item, items);

                        log($"Copied {path}");
                    }

                    Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack
#if CS8
                        ?
#endif
                        temporaryFiles = ProcessData.GetTemporaryFiles((string errorMessage, bool recoverable) =>
                        {
                            MessageBoxResult show(in string msg, in MessageBoxButton buttons, in MessageBoxResult result) => MessageBox.Show($"{errorMessage}\n{msg}, the installation could be unstable.", "Gathering Files Error", buttons, MessageBoxImage.Error, result);

                            if (recoverable)
                            {
                                switch (show("Do you want to try again? If you skip this file", MessageBoxButton.YesNoCancel, MessageBoxResult.Yes))
                                {
                                    case MessageBoxResult.Yes:

                                        setError(RecoveredError);

                                        return true;

                                    case MessageBoxResult.No:

                                        setError(NotRecoveredError);

                                        return false;
                                }
                            }

                            else

                                switch (show("Do you want to continue anyway? If you continue", MessageBoxButton.YesNo, MessageBoxResult.No))
                                {
                                    case MessageBoxResult.Yes:

                                        setError(RecoveredError);

                                        return true;
                                }

                            setError(FatalError);

                            return false;
                        });

                    void _process()
                    {
                        if (temporaryFiles != null)

                            foreach (ITemporaryFileEnumerable enumerable in temporaryFiles)
                            {
                                process(tmpFiles, items => doInnerWork("Starting " + enumerable.ActionName, enumerable.ActionName, enumerable.ActionName, "Completed " + enumerable.ActionName, enumerable.Files, (in KeyValuePair<string, IValidableFile> item) => copy(ProcessData.Installer.ProgramName, enumerable.Files, item, items, (in string destination, in IFileEnumerableBase current, in KeyValuePair<string, IValidableFile> item, in Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items) =>
                                {
                                    if (!validate())

                                        doCopy(destination, current, item, items);
                                })));

                                if (ProcessData.Installer.Error >= FatalError)

                                    return;
                            }

                        process(Collections.Generic.EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack(), items =>
                        {
                            bool doWork()
                            {
                                try
                                {
                                    ProcessData.DeleteOldFiles(message => log(message));
                                }
                                catch (Exception ex)
                                {
                                    log($"Error: {ex.Message}");

                                    return false;
                                }

                                return doInnerWork("Copying", "copy", "Copy", "Files copied.", ProcessData, (in KeyValuePair<string, IFile> item) => copy(location, ProcessData, item, items, doCopy));
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
                        });
                    }

                    _process();

                    while (tmpFiles.TryPeek(out KeyValuePair<string, Action> item))
                    {
                        try
                        {
                            item.Value();
                        }
                        catch (Exception ex)
                        {
                            switch (MessageBox.Show($"Could not delete the file '{item.Key}'.\nMessage: {ex.Message}\nClick 'Yes' to retry, 'No' to skip this file or 'Cancel' to quit the installation.", $"File Deletion Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes))
                            {
                                case MessageBoxResult.Yes:

                                    trySetError();

                                    continue;

                                case MessageBoxResult.No:

                                    setError(NotRecoveredError);

                                    break;

                                case MessageBoxResult.Cancel:

                                    setError(SuperFatalError);

                                    return;
                            }
                        }

                        onSucceeded();

                        tmpFiles.Pop();
                    }
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

            public Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack
#if CS8
            ?
#endif
            GetTemporaryFiles(Func<string, bool, bool> onError) => ModelGeneric.GetTemporaryFiles(onError);

            public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => ModelGeneric.GetEnumerator();

#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this.Select(item => GetKeyValuePair(item.Key, item.Value.AsFromType()));
#endif
        }

        public sealed override bool CanBrowseBack => false;

        internal ProcessPageViewModel(in IProcessPage installerPage, in InstallerViewModel installer) : base(installerPage, installer) => MarkAsBusy();

        public override IProcessData GetData() => new ProcessDataViewModel(this);

        public sealed override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();
        public override IInstallerPageViewModel GetNext() => new EndPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
