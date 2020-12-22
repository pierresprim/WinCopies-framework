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

#if DEBUG
using System;
using System.Collections.Generic;

using WinCopies.Collections.DotNetFix
    #if WinCopies3
    .Generic
#endif
    ;
using WinCopies.Linq;

namespace WinCopies
{
    public static class Temp
    {
        public static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => keyValuePair.Value;

        public class ReadOnlyList<TSource, TDestination> : IReadOnlyList<TDestination>, ICountableEnumerable<TDestination>
        {
            protected IReadOnlyList<TSource> InnerList { get; }

            protected Func<TSource, TDestination> Selector { get; }

            public int Count => InnerList.Count;

            public TDestination this[int index] => Selector(InnerList[index]);

            public ReadOnlyList(IReadOnlyList<TSource> innerList, Func<TSource, TDestination> selector)
            {
                InnerList = innerList;

                Selector = selector;
            }

            public System.Collections.Generic.IEnumerator<TDestination> GetEnumerator() => InnerList.GetEnumerator().Select(Selector);

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
#endif
