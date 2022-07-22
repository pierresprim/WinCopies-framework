#region Usings
#region System
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
#endregion System

using WinCopies.Desktop;
using WinCopies.DotNetFix;
using WinCopies.Installer;
using WinCopies.Installer.GUI;

using static WinCopies.Sample.Installer.Installer;
using static WinCopies.Sample.Installer.Properties.Resources;

using InstallerPageData = WinCopies.Installer.InstallerPageData;
using LocalResources = WinCopies.Sample.Installer.Properties.Resources;
#endregion Usings

namespace WinCopies.Sample.Installer
{
    public class Installer : WinCopies.Installer.Installer
    {
        public sealed override string ProgramName => "WinCopies Sample Installer";

        public override bool Is32Bit => false;

        public override bool RequiresRestart => true;

        protected override IStartPage GetStartPage() => new StartPage(this);

        public static ImageSource GetHorizontalImageSource() => horizontal.ToImageSource();

        public static ImageSource GetVerticalImageSource() => vertical.ToImageSource();
    }

    public class StartPage : WinCopies.Installer.StartPage
    {
        public sealed override string Title => "Welcome to WinCopies Sample Installer";

        public override ImageSource ImageSource { get; } = GetVerticalImageSource();

        internal StartPage(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

        protected override ILicenseAgreementPage GetNextPage() => new LicenseAgreementPage(this);
    }

    public class LicenseAgreementPage : WinCopies.Installer.LicenseAgreementPage
    {
        private class LicenseAgreementData : InstallerPageData, ILicenseAgreementData
        {
            public DataFormat DataFormat => DataFormats.GetDataFormat(DataFormats.Text);

            public LicenseAgreementData(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

            public System.IO.Stream GetText()
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);

                sw.WriteLine("This is a dummy license text.");
                sw.Flush();

                return ms;
            }
        }

        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        protected internal LicenseAgreementPage(in StartPage startPage) : base(startPage) { /* Left empty. */ }

        protected override IUserGroupPage GetNextPage() => new UserGroupPage(this);

        protected override ILicenseAgreementData GetData() => new LicenseAgreementData(Installer);
    }

    public class UserGroupPage : WinCopies.Installer.UserGroupPage
    {
        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        internal UserGroupPage(in LicenseAgreementPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IDestinationPage GetNextPage() => new DestinationPage(this);
    }

    public class DestinationPage : WinCopies.Installer.DestinationPage
    {
        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        protected override IOptionsPage GetNextPage() => new OptionsPage(this);

        internal DestinationPage(in UserGroupPage previousPage) : base(previousPage) { /* Left empty. */ }
    }

    public class OptionsPage : WinCopies.Installer.OptionsPage
    {
        protected new class OptionsData : WinCopies.Installer.OptionsPage.OptionsData
        {
            protected class OptionGroup : WinCopies.Installer.OptionGroup
            {
                protected class Option : OptionBase2
                {
                    public Option(in IOptionGroup optionGroup, in bool isChecked, in string name) : base(optionGroup, isChecked, name) { }

                    public override bool Action(out string? log)
                    {
                        log = $"Action '{Name}' selected.";

                        return true;
                    }
                }

                public struct OptionsData
                {
                    public IOption RegisterFileAssociations { get; }

                    public IOption InstallAsPortableApplication { get; }

                    public OptionsData(IOptionGroup optionGroup)
                    {
                        IOption getOption(in string name) => new Option(optionGroup, false, name);

                        RegisterFileAssociations = getOption("Register file associations");
                        InstallAsPortableApplication = getOption("Install as portable application");
                    }
                }

                public OptionsData Options { get; }

                public OptionGroup(in bool isMultipleChoice) : base(isMultipleChoice)
                {
                    Name = "Misc";

                    OptionsData options = Options = new(this);

                    Items = typeof(OptionsData).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(property => new OptionViewModel(property.GetValue(options) as IOption ?? throw new InvalidOperationException($"{nameof(Options)} does not contain valid data.")));
                }
            }

            public OptionsData(in WinCopies.Installer.Installer installer) : base(installer) => Options = Collections.Enumerable.GetEnumerable(new OptionGroup(true));

            protected override void CreateShortcut(in string source, in string destination) => Debug.WriteLine($"Shortcut creation: source: '{source}'; destination: '{destination}'.");
        }

        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public override Icon Icon => LocalResources.WinCopies;

        internal OptionsPage(in WinCopies.Installer.DestinationPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IProcessPage GetNextPage() => new ProcessPage(Installer);

        protected override IOptionsData GetData() => new OptionsData(Installer);
    }

    public class ProcessPage : WinCopies.Installer.ProcessPage
    {
        protected new class ProcessData : WinCopies.Installer.ProcessPage.ProcessData
        {
            protected class DummyStreamReader : ReadOnlyStream
            {
                private long _position;

                public override bool CanSeek => false;
                public override long Length { get; }
                public override long Position { get => _position; set => throw new NotSupportedException(); }

                public DummyStreamReader(in long length) => Length = length;

                public override int Read(byte[] buffer, int offset, int count)
                {
                    long length = buffer.Length < count ? throw new ArgumentOutOfRangeException(nameof(count)) : Length - _position;

                    if (length < count)

                        count = (int)length;

                    var random = new Random();

                    random.NextBytes(new Span<byte>(buffer, offset, count));

                    _position += count;

                    return count;
                }

                public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
                public override void Flush() => throw new NotSupportedException();
                public override void SetLength(long value) => throw new NotSupportedException();
                public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            }

            protected class DummyStreamWriter : System.IO.Stream, IInstallerStream
            {
                private long _length;

                public override bool CanRead => false;
                public override bool CanSeek => false;
                public override bool CanWrite => true;
                public override long Length => _length;
                ulong IWriterStream.Length => (ulong)Length;
                public override long Position { get => _length; set => throw new NotSupportedException(); }

                public ActionIn<string> Logger { get; }

                public string Path { get; }

                public DummyStreamWriter(in string path, in ActionIn<string> logger)
                {
                    Debug.WriteLine($"Created dummy stream for {path}.");

                    Path = path;

                    Logger = logger;
                }

                public override void Flush() { /* Left empty. */ }

                public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
                public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
                public override void SetLength(long value) => throw new NotSupportedException();

                protected void Log(string message) => System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => Logger(message)), DispatcherPriority.Normal);

                public override void Write(byte[] buffer, int offset, int count)
                {
                    Log(Encoding.ASCII.GetString(buffer, offset, count));

                    _length += count;

                    Thread.Sleep(10);
                }

                public void Write(byte[] buffer, uint offset, uint count) => Write(buffer, (int)offset, (int)count);

                public Action GetDeleter() => () => Log($"Deleted {Path}.");
            }

            protected IDictionary<string, WinCopies.Installer.File> InnerDictionary { get; } = new Dictionary<string, WinCopies.Installer.File>();

            public override IEnumerable<KeyValuePair<string, string>>? Resources => null;

            public override IInstallerStream GetWriter(string path) => new DummyStreamWriter(path, (in string text) => Debug.WriteLine(text));

            public ProcessData(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

            protected override IReadOnlyDictionary<string, WinCopies.Installer.File> GetFiles()
            {
                IDictionary<string, WinCopies.Installer.File> dic = InnerDictionary;

                var random = new Random();

                WinCopies.Installer.File getFile(in bool isMainApp) => new(isMainApp, new DummyStreamReader(random.NextInt64(1048576)));

                Func<WinCopies.Installer.File> func = () =>
                {
                    func = () => getFile(false);

                    return getFile(true);
                };

                for (char c = 'a'; c <= 'z'; c++)

                    dic.Add(c.ToString(), func());

                return new ReadOnlyDictionary<string, WinCopies.Installer.File>(InnerDictionary);
            }

            public override void DeleteOldFiles(Action<string> logger) => logger("Deleted old files.");
        }

        public override Icon Icon => LocalResources.WinCopies;

        public override ImageSource ImageSource { get; } = GetHorizontalImageSource();

        public ProcessPage(in WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }

        protected override IProcessData GetData() => new ProcessData(Installer);
        protected override IEndPage GetNextPage() => new EndPage(Installer);
    }

    public class EndPage : WinCopies.Installer.EndPage
    {
        public override ImageSource ImageSource { get; } = GetVerticalImageSource();

        public EndPage(WinCopies.Installer.Installer installer) : base(installer) { /* Left empty. */ }
    }

    public partial class MainWindow : InstallerWindow
    {
        public MainWindow() : base(new InstallerViewModel(new Installer())) { /* Left empty. */ }
    }
}
