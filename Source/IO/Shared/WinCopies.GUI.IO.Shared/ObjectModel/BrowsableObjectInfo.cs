/* Copyright © Pierre Sprimont, 2022
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

using WinCopies.IO;
using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ObjectModel;

namespace WinCopies.GUI.IO.ObjectModel
{
    public abstract class PluginInfo : PluginInfo<IBrowsableObjectInfoPlugin>
    {
        private IBrowsableObjectInfo _parent;

        protected override IBrowsableObjectInfo ParentOverride => _parent
#if CS8
                ??=
#else
            ?? (_parent =
#endif
            GetBrowsableObjectInfoStartPage()
#if !CS8
            )
#endif
            ;

        protected PluginInfo(in IBrowsableObjectInfoPlugin plugin, in ClientVersion clientVersion) : base(plugin, clientVersion) { /* Left empty. */ }

        protected abstract BrowsableObjectInfoStartPage GetBrowsableObjectInfoStartPage();
    }

    public abstract class BrowsableObjectInfoStartPage : BrowsableObjectInfoStartPage<IEncapsulatorBrowsableObjectInfo<IBrowsableObjectInfoPlugin>>
    {
        protected override IBitmapSourceProvider BitmapSourceProviderOverride { get; }

        protected BrowsableObjectInfoStartPage(in System.Collections.Generic.IEnumerable<PluginInfo>
#if CS8
            ?
#endif
            pluginInfo, in ClientVersion clientVersion) : base(pluginInfo, clientVersion) => BitmapSourceProviderOverride = new BitmapSourceProvider(new IconBitmapSources(GetIcon()), null, null, true);

        protected abstract System.Drawing.Icon GetIcon();
    }
}
