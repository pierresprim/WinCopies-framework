using System;
using System.Collections.Generic;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.IO
{
    public interface IFileSystemDialog
    {
        IEnumerable<INamedObject<string>> Filters { get; set; }

        INamedObject<string> SelectedFilter { get; set; }

        Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        IExplorerControlViewModel Path { get; set; }

        IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        FileSystemDialogBoxMode Mode { get; }
    }

    public class FileSystemDialog : IFileSystemDialog
    {
        private IExplorerControlViewModel _path;
        private IEnumerable<INamedObject<string>> _filters;
        private FileSystemDialogBoxMode _mode;

        public IEnumerable<INamedObject<string>> Filters
        {
            get => _filters;

            set
            {
                ValidateFilters(Mode, nameof(Mode));

                _filters = value;
            }
        }

        public INamedObject<string> SelectedFilter { get; set; }

        public Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        public IExplorerControlViewModel Path { get => _path; set => _path = value ?? throw GetArgumentNullException(nameof(value)); }

        public IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        public FileSystemDialogBoxMode Mode
        {
            get => _mode;

            set
            {
                ValidateMode(Mode, nameof(Mode), Filters, nameof(Filters));

                _mode = value;
            }
        }

        public  FileSystemDialog(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path)
        {
            _mode = mode;
            _path = path?? throw GetArgumentNullException(nameof(path));
        }

        public FileSystemDialog(in FileSystemDialogBoxMode mode, IBrowsableObjectInfoViewModel path) : this(mode, ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(path, true)) { /* Left empty. */ }

        public FileSystemDialog(in FileSystemDialogBoxMode mode, IBrowsableObjectInfo path) : this(mode, new BrowsableObjectInfoViewModel(path)) { /* Left empty. */ }

        public static void ValidateFilters(in FileSystemDialogBoxMode mode, in string modePropertyName)
        {
            if (mode == FileSystemDialogBoxMode.SelectFolder)

                throw new InvalidOperationException($"{modePropertyName} must not be set to {nameof(FileSystemDialogBoxMode.SelectFolder)} when modifying {nameof(Filters)}.");
        }

        public static void ValidateMode(in FileSystemDialogBoxMode mode, in string modePropertyName, in IEnumerable<INamedObject<string>> filters, in string filtersPropertyName)
        {
            if (mode == FileSystemDialogBoxMode.SelectFolder && filters != null)

                throw new InvalidOperationException($"Cannot set {modePropertyName} to {nameof(FileSystemDialogBoxMode.SelectFolder)} when {filtersPropertyName} has value.");
        }
    }
}
