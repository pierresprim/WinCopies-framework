using System.Windows;
using System.Windows.Documents;

namespace WinCopies.Installer
{
    public class LicenseAgreementPageViewModel : CommonPageViewModel3<IStartPage, ILicenseAgreementPage, IUserGroupPage, ILicenseAgreementData>, ILicenseAgreementPage
    {
        protected class LicenseAgreementDataViewModel : InstallerPageDataViewModel<ILicenseAgreementData>, ILicenseAgreementDataViewModel
        {
            private FlowDocument _document;

            public DataFormat DataFormat => ModelGeneric.DataFormat;

            public FlowDocument Document
            {
                get
                {
                    if (_document == null)
                    {
                        _document = new FlowDocument();

                        using
#if !CS8
                            (
#endif
                            System.IO.Stream s = GetText()
#if CS8
                            ;
#else
                            )
#endif
                        new TextRange(_document.ContentStart, _document.ContentEnd).Load(s, DataFormat.Name);
                    }

                    return _document;
                }
            }

            public LicenseAgreementDataViewModel(in LicenseAgreementPageViewModel licenseAgreementPage) : base(licenseAgreementPage.ModelGeneric.Data, licenseAgreementPage.Installer) { /* Left empty. */ }

            public System.IO.Stream GetText() => ModelGeneric.GetText();
        }

        internal LicenseAgreementPageViewModel(in ILicenseAgreementPage licenseAgreementPage, in InstallerViewModel installer) : base(licenseAgreementPage, installer) { /* Left empty. */ }

        public override ILicenseAgreementData GetData() => new LicenseAgreementDataViewModel(this);

        public override IInstallerPageViewModel GetPrevious() => new StartPageViewModel(ModelGeneric.PreviousPage, Installer);
        public override IInstallerPageViewModel GetNext() => new UserGroupPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
