namespace WinCopies.Installer
{
    public class DestinationPageViewModel : CommonPageViewModel3<IUserGroupPage, IDestinationPage, IOptionsPage, IDestinationData>, IDestinationPage
    {
        protected class DestinationDataViewModel : InstallerPageDataViewModel<IDestinationData>, IDestinationData
        {
            public string Location
            {
                get => ModelGeneric.Location;

                set
                {
                    ModelGeneric.Location = value;

                    OnInstallerPropertyChanged(nameof(Location));
                }
            }

            public DestinationDataViewModel(in DestinationPageViewModel destinationPage) : base(destinationPage.ModelGeneric.Data, destinationPage.Installer) { /* Left empty. */ }
        }

        internal DestinationPageViewModel(in IDestinationPage destinationPage, in InstallerViewModel installer) : base(destinationPage, installer) { /* Left empty. */ }

        public override IDestinationData GetData() => new DestinationDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new UserGroupPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => Installer.OptionsPage
#if CS8
            ??=
#else
            ?? (Installer.OptionsPage =
#endif
            new OptionsPageViewModel(ModelGeneric.NextPage, Installer)
#if !CS8
            )
#endif
            ;
    }
}
