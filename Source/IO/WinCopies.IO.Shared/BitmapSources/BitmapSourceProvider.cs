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

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public interface IBitmapSourceProvider : DotNetFix.IDisposable
    {
        IBitmapSources Default { get; }

        IBitmapSources Intermediate { get; }

        IBitmapSources Sources { get; }
    }

    public abstract class BitmapSourceProviderAbstract : IBitmapSourceProvider
    {
        protected abstract IBitmapSources DefaultOverride { get; }

        protected abstract IBitmapSources IntermediateOverride { get; }

        protected abstract IBitmapSources SourcesOverride { get; }

        public IBitmapSources Default => GetOrThrowIfDisposed(this, DefaultOverride);

        public IBitmapSources Intermediate => GetOrThrowIfDisposed(this, IntermediateOverride);

        public IBitmapSources Sources => GetOrThrowIfDisposed(this, SourcesOverride);

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
        private IBitmapSources _default;
        private IBitmapSources _intermediate;
        private IBitmapSources _sources;

        protected override IBitmapSources DefaultOverride => _default;

        protected override IBitmapSources IntermediateOverride => _intermediate;

        protected override IBitmapSources SourcesOverride => _sources;

        protected override bool DisposeBitmapSources { get; }

        public BitmapSourceProvider(in IBitmapSources @default, in IBitmapSources intermediate, in IBitmapSources sources, in bool disposeBitmapSources)
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
