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
#if CS8
            =>
#else
        {
            switch (
#endif
            value
#if CS8
            switch
#else
            )
#endif
            {
#if !CS8
                case
#endif
                FileSystemDialogBoxMode.SelectFolder
#if CS8
                =>
#else
                :
                return
#endif
                Properties.Resources.SelectFolder
#if CS8
                ,
#else
                ;
                case
#endif
                FileSystemDialogBoxMode.OpenFile
#if CS8
                =>
#else
                :
                return
#endif
                Properties.Resources.OpenFile
#if CS8
                ,
#else
                ;
                case
#endif
                FileSystemDialogBoxMode.Save
#if CS8
                =>
#else
                :
                return
#endif
                Properties.Resources.Save
#if CS8
                ,
                _ =>
#else
                ;
                default:
#endif
                throw new InvalidEnumArgumentException(nameof(value), value)
#if CS8
            };
#else
                ;
            }
        }
#endif
    }
}
