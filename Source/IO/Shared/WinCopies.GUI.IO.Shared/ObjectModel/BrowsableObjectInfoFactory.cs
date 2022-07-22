/* Copyright © Pierre Sprimont, 2020
*
* This file is part of the WinCopies Framework.
*
* The WinCopies Framework is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* The WinCopies Framework is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;

using WinCopies.GUI.IO.ObjectModel;
using WinCopies.IO;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO
{
    public interface IBrowsableObjectInfoFactory
    {
        ClientVersion ClientVersion { get; }

        Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo);

        IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, IBrowsableObjectInfoViewModel parent);

        IExplorerControlViewModel GetExplorerControlViewModel(IBrowsableObjectInfoViewModel browsableObjectInfo);
    }

    public class BrowsableObjectInfoFactory : IBrowsableObjectInfoFactory
    {
        public ClientVersion ClientVersion { get; }

        public Comparison<IBrowsableObjectInfo> SortComparison { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoFactory"/> class.
        /// </summary>
        /// <param name="clientVersion">The <see cref="ClientVersion"/> value for PortableDevice items creation. See <see cref="ClientVersion"/>.</param>
        public BrowsableObjectInfoFactory(in ClientVersion clientVersion) => ClientVersion = clientVersion;

        public BrowsableObjectInfoFactory() : this(WinCopies.IO.ObjectModel.BrowsableObjectInfo.GetDefaultClientVersion()) { /* Left empty. */ }

        protected virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, bool rootParentIsRootNode) => new BrowsableObjectInfoViewModel(browsableObjectInfo, rootParentIsRootNode) { SortComparison = SortComparison, Factory = this };

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo) => GetBrowsableObjectInfoViewModel(browsableObjectInfo, true);

        public virtual IBrowsableObjectInfoViewModel GetBrowsableObjectInfoViewModel(IBrowsableObjectInfo browsableObjectInfo, IBrowsableObjectInfoViewModel parent) => GetBrowsableObjectInfoViewModel(browsableObjectInfo, parent.RootParentIsRootNode);

        public virtual IExplorerControlViewModel GetExplorerControlViewModel(IBrowsableObjectInfoViewModel browsableObjectInfo) => ObjectModel.BrowsableObjectInfo.GetDefaultExplorerControlViewModel(browsableObjectInfo);
    }
}
