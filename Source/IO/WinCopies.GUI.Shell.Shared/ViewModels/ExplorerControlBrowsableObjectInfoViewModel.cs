using System;
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Shell.ObjectModel
{
    public class ExplorerControlViewModel : IO.ObjectModel.ExplorerControlViewModel
    {
        protected ExplorerControlViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) : base(path, treeViewItems, factory, converter)
        {
            // Left empty.
        }

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison }, converter);

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, factory ?? throw GetArgumentNullException(nameof(factory)), converter);

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlViewModel(path, path.GetRootItems().Select<IBrowsableObjectInfo, IBrowsableObjectInfoViewModel>(factory.GetBrowsableObjectInfoViewModel), factory, converter);

        public static IExplorerControlViewModel From(in IBrowsableObjectInfoViewModel path, in Converter<string, IBrowsableObjectInfo> converter) => From(path, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison }, converter);
    }
}
