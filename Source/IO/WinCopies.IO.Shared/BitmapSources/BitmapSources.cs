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

using System;
using System.Drawing;
using System.Windows.Media.Imaging;

using WinCopies.Desktop;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    /// <summary>
    /// Represents a <see cref="BitmapSource"/>s provider for a GUI.
    /// </summary>
    public interface IBitmapSources : DotNetFix.IDisposable
    {
        /// <summary>
        /// Gets the small <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Small { get; }

        /// <summary>
        /// Gets the medium <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Medium { get; }

        /// <summary>
        /// Gets the large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource Large { get; }

        /// <summary>
        /// Gets the extra large <see cref="BitmapSource"/> of the object related to the current instance.
        /// </summary>
        BitmapSource ExtraLarge { get; }
    }

    public abstract class BitmapSources : IBitmapSources
    {
        public bool IsDisposed { get; private set; }

        protected abstract BitmapSource SmallOverride { get; }

        protected abstract BitmapSource MediumOverride { get; }

        protected abstract BitmapSource LargeOverride { get; }

        protected abstract BitmapSource ExtraLargeOverride { get; }



        public BitmapSource Small => GetValueIfNotDisposed(() => SmallOverride);

        public BitmapSource Medium => GetValueIfNotDisposed(() => MediumOverride);

        public BitmapSource Large => GetValueIfNotDisposed(() => LargeOverride);

        public BitmapSource ExtraLarge => GetValueIfNotDisposed(() => ExtraLargeOverride);

        protected TParam GetValueIfNotDisposed<TParam>(in Func<TParam> func) => IsDisposed ? throw GetExceptionForDispose(false) : func();

        protected virtual void Dispose(bool disposing) => IsDisposed = true;

        public void Dispose()
        {
            if (IsDisposed)

                return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~BitmapSources() => Dispose(false);
    }

    public abstract class BitmapSources<T> : BitmapSources
    {
        private T _innerObject;

        public T InnerObject => IsDisposed ? throw GetExceptionForDispose(false) : _innerObject;

        public BitmapSources(in T browsableObjectInfo) => _innerObject = browsableObjectInfo;

        protected override void Dispose(bool disposing)
        {
            _innerObject = default;

            base.Dispose(disposing);
        }
    }

    public class BrowsableObjectInfoBitmapBitmapSources : BitmapSources<Bitmap>
    {
        protected override BitmapSource SmallOverride => InnerObject.ToImageSource();

        protected override BitmapSource MediumOverride => InnerObject.ToImageSource();

        protected override BitmapSource LargeOverride => InnerObject.ToImageSource();

        protected override BitmapSource ExtraLargeOverride => InnerObject.ToImageSource();

        public BrowsableObjectInfoBitmapBitmapSources(in Bitmap bitmap) : base(bitmap) { /* Left empty. */ }
    }
}
