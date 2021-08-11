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

using SevenZip;

using System;
using System.Management;
using System.Security;

using WinCopies.IO.ObjectModel;

#if !WinCopies3
using System.Collections;
using System.Collections.Generic;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Util;

using static WinCopies.Util.Util;
#else
using WinCopies.Collections.Generic;
#endif

namespace WinCopies.IO.Enumeration
{
    //    public sealed class RecursiveSubEnumerator<T> : IEnumerator<T>
    //    {
    //        private IEnumerator<IEnumerator<T>> _enumerator;
    //        // private EmptyCheckEnumerator<string> _fileEnumerable;

    //        private bool _completed = false;

    //        private T _current;

    //        public T Current => IsDisposed ? throw GetExceptionForDispose(false) : _current;

    //        object IEnumerator.Current => Current;

    //#if DEBUG
    //        public IPathInfo PathInfo { get; }
    //#endif 

    //        public FileSystemEntryEnumerator(
    //#if DEBUG
    //            IPathInfo pathInfo,
    //#endif 
    //            System.Collections.Generic.IEnumerable<string> directoryEnumerable, System.Collections.Generic.IEnumerable<string> fileEnumerable)
    //        {
    //#if DEBUG
    //            ThrowIfNull(pathInfo, nameof(pathInfo));

    //            PathInfo = pathInfo;
    //#endif
    //            ThrowIfNull(directoryEnumerable, nameof(directoryEnumerable));
    //            ThrowIfNull(fileEnumerable, nameof(fileEnumerable));

    //            _directoryEnumerable = directoryEnumerable.GetEnumerator();

    //            _fileEnumerable = new EmptyCheckEnumerator<string>(fileEnumerable.GetEnumerator());
    //        }

    //        public bool MoveNext()
    //        {
    //            if (_completed) return false;

    //                if (_enumerator.MoveNext())
    //                {
    //                    _current = new PathInfo(_enumerator.Current, true);

    //                    return true;
    //                }

    //                _directoryEnumerable = null;

    //                _completed = true;

    //                return false;
    //        }

    //        public void Reset() => throw new NotSupportedException();

    //        public void Dispose()
    //        {
    //            if (_completed)

    //                _current = null;

    //            else
    //            {
    //                if (_directoryEnumerable != null)
    //                {
    //                    _directoryEnumerable.Dispose();

    //                    _directoryEnumerable = null;
    //                }

    //                if (_fileEnumerable != null)
    //                {
    //                    _fileEnumerable.Dispose();

    //                    _fileEnumerable = null;
    //                }
    //            }

    //            _current = null;

    //            IsDisposed = true;
    //        }

    //        public bool IsDisposed { get; private set; }
    //    }
}
