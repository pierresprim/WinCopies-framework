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

using System.Diagnostics.CodeAnalysis;

namespace WinCopies.IO.Shell
{
    public interface IPathInfo : IO.IPathInfo
    {
        bool AlreadyPushed { get; }
    }

    public struct PathInfo : IPathInfo
    {
        public IO.IPathInfo Path { get; }

        public bool AlreadyPushed { get; internal set; }

        public string RelativePath => Path.RelativePath;

        public IPathCommon Parent => Path.Parent;

        string IPathCommon.Path => Path.Path;

        public bool IsDirectory => Path.IsDirectory;

        public Size? Size => Path.Size;

        public PathInfo(in IO.IPathInfo path)
        {
            Path = path;

            AlreadyPushed = false;
        }

        public bool Equals(
#if CS8
            [AllowNull]
#endif
        IPathCommon other) => Path.Equals(other);
    }
}
