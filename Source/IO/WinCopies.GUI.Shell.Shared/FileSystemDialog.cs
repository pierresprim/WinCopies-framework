using System;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Shell
{
    public interface IFileSystemDialog
    {
        System.Collections.Generic.IEnumerable<string> Filter { get; set; }

        Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        IExplorerControlViewModel Path { get; set; }

        System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        FileSystemDialogBoxMode Mode { get; }
    }

    public class FileSystemDialog : IFileSystemDialog
    {
        private IExplorerControlViewModel _path;

        public System.Collections.Generic.IEnumerable<string> Filter { get; set; }

        public Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        public IExplorerControlViewModel Path { get => _path; set => _path = value ?? throw GetArgumentNullException(nameof(value)); }

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        public FileSystemDialogBoxMode Mode { get; }

        private FileSystemDialog(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path)
        {
            Mode = mode;

            _path = path;
        }

        public FileSystemDialog(in FileSystemDialogBoxMode mode) : this(mode, ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel())
        {
            // Left empty.
        }

        public static FileSystemDialog Create(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path) => new FileSystemDialog(mode, path ?? throw GetArgumentNullException(nameof(path)));
    }
}
