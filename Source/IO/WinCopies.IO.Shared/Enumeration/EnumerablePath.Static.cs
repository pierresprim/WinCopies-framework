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

namespace WinCopies.IO.Enumeration
{

    public static class EnumerablePath
    {
        public static System.Collections.Generic.IEnumerable<string> GetFileSystemEntryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
            //#if DEBUG
            //            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path) : System.IO.Directory.EnumerateFileSystemEntries(path);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.All);
                //#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
                //#if DEBUG
                //{
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern);
            //#if DEBUG

            //                else

            //                    return simulationParameters.EnumerateFunc(path, PathType.All);
            //}
            //#endif

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.All);
                //#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)

                //#endif
                return safeEnumeration ? Directory.EnumerateFileSystemEntriesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption.Value);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.All);
                //#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetDirectoryEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
            //#if DEBUG
            //            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path) : System.IO.Directory.EnumerateDirectories(path);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
                //#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
            {
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateDirectories(path, searchPattern);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
                //#endif
            }

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
                //#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)

                //#endif
                return safeEnumeration ? Directory.EnumerateDirectoriesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption.Value);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Directories);
                //#endif
            }
        }

        public static System.Collections.Generic.IEnumerable<string> GetFileEnumerable(in string path, string searchPattern, in SearchOption? searchOption
#if CS8
            , in EnumerationOptions enumerationOptions
#endif
            , in bool safeEnumeration
            //#if DEBUG
            //            , in FileSystemEntryEnumeratorProcessSimulation simulationParameters
            //#endif
            )
        {
            if (string.IsNullOrEmpty(searchPattern) && searchOption == null
#if CS8
                     && enumerationOptions == null
#endif
                    )
            {
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFilesIOSafe(path) : System.IO.Directory.EnumerateFiles(path);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Files);
                //#endif
            }

            else if (searchPattern != null && searchOption == null
#if CS8
                    && enumerationOptions == null
#endif
                    )
                //#if DEBUG
                //{
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern) : System.IO.Directory.EnumerateFiles(path, searchPattern);
            //#if DEBUG

            //                else

            //                    return simulationParameters.EnumerateFunc(path, PathType.Files);
            //}
            //#endif

#if CS8
            else if (searchOption == null)
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)
                //#endif
                return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern, enumerationOptions) : System.IO.Directory.EnumerateFiles(path, searchPattern, enumerationOptions);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Files);
                //#endif
            }
#endif

            else
            {
                if (searchPattern == null)

                    searchPattern = "";
                //#if DEBUG
                //                if (simulationParameters == null)

                //#endif
                return safeEnumeration ? Directory.EnumerateFilesIOSafe(path, searchPattern, searchOption.Value) : System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption.Value);
                //#if DEBUG

                //                else

                //                    return simulationParameters.EnumerateFunc(path, PathType.Files);
                //#endif
            }
        }
    }
}
