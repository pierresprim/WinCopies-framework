using System.Windows;
using WinCopies.IO;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IBrowsableObjectInfoPlugin pluginParameters = Shell.ObjectModel.BrowsableObjectInfo.GetPluginParameters();

            pluginParameters.RegisterBrowsabilityPaths();
            pluginParameters.RegisterItemSelectors();
            pluginParameters.RegisterProcessSelectors();
        }
    }
}
