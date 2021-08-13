using System;

using WinCopies.IO.Process.ObjectModel;

namespace WinCopies.IO.Process
{
    public class ProcessOptionsBase<T>
    {
        public Predicate<T> PathLoadedDelegate { get; }

        public bool ClearOnError { get; }

        public ProcessOptionsBase(in Predicate<T> pathLoadedDelegate, in bool clearOnError)
        {
            PathLoadedDelegate = pathLoadedDelegate;

            ClearOnError = clearOnError;
        }
    }

    public class ProcessOptionsCommon<T> : ProcessOptionsBase<T>
    {
        private IProcess _process;

        public IProcess Process { get => _process; internal set => _process = _process == null ? value : throw new InvalidOperationException("Another process is already registered."); }

        public ProcessOptionsCommon(in Predicate<T> pathsLoadedDelegate, in bool clearOnError) : base(pathsLoadedDelegate, clearOnError)
        {
            // Left empty.
        }

        protected virtual bool UpdateValue<TValue>(ref TValue value, in TValue newValue) => Process?.Status == ProcessStatus.Running
                ? throw new InvalidOperationException("The associated process is running.")
                : Temp.Temp.UpdateValue(ref value, newValue);

        protected void Dispose() => Process = default;
    }
}
