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
using System.Linq;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    /// <summary>
    /// Indicates the browsing ability for an <see cref="IBrowsableObjectInfo"/>.
    /// </summary>
    public enum Browsability : byte
    {
        /// <summary>
        /// The item is not browsable.
        /// </summary>
        NotBrowsable = 0,

        /// <summary>
        /// The item is browsable by default.
        /// </summary>
        BrowsableByDefault = 1,

        /// <summary>
        /// The item is browsable but browsing should not be the default action to perform on this item.
        /// </summary>
        Browsable = 2,

        /// <summary>
        /// The item redirects to an other item, with this item browsable.
        /// </summary>
        RedirectsToBrowsableItem = 3
    }

    public interface IBrowsabilityPathBase
    {
        string Name { get; }
    }

    public interface IBrowsabilityPath : IBrowsabilityPathBase
    {
        IBrowsableObjectInfo GetPath();

        bool IsValid();
    }

    public interface IBrowsabilityPath<in T> : IBrowsabilityPathBase where T : IBrowsableObjectInfo
    {
        IBrowsableObjectInfo GetPath(T browsableObjectInfo);

        bool IsValidFor(T browsableObjectInfo);
    }

    public sealed class BrowsabilityPath<T> : IBrowsabilityPath, DotNetFix.IDisposable where T : IBrowsableObjectInfo
    {
        private IBrowsabilityPath<T> _browsabilityPath;
        private T _browsableObjectInfo;

        public bool IsDisposed { get; private set; }

        private IBrowsabilityPath<T> _BrowsabilityPath => IsDisposed ? throw GetExceptionForDispose(false) : _browsabilityPath;

        string IBrowsabilityPathBase.Name => _BrowsabilityPath.Name;

        public BrowsabilityPath(in IBrowsabilityPath<T> browsabilityPath, in T browsableObjectInfo)
        {
            _browsabilityPath = browsabilityPath;

            _browsableObjectInfo = browsableObjectInfo;
        }

        IBrowsableObjectInfo IBrowsabilityPath.GetPath() => _BrowsabilityPath.GetPath(_browsableObjectInfo);

        bool IBrowsabilityPath.IsValid() => _BrowsabilityPath.IsValidFor(_browsableObjectInfo);

        public void Dispose()
        {
            if (IsDisposed)

                return;

            _browsabilityPath = null;

            _browsableObjectInfo = default;

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }

        ~BrowsabilityPath() => Dispose();
    }

    public interface IBrowsabilityPathStack<T> : IBrowsableObjectInfoStack<IBrowsabilityPath<T>> where T : IBrowsableObjectInfo
    {
        // Left empty.
    }

    public sealed class BrowsabilityPathStack<T> : BrowsableObjectInfoEnumerableStack<IBrowsabilityPath<T>>, IBrowsabilityPathStack<T> where T : IBrowsableObjectInfo
    {
        public BrowsabilityPathStack(in IEnumerableStack<IBrowsabilityPath<T>> stack) : base(stack) { }

        public BrowsabilityPathStack() : this(new EnumerableStack<IBrowsabilityPath<T>>()) { /* Left empty. */ }

        public System.Collections.Generic.IEnumerable<IBrowsabilityPath> GetBrowsabilityPaths(T browsableObjectInfo) => Stack.Select(item => new BrowsabilityPath<T>(item, browsableObjectInfo));

        public WriteOnlyBrowsabilityPathStack<T> AsWriteOnly() => new
#if !CS9
            WriteOnlyBrowsabilityPathStack<T>
#endif
            (Stack);
    }

    public sealed class WriteOnlyBrowsabilityPathStack<T> : BrowsableObjectInfoStack<IBrowsabilityPath<T>, IEnumerableStack<IBrowsabilityPath<T>>>, IBrowsabilityPathStack<T> where T : IBrowsableObjectInfo
    {
        public WriteOnlyBrowsabilityPathStack(in IEnumerableStack<IBrowsabilityPath<T>> stack) : base(stack) { }
    }

    public interface IBrowsabilityOptions
    {
        Browsability Browsability { get; }

        IBrowsableObjectInfo RedirectToBrowsableItem();
    }

    public static class BrowsabilityOptions
    {
        private class _Browsability : IBrowsabilityOptions
        {
            public Browsability Browsability { get; }

            internal _Browsability(Browsability browsability) => Browsability = browsability;

            IBrowsableObjectInfo IBrowsabilityOptions.RedirectToBrowsableItem() => null;
        }

        public static IBrowsabilityOptions NotBrowsable { get; } = new _Browsability(Browsability.NotBrowsable);

        public static IBrowsabilityOptions BrowsableByDefault { get; } = new _Browsability(Browsability.BrowsableByDefault);

        public static IBrowsabilityOptions Browsable { get; } = new _Browsability(Browsability.Browsable);

        public static bool IsBrowsable(this Browsability browsability)
#if CS8
            => browsability switch
            {
#if CS9
                Browsability.BrowsableByDefault or Browsability.Browsable => true,
#else
                Browsability.BrowsableByDefault => true,
                Browsability.Browsable => true,
#endif
                _ => false,
            };
#else
        {
            switch (browsability)
            {
                case Browsability.BrowsableByDefault:
                case Browsability.Browsable:

                    return true;
            }

            return false;
        }
#endif
    }
}
