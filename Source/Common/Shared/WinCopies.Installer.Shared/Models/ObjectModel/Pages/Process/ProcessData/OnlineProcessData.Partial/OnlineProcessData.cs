#if CS8
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using WinCopies.Util;

using static WinCopies.UtilHelpers;

namespace WinCopies.Installer
{
    public abstract partial class OnlineProcessData : DefaultProcessData
    {
        private const string FILES = "zip";

        private string _absoluteTemporaryDirectory;

        //public override string? ValidationDirectory => SHA;

        protected abstract string? RelativeTemporaryDirectory { get; }

        protected abstract string URL { get; }

        protected abstract string RelativeDirectoryURL { get; }
        protected abstract string RelativeFileURL { get; }

        protected abstract string
#if CS8
            ?
#endif
            Extension
        { get; }

        protected string TemporaryDirectory => Installer.TemporaryDirectory;

        protected string AbsoluteTemporaryDirectory => _absoluteTemporaryDirectory ??= Path.Combine(TemporaryDirectory, RelativeTemporaryDirectory);

        protected OnlineProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

        private static System.IO.Stream GetStream(Task<System.IO.Stream> task) => Task.Run(() => task).GetAwaiter().GetResult();
        private static Task<System.IO.Stream> GetStream(in string file, in HttpClient httpClient) => httpClient.GetStreamAsync(file);

        protected virtual IRemoteFileEnumerable GetRemoteFiles(Func<string, bool, bool> onError) => new RemoteFileEnumerable(this, onError);

        protected virtual IEnumerable<string> GetPhysicalFiles(string directory)
        {
            static IEnumerable<string> getDirectories(string directory)
            {
                yield return directory;

                foreach (string _directory in Directory.EnumerateDirectories(directory))
                {
                    foreach (string __directory in getDirectories(_directory))

                        yield return __directory;
                }
            }

            if (!Directory.Exists(directory))

                _ = Directory.CreateDirectory(directory);

            foreach (string _directory in getDirectories(directory))

                foreach (string file in Directory.EnumerateFiles(_directory))

                    yield return file;
        }

        protected virtual string GetFullPath(string relativePath) => relativePath.StartsWith('\\') ? TemporaryDirectory + relativePath : Path.Combine(TemporaryDirectory, relativePath);

        protected virtual IEnumerable<string> GetPhysicalFilesFromRelativeDirectory(string directory) => GetPhysicalFiles(GetFullPath(directory));

        protected virtual Func<FileStream>? GetLocalValidator(string relativePath) => System.IO.File.Exists(relativePath = GetFullPath(relativePath)) ? () => new FileStream(relativePath, FileMode.Open, FileAccess.Read) :
#if !CS9
            (Func<FileStream>
#if CS8
            ?
#endif
            )
#endif
            null;

        //protected virtual System.IO.Stream GetValidatorWriter(string relativePath) => new FileStream(GetLocalValidatorPath(relativePath), FileMode.Create, FileAccess.Write);

        protected virtual void DeleteIfExists(string relativePath)
        {
            if (System.IO.File.Exists(relativePath = GetFullPath(relativePath)))

                System.IO.File.Delete(relativePath);
        }

        private static System.IO.Stream GetStream(in string url, in string file, in Converter<string, Task<System.IO.Stream>> converter, out string fullURL) => GetStream(converter(fullURL = url + file));

        protected virtual System.IO.Stream GetRemoteValidator(string file, HttpClient httpClient, out string url) => GetStream(URL, file + ".sha256", file => GetStream(file, httpClient), out url);

        protected virtual FuncOut<ulong?, System.IO.Stream> GetRemoteValidatorData(string file, bool size, HttpClient httpClient)
        {
            System.IO.Stream result = GetRemoteValidator(file, httpClient, out file);

            return size ? (out ulong? length) =>
        {
            long? _length = GetHttpFileSize(IO.Path.RemoveExtension(file), httpClient);

            length = _length.HasValue ? (ulong)_length.Value :
#if !CS9
                (ulong?)
#endif
                null;

            return result;
        }
            :
#if !CS9
            (FuncOut<ulong?, System.IO.Stream>)(
#endif
            (out ulong? length) =>
        {
            length = null;

            return result;
        }
#if !CS9
        )
#endif
        ;
        }

        protected virtual /*Converter<bool,*/ ValidableFileEnumerable/*>*/ GetRemoteFileEnumerable(Func<string, bool, bool> onError, out IRemoteFileEnumerable files)
        {
            IRemoteFileEnumerable _files = GetRemoteFiles(onError);

            files = _files;

            return /*addExtension =>*/ new ValidableFileEnumerable(FILES, null, /*(addExtension ?*/ _files /*: _files.AsLocalFileEnumerable())*/.Select(file => GetKeyValuePair(file, new DefaultFile(file, GetStream(URL, file, _file => _files.HttpClient.GetStreamWithLengthAsync(_file), out _)).AsFromType<IFile>())));
        }

        protected virtual Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableQueue GetTemporaryFileQueue(Func<string, bool, bool> onError, out HttpClient httpClient/*, out Converter<bool, ValidableFileEnumerable> remoteFilesProvider*/)
        {
            Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableQueue enumerables = Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.GetEnumerableQueue();

            ValidableFileEnumerable _files = /*(remoteFilesProvider =*/ GetRemoteFileEnumerable(onError, out IRemoteFileEnumerable files)/*)(true)*/;

            HttpClient _httpClient = files.HttpClient;

            enumerables.Enqueue(new TemporaryFileEnumerable("Download", file => GetLocalValidator(FILES + file), _files, /*_files,*/ files, GetPhysicalFiles(Path.Combine(TemporaryDirectory, FILES)), Copy, /*file => GetValidatorWriter(FILES + file),*/  file => GetRemoteValidatorData(file, true, _httpClient), GetValidationData, file => DeleteIfExists(FILES + file), files.Dispose));

            httpClient = _httpClient;

            return enumerables;
        }

        public override Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable> GetTemporaryFiles(Func<string, bool, bool> onError) => GetTemporaryFileQueue(onError, out _/*, out _*/);

        protected virtual IEnumerable<string> GetPhysicalFiles()
        {
#if CS8
            static
#endif
            IEnumerable<string> getFiles(string directory)
            {
                foreach (string file in Directory.EnumerateFiles(directory))

                    yield return file;

                foreach (string _directory in Directory.EnumerateDirectories(directory))

                    foreach (string file in getFiles(_directory))

                        yield return file;
            }

            foreach (string file in getFiles(AbsoluteTemporaryDirectory))

                yield return file;
        }

        protected override IEnumerable<KeyValuePair<string, IFile>> GetFiles() => GetPhysicalFiles().Select(file => GetKeyValuePair(file[AbsoluteTemporaryDirectory.Length..], new File(Predicate(file), file, new FileStream(file, FileMode.Open, FileAccess.Read)).AsFromType<IFile>()));
    }
}
#endif
