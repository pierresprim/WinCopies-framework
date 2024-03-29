﻿/*
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

using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using static Microsoft.WindowsAPICodePack.NativeAPI.Consts.Shell;

using static WinCopies.
#if !WinCopies3
    Util.Util
#else
    ThrowHelper
#endif
    ;

namespace WinCopies.GUI.Drawing
{
    public class IconExtractor
    {
        #region Constants
        // Flags for LoadLibraryEx().

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        // Resource types for EnumResourceNames().

        private static readonly IntPtr RT_ICON = (IntPtr)3;
        private static readonly IntPtr RT_GROUP_ICON = (IntPtr)14;
        #endregion

        private byte[][] iconData = null;   // Binary data of each icon.

        #region Public properties
        /// <summary>
        /// Gets the full path of the associated file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the count of the icons in the associated file.
        /// </summary>
        public int Count => iconData.Length;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IconExtractor"/> class from the specified file name.
        /// </summary>
        /// <param name="fileName">The file to extract icons from.</param>
        public IconExtractor(in string fileName) => Initialize(fileName);

        /// <summary>
        /// Extracts an icon from the file.
        /// </summary>
        /// <param name="index">Zero based index of the icon to be extracted.</param>
        /// <returns>A <see cref="Icon"/> object.</returns>
        /// <remarks>Always returns new copy of the <see cref="Icon"/>. It should be disposed by the user.</remarks>
        public Icon GetIcon(in int index)
        {
            // Create an Icon based on a .ico file in memory.

            using (var ms = index < 0 || Count <= index ? throw new ArgumentOutOfRangeException(nameof(index)) : new MemoryStream(iconData[index]))

                return new Icon(ms);
        }

        /// <summary>
        /// Extracts all the icons from the file.
        /// </summary>
        /// <returns>An array of <see cref="Icon"/> objects.</returns>
        /// <remarks>Always returns new copies of the Icons. They should be disposed by the user.</remarks>
        public Icon[] GetAllIcons()
        {
            var icons = new Icon[Count];

            for (int i = 0; i < Count; ++i)

                icons[i] = GetIcon(i);

            return icons;
        }

        /// <summary>
        /// Save an icon to the specified output <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="index">Zero based index of the icon to be saved.</param>
        /// <param name="outputStream">The <see cref="System.IO.Stream"/> to save to.</param>
        public void Save(in int index, in System.IO.Stream outputStream)
        {
            if (index < 0 || Count <= index)

                throw new ArgumentOutOfRangeException(nameof(index));

            ThrowIfNull(outputStream, nameof(outputStream));

            byte[] data = iconData[index];
            outputStream.Write(data, 0, data.Length);
        }

        private void Initialize(string fileName)
        {
            ThrowIfNullEmptyOrWhiteSpace(fileName, nameof(fileName));

            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = Core.LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);

                FileName = hModule == IntPtr.Zero ? throw new Win32Exception() : GetFileName(hModule);

                // Enumerate the icon resource and build .ico files in memory.

                var tmpData = new List<byte[]>();

                bool callback(IntPtr h, IntPtr t, IntPtr name, IntPtr l)
                {
                    // Refer the following URL for the data structures used here:
                    // http://msdn.microsoft.com/en-us/library/ms997538.aspx

                    // RT_GROUP_ICON resource consists of a GRPICONDIR and GRPICONDIRENTRY's.

                    byte[] dir = IconExtractor.GetDataFromResource(hModule, RT_GROUP_ICON, name);

                    // Calculate the size of an entire .icon file.

                    int count = BitConverter.ToUInt16(dir, 4);  // GRPICONDIR.idCount
                    int len = 6 + (16 * count);                   // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                    for (int i = 0; i < count; ++i)

                        len += BitConverter.ToInt32(dir, 6 + 14 * i + 8);   // GRPICONDIRENTRY.dwBytesInRes

                    using (var dst = new BinaryWriter(new MemoryStream(len)))
                    {
                        // Copy GRPICONDIR to ICONDIR.

                        dst.Write(dir, 0, 6);

                        int picOffset = 6 + (16 * count); // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                        for (int i = 0; i < count; ++i)
                        {
                            // Load the picture.

                            ushort id = BitConverter.ToUInt16(dir, 6 + (14 * i) + 12);    // GRPICONDIRENTRY.nID
                            byte[] pic = IconExtractor.GetDataFromResource(hModule, RT_ICON, (IntPtr)id);

                            // Copy GRPICONDIRENTRY to ICONDIRENTRY.

                            _ = dst.Seek(6 + (16 * i), SeekOrigin.Begin);

                            dst.Write(dir, 6 + (14 * i), 8);  // First 8bytes are identical.
                            dst.Write(pic.Length);          // ICONDIRENTRY.dwBytesInRes
                            dst.Write(picOffset);           // ICONDIRENTRY.dwImageOffset

                            // Copy a picture.

                            _ = dst.Seek(picOffset, SeekOrigin.Begin);
                            dst.Write(pic, 0, pic.Length);

                            picOffset += pic.Length;
                        }

                        tmpData.Add(((MemoryStream)dst.BaseStream).ToArray());
                    }

                    return true;
                }

                _ = Core.EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);

                iconData = tmpData.ToArray();
            }

            finally
            {
                if (hModule != IntPtr.Zero)

                    _ = Core.FreeLibrary(hModule);
            }
        }

        private static byte[] GetDataFromResource(IntPtr hModule, IntPtr type, IntPtr name)
        {
            // Load the binary data from the specified resource.

            IntPtr hResInfo = Core.FindResource(hModule, name, type);

            IntPtr hResData = hResInfo == IntPtr.Zero ? throw new Win32Exception() : Core.LoadResource(hModule, hResInfo);

            IntPtr pResData = hResData == IntPtr.Zero ? throw new Win32Exception() : Core.LockResource(hResData);

            uint size = pResData == IntPtr.Zero ? throw new Win32Exception() : Core.SizeofResource(hModule, hResInfo);

            byte[] buf = new byte[size == 0 ? throw new Win32Exception() : size];
            Marshal.Copy(pResData, buf, 0, buf.Length);

            return buf;
        }

        private static string GetFileName(IntPtr hModule)
        {
            // Alternative to GetModuleFileName() for the module loaded with
            // LOAD_LIBRARY_AS_DATAFILE option.

            // Get the file name in the format like:
            // "\\Device\\HarddiskVolume2\\Windows\\System32\\shell32.dll"

            string fileName;
            {
                var buf = new StringBuilder(MaxPath);

                fileName = Core.GetMappedFileName(
                    Core.GetCurrentProcess(), hModule, buf, (uint)buf.Capacity) == 0 ? throw new Win32Exception() : buf.ToString();
            }

            // Convert the device name to drive name like:
            // "C:\\Windows\\System32\\shell32.dll"

            for (char c = 'A'; c <= 'Z'; ++c)
            {
                string drive = c + ":";
                var buf = new StringBuilder(MaxPath);
                uint len = Core.QueryDosDevice(drive, buf, (uint)buf.Capacity);

                if (len == 0)

                    continue;

                string devPath = buf.ToString();

                if (fileName.StartsWith(devPath, StringComparison.OrdinalIgnoreCase))

                    return drive + fileName.Substring(devPath.Length);
            }

            return fileName;
        }
    }
}
