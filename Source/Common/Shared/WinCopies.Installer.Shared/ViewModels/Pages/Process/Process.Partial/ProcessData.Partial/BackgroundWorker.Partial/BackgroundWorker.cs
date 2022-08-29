using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections.Generic;

using static WinCopies.Installer.Error;

namespace WinCopies.Installer
{
    [Flags]
    public enum Actions
    {
        Prepare = 1,
        Install,
        Both
    }

    public partial class ProcessPageViewModel
    {
        protected partial class ProcessDataViewModel
        {
            protected partial class BackgroundWorker : System.ComponentModel.BackgroundWorker
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
                    ProcessDataViewModel processData = ProcessData;

                    processData.OverallProgress = (byte)e.ProgressPercentage;
                    processData.CurrentItemProgress = (byte)e.UserState;

                    base.OnProgressChanged(e);
                }

                protected virtual void DoExtraWork(DoWorkEventArgs e) => base.OnDoWork(e);

                protected override void OnDoWork(DoWorkEventArgs e)
                {
                    var processorInfo = new ProcessorInfo(this, e);

                    processorInfo.DoWork((ProcessorInfo.Processor processor, Action action, Action reset) => () =>
                    {
                        ProcessDataViewModel processData = processor.ProcessData;
                        Actions actions = processData.Installer.Actions;
                        void setError(in Error error) => processor.Error = error;
                        EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList
#if CS8
                            ?
#endif
                            temporaryFiles = processor.TemporaryFiles;

                        Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable>
#if CS8
                            ?
#endif
                            tmpEnumerable = processData.GetTemporaryFiles((string errorMessage, bool recoverable) =>
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

                        bool hasFatalErrorOccurred() => processor.HasFatalErrorOccurred();

                        if (hasFatalErrorOccurred())

                            return;

                        bool noTemporaryFiles()
                        {
                            bool result = tmpEnumerable == null;

                            processor.SetBit(FlagPosition.ClearTemporaryFiles, !result);

                            return result;
                        }

                        void addPrepareActions() => processorInfo.AddPrepareActions();
                        void addInstallActions() => processorInfo.AddInstallActions();

                        if (noTemporaryFiles())
                        {
                            if (actions == Actions.Install)

                                addInstallActions();

                            return;
                        }

                        if (actions == Actions.Prepare)
                        {
                            if (tmpEnumerable.TryPeek(out ITemporaryFileEnumerable enumerable))
                            {
                                action();

                                temporaryFiles.AddLast(enumerable);

                                addPrepareActions();
                            }

                            else

                                reset();

                            return;
                        }

                        void init(in System.Collections.Generic.IEnumerable<ITemporaryFileEnumerable> enumerables)
                        {
                            action();

                            byte steps = 2;

                            foreach (ITemporaryFileEnumerable enumerable in enumerables)
                            {
                                ValidateStep(++steps);

                                if (hasFatalErrorOccurred())
                                {
                                    temporaryFiles.Clear();
                                    reset();

                                    return;
                                }

                                temporaryFiles.AddLast(enumerable);
                            }
                        }

                        if (temporaryFiles.HasItems)

                            addPrepareActions();

                        if (actions == Actions.Install)
                        {
                            init(tmpEnumerable.Skip(1));

                            addInstallActions();

                            return;
                        }

                        //EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack tmpFiles = EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack();

                        init(tmpEnumerable);

                        addInstallActions();
                    });
                }
            }
        }
    }
}
