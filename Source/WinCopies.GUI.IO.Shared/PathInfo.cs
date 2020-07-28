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

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO
{
    public interface IPathInfo : WinCopies.IO.IPathInfo
    {
        Size? Size { get; }
    }

    namespace Process
    {
        public interface IErrorPathInfo : IPathInfo
        {
            IPathInfo Path { get; }

            ProcessError Error { get; }
        }

        public readonly struct PathInfo : IPathInfo
        {
            public string Path { get; }

            public Size? Size { get; }

            public bool IsDirectory => !Size.HasValue;

            public PathInfo(string path, Size? size)
            {
                Path = path;

                Size = size;
            }
        }

        public struct ErrorPathInfo : IErrorPathInfo
        {
            public IPathInfo Path { get; }

            public ProcessError Error { get; }

            public ErrorPathInfo(IPathInfo path, ProcessError error)
            {
                Path = path;

                Error = error;
            }

            #region IPathInfo implementation
            string WinCopies.IO.IPathInfo.Path => Path.Path;

            bool WinCopies.IO.IPathInfo.IsDirectory => Path.IsDirectory;

            Size? IPathInfo.Size => Path.Size;
            #endregion
        }
    }
}
