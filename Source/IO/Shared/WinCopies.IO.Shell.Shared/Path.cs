//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.Shell;

//using System.Collections;
//using System.IO;
//using System.Linq;
//using System.Security;

//using WinCopies.Collections;

//using IfCT = WinCopies.Util.Util.ComparisonType;
//using IfCM = WinCopies.Util.Util.ComparisonMode;
//using IfComp = WinCopies.Util.Util.Comparison;

using Microsoft.Win32;

using SevenZip;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

using WinCopies.Collections;
using WinCopies.Linq;
using Microsoft.WindowsAPICodePack.Shell;
using WinCopies.IO.ObjectModel;
using System.Collections.Specialized;

#if !WinCopies3
using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using static WinCopies.UtilHelpers;

using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO.Shell
{
    public static class Path
    {
        private static readonly Dictionary<InArchiveFormat, string[]> dic = new Dictionary<InArchiveFormat, string[]>();

        public static ReadOnlyDictionary<InArchiveFormat, string[]> InArchiveFormats { get; }

        static Path()
        {
            // todo: to add the other 'in' archive formats

            dic.Add(InArchiveFormat.Zip, new string[] { ".zip" });

            dic.Add(InArchiveFormat.SevenZip, new string[] { ".7z" });

            dic.Add(InArchiveFormat.Arj, new string[] { ".arj" });

            dic.Add(InArchiveFormat.BZip2, new string[] { ".bz2", ".tar", ".xz" });

            dic.Add(InArchiveFormat.Cab, new string[] { ".cab" });

            dic.Add(InArchiveFormat.Chm, new string[] { ".chm" });

            dic.Add(InArchiveFormat.Compound, new string[] { ".cfb" });

            dic.Add(InArchiveFormat.Cpio, new string[] { ".cpio" });

            dic.Add(InArchiveFormat.CramFS, null);

            dic.Add(InArchiveFormat.Deb, new string[] { ".deb", ".udeb" });

            dic.Add(InArchiveFormat.Dmg, new string[] { ".dmg" });

            dic.Add(InArchiveFormat.Elf, new string[] { ".axf", ".bin", ".elf", ".o", ".prx", ".puff", ".ko", ".mod", ".so" });

            dic.Add(InArchiveFormat.Fat, null);

            dic.Add(InArchiveFormat.Flv, new string[] { ".flv" });

            dic.Add(InArchiveFormat.GZip, new string[] { ".gz" });

            dic.Add(InArchiveFormat.Hfs, new string[] { ".hfs" });

            dic.Add(InArchiveFormat.Iso, new string[] { ".iso" });

            dic.Add(InArchiveFormat.Lzh, new string[] { ".lzh" });

            dic.Add(InArchiveFormat.Lzma, new string[] { "lzma" });

            dic.Add(InArchiveFormat.Lzma86, new string[] { ".lzma86" });

            dic.Add(InArchiveFormat.Lzw, new string[] { ".lzw" });

            dic.Add(InArchiveFormat.MachO, new string[] { ".o", ".dylib", ".bundle" });

            dic.Add(InArchiveFormat.Mbr, new string[] { ".mbr" });

            dic.Add(InArchiveFormat.Msi, new string[] { ".msi", ".msp" });

            dic.Add(InArchiveFormat.Mslz, new string[] { ".mslz" });

            dic.Add(InArchiveFormat.Mub, new string[] { ".mub" });

            dic.Add(InArchiveFormat.Nsis, new string[] { ".exe" });

            dic.Add(InArchiveFormat.Ntfs, null);

            dic.Add(InArchiveFormat.PE, new string[] { ".dll", ".ocx", ".sys", ".scr", ".drv", ".efi" });

            dic.Add(InArchiveFormat.Ppmd, new string[] { ".ppmd" });

            dic.Add(InArchiveFormat.Rar, new string[] { ".rar" });

            dic.Add(InArchiveFormat.Rar4, null);

            dic.Add(InArchiveFormat.Rpm, new string[] { ".rpm" });

            dic.Add(InArchiveFormat.Split, new string[] { ".split" });

            dic.Add(InArchiveFormat.SquashFS, null);

            dic.Add(InArchiveFormat.Swf, new string[] { ".swf" });

            dic.Add(InArchiveFormat.Swfc, null);

            dic.Add(InArchiveFormat.Tar, new string[] { ".tar", "tar.gz", "tar.bz2", "tar.xz" });

            dic.Add(InArchiveFormat.TE, null);

            dic.Add(InArchiveFormat.Udf, null);

            dic.Add(InArchiveFormat.UEFIc, null);

            dic.Add(InArchiveFormat.UEFIs, null);

            dic.Add(InArchiveFormat.Vhd, new string[] { ".vhd" });

            dic.Add(InArchiveFormat.Wim, new string[] { ".wim", ".swm" });

            dic.Add(InArchiveFormat.Xar, new string[] { ".xar" });

            dic.Add(InArchiveFormat.XZ, new string[] { ".xz" });

            InArchiveFormats = new ReadOnlyDictionary<InArchiveFormat, string[]>(dic);
        }

        public static StringCollection GetStringCollection(in System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => IO.Path. GetStringCollection(paths.WhereSelect(path => path.InnerObject is ShellObject, path => ((ShellObject)path.InnerObject).ParsingName));

        public static bool IsSupportedArchiveFormat(in string extension)
        {
            foreach (KeyValuePair<InArchiveFormat, string[]> value in InArchiveFormats)

                if (value.Value != null)

                    foreach (string _extension in value.Value)

                        if (_extension == extension)

                            return true;

            return false;
        }

        public static bool IsRegistryPath(in string path)
        {
            FieldInfo[] fields = typeof(Microsoft.Win32.Registry).GetFields();

            foreach (FieldInfo field in fields)

                if (path.StartsWith(((RegistryKey)field.GetValue(null)).Name))

                    return true;

            return false;
        }

        public static bool IsWMIPath(in string path) => (IsNullEmptyOrWhiteSpace(path) ? throw GetArgumentNullException(nameof(path)) : path).StartsWith(IO.Path.PathSeparator);

        //public static string GetShortcutPath(string path)
        //{
        //    var paths = new List<KeyValuePair<string, string>>();

        //    foreach (string environmentPathVariable in PathEnvironmentVariables)
        //    {
        //        string _path = Environment.GetEnvironmentVariable(environmentPathVariable);

        //        if (_path != null)

        //            paths.Add(new KeyValuePair<string, string>(environmentPathVariable, _path));
        //    }



        //    paths.Sort((KeyValuePair<string, string> x, KeyValuePair<string, string> y) => x.Value.Length < y.Value.Length ? 1 : x.Value.Length == y.Value.Length ? 0 : -1);



        //    foreach (KeyValuePair<string, string> _path in paths)

        //        if (path.StartsWith(_path.Value))
        //        {
        //            path = "%" + _path.Key + "%" + path.Substring(_path.Value.Length);

        //            break;
        //        }

        //    return path;
        //}
    }
}
