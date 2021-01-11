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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using static WinCopies.Temp;

namespace WinCopies.IO.PropertySystem
{

    public struct PropertyId : IEquatable<PropertyId>
    {
        public string Name { get; }

        public ShellPropertyGroup PropertyGroup { get; }

        public PropertyId(in string name, in ShellPropertyGroup propertyGroup)
        {
            Name = name;

            PropertyGroup = propertyGroup;
        }

        public bool Equals(
#if CS8
            [AllowNull]
        #endif
        PropertyId other) => other.PropertyGroup == PropertyGroup && other.Name == Name;

        public override bool Equals(object obj) => obj is PropertyId _obj && Equals(_obj);

        public override int GetHashCode() => PropertyGroup.GetHashCode() ^ Name.GetHashCode(
            #if CS8
             StringComparison.CurrentCulture
            #endif
            );

        public override string ToString() => $"{PropertyGroup}.{Name}";

        public static bool operator ==(PropertyId x, PropertyId y) => x.Equals(y);

        public static bool operator !=(PropertyId x, PropertyId y) => !(x == y);
    }

    public interface IPropertySystemCollection : IReadOnlyCollection<IProperty>, System.Collections.Generic.IReadOnlyList<IProperty>, IReadOnlyDictionary<PropertyId, IProperty>, ICountableEnumerable<IProperty>, IEnumerableInfo<KeyValuePair<PropertyId, IProperty>>, IEnumerableInfo<IProperty>
    {
        // Left empty.
    }
}

#endif
