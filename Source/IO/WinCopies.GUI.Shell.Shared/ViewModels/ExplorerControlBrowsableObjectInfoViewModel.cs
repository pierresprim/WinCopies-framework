using System;
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Shell.ObjectModel
{
    public class ExplorerControlBrowsableObjectInfoViewModel : IO.ObjectModel.ExplorerControlBrowsableObjectInfoViewModel
    {
        public ExplorerControlBrowsableObjectInfoViewModel(in IBrowsableObjectInfoViewModel path, in System.Collections.Generic.IEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) : base(path, treeViewItems, factory, converter)
        {
            // Left empty.
        }

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison }, converter);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IDisposableEnumerable<IBrowsableObjectInfoViewModel> treeViewItems, IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlBrowsableObjectInfoViewModel(path ?? throw GetArgumentNullException(nameof(path)), treeViewItems, factory ?? throw GetArgumentNullException(nameof(factory)), converter);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in IBrowsableObjectInfoFactory factory, in Converter<string, IBrowsableObjectInfo> converter) => new ExplorerControlBrowsableObjectInfoViewModel(path, path.GetRootItems().Select<IBrowsableObjectInfo, IBrowsableObjectInfoViewModel>(factory.GetBrowsableObjectInfoViewModel), factory, converter);

        public static IExplorerControlBrowsableObjectInfoViewModel From(in IBrowsableObjectInfoViewModel path, in Converter<string, IBrowsableObjectInfo> converter) => From(path, new BrowsableObjectInfoFactory(path.ClientVersion) { SortComparison = path.SortComparison }, converter);
    }
}
