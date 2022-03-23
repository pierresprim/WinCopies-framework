using System.Globalization;
using System.Windows.Data;

using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    [ValueConversion(typeof(FileSystemDialogBoxMode), typeof(string))]
    public class FileSystemDialogBoxModeConverter : AlwaysConvertibleOneWayConverter<FileSystemDialogBoxMode, object, string>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        protected override string Convert(FileSystemDialogBoxMode value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case FileSystemDialogBoxMode.SelectFolder:

                    return Properties.Resources.SelectFolder;

                case FileSystemDialogBoxMode.OpenFile:

                    return Properties.Resources.OpenFile;

                case FileSystemDialogBoxMode.Save:

                    return Properties.Resources.Save;
            }

            throw new InvalidEnumArgumentException(nameof(value), value);
        }
    }
}
