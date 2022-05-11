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

using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;
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

        protected T InnerObject => IsDisposed ? throw GetExceptionForDispose(false) : _innerObject;

        public BitmapSources(in T obj) => _innerObject = obj;

        protected override void Dispose(bool disposing)
        {
            _innerObject = default;

            base.Dispose(disposing);
        }
    }

    public struct BitmapSourcesStruct
    {
        public BitmapSource Small;
        public BitmapSource Medium;
        public BitmapSource Large;
        public BitmapSource ExtraLarge;

        public void Dispose()
        {
            Small = null;
            Medium = null;
            Large = null;
            ExtraLarge = null;
        }
    }

    public abstract class BitmapSources2<T> : BitmapSources<T>
    {
        private BitmapSourcesStruct _bitmapSources;

        protected sealed override BitmapSource SmallOverride => _bitmapSources.Small
#if CS8
            ??=
#else
            ?? (_bitmapSources.Small =
#endif
            GetSmall()
#if !CS8
            )
#endif
            ;

        protected sealed override BitmapSource MediumOverride => _bitmapSources.Medium
#if CS8
            ??=
#else
            ?? (_bitmapSources.Medium =
#endif
         GetMedium()
#if !CS8
            )
#endif
            ;

        protected sealed override BitmapSource LargeOverride => _bitmapSources.Large
#if CS8
            ??=
#else
            ?? (_bitmapSources.Large =
#endif
         GetLarge()
#if !CS8
            )
#endif
            ;

        protected sealed override BitmapSource ExtraLargeOverride => _bitmapSources.ExtraLarge
#if CS8
            ??=
#else
            ?? (_bitmapSources.ExtraLarge =
#endif
         GetExtraLarge()
#if !CS8
            )
#endif
            ;

        public BitmapSources2(in T obj) : base(obj) { /* Left empty. */ }

        public BitmapSources2(in T obj, in BitmapSourcesStruct bitmapSources) : base(obj) => _bitmapSources = bitmapSources;

        protected abstract BitmapSource GetSmall();

        protected abstract BitmapSource GetMedium();

        protected abstract BitmapSource GetLarge();

        protected abstract BitmapSource GetExtraLarge();

        protected override void Dispose(bool disposing)
        {
            _bitmapSources.Dispose();

            base.Dispose(disposing);
        }
    }

    public class IconBitmapSources : BitmapSources2<Icon>
    {
        public IconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }

        protected virtual BitmapSource TryGetBitmapSource(in ushort size) => ObjectModel.BrowsableObjectInfo.TryGetBitmapSource(InnerObject, size);

        protected override BitmapSource GetSmall() => TryGetBitmapSource(SmallIconSize);

        protected override BitmapSource GetMedium() => TryGetBitmapSource(MediumIconSize);

        protected override BitmapSource GetLarge() => TryGetBitmapSource(LargeIconSize);

        protected override BitmapSource GetExtraLarge() => TryGetBitmapSource(ExtraLargeIconSize);
    }

    public class BitmapBitmapSources : BitmapSources2<Bitmap>
    {
        public BitmapBitmapSources(in Bitmap bitmap) : base(bitmap) { /* Left empty. */ }

        protected override BitmapSource GetSmall() => InnerObject.ToImageSource();

        protected override BitmapSource GetMedium() => InnerObject.ToImageSource();

        protected override BitmapSource GetLarge() => InnerObject.ToImageSource();

        protected override BitmapSource GetExtraLarge() => InnerObject.ToImageSource();
    }
}
