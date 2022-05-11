using System;
using System.Globalization;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Shell;
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

        public static Predicate<IBrowsableObjectInfo> GetPredicate(Predicate<IBrowsableObjectInfo> predicate, INamedObject<string> filter, FileSystemDialogBoxMode mode)
        {
            bool _predicate(in IBrowsableObjectInfo browsableObjectInfo, in PredicateIn<IShellObjectInfo> ___predicate)
            {
                var shellObjectInfo = browsableObjectInfo as IShellObjectInfo;

                if (shellObjectInfo == null && browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel)

                    shellObjectInfo = viewModel.Model as IShellObjectInfo;

                return shellObjectInfo == null || ___predicate(shellObjectInfo);
            }

            bool isFolder(in IShellObjectInfo shellObjectInfo) => shellObjectInfo.ObjectProperties.FileType.IsFolder() || ((shellObjectInfo.ObjectProperties.FileType == WinCopies.IO.FileType.Link) && shellObjectInfo.Browsability.Browsability == WinCopies.IO.Browsability.RedirectsToBrowsableItem);

            bool result(IBrowsableObjectInfo browsableObjectInfo) => _predicate(browsableObjectInfo, mode == FileSystemDialogBoxMode.SelectFolder
                ?
#if !CS9
                (PredicateIn<IShellObjectInfo>)
#endif
                isFolder
                : ((in IShellObjectInfo shellObjectInfo) => isFolder(shellObjectInfo)
                        || (shellObjectInfo.ObjectProperties.FileType.IsFile()
                        ? filter?.Value == null ? true : WinCopies.IO.Path.Match(shellObjectInfo.Name, filter.Value)
                        : true)));

            return predicate == null ?
#if !CS9
                (Predicate<IBrowsableObjectInfo>)
#endif
                result : (browsableObjectInfo => result(browsableObjectInfo) && predicate(browsableObjectInfo));
        }
    }
}
