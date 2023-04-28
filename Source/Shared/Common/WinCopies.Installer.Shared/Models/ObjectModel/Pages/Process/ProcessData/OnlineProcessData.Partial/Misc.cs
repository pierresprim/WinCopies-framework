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

using System.Collections.Generic;

#if CS8
using System;
using System.IO;
using System.Net.Http;

using static WinCopies.UtilHelpers;
#endif

namespace WinCopies.Installer
{
    public interface IRemoteFileEnumerable :
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<string
#if CS8
            ?
#endif
            >
    {
        IEnumerable<string
#if CS8
            ?
#endif
            > AsLocalFileEnumerable();
    }
#if CS8
    public partial class OnlineProcessData
    {
        protected readonly struct ValidableFileEnumerable : IFileEnumerable
        {
            private readonly IEnumerable<KeyValuePair<string, IFile>> _files;

            public string
#if CS8
                ?
#endif
                RelativeDirectory
            { get; }

            public string
#if CS8
                ?
#endif
                OldRelativeDirectory
            { get; }

            public IEnumerable<KeyValuePair<string, string>>
#if CS8
                ?
#endif
                Resources => null;

            public ValidableFileEnumerable(in string? temporaryDirectory, in string
#if CS8
                ?
#endif
                oldTemporaryDirectory, in IEnumerable<KeyValuePair<string, IFile>> files)
            {
                RelativeDirectory = temporaryDirectory;
                OldRelativeDirectory = oldTemporaryDirectory;
                _files = files;
            }

            public System.IO.Stream GetWriter(string path) => GetFileStream(path);

            public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => _files.GetEnumerator();
#if !CS8
            IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        protected struct TemporaryFileEnumerable : ITemporaryFileEnumerable
        {
            private Converter<System.IO.Stream, byte[]>
#if CS8
                ?
#endif
                _getValidationData;
            private Converter<string, FuncOut<ulong?, System.IO.Stream
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                _remoteValidationStreamProvider;
            private Action<string> _deleteAction;
            private Action _disposeAction;

            public string ActionName { get; }

            public Converter<string, Func<System.IO.Stream>
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                Validator
            { get; }

            public IFileEnumerable Files { get; }

            //public IEnumerable<KeyValuePair<string, IValidableFile>> ValidationFiles { get; }

            public WinCopies.Installer.IRemoteFileEnumerable Paths { get; }

            public IEnumerable<string> PhysicalFiles { get; }

            public Copier Copier { get; private set; }

            public TemporaryFileEnumerable(in string actionName, in Converter<string, Func<System.IO.Stream>
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                validator, in IFileEnumerable files, /*in IEnumerable<KeyValuePair<string, IValidableFile>> validationFiles,*/ in WinCopies.Installer.IRemoteFileEnumerable paths, in IEnumerable<string> physicalFiles, in Copier copier, Converter<string, FuncOut<ulong?, System.IO.Stream
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                >
#if CS8
                ?
#endif
                remoteValidationStreamProvider, /*in Converter<string, System.IO.Stream> getValidatorWriter,*/ Converter<System.IO.Stream, byte[]>
#if CS8
                ?
#endif
                getValidationData, in Action<string> deleteAction, in Action disposeAction)
            {
                ActionName = actionName;
                Validator = validator;
                Files = files;
                //ValidationFiles = validationFiles;
                Paths = paths;
                PhysicalFiles = physicalFiles;
                Copier = copier;
                _getValidationData = getValidationData;
                _remoteValidationStreamProvider = remoteValidationStreamProvider;
                _deleteAction = deleteAction;
                _disposeAction = disposeAction;
            }

            public readonly Func<System.IO.Stream>
#if CS8
                ?
#endif
                GetLocalValidationStream(string file) => Validator?.Invoke(file);

            public readonly System.IO.Stream
#if CS8
                ?
#endif
                GetRemoteValidationStream(string file, out ulong? length)
            {
                FuncOut<ulong?, System.IO.Stream
#if CS8
                    ?
#endif
                    >
#if CS8
                    ?
#endif
                func = _remoteValidationStreamProvider?.Invoke(file);

                if (func == null)
                {
                    length = null;

                    return null;
                }

                return func(out length);
            }

            public readonly byte[]
#if CS8
                ?
#endif
                GetValidationData(System.IO.Stream stream) => _getValidationData?.Invoke(stream);

            //public System.IO.Stream GetValidationFileWriter(string relativePath) => _getValidatorWriter(relativePath);

            public readonly void Delete(string relativePath) => _deleteAction(relativePath);

            public void Dispose()
            {
                _disposeAction();

                Copier = null;
                _getValidationData = null;
                _remoteValidationStreamProvider = null;
                _deleteAction = null;
                _disposeAction = null;
            }
        }

        protected interface IRemoteFileEnumerable : WinCopies.Installer.IRemoteFileEnumerable, DotNetFix.IDisposable
        {
            HttpClient HttpClient { get; }
        }

        private sealed class RemoteFileEnumerable : DisposableBase, IRemoteFileEnumerable
#if CS8
            , Collections.DotNetFix.Generic.IEnumerable<string>
#endif
        {
            private OnlineProcessData _data;
            private Func<string, bool, bool> _onError;

            public HttpClient HttpClient { get; private set; }

            public override bool IsDisposed => HttpClient == null;

            internal RemoteFileEnumerable(in OnlineProcessData data, in Func<string, bool, bool> onError)
            {
                HttpClient = new HttpClient();
                _data = data;
                _onError = onError;
            }

            private IEnumerator<string
#if CS8
                ?
#endif
                > GetEnumerator(bool addExtension)
            {
                StreamReader reader;
                string
#if CS8
                    ?
#endif
                    value = null;
                string relativeDirectoryURL = _data.RelativeDirectoryURL;
                string relativeFileURL = _data.RelativeFileURL;
                string url = _data.URL;

                string getCurrent(in string
#if CS8
                    ?
#endif
                    directory) => $"{directory}/{value}";

                ConverterIn<string
#if CS8
                    ?
#endif
                    , string> converter;

                ConverterIn<string
#if CS8
                    ?
#endif
                    , string> getConverter()
                {
                    if (addExtension)
                    {
                        string? extension = _data.Extension;

                        if (!IsNullEmptyOrWhiteSpace(extension))

                            return (in string? directory) => $"{getCurrent(directory)}.{extension}";
                    }

                    return getCurrent;
                }

                converter = getConverter();

                IEnumerable<string
#if CS8
                    ?
#endif
                    > doWork(string
#if CS8
                    ?
#endif
                    directory)
                {
                    void updateStream(in string file) => reader = new StreamReader(GetStream(GetStream($"{url}{directory}/{file}", HttpClient)));

                    bool check() => !reader.EndOfStream;

                    bool checkIfValueEmpty() => IsNullEmptyOrWhiteSpace(value = reader.ReadLine());

                    void onError(in string msg) => _onError($"{msg} index error.", false);

                    updateStream(relativeDirectoryURL);

                    while (check())
                    {
                        if (checkIfValueEmpty())
                        {
                            onError("Directory");

                            yield return null;
                        }

                        foreach (string
#if CS8
                            ?
#endif
                            item in doWork($"{directory}/{value}"))

                            yield return item;
                    }

                    updateStream(relativeFileURL);

                    while (check())
                    {
                        if (checkIfValueEmpty())
                        {
                            onError("File");

                            yield return null;
                        }

                        yield return converter(directory);
                    }
                }

                foreach (string
#if CS8
                    ?
#endif
                    item in doWork(null))

                    yield return item;
            }

            public IEnumerator<string> GetEnumerator() => GetEnumerator(true);

            public IEnumerable<string> AsLocalFileEnumerable() => Collections.Enumerable.FromEnumeratorFunc(() => GetEnumerator(false));

            protected override void DisposeUnmanaged() => HttpClient.Dispose();

            protected override void DisposeManaged()
            {
                HttpClient = null;
                _data = null;
                _onError = null;
            }
        }
    }
#endif
}
