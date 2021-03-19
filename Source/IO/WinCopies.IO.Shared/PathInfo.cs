﻿/* Copyright © Pierre Sprimont, 2020
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

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public interface IPathCommon
    {
        string RelativePath { get; }

        IPathCommon Parent { get; }

        string Path { get; }
    }

    public interface IPath
    {
        bool IsDirectory { get; }

        Size? Size { get; }
    }

    public interface IPathInfo : IPathCommon, IPath
    {
        // Left empty.
    }

    public static class PathTypes<T> where T : IPathCommon
    {
        public interface IPathCommon : IO.IPathCommon
        {
            new NullableGeneric<T> Parent { get; }

#if CS9
            IO.IPathCommon IO.IPathCommon.Parent => Parent == null ? null : Parent.Value;
#endif
        }

        public interface IPathInfo : IO.IPathInfo, IPathCommon
        {
            // Left empty.
        }

        public class PathInfoCommon : IPathCommon
        {
            public NullableGeneric<T> Parent { get; }

            public string RelativePath { get; }

            public string Path => this.GetPath(false);

            public PathInfoCommon(in string relativePath, in NullableGeneric<T> parent)
            {
                parent.ThrowIfInvalidPath(nameof(parent));

                Parent = parent;

                RelativePath = relativePath;
            }
        }

        public class PathInfoBase
        {
            public string RelativePath { get; }

            public bool IsDirectory { get; }

            public Size? Size { get; init; }

            public PathInfoBase(in string relativePath, in bool isDirectory)
            {
                ThrowIfNullEmptyOrWhiteSpace(relativePath, nameof(relativePath));

                RelativePath = relativePath;

                IsDirectory = isDirectory;
            }
        }

        public class PathInfo : PathInfoBase, IPathInfo
        {
            public NullableGeneric<T> Parent { get; }

            public string Path => this.GetPath(false);

            public PathInfo(in string relativePath, in NullableGeneric<T> parent, in bool isDirectory) : base(relativePath, isDirectory)
            {
                parent.ThrowIfInvalidPath(nameof(parent));

                Parent = parent;
            }

            public PathInfo(in IPathCommon path, in bool isDirectory) : this((path ?? throw GetArgumentNullException(nameof(path))).RelativePath, path.Parent, isDirectory)
            {
                // Left empty.
            }

#if !CS9
            IO.IPathCommon IO.IPathCommon.Parent => Parent;
#endif
        }

        public class RootPath : PathInfoBase, IPathInfo
        {
            public NullableGeneric<T> Parent => null;

            public string Path => RelativePath;

            public RootPath(in string path, in bool isDirectory) : base(path, isDirectory)
            {
                // Left empty.
            }
        }
    }
}