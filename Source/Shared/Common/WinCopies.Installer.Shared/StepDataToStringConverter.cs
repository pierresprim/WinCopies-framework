using System.Globalization;

using WinCopies.Util.Data;

namespace WinCopies.Installer
{
    public class StepDataToStringConverter : AlwaysConvertibleOneWayConverter<byte, object, string>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        protected override string Convert(byte value, object parameter, CultureInfo culture) => $"Step {value >> 4} of {(byte)(value << 4) >> 4}";
    }
}
