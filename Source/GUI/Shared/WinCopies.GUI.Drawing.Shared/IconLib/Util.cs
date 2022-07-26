using Microsoft.WindowsAPICodePack.Win32Native.GDI;

using System;
using System.Runtime.InteropServices;

namespace WinCopies.GUI.Drawing
{
    internal static class Util
    {
        #region Methods
        public unsafe static void Read<T>(ref T @struct, in System.IO.Stream stream) where T : unmanaged
        {
            byte[] array = new byte[Marshal.SizeOf<T>()];
            _ = stream.Read(array, 0, array.Length);
            fixed (byte* pData = array)
                @struct = *(T*)pData;
        }

        public unsafe static void Write<T>(in T @struct, in System.IO.Stream stream) where T : unmanaged
        {
            byte[] array = new byte[Marshal.SizeOf<T>()];
            fixed (T* ptr = &@struct)
                Marshal.Copy((IntPtr)ptr, array, 0, Marshal.SizeOf<T>());
            stream.Write(array, 0, sizeof(T));
        }
        #endregion

        public static void Write<T>( in T[] structs, in System.IO.Stream stream) where T : unmanaged
        {
            foreach (T @struct in structs)
                Write(@struct, stream);
        }

        #region BitmapInfoHeader
        #region Constructors
        public static BitmapInfoHeader GetBitmapInfoHeader(System.IO.Stream stream)
        {
            var header = new BitmapInfoHeader();

            Read(ref header, stream);

            return header;
        }
        #endregion
        #endregion

        #region RGBQuad
        #region Methods
        public static void Set(this RGBQuad RGBQuad, byte r, byte g, byte b)
        {
            RGBQuad.rgbRed = r;
            RGBQuad.rgbGreen = g;
            RGBQuad.rgbBlue = b;
        }
        #endregion
        #endregion
    }
}
