using System;
using System.Globalization;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO
{
    [MultiValueConversion(typeof(Predicate<IBrowsableObjectInfo>))]
    public class FileSystemDialogBoxPredicateConverter : AlwaysConvertibleOneWayMultiConverter<object, Predicate<IBrowsableObjectInfo>>
    {
        public override IReadOnlyConversionOptions ConvertOptions => ConverterHelper.ParameterCanBeNull;

        protected override Predicate<IBrowsableObjectInfo> Convert(object[] values, object parameter, CultureInfo culture) => values.Length <= 3 && values[2] is FileSystemDialogBoxMode mode
                ? GetPredicate(values[0] as Predicate<IBrowsableObjectInfo>, values[1] as INamedObject<string>, mode)
                : throw new ArgumentException("The arguments do not match the required pattern.", nameof(values));

        public static Predicate<IBrowsableObjectInfo> GetPredicate( Predicate<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            predicate, INamedObject<string>
#if CS8
            ?
#endif
            filter, FileSystemDialogBoxMode mode)
        {
            bool _predicate(in IBrowsableObjectInfo browsableObjectInfo, in PredicateIn<IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties>> ___predicate)
            {
                var shellObjectInfo = browsableObjectInfo as IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties>;

                if (shellObjectInfo == null && browsableObjectInfo is IBrowsableObjectInfoViewModel viewModel)

                    shellObjectInfo = viewModel.Model as IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties>;

                return shellObjectInfo == null || ___predicate(shellObjectInfo);
            }

            bool isFolder(in IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties> shellObjectInfo) => shellObjectInfo.ObjectProperties.FileType.IsFolder() || ((shellObjectInfo.ObjectProperties.FileType == FileType.Link) && shellObjectInfo.Browsability.Browsability == Browsability.RedirectsToBrowsableItem);

            bool result(IBrowsableObjectInfo browsableObjectInfo) => _predicate(browsableObjectInfo, mode == FileSystemDialogBoxMode.SelectFolder
                ?
#if !CS9
                (PredicateIn<IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties>>)
#endif
                isFolder
                : (in IBrowsableObjectInfo<WinCopies.IO.PropertySystem.IFileSystemObjectInfoProperties> shellObjectInfo) => isFolder(shellObjectInfo)
                    || !shellObjectInfo.ObjectProperties.FileType.IsFile()
                    || filter?.Value == null
                    || WinCopies.IO.Path.Match(shellObjectInfo.Name, filter.Value));

            return predicate == null ?
#if !CS9
                (Predicate<IBrowsableObjectInfo>)
#endif
                result : browsableObjectInfo => result(browsableObjectInfo) && predicate(browsableObjectInfo);
        }
    }
}
