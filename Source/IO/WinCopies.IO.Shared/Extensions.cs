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

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    // TODO:
    //#if !WinCopies4
    //    internal class CustomEnumeratorEnumerable<TItems, TEnumerator> : System.Collections.Generic.IEnumerable<TItems> where TEnumerator : System.Collections.Generic.IEnumerator<TItems>
    //    {
    //        protected TEnumerator InnerEnumerator { get; }

    //        public CustomEnumeratorEnumerable(TEnumerator enumerator) => InnerEnumerator = enumerator;

    //        public TEnumerator GetEnumerator() => InnerEnumerator;

    //        System.Collections.Generic.IEnumerator<TItems> System.Collections.Generic.IEnumerable<TItems>.GetEnumerator() => InnerEnumerator;

    //        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => InnerEnumerator;
    //    }

    //    internal class UIntCountableEnumerable<TItems, TEnumerator> : CustomEnumeratorEnumerable<TItems, TEnumerator>, IUIntCountableEnumerable<TItems> where TEnumerator : IUIntCountableEnumerator<TItems>
    //    {
    //        uint IUIntCountable.Count => InnerEnumerator.Count;

    //        public UIntCountableEnumerable(in TEnumerator enumerator) : base(enumerator) { /* Left empty. */ }

    //        IUIntCountableEnumerator<TItems> IUIntCountableEnumerable<TItems, IUIntCountableEnumerator<TItems>>.GetEnumerator() => GetEnumerator();

    //#if !CS8
    //        IUIntCountableEnumerator<TItems> Collections.Enumeration.DotNetFix.IEnumerable<IUIntCountableEnumerator<TItems>>.GetEnumerator() => GetEnumerator();

    //        IUIntCountableEnumerator<TItems> Collections.DotNetFix.Generic.IEnumerable<TItems, IUIntCountableEnumerator<TItems>>.GetEnumerator() => GetEnumerator();
    //#endif
    //    }
    //#endif

    public static class Extensions
    {
        public static string GetPath(this IPathCommon path, in bool ignoreRoot)
        {
            ThrowIfNull(path, nameof(path));

            var merger = new ArrayMerger<char>();

            IPathCommon _path = path;

            while ((_path = _path.Parent) != null)
            {
                _ = merger.AddFirst(new UIntCountableEnumerable<char, IUIntCountableEnumerator<char>>(RepeatEnumerator<char>.Get('\\', 1)));

                _ = merger.AddFirst(new UIntCountableEnumerable<char>(new StringCharArray(_path.RelativePath)));
            }

            if (ignoreRoot && merger.Count > 0)
            {
                merger.RemoveFirst();

                merger.RemoveFirst();
            }

            _ = merger.AddLast(new UIntCountableEnumerable<char>(new StringCharArray(path.RelativePath)));

            char[] result = merger.ToArray();

            merger.Clear();

            return new string(result);
        }

        public static void ThrowIfInvalidPath<T>(this NullableGeneric<T> path, in string argumentName) where T : IPathCommon
        {
            if ((path ?? throw GetArgumentNullException(argumentName)).Value == null)

                throw new ArgumentException($"{argumentName} must have a non-null value.");
        }

        public static NullableGeneric<T> GetOrThrowIfInvalidPath<T>(this NullableGeneric<T> path, in string argumentName) where T : IPathCommon
        {
            if ((path ?? throw GetArgumentNullException(argumentName)).Value == null)

                throw new ArgumentException($"{argumentName} must have a non-null value.");

            return path;
        }
    }
}
