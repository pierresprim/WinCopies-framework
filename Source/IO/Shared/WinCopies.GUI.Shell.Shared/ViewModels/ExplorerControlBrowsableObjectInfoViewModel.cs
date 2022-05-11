using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Shell.ObjectModel
{
    public class ExplorerControlViewModel : IO.ObjectModel.ExplorerControlViewModel
    {
        protected ExplorerControlViewModel(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) : base(path, factory) { /* Left empty. */ }

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path) => new ExplorerControlViewModel(path ?? throw GetArgumentNullException(nameof(path)), new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison });

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory) => new ExplorerControlViewModel(path, factory);
    }
}
