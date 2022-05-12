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
using System.Windows.Media.Imaging;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.Util;

using static WinCopies.ThrowHelper;
using static WinCopies.IO.ObjectModel.BrowsableObjectInfo;

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
        public static string GetCanonicalName(this ExtensionCommand command)
        {
            const string COPY = "Copy";

            string valueName =
#if CS9
                Enum.GetName(
#endif
                command
#if CS9
                )
#else
                .GetName()
#endif
                ;

            return valueName != null && valueName.StartsWith(COPY) ? COPY.ToLower() : null;
        }

        public static BrowsableAs GetBrowsableAsValue(this IBrowsableObjectInfo browsableObjectInfo) => browsableObjectInfo.IsBrowsable() ? browsableObjectInfo.IsLocalRoot ? BrowsableAs.LocalRoot : BrowsableAs.Folder : BrowsableAs.File;

        public static BitmapSource TryGetBitmapSource(this IBitmapSources bitmapSources, in int size)
#if CS8
            =>
#else
        {
            ThrowIfNull(
#endif
            bitmapSources
#if CS8
            == null ? throw GetArgumentNullException(
#else
            ,
#endif
                nameof(bitmapSources))
#if CS8
            : size
#else
            ;
#endif
            switch
#if CS8
            {
#else
                (size)
                {
                    case
#endif
                SmallIconSize
#if CS8
                =>
#else
                : return
#endif
                bitmapSources.Small
#if CS8
                ,
#else
                    ; case
#endif
                MediumIconSize
#if CS8
                =>
#else
                : return
#endif
               bitmapSources.Medium
#if CS8
                ,
#else
                    ; case
#endif
                LargeIconSize
#if CS8
                =>
#else
                : return
#endif
               bitmapSources.Large
#if CS8
                ,
#else
                    ; case
#endif
                ExtraLargeIconSize
#if CS8
                =>
#else
                : return
#endif
               bitmapSources.ExtraLarge
#if CS8
                ,

                _ =>
#else
                    ; default:

                        return
#endif
                null
#if CS8
            };
#else
                ;
            }
#endif
#if !CS8
        }
#endif

        public static string GetPath(this IPathCommon path, in bool ignoreRoot)
        {
            ThrowIfNull(path, nameof(path));

            var merger = new ArrayMerger<char>();

            IPathCommon _path = path;

            while ((_path = _path.Parent) != null)
            {
                _ = merger.AddFirst(new UIntCountableEnumerable<char, IUIntCountableEnumerator<char>>(RepeatEnumerator<char>.Get('\\', 1)));

                _ = merger.AddFirst(new Collections.Generic.UIntCountableEnumerable<char>(new StringCharArray(_path.RelativePath)));
            }

            if (ignoreRoot && merger.Count > 0)
            {
                merger.RemoveFirst();

                merger.RemoveFirst();
            }

            _ = merger.AddLast(new Collections.Generic.UIntCountableEnumerable<char>(new StringCharArray(path.RelativePath)));

            char[] result = merger.ToArray();

            merger.Clear();

            return new string(result);
        }

        public static void ThrowIfInvalidPath<T>(this NullableGeneric<T> path, in string argumentName) where T : IPathCommon
        {
            if ((path ?? throw GetArgumentNullException(argumentName)).Value == null)

                throw new ArgumentException($"{argumentName} must have a non-null value.");
        }

        public static NullableGeneric<T> GetOrThrowIfInvalidPath<T>(this NullableGeneric<T> path, in string argumentName) where T : IPathCommon => (path ?? throw GetArgumentNullException(argumentName)).Value == null
                ? throw new ArgumentException($"{argumentName} must have a non-null value.")
                : path;

        public static T GetPathOrThrowIfInvalid<T>(this NullableGeneric<T> path, in string argumentName) where T : IPathCommon => path.GetOrThrowIfInvalidPath(argumentName).Value;
    }
}
