using System;
using System.Collections.Generic;

namespace WinCopies.Installer
{
    public interface IFileValidationProvider
    {

        Func<System.IO.Stream>
#if CS8
            ?
#endif
            GetLocalValidationStream(string file);

        System.IO.Stream
#if CS8
           ?
#endif
           GetRemoteValidationStream(string file, out ulong? length);

        byte[]
#if CS8
            ?
#endif
            GetValidationData(System.IO.Stream stream);
    }

    public interface IFileEnumerable :
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<KeyValuePair<string, IFile>>
    {
        string
#if CS8
            ?
#endif
            RelativeDirectory
        { get; }

        string
#if CS8
            ?
#endif
            OldRelativeDirectory
        { get; }

        IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
        { get; }

        System.IO.Stream GetWriter(string path);
    }

    public delegate void Copier(System.IO.Stream reader, System.IO.Stream writer, Action<uint> progressReporter, out ulong? length);

    public interface ITemporaryFileEnumerable : IFileValidationProvider, System.IDisposable
    {
        string ActionName { get; }

        IFileEnumerable Files { get; }

        //IEnumerable<KeyValuePair<string, IValidableFile>> ValidationFiles { get; }

        IRemoteFileEnumerable Paths { get; }

        IEnumerable<string> PhysicalFiles { get; }

        Copier Copier { get; }

        /*System.IO.Stream
#if CS8
            ?
#endif
            GetValidationFileWriter(string relativePath);*/

        void Delete(string relativePath);
    }
}
