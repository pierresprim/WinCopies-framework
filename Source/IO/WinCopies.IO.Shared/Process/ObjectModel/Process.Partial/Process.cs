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

using System;
using System.Collections.Generic;
using WinCopies.Collections.DotNetFix.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process.ObjectModel
{
    public static partial class ProcessObjectModelTypes<TItems, TFactory, TError, TProcessDelegates, TProcessDelegateParam> where TItems : IPathInfo where TFactory : ProcessTypes<TItems>.ProcessErrorTypes<TError>.IProcessErrorFactories where TProcessDelegates : ProcessTypes<TItems>.ProcessDelegates<TProcessDelegateParam>
    {
        public abstract partial class Process : IProcess<TItems, TError>, DotNetFix.IDisposable
        {
            public class ProcessErrorItem : ProcessTypes<TItems>.ProcessErrorTypes<TError>.ProcessErrorItem
            {
                public ProcessErrorItem(in TItems path, in IProcessError<TError> error) : base(path, error)
                {
                    // Left empty.
                }
            }

            #region Fields
            private IQueueBase<TItems> _initialPaths;
            private IQueue<TItems> _paths;
            private ProcessTypes<TItems>.ProcessCollection _pathsReadOnlyQueue;
            private TProcessDelegates _processDelegates;
            private IEnumerableInfoLinkedList<IProcessErrorItem<TItems, TError>> _errorPaths;
            private IQueue<IProcessErrorItem<TItems, TError>> _errorPathsReadOnlyQueue;
            private NullableGeneric<TFactory> _factory;
            #endregion Fields

            #region Properties
            public abstract TError NoError { get; }

            public abstract TError UnknownError { get; }

            public abstract TError WrongStatus { get; }

            public TFactory Factory => this.GetIfNotDisposed(_factory).Value;

            protected TProcessDelegates ProcessDelegates => this.GetIfNotDisposed(_processDelegates);

            public bool ArePathsLoaded { get; private set; }

            public abstract TItems SourcePath { get; }

            public ProcessTypes<TItems>.ProcessCollection Paths => this.GetIfNotDisposed(_pathsReadOnlyQueue);

            IQueue<TItems> IProcess<TItems, TError>.Paths => Paths;

            public IQueueBase<TItems> InitialPaths => this.GetIfNotDisposed(_initialPaths);

            public abstract string Name { get; }

            public IProcessError<TError> Error { get; private set; }

            public IQueue<IProcessErrorItem<TItems, TError>> ErrorPaths => this.GetIfNotDisposed(_errorPathsReadOnlyQueue);

            public Size InitialTotalSize { get; private set; }

            public IPathInfo CurrentItem { get; private set; }

            public abstract IList<IProcessData> ProcessData { get; }
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
            }

            #region Methods
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
                    Error = _factory.Value.GetError(UnknownError, ex);
                }

                finally
                {
                    try
                    {
                        ResetStatus();
                    }

                    catch (Exception ex)
                    {
                        Error = _factory.Value.GetError(WrongStatus, ex);
                    }
                }

                return false;
            }

            private bool _LoadPaths()
            {
                bool result = LoadPathsOverride(out IProcessError<TError> error, out bool clearOnError);

                if (result && Equals(Error, NoError))
                {
                    ArePathsLoaded = true;

                    InitialTotalSize = Paths.TotalSize;
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

            protected abstract bool DoWork(out IProcessError<TError> error, out bool isErrorGlobal);

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
                    CurrentItem = paths.Peek();

                    _processDelegates.ProgressDelegate(getPathDelegate(paths.Peek()));

                    result = func(paths.Peek(), out error, out isErrorGlobal);

                    if (Equals(error.Error, NoError))

                        _ = paths.Dequeue();

                    else if (isErrorGlobal)
                    {
                        Error = error;

                        return false;
                    }

                    else

                        _ = _errorPaths.AddLast(_func(paths.Dequeue(), error));
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

                if ((result &= _processDelegates.CheckPerformedDelegate(result)))

                    result = Equals(globalError.Error, NoError);

                if (!result) // We do not perform an 'else if' because we want to check the latest result, but we do not have to check the equality of globalError.Error and NoError if the previous result was false; we exit the method if at least one result is false.

                    return false;

                if (_paths.HasItems)

                    return DoWork<TItems>(new Queue(_paths), path => path, (TItems path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(out error, out isErrorGlobal), (_path, _error) => new ProcessErrorItem(_path, _error));

                else if (_errorPaths.Count != 0)

                    return DoWork<IProcessErrorItem<TItems, TError>>(new LinkedList(_errorPaths), path => path.Path, (IProcessErrorItem<TItems, TError> path, out IProcessError<TError> error, out bool isErrorGlobal) => DoWork(path, out error, out isErrorGlobal), (_path, _error) => new ProcessErrorItem(_path.Path, _error));

                return true;
            }

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
                _factory = null;
            }

            public virtual void Dispose()
            {
                DisposeManaged();

                GC.SuppressFinalize(this);
            }
            #endregion IDisposable Support
        }

        public abstract class DestinationProcess : Process, IDestinationProcess<TItems, TError>
        {
            public abstract TItems DestinationPath { get; }

            public DestinationProcess(in IQueueBase<TItems> initialPaths, in QueueParameter paths, in LinkedListParameter errorsQueue, in TProcessDelegates processDelegates) : base(initialPaths, paths, errorsQueue, processDelegates)
            {
                // Left empty.
            }
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
