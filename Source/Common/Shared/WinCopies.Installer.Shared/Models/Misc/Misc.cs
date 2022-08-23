using System;

namespace WinCopies.Installer
{
    public interface IFile
    {
        bool IsMainApp { get; }

        string Name { get; }

        System.IO.Stream Stream { get; }
    }

    public struct File : IFile
    {
        public bool IsMainApp { get; }

        public string Name { get; }

        public System.IO.Stream Stream { get; }

        public File(in bool isMainApp, in string name, in System.IO.Stream stream)
        {
            IsMainApp = isMainApp;
            Name = name;
            Stream = stream;
        }
    }

    public struct DefaultFile : IFile
    {
        public bool IsMainApp => false;

        public string Name { get; }

        public System.IO.Stream Stream { get; }

        public DefaultFile(in string name, in System.IO.Stream stream)
        {
            Name = name;

            Stream = stream;
        }
    }

    /*public struct ValidableFile : IValidableFile
    {
        private readonly /*FuncOut<ulong?, */ /*Func<System.IO.Stream
#if CS8
              ?
#endif
              >
#if CS8
              ?
#endif
              _remoteValidationStreamProvider;

        public IFile File { get; }

        bool IFile.IsMainApp => File.IsMainApp;
        string IFile.Name => File.Name;
        System.IO.Stream IFile.Stream => File.Stream;

        public ValidableFile(in IFile file, in /*FuncOut<ulong?, */ /*Func<System.IO.Stream
#if CS8
            ?
#endif
            >
#if CS8
            ?
#endif
            remoteValidationStreamProvider)
        {
            File = file;
            _remoteValidationStreamProvider = remoteValidationStreamProvider;
        }
    }*/

    public enum Error : byte
    {
        Succeeded = 0,

        RecoveredError = 1,

        NotRecoveredError,

        FatalError,

        SuperFatalError,
    }
}
