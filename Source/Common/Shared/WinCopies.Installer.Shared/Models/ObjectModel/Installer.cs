using System.Collections.Generic;

using WinCopies.Util;

namespace WinCopies.Installer
{
    public interface IInstaller
    {
        string ProgramName { get; }

        bool Is32Bit { get; }

        bool InstallForCurrentUserOnly { get; }

        string Location { get; }

        string
#if CS8
           ?
#endif
           TemporaryDirectory
        { get; }

        Actions Actions { get; }

        bool Completed { get; }

        bool RequiresRestart { get; }

        IStartPage StartPage { get; }

        IOptionsData Options { get; }

        Error Error { get; }

        IInstallerPage Current { get; set; }

        ExtraData? ExtraData { get; }
    }

    public abstract class Installer : IInstaller
    {
        private byte _bools;

        public abstract string ProgramName { get; }

        public abstract bool Is32Bit { get; }

        public bool InstallForCurrentUserOnly { get => GetBit(0); internal set => SetBit(0, value); }

        public string Location { get; internal set; }

        public string
#if CS8
            ?
#endif
            TemporaryDirectory
        { get; internal set; }

        public Actions Actions { get; internal set; }

        public IEnumerable<KeyValuePair<string, IFile>> Files { get; internal set; }

        public bool Completed { get => GetBit(1); private set => SetBit(1, value); }

        public IStartPage StartPage { get; }

        public IInstallerPage Current { get; set; }

        public IOptionsData Options { get; internal set; }

        public Error Error { get; internal set; }

        public abstract bool RequiresRestart { get; }

        ExtraData? IInstaller.ExtraData => null;

        public Installer() => StartPage = GetStartPage();

        private bool GetBit(in byte pos) => _bools.GetBit(pos);
        private void SetBit(in byte pos, in bool value) => UtilHelpers.SetBit(ref _bools, pos, value);

        internal void MarkAsCompleted() => Completed = true;

        protected abstract IStartPage GetStartPage();
    }
}
