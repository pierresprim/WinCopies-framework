using Microsoft.WindowsAPICodePack.Shell;

using System;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.Shell
{
    public class BrowsableObjectInfoFactory : IO.BrowsableObjectInfoFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoFactory"/> class.
        /// </summary>
        /// <param name="clientVersion">The <see cref="ClientVersion"/> value for PortableDevice items creation. See <see cref="ClientVersion"/>.</param>
        public BrowsableObjectInfoFactory(in ClientVersion clientVersion) : base(clientVersion) { /* Left empty. */ }

        public BrowsableObjectInfoFactory() : base() { /* Left empty. */ }
    }
}
