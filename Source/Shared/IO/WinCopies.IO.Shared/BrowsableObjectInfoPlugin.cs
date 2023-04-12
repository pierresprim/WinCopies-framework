/* Copyright © Pierre Sprimont, 2021
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

using System.Collections.Generic;

using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO
{
    public interface IBrowsableObjectInfoPlugin
    {
        IBitmapSourceProvider BitmapSourceProvider { get; }

        IEnumerable<IBrowsableObjectInfo> GetStartPages(ClientVersion clientVersion);

        IEnumerable<IBrowsableObjectInfo> GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion);

        void RegisterBrowsabilityPaths();

        void RegisterBrowsableObjectInfoSelectors();

        void RegisterProcessSelectors();

        void RegisterItemSelectors();

        void OnRegistrationCompleted();
    }

    public abstract class BrowsableObjectInfoPlugin : IBrowsableObjectInfoPlugin
    {
        public static BrowsableObjectInfoPluginStack RegisterBrowsabilityPathsStack { get; } = new BrowsableObjectInfoPluginStack();

        public static BrowsableObjectInfoPluginStack RegisterBrowsableObjectInfoSelectorsStack { get; } = new BrowsableObjectInfoPluginStack();

        public static BrowsableObjectInfoPluginStack RegisterProcessSelectorsStack { get; } = new BrowsableObjectInfoPluginStack();

        public static BrowsableObjectInfoPluginStack RegisterItemSelectorsStack { get; } = new BrowsableObjectInfoPluginStack();

        public static BrowsableObjectInfoPluginStack OnRegistrationCompletedStack { get; } = new BrowsableObjectInfoPluginStack();

        public abstract IBitmapSourceProvider BitmapSourceProvider { get; }

        public abstract IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetStartPages(ClientVersion clientVersion);

        public abstract IEnumerable<IBrowsableObjectInfo>
#if CS8
            ?
#endif
            GetProtocols(IBrowsableObjectInfo parent, ClientVersion clientVersion);

        public void RegisterBrowsabilityPaths() => RegisterBrowsabilityPathsStack.RunActions();

        public void RegisterBrowsableObjectInfoSelectors() => RegisterBrowsableObjectInfoSelectorsStack.RunActions();

        public void RegisterProcessSelectors() => RegisterProcessSelectorsStack.RunActions();

        public void RegisterItemSelectors() => RegisterItemSelectorsStack.RunActions();

        public void OnRegistrationCompleted() => OnRegistrationCompletedStack.RunActions();
    }
}
