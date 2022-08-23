using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

using WinCopies.Util;

using static WinCopies.UtilHelpers;

namespace WinCopies.Installer.Decompressor
{
    public abstract class ProcessData : OnlineProcessData
    {
        private struct RemoteFileEnumerable : WinCopies.Installer.IRemoteFileEnumerable
        {
            private string Select(string file) => IO.Path.RemoveExtension(file[ZIP.Length..]);

            public readonly IEnumerable<string> Files;

            public RemoteFileEnumerable(in IEnumerable<string> files) => Files = files;

            public IEnumerable<string> AsLocalFileEnumerable() => null;
            public IEnumerator<string> GetEnumerator() => Files.Select(Select).GetEnumerator();
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        private const string ZIP = "\\zip";

        protected abstract SevenZip.InArchiveFormat InArchiveFormat { get; }

        public ProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

        protected virtual IEnumerable<string> GetArchives() => GetPhysicalFiles(Path.Combine(TemporaryDirectory, "zip"));

        protected override Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableQueue GetTemporaryFileQueue(Func<string, bool, bool> onError, out HttpClient httpClient/*, out Converter<bool, ValidableFileEnumerable> remoteFilesProvider*/)
        {
            Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableQueue enumerables = base.GetTemporaryFileQueue(onError, out HttpClient _httpClient/*, out remoteFilesProvider*/);

            RemoteFileEnumerable remoteFileEnumerable = new RemoteFileEnumerable(GetArchives().Select(file => file[TemporaryDirectory.Length..]));
            const string FILES = "pkg";

            static string getFile(in string file) => FILES + IO.Path.RemoveExtension(file);

            enumerables.Enqueue(new TemporaryFileEnumerable("Decompression", file => GetLocalValidator(getFile(file)), new ValidableFileEnumerable(FILES, ZIP, remoteFileEnumerable.Files.Select(file => GetKeyValuePair(file.Remove(file.LastIndexOf('.')), new DefaultFile(file = TemporaryDirectory + file, new FileStream(file, FileMode.Open, FileAccess.Read)).AsFromType<IFile>()))), /*remoteFilesProvider(false),*/ remoteFileEnumerable, GetPhysicalFilesFromRelativeDirectory(FILES), (System.IO.Stream reader, System.IO.Stream writer, Action<uint> progressReporter, out ulong? length) =>
            {
                length = (ulong)reader.Length;

                var extractor = new SevenZip.SevenZipExtractor(reader, InArchiveFormat);

                extractor.Extracting += (object? sender, SevenZip.ProgressEventArgs e) => progressReporter((uint)(e.PercentDelta / 100 * reader.Length));

                extractor.ExtractFile(0, writer);

                extractor.Dispose();
            },
            /*file => GetValidatorWriter(FILES + file),*/ file => GetRemoteValidatorData(IO.Path.RemoveExtension(file), false, _httpClient), GetValidationData, file => DeleteIfExists(getFile(file)), Delegates.EmptyVoid));

            httpClient = _httpClient;

            return enumerables;
        }
    }
}
