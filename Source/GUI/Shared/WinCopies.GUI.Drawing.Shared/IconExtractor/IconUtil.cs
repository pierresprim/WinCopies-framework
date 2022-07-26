/*
 *  IconExtractor/IconUtil for .NET
 *  Copyright (C) 2014 Tsuda Kageyu. All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 *  TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 *  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 *  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WinCopies.Util;
using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;

namespace WinCopies.GUI.Drawing
{
    public static class IconUtil
    {
        //private delegate byte[] GetIconDataDelegate(Icon icon);

        public static byte[] ToBytes(this Icon icon)
        {
            using (var ms = new MemoryStream())
            {
                icon.Save(ms);

                return ms.ToArray();
            }
        }

        public static Icon ToIcon(in byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))

                return new Icon(ms);
        }

        //private static readonly GetIconDataDelegate getIconData;

        //static IconUtil()
        //{
        //    // Create a dynamic method to access Icon.iconData private field.

        //    var dm = new DynamicMethod(
        //        "GetIconData", typeof(byte[]), new Type[] { typeof(Icon) }, typeof(Icon));
        //    FieldInfo fi = typeof(Icon).GetField(
        //        "iconData", BindingFlags.Instance | BindingFlags.NonPublic);
        //    ILGenerator gen = dm.GetILGenerator();
        //    gen.Emit(OpCodes.Ldarg_0);
        //    gen.Emit(OpCodes.Ldfld, fi);
        //    gen.Emit(OpCodes.Ret);

        //    getIconData = (GetIconDataDelegate)dm.CreateDelegate(typeof(GetIconDataDelegate));
        //}

        /// <summary>
        /// Splitting an <see cref="Icon"/> consists of multiple icons into an array of <see cref="Icon"/> each
        /// consists of single icon.
        /// </summary>
        /// <param name="icon">A <see cref="Icon"/> to be split.</param>
        /// <returns>An array of <see cref="Icon"/>s.</returns>
        public static Icon[] Split(this Icon icon)
        {
            // Get an .ico file in memory, then split it into separate icons.

            byte[] src = icon == null ? throw new ArgumentNullException(nameof(icon)) : GetIconData(icon);

            var splitIcons = new List<Icon>();
            {
                int count = BitConverter.ToUInt16(src, 4);

                for (int i = 0; i < count; i++)
                {
                    int length = BitConverter.ToInt32(src, 6 + (16 * i) + 8);    // ICONDIRENTRY.dwBytesInRes
                    int offset = BitConverter.ToInt32(src, 6 + (16 * i) + 12);   // ICONDIRENTRY.dwImageOffset

                    using (var dst = new BinaryWriter(new MemoryStream(6 + 16 + length)))
                    {
                        // Copy ICONDIR and set idCount to 1.

                        dst.Write(src, 0, 4);
                        dst.Write((short)1);

                        // Copy ICONDIRENTRY and set dwImageOffset to 22.

                        dst.Write(src, 6 + (16 * i), 12); // ICONDIRENTRY except dwImageOffset
                        dst.Write(22);                   // ICONDIRENTRY.dwImageOffset

                        // Copy a picture.

                        dst.Write(src, offset, length);

                        // Create an icon from the in-memory file.

                        _ = dst.BaseStream.Seek(0, SeekOrigin.Begin);
                        splitIcons.Add(new Icon(dst.BaseStream));
                    }
                }
            }

            return splitIcons.ToArray();
        }

        public static Icon
#if CS8
            ?
#endif
            TryGetIcon(this Icon icon, in Size size, in int bits, in bool tryResize, in bool tryRedefineBitsCount) => icon.Split().TryGetIcon(size, bits, tryResize, tryRedefineBitsCount);

        public static Icon
#if CS8
            ?
#endif
            TryGetIcon(this Icon[] icons, in Size size, in int bits, in bool tryResize, in bool tryRedefineBitsCount)
        {
            //Icon[] icons = icon.Split();

            foreach (Icon i in icons)

                if (i.Size == size && i.GetBitCount() == bits)

                    return i;

            if (tryResize || tryRedefineBitsCount)
            {
                Icon icon = icons[0];
                Icon i;

                for (int _i = 0; _i < icons.Length; _i++)
                {
                    i = icons[_i];

                    bool result = (i.Size == size || tryResize) && ((i.Size.Height > size.Height && (icon == null || i.Size.Height > icon.Size.Height)) || (i.Size.Height < size.Height && (icon == null || i.Size.Height > icon.Size.Height)));

                    if (!result)
                    {
                        int i_bits = i.GetBitCount();

                        int icon_bits = icon.GetBitCount();

                        result = (i_bits == bits || tryRedefineBitsCount) && ((i_bits > bits && (icon == null || i_bits > icon_bits)) || (i_bits < bits && (icon == null || i_bits > icon_bits)));
                    }

                    if (result)

                        icon = i;
                }

                return icon;
            }

            return null;
        }

        /// <summary>
        /// Converts an <see cref="Icon"/> to a GDI+ <see cref="Bitmap"/> preserving the transparent area.
        /// </summary>
        /// <param name="icon">An <see cref="Icon"/> to be converted.</param>
        /// <returns>A <see cref="Bitmap"/> object.</returns>
        public static Bitmap ToBitmap(in Icon icon)
        {
            ThrowIfNull(icon, nameof(icon));

            // Quick workaround: Create an .ico file in memory, then load it as a Bitmap.

            using (var ms = new MemoryStream())
            {
                icon.Save(ms);

                using (var bmp = (Bitmap)Image.FromStream(ms))

                    return new Bitmap(bmp);
            }
        }

        /// <summary>
        /// Gets the bit depth of an <see cref="Icon"/>.
        /// </summary>
        /// <param name="icon">An <see cref="Icon"/> object.</param>
        /// <returns>Bit depth of the <see cref="Icon"/>.</returns>
        /// <remarks>
        /// This method takes into account the PNG header.
        /// If the <see cref="Icon"/> has multiple variations, this method returns the bit
        /// depth of the first variation.
        /// </remarks>
        public static int GetBitCount(this Icon icon)
        {
            ThrowIfNull(icon, nameof(icon));

            // Get an .ico file in memory, then read the header.

            byte[] data = GetIconData(icon);
            var values = new ArrayValueProvider<byte>(data, 22);

            bool check(params byte[] valuesToCheck)
            {
                using (IEnumerator<byte> enumerator = valuesToCheck.AsFromType<IReadOnlyList<byte>>().GetEnumerator())
                {
                    bool moveNextSucceeded;

                    while ((moveNextSucceeded = enumerator.MoveNext()) && values.CurrentValue == enumerator.Current) { /* Left empty. */ }

                    return !moveNextSucceeded;
                }
            }

            if (data.Length >= 51
                && check(0x89, 0x50, 0x4e, 0x47,
                         0x0d, 0x0a, 0x1a, 0x0a,
                         0x00, 0x00, 0x00, 0x0d,
                         0x49, 0x48, 0x44, 0x52))

                // The picture is PNG. Read IHDR chunk.

                switch (data[47])
                {
                    case 0:
                        return data[46];
                    case 2:
                        return data[46] * 3;
                    case 3:
                        return data[46];
                    case 4:
                        return data[46] * 2;
                    case 6:
                        return data[46] * 4;
                    default:
                        // NOP
                        break;
                }

            else if (data.Length >= 22)

                // The picture is not PNG. Read ICONDIRENTRY structure.

                return BitConverter.ToUInt16(data, 12);

            throw new ArgumentException("The icon is corrupt. Couldn't read the header.", nameof(icon));
        }

        private static byte[] GetIconData(in Icon icon)
        {
            byte[] data = icon.ToBytes();

            if (data == null)

                using (var ms = new MemoryStream())
                {
                    icon.Save(ms);
                    return ms.ToArray();
                }

            else

                return data;
        }
    }
}
