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
using System.Globalization;

using WinCopies.Util.Data;
using WinCopies.Collections.DotNetFix
#if WinCopies3
    .Generic
#endif
    ;
using WinCopies.Linq;
using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util;

using static WinCopies.Util.ThrowHelper
#endif
    ;
using System.Diagnostics;
using WinCopies.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Linq;

using WinCopies.Collections;
using System.Windows.Markup;
using System.Text;

#if !WinCopies3
using System.Collections;
using WinCopies.Util;
#endif

namespace WinCopies
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDescriptionAttribute : Attribute
    {
        public string FriendlyName { get; }

        public string Description { get; }

        public PropertyDescriptionAttribute(string friendlyName, string description)
        {
            ThrowIfNullEmptyOrWhiteSpace(friendlyName);
            ThrowIfNullEmptyOrWhiteSpace(description);

            FriendlyName = friendlyName;

            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyDescriptionFindingAttribute : Attribute
    {
        public Type PropertyDescriptionType { get; }

        public string NameFormat { get; }

        public string DescriptionFormat { get; }

        public PropertyDescriptionFindingAttribute(Type propertyDescriptionType, string nameFormat, string descriptionFormat)
        {
            ThrowIfNullEmptyOrWhiteSpace(nameFormat);
            ThrowIfNullEmptyOrWhiteSpace(descriptionFormat);

            PropertyDescriptionType = propertyDescriptionType;

            NameFormat = nameFormat;

            DescriptionFormat = descriptionFormat;
        }
    }

    public static class Extensions
    {
        public static
#if !WinCopies3
System.Collections.Generic.IEnumerator
#else
            IEnumeratorInfo2
#endif
            <TDestination> Select<TSource, TDestination>(this System.Collections.Generic.IEnumerator<TSource> enumerator, Converter<TSource, TDestination> func) => new SelectEnumerator<TSource, TDestination>(enumerator, value => func(value));
    }

    public static class Temp
    {
        public static System.Collections.Generic.IEnumerable<T> GetEmptyEnumerable<T>() => new WinCopies.Collections.Generic.Enumerable<T>(() => new EmptyEnumerator<T>());

        // Already implemented in WinCopies.Util.

        public static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => keyValuePair.Value;

        public interface IReadOnlyProperty
        {
            // bool IsEnabled { get; }

            string Name { get; }

            string DisplayName { get; }

            string Description { get; }

            string EditInvitation { get; }

            object PropertyGroup { get; }

            object Value { get; }

            Type Type { get; }

            // string GetDisplayGroupName();
        }

        public interface IProperty : IReadOnlyProperty
        {
            bool IsReadOnly { get; }
        }

        public interface IReadOnlyProperty<T> : IReadOnlyProperty
        {
            new T PropertyGroup { get; }
        }

        public interface IProperty<T> : IReadOnlyProperty<T>, IProperty
        {
            new T PropertyGroup { get; }
        }
    }
}
#endif
