using System;
using System.Globalization;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Shell;
using WinCopies.Temp;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    [MultiValueConversion(typeof(Predicate<IBrowsableObjectInfo>))]
    public class FileSystemDialogBoxPredicateConverter : AlwaysConvertibleOneWayMultiConverter<object, Predicate<IBrowsableObjectInfo>>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        protected override Predicate<IBrowsableObjectInfo> Convert(object[] values, object parameter, CultureInfo culture) => values.Length >= 3 && values[2] is FileSystemDialogBoxMode mode
                ? GetPredicate(values[0] as Predicate<IBrowsableObjectInfo>, values[1] as INamedObject<string>, mode)
                : throw new ArgumentException("The arguments do not match the required pattern.", nameof(values));

        public static Predicate<IBrowsableObjectInfo> GetPredicate(Predicate<IBrowsableObjectInfo> predicate, INamedObject<string> filter, in FileSystemDialogBoxMode mode)
        {
            bool _predicate(in IBrowsableObjectInfo browsableObjectInfo, in PredicateIn<IShellObjectInfo> ___predicate)
            {
                var shellObjectInfo = browsableObjectInfo as IShellObjectInfo;

                if (shellObjectInfo == null && browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel)

                    shellObjectInfo = viewModel.Model as IShellObjectInfo;

                return shellObjectInfo == null || ___predicate(shellObjectInfo);
            }

            bool isFolder(in IShellObjectInfo shellObjectInfo) => shellObjectInfo.ObjectProperties.FileType.IsFolder() || ((shellObjectInfo.ObjectProperties.FileType == WinCopies.IO.FileType.Link) && shellObjectInfo.Browsability.Browsability == WinCopies.IO.Browsability.RedirectsToBrowsableItem);

            PredicateIn<IShellObjectInfo> __predicate;

            if (mode == FileSystemDialogBoxMode.SelectFolder)

                __predicate = isFolder;

            else

                __predicate = (in IShellObjectInfo shellObjectInfo) =>
                {
                    if (isFolder(shellObjectInfo))

                        return true;

                    if (shellObjectInfo.ObjectProperties.FileType.IsFile())

                        if (filter?.Value != null)

                            return WinCopies.IO.Path.Match(shellObjectInfo.Name, filter.Value);

                        else

                            return true;

                    else

                        return true;
                };

            bool result(IBrowsableObjectInfo browsableObjectInfo) => _predicate(browsableObjectInfo, __predicate);

            if (predicate == null)

                return result;

            else

                return browsableObjectInfo => result(browsableObjectInfo) && predicate(browsableObjectInfo);
        }
    }
}
