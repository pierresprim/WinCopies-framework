﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static WinCopies.Win32NativeInterop.NativeMethods;

namespace WinCopies.Win32Interop
{


    /*
     * Example using ExtractIconEx
     * Created by Martin Hyldahl (alanadin@post8.tele.dk)
     * http://www.hyldahlnet.dk
     */

    public static class Icon
    {

        public static System.Drawing.Icon ExtractIconFromExe(string file, int index, bool large)
        {
            int readIconCount = 0;
            IntPtr[] hDummy = new IntPtr[1] { IntPtr.Zero };
            IntPtr[] hIconEx = new IntPtr[1] { IntPtr.Zero };

            try
            {
                if (large)
                    readIconCount = ExtractIconEx(file, index, hIconEx, hDummy, 1);
                else
                    readIconCount = ExtractIconEx(file, index, hDummy, hIconEx, 1);

                if (readIconCount > 0 && hIconEx[0] != IntPtr.Zero)
                {
                    // GET FIRST EXTRACTED ICON
                   System.Drawing. Icon extractedIcon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIconEx[0]).Clone();

                    return extractedIcon;
                }
                else // NO ICONS READ
                    return null;
            }
            catch (Exception ex)
            {
                /* EXTRACT ICON ERROR */

                // BUBBLE UP
                throw new ApplicationException("Could not extract icon", ex);
            }
            finally
            {
                // RELEASE RESOURCES
                foreach (IntPtr ptr in hIconEx)
                    if (ptr != IntPtr.Zero)
                        DestroyIcon(ptr);

                foreach (IntPtr ptr in hDummy)
                    if (ptr != IntPtr.Zero)
                        DestroyIcon(ptr);
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using WinCopies.Win32Interop;
//using static WinCopies.Win32NativeInterop.NativeMethods;
//using static WinCopies.Util.Util;
//// using static WinCopies.Win32Interop.Icon.Icon;

//namespace WinCopies.Win32Interop.Icon
//{

//    using System;
//    using System.Collections.Generic;
//    using System.Collections.ObjectModel;
//    using System.Drawing;
//    using System.IO;
//    using System.Runtime.InteropServices;
//    using System.Windows.Media;
//    using WinCopies.Win32NativeInterop;

//    /// <summary>
//    /// Internals are mostly from here: http://www.codeproject.com/Articles/2532/Obtaining-and-managing-file-and-folder-icons-using
//    /// Caches all results.
//    /// </summary>
//    public class Icon : IDisposable
//    {

//        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
//        private const int MAX_PATH = 260;
//        /// <summary>Maximal Length of unmanaged Typename</summary>
//        private const int MAX_TYPE = 80;

//        public const int FILE_ATTRIBUTE_NORMAL = 0x80;

//        private static readonly Dictionary<string, Icon> _smallIconCache = new Dictionary<string, Icon>();
//        private static readonly Dictionary<string, Icon> _mediumIconCache = new Dictionary<string, Icon>();
//        private static readonly Dictionary<string, Icon> _largeIconCache = new Dictionary<string, Icon>();
//        private static readonly Dictionary<string, Icon> _extraLargeIconCache = new Dictionary<string, Icon>();

//        public static ReadOnlyDictionary<string, Icon> SmallIconCache { get; } = new ReadOnlyDictionary<string, Icon>(_smallIconCache);
//        public static ReadOnlyDictionary<string, Icon> MediumIconCache { get; } = new ReadOnlyDictionary<string, Icon>(_mediumIconCache);
//        public static ReadOnlyDictionary<string, Icon> LargeIconCache { get; } = new ReadOnlyDictionary<string, Icon>(_largeIconCache);
//        public static ReadOnlyDictionary<string, Icon> ExtraLargeIconCache { get; } = new ReadOnlyDictionary<string, Icon>(_extraLargeIconCache);

//        public string Extension { get; }

//        public IntPtr Ptr { get; }

//        private Icon(string extension, IntPtr ptr)

//        {

//            Extension = extension;

//            Ptr = ptr;

//        }

//        public bool TryRemoveFromCache() => _smallIconCache.Remove(Extension) | _mediumIconCache.Remove(Extension) | _largeIconCache.Remove(Extension) | _extraLargeIconCache.Remove(Extension);

//        public static Guid IID_IImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

//        /*
// * @method
// *        getFileIcon()
// * @param
// *        int       : icon type--FILE_ICON_SMALL                     0     16x16
// *                                         FILE_ICON_MEDIUM                  1     32x32
// *                                         FILE_ICON_LARGE                      2      48x48
// *                                         FILE_ICON_EXTRALARGE        3      256x256
// */
//        /// <summary>
//        /// Get associated file icon from the extension name
//        /// </summary>
//        /// <param name="ext"></param>
//        /// <param name="type"></param>
//        /// <param name="linkOverlay">Whether to include the link icon</param>
//        /// <returns>the return icon</returns>
//        public static Icon getFileIcon(string ext, IconSize type, bool linkOverlay, bool useCache)
//        {

//            Dictionary<string, Icon> cache = null;

//            if (useCache)

//            {

//                switch (type)

//                {

//                    case IconSize.Small:

//                        cache = _smallIconCache;

//                        break;

//                    case IconSize.Medium:

//                        cache = _mediumIconCache;

//                        break;

//                    case IconSize.Large:

//                        cache = _largeIconCache;

//                        break;

//                    case IconSize.ExtraLarge:

//                        cache = _extraLargeIconCache;

//                        break;

//                }

//                if (cache.TryGetValue(ext, out Icon icon))

//                    return icon;

//            }

//            IntPtr hIcon;
//            SHFILEINFO sfi = new SHFILEINFO();
//            SHGFI flag = SHGFI.Icon | SHGFI.UseFileAttributes;

//            switch (type)
//            {
//                case IconSize.Small: flag |= SHGFI.SmallIcon; break;
//                case IconSize.Medium: flag |= SHGFI.LargeIcon; break;
//                case IconSize.Large:
//                case IconSize.ExtraLarge: flag |= SHGFI.SysIconIndex; break;
//            }

//            if (linkOverlay) flag |= SHGFI.LinkOverlay;

//            HResult hr = (HResult)Marshal.ReadInt32(SHGetFileInfo(ext.ToLowerInvariant(), FILE_ATTRIBUTE_NORMAL, ref sfi, (uint)Marshal.SizeOf(sfi), flag));
//            //if (hr == HResult.Ok)
//            //{
//            if (type == IconSize.Large || type == IconSize.ExtraLarge)
//            {
//                // Retrieve the system image list.
//                IImageList imageList;
//                hr = SHGetImageList(((type == IconSize.ExtraLarge) ? SHIL.JUMBO : SHIL.EXTRALARGE), ref IID_IImageList, out imageList);

//                if (hr == HResult.Ok)

//                {

//                    // Get the icon we need from the list. Note that the HIMAGELIST we retrieved
//                    // earlier needs to be casted to the IImageList interface before use.
//                    hr = ((IImageList)imageList).GetIcon(sfi.iIcon, ILD_FLAGS.ILD_TRANSPARENT, out hIcon);

//                    if (hr != HResult.Ok)

//                        throw new Win32Exception((int)hr);

//                }

//                else

//                    throw new Win32Exception((int)hr);
//            }
//            else
//            {
//                hIcon = sfi.hIcon;
//            }
//            //}

//            //else

//            //    throw new Win32Exception((int)hr);

//            Icon _icon = new Icon(ext, hIcon);

//            if (useCache)

//                cache.Add(ext, _icon);

//            return _icon;
//        }

//        ///// <returns>null if path is null, otherwise - an icon</returns>
//        //public static ImageSource FindIconForFilename(string fileName, bool large)
//        //{
//        //    // string extension = Path.GetExtension(fileName);
//        //    // todo: exception
//        //    // if (extension == null)
//        //    // return null;
//        //    ImageSource icon;
//        //    //icon = IconReader.GetFileIcon(fileName, large ? IconSize.Large : IconSize.Small, false).ToImageSource();

//        //    return icon;
//        //}
//        ///// <summary>
//        ///// http://stackoverflow.com/a/6580799/1943849
//        ///// </summary>
//        //static ImageSource ToImageSource(this Icon icon)
//        //{
//        //    var imageSource = Imaging.CreateBitmapSourceFromHIcon(
//        //        icon.Handle,
//        //        Int32Rect.Empty,
//        //        BitmapSizeOptions.FromEmptyOptions());
//        //    return imageSource;
//        //}

//        public void Dispose()

//        {

//            TryRemoveFromCache();

//            DestroyIcon(Ptr);     // Cleanup

//        }

//        ///// <summary>
//        ///// Provides static methods to read system icons for both folders and files.
//        ///// </summary>
//        ///// <example>
//        ///// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
//        ///// </example>
//        //static class IconReader
//        //{
//        //    ///// <summary>
//        //    ///// Returns an icon for a given file - indicated by the name parameter.
//        //    ///// </summary>
//        //    ///// <param name="name">Pathname for file.</param>
//        //    ///// <param name="size">Large or small</param>
//        //    //public static Icon GetFileIcon(string name, IconSize size
//        //    //{
//        //    //    /* Check the size specified for return. */
//        //    //    //if (IconSize.Small == size)
//        //    //    //    flags += Shell32.ShgfiSmallicon;
//        //    //    //else
//        //    //    //    flags += Shell32.ShgfiLargeicon;
//        //    //    //Shell32.SHGetFileInfo(name,
//        //    //    //    Shell32.FileAttributeNormal,
//        //    //    //    ref shfi,
//        //    //    //    (uint)Marshal.SizeOf(shfi),
//        //    //    //    flags);
//        //    //    // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
//        //    //    Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
//        //    //    
//        //    //    return icon;
//        //    //}
//        //}
//        private const int MaxPath = 256;
//        ///// <summary>
//        ///// Wraps necessary Shell32.dll structures and functions required to retrieve Icon Handles using SHGetFileInfo. Code
//        ///// courtesy of MSDN Cold Rooster Consulting case study.
//        ///// </summary>
//        //static class Shell32
//        //{
//        //    //[StructLayout(LayoutKind.Sequential)]
//        //    //public struct Shfileinfo
//        //    //{
//        //    //    private const int Namesize = 80;
//        //    //    public readonly IntPtr hIcon;
//        //    //    private readonly int iIcon;
//        //    //    private readonly uint dwAttributes;
//        //    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
//        //    //    private readonly string szDisplayName;
//        //    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Namesize)]
//        //    //    private readonly string szTypeName;
//        //    //};
//        //    //public const uint ShgfiIcon = 0x000000100;     // get icon
//        //    //public const uint ShgfiLinkoverlay = 0x000008000;     // put a link overlay on icon
//        //    //public const uint ShgfiLargeicon = 0x000000000;     // get large icon
//        //    //public const uint ShgfiSmallicon = 0x000000001;     // get small icon
//        //    //public const uint ShgfiUsefileattributes = 0x000000010;     // use passed dwFileAttribute
//        //    //public const uint FileAttributeNormal = 0x00000080;
//        //    //[DllImport("Shell32.dll")]
//        //    //public static extern IntPtr SHGetFileInfo(
//        //    //    string pszPath,
//        //    //    uint dwFileAttributes,
//        //    //    ref Shfileinfo psfi,
//        //    //    uint cbFileInfo,
//        //    //    uint uFlags
//        //    //    );
//        //}
//    }

//    public enum IconSize
//    {
//        Small = 0,
//        Medium = 1,
//        Large = 2,
//        ExtraLarge = 3
//    }


//}
