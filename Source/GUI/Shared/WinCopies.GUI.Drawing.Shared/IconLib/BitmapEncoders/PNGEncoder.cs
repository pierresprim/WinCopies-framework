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
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace WinCopies.GUI.Drawing.BitmapEncoders
{
    [Author("Franco, Gustavo")]
    internal class PNGEncoder : ImageEncoder
    {
        #region Properties
        public override IconImageFormat IconImageFormat => IconImageFormat.PNG;

        public override unsafe int ImageSize
        {
            get
            {
                // This is a fast and temporary solution,
                // Soon Ill implement a png cache, 
                // then the image will be generated just once between calls and writes
                int length = 0;
                using (var ms = new MemoryStream())
                {
                    Icon.ToBitmap().Save(ms, ImageFormat.Png);
                    length = (int)ms.Length;
                }
                return length;
            }
        }
        #endregion

        #region Constructors
        public PNGEncoder() { }
        #endregion

        #region Methods
        public unsafe override void Read(in System.IO.Stream stream, in int resourceSize)
        {
            // Buffer a PNG image
            byte[] buffer = new byte[resourceSize];
            _ = stream.Read(buffer, 0, buffer.Length);
            var ms = new MemoryStream(buffer);
            var pngBitmap = new Bitmap(ms);

            // Set XOR and AND Image
            var iconImage = new IconImage();
            iconImage.Set(pngBitmap, null, Color.Transparent);
            pngBitmap.Dispose();

            //Transfer the data from the BMPEncoder to the PNGEncoder
            CopyFrom(iconImage.Encoder);
        }

        public override void Write(in System.IO.Stream stream)
        {
            var ms = new MemoryStream();
            Icon.ToBitmap().Save(ms, ImageFormat.Png);
            byte[] buffer = ms.GetBuffer();
            stream.Write(buffer, 0, (int)ms.Length);
        }
        #endregion
    }
}
