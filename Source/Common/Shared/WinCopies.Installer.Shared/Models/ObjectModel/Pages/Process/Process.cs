namespace WinCopies.Installer
{
    public interface IProcessPage : IBrowsableForwardCommonPage<IEndPage, IProcessData>
    {
        // Left empty.
    }

    public abstract class ProcessPage : CommonInstallerPageBase<IEndPage, IProcessData>, IProcessPage
    {
        public override string Title => "Installing...";

        public override string Description => $"Please wait while {Installer.ProgramName} is being installed on your computer.";

        protected ProcessPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }
}
