using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static System.IO.Path;

using static WinCopies.Collections.DotNetFix.EnumerationDirection;
using static WinCopies.Installer.Error;
using static WinCopies.UtilHelpers;

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
            protected class BackgroundWorker : System.ComponentModel.BackgroundWorker
            {
                private struct Stack : EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack, IEnumerable<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode>
                {
                    private struct Node : EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
                    {
                        private readonly EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList _list;
                        private readonly Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode> _previousNodeProvider;
                        private readonly Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode>
#if CS8
                            ?
#endif
                            _nextNodeProvider;

                        public EnumerableHelper<ITemporaryFileEnumerable>.ILinkedList List => _list;

                        public ITemporaryFileEnumerable Value { get; }

                        public EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode Previous => _previousNodeProvider();

                        public EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode Next => _nextNodeProvider?.Invoke();

                        public Node(in EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList list, in ITemporaryFileEnumerable enumerable, in Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode> previousNodeProvider, Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode>
#if CS8
                            ?
#endif
                            nextNodeProvider)
                        {
                            _list = list;
                            _previousNodeProvider = previousNodeProvider;
                            _nextNodeProvider = nextNodeProvider;

                            Value = enumerable;
                        }

                        public void Remove() => throw new NotImplementedException();
                    }

                    private readonly EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList _linkedList;
                    private readonly ITemporaryFileEnumerable _value;

                    private Collections.DotNetFix.Generic.IStackBase<ITemporaryFileEnumerable> LinkedList => _linkedList.AsFromType<Collections.DotNetFix.Generic.IStackBase<ITemporaryFileEnumerable>>();

                    private IEnumerable<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode> InnerStack => _linkedList.AsEnumerable(LIFO);

                    public bool IsReadOnly => false;

                    public bool HasItems => _linkedList.HasItems;

                    public bool SupportsReversedEnumeration => true;

                    public Stack(in EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList linkedList, in ITemporaryFileEnumerable value)
                    {
                        _linkedList = linkedList;
                        _value = value;
                    }

                    public void Push(ITemporaryFileEnumerable item) => _linkedList.Push(item);
                    public ITemporaryFileEnumerable Pop() => _linkedList.Pop();
                    public bool TryPop(out ITemporaryFileEnumerable result) => _linkedList.TryPop(out result);
                    public ITemporaryFileEnumerable Peek() => LinkedList.Peek();
                    public bool TryPeek(out ITemporaryFileEnumerable result) => LinkedList.TryPeek(out result);

                    public void Clear() => _linkedList.Clear();

                    public IEnumerator<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode> GetEnumerator()
                    {
                        EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList linkedList = _linkedList;
                        ITemporaryFileEnumerable value = _value;
                        EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                            ?
#endif
                            node = null;

                        EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode getNextNode()
                        {
                            EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode _node = new Node(linkedList, value, () => linkedList.FirstNode, null);

                            node = _node;

                            return _node;
                        }

                        foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode item in InnerStack)

                            yield return item.Previous == null ? new Node(linkedList, item.Value, () => item.Previous, getNextNode) : item;

                        yield return node ?? getNextNode();
                    }
                    IEnumerable<ITemporaryFileEnumerable> GetEnumerable() => InnerStack.Select(node => node.Value);
                    IEnumeratorInfo<ITemporaryFileEnumerable> Collections.DotNetFix.Generic.IEnumerable<ITemporaryFileEnumerable, IEnumeratorInfo<ITemporaryFileEnumerable>>.GetEnumerator() => new EnumeratorInfo<ITemporaryFileEnumerable>(GetEnumerable());
                    public IEnumeratorInfo<ITemporaryFileEnumerable> GetReversedEnumerator() => _linkedList.GetEnumerator();
#if !CS8
                    IEnumerator<ITemporaryFileEnumerable> IEnumerable<ITemporaryFileEnumerable>.GetEnumerator() => GetEnumerable().GetEnumerator();
                    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
                    IEnumerator<ITemporaryFileEnumerable> Collections.Extensions.Generic.IEnumerable<ITemporaryFileEnumerable>.GetReversedEnumerator() => GetReversedEnumerator();
                    System.Collections.IEnumerator Collections.Enumeration.IEnumerable.GetReversedEnumerator() => GetReversedEnumerator();
#endif
                }

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

                protected override void OnDoWork(DoWorkEventArgs e)
                {
                    void doWorkSafe(in Action action, in Error _error)
                    {
                        try
                        {
                            action();
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred. Message:\n" + ex.Message, "Installation Error", MessageBoxButton.OK, MessageBoxImage.Error);

                            setError(_error);
                        }
                    }

                    void log(in string message)
                    {
                        ProcessData.AddLogMessage(message);
                        System.Console.WriteLine(message);
                    }

                    void setError(in Error _error) => ProcessData.Installer.Model.Error = _error;

                    void reportProgress(in byte percentProgress) => ReportProgress(percentProgress, percentProgress);

                    byte bools = 0;

                    bool isOK() => bools.GetBit(0);
                    void ok(in bool value) => SetBit(ref bools, 0, value);
                    bool hasError() => bools.GetBit(1);
                    void error(in bool value) => bools.SetBit(1, value);

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

                                    switch (MessageBox.Show($"Could not perform '{errorMessage}' action for the file '{item.Key}'.\nMessage: {message}\nClick 'Yes' to retry, 'No' to skip this file or 'Cancel' to quit the installation.", $"File {errorCaption} Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes))
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

                    ulong total = 0;
                    ulong totalWritten = 0;

                    string replace(in string _path) => _path.Replace('/', DirectorySeparatorChar);

                    void _replace(ref string _path) => _path = replace(_path);

                    void process(in IEnumerable<IFile> files, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Action/*<EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack>*/ action)
                    {
                        total = 0;
                        bools = 0;

                        reportProgress(0);

                        foreach (IFile file in files)
                        {
                            total += totalWritten = (ulong)file.Stream.Length;

                            log($"Found {file.Name} {Consts.Symbols.GeneralPunctuation.Bullet} Size: {new IO.Size(totalWritten)}");

                            file.Stream.Dispose();
                        }

                        totalWritten = 0;

                        action(/*items*/);
                    }

                    IEnumerable<KeyValuePair<string, string>>
#if CS8
                        ?
#endif
                        resources;
                    string tmp;
                    uint count = 0;
                    byte currentTotalWritten = 0;

                    byte setPercentProgress() => currentTotalWritten = (byte)((float)(totalWritten += count) / total * 100);

                    string path = null;
                    string relativePath = null;
                    ulong? length = null;
                    const byte COMPLETED = 100;

                    void doCopy<T>(in string destination, in IFileEnumerableBase current, in KeyValuePair<string, T> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Copier action) where T : IFile
                    {
                        long currentTotal = 0;

                        string getRelativePath(string
#if CS8
                            ?
#endif
                            oldRelativeDirectory)
                        {
                            _replace(ref relativePath);

                            if (oldRelativeDirectory != null)
                            {
                                _replace(ref oldRelativeDirectory);

                                if (!oldRelativeDirectory.StartsWith(DirectorySeparatorChar))

                                    oldRelativeDirectory = DirectorySeparatorChar + oldRelativeDirectory;

                                if (oldRelativeDirectory.EndsWith(DirectorySeparatorChar))

                                    oldRelativeDirectory = oldRelativeDirectory.Remove(oldRelativeDirectory.Length - 1);

                                if (relativePath.StartsWith(oldRelativeDirectory))

                                    return relativePath
#if CS8
                                        [
#else
                                        .Substring(
#endif
                                        oldRelativeDirectory.Length
#if CS8
                                        ..]
#else
                                        )
#endif
                                        ;
                            }

                            return relativePath;
                        }

                        using
#if !CS8
                        (
#endif
                            System.IO.Stream writer = current.GetWriter(path = $"{destination}{(current.RelativeDirectory == null ? null : '\\' + current.RelativeDirectory)}{getRelativePath(current.OldRelativeDirectory)}")
#if CS8
                            ;
#else
                        )
#endif
                        {
                            // todo: push only when called from the install operation
                            //items.Push(GetKeyValuePair(item.Key, writer.GetDeleter()));

                            action(item.Value.Stream, writer, (uint progress) =>
                            {
                                count = progress;

                                ReportProgress(setPercentProgress(), (byte)((float)(currentTotal += count) / length.Value * 100));
                            }, out length);

                            ReportProgress(currentTotalWritten, COMPLETED);

                            item.Value.Stream.Dispose();
                        }
                    }

                    void copy<T>(in string destination, in IFileEnumerableBase current, in KeyValuePair<string, T> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in ActionIn<string, IFileEnumerableBase, KeyValuePair<string, T>, /*EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack,*/ Copier> action, in Copier copier) where T : IFile
                    {
                        count = 0;
                        relativePath = item.Key;

                        if ((resources = current.Resources) != null)
                        {
                            string __replace(in string text) => text.Replace("_u", "_").Replace("_b", "\\").Replace("_d", ".");

                            foreach (KeyValuePair<string, string> resource in resources)

                                if (__replace(relativePath).StartsWith(tmp = __replace(resource.Key) + '.'))
                                {
                                    relativePath = $"{resource.Value}\\{relativePath.Substring(tmp.Length)}";

                                    break;
                                }
                        }

                        action(destination, current, item, /*items,*/ copier);

                        log($"Copied {path}");
                    }

                    void incrementStep(in string stepName)
                    {
                        ProcessData.StepName = stepName;

                        ProcessData.IncrementStep();
                    }

                    bool hasFatalErrorOccurred() => ProcessData.Installer.Error >= FatalError;

                    EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList
#if CS8
                            ?
#endif
                            temporaryFiles = null;

                    doWorkSafe(() =>
                    {
                        IPeekableEnumerable<ITemporaryFileEnumerable>
#if CS8
                            ?
#endif
                            tmpEnumerable = ProcessData.GetTemporaryFiles((string errorMessage, bool recoverable) =>
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

                        if (hasFatalErrorOccurred())

                            return;

                        void setTotalSteps(in byte b) => ProcessData.SetTotalSteps(b);

                        if (tmpEnumerable == null || ProcessData.Installer.Actions == Actions.Install)
                        {
                            setTotalSteps(1);

                            return;
                        }

                        temporaryFiles = EnumerableHelper<ITemporaryFileEnumerable>.GetEnumerableLinkedList();

                        if (ProcessData.Installer.Actions == Actions.Prepare)
                        {
                            setTotalSteps(2);

                            if (tmpEnumerable.TryPeek(out ITemporaryFileEnumerable enumerable))

                                temporaryFiles.Enqueue(enumerable);

                            else

                                temporaryFiles = null;

                            return;
                        }

                        byte steps = 0;

                        foreach (ITemporaryFileEnumerable enumerable in tmpEnumerable)
                        {
                            ValidateStep(++steps);

                            if (hasFatalErrorOccurred())
                            {
                                temporaryFiles.Clear();
                                temporaryFiles = null;

                                return;
                            }

                            temporaryFiles.Enqueue(enumerable);
                        }

                        setTotalSteps((byte)(steps + 2));
                    }, FatalError);

                    if (temporaryFiles?.HasItems == true && ProcessData.Installer.Actions.HasFlag(Actions.Prepare))
                    {
                        //EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack tmpFiles = EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack();

                        doWorkSafe(() =>
                        {
                            Func<System.IO.Stream>
#if CS8
                                ?
#endif
                                getReader = null;
                            System.IO.Stream
#if CS8
                                ?
#endif
                                reader = null;
                            System.IO.Stream
#if CS8
                                ?
#endif
                                sha256 = null;
                            byte[]
#if CS8
                                ?
#endif
                                buffer1 = null;
                            byte[]
#if CS8
                                ?
#endif
                                buffer2 = null;

                            void _process()
                            {

                                bool __process(in IEnumerable<ITemporaryFileEnumerable> _temporaryFiles, in Action<ITemporaryFileEnumerable> __action, ConverterIn<string, string> msg)
                                {
                                    foreach (ITemporaryFileEnumerable enumerable in _temporaryFiles)
                                    {
                                        void _log(in ConverterIn<string, string> _msg) => log(_msg(msg(enumerable.ActionName)));

                                        _log((in string _msg) => $"Starting {_msg}");

                                        __action(enumerable);

                                        if (hasFatalErrorOccurred())

                                            return true;

                                        _log((in string _msg) => $"{_msg} Completed");
                                    }

                                    return false;
                                }

                                const string STARTING_PARSING = "Starting Parsing";

                                log(STARTING_PARSING);

                                incrementStep(STARTING_PARSING);

                                {
                                    EnumerableHelper<IEnumerable<string>>.IEnumerableStack paths = EnumerableHelper<IEnumerable<string>>.GetEnumerableStack();

                                    ITemporaryFileEnumerable enumerable = null;

                                    void ___process()
                                    {
                                        bool? contains;

                                        foreach (string physicalFile in enumerable.PhysicalFiles)
                                        {
                                            bool? mustBeDeleted() // We cannot use the Contains method from LINQ because if it was, the enumeration would continue even if an error occurs, we have to cancel it manually in that case. A custom method is needed for this purpose.
                                            {
                                                foreach (string file in enumerable.Paths.AppendValues(paths))
                                                {
                                                    if (hasFatalErrorOccurred())

                                                        return null;

                                                    if (physicalFile.EndsWith(replace(file)))

                                                        return false;
                                                }

                                                return true;
                                            }

                                            if ((contains = mustBeDeleted()).HasValue)
                                            {
                                                if (contains.Value && System.IO.File.Exists(physicalFile))

                                                    System.IO.File.Delete(physicalFile);
                                            }

                                            else

                                                return;
                                        }
                                    }

                                    void doProcess(in ITemporaryFileEnumerable _enumerable)
                                    {
                                        enumerable = _enumerable;

                                        ___process();
                                    }

                                    ActionIn<ITemporaryFileEnumerable> __action = (in ITemporaryFileEnumerable _enumerable) =>
                                    {
                                        doProcess(_enumerable);

                                        __action = (in ITemporaryFileEnumerable __enumerable) =>
                                        {
                                            paths.Push(enumerable.Paths.AsLocalFileEnumerable());

                                            doProcess(__enumerable);
                                        };
                                    };

                                    if (__process(temporaryFiles, _enumerable => __action(_enumerable), (in string actionName) => $"Parsing items for {actionName} action"))

                                        return;
                                }

                                log("Parsing Completed\n");

                                /*const string DOWNLOADING_VALIDATION_FILES = "Downloading Validation Files";

                                /log("Parsing Completed\nStarting " + DOWNLOADING_VALIDATION_FILES);

                                incrementStep(DOWNLOADING_VALIDATION_FILES);

                                if (__process(temporaryFiles, enumerable =>
                                {
                                    System.IO.Stream
    #if CS8
                                        ?
    #endif
                                    writer;
                                    buffer1 = new byte[4096];

                                    Directory.CreateDirectory(Combine(ProcessData.TemporaryDirectory, ProcessData.ValidationDirectory, enumerable.Files.RelativeDirectory));

                                    foreach (KeyValuePair<string, IValidableFile> item in enumerable.ValidationFiles)

                                        if (!((reader = item.Value.GetRemoteValidationStream()) == null || (writer = enumerable.GetValidationFileWriter(relativePath = replace(item.Key))) == null))
                                        {
                                            log($"Downloading validation file for {relativePath}");

                                            while ((count = (uint)reader.Read(buffer1, 0, 4096)) > 0)

                                                writer.Write(buffer1, 0, (int)count);

                                            reader.Dispose();
                                            writer.Dispose();
                                        }

                                    buffer1 = null;
                                    relativePath = null;
                                    count = 0;
                                },
                                (in string actionName) => $"{DOWNLOADING_VALIDATION_FILES} for {actionName} action"))

                                    return;

                                log("Completed Validation Files Download");*/

                                void copyTempFiles(ITemporaryFileEnumerable fileEnumerable, in string destination, in IFileEnumerableBase current, in KeyValuePair<string, IFile> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ Copier copier)
                                {
                                    bool _ok = false;
                                    int i;
                                    EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                        ?
#endif
                                        node;
                                    EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                        ?
#endif
                                        tmpNode = null;

                                    bool validate(in KeyValuePair<string, IFile> __item, in Converter<string, Func<System.IO.Stream>
#if CS8
                                        ?
#endif
                                        >
#if CS8
                                        ?
#endif
                                        validator, in FuncOut<string, ulong?, System.IO.Stream
#if CS8
                                        ?
#endif
                                        > remote, in Converter<System.IO.Stream, byte[]
#if CS8
                                        ?
#endif
                                        > validatorDataProvider, out bool noValidator)
                                    {
                                        try
                                        {
                                            int bufferLength;
                                            ulong? _length = null;

                                            if (!(noValidator = (getReader = validator?.Invoke(replace(__item.Key))) == null || (sha256 = remote(__item.Key, out _length)) == null))
                                            {
                                                reader = getReader();

                                                if ((!_length.HasValue || _length.Value == (ulong)reader.Length) && (buffer1 = validatorDataProvider(reader)) != null)
                                                {
                                                    buffer2 = new byte[buffer1.Length];

                                                    i = 0;
                                                    bufferLength = 0;

                                                    do

                                                        i = sha256.Read(buffer2, i, buffer1.Length);

                                                    while ((bufferLength += i) < buffer1.Length);

                                                    if (sha256.Read(new byte[1], 0, 1) == 0)
                                                    {
                                                        i = 0;

                                                        while (i < buffer1.Length && (_ok = buffer1[i] == buffer2[i++])) { /* Left empty. */ }

                                                        return _ok;
                                                    }
                                                }
                                            }

                                            return false;
                                        }
                                        finally
                                        {
                                            void dispose(ref System.IO.Stream
#if CS8
                                                ?
#endif
                                                stream)
                                            {
                                                if (stream != null)
                                                {
                                                    stream.Dispose();
                                                    stream = null;
                                                }
                                            }

                                            dispose(ref reader);
                                            dispose(ref sha256);

                                            buffer1 = null;
                                            buffer2 = null;
                                        }
                                    }

                                    var stack = new Stack(temporaryFiles, fileEnumerable);

                                    EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                        ?
#endif
                                    mustCopy(in KeyValuePair<string, IFile> _item)
                                    {
                                        foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode _node in stack)

                                            if (validate(_item, _node.Value.Validator, _node.Value.GetRemoteValidationStream, _node.Value.GetValidationData, out _))

                                                return _node;

                                        return null;
                                    }

                                    if ((node = mustCopy(item)) == null)
                                    {
                                        foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode _node in stack)

                                            _node.Value.Delete(replace(item.Key));

                                        while (true)
                                        {
                                            bool tryCopy(in ITemporaryFileEnumerable _fileEnumerable, in string _destination, in IFileEnumerableBase _current, in KeyValuePair<string, IFile> _item/*, in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack _items*/)
                                            {
                                                i = 0;

                                                do
                                                {
                                                    doCopy(_destination, _current, _item, /*_items,*/ copier);

                                                    if (validate(_item, _fileEnumerable.Validator, _fileEnumerable.GetRemoteValidationStream, _fileEnumerable.GetValidationData, out bool noValidator) || noValidator)

                                                        return true;

                                                    i++;

                                                    _fileEnumerable.Delete(replace(_item.Key));
                                                }
                                                while (i < 3);

                                                return false;
                                            }

                                            if (tryCopy(fileEnumerable, destination, current, item/*, items*/))
                                            {
                                                if (hasError())

                                                    setError(RecoveredError);
                                            }

                                            else
                                            {
                                                MessageBoxResult result = MessageBox.Show($"The validation of the file:\n{item.Key}\ndid not succeeded after three trials. Do you want to try again? Choose 'Yes' to try again at most three times in a row (you will prompted after that new range of trials if the error persists), 'No' to skip this file (keep in mind that, in that case, the resulting installation could be unstable) or 'Cancel' to cancel the installation process.", "File validation error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);

                                                switch (result)
                                                {
                                                    case MessageBoxResult.Yes:

                                                        error(true);

                                                        continue;

                                                    case MessageBoxResult.No:

                                                        setError(NotRecoveredError);

                                                        break;

                                                    case MessageBoxResult.Cancel:

                                                        setError(FatalError);

                                                        break;
                                                }
                                            }

                                            error(false);

                                            return;
                                        }
                                    }

                                    else
                                    {
                                        void removeInvalidFiles(in KeyValuePair<string, IFile> _item, in Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                            ?
#endif
                                        > func)
                                        {
                                            tmpNode = node;

                                            while ((tmpNode = func()) != null)

                                                tmpNode.Value.Delete(replace(_item.Key));
                                        }

                                        removeInvalidFiles(item, () => tmpNode.Previous);
                                        removeInvalidFiles(item, () => tmpNode.Next);
                                    }
                                }

                                ActionIn<string, IFileEnumerableBase, KeyValuePair<string, IFile>, /*EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack,*/ Copier> action;

                                void _doCopy(ITemporaryFileEnumerable enumerable) => process(enumerable.Files.Select(item => item.Value), /*tmpFiles,*/ /*items*/ () => doInnerWork("Starting " + enumerable.ActionName, enumerable.ActionName, enumerable.ActionName, "Completed " + enumerable.ActionName, enumerable.Files, (in KeyValuePair<string, IFile> item) => copy(ProcessData.Installer.TemporaryDirectory, enumerable.Files, item, /*items,*/ action, enumerable.Copier)));

                                Action<ITemporaryFileEnumerable> _action = enumerable =>
                                {
                                    action = (in string destination, in IFileEnumerableBase current, in KeyValuePair<string, IFile> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Copier copier) => copyTempFiles(enumerable, destination, current, item, /*items,*/ copier);

                                    _doCopy(enumerable);

                                    action = doCopy;
                                    _action = _doCopy;
                                };

                                __process(temporaryFiles.GetDequeuerEnumerable(enumerable =>
                                {
                                    incrementStep(enumerable.ActionName);

                                    return true;
                                },
                                enumerable =>
                                {
                                    totalWritten = 0;
                                    currentTotalWritten = 0;

                                    reportProgress(100);

                                    enumerable.Dispose();

                                    return true;
                                }), _action, Delegates.SelfIn);
                            }

                            _process();
                        }, FatalError);
                    }

                    if (ProcessData.Installer.Actions.HasFlag(Actions.Install))
                    {
                        void _doWorkSafe(in Action action) => doWorkSafe(action, SuperFatalError);

                        string location = ProcessData.Installer.Location;

                        if (ProcessData.Installer.Error < FatalError)

                            _doWorkSafe(() =>
                            {
                                incrementStep("Installing");

                                process(ProcessData.Select(item => item.Value), /*EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack(),*/ () /*items*/ =>
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

                                        return doInnerWork("Copying", "copy", "Copy", "Files copied.", ProcessData, (in KeyValuePair<string, IFile> item) => copy(location, ProcessData, item, /*items,*/ doCopy, WinCopies.Installer.ProcessData.Copy));
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
                                    ("Cancelling...", "All files deleted.", 0, () =>
                                    {
                                        IEnumerable<KeyValuePair<string, Action>> getFiles()
                                        {
                                            IEnumerable<KeyValuePair<string, Action>> enumerate(string _path)
                                            {
                                                foreach (string directory in Directory.EnumerateDirectories(_path))

                                                    foreach (KeyValuePair<string, Action> item in enumerate(directory))

                                                        yield return item;

                                                foreach (string file in Directory.EnumerateFiles(_path))

                                                    yield return new KeyValuePair<string, Action>(file, () => System.IO.File.Delete(file));

                                                yield return new KeyValuePair<string, Action>(_path, () => Directory.Delete(_path));
                                            }

                                            return enumerate(ProcessData.Installer.Location);
                                        }

                                        setError(doInnerWork("Deleting", "delete", "Deletion", "Cancelled.", /*items,*/ getFiles(), (in KeyValuePair<string, Action> item) => item.Value()) ? FatalError : SuperFatalError);

                                        currentTotalWritten = 0;
                                        totalWritten = 0;

                                        setPercentProgress();
                                    }
                                    ));
                                });
                            });

                        _doWorkSafe(() =>
                        {
                            if (MessageBox.Show("Do you want to delete the temporary files?", "Temporary files", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                bool enumerate(string directory)
                                {
                                    MessageBoxResult? delete(in string item)
                                    {
                                        try
                                        {
                                            System.IO.File.Exists(item);

                                            return null;
                                        }
                                        catch (Exception ex)
                                        {
                                            return MessageBox.Show($"Could not delete the file '{item}'.\nMessage: {ex.Message}\nClick 'Yes' to retry, 'No' to skip this file or 'Cancel' to quit the installation.", $"File Deletion Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
                                        }
                                    }

                                    foreach (string item in Directory.EnumerateDirectories(directory))

                                        enumerate(item);

                                    foreach (string file in Directory.EnumerateFiles(directory))
                                    {
                                        ok(true);

                                        while (isOK())
                                        {
                                            switch (delete(file))
                                            {
                                                case null:

                                                    break;

                                                case MessageBoxResult.Yes:

                                                    trySetError();

                                                    continue;

                                                case MessageBoxResult.No:

                                                    setError(NotRecoveredError);

                                                    break;

                                                case MessageBoxResult.Cancel:

                                                    setError(SuperFatalError);

                                                    return false;
                                            }

                                            ok(false);
                                        }
                                    }

                                    return true;
                                }

                                if (enumerate(ProcessData.Installer.TemporaryDirectory))

                                    onSucceeded();
                            }
                        });
                    }
                }
            }
        }
    }
}
