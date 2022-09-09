using System;

namespace WinCopies.Installer
{
    public partial class ProcessPageViewModel : CommonPageViewModel2<IProcessPage, IEndPage, IProcessData>, IProcessPageViewModel
    {
        public sealed override bool CanBrowseBack => false;

        internal ProcessPageViewModel(in IProcessPage installerPage, in InstallerViewModel installer) : base(installerPage, installer) => MarkAsBusy();

        public override IProcessData GetData() => new ProcessDataViewModel(this);

        public sealed override IInstallerPageViewModel GetPrevious() => throw new InvalidOperationException();
        public override IInstallerPageViewModel GetNext() => new EndPageViewModel(ModelGeneric.NextPage, Installer);
    }
}
