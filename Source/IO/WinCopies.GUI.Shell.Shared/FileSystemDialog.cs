using System;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.Shell
{
    public interface IFileSystemDialog
    {
        System.Collections.Generic.IEnumerable<string> Filter { get; set; }

        Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        IBrowsableObjectInfo Path { get; set; }

        System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        FileSystemDialogBoxMode Mode { get; }
    }

    public class FileSystemDialog : IFileSystemDialog
    {
        public System.Collections.Generic.IEnumerable<string> Filter { get; set; }

        public Predicate<IBrowsableObjectInfo> Predicate { get; set; }

        public IBrowsableObjectInfo Path { get; set; }

        public System.Collections.Generic.IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get; set; }

        public FileSystemDialogBoxMode Mode { get; }

        public FileSystemDialog(in FileSystemDialogBoxMode mode) => Mode = mode;
    }
}
