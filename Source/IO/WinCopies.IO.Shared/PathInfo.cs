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
using System.Diagnostics.CodeAnalysis;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public interface IPathCommon : IEquatable<IPathCommon>
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

    public static class PathHelper
    {
        public static string ToString(in IPathCommon path) => (path ?? throw GetArgumentNullException(nameof(path))).Path;

        private static bool _Equals(in IPathCommon path, in IPathCommon other) => path.Path == other.Path;

        public static bool Equals(in IPathCommon path, in IPathCommon other) => other != null && _Equals(path, other);

        public static bool Equals(in IPathCommon path, in object obj) => obj is IPathCommon other && _Equals(path, other);

        public static int GetHashCode(in IPathCommon path) => (path ?? throw GetArgumentNullException(nameof(path))).Path.GetHashCode();
    }

    public abstract class PathInfoBase
    {
        private NullableGeneric<Size?> _size;

        public string RelativePath { get; }

        public bool IsDirectory { get; }

        public abstract string Path { get; }

        public Size? Size
        {
            get
            {
                if (IsDirectory)

                    return null;

                if (_size == null)

                    try
                    {
                        _size = new NullableGeneric<Size?>(new Size((ulong)new System.IO.FileInfo(Path).Length));
                    }

                    catch
                    {
                        _size = new NullableGeneric<Size?>(null);
                    }

                return _size.Value;
            }
        }

        public PathInfoBase(in string relativePath, in bool isDirectory)
        {
            ThrowIfNullEmptyOrWhiteSpace(relativePath, nameof(relativePath));

            RelativePath = relativePath;

            IsDirectory = isDirectory;
        }
    }

    public static class PathTypes<T> where T : IPathCommon
    {
        public interface IPathCommon : IO.IPathCommon
        {
            new NullableGeneric<T> Parent { get; }

#if CS8
            IO.IPathCommon IO.IPathCommon.Parent => Parent == null ?
#if CS9
                null
#else
                default
#endif
                : Parent.Value;
#endif
        }

        public interface IPathInfo : IO.IPathInfo, IPathCommon
        {
            // Left empty.
        }

        public class PathInfoCommon : IPathCommon
        {
            private string _path;

            public NullableGeneric<T> Parent { get; }

            public string RelativePath { get; }

            public string Path => _path
#if CS8
            ??=
#else
                ?? (_path =
#endif
                this.GetPath(false)
#if !CS8
                )
#endif
                ;

            public PathInfoCommon(in string relativePath, in T parent)
            {
                Parent = new NullableGeneric<T>(parent);

                RelativePath = relativePath;
            }

            public override string ToString() => PathHelper.ToString(this);

            public bool Equals(IO.IPathCommon other) => PathHelper.Equals(this, other);

            public override bool Equals(object obj) => PathHelper.Equals(this, obj);

            public override int GetHashCode() => PathHelper.GetHashCode(this);

#if !CS8
            IO.IPathCommon IO.IPathCommon.Parent => Parent == null ? default : Parent.Value;
#endif
        }

        public abstract class PathInfoBase : IO.PathInfoBase, IPathInfo
        {
            public abstract NullableGeneric<T> Parent { get; }

            public PathInfoBase(in string relativePath, in bool isDirectory) : base(relativePath, isDirectory) { /* Left empty. */ }

            public override int GetHashCode() => PathHelper.GetHashCode(this);

            public override string ToString() => PathHelper.ToString(this);

            public bool Equals(
#if CS8
                [AllowNull]
#endif
            IO.IPathCommon other) => PathHelper.Equals(this, other);

            public override bool Equals(object obj) => PathHelper.Equals(this, obj);

#if !CS8
            IO.IPathCommon IO.IPathCommon.Parent => Parent.Value;
#endif
        }

        public class PathInfo : PathInfoBase
        {
            public override NullableGeneric<T> Parent { get; }

            public override string Path => this.GetPath(false);

            public PathInfo(in string relativePath, in T parent, in bool isDirectory) : base(relativePath, isDirectory) => Parent = new NullableGeneric<T>(parent);

            public PathInfo(in string relativePath, in T parent) : this(relativePath, parent, GetIsDirectory(new PathInfoCommon(relativePath, parent).Path)) { /* Left empty. */ }

            public PathInfo(in IPathCommon path, in bool isDirectory) : this((path ?? throw GetArgumentNullException(nameof(path))).RelativePath, GetParentPathOrThrowIfInvalid(path), isDirectory)
            {
                // Left empty.
            }

            public PathInfo(in IPathCommon path) : this((path ?? throw GetArgumentNullException(nameof(path))).RelativePath, GetParentPathOrThrowIfInvalid(path), GetIsDirectory(path.Path))
            {
                // Left empty.
            }

            public static T GetParentPathOrThrowIfInvalid(in IPathCommon path) => path.Parent == null ? default : path.Parent.GetPathOrThrowIfInvalid(nameof(path));

            internal static bool GetIsDirectory(in string path)
            {
                try
                {
                    return System.IO.Directory.Exists(path);
                }

                catch
                {
                    return false;
                }
            }
        }

        public class RootPath : PathInfoBase
        {
            public override NullableGeneric<T> Parent => null;

            public override string Path => RelativePath;

            public RootPath(in string path, in bool isDirectory) : base(path, isDirectory) { /* Left empty. */ }

            public RootPath(in string path) : this(path, PathInfo.GetIsDirectory(path)) { /* Left empty. */ }
        }
    }
}
