using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private struct Selection
        {
            private const int DefaultSelectedIndex = -1;

            public bool _isSelected;
            public IBrowsableObjectInfo _selectedItem;
            public int _selectedIndex;

            private Selection(in bool isSelected, in IBrowsableObjectInfo selectedItem)
            {
                _isSelected = isSelected;

                _selectedItem = selectedItem;

                _selectedIndex = DefaultSelectedIndex;
            }

            public static Selection GetInstance() => new
#if !CS9
                Selection
#endif
                (false, null);
        }
    }
}
