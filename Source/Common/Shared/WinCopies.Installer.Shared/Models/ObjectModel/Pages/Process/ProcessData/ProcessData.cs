using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using WinCopies.Linq;
using WinCopies.Util;

using static WinCopies.UtilHelpers;

namespace WinCopies.Installer
{
    public interface IProcessData : IInstallerPageData, IFileEnumerable
    {
        /*string
#if CS8
            ?
#endif
            ValidationDirectory
        { get; }*/

        Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable>
#if CS8
            ?
#endif
            GetTemporaryFiles(Func<string, bool, bool> onError);

        byte[] GetValidationData(System.IO.Stream stream);

        void DeleteOldFiles(Action<string> action);
    }

    public abstract class ProcessData : InstallerPageData, IProcessData
    {
        protected IEnumerable<KeyValuePair<string, IFile>> Files { get; }

        public abstract IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
        { get; }

        /*public abstract string
#if CS8
            ?
#endif
            ValidationDirectory
        { get; }*/

        public abstract string
#if CS8
            ?
#endif
            RelativeDirectory
        { get; }

        public abstract string
#if CS8
            ?
#endif
            OldRelativeDirectory
        { get; }

        public ProcessData(in Installer installer) : base(installer) => Installer.Files = Files = GetFiles();

        public abstract byte[] GetValidationData(System.IO.Stream stream);

        public static void Copy(System.IO.Stream reader, System.IO.Stream writer, Action<uint> progressReporter, out ulong? length)
        {
            const ushort MAX_LENGTH = 4096;
            var buffer = new byte[MAX_LENGTH];
            int count = 0;

            void write() => writer.Write(buffer, 0, count);

            length = reader.Length > 0 ? (ulong)reader.Length :
#if !CS9
                (ulong?)
#endif
                null;

            Action action = length.HasValue ? () =>
            {
                write();

                progressReporter((uint)count);
            }
            :
#if !CS9
                (Action)
#endif
            write;

            while ((count = reader.Read(buffer, 0, 4096)) > 0)

                action();
        }

        protected abstract IEnumerable<KeyValuePair<string, IFile>> GetFiles();

        public abstract Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable>
#if CS8
             ?
#endif
             GetTemporaryFiles(Func<string, bool, bool> onError);

        public abstract System.IO.Stream GetWriter(string path);

        public abstract void DeleteOldFiles(Action<string> logger);

        /*public abstract Func<System.IO.Stream>
#if CS8
                ?
#endif
                GetLocalValidationStream(string file);
        public abstract System.IO.Stream
#if CS8
                ?
#endif
                GetRemoteValidationStream(string file, out ulong? length);
        public abstract byte[]
#if CS8
                ?
#endif
                GetValidationData(System.IO.Stream stream);*/

        public IEnumerator<KeyValuePair<string, IFile>> GetEnumerator() => Files.GetEnumerator();
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
    }

    public abstract class DefaultProcessData : ProcessData
    {
        protected abstract Predicate<string> Predicate { get; }

        protected DefaultProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

        public static System.IO.Stream GetFileStream(string path) => new FileStream(path, FileMode.CreateNew);

        public override System.IO.Stream GetWriter(string path) => GetFileStream(path);

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

    public abstract class DefaultEmbeddedProcessData : DefaultProcessData
    {
        protected abstract string RelativePath { get; }

        protected abstract Type RelativePathResourcesType { get; }

        public override IEnumerable<KeyValuePair<string, string>> Resources => GetResources();

        protected DefaultEmbeddedProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

        public override Collections.DotNetFix.Generic.IPeekableEnumerable<ITemporaryFileEnumerable>
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
            , new File(Predicate(item.Key), item.Key, item.Value).AsFromType<IFile>()));

        protected override IEnumerable<KeyValuePair<string, IFile>> GetFiles() => PerformActionIfNotNull(Assembly.GetEntryAssembly(), GetFiles);
    }
}
