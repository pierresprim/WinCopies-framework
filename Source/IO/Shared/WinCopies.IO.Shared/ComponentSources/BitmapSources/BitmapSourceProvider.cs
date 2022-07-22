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

using WinCopies.IO.ComponentSources.Bitmap;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    namespace ComponentSources.Bitmap
    {
        public interface IBitmapSourceProvider : DotNetFix.IDisposable
        {
            IBitmapSources
#if CS8
                    ?
#endif
                    Default
            { get; }

            IBitmapSources
#if CS8
                    ?
#endif
                    Intermediate
            { get; }

            IBitmapSources
#if CS8
                    ?
#endif
                    Sources
            { get; }
        }

        public abstract class BitmapSourceProviderAbstract : IBitmapSourceProvider
        {
            protected abstract IBitmapSources
#if CS8
                    ?
#endif
                    DefaultOverride
            { get; }

            protected abstract IBitmapSources
#if CS8
                    ?
#endif
                    IntermediateOverride
            { get; }

            protected abstract IBitmapSources
#if CS8
                    ?
#endif
                    SourcesOverride
            { get; }

            public IBitmapSources
#if CS8
                    ?
#endif
                    Default => GetOrThrowIfDisposed(this, DefaultOverride);

            public IBitmapSources
#if CS8
                    ?
#endif
                    Intermediate => GetOrThrowIfDisposed(this, IntermediateOverride);

            public IBitmapSources
#if CS8
                    ?
#endif
                    Sources => GetOrThrowIfDisposed(this, SourcesOverride);

            protected abstract bool DisposeBitmapSources { get; }

            public bool IsDisposed { get; private set; }

            protected virtual void DisposeManaged() => IsDisposed = true;

            protected virtual void DisposeUnmanaged()
            {
                if (DisposeBitmapSources)
                {
                    DefaultOverride?.Dispose();

                    IntermediateOverride?.Dispose();

                    SourcesOverride?.Dispose();
                }
            }

            public void Dispose()
            {
                if (IsDisposed)

                    return;

                DisposeManaged();

                DisposeUnmanaged();

                GC.SuppressFinalize(this);
            }

            ~BitmapSourceProviderAbstract() => DisposeUnmanaged();
        }

        public class BitmapSourceProvider : BitmapSourceProviderAbstract
        {
            private IBitmapSources
#if CS8
                    ?
#endif
                    _default;
            private IBitmapSources
#if CS8
                    ?
#endif
                    _intermediate;
            private IBitmapSources
#if CS8
                    ?
#endif
                    _sources;

            protected override IBitmapSources
#if CS8
                    ?
#endif
                    DefaultOverride => _default;

            protected override IBitmapSources
#if CS8
                    ?
#endif
                    IntermediateOverride => _intermediate;

            protected override IBitmapSources
#if CS8
                    ?
#endif
                    SourcesOverride => _sources;

            protected override bool DisposeBitmapSources { get; }

            public BitmapSourceProvider(in IBitmapSources
#if CS8
                    ?
#endif
                    @default, in IBitmapSources
#if CS8
                    ?
#endif
                    intermediate, in IBitmapSources
#if CS8
                    ?
#endif
                    sources, in bool disposeBitmapSources)
            {
                _default = @default;
                _intermediate = intermediate;
                _sources = sources;

                DisposeBitmapSources = disposeBitmapSources;
            }

            protected override void DisposeUnmanaged()
            {
                base.DisposeUnmanaged();

                _default = null;
                _intermediate = null;
                _sources = null;
            }
        }
    }

    namespace Shell.ComponentSources.Bitmap
    {
        public class BitmapSourceProvider : IO.ComponentSources.Bitmap.BitmapSourceProvider
        {
            private static IBitmapSources GetDefaultBitmapSources(in BrowsableAs browsableAs)
            {
#if CS8
                return
#else
                switch (
#endif
                    browsableAs
#if CS8
                    switch
#else
                    )
#endif
                    {
#if !CS8
                    case
#endif
                        BrowsableAs.File
#if CS8
                        =>
#else
                        :
                        return
#endif
                        Icons.File.Instance
#if CS8
                        ,
#else
                        ;
                    case
#endif
                        BrowsableAs.Folder
#if CS8
                        =>
#else
                        :
                        return
#endif
                        Icons.Folder.Instance
#if CS8
                        ,
#else
                        ;
                    case
#endif
                        BrowsableAs.LocalRoot
#if CS8
                        =>
#else
                        :
                        return
#endif
                        Icons.Computer.Instance
#if CS8
                        ,
                        _ =>
#else
                        ;
                    default:
#endif
                        throw new InvalidEnumArgumentException(nameof(BrowsableAs), browsableAs)
#if CS8
                    };
#else
                    ;
                }
#endif
            }

            public BitmapSourceProvider(in BrowsableAs browsableAs, in IBitmapSources intermediate, in IBitmapSources
#if CS8
                ?
#endif
                sources, in bool disposeBitmapSources) : base(GetDefaultBitmapSources(browsableAs), intermediate, sources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo, in IBitmapSources intermediate, in IBitmapSources
#if CS8
                ?
#endif
                bitmapSources, in bool disposeBitmapSources) : this(browsableObjectInfo.GetBrowsableAsValue(), intermediate, bitmapSources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo, in IBitmapSources
#if CS8
                ?
#endif
                bitmapSources, in bool disposeBitmapSources) : this(browsableObjectInfo.GetBrowsableAsValue(), null, bitmapSources, disposeBitmapSources) { /* Left empty. */ }

            public BitmapSourceProvider(in IBrowsableObjectInfo browsableObjectInfo) : this(browsableObjectInfo, null, false) { /* Left empty. */ }
        }
    }
}
