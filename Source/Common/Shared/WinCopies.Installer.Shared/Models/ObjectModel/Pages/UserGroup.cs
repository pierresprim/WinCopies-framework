namespace WinCopies.Installer
{
    public interface IUserGroupData : IInstallerPageData
    {
        bool InstallForCurrentUserOnly { get; set; }
    }

    public interface IUserGroupPage : IBrowsableCommonPage<ILicenseAgreementPage, IDestinationPage, IUserGroupData>
    {
        // Left empty.
    }

    public abstract class UserGroupPage : CommonInstallerPage<LicenseAgreementPage, IDestinationPage, IUserGroupData>, IUserGroupPage
    {
        protected class UserGroupData : InstallerPageData, IUserGroupData
        {
            public bool InstallForCurrentUserOnly { get => Installer.InstallForCurrentUserOnly; set => Installer.InstallForCurrentUserOnly = value; }

            public UserGroupData(in Installer installer) : base(installer) { /* Left empty. */ }
        }

        public override string Title => "User Group";

        public override string Description => $"Choose between the two user groups the one {Installer.ProgramName} will be installed for.";

        ILicenseAgreementPage IBrowsableInstallerPage<ILicenseAgreementPage, IDestinationPage>.PreviousPage => PreviousPage;

        protected UserGroupPage(in LicenseAgreementPage previousPage) : base(previousPage) { /* Left empty. */ }

        protected override IUserGroupData GetData() => new UserGroupData(Installer);
    }
}
