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

using System;

using WinCopies.IO.ObjectModel;

namespace WinCopies.IO.Process
{
    public interface IProcessParameters
    {
        Guid Guid { get; }

        System.Collections.Generic.IEnumerable<string> Parameters { get; }
    }

    public class ProcessParameters : IProcessParameters
    {
        public Guid Guid { get; }

        public System.Collections.Generic.IEnumerable<string> Parameters { get; }

        public ProcessParameters(Guid guid, System.Collections.Generic.IEnumerable<string> parameters)
        {
            Guid = guid;
            Parameters = parameters;
        }

        public ProcessParameters(string guid, System.Collections.Generic.IEnumerable<string> parameters) : this(new Guid(guid), parameters) { /* Left empty. */ }
    }

    [Flags]
    public enum ProcessValidityScopeFlags
    {
        Global = 1,

        Local = 2
    }

    public interface IProcessInfo : IProcessFactoryProcessInfoBase
    {
        string GroupName { get; }

        string Name { get; }

        bool CanRun(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

        IProcessParameters GetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

        IProcessParameters TryGetProcessParameters(object parameter, IBrowsableObjectInfo sourcePath, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
    }

    public interface IProcessCommand : DotNetFix.IDisposable
    {
        string Name { get; }

        string Caption { get; }

        bool CanExecute(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items);

        bool TryExecute(string name, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items, out IProcessParameters result);

        IProcessParameters Execute(string name, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items);
    }

    public interface IProcessInfoBase
    {
        bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
    }
}
