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

using System.IO;
using WinCopies.Collections.Generic;

namespace WinCopies.IO.Enumeration
{
    public interface IEnumerablePath<T> : System.Collections.Generic.IEnumerable<T> where T : IPathInfo
    {
        IPathInfo Path { get; }

        FileSystemEntryEnumerationOrder EnumerationOrder { get; }

        System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            );

        System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            );

        System.Collections.Generic.IEnumerable<string> GetFileEnumerable(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
            , bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            );

#if !WinCopies3
        WinCopies.Collections.Generic.IDisposableEnumeratorInfo
#else
        Collections.Generic.IEnumeratorInfo2
#endif
            <T> GetEnumerator(string searchPattern, SearchOption? searchOption
#if CS8
            , EnumerationOptions enumerationOptions
#endif
                , FileSystemEntryEnumerationOrder enumerationOrder, RecursiveEnumerationOrder recursiveEnumerationOrder, bool safeEnumeration
            //#if DEBUG
            //            , FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            );
    }
}
