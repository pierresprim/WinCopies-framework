namespace WinCopies.Installer
{
    public interface IFile
    {
        bool IsMainApp { get; }

        System.IO.Stream Stream { get; }
    }

    public interface IValidableFile : IFile
    {
        System.IO.Stream SHA256 { get; }
    }

    public struct File : IFile
    {
        public bool IsMainApp { get; }

        public System.IO.Stream Stream { get; }

        public File(in bool isMainApp, in System.IO.Stream stream)
        {
            IsMainApp = isMainApp;

            Stream = stream;
        }
    }

    public struct ValidableFile : IValidableFile
    {
        public File File { get; }

        public System.IO.Stream SHA256 { get; }

        bool IFile.IsMainApp => File.IsMainApp;
        System.IO.Stream IFile.Stream => File.Stream;

        public ValidableFile(in File file, in System.IO.Stream sha256)
        {
            File = file;

            SHA256 = sha256;
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
