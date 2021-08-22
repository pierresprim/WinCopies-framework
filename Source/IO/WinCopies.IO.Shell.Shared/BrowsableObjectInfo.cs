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
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

using static WinCopies.IO.Shell.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO.Shell
{
    public class BrowsableObjectInfoIconBitmapSources : BrowsableObjectInfoBitmapSources<Icon>
    {
        protected override BitmapSource SmallBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource MediumBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource LargeBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource ExtraLargeBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        public BrowsableObjectInfoIconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }
    }

    namespace ObjectModel
    {
        public static class BrowsableObjectInfo
        {
            private static void EmptyVoid() { /* Left empty. */ }

            public static Action RegisterDefaultBrowsabilityPaths { get; private set; } = () =>
            {
                ShellObjectInfo.BrowsabilityPathStack.Push(new MultiIconInfo.BrowsabilityPath());
                ShellObjectInfo.BrowsabilityPathStack.Push(new DotNetAssemblyInfo.BrowsabilityPath());

                RegisterDefaultBrowsabilityPaths = EmptyVoid;
            };

            public static Action RegisterDefaultProcessSelectors { get; private set; } = () =>
            {
                ShellObjectInfo.RegisterProcessSelectors();

                RegisterDefaultProcessSelectors = EmptyVoid;
            };

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
