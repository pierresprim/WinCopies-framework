/* Copyright © Pierre Sprimont, 2022
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

using SevenZip;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

        protected abstract InArchiveFormat InArchiveFormat { get; }

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

                var extractor = new SevenZipExtractor(reader, InArchiveFormat);

                extractor.Extracting += (object? sender, ProgressEventArgs e) => progressReporter((uint)(e.PercentDelta / 100 * reader.Length));

                extractor.ExtractFile(0, writer);

                extractor.Dispose();
            },
            /*file => GetValidatorWriter(FILES + file),*/ file => GetRemoteValidatorData(IO.Path.RemoveExtension(file), false, _httpClient), GetValidationData, file => DeleteIfExists(getFile(file)), Delegates.EmptyVoid));

            httpClient = _httpClient;

            return enumerables;
        }
    }

    public abstract class DefaultProcessData : ProcessData
    {
        private string? _url;

        public DefaultProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

        public override IEnumerable<KeyValuePair<string, string>>? Resources => null;

        protected abstract string RootURL { get; }

        protected override string URL
        {
            get
            {
                if (_url == null)
                {
                    string rootURL = RootURL;

                    _url = rootURL + new StreamReader(new HttpClient().GetStreamAsync(rootURL + "latest").GetAwaiter().GetResult()).ReadLine();
                }

                return _url;
            }
        }

        protected override string RelativeDirectoryURL => "directories";
        protected override string RelativeFileURL => "files";

        public override string? RelativeDirectory => null;
        public override string? OldRelativeDirectory => null;

        protected abstract System.Security.Cryptography.HashAlgorithmName HashAlgorithmName { get; }

        protected override Predicate<string> Predicate => item => item.EndsWith($"{nameof(Installer.ProgramName)}.exe");

        /*public override Func<System.IO.Stream>? GetLocalValidationStream(string file) => GetLocalValidator(IO.Path.RemoveExtension(file));
        public override System.IO.Stream? GetRemoteValidationStream(string file, out ulong? length) => GetRemoteValidatorData(file, true, )(out length);*/

        public override byte[]? GetValidationData(System.IO.Stream stream)
        {
            string? name = HashAlgorithmName.Name;

            if (name == null)

                return null;

            System.Security.Cryptography.HashAlgorithm? hashAlgorithm = System.Security.Cryptography.HashAlgorithm.Create(name);

            return hashAlgorithm?.ComputeHash(stream);
        }

        protected override string? RelativeTemporaryDirectory => "pkg";
    }
}
