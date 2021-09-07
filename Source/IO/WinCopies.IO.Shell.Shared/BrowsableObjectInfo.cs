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

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;

using static WinCopies.IO.Shell.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO.Shell
{
    public class BrowsableObjectInfoIconBitmapSources : BitmapSources<Icon>
    {
        protected override BitmapSource SmallOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource MediumOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource LargeOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource ExtraLargeOverride => TryGetBitmapSource(InnerObject);

        public BrowsableObjectInfoIconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }
    }

    public class BrowsableObjectInfoPlugin : IO.BrowsableObjectInfoPlugin
    {
        public BrowsableObjectInfoPlugin()
        {
            RegisterBrowsabilityPathsStack.Push(ShellObjectInfo.RegisterDefaultBrowsabilityPaths);

            RegisterProcessSelectorsStack.Push(ShellObjectInfo.RegisterDefaultProcessSelectors);

            OnRegisterCompletedStack.Push(() => PlugInParameters = null);
        }
    }

    namespace ObjectModel
    {
        public static class BrowsableObjectInfo
        {
            public static IBrowsableObjectInfoPlugin PlugInParameters { get; internal set; } = new BrowsableObjectInfoPlugin();

            public static Icon TryGetIcon(in Icon[] icons, in ushort size) => TryGetIcon(icons, new System.Drawing.Size(size, size));

            public static Icon TryGetIcon(in Icon icon, in ushort size) => TryGetIcon(icon, new System.Drawing.Size(size, size));

            public static Icon TryGetIcon(in Icon[] icons, in System.Drawing.Size size) => icons?.TryGetIcon(size, 32, true, true);

            public static Icon TryGetIcon(in Icon icon, in System.Drawing.Size size) => TryGetIcon(icon?.Split(), size);

            public static Icon TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => TryGetIcon(new IconExtractor(WinCopies.IO.Path.GetRealPathFromEnvironmentVariables(IO.Path.System32Path + dll)).GetIcon(iconIndex), size);

            public static BitmapSource TryGetBitmapSource(in int iconIndex, in string dllName, in int size)
            {
                using
#if !CS8
            (
#endif
                Icon icon = TryGetIcon(iconIndex, dllName, new System.Drawing.Size(size, size))
#if CS8
            ;
#else
            )
#endif

                return TryGetBitmapSource(icon);
            }

            public static BitmapSource TryGetBitmapSource(in Icon icon) => icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
