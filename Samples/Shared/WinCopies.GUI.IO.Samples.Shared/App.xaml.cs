using System.Collections.Generic;

using WinCopies.IO;

namespace WinCopies.GUI.IO.Samples
{
    public partial class App : Application
    {
        internal new IEnumerable<IBrowsableObjectInfoPlugin> PluginParameters => base.PluginParameters;

        public static new App Current => (App)System.Windows.Application.Current;
    }
}
