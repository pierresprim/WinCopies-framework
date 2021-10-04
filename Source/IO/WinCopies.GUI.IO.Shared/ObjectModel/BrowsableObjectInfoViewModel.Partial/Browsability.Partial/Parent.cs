namespace WinCopies.GUI.IO.ObjectModel
{
    public partial class BrowsableObjectInfoViewModel
    {
        private partial struct Browsability
        {
            public struct Parent
            {
                public IBrowsableObjectInfoViewModel _parent;
                public bool _parentLoaded;
            }
        }
    }
}
