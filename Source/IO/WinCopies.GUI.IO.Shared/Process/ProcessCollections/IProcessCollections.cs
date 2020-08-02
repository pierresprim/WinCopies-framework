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

using System.ComponentModel;

using WinCopies.Collections.DotNetFix;
using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{
    public interface IProcessCollection<T> : ICountableEnumerable<T>, INotifyPropertyChanged, INotifySimpleLinkedCollectionChanged<T> where T : IPathInfo
    {
        Size Size { get; }

        void Add(T path);

        T Remove();

        void Clear();

        T Peek();

        void DecrementSize(ulong sizeInBytes);
    }
    public interface IProcessCollection : IProcessCollection<IPathInfo>
    {
        // Left empty.
    }

    public interface IReadOnlyProcessCollection : ICountableEnumerable<IPathInfo>, INotifyPropertyChanged, INotifySimpleLinkedCollectionChanged<IPathInfo>
    {
        Size Size { get; }
    }

    public interface IProcessErrorPathCollection : IProcessCollection<IErrorPathInfo>
    {
        // Left empty.
    }

    public interface IReadOnlyProcessErrorPathCollection : ICountableEnumerable<IErrorPathInfo>, INotifyPropertyChanged, INotifySimpleLinkedCollectionChanged<IErrorPathInfo>
    {
        // Left empty.
    }
}
