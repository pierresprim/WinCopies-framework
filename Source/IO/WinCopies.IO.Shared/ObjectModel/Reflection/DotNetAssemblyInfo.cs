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

using Microsoft.WindowsAPICodePack.PortableDevices;
using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Selectors;

namespace WinCopies.IO.ObjectModel.Reflection
{
    public class DotNetAssemblyInfo : ShellObjectInfo<IFileSystemObjectInfoProperties2, DotNetNamespaceInfoEnumeratorStruct, IBrowsableObjectInfoSelectorDictionary<DotNetAssemblyInfoItemProvider>, DotNetAssemblyInfoItemProvider>, IDotNetAssemblyInfo // <IFileSystemObjectInfoProperties>
    {
        #region Properties
        public static IBrowsableObjectInfoSelectorDictionary<ShellObjectInfoItemProvider> DefaultItemSelectorDictionary { get; } = new ShellObjectInfoSelectorDictionary

        public static Action RegisterSelectors { get; private set; } = () =>
        {
            ShellObjectInfo.DefaultItemSelectorDictionary.Push(item => item.ShellObject is ShellFile shellFile && shellFile.Path.EndsWith(".exe", ".dll"), _item => new DotNetAssemblyInfo(ShellObjectInfo.GetInitInfo(_item.ShellObject).Path, _item.ShellObject, _item.ClientVersion.Value));

            RegisterSelectors = () => { /* Left empty. */ };
        };

        public override bool IsBrowsable => true;

        public sealed override bool IsRecursivelyBrowsable => false;

        public override bool IsBrowsableByDefault => false;

        public Assembly Assembly { get; private set; }

        public override IFileSystemObjectInfoProperties2 ObjectPropertiesGeneric => throw new NotImplementedException();
        #endregion

        protected DotNetAssemblyInfo(in string path, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, shellObject, clientVersion) { /* Left empty. */ }

        #region Methods
        public static DotNetAssemblyInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
        {
            ShellObjectInitInfo initInfo = ShellObjectInfo.GetInitInfo(shellObject);

            return initInfo.FileType == FileType.File ? initInfo.Path.EndsWith(".exe", ".dll") ? new DotNetAssemblyInfo(initInfo.Path, shellObject, clientVersion) : throw new ArgumentException($"{nameof(shellObject)} must be an exe (.exe) or a dll (.dll).") : throw new ArgumentException($"{nameof(shellObject)} is not a file.");
        }

        public override IBrowsableObjectInfoSelectorDictionary<DotNetAssemblyInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

        public void OpenAssembly()
        {
            try
            {
                Assembly = Assembly.LoadFrom(Path);
            }

            catch (Exception ex) when (ex.Is(false, typeof(FileNotFoundException), typeof(FileLoadException), typeof(BadImageFormatException), typeof(SecurityException)))
            {
                Assembly = null;
            }
        }

        public void CloseAssembly() => Assembly = null;

        protected override IEnumerable<DotNetAssemblyInfoItemProvider> GetItemProviders() => GetItemsOverride(new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate }, null);

        protected virtual IEnumerable<DotNetAssemblyInfoItemProvider> GetItemsOverride(IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoEnumeratorStruct> func) => new WinCopies.Collections.Generic.Enumerable<DotNetAssemblyInfoItemProvider>(() => new DotNetNamespaceInfoEnumerator(this, Assembly.DefinedTypes, typesToEnumerate, func));

        public IEnumerable<IBrowsableObjectInfo> GetItems(IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoEnumeratorStruct> func) => GetItems(GetItemsOverride(typesToEnumerate, func));

        public override IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ShellObjectInfoEnumeratorStruct> func) => Temp.GetEmptyEnumerable<IBrowsableObjectInfo>();

        public override IEnumerable<IBrowsableObjectInfo> GetItems(Predicate<ArchiveFileInfoEnumeratorStruct> func) => Temp.GetEmptyEnumerable<IBrowsableObjectInfo>();
        #endregion
    }
}
