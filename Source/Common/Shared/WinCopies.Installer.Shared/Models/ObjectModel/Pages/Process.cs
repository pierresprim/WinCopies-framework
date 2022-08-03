using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.UtilHelpers;

#if CS8
using System.Net.Http;
#endif

namespace WinCopies.Installer
{
    public interface IFileEnumerableBase : IAsEnumerable<KeyValuePair<string, IFile>>
    {
        IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
        { get; }

        IInstallerStream GetWriter(string path);
    }

    public interface IFileEnumerable<T> : IFileEnumerableBase,
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<KeyValuePair<string, T>>
    {

    }

    public interface IFileEnumerable : IFileEnumerable<IFile>
    {
#if CS8
        IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this;
#endif
    }

    public interface IValidableFileEnumerable : IFileEnumerable<IValidableFile>
    {
#if CS8
        IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this.Select(item => GetKeyValuePair(item.Key, item.Value.AsFromType<IFile>()));
#endif
    }

    public interface ITemporaryFileEnumerable
    {
        string ActionName { get; }

        Converter<string, System.IO.Stream> Validator { get; }

        IValidableFileEnumerable Files { get; }
    }

    public interface IProcessData : IInstallerPageData, IFileEnumerable
    {
        Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack
#if CS8
            ?
#endif
            GetTemporaryFiles(Func<string, bool, bool> onError);

        void DeleteOldFiles(Action<string> action);
    }

    public interface IProcessDataViewModel : IProcessData, INotifyPropertyChanged, ICommand
    {
        byte OverallProgress { get; set; }

        byte CurrentItemProgress { get; set; }

        string Log { get; set; }

        void AddLogMessage(string message);

        void Start();
    }

    public interface IProcessPage : IBrowsableForwardCommonPage<IEndPage, IProcessData>
    {
        // Left empty.
    }

    public abstract class ProcessPage : CommonInstallerPageBase<IEndPage, IProcessData>, IProcessPage
    {
        protected abstract class ProcessData : InstallerPageData, IProcessData
        {
            protected IEnumerable<KeyValuePair<string, IFile>> Files { get; }

            public abstract IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
            { get; }

            public ProcessData(in Installer installer) : base(installer) => Installer.Files = Files = GetFiles();

            protected abstract IEnumerable<KeyValuePair<string, IFile>> GetFiles();

            public abstract Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack
#if CS8
                 ?
#endif
                 GetTemporaryFiles(Func<string, bool, bool> onError);

            public abstract IInstallerStream GetWriter(string path);

            public abstract void DeleteOldFiles(Action<string> logger);

            public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => Files.GetEnumerator();
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IEnumerable<KeyValuePair<string, IFile>> IAsEnumerable<KeyValuePair<string, IFile>>.AsEnumerable() => this.Select(item => GetKeyValuePair(item.Key, item.Value.AsFromType()));
#endif
        }

        protected abstract class DefaultProcessData : ProcessData
        {
            protected abstract Predicate<string> Predicate { get; }

            protected DefaultProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

            public override IInstallerStream GetWriter(string path) => new InstallerStream(path);

            public override void DeleteOldFiles(Action<string> logger)
            {
                string location = Installer.Location;

                if (Directory.Exists(location))
                {
                    logger($"'{location}' found. Deleting content.\nDeleting directories.");

                    void action(in string item, in Action<string> _action)
                    {
                        logger($"Deleting '{item}'...");

                        _action(item);

                        logger("Deleted.");
                    }

                    foreach (string directory in Directory.EnumerateDirectories(location))

                        action(directory, item => Directory.Delete(item, true));

                    logger("Deleted directories.\nDeleting files.");

                    foreach (string file in Directory.EnumerateFiles(location))

                        action(file, System.IO.File.Delete);

                    logger("Deleted files.");
                }

                else
                {
                    logger($"'{location}' not found. Creating directory.");

                    _ = Directory.CreateDirectory(location);

                    logger("Directory created.");
                }
            }
        }

        protected abstract class DefaultEmbeddedProcessData : DefaultProcessData
        {
            protected abstract string RelativePath { get; }

            protected abstract Type RelativePathResourcesType { get; }

            public override IEnumerable<KeyValuePair<string, string>> Resources => GetResources();

            protected DefaultEmbeddedProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

            public override Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack
#if CS8
                 ?
#endif
                 GetTemporaryFiles(Func<string, bool, bool> onError) => null;

            protected IEnumerable<KeyValuePair<string, string>> GetResources() => Linq.Enumerable.ForEachIfNotNull(RelativePathResourcesType, t => t.GetProperties(BindingFlags.Public | BindingFlags.Static), (PropertyInfo p, out KeyValuePair<string, string> result) =>
            {
                if (p.GetValue(null) is string value)
                {
                    result = new KeyValuePair<string, string>(p.Name, value);

                    return true;
                }

                result = default;

                return false;
            });

            protected IEnumerable<KeyValuePair<string, IFile>> GetFiles(Assembly assembly) => assembly.EnumerateEmbeddedResources().WhereSelect(item => item.Key.StartsWith(RelativePath), item => GetKeyValuePair(item.Key
#if CS8
                [
#else
                .Substring(
#endif
                RelativePath.Length
#if CS8
                ..]
#else
                )
#endif
                , new File(Predicate(item.Key), item.Value).AsFromType<IFile>()));

            protected override IEnumerable<KeyValuePair<string, IFile>> GetFiles() => PerformActionIfNotNull(Assembly.GetEntryAssembly(), GetFiles);
        }

        private struct TemporaryFileEnumerable : ITemporaryFileEnumerable
        {
            public string ActionName { get; }

            public Converter<string, System.IO.Stream> Validator { get; }

            public IValidableFileEnumerable Files { get; }

            public TemporaryFileEnumerable(in string actionName, in Converter<string, System.IO.Stream
#if CS8
                ?
#endif
                > validator, in IValidableFileEnumerable files)
            {
                ActionName = actionName;
                Validator = validator;
                Files = files;
            }
        }
#if CS8
        protected abstract class DefaultOnlineProcessData : DefaultProcessData
        {
            private string _temporaryDirectory;

            private struct ValidableFileEnumerable : IValidableFileEnumerable
            {
                private readonly DefaultOnlineProcessData _data;
                private readonly IEnumerable<KeyValuePair<string, IValidableFile>> _files;

                public IEnumerable<KeyValuePair<string, string>>? Resources => null;

                public ValidableFileEnumerable(in DefaultOnlineProcessData data, in IEnumerable<KeyValuePair<string, IValidableFile>> files)
                {
                    _data = data;
                    _files = files;
                }

                public IInstallerStream GetWriter(string path) => new InstallerStream(Path.Combine(_data._temporaryDirectory, path));
                public IEnumerator<KeyValuePair<string, IValidableFile>> GetEnumerator() => _files.GetEnumerator();
            }

            protected abstract string URL { get; }

            protected abstract string RelativeDirectoryURL { get; }
            protected abstract string RelativeFileURL { get; }

            protected DefaultOnlineProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

            public override Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack? GetTemporaryFiles(Func<string, bool, bool> onError)
            {
                var httpClient = new HttpClient();
                System.IO.Stream getStream(string directory, string file) => Task.Run(() => httpClient.GetStreamAsync($"{directory}/{file}")).GetAwaiter().GetResult();
                System.IO.Stream _getStream(in string directory, in string file, in string extension) => getStream(directory, $"{file}.{extension}");

                IValidableFileEnumerable download()
                {
                    IEnumerable<KeyValuePair<string, IValidableFile>> getFiles()
                    {
                        StreamReader reader;
                        string? value;
                        string relativeDirectoryURL = RelativeDirectoryURL;
                        string relativeFileURL = RelativeFileURL;

                        IEnumerable<KeyValuePair<string, IValidableFile>> doWork(string directory)
                        {
                            System.IO.Stream getStream(string file) => Task.Run(() => httpClient.GetStreamAsync($"{directory}/{file}")).GetAwaiter().GetResult();
                            System.IO.Stream _getStream(in string file, in string extension) => getStream($"{file}.{extension}");

                            void updateStream(in string file) => reader = new StreamReader(getStream(file));

                            bool check() => !reader.EndOfStream;

                            bool checkIfValueEmpty() => IsNullEmptyOrWhiteSpace(value = reader.ReadLine());

                            void _onError(in string msg) => onError($"{msg} index error.", false);

                            updateStream(relativeDirectoryURL);

                            while (check())
                            {
                                if (checkIfValueEmpty())
                                {
                                    _onError("Directory");

                                    yield return default;
                                }

                                foreach (KeyValuePair<string, IValidableFile> item in doWork($"{directory}/{value}"))

                                    yield return item;
                            }

                            updateStream(relativeFileURL);

                            while (check())
                            {
                                if (checkIfValueEmpty())
                                {
                                    _onError("File");

                                    yield return default;
                                }

                                yield return GetKeyValuePair(value = reader.ReadLine(), new ValidableFile(new File(false, _getStream(value, "zip")), _getStream(value, ".sha256")).AsFromType<IValidableFile>());
                            }
                        }

                        foreach (KeyValuePair<string, IValidableFile> item in doWork(URL))

                            yield return item;
                    }

                    return new ValidableFileEnumerable(this, getFiles());
                }

                Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableStack enumerables = Collections.Generic.EnumerableHelper<ITemporaryFileEnumerable>.GetEnumerableStack();

                _temporaryDirectory = Path.Combine(Environment.GetFolderPath(Installer.InstallForCurrentUserOnly ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData), "Temp");

                enumerables.Push(new TemporaryFileEnumerable("Download", relativePath => _getStream(URL, relativePath, ".sha256"), download()));

                return enumerables;
            }

            protected override IEnumerable<KeyValuePair<string, IFile>> GetFiles()
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

                foreach (string file in getFiles(Path.Combine(_temporaryDirectory, Installer.ProgramName)))

                    yield return GetKeyValuePair(file, new File(Predicate(file), new FileStream(file, FileMode.Create, FileAccess.Read)).AsFromType<IFile>());
            }
        }
#endif
        public override string Title => "Installing...";

        public override string Description => $"Please wait while {Installer.ProgramName} is being installed on your computer.";

        protected ProcessPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }
}
