using System.Globalization;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
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
