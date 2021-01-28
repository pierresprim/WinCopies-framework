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

#if WinCopies3

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using WinCopies.Collections.Generic;
using WinCopies.Linq;
using static WinCopies.Temp;

namespace WinCopies.IO.PropertySystem
{
    public abstract class PropertySystemCollection : IPropertySystemCollection
    {
        public abstract IProperty this[int index] { get; }

        public abstract IProperty this[PropertyId key] { get; }

        public abstract int Count { get; }

        public abstract System.Collections.Generic.IReadOnlyList<PropertyId> Keys { get; }

        public abstract System.Collections.Generic.IReadOnlyList<IProperty> Values { get; }

        System.Collections.Generic.IEnumerable<PropertyId> IReadOnlyDictionary<PropertyId, IProperty>.Keys => Keys;

        System.Collections.Generic.IEnumerable<IProperty> IReadOnlyDictionary<PropertyId, IProperty>.Values => Values;

        public bool ContainsKey(PropertyId key) => Keys.Contains(key);

        public bool SupportsReversedEnumeration => true;

        public abstract IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetKeyValuePairEnumerator();

        public abstract IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> GetReversedKeyValuePairEnumerator();

        public IEnumeratorInfo2<IProperty> GetEnumerator() => GetKeyValuePairEnumerator().Select(Temp.GetValue);

        public IEnumeratorInfo2<IProperty> GetReversedEnumerator() => GetReversedKeyValuePairEnumerator().Select(Temp.GetValue);

        IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> IEnumerableInfo<KeyValuePair<PropertyId, IProperty>>.GetEnumerator() => GetKeyValuePairEnumerator();

        IEnumeratorInfo2<KeyValuePair<PropertyId, IProperty>> IEnumerableInfo<KeyValuePair<PropertyId, IProperty>>.GetReversedEnumerator() => GetReversedKeyValuePairEnumerator();

        IEnumerator<KeyValuePair<PropertyId, IProperty>> System.Collections.Generic.IEnumerable<KeyValuePair<PropertyId, IProperty>>.GetEnumerator() => GetKeyValuePairEnumerator();

        IEnumerator<KeyValuePair<PropertyId, IProperty>> Collections.Generic.IEnumerable<KeyValuePair<PropertyId, IProperty>>.GetReversedEnumerator() => GetReversedKeyValuePairEnumerator();

        IEnumerator<IProperty> System.Collections.Generic.IEnumerable<IProperty>.GetEnumerator() => GetEnumerator();

        IEnumerator<IProperty> Collections.Generic.IEnumerable<IProperty>.GetReversedEnumerator() => GetReversedEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetValue(PropertyId key,
#if CS8
            [MaybeNullWhen(false)]
#endif
        out IProperty value) => (value = new Enumerable<KeyValuePair<PropertyId, IProperty>>(() => GetKeyValuePairEnumerator()).FirstOrDefault(keyValuePair => keyValuePair.Key == key).Value) != null;
    }
}

#endif
