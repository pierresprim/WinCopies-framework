using System;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private partial struct _Browsability
        {
            public partial struct Items
            {
                public struct ItemManagement
                {
                    public Predicate<IBrowsableObjectInfo> _filter;
                    public Comparison<IBrowsableObjectInfo> _sortComparison;
                    public System.ComponentModel.ICollectionView _collectionView;
                }
            }
        }
    }
}
