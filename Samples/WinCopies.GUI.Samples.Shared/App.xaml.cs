using System.Windows;
using WinCopies.IO;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal IBrowsableObjectInfoPlugin PluginParameters { get; private set; }

        public static new App Current => (App)Application.Current;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            PluginParameters = Shell.ObjectModel.BrowsableObjectInfo.GetPluginParameters();

            PluginParameters.RegisterBrowsabilityPaths();
            PluginParameters.RegisterItemSelectors();
            PluginParameters.RegisterProcessSelectors();
        }
    }
}
