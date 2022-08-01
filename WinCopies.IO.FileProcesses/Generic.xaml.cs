using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinCopies.IO.FileProcesses
{
    public partial class Generic : ResourceDictionary
    {

        public static string GetStringResource(object key) => (string)ResourceDictionary[key];

        public static ResourceDictionary ResourceDictionary { get; } = WinCopies.Util.Generic.AddNewDictionary("/WinCopies.IO.FileProcesses;component/Generic.xaml");

        public static string IncompatibleValues => GetStringResource(nameof(IncompatibleValues));

        public static string LoadFilesInfoModuleHasNotRanYet => GetStringResource(nameof(LoadFilesInfoModuleHasNotRanYet));

        public static string LoadFilesInfoModuleIsRunning => GetStringResource(nameof(LoadFilesInfoModuleIsRunning));

    }
}
