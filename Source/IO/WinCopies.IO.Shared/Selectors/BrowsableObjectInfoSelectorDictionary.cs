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
using System.Collections.Generic;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.Linq;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Selectors
{
    public interface IBrowsableObjectInfoSelectorDictionary<T> : WinCopies.DotNetFix.IDisposable
    {
        void Push(Predicate<T> predicate, Converter<T, IBrowsableObjectInfo> selector);

        Converter<T, IBrowsableObjectInfo> DefaultSelector { get; }

        IBrowsableObjectInfo Select(T item);

        System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> Select(System.Collections.Generic.IEnumerable<T> items);
    }

    public static class BrowsableObjectInfoSelectorDictionary
    {
        public static ArgumentException GetInvalidItemProviderException() => new ArgumentException(Resources.ExceptionMessages.ItemProviderNotSupported);
    }

    public abstract class BrowsableObjectInfoSelectorDictionary<T> : IBrowsableObjectInfoSelectorDictionary<T>
    {
        private EnumerableStack<KeyValuePair<Predicate<T>, Converter<T, IBrowsableObjectInfo>>> _stack;

        protected abstract Converter<T, IBrowsableObjectInfo> DefaultSelectorOverride { get; }

        public Converter<T, IBrowsableObjectInfo> DefaultSelector => IsDisposed ? throw GetExceptionForDispose(false) : DefaultSelectorOverride;

        public bool IsDisposed { get; private set; }

        public void Push(Predicate<T> predicate, Converter<T, IBrowsableObjectInfo> selector)
        {
            ThrowIfDisposed(this);

            _stack.Push(new KeyValuePair<Predicate<T>, Converter<T, IBrowsableObjectInfo>>(predicate ?? throw GetArgumentNullException(nameof(predicate)), selector ?? throw GetArgumentNullException(nameof(selector))));
        }

        private IBrowsableObjectInfo _Select(T item)
        {
            foreach (KeyValuePair<Predicate<T>, Converter<T, IBrowsableObjectInfo>> _item in _stack)

                if (_item.Key(item))

                    return _item.Value(item);

            return DefaultSelectorOverride(item);
        }

        public IBrowsableObjectInfo Select(T item)
        {
            ThrowIfDisposed(this);

            return _Select(item ?? throw GetArgumentNullException(nameof(item)));
        }

        public System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> Select(System.Collections.Generic.IEnumerable<T> items) => IsDisposed ? throw GetExceptionForDispose(false) : items.SelectConverter(_stack.Count == 0 ? DefaultSelectorOverride : _Select);

        /// <summary>
        /// This method is called by <see cref="Dispose"/> and by the deconstructor. This overload does nothing and it is not necessary to call this base overload in derived classes.
        /// </summary>
        protected virtual void DisposeUnmanaged() { /* Left empty. */ }

        protected virtual void DisposeManaged()
        {
            _stack.Clear();

            _stack = null;

            IsDisposed = true;
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            DisposeUnmanaged();

            DisposeManaged();
        }

        ~BrowsableObjectInfoSelectorDictionary() => DisposeUnmanaged();
    }
}
