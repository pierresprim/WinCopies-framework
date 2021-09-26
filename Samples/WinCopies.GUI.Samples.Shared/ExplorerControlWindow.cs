using WinCopies.GUI.Shell;

namespace WinCopies.GUI.Samples
{
    public partial class ExplorerControlWindow : BrowsableObjectInfoWindow
    {
        protected override BrowsableObjectInfoWindow GetNewBrowsableObjectInfoWindow() => new ExplorerControlWindow();

        protected override void OnAboutWindowRequested() { }

        protected override void OnDelete() { }

        protected override void OnEmpty() { }

        protected override void OnPaste() { }

        protected override void OnQuit() { }

        protected override void OnRecycle() { }

        protected override void OnSubmitABug()
        {
            string url = "https://github.com/pierresprim/WinCopies/issues";

            _ = UtilHelpers.StartProcessNetCore(url);
        }
    }
}
