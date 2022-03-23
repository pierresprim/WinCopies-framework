using System.Drawing;
using System.Globalization;
using System.Windows.Data;

using WinCopies.IO;
using WinCopies.Util.Data;

using static WinCopies.GUI.Icons.Properties.Resources;

namespace WinCopies.GUI.Shell
{
    [ValueConversion(typeof(object), typeof(Bitmap))]
    public class BrowsabilityToOpenIconConverter : AlwaysConvertibleOneWayConverter<object, object, Bitmap>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.AllowNull;

        protected override Bitmap Convert(object value, object parameter, CultureInfo culture) => value is Browsability _value && _value == Browsability.BrowsableByDefault ? folder_go : page_go;
    }
}
