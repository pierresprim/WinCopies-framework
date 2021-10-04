using System.Collections.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private partial struct Browsability
        {
            public partial struct Items
            {
                private ObservableCollection<IBrowsableObjectInfoViewModel> _items;
                private bool _itemsLoaded;
            }
        }
    }
}