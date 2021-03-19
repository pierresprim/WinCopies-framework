﻿/* Copyright © Pierre Sprimont, 2021
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

using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process.ObjectModel
{
    public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessEventDelegates, TProcessDelegateParam> where TItems : IPathInfo where TFactory : ProcessTypes<TItems>.ProcessErrorTypes<TError>.IProcessErrorFactories where TProcessDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.ProcessDelegatesAbstract<TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<TItems, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
    {
        public abstract partial class Process : ProcessInterfaceModelTypes<TItems, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>
        {
            public class ProcessErrorItem : ProcessTypes<TItems, TError>.ProcessErrorItem
            {
                public ProcessErrorItem(in TItems path, in IProcessError<TError> error) : base(path, error)
                {
                    // Left empty.
                }
            }

            #region Fields
            private bool _arePathsLoaded;
            private uint _initialItemCount;
            private IProcessError<TError> _error;
            private IQueueBase<TItems> _initialPaths;
            private Size _actualRemainingSize;
            private ProcessTypes<TItems>.IProcessCollection _paths;
            private ProcessTypes<TItems>.IProcessCollection _pathsReadOnlyQueue;
            private TProcessDelegates _processDelegates;
            private NullableGeneric<TProcessEventDelegates> _processEventDelegates;
            private IEnumerableInfoLinkedList _errorPaths;
            private ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessCollection _errorPathsReadOnlyQueue;
            private NullableGeneric<TFactory> _factory;
            private Size _initialTotalSize;
            private EventDelegate<string> _propertyEventDelegate = new
#if !CS9
                EventDelegate<string>
#endif
                ();
            #endregion Fields

            #region Properties
            public TFactory Factory => this.GetIfNotDisposed(_factory).Value;

            IProcessErrorFactory<TError> ProcessInterfaceModelTypes<TItems, TError>.IProcess<TProcessDelegateParam, TProcessEventDelegates>.Factory => Factory;

            protected TProcessDelegates ProcessDelegates => this.GetIfNotDisposed(_processDelegates);

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

            public ProcessTypes<TItems>.IProcessCollection Paths => this.GetIfNotDisposed(_pathsReadOnlyQueue);

            public IQueueBase<TItems> InitialPaths => this.GetIfNotDisposed(_initialPaths);

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

            public ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessCollection ErrorPaths => this.GetIfNotDisposed(_errorPathsReadOnlyQueue);

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

            public Process(in IQueueBase<TItems> initialPaths, QueueParameter paths, in LinkedListParameter errorsQueue, in TProcessDelegates processDelegates)
            {
                ThrowIfNull(initialPaths, nameof(initialPaths));

                if (!initialPaths.IsReadOnly)

                    throw new ArgumentException($"{nameof(initialPaths)} must be read-only.");

                ThrowIfNull(paths, nameof(paths));
                ThrowIfNull(errorsQueue, nameof(errorsQueue));

                _initialPaths = initialPaths;

                _paths = paths.ProvideValues(out _pathsReadOnlyQueue);

                _errorPaths = errorsQueue.ProvideValues(out _errorPathsReadOnlyQueue);

                _processDelegates = processDelegates;
                _processEventDelegates = new NullableGeneric<TProcessEventDelegates>(processDelegates.GetProcessEventDelegates());
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
                    Error = _factory.Value.GetError(_factory.Value.UnknownError, ex);
                }

                finally
                {
                    try
                    {
                        ResetStatus();
                    }

                    catch (Exception ex)
                    {
                        Error = _factory.Value.GetError(_factory.Value.WrongStatusError, ex);
                    }
                }

                return false;
            }

            private bool _LoadPaths()
            {
                bool result = LoadPathsOverride(out IProcessError<TError> error, out bool clearOnError);

                if (result && Equals(Error, _factory.Value.NoError))
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
                    _processDelegates.ProgressDelegate.RaiseEvent(getPathDelegate(paths.Peek()));

                    result = func(paths.Peek(), out error, out isErrorGlobal);

                    if (Equals(error.Error, _factory.Value.NoError))

                        _ = paths.Dequeue();

                    else if (isErrorGlobal)
                    {
                        Error = error;

                        return false;
                    }

                    else

                        _ = _errorPaths.AddLast(_func(paths.Dequeue(), error));

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

                if ((result &= _processDelegates.CheckPerformedDelegate.RaiseEvent(result)))

                    result = Equals(globalError.Error, _factory.Value.NoError);

                if (!result) // We do not perform an 'else if' because we want to check the latest result, but we do not have to check the equality of globalError.Error and NoError if the previous result was false; we exit the method if at least one result is false.

                    return false;

                if (_paths.HasItems)

                    return DoWork(new Queue(_paths), path => path, (TItems path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessErrorItem(_path, _error));

                else if (_errorPaths.Count != 0)

                    return DoWork(new LinkedList(_errorPaths), path => path.Path, (IProcessErrorItem<TItems, TError> path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessErrorItem(_path.Path, _error));

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
            IProcessErrorFactory IProcess.Factory => Factory;

            IProcessEventDelegates IProcess.ProcessEventDelegates => ProcessEventDelegates;

            IPathCommon IProcess.SourcePath => SourcePath;

            ProcessTypes<IPathInfo>.IProcessCollection IProcess.Paths => new AbstractionProcessCollection<TItems, IPathInfo>(Paths);

            IProcessError IProcess.Error => Error;

            IProcessErrorFactoryData IProcess.ProcessErrorFactoryData => ProcessErrorFactoryData;

            ProcessTypes<IProcessErrorItem>.IProcessCollection IProcess.ErrorPaths => new AbstractionProcessCollection<IProcessErrorItem<TItems, TError>, IProcessErrorItem>(ErrorPaths);
            #endregion
#endif

        }

        public abstract class DestinationProcess : Process, ProcessInterfaceModelTypes<TItems, TError>.IDestinationProcess
        {
            public abstract TItems DestinationPath { get; }

            public DestinationProcess(in IQueueBase<TItems> initialPaths, in QueueParameter paths, in LinkedListParameter errorsQueue, in TProcessDelegates processDelegates) : base(initialPaths, paths, errorsQueue, processDelegates)
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

            public DefaultDestinationProcess(in IQueueBase<TItems> initialPaths, in TItems sourcePath, in TItems destinationPath, in QueueParameter paths, in LinkedListParameter errorsQueue, in TProcessDelegates processDelegates) : base(initialPaths, paths, errorsQueue, processDelegates)
            {
                SourcePath = sourcePath;

                DestinationPath = destinationPath;
            }
        }
    }
}