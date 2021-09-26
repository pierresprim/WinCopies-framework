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

using System.Drawing.Imaging;
using WinCopies.GUI.Drawing;

namespace WinCopies.IO.PropertySystem
{
    public interface IIconImageInfoProperties : DotNetFix.IDisposable
    {
        string FriendlyBitDepth { get; }

        PixelFormat PixelFormat { get; }

        int ColorsInPalette { get; }

        IconImageFormat IconImageFormat { get; }
    }

    public class IconImageInfoProperties : BrowsableObjectInfoProperties<IconImage>, IIconImageInfoProperties
    {
        public string FriendlyBitDepth => IconImageInfoHelper.GetFriendlyBitDepth(InnerObject.PixelFormat);

        public PixelFormat PixelFormat => InnerObject.PixelFormat;

        public int ColorsInPalette => InnerObject.ColorsInPalette;

        public IconImageFormat IconImageFormat => InnerObject.IconImageFormat;

        public IconImageInfoProperties(in IconImage icon) : base(icon) { /* Left empty. */ }
    }
}
