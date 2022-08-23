using System.Windows;
using System.Windows.Documents;

namespace WinCopies.Installer
{
    public interface ILicenseAgreementData : IInstallerPageData
    {
        DataFormat DataFormat { get; }

        System.IO.Stream GetText();
    }

    public interface ILicenseAgreementDataViewModel : ILicenseAgreementData
    {
        FlowDocument Document { get; }
    }

    public interface ILicenseAgreementPage : IBrowsableCommonPage<IStartPage, IUserGroupPage, ILicenseAgreementData>
    {
        // Left empty.
    }

    public abstract class LicenseAgreementPage : CommonInstallerPage<StartPage, IUserGroupPage, ILicenseAgreementData>, ILicenseAgreementPage
    {
        public override string Title => "License Agreement";

        public override string Description => $"Please read the terms of the license of {Installer.ProgramName} before starting the installation.";

        public override string NextStepName => "I _Agree";

        IStartPage IBrowsableInstallerPage<IStartPage, IUserGroupPage>.PreviousPage => PreviousPage;

        protected LicenseAgreementPage(in StartPage previousPage) : base(previousPage) { /* Left empty. */ }
    }
}
