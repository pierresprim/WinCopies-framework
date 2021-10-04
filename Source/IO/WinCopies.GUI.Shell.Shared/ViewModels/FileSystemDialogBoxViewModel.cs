using System;
using System.Collections.Generic;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies.GUI.Shell
{
    public class FileSystemDialogBoxViewModel : ViewModel<IFileSystemDialog>, IFileSystemDialog
    {
        public System.Collections.Generic.IEnumerable<string> Filter { get => ModelGeneric.Filter; set { ModelGeneric.Filter = value; OnPropertyChanged(nameof(Filter)); } }

        public Predicate<IBrowsableObjectInfo> Predicate { get => ModelGeneric.Predicate; set { ModelGeneric.Predicate = value; OnPropertyChanged(nameof(Predicate)); } }

        public IExplorerControlViewModel Path { get => ModelGeneric.Path; set { ModelGeneric.Path = value; OnPropertyChanged(nameof(Path)); } }

        public IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => ModelGeneric.SelectedItems; set { ModelGeneric.SelectedItems = value; OnPropertyChanged(nameof(SelectedItems)); } }

        public FileSystemDialogBoxMode Mode => ModelGeneric.Mode;

        public FileSystemDialogBoxViewModel(in IFileSystemDialog model) : base(model) { /* Left empty. */ }

        public FileSystemDialogBoxViewModel(in FileSystemDialogBoxMode mode) : this(new FileSystemDialog(mode)) { /* Left empty. */ }
    }
}
