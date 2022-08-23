namespace WinCopies.Installer
{
    public class UserGroupPageViewModel : CommonPageViewModel3<ILicenseAgreementPage, IUserGroupPage, IDestinationPage, IUserGroupData>, IUserGroupPage
    {
        protected class UserGroupDataViewModel : InstallerPageDataViewModel<IUserGroupData>, IUserGroupData
        {
            public bool InstallForCurrentUserOnly
            {
                get => ModelGeneric.InstallForCurrentUserOnly;

                set
                {
                    ModelGeneric.InstallForCurrentUserOnly = value;

                    OnInstallerPropertyChanged(nameof(InstallForCurrentUserOnly));
                }
            }

            public object
#if CS8
                ?
#endif
                ExtraData => Installer.ExtraData?.UserGroup;

            public UserGroupDataViewModel(in UserGroupPageViewModel userGroupPage) : base(userGroupPage.ModelGeneric.Data, userGroupPage.Installer) { /* Left empty. */ }
        }

        internal UserGroupPageViewModel(in IUserGroupPage userGroupPage, in InstallerViewModel installer) : base(userGroupPage, installer) { /* Left empty. */ }

        public override IUserGroupData GetData() => new UserGroupDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new LicenseAgreementPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => new DestinationPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
