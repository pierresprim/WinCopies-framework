/*using System;
using System.IO;

using ICustomStream = WinCopies.DotNetFix.IWriterStream;

namespace WinCopies.Installer
{
    public interface IInstallerStream : ICustomStream
    {
        Action GetDeleter();

        System.IO.Stream AsSystemStream();
    }

    public class InstallerStream : DotNetFix.FileStream, IInstallerStream
    {
        public InstallerStream(in string path) : base(GetStream(path)) { /* Left empty. */ /*}

        /*protected static Action GetAction(string path) => () => System.IO.File.Delete(path);

        protected static FileStream GetStream(in string path)
        {
            string
#if CS8
                ?
#endif
                directory = Path.GetDirectoryName(path);

            if (!(directory == null || Directory.Exists(directory)))

                Directory.CreateDirectory(directory);

            return new FileStream(path, FileMode.CreateNew);
        }

        public Action GetDeleter() => GetAction(Path);

        public System.IO.Stream AsSystemStream() => this;
    }
}*/
