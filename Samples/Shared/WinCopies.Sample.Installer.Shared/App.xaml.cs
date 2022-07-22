using System.Windows;

namespace WinCopies.Sample.Installer
{
    public partial class App : Desktop.Application
    {
        protected override Window GetWindow() => new MainWindow();
    }
}
