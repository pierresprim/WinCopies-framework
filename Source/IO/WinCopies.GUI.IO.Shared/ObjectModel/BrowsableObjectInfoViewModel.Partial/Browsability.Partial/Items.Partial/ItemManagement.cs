using System;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private partial struct Browsability
        {
            public partial struct Items
            {
                public struct ItemManagement
                {
                    public Predicate<IBrowsableObjectInfo> _filter;
                    public Comparison<IBrowsableObjectInfo> _sortComparison;
                }
            }
        }
    }
}
