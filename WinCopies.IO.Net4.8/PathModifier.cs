using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO
{
    public interface IPathModifier
    {

        bool AreItemsLoaded { set; }

        // IBrowsableObjectInfo Parent { set; }

        IBrowsableObjectInfoCollection<IBrowsableObjectInfo> Items { get; }

    }

    internal class PathModifier<TParent, TItems, TFactory> : IPathModifier where TParent : class, IBrowsableObjectInfo where TItems : class, IBrowsableObjectInfo where TFactory : IBrowsableObjectInfoFactory

    {

        private readonly BrowsableObjectInfo<TParent, TItems, TFactory> _path;

        public bool AreItemsLoaded { set => _path.AreItemsLoaded = value; }

        // public IBrowsableObjectInfo Parent { set => _path.Parent = value; }

        public IBrowsableObjectInfoCollection<TItems> Items => _path.items;

        IBrowsableObjectInfoCollection<IBrowsableObjectInfo> IPathModifier.Items => (IBrowsableObjectInfoCollection<IBrowsableObjectInfo>) _path.items ; 

        public PathModifier(BrowsableObjectInfo<TParent, TItems, TFactory> path) => _path = path;

    }
}
