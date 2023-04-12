namespace WinCopies.Installer
{
    public struct File
    {
        public bool IsMainApp { get; }

        public System.IO.Stream Stream { get; }

        public File(in bool isMainApp, in System.IO.Stream stream)
        {
            IsMainApp = isMainApp;

            Stream = stream;
        }
    }

    public enum Error : byte
    {
        Succeeded = 0,

        RecoveredError = 1,

        NotRecoveredError,

        FatalError,

        SuperFatalError,
    }
}
