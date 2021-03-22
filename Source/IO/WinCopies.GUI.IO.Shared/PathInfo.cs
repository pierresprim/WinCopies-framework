///* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Size = WinCopies.IO.Size;

//namespace WinCopies.GUI.IO
//{
//    public interface IPathInfo : WinCopies.IO.IPathInfo
//    {
//        Size? Size { get; }
//    }

//    namespace Process
//    {
//        public interface IErrorPathInfo : IPathInfo
//        {
//            IPathInfo PathInfo { get; }

//            ProcessError Error { get; }
//        }

//        public readonly struct PathInfo : IPathInfo
//        {
//            public string Path { get; }

//            public Size? Size { get; }

//            public bool IsDirectory => !Size.HasValue;

//            public PathInfo(string path, Size? size)
//            {
//                Path = path;

//                Size = size;
//            }
//        }

//        public readonly struct ErrorPathInfo : IErrorPathInfo
//        {
//            public IPathInfo PathInfo { get; }

//            public ProcessError Error { get; }

//            public ErrorPathInfo(IPathInfo path, ProcessError error)
//            {
//                PathInfo = path;

//                Error = error;
//            }

//            #region IPathInfo implementation
//            string WinCopies.IO.IPathInfo.Path => PathInfo.Path;

//            bool WinCopies.IO.IPathInfo.IsDirectory => PathInfo.IsDirectory;

//            Size? IPathInfo.Size => PathInfo.Size;
//            #endregion
//        }
//    }
//}
