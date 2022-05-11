//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using WinCopies.GUI.Drawing.Exceptions;
using WinCopies.GUI.Drawing.EncodingFormats;
using WinCopies.GUI.Drawing.ColorProcessing;

using Microsoft.WindowsAPICodePack.Win32Native;

using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util
#endif
    ;

using Size = System.Drawing.Size;

namespace WinCopies.GUI.Drawing
{
    [Author("Franco, Gustavo")]
    public class SingleIcon : IEnumerable<IconImage>
    {
        #region Variables Declaration
        private string mName = "";
        private List<IconImage> mIconImages = new List<IconImage>();
        #endregion

        #region Constructors
        public SingleIcon(in string name) => mName = name;
        #endregion

        #region Properties
        public int Count => mIconImages.Count;

        public string Name
        {
            get => mName;
            set => mName = value ?? string.Empty;
        }

        public Icon Icon
        {
            get
            {
                if (mIconImages.Count == 0)

                    return null;

                var ms = new MemoryStream();
                Save(ms);
                ms.Position = 0;
                return new Icon(ms);
            }
        }
        #endregion

        #region Public Methods
        public void Clear() => mIconImages.Clear();

        public IconImage RemoveAt(in int index)
        {
            if (index < 0 || index >= mIconImages.Count)

                return null;

            IconImage iconImage = mIconImages[index];

            mIconImages.RemoveAt(index);

            return iconImage;
        }

        public IEnumerator<IconImage> GetEnumerator() => new Enumerator(this);

        public void Load(in string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                Load(fs);
            }
            finally
            {
                fs?.Close();
            }
        }

        public void Load(in System.IO.Stream stream)
        {
            ThrowIfNull(stream, nameof(stream));

            var iconFormat = new IconFormat();

            if (!iconFormat.IsRecognizedFormat(stream))

                throw new InvalidFileException();

            MultiIcon multiIcon = iconFormat.Load(stream);

            if (multiIcon.Count < 1)

                return;

            CopyFrom(multiIcon[0]);
        }

        public static IconImage GetIconImage(in System.IO.Stream stream)
        {
            ThrowIfNull(stream, nameof(stream));

            var iconFormat = new IconFormat();

            if (!iconFormat.IsRecognizedFormat(stream))

                throw new InvalidFileException();

            MultiIcon multiIcon = iconFormat.Load(stream);

            if (multiIcon.Count < 1)

                return null;

            return multiIcon[0].FirstOrDefault();
        }

        public static IconImage GetIconImage(in string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                return GetIconImage(fs);
            }
            finally
            {
                fs?.Close();
            }
        }

        public void Save(in string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

            try
            {
                Save(fs);
            }
            finally
            {
                fs?.Close();
            }
        }

        public void Save(in System.IO.Stream stream) => new IconFormat().Save(new MultiIcon(this), stream);

        public IconImage Add(in Bitmap bitmap)
        {
            if ((bitmap ?? throw GetArgumentNullException(nameof(bitmap))).PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
            {
                IconImage iconImage = Add(bitmap, null, Color.Transparent);

                if (bitmap.RawFormat.Guid == ImageFormat.Png.Guid)

                    iconImage.IconImageFormat = IconImageFormat.PNG;

                return iconImage;
            }

            return Add(bitmap, null, bitmap.GetPixel(0, 0));
        }

        public IconImage Add(in Bitmap bitmap, in Color transparentColor) => Add(bitmap, null, transparentColor);

        public IconImage Add(in Bitmap bitmap, in Bitmap bitmapMask) => Add(bitmap ?? throw GetArgumentNullException(nameof(bitmapMask)), bitmapMask, Color.Empty);

        public IconImage Add(in Icon icon)
        {
            if (!Core.GetIconInfo((icon ?? throw GetArgumentNullException(nameof(icon))).Handle, out IconInfo iconInfo))

                throw new InvalidMultiIconFileException();

            Bitmap XORImage = null;
            Bitmap ANDImage = null;

            try
            {
                XORImage = Image.FromHbitmap(iconInfo.hbmColor);
                ANDImage = Image.FromHbitmap(iconInfo.hbmMask);

                // Bitmap.FromHbitmap will give a DDB and not a DIB, if the screen is 16 bits Icon with 16bits are not supported
                // then make them XP format Icons
                if (Tools.BitsFromPixelFormat(XORImage.PixelFormat) == 16)
                {
                    XORImage.Dispose();
                    ANDImage.Dispose();

                    return Add(icon.ToBitmap(), Color.Transparent);
                }

                else

                    return Add(XORImage, ANDImage, Color.Empty);
            }
            finally
            {
                XORImage?.Dispose();
                ANDImage?.Dispose();
            }
        }

        //private Bitmap CreateSmoothBitmap(Bitmap bmp, int width, int height)
        //{
        //    Bitmap newBitmap = new Bitmap(width, height);
        //    Graphics g = Graphics.FromImage(newBitmap);
        //    g.CompositingQuality = CompositingQuality.HighQuality;
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    g.SmoothingMode = SmoothingMode.HighQuality;
        //    g.DrawImage(bmp, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
        //           new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
        //    g.Dispose();
        //    return newBitmap;
        //}

        public void CreateFrom(in string fileName) => CreateFrom(fileName, IconOutputFormat.FromWin95);

        public void CreateFrom(in string fileName, in IconOutputFormat format)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(fileName) ?? throw new InvalidFileException();

            try
            {
                CreateFrom(bmp, format);
            }
            finally
            {
                bmp.Dispose();
            }
        }

        public void CreateFrom(in Bitmap bitmap) => CreateFrom(bitmap, IconOutputFormat.FromWin95);

        public void CreateFrom(in Bitmap bitmap, in IconOutputFormat format)
        {
            if ((bitmap ?? throw GetArgumentNullException(nameof(bitmap))).PixelFormat != PixelFormat.Format32bppArgb)

                throw new InvalidPixelFormatException(PixelFormat.Undefined, PixelFormat.Format32bppArgb);

            Bitmap bmp;
            IconImage iconImage = null;
            IColorQuantizer colorQuantizer = new EuclideanQuantizer();

            mIconImages.Clear();

            // Vista
            // 256x256x32
            if ((format & IconOutputFormat.Vista) == IconOutputFormat.Vista)

                _ = Add(bitmap);

            if ((format & IconOutputFormat.WinXPUnpopular) == IconOutputFormat.WinXPUnpopular)

                using (bmp = new Bitmap(bitmap, 64, 64))

                    iconImage = Add(bmp); // XP

            using (bmp = new Bitmap(bitmap, 48, 48))
            {
                if ((format & IconOutputFormat.WinXP) == IconOutputFormat.WinXP)

                    iconImage = Add(bmp); // XP

                if ((format & IconOutputFormat.Win95) == IconOutputFormat.Win95)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format8bppIndexed), iconImage.Mask); // W95

                if ((format & IconOutputFormat.Win31) == IconOutputFormat.Win31)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format4bppIndexed), iconImage.Mask); // W95
            }

            using (bmp = new Bitmap(bitmap, 32, 32))
            {
                if ((format & IconOutputFormat.WinXP) == IconOutputFormat.WinXP)

                    iconImage = Add(bmp); // XP

                if ((format & IconOutputFormat.Win95) == IconOutputFormat.Win95)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format8bppIndexed), iconImage.Mask); // W95

                if ((format & IconOutputFormat.Win31) == IconOutputFormat.Win31)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format4bppIndexed), iconImage.Mask); // W31

                if ((format & IconOutputFormat.Win30) == IconOutputFormat.Win30)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format1bppIndexed), iconImage.Mask); // W30
            }

            using (bmp = new Bitmap(bitmap, 24, 24))
            {
                if ((format & IconOutputFormat.WinXPUnpopular) == IconOutputFormat.WinXPUnpopular)

                    iconImage = Add(bmp); // XP

                if ((format & IconOutputFormat.Win95Unpopular) == IconOutputFormat.Win95Unpopular)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format8bppIndexed), iconImage.Mask); // W95

                if ((format & IconOutputFormat.Win31Unpopular) == IconOutputFormat.Win31Unpopular)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format4bppIndexed), iconImage.Mask); // W31

                if ((format & IconOutputFormat.Win30) == IconOutputFormat.Win30)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format1bppIndexed), iconImage.Mask); // W30
            }

            using (bmp = new Bitmap(bitmap, 16, 16))
            {
                if ((format & IconOutputFormat.WinXP) == IconOutputFormat.WinXP)

                    iconImage = Add(bmp); // XP

                if ((format & IconOutputFormat.Win95) == IconOutputFormat.Win95)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format8bppIndexed), iconImage.Mask); // W95

                if ((format & IconOutputFormat.Win31) == IconOutputFormat.Win31)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format4bppIndexed), iconImage.Mask); // W31

                if ((format & IconOutputFormat.Win30) == IconOutputFormat.Win30)

                    _ = Add(colorQuantizer.Convert(bmp, PixelFormat.Format1bppIndexed), iconImage.Mask); // W30
            }
        }

        public IconImage Add(in IconImage iconImage)
        {
            mIconImages.Add(iconImage);

            return iconImage;
        }

        internal void CopyFrom(in SingleIcon singleIcon)
        {
            mName = singleIcon.mName;
            mIconImages = singleIcon.mIconImages;
        }
        #endregion

        #region Private Methods
        private unsafe IconImage Add(in Bitmap bitmap, in Bitmap bitmapMask, in Color transparentColor)
        {
            if (IndexOf((bitmap ?? throw GetArgumentNullException(nameof(bitmap))).Size, Tools.BitsFromPixelFormat(bitmap.PixelFormat)) != -1)

                throw new ImageAlreadyExistsException();

            if (bitmap.Width > 256 || bitmap.Height > 256)

                throw new ImageTooBigException();

            var iconImage = new IconImage();

            iconImage.Set(bitmap, bitmapMask, transparentColor);

            mIconImages.Add(iconImage);

            return iconImage;
        }

        private int IndexOf(in Size size, in int bitCount)
        {
            for (int i = 0; i < Count; i++)

                if (this[i].Size == size && Tools.BitsFromPixelFormat(this[i].PixelFormat) == bitCount)

                    return i;

            return -1;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Overrides
        public override string ToString() => Name;
        #endregion

        #region Indexers
        public IconImage this[in int index] => mIconImages[index];
        #endregion

        #region Helper Classes
        [Serializable]
        public
#if !WinCopies3
struct
#else
            class
#endif
            Enumerator :
#if !WinCopies3
            IEnumerator<IconImage>, IDisposable, IEnumerator
#else
            WinCopies.Collections.Generic.Enumerator<IconImage>
#endif
        {
            #region Variables Declaration
            private readonly SingleIcon mList;
            private int mIndex;
#if WinCopies3
            private IconImage _current = null;
#endif
            #endregion

            #region Constructors
            internal Enumerator(SingleIcon list)
            {
                mList = list;
                mIndex = 0;
#if !WinCopies3
                Current = null;
#endif
            }
            #endregion

            #region Properties
#if !WinCopies3
            public IconImage Current { get; private set; }

            object IEnumerator.Current => Current;
#else
            protected override IconImage CurrentOverride => _current;

            public override bool? IsResetSupported => true;
#endif
            #endregion

            #region Methods
#if !WinCopies3
            public void Dispose() { }

            public bool MoveNext()
#else
            protected override bool MoveNextOverride()
#endif
            {
                if (mIndex < mList.Count)
                {
#if !WinCopies3
                    Current
#else
                    _current
#endif
                        = mList[mIndex];
                    mIndex++;
                    return true;
                }

                mIndex = mList.Count + 1;

                ResetCurrent();

                return false;
            }

#if !WinCopies3
            private
#else
            protected override
#endif
                void ResetCurrent() =>
#if !WinCopies3
                Current
#else
                _current
#endif
                = null;

#if !WinCopies3
            void IEnumerator.Reset()
            {
                mIndex = 0;
                ResetCurrent();
            }
#else
            protected override void ResetOverride2() => mIndex = 0;
#endif
            #endregion
        }
        #endregion
    }
}
