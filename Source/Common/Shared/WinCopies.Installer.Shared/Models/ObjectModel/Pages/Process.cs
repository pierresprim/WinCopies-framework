using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;

using WinCopies.Linq;
using WinCopies.Util;

namespace WinCopies.Installer
{
    public interface IProcessData : IInstallerPageData,
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<KeyValuePair<string, File>>
    {
        IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources { get; }

        IInstallerStream GetWriter(string path);

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
            protected IEnumerable<KeyValuePair<string, File>> Files { get; }

            public abstract IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
            { get; }

            public ProcessData(in Installer installer) : base(installer) => Installer.Files = Files = GetFiles();

            protected abstract IEnumerable<KeyValuePair<string, File>> GetFiles();

            public abstract IInstallerStream GetWriter(string path);

            public abstract void DeleteOldFiles(Action<string> logger);

            public IEnumerator<KeyValuePair<string, File>> GetEnumerator() => Files.GetEnumerator();
#if !CS8
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif
        }

        protected abstract class DefaultProcessData : ProcessData
        {
            protected abstract string RelativePath { get; }

            protected abstract Predicate<KeyValuePair<string, System.IO.Stream>> Predicate { get; }

            protected abstract Type RelativePathResourcesType { get; }

            public override IEnumerable<KeyValuePair<string, string>> Resources => GetResources();

            protected DefaultProcessData(in Installer installer) : base(installer) { /* Left empty. */ }

            protected IEnumerable<KeyValuePair<string, string>> GetResources()
            {
                foreach (PropertyInfo property in RelativePathResourcesType.GetProperties(BindingFlags.Public | BindingFlags.Static))

                    if (property.GetValue(null) is string value)

                        yield return new KeyValuePair<string, string>(property.Name, value);
            }

            public override IInstallerStream GetWriter(string path) => new InstallerStream(path);

            protected IEnumerable<KeyValuePair<string, File>> GetFiles(Assembly assembly) => assembly.EnumerateEmbeddedResources().WhereSelect(item => item.Key.StartsWith(RelativePath), item => UtilHelpers.GetKeyValuePair(item.Key.Substring(RelativePath.Length), new File(Predicate(item), item.Value)));

            protected override IEnumerable<KeyValuePair<string, File>> GetFiles() => UtilHelpers.PerformActionIfNotNull(Assembly.GetEntryAssembly(), GetFiles);

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

        public override string Title => "Installing...";

        public override string Description => $"Please wait while {Installer.ProgramName} is being installed on your computer.";

        protected ProcessPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }
}
