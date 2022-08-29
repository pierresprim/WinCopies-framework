using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

using WinCopies.Collections;
using WinCopies.Collections.Generic;
using WinCopies.Linq;
using WinCopies.Util;

using static System.IO.Path;

using static WinCopies.Collections.DotNetFix.EnumerationDirection;
using static WinCopies.Installer.Error;

namespace WinCopies.Installer
{
    public partial class ProcessPageViewModel
    {
        protected partial class ProcessDataViewModel
        {
            protected partial class BackgroundWorker
            {
                private partial class ProcessorInfo
                {
                    public class Processor
                    {
                        private struct FileValidationProvider : IFileValidationProvider
                        {
                            private readonly IProcessData _processData;
                            private readonly ITemporaryFileEnumerable _enumerable;

                            public FileValidationProvider(in IProcessData processData, in ITemporaryFileEnumerable enumerable)
                            {
                                _processData = processData;
                                _enumerable = enumerable;
                            }

                            public Func<System.IO.Stream>
#if CS8
                                ?
#endif
                                GetLocalValidationStream(string file) => _enumerable.GetLocalValidationStream(file);
                            public System.IO.Stream
#if CS8
                                ?
#endif
                                GetRemoteValidationStream(string file, out ulong? length) => _enumerable.GetRemoteValidationStream(file, out length);
                            public byte[]
#if CS8
                                ?
#endif
                                GetValidationData(System.IO.Stream stream) => _processData.GetValidationData(stream);
                        }

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

                        #region Fields
                        private readonly ProcessorInfo _processorInfo;
                        private byte _bools;
                        private ulong _total = 0;
                        private ulong _totalWritten = 0;
                        private string _tmp;
                        private uint _count = 0;
                        private byte _currentTotalWritten = 0;
                        private string _path = null;
                        private string _relativePath = null;
                        private ulong? _length = null;
                        private const byte COMPLETED = 100;
                        private readonly DoWorkEventArgs _doWorkEventArgs;
                        private IEnumerable<KeyValuePair<string, string>>
#if CS8
                            ?
#endif
                            _resources;
                        private IEnumerable<IEnumerable<string>>
#if CS8
                            ?
#endif
                            _paths;
                        #endregion Fields

                        private BackgroundWorker Worker => _processorInfo.Worker;
                        public ProcessDataViewModel ProcessData => Worker.ProcessData;
                        private Installer Installer => ProcessData.Installer.Model;
                        public Error Error { get => Installer.Error; set => Installer.Error = value; }

                        public EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList
#if CS8
                            ?
#endif
                            TemporaryFiles => _processorInfo.TemporaryFiles;

                        public Processor(in ProcessorInfo processorInfo, in DoWorkEventArgs doWorkEventArgs)
                        {
                            _processorInfo = processorInfo;
                            _doWorkEventArgs = doWorkEventArgs;
                        }

                        private byte SetPercentProgress() => _currentTotalWritten = (byte)((float)(_totalWritten += _count) / _total * 100);

                        public static string Replace(in string path) => path.Replace('/', DirectorySeparatorChar);

                        public void Log(in string message)
                        {
                            ProcessData.AddLogMessage(message);
                            System.Console.WriteLine(message);
                        }

                        public void ReportProgress(in byte percentProgress) => Worker.ReportProgress(percentProgress, percentProgress);

                        public bool GetBit(in FlagPosition pos) => _bools.GetBit((byte)pos);
                        public void SetBit(in FlagPosition pos, in bool value) => UtilHelpers.SetBit(ref _bools, (byte)pos, value);

                        public bool IsOK() => GetBit(FlagPosition.IsOK);
                        public void OK(in bool value) => SetBit(FlagPosition.IsOK, value);
                        public bool HasError() => GetBit(FlagPosition.HasError);
                        public void SetErrorFlag(in bool value) => SetBit(FlagPosition.HasError, value);

                        public void TrySetError()
                        {
                            if (Error <= RecoveredError)

                                SetErrorFlag(true);
                        }

                        public void OnSucceeded()
                        {
                            if (HasError())
                            {
                                SetErrorFlag(false);

                                Error = RecoveredError;
                            }
                        }

                        public bool HasFatalErrorOccurred() => Error >= FatalError;

                        public void Process(in IEnumerable<IFile> files, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Action/*<EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack>*/ action)
                        {
                            _total = 0;
                            _bools = 0;

                            ReportProgress(0);

                            foreach (IFile file in files)
                            {
                                _total += _totalWritten = (ulong)file.Stream.Length;

                                Log($"Found {file.Name} {Consts.Symbols.GeneralPunctuation.Bullet} Size: {new IO.Size(_totalWritten)}");

                                file.Stream.Dispose();
                            }

                            _totalWritten = 0;

                            action(/*items*/);
                        }

                        public static bool Process(in IEnumerable<ITemporaryFileEnumerable> temporaryFiles, in Predicate<ITemporaryFileEnumerable> predicate)
                        {
                            foreach (ITemporaryFileEnumerable enumerable in temporaryFiles)

                                if (predicate(enumerable))

                                    return true;

                            return false;
                        }

                        public bool Process(in IEnumerable<ITemporaryFileEnumerable> temporaryFiles, Action<ITemporaryFileEnumerable> action, ConverterIn<string, string> msg) => Process(temporaryFiles, enumerable =>
                        {
                            void _log(in ConverterIn<string, string> _msg) => Log(_msg(msg(enumerable.ActionName)));
                            _log((in string _msg) => $"Starting {_msg}");

                            action(enumerable);

                            if (HasFatalErrorOccurred())

                                return true;

                            _log((in string _msg) => $"{_msg} Completed");

                            return false;
                        });

                        public bool DoInnerWork<T>(in string enter, in string errorMessage, in string errorCaption, in IEnumerable<KeyValuePair<string, T>> _items, in ActionIn<KeyValuePair<string, T>> action)
                        {
                            void onError() => Error = FatalError;
                            void logError(in string
#if CS8
                                ?
#endif
                                msg, in string _msg) => Log($"An {msg}error occurred. Error message: {_msg}");

                            foreach (KeyValuePair<string, T> item in _items)
                            {
                                if (ProcessData.Installer.Error >= FatalError)
                                {
                                    Log("Fatal error during file enumeration.");

                                    return false;
                                }

                                do
                                {
                                    Log($"{enter} {item.Key}...");

                                    OK(false);

                                    try
                                    {
                                        action(item);

                                        OnSucceeded();
                                    }

                                    catch (IOException ex)
                                    {
                                        string message = ex.Message;

                                        logError("I/O ", message);

                                        switch (MessageBox.Show($"Could not perform '{errorMessage}' action for the file '{item.Key}'.\nMessage: {message}\nClick 'Yes' to retry, 'No' to skip this file or 'Cancel' to quit the installation.", $"File {errorCaption} Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes))
                                        {
                                            case MessageBoxResult.Yes:

                                                OK(true);

                                                TrySetError();

                                                break;

                                            case MessageBoxResult.No:

                                                Error = NotRecoveredError;

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

                                while (IsOK());
                            }

                            return true;
                        }

                        public void DoCopy<T>(in string destination, in IFileEnumerable current, in KeyValuePair<string, T> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Copier action) where T : IFile
                        {
                            long currentTotal = 0;

                            string getRelativePath(string
#if CS8
                                ?
#endif
                                oldRelativeDirectory)
                            {
                                void replace(ref string _path) => _path = Replace(_path);

                                replace(ref _relativePath);

                                if (oldRelativeDirectory != null)
                                {
                                    replace(ref oldRelativeDirectory);

                                    if (!oldRelativeDirectory.StartsWith(DirectorySeparatorChar))

                                        oldRelativeDirectory = DirectorySeparatorChar + oldRelativeDirectory;

                                    if (oldRelativeDirectory.EndsWith(DirectorySeparatorChar))

                                        oldRelativeDirectory = oldRelativeDirectory.Remove(oldRelativeDirectory.Length - 1);

                                    if (_relativePath.StartsWith(oldRelativeDirectory))

                                        return _relativePath
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

                                return _relativePath;
                            }

                            using
#if !CS8
                        (
#endif
                                System.IO.Stream writer = current.GetWriter(_path = $"{destination}{(current.RelativeDirectory == null ? null : '\\' + current.RelativeDirectory)}{getRelativePath(current.OldRelativeDirectory)}")
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
                                    _count = progress;

                                    Worker.ReportProgress(SetPercentProgress(), (byte)((float)(currentTotal += _count) / _length.Value * 100));
                                }, out _length);

                                Worker.ReportProgress(_currentTotalWritten, COMPLETED);

                                item.Value.Stream.Dispose();
                            }
                        }

                        public void Copy<T>(in string destination, in IFileEnumerable current, in KeyValuePair<string, T> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in ActionIn<string, IFileEnumerable, KeyValuePair<string, T>, /*EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack,*/ Copier> action, in Copier copier) where T : IFile
                        {
                            _count = 0;
                            _relativePath = item.Key;

                            if ((_resources = current.Resources) != null)
                            {
#if CS8
                                static
#endif
                                    string replace(in string text) => text.Replace("_u", "_").Replace("_b", "\\").Replace("_d", ".");

                                foreach (KeyValuePair<string, string> resource in _resources)

                                    if (replace(_relativePath).StartsWith(_tmp = replace(resource.Key) + '.'))
                                    {
                                        _relativePath = $"{resource.Value}\\{_relativePath.Substring(_tmp.Length)}";

                                        break;
                                    }
                            }

                            action(destination, current, item, /*items,*/ copier);

                            Log($"Copied {_path}");
                        }

                        private void DeleteOldFiles(IEnumerable<string> enumerable, IEnumerable<string> paths)
                        {
                            bool? contains;

                            foreach (string physicalFile in enumerable)
                            {
                                bool? mustBeDeleted() // We cannot use the Contains method from LINQ because if it was, the enumeration would continue even if an error occurs, we have to cancel it manually in that case. A custom method is needed for this purpose.
                                {
                                    foreach (string file in paths)
                                    {
                                        if (HasFatalErrorOccurred())

                                            return null;

                                        if (physicalFile.EndsWith(Replace(file)))

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

                        public IEnumerable<ActionProvider> GetPrepareActions()
                        {
                            yield return processor => new ProcessAction("Parsing", false, FatalError, () =>
                            {
                                EnumerableHelper<IEnumerable<string>>.IEnumerableStack paths = EnumerableHelper<IEnumerable<string>>.GetEnumerableStack();

                                ITemporaryFileEnumerable enumerable = null;

                                void doProcess(in ITemporaryFileEnumerable _enumerable) => DeleteOldFiles((enumerable = _enumerable).PhysicalFiles, enumerable.Paths.AppendValues(paths));

                                void push() => paths.Push(enumerable.Paths.AsLocalFileEnumerable());

                                ActionIn<ITemporaryFileEnumerable> ___action = (in ITemporaryFileEnumerable _enumerable) =>
                                {
                                    doProcess(_enumerable);

                                    ___action = (in ITemporaryFileEnumerable __enumerable) =>
                                    {
                                        push();

                                        doProcess(__enumerable);
                                    };
                                };

                                if (processor.Process(TemporaryFiles, _enumerable => ___action(_enumerable), (in string actionName) => $"Parsing items for {actionName} action"))

                                    return;

                                if (ProcessData.Installer.Actions.HasFlag(Actions.Install))
                                {
                                    push();

                                    _paths = paths;
                                }
                            });

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

                            void copyTempFiles(ITemporaryFileEnumerable fileEnumerable, in string destination, in IFileEnumerable current, in KeyValuePair<string, IFile> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ Copier copier)
                            {
                                int i;
                                EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                ?
#endif
                                _node;
                                EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                ?
#endif
                                tmpNode = null;

                                bool validate(in KeyValuePair<string, IFile> __item, in IFileValidationProvider fileValidationProvider, out bool noValidator)
                                {
                                    try
                                    {
                                        int bufferLength;
                                        ulong? _length = null;

                                        if (!(noValidator = (getReader = fileValidationProvider.GetLocalValidationStream(Replace(__item.Key))) == null || (sha256 = fileValidationProvider.GetRemoteValidationStream(__item.Key, out _length)) == null))
                                        {
                                            reader = getReader();

                                            if ((!_length.HasValue || _length.Value == (ulong)reader.Length) && (buffer1 = fileValidationProvider.GetValidationData(reader)) != null)
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

                                                    bool _ok = false;

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

                                var stack = new Stack(TemporaryFiles, fileEnumerable);

                                // The following method scans the installation and temporary directories for a local file that did not changed from the last download/installation. The temporary directories are the download directories of the file pre-processing steps. If the return value is an instance of the return interface, then a local temporary file could be validated and the corresponding files between it have to be removed, if any. Null is returned otherwise. In addition to the return value, the installation file changed flag have to be tested. If it is set to false, then it indicates that the corresponding installation file could be validated. It is set to true when no file could be validated.

                                EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                    ?
#endif
                                isCopyRequested(KeyValuePair<string, IFile> _item)
                                {
                                    bool _validate(in IFileValidationProvider fileValidationProvider) => validate(_item, fileValidationProvider, out _);

                                    void installationFileChanged(in bool value) => SetBit(FlagPosition.InstallationFileChanged, true);

                                    if (_validate(new FileValidationProvider(Worker.ProcessData, stack.Peek())))
                                    {
                                        installationFileChanged(false);

                                        return null;
                                    }

                                    // We enumerate the temporary file enumerables as a stack (LIFO mode) instead of a queue (FIFO mode) because we want to keep the closest validated file, if any, of the final step that is the installation of the pre-processed files.

                                    foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode __node in stack)

                                        // A local file could be validated, so we return its node from the stack.

                                        if (_validate(__node.Value))

                                            return __node;

                                    // No local file could be validated, so we return null.

                                    installationFileChanged(true);

                                    return null;
                                }

                                // No local temporary file matches the current remote one -

                                if ((_node = isCopyRequested(item)) == null)
                                {
                                    // - so we remove them all -

                                    foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode __node in stack)

                                        __node.Value.Delete(Replace(item.Key));

                                    // - and then we copy the corresponding remote file to the temporary directory if the corresponding installation file could not be validated either.

                                    if (GetBit(FlagPosition.InstallationFileChanged))

                                        while (true)
                                        {
                                            bool tryCopy(in ITemporaryFileEnumerable _fileEnumerable, in string _destination, in IFileEnumerable _current, in KeyValuePair<string, IFile> _item/*, in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack _items*/)
                                            {
                                                i = 0;

                                                do
                                                {
                                                    DoCopy(_destination, _current, _item, /*_items,*/ copier);

                                                    if (validate(_item, _fileEnumerable, out bool noValidator) || noValidator)

                                                        return true;

                                                    i++;

                                                    _fileEnumerable.Delete(Replace(_item.Key));
                                                }
                                                while (i < 3);

                                                return false;
                                            }

                                            if (tryCopy(fileEnumerable, destination, current, item/*, items*/))
                                            {
                                                if (HasError())

                                                    Error = RecoveredError;
                                            }

                                            else
                                            {
                                                MessageBoxResult result = MessageBox.Show($"The validation of the file:\n{item.Key}\ndid not succeeded after three trials. Do you want to try again? Choose 'Yes' to try again at most three times in a row (you will prompted after that new range of trials if the error persists), 'No' to skip this file (keep in mind that, in that case, the resulting installation could be unstable) or 'Cancel' to cancel the installation process.", "File validation error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);

                                                switch (result)
                                                {
                                                    case MessageBoxResult.Yes:

                                                        SetErrorFlag(true);

                                                        continue;

                                                    case MessageBoxResult.No:

                                                        Error = NotRecoveredError;

                                                        break;

                                                    case MessageBoxResult.Cancel:

                                                        Error = FatalError;

                                                        break;
                                                }
                                            }

                                            SetErrorFlag(false);

                                            return;
                                        }
                                }

                                // At least one local file is equal to the current remote one.

                                else
                                {
                                    void removeInvalidFiles(in KeyValuePair<string, IFile> _item, in Func<EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode
#if CS8
                                    ?
#endif
                                > func)
                                    {
                                        tmpNode = _node;

                                        while ((tmpNode = func()) != null)

                                            tmpNode.Value.Delete(Replace(_item.Key));
                                    }

                                    // We remove the corresponding local files excepting the validated one, assuming that the other versions won't be useful for the following steps. Furthermore, as the algorythms beyond the local file enumerations may parse all the file system entries and as the current parsing is made using the LIFO enumeration mode on the file enumerables, not removing these corresponding files could replace the valid file by themselves when these steps will be running.

                                    removeInvalidFiles(item, () => tmpNode.Previous);
                                    removeInvalidFiles(item, () => tmpNode.Next);
                                }
                            }

                            ActionIn<string, IFileEnumerable, KeyValuePair<string, IFile>, /*EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack,*/ Copier> action;

                            void _doCopy(ITemporaryFileEnumerable _enumerable) => Process(_enumerable.Files.Select(item => item.Value), /*tmpFiles,*/ /*items*/ () =>
                            {
                                string actionName = _enumerable.ActionName;

                                DoInnerWork(actionName, actionName, actionName, _enumerable.Files, (in KeyValuePair<string, IFile> item) => Copy(Worker.ProcessData.Installer.TemporaryDirectory, _enumerable.Files, item, /*items,*/ action, _enumerable.Copier));
                            });

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

                            Action<ITemporaryFileEnumerable> _action = enumerable =>
                            {
                                action = (in string destination, in IFileEnumerable current, in KeyValuePair<string, IFile> item, /*in EnumerableHelper<KeyValuePair<string, Action>>.IEnumerableStack items,*/ in Copier copier) => copyTempFiles(enumerable, destination, current, item, /*items,*/ copier);

                                _doCopy(enumerable);

                                action = DoCopy;
                                _action = _doCopy;
                            };

                            ITemporaryFileEnumerable ___enumerable = null;

                            void doWork()
                            {
                                _action(___enumerable);

                                _totalWritten = 0;
                                _currentTotalWritten = 0;

                                ReportProgress(100);
                            }

                            EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode node;

                            Action __action;

                            if (Installer.Actions.HasFlag(Actions.Install))

                                __action = doWork;

                            else
                            {
                                node = null;

                                __action = () =>
                                {
                                    doWork();

                                    ___enumerable.Dispose();

                                    node.Remove();
                                };
                            }

                            node = TemporaryFiles.FirstNode;

                        LOOP:
                            if (node == null)

                                yield break;

                            yield return processor => new ProcessAction((___enumerable = node.Value).ActionName, false, FatalError, __action);

                            node = node.Next;

                            goto LOOP;
                        }

                        public IEnumerable<ActionProvider> GetInstallActions()
                        {
                            string location = ProcessData.Installer.Location;

                            yield return processor => new ProcessAction("Installing", false, SuperFatalError, () => Process(ProcessData.Select(item => item.Value), /*EnumerableHelper<KeyValuePair<string, Action>>.GetEnumerableStack(),*/ () /*items*/ =>
                            {
                                bool doWork()
                                {
                                    try
                                    {
                                        bool noValidation()
                                        {
                                            if (_paths == null)
                                            {
                                                if (TemporaryFiles?.HasItems == false)

                                                    return true;

                                                _paths = TemporaryFiles.AsEnumerable(LIFO).Select(node => node.Value.Paths.AsLocalFileEnumerable());
                                            }

                                            return false;
                                        }

                                        if (noValidation())

                                            ProcessData.DeleteOldFiles(message => Log(message));

                                        else

                                            DeleteOldFiles(IO.Directory.EnumerateFilesRecursively(ProcessData.Installer.Location), _paths.Merge());
                                    }
                                    catch (Exception ex)
                                    {
                                        Log($"Error: {ex.Message}");

                                        return false;
                                    }
                                    finally
                                    {
                                        if (_paths != null)

                                            foreach (EnumerableHelper<ITemporaryFileEnumerable>.ILinkedListNode node in UtilHelpers.EnumerateUntilNull(TemporaryFiles.FirstNode, node => node.Next))
                                            {
                                                node.Value.Dispose();

                                                node.Remove();
                                            }
                                    }

                                    return DoInnerWork("Copying", "copy", "Copy", ProcessData, (in KeyValuePair<string, IFile> item) => Copy(location, ProcessData, item, /*items,*/ DoCopy, WinCopies.Installer.ProcessData.Copy));
                                }

                                if (!doWork())

                                    Error = SuperFatalError;
                            }));

                            void doExtraWork(in byte progress, in Action action)
                            {
                                action();

                                Worker.DoExtraWork(_doWorkEventArgs);

                                ReportProgress(progress);
                            }

                            ProcessAction getProcessAction(in string name, bool force, Action action) => new ProcessAction(name, force, SuperFatalError, () => doExtraWork(force ? (byte)0 : (byte)100, action));

                            yield return processor => Error < FatalError ? getProcessAction("Custom actions", false, () =>
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
                                            OK(option.Action(out _log));
                                        }

                                        catch (Exception ex)
                                        {
                                            OK(false);
                                            errorMessage = ex.Message;
                                        }

                                        Log($"Custom Action: {option.Name} -- {_log ?? "<No log could be retrieved or provided.>"}");

                                        if (IsOK())
                                        {
                                            OnSucceeded();

                                            break;
                                        }

                                        MessageBoxResult result = MessageBox.Show($"Could not perform the action '{option.Name}'.\nMessage: {errorMessage ?? "<No error message.>"}\nClick 'Yes' to retry, 'No' to skip this action, 'Cancel' to skip all custom actions.", "Custom Action Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

                                        switch (result)
                                        {
                                            case MessageBoxResult.Yes:

                                                OK(true);

                                                TrySetError();

                                                break;

                                            case MessageBoxResult.No:

                                                Error = NotRecoveredError;

                                                break;

                                            case MessageBoxResult.Cancel:

                                                Error = FatalError;

                                                return;
                                        }
                                    }
                                    while (IsOK());
                            }) : getProcessAction("Cancelling", true, () =>
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

                                Error = DoInnerWork("Deleting", "delete", "Deletion", /*items,*/ getFiles(), (in KeyValuePair<string, Action> item) => item.Value()) ? FatalError : SuperFatalError;

                                _currentTotalWritten = 0;
                                _totalWritten = 0;

                                SetPercentProgress();
                            });

                            if (GetBit(FlagPosition.ClearTemporaryFiles))

                                yield return processor => new ProcessAction("Cleaning", true, SuperFatalError, () =>
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
                                                OK(true);

                                                while (IsOK())
                                                {
                                                    switch (delete(file))
                                                    {
                                                        case null:

                                                            break;

                                                        case MessageBoxResult.Yes:

                                                            TrySetError();

                                                            continue;

                                                        case MessageBoxResult.No:

                                                            Error = NotRecoveredError;

                                                            break;

                                                        case MessageBoxResult.Cancel:

                                                            Error = SuperFatalError;

                                                            return false;
                                                    }

                                                    OK(false);
                                                }
                                            }

                                            return true;
                                        }

                                        if (enumerate(ProcessData.Installer.TemporaryDirectory))

                                            OnSucceeded();
                                    }
                                });
                        }
                    }
                }
            }
        }
    }
}
