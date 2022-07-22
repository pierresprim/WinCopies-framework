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

using static System.Drawing.Imaging.PixelFormat;

namespace WinCopies.IO
{
    public static class IconImageInfoHelper
    {
        public static string GetFriendlyBitDepth(PixelFormat pixelFormat)
#if CS9
            =>
#else
        {
            switch (
#endif
            pixelFormat
#if CS9
        switch
#else
            )
#endif
            {
#if !CS9
                case
#endif
                Format1bppIndexed
#if CS9
                =>
#else
                :
                    return
#endif
                "1-bit B/W"
#if CS9
                ,
#else
                ;
                case
#endif
                Format24bppRgb
#if CS9
                =>
#else
                :
                    return
#endif
                "24-bit True Colors"
#if CS9
                ,
#else
                ;
                case
#endif
                Format32bppArgb
#if CS9
                or
#else
                :
                case
#endif
                Format32bppRgb
#if CS9
                =>
#else
                :
                    return
#endif
                "32-bit Alpha Channel"
#if CS9
                ,
#else
                ;
                case
#endif
                Format8bppIndexed
#if CS9
                =>
#else
                :
                    return
#endif
                "8-bit 256 Colors"
#if CS9
                ,
#else
                ;
                case
#endif
                Format4bppIndexed
#if CS9
                =>
#else
                :
                    return
#endif
                "4-bit 16 Colors"
#if CS9
                ,

                _ =>
#else
                ;
                default:
                    return
#endif
                "Unknown"
#if CS9
                ,
#else
                ;
#endif
            };
#if !CS9
        }
#endif
    }
}
