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
            public int _selectedIndex
#if CS10
                = DefaultSelectedIndex;
#else
                ;

            private Selection(in bool isSelected, in IBrowsableObjectInfo selectedItem, in int selectedIndex)
            {
                _isSelected = isSelected;

                _selectedItem = selectedItem;

                _selectedIndex = selectedIndex;
            }

            public static Selection GetInstance() => new
#if !CS9
                Selection
#endif
                (false, null, DefaultSelectedIndex);
#endif
        }
    }
}
