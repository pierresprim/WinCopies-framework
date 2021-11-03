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
using System.IO;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Enumeration
{
    public class MoveRecursivelyEnumerablePath<T> : RecursivelyEnumerablePath<T> where T : IPathInfo
    {
        protected class RecursiveEnumerator : RecursiveEnumeratorAbstract<T>
        {
            public RecursiveEnumerator(in System.Collections.Generic.IEnumerable<IRecursiveEnumerable<T>> enumerable) : base(enumerable, RecursiveEnumerationOrder.Both) { }

            public RecursiveEnumerator(in IRecursiveEnumerableProviderEnumerable<T> enumerable) : base(enumerable, RecursiveEnumerationOrder.Both) { }

            public RecursiveEnumerator(in System.Collections.Generic.IEnumerable<IRecursiveEnumerable<T>> enumerable, in IStack<RecursiveEnumeratorStruct<T>> stack) : base(enumerable, RecursiveEnumerationOrder.Both, stack) { }

            public RecursiveEnumerator(IRecursiveEnumerableProviderEnumerable<T> enumerable, in IStack<RecursiveEnumeratorStruct<T>> stack) : base(enumerable, RecursiveEnumerationOrder.Both, stack) { /* Left empty. */ }

            protected override bool AddAsDuplicate(T value) => value.IsDirectory;
        }

        public MoveRecursivelyEnumerablePath(in T path, string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, in Func<PathTypes<IPathInfo>.PathInfo, T> getNewPathInfoDelegate, bool safeEnumeration
           //#if DEBUG
           //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
           //#endif
           ) : base(path
#if CS8
            ??
#else
            == null ?
#endif
            throw GetArgumentNullException(nameof(path))
#if !CS8
            : path
#endif
            , searchPattern, searchOption
#if CS8
            , enumerationOptions
#endif
               , enumerationOrder, RecursiveEnumerationOrder.Both, getNewPathInfoDelegate, safeEnumeration
      //#if DEBUG
      //            , simulationParameters
      //#endif
      )
        { /* Left empty. */ }

        public override RecursiveEnumeratorAbstract<T> GetEnumerator() => new RecursiveEnumerator(this);
    }
}
