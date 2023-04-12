using System;

namespace WinCopies.Installer
{
    public class StartPageViewModel : CommonPageViewModel<IStartPage>, IStartPage
    {
        public sealed override bool CanBrowseBack => false;

        public ILicenseAgreementPage NextPage => ModelGeneric.NextPage;

        public StartPageViewModel(in InstallerViewModel installer) : base(installer.StartPage, installer) { /* Left empty. */ }
        internal StartPageViewModel(in IStartPage startPage, in InstallerViewModel installer) : base(startPage, installer) { /* Left empty. */ }

        public sealed override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();

        public sealed override IInstallerPageViewModel GetNext() => new LicenseAgreementPageViewModel(ModelGeneric.NextPage, Installer);
    }

    public class EndPageViewModel : CommonPageViewModel<IEndPage>, IEndPage
    {
        public override bool CanBrowseBack => false;
        public override bool CanBrowseForward => false;
        public override bool CanCancel => false;

        internal EndPageViewModel(in IEndPage installerPage, in InstallerViewModel installer) : base(installerPage, installer) { /* Left empty. */ }

        public override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();
        public override IInstallerPageViewModel GetNext() => throw new InvalidOperationException();
    }
}
