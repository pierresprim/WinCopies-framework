using System;
using System.Collections.Generic;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;
using WinCopies.Util.Data;

namespace WinCopies.GUI.IO
{
    public class FileSystemDialogBoxViewModel : ViewModel<IFileSystemDialog>, IFileSystemDialog
    {
        public IEnumerable<INamedObject<string>> Filters { get => ModelGeneric.Filters; set { ModelGeneric.Filters = value; OnPropertyChanged(nameof(Filters)); } }

        public INamedObject<string> SelectedFilter { get => ModelGeneric.SelectedFilter; set { ModelGeneric.SelectedFilter = value; OnPropertyChanged(nameof(Filters)); } }

        public Predicate<IBrowsableObjectInfo> Predicate { get => ModelGeneric.Predicate; set { ModelGeneric.Predicate = value; OnPropertyChanged(nameof(Predicate)); } }

        public IExplorerControlViewModel Path { get => ModelGeneric.Path; set { ModelGeneric.Path = value; OnPropertyChanged(nameof(Path)); } }

        public IReadOnlyList<IBrowsableObjectInfo> SelectedItems { get => ModelGeneric.SelectedItems; set { ModelGeneric.SelectedItems = value; OnPropertyChanged(nameof(SelectedItems)); } }

        public FileSystemDialogBoxMode Mode => ModelGeneric.Mode;

        public FileSystemDialogBoxViewModel(in IFileSystemDialog model) : base(model) { /* Left empty. */ }

        public FileSystemDialogBoxViewModel(in FileSystemDialogBoxMode mode, in IExplorerControlViewModel path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        public FileSystemDialogBoxViewModel(in FileSystemDialogBoxMode mode, IBrowsableObjectInfoViewModel path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        public FileSystemDialogBoxViewModel(in FileSystemDialogBoxMode mode, IBrowsableObjectInfo path) : this(new FileSystemDialog(mode, path)) { /* Left empty. */ }

        public bool CompletePredicate(IBrowsableObjectInfo browsableObjectInfo)
        {
            if (SelectedFilter != null)
            {
                string filter = SelectedFilter.Value;

                if (!WinCopies.IO.Path.Match(browsableObjectInfo.Name, filter))

                    return false;
            }

            return Predicate == null || Predicate(browsableObjectInfo);
        }
    }
}
