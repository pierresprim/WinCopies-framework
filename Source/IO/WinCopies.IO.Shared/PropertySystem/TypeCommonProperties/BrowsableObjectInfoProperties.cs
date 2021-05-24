/* Copyright © Pierre Sprimont, 2020
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
using WinCopies.IO.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public static class IOHelper
    {
        public static ArgumentException GetInvalidInnerObjectException(in string expectedType, in string argumentName) => throw new ArgumentException(string.Format(Resources.ExceptionMessages.GivenItemInnerObjectIsNotFromSupportedType, expectedType), argumentName);
    }
}

namespace WinCopies.IO.PropertySystem
{
    public abstract class BrowsableObjectInfoProperties<T> : WinCopies.DotNetFix.IDisposable where T : IBrowsableObjectInfo
    {
        private T _browsableObjectInfo;

        public bool IsDisposed => _browsableObjectInfo == null;

        protected TValue GetValueIfNotDisposed<TValue>(in TValue value) => IsDisposed ? throw GetExceptionForDispose(false) : value;

        protected T BrowsableObjectInfo => GetValueIfNotDisposed(_browsableObjectInfo);

        protected BrowsableObjectInfoProperties(in T browsableObjectInfo) => _browsableObjectInfo = browsableObjectInfo
#if CS9
        ??
#else
            == null ? 
#endif
            throw GetArgumentNullException(nameof(browsableObjectInfo))
#if !CS9
: browsableObjectInfo
#endif
            ;

        protected virtual void DisposeManaged()
        {

        }

        protected virtual void DisposeUnmanaged()
        {
            _browsableObjectInfo = default;
        }

        public void Dispose()
        {
            if (IsDisposed)

                return;

            DisposeManaged();

            DisposeUnmanaged();

            GC.SuppressFinalize(this);
        }

        ~BrowsableObjectInfoProperties() => DisposeUnmanaged();
    }
}
