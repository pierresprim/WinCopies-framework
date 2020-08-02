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
using System.Collections.Generic;

using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{
    public abstract class PathInfoPathCollection : PathCollection<IPathInfo>
    {
        protected override Func<IPathInfo> GetNewEmptyEnumeratorPathInfoDelegate { get; }

        protected override Func<IPathInfo, IPathInfo> GetNewEnumeratorPathInfoDelegate { get; } = current => current;

        public override Func<IPathInfo, Size?> GetPathSizeDelegate { get; } = path => path.Size;

        public PathInfoPathCollection(in string path) : this(path, new List<IPathInfo>()) { }

        public PathInfoPathCollection(string path, in IList<IPathInfo> list) : base(path, list) => GetNewEmptyEnumeratorPathInfoDelegate = () => new PathInfo(Path, null);
    }
}
