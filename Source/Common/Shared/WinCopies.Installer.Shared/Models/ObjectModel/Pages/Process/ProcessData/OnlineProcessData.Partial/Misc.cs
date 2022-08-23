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
        IEnumerable<string>
    {
        IEnumerable<string> AsLocalFileEnumerable();
    }
#if CS8
    public partial class OnlineProcessData
    {
        protected struct ValidableFileEnumerable : IFileEnumerable
        {
            private readonly IEnumerable<KeyValuePair<string, IFile>> _files;

            public string? RelativeDirectory { get; }

            public string? OldRelativeDirectory { get; }

            public IEnumerable<KeyValuePair<string, string>>? Resources => null;

            public ValidableFileEnumerable(in string? temporaryDirectory, in string? oldTemporaryDirectory, in IEnumerable<KeyValuePair<string, IFile>> files)
            {
                RelativeDirectory = temporaryDirectory;
                OldRelativeDirectory = oldTemporaryDirectory;
                _files = files;
            }

            public System.IO.Stream GetWriter(string path) => DefaultProcessData.GetFileStream(path);

            public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => _files.GetEnumerator();
#if !CS8
            IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        protected struct TemporaryFileEnumerable : ITemporaryFileEnumerable
        {
            private Converter<System.IO.Stream, byte[]>? _getValidationData;
            private Converter<string, FuncOut<ulong?, System.IO.Stream?>?>? _remoteValidationStreamProvider;
            private Action<string> _deleteAction;
            private Action _disposeAction;

            public string ActionName { get; }

            public Converter<string, Func<System.IO.Stream>?>? Validator { get; }

            public IFileEnumerable Files { get; }

            //public IEnumerable<KeyValuePair<string, IValidableFile>> ValidationFiles { get; }

            public WinCopies.Installer.IRemoteFileEnumerable Paths { get; }

            public IEnumerable<string> PhysicalFiles { get; }

            public Copier Copier { get; private set; }

            public TemporaryFileEnumerable(in string actionName, in Converter<string, Func<System.IO.Stream>?>? validator, in IFileEnumerable files, /*in IEnumerable<KeyValuePair<string, IValidableFile>> validationFiles,*/ in WinCopies.Installer.IRemoteFileEnumerable paths, in IEnumerable<string> physicalFiles, in Copier copier, Converter<string, FuncOut<ulong?, System.IO.Stream?>?>? remoteValidationStreamProvider, /*in Converter<string, System.IO.Stream> getValidatorWriter,*/ Converter<System.IO.Stream, byte[]>? getValidationData, in Action<string> deleteAction, in Action disposeAction)
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

            public System.IO.Stream? GetRemoteValidationStream(string file, out ulong? length)
            {
                FuncOut<ulong?, System.IO.Stream?>? func = _remoteValidationStreamProvider?.Invoke(file);

                if (func == null)
                {
                    length = null;

                    return null;
                }

                return func(out length);
            }

            public byte[]? GetValidationData(System.IO.Stream stream) => _getValidationData?.Invoke(stream);

            //public System.IO.Stream GetValidationFileWriter(string relativePath) => _getValidatorWriter(relativePath);

            public void Delete(string relativePath) => _deleteAction(relativePath);

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

        private sealed class RemoteFileEnumerable : DisposableBase, IRemoteFileEnumerable, Collections.DotNetFix.Generic.IEnumerable<string>
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

            private IEnumerator<string> GetEnumerator(bool addExtension)
            {
                StreamReader reader;
                string? value = null;
                string relativeDirectoryURL = _data.RelativeDirectoryURL;
                string relativeFileURL = _data.RelativeFileURL;
                string url = _data.URL;

                string getCurrent(in string? directory) => $"{directory}/{value}";

                ConverterIn<string?, string> converter;

                ConverterIn<string?, string> getConverter()
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

                IEnumerable<string> doWork(string? directory)
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

                            yield return default;
                        }

                        foreach (string item in doWork($"{directory}/{value}"))

                            yield return item;
                    }

                    updateStream(relativeFileURL);

                    while (check())
                    {
                        if (checkIfValueEmpty())
                        {
                            onError("File");

                            yield return default;
                        }

                        yield return converter(directory);
                    }
                }

                foreach (string item in doWork(null))

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
