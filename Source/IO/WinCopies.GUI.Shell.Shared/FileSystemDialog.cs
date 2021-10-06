using System;
using System.Collections.Generic;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Shell
{
    public interface IFileSystemDialog
    {
        System.Collections.Generic.IEnumerable<INamedObject<string>> Filters { get; set; }

        INamedObject<string> SelectedFilter { get; set; }

        Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        IExplorerControlViewModel Path { get; set; }

        System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        FileSystemDialogBoxMode Mode { get; }
    }

    public class FileSystemDialog : IFileSystemDialog
    {
        private IExplorerControlViewModel _path;
        private IEnumerable<INamedObject<string>> _filters;
        private FileSystemDialogBoxMode _mode;

        public System.Collections.Generic.IEnumerable<INamedObject<string>> Filters
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

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        public FileSystemDialogBoxMode Mode
        {
            get => _mode;

            set
            {
                ValidateMode(Mode, nameof(Mode), Filters, nameof(Filters));

                _mode = value;
            }
        }

        private FileSystemDialog(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path)
        {
            _mode = mode;

            _path = path;
        }

        public FileSystemDialog(in FileSystemDialogBoxMode mode) : this(mode, ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel())
        {
            // Left empty.
        }

        public static FileSystemDialog Create(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path) => new FileSystemDialog(mode, path ?? throw GetArgumentNullException(nameof(path)));

        public static void ValidateFilters(in FileSystemDialogBoxMode mode, in string modePropertyName)
        {
            if (mode == FileSystemDialogBoxMode.SelectFolder)

                throw new InvalidOperationException($"{modePropertyName} must not be set to {nameof(FileSystemDialogBoxMode.SelectFolder)} when modifying {nameof(Filters)}.");
        }

        public static void ValidateMode(in FileSystemDialogBoxMode mode, in string modePropertyName, in System.Collections.Generic.IEnumerable<INamedObject<string>> filters, in string filtersPropertyName)
        {
            if (mode == FileSystemDialogBoxMode.SelectFolder && filters != null)

                throw new InvalidOperationException($"Cannot set {modePropertyName} to {nameof(FileSystemDialogBoxMode.SelectFolder)} when {filtersPropertyName} has value.");
        }
    }
}
