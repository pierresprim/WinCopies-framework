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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Reflection;

using WinCopies.Collections;
using WinCopies.IO.Reflection;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public class DotNetAssemblyInfo : ShellObjectInfo, IDotNetAssemblyInfo<IFileSystemObjectInfoProperties>
    {
        public override bool IsBrowsable { get; } = true;

        public sealed override bool IsRecursivelyBrowsable { get; } = false;

        public Assembly Assembly { get; }

        Assembly IEncapsulatorBrowsableObjectInfo<Assembly>.EncapsulatedObject => Assembly;

        protected DotNetAssemblyInfo(in string path, in ShellObject shellObject) : base(path, FileType.File, shellObject, null) => Assembly = Assembly.LoadFrom(path);

        public static DotNetAssemblyInfo From(in ShellObject shellObject)
        {
            ShellObjectInitInfo initInfo = ShellObjectInfo.GetInitInfo(shellObject);

            return initInfo.FileType == FileType.File ? new DotNetAssemblyInfo(initInfo.Path, shellObject) : throw new ArgumentException($"{nameof(shellObject)} is not a file.");
        }

        public override IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => throw new NotSupportedException();

        public override IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ShellObjectInfoEnumeratorStruct> func) => throw new NotSupportedException();

        public override IEnumerable<IBrowsableObjectInfo> GetItems() => GetItems(new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate }, null);

        public virtual IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoEnumeratorStruct> func) => new Enumerable<IDotNetItemInfo>(() => new DotNetNamespaceInfoEnumerator(this, Assembly.DefinedTypes, typesToEnumerate, func));
    }
}
