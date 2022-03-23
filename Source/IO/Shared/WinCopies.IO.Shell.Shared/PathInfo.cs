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

#if CS8
using System.Diagnostics.CodeAnalysis;
#endif

namespace WinCopies.IO.Shell
{
    public interface IPathInfo : IO.IPathInfo
    {
        bool AlreadyPushed { get; }
    }

    public class PathInfo : IPathInfo
    {
        public IO.IPathInfo InnerPath { get; }

        public bool AlreadyPushed { get; internal set; }

        public string RelativePath => InnerPath.RelativePath;

        public IPathCommon Parent => InnerPath.Parent;

        public string Path => InnerPath.Path;

        public bool IsDirectory => InnerPath.IsDirectory;

        public Size? Size => InnerPath.Size;

        public PathInfo(in IO.IPathInfo path)
        {
            InnerPath = path;

            AlreadyPushed = false;
        }

        public bool Equals(
#if CS8
            [AllowNull]
#endif
        IPathCommon other) => InnerPath.Equals(other);
    }
}
