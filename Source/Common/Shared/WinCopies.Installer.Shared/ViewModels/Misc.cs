using WinCopies.Util.Data;

namespace WinCopies.Installer
{
    public class ActionsViewModel : ViewModel<OnlineInstallerViewModel>
    {
        public Actions Actions { get => ModelGeneric.Actions; set { ModelGeneric.Actions = value; OnPropertyChanged(nameof(Actions)); } }

        public ActionsViewModel(in OnlineInstallerViewModel model) : base(model) { /* Left empty. */ }
    }

    public class TemporaryDestinationViewModel : ViewModel<OnlineInstallerViewModel>
    {
        public IInstaller Installer => ModelGeneric;

        public string
#if CS8
            ?
#endif
            TemporaryDirectory
        { get => ModelGeneric.TemporaryDirectory; set { ModelGeneric.TemporaryDirectory = value; OnPropertyChanged(nameof(TemporaryDirectory)); } }

        public TemporaryDestinationViewModel(in OnlineInstallerViewModel model) : base(model) { /* Left empty. */ }
    }

    public class OnlineInstallerViewModel : InstallerViewModel
    {
        public OnlineInstallerViewModel(in Installer model) : base(model) { /* Left empty. */ }

        protected override ExtraData GetExtraData() => new
#if !CS9
            ExtraData
#endif
            (new ActionsViewModel(this), new TemporaryDestinationViewModel(this));
    }
}
