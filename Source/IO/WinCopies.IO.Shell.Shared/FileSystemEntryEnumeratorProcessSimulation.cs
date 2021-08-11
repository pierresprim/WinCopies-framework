using System;

#if DEBUG
namespace WinCopies.IO.Enumeration
{
    public class FileSystemEntryEnumeratorProcessSimulation
    {
        private Func<string, PathType, System.Collections.Generic.IEnumerable<string>> _enumerateFunc;
        private Action<string> _writeLogAction;

        public Func<string, PathType, System.Collections.Generic.IEnumerable<string>> EnumerateFunc { get => _enumerateFunc ?? throw GetInvalidOperationException(); set => _enumerateFunc = value ?? throw GetInvalidOperationException(); }

        public Action<string> WriteLogAction { get => _writeLogAction ?? throw GetInvalidOperationException(); set => _writeLogAction = value ?? throw GetInvalidOperationException(); }

        private InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("Value cannot be null.");
    }
}
#endif
