using System.Collections.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private partial struct _Browsability
        {
            public partial struct Items
            {
                public ObservableCollection<IBrowsableObjectInfoViewModel> _items;
                public bool _itemsLoaded;
                public ItemManagement _itemManagement;
            }
        }
    }
}