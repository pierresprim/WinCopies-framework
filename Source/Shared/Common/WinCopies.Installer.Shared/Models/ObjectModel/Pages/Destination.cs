namespace WinCopies.Installer
{
    public interface IDestinationData : IInstallerPageData
    {
        string Location { get; set; }
    }

    public interface IDestinationPage : IBrowsableCommonPage<IUserGroupPage, IOptionsPage, IDestinationData>
    {
        // Left empty.
    }

    public abstract class DestinationPage : CommonInstallerPage<UserGroupPage, IOptionsPage, IDestinationData>, IDestinationPage
    {
        protected class DestinationData : InstallerPageData, IDestinationData
        {
            public string Location { get => Installer.Location; set => Installer.Location = value; }

            public DestinationData(in Installer installer) : base(installer) { /* Left empty. */ }
        }

        public override string Title => "Destination directory";

        public override string Description => $"Please choose the directory to which to install {Installer.ProgramName}.";

        IUserGroupPage IBrowsableInstallerPage<IUserGroupPage, IOptionsPage>.PreviousPage => PreviousPage;

        protected DestinationPage(in UserGroupPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IDestinationData GetData() => new DestinationData(Installer);
    }
}
