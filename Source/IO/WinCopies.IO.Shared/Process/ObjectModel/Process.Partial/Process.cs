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

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO.Resources;
using WinCopies.Util;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProcessGuidAttribute : Attribute
    {
        public string Guid { get; }

        public ProcessGuidAttribute(string guid) => Guid = guid;
    }

    namespace ObjectModel
    {
        public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam> where TItems : IPathInfo where TFactory : ProcessTypes<TItems>.ProcessErrorTypes<TError>.IProcessErrorFactories where TProcessDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
        {
            public abstract partial class Process : ProcessInterfaceModelTypes<TItems, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>
            {
                #region Fields
                private bool _arePathsLoaded;
                private uint _initialItemCount;
                private IProcessError<TError> _error;
                private IEnumerableQueue<TItems> _initialPaths;
                private Size _actualRemainingSize;
                private ProcessTypes<TItems>.IProcessQueue _paths;
                private ProcessTypes<TItems>.IProcessQueue _pathsReadOnlyQueue;
                private NullableGeneric<TProcessDelegates> _processDelegates;
                private NullableGeneric<TProcessEventDelegates> _processEventDelegates;
                private IProcessLinkedList<TItems, TError> _errorPaths;
                private ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue _errorPathsReadOnlyQueue;
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

                IProcessErrorFactory<TError> ProcessInterfaceModelTypes<TItems, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>.Factory => Factory;

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

                public abstract TItems SourcePath { get; }

                public ProcessTypes<TItems>.IProcessQueue Paths => this.GetIfNotDisposed(_pathsReadOnlyQueue);

                public IEnumerableQueue<TItems> InitialPaths => this.GetIfNotDisposed(_initialPaths);

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

                public ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessQueue ErrorPaths => this.GetIfNotDisposed(_errorPathsReadOnlyQueue);

                public Size InitialTotalSize
                {
                    get => _initialTotalSize;

                    private set
                    {
                        _initialTotalSize = value;
                        ActualRemainingSize = value;

                        _propertyEventDelegate.RaiseEvent(nameof(InitialTotalSize));
                        _propertyEventDelegate.RaiseEvent(nameof(ActualRemainingSize));
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

                public Process(in IEnumerableQueue<TItems> initialPaths, in ProcessTypes<TItems>.IProcessQueue paths, in IProcessLinkedList<TItems, TError> errorsQueue, in TProcessDelegates processDelegates, in TFactory factory)
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

                protected void AddPath(in TItems path) => _paths.Enqueue(path);

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

                protected abstract bool DoWork(TItems path, out IProcessError<TError> error, out bool isErrorGlobal);

                protected abstract bool DoWork(IProcessErrorItem<TItems, TError> path, out IProcessError<TError> error, out bool isErrorGlobal);

                private delegate bool DoWorkFunc<T>(T path, out IProcessError<TError> error, out bool isErrorGlobal);

                private bool _DoWork<T>(in _IQueue<T> paths, in Func<T, TItems> getPathDelegate, in DoWorkFunc<T> func, in Func<T, IProcessError<TError>, IProcessErrorItem<TItems, TError>> _func) where T : IPathInfo
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

                private bool DoWork<T>(in _IQueue<T> paths, in Func<T, TItems> getPathDelegate, in DoWorkFunc<T> func, in Func<T, IProcessError<TError>, IProcessErrorItem<TItems, TError>> _func) where T : IPathInfo
                {
                    bool result = _DoWork(paths, getPathDelegate, func, _func);

                    paths.Dispose();

                    return result;
                }

                private bool _Start()
                {
                    bool result = Check(out IProcessError<TError> globalError);

                    Error = globalError;

                    if ((result &= _processDelegates.Value.CheckPerformedDelegate.RaiseEvent(result)))

                        result = Equals(globalError.Error, _factory.Value.NoError);

                    if (!result) // We do not perform an 'else if' because we want to check the latest result, but we do not have to check the equality of globalError.Error and NoError if the previous result was false; we exit the method if at least one result is false.

                        return false;

                    if (_paths.HasItems)

                        return DoWork(new Queue(_paths), path => path, (TItems path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessTypes<TItems, TError>.ProcessErrorItem(_path, _error));

                    else if (_errorPaths.Count != 0)

                        return DoWork(new LinkedList(_errorPaths), path => path.Item, (IProcessErrorItem<TItems, TError> path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessTypes<TItems, TError>.ProcessErrorItem(_path.Item, _error));

                    return true;
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

                ProcessTypes<IPathInfo>.IProcessQueue IProcess.Paths => new AbstractionProcessCollection<TItems, IPathInfo>(Paths);

                IProcessError IProcess.Error => Error;

                IProcessErrorFactoryData IProcess.ProcessErrorFactoryData => ProcessErrorFactoryData;

                ProcessTypes<IProcessErrorItem>.IProcessQueue IProcess.ErrorPaths => new AbstractionProcessCollection<IProcessErrorItem<TItems, TError>, IProcessErrorItem>(ErrorPaths);
                #endregion
#endif
            }

            public abstract class DestinationProcess : Process, ProcessInterfaceModelTypes<TItems, TError>.IDestinationProcess
            {
                public abstract TItems DestinationPath { get; }

                public DestinationProcess(in IEnumerableQueue<TItems> initialPaths, in ProcessTypes<TItems>.IProcessQueue paths, in IProcessLinkedList<TItems, TError> errorsQueue, in TProcessDelegates processDelegates, TFactory factory) : base(initialPaths, paths, errorsQueue, processDelegates, factory)
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
                public override TItems SourcePath { get; }

                public override TItems DestinationPath { get; }

                public DefaultDestinationProcess(in IEnumerableQueue<TItems> initialPaths, in TItems sourcePath, in TItems destinationPath, in ProcessTypes<TItems>.IProcessQueue paths, in IProcessLinkedList<TItems, TError> errorsQueue, in TProcessDelegates processDelegates, TFactory factory) : base(initialPaths, paths, errorsQueue, processDelegates, factory)
                {
                    SourcePath = sourcePath;

                    DestinationPath = destinationPath;
                }
            }
        }
    }
}
