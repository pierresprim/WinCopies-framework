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

        /// <summary>
        /// Creates an <see cref="IBrowsableObjectInfo"/> for a given path. See Remarks section.
        /// </summary>
        /// <param name="path">The path of the <see cref="IBrowsableObjectInfo"/> to create.</param>
        /// <returns>An <see cref="IBrowsableObjectInfo"/> for <paramref name="path"/>.</returns>
        /// <remarks>This method cannot create <see cref="IBrowsableObjectInfo"/> for WMI paths.</remarks>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a Shell or a Registry path.</exception>
        public override IBrowsableObjectInfo GetBrowsableObjectInfo(string path)
        {
            if (Path.IsFileSystemPath(path))

                return ShellObjectInfo.From(ShellObjectFactory.Create(path), ClientVersion);

            else if (WinCopies.IO.Shell.Path.IsRegistryPath(path))

                return new RegistryItemInfo(path, BrowsableObjectInfo.GetDefaultClientVersion());

            throw new ArgumentException("The factory cannot create an object for the given path.");
        }

        public override IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(string path) => GetBrowsableObjectInfoViewModel(GetBrowsableObjectInfo(path), false);
    }
}
