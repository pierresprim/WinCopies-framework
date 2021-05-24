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

using Microsoft.WindowsAPICodePack.Win32Native;
using System;
using System.Runtime.InteropServices;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Util;

using static WinCopies.ThrowHelper;
using static WinCopies.IO.Resources.ExceptionMessages;
using System.IO;
using System.Security;
using WinCopies.Collections.Generic;
using System.Linq;
using WinCopies.Collections;

namespace WinCopies.IO.Process
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProcessGuidAttribute : Attribute
    {
        public string Guid { get; }

        public ProcessGuidAttribute(string guid) => Guid = guid;
    }

    public static class ProcessHelper
    {
        public static bool OnPathLoaded<TItems, TError, TProcessDelegateParam, TProcessEventDelegates>(in TItems _path, in ProcessTypes<TItems>.ProcessErrorTypes<TError>.ProcessOptions options, in ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in object cancellationPendingDelegateParam, in Action<TItems> action) where TItems : IPathInfo where TProcessEventDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
        {
            if (options.PathLoadedDelegate(_path) && !processDelegates.CancellationPendingDelegate.RaiseEvent(cancellationPendingDelegateParam))
            {
                action(_path);

                return true;
            }

            return false;
        }

        public static bool OnPathLoaded<TItems, TError, TProcessDelegateParam, TProcessEventDelegates>(in TItems _path, in ProcessTypes<TItems>.ProcessErrorTypes<TError>.ProcessOptions options, in ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in Action<TItems> action) where TItems : IPathInfo where TProcessEventDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter => OnPathLoaded(_path, options, processDelegates, null, action);

        public static string GetDestinationPath(in IPathInfo x, in IPathInfo y) => $"{x.Path}{Path.PathSeparator}{y.GetPath(true)}";

        public static IProcessError<ProcessError> GetIOError<T>(ProcessTypes<T>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories factory, out ErrorCode errorCode) where T : IPath
        {
            errorCode = (ErrorCode)Marshal.GetLastWin32Error();

            var _errorCode = errorCode;

            IProcessError<ProcessError> getError(in ProcessError error, in string message) => factory.GetError(error, message, _errorCode);

            switch (errorCode)
            {
                case ErrorCode.AlreadyExists:

                    return getError(ProcessError.FileSystemEntryAlreadyExists, FileSystemEntryAlreadyExists);

                case ErrorCode.PathNotFound:

                    return getError(ProcessError.PathNotFound, PathNotFound);

                case ErrorCode.AccessDenied:

                    return getError(ProcessError.ReadProtection, string.Format(ReadProtection, Source));

                case ErrorCode.DiskFull:

                    return getError(ProcessError.NotEnoughSpace, NotEnoughSpace);

                case ErrorCode.DiskOperationFailed:

                    return getError(ProcessError.DiskError, DiskError);

                default:

                    return getError(ProcessError.UnknownError, UnknownError);
            }
        }

        public static FileStream TryGetFileStream<T>(in ProcessTypes<T>.ProcessErrorTypes<ProcessError>.IProcessErrorFactories factory, in string path, in int bufferLength, out IProcessError<ProcessError> error) where T : IPath
        {
            try
            {
                FileStream result = File.GetFileStream(path, bufferLength);

                error = factory.GetNoErrorError();

                return result;
            }

            catch (System.IO.IOException ex) when (ex.Is(false, typeof(System.IO.FileNotFoundException), typeof(System.IO.DirectoryNotFoundException)))
            {
                error = factory.GetError(ProcessError.PathNotFound, PathNotFound, ErrorCode.PathNotFound);
            }

            catch (System.IO.PathTooLongException)
            {
                error = factory.GetError(ProcessError.PathTooLong, PathTooLong, ErrorCode.InvalidName);
            }

            catch (System.IO.IOException)
            {
                error = factory.GetError(ProcessError.UnknownError, UnknownError, (ErrorCode)(-1));
            }

            catch (Exception ex) when (ex.Is(false, typeof(UnauthorizedAccessException), typeof(SecurityException)))
            {
                error = factory.GetError(ProcessError.ReadProtection, string.Format(ReadProtection, Source), ErrorCode.ReadFault);
            }

            return null;
        }

        public static ReadOnlyEnumerableQueueCollection<IPathInfo> GetInitialPaths(System.Collections.Generic.IEnumerator<string> enumerator, IPathInfo sourcePath) => new ReadOnlyEnumerableQueueCollection<IPathInfo>(
                new Enumerable<string>(
                    () => enumerator
                    ).Select(
                        path => path.EndsWith(":\\") || path.EndsWith(":\\\\")
                                ? new PathTypes<IPathInfo>.RootPath(path, true)
                                : (IPathInfo)new PathTypes<IPathInfo>.PathInfo(System.IO.Path.GetFileName(path), sourcePath))
                    .ToEnumerableQueue());

        public static ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>.ProcessDelegates GetDefaultProcessDelegates() => new ProcessDelegateTypes<IPathInfo, IProcessProgressDelegateParameter>
                                .ProcessDelegates(
                                    new EventDelegate<IPathInfo>(),
                                    EventAndQueryDelegate<bool>.GetANDALSO_Delegate(true),
                                    EventAndQueryDelegate<object>.GetANDALSO_Delegate(false),
                                    EventAndQueryDelegate<IProcessProgressDelegateParameter>.GetANDALSO_Delegate(true));
    }

    namespace ObjectModel
    {
        public static partial class ProcessObjectModelTypes<TItemsIn, TItemsOut, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam> where TItemsIn : IPathInfo where TItemsOut : IPathInfo where TFactory : ProcessTypes<TItemsOut>.ProcessErrorTypes<TError>.IProcessErrorFactories where TProcessDelegates : ProcessDelegateTypes<TItemsOut, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<TItemsOut, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
        {
            public abstract partial class Process : ProcessInterfaceModelTypes<TItemsIn, TItemsOut, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>
            {
                #region Fields
                private bool _arePathsLoaded;
                private uint _initialItemCount;
                private IProcessError<TError> _error;
                private IEnumerableQueue<TItemsIn> _initialPaths;
                private Size _actualRemainingSize;
                private ProcessTypes<TItemsOut>.IProcessQueue _paths;
                private ProcessTypes<TItemsOut>.IProcessQueue _pathsReadOnlyQueue;
                private NullableGeneric<TProcessDelegates> _processDelegates;
                private NullableGeneric<TProcessEventDelegates> _processEventDelegates;
                private IProcessLinkedList<TItemsOut, TError> _errorPaths;
                private ProcessTypes<IProcessErrorItem<TItemsOut, TError>>.IProcessQueue _errorPathsReadOnlyQueue;
                private NullableGeneric<TFactory> _factory;
                private Size _initialTotalSize;
                private EventDelegate<string> _propertyEventDelegate = new
#if !CS9
                EventDelegate<string>
#endif
                ();
                #endregion Fields

                #region Properties
                public abstract string Guid { get; }

                public TFactory Factory => this.GetIfNotDisposed(_factory).Value;

                IProcessErrorFactory<TError> ProcessInterfaceModelTypes<TItemsIn, TItemsOut, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>.Factory => Factory;

                protected TProcessDelegates ProcessDelegates => this.GetIfNotDisposed(_processDelegates).Value;

                public TProcessEventDelegates ProcessEventDelegates => this.GetIfNotDisposed(_processEventDelegates).Value;

                public bool ArePathsLoaded
                {
                    get => _arePathsLoaded;

                    private set
                    {
                        _arePathsLoaded = value;

                        _propertyEventDelegate.RaiseEvent(nameof(ArePathsLoaded));
                    }
                }

                public abstract TItemsIn SourcePath { get; }

                public ProcessTypes<TItemsOut>.IProcessQueue Paths => this.GetIfNotDisposed(_pathsReadOnlyQueue);

                public IEnumerableQueue<TItemsIn> InitialPaths => this.GetIfNotDisposed(_initialPaths);

                public abstract string Name { get; }

                public IProcessError<TError> Error
                {
                    get => _error;

                    private set
                    {
                        _error = value;

                        _propertyEventDelegate.RaiseEvent(nameof(Error));
                    }
                }

                public IProcessErrorFactoryData<TError> ProcessErrorFactoryData => this.GetIfNotDisposed(_factory).Value;

                public ProcessTypes<IProcessErrorItem<TItemsOut, TError>>.IProcessQueue ErrorPaths => this.GetIfNotDisposed(_errorPathsReadOnlyQueue);

                public Size InitialTotalSize
                {
                    get => _initialTotalSize;

                    private set
                    {
                        _initialTotalSize = value;
                        ActualRemainingSize = value;

                        _propertyEventDelegate.RaiseEvent(nameof(InitialTotalSize));
                    }
                }

                public uint InitialItemCount
                {
                    get => _initialItemCount;

                    private set
                    {
                        _initialItemCount = value;

                        _propertyEventDelegate.RaiseEvent(nameof(InitialItemCount));
                    }
                }

                public Size ActualRemainingSize
                {
                    get => _actualRemainingSize;

                    private set
                    {
                        _actualRemainingSize = value;

                        _propertyEventDelegate.RaiseEvent(nameof(ActualRemainingSize));
                    }
                }
                #endregion Properties

                public Process(in IEnumerableQueue<TItemsIn> initialPaths, in ProcessTypes<TItemsOut>.IProcessQueue paths, in IProcessLinkedList<TItemsOut, TError> errorsQueue, in TProcessDelegates processDelegates, in TFactory factory)
                {
                    ThrowIfNull(initialPaths, nameof(initialPaths));

                    if (!initialPaths.IsReadOnly)

                        throw new ArgumentException($"{nameof(initialPaths)} must be read-only.");

                    ThrowIfNull(paths, nameof(paths));
                    ThrowIfNull(errorsQueue, nameof(errorsQueue));

                    if (processDelegates == null)

                        throw GetArgumentNullException(nameof(processDelegates));

                    _initialPaths = initialPaths;

                    _paths = paths;
                    _pathsReadOnlyQueue = paths.AsReadOnly();

                    _errorPaths = errorsQueue;
                    _errorPathsReadOnlyQueue = errorsQueue.AsReadOnly();

                    _processDelegates = new NullableGeneric<TProcessDelegates>(processDelegates);
                    _processEventDelegates = new NullableGeneric<TProcessEventDelegates>(processDelegates.GetProcessEventDelegates());

                    if (_processEventDelegates.Value == null)

                        throw new ArgumentException($"The given {nameof(processDelegates)} did not provide a non-null value for process event delegates.");

                    _factory = new NullableGeneric<TFactory>(factory == null ? throw GetArgumentNullException(nameof(factory)) : factory);

                    _error = _factory.Value.GetNoErrorError();
                }

                #region Methods
                void IPropertyObservable.AddPropertyChangedDelegate(Action<string> action) => this.GetIfNotDisposed(_propertyEventDelegate).Add(action);

                void IPropertyObservable.RemovePropertyChangedDelegate(Action<string> action) => this.GetIfNotDisposed(_propertyEventDelegate).Remove(action);

                protected abstract bool Check(out IProcessError<TError> error);

                protected abstract bool LoadPathsOverride(out IProcessError<TError> error, out bool clearOnError);

                protected void AddPath(TItemsOut path) => _paths.Enqueue(path);

                protected bool DoWorkSafe(in Func<bool> _delegate)
                {
                    try
                    {
                        return _delegate();
                    }

                    catch (Exception ex)
                    {
                        Error = _factory.Value.GetError(_factory.Value.UnknownError, ex, (ErrorCode)(-1));
                    }

                    finally
                    {
                        try
                        {
                            ResetStatus();
                        }

                        catch (Exception ex)
                        {
                            Error = _factory.Value.GetError(_factory.Value.WrongStatusError, ex, (ErrorCode)(-1));
                        }
                    }

                    return false;
                }

                private bool _LoadPaths()
                {
                    bool result = LoadPathsOverride(out IProcessError<TError> error, out bool clearOnError);

                    Error = error;

                    if (result && Equals(error.Error, _factory.Value.NoError))
                    {
                        ArePathsLoaded = true;

                        InitialTotalSize = Paths.TotalSize;

                        InitialItemCount = Paths.Count;
                    }

                    else if (clearOnError)
                    {
                        Error = error;

                        _paths.Clear();
                    }

                    return result;
                }

                public bool LoadPaths()
                {
                    ThrowIfDisposed(this);

                    return DoWorkSafe(_LoadPaths);
                }

                protected abstract bool DoWork(TItemsOut path, out IProcessError<TError> error, out bool isErrorGlobal);

                protected abstract bool DoWork(IProcessErrorItem<TItemsOut, TError> path, out IProcessError<TError> error, out bool isErrorGlobal);

                private delegate bool DoWorkFunc<T>(T path, out IProcessError<TError> error, out bool isErrorGlobal);

                private bool _DoWork<T>(in _IQueue<T> paths, in Func<T, TItemsOut> getPathDelegate, in DoWorkFunc<T> func, in Func<T, IProcessError<TError>, IProcessErrorItem<TItemsOut, TError>> _func) where T : IPathInfo
                {
#pragma warning disable IDE0018 // Inline variable declaration
                    IProcessError<TError> error;
                    bool isErrorGlobal;
#pragma warning restore IDE0018 // Inline variable declaration
                    bool result;

                    do
                    {
                        _processDelegates.Value.ProgressDelegate.RaiseEvent(getPathDelegate(paths.Peek()));

                        result = func(paths.Peek(), out error, out isErrorGlobal);

                        if (Equals(error.Error, _factory.Value.NoError))

                            _ = paths.Dequeue();

                        else if (isErrorGlobal)
                        {
                            Error = error;

                            return false;
                        }

                        else

                            _errorPaths.Enqueue(_func(paths.Dequeue(), error));

                        ActualRemainingSize = _paths.TotalSize;

                    } while (result && paths.HasItems);

                    return true;
                }

                private bool DoWork<T>(in _IQueue<T> paths, in Func<T, TItemsOut> getPathDelegate, in DoWorkFunc<T> func, in Func<T, IProcessError<TError>, IProcessErrorItem<TItemsOut, TError>> _func) where T : IPathInfo
                {
                    bool result = _DoWork(paths, getPathDelegate, func, _func);

                    paths.Dispose();

                    return result;
                }

                protected virtual bool OnRunWorkerCompleted() => true;

                private bool _Start()
                {
                    bool result = Check(out IProcessError<TError> globalError);

                    Error = globalError;

                    if ((result &= _processDelegates.Value.CheckPerformedDelegate.RaiseEvent(result)))

                        result = Equals(globalError.Error, _factory.Value.NoError);

                    if (!result) // We do not perform an 'else if' because we want to check the latest result, but we do not have to check the equality of globalError.Error and NoError if the previous result was false; we exit the method if at least one result is false.

                        return false;

                    if (_paths.HasItems)

                        return DoWork(new Queue(_paths), path => path, (TItemsOut path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessTypes<TItemsOut, TError>.ProcessErrorItem(_path, _error));

                    else if (_errorPaths.Count != 0)

                        return DoWork(new LinkedList(_errorPaths), path => path.Item, (IProcessErrorItem<TItemsOut, TError> path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessTypes<TItemsOut, TError>.ProcessErrorItem(_path.Item, _error));

                    return OnRunWorkerCompleted();
                }

                protected virtual void DecrementActualRemainingSize(Size size) => ActualRemainingSize -= size;

                public bool Start()
                {
                    ThrowIfDisposed(this);

                    return ArePathsLoaded ? DoWorkSafe(_Start) : throw new InvalidOperationException("The paths have not been loaded.");
                }

                protected abstract void ResetStatus();

                public void Reset()
                {
                    ThrowIfDisposed(this);

                    ArePathsLoaded = false;

                    _paths.Clear();

                    _errorPaths.Clear();

                    ResetStatus();

                    InitialTotalSize = new Size();

                    InitialItemCount = 0;
                }
                #endregion Methods

                #region IDisposable Support
                public bool IsDisposed { get; private set; }

                protected virtual void DisposeManaged()
                {
                    IsDisposed = true;

                    _initialPaths.Clear();
                    _initialPaths = null;

                    _paths.Clear();
                    _paths = null;

                    _pathsReadOnlyQueue = null;

                    _errorPaths.Clear();
                    _errorPaths = null;

                    _errorPathsReadOnlyQueue = null;

                    _processDelegates = null;
                    _processEventDelegates = null;
                    _factory = null;

                    _propertyEventDelegate.Dispose();
                    _propertyEventDelegate = null;
                }

                public virtual void Dispose()
                {
                    DisposeManaged();

                    GC.SuppressFinalize(this);
                }
                #endregion IDisposable Support

#if !CS8
                #region IProcess Support
                IProcessErrorFactoryBase IProcess.Factory => Factory;

                IProcessEventDelegates IProcess.ProcessEventDelegates => ProcessEventDelegates;

                IPathCommon IProcess.SourcePath => SourcePath;

                ProcessTypes<IPathInfo>.IProcessQueue IProcess.Paths => new AbstractionProcessCollection<TItemsOut, IPathInfo>(Paths);

                IProcessError IProcess.Error => Error;

                IProcessErrorFactoryData IProcess.ProcessErrorFactoryData => ProcessErrorFactoryData;

                ProcessTypes<IProcessErrorItem>.IProcessQueue IProcess.ErrorPaths => new AbstractionProcessCollection<IProcessErrorItem<TItemsOut, TError>, IProcessErrorItem>(ErrorPaths);
                #endregion
#endif
            }

            public abstract class DestinationProcess : Process, ProcessInterfaceModelTypes<TItemsIn, TItemsOut, TError>.IDestinationProcess
            {
                public abstract TItemsIn DestinationPath { get; }

                public DestinationProcess(in IEnumerableQueue<TItemsIn> initialPaths, in ProcessTypes<TItemsOut>.IProcessQueue paths, in IProcessLinkedList<TItemsOut, TError> errorsQueue, in TProcessDelegates processDelegates, TFactory factory) : base(initialPaths, paths, errorsQueue, processDelegates, factory)
                {
                    // Left empty.
                }

#if !CS8
                #region IDestinationProcess Support
                IPathCommon ObjectModel.IDestinationProcess.DestinationPath => DestinationPath;
                #endregion
#endif
            }

            public abstract class DefaultDestinationProcess : DestinationProcess
            {
                public override TItemsIn SourcePath { get; }

                public override TItemsIn DestinationPath { get; }

                public DefaultDestinationProcess(in IEnumerableQueue<TItemsIn> initialPaths, in TItemsIn sourcePath, in TItemsIn destinationPath, in ProcessTypes<TItemsOut>.IProcessQueue paths, in IProcessLinkedList<TItemsOut, TError> errorsQueue, in TProcessDelegates processDelegates, TFactory factory) : base(initialPaths, paths, errorsQueue, processDelegates, factory)
                {
                    SourcePath = sourcePath;

                    DestinationPath = destinationPath;
                }

                public string GetDestinationPath(in IPathInfo path) => ProcessHelper.GetDestinationPath(DestinationPath, path);
            }
        }
    }
}
