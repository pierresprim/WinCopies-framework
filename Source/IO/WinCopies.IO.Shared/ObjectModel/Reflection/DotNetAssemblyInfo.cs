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

using WinCopies.Collections.Generic;
using WinCopies.IO.AbstractionInterop.Reflection;
using WinCopies.IO.Enumeration;
using WinCopies.IO.Enumeration.Reflection;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Reflection;
using WinCopies.IO.Selectors;

using static WinCopies.IO.DotNetAssemblyInfoHandledExtensions;
using static WinCopies.UtilHelpers;

namespace WinCopies.IO
{
    public static class DotNetAssemblyInfoHandledExtensions
    {
        public const string Exe = ".exe";
        public const string Dll = ".dll";
    }
}

namespace WinCopies.IO.ObjectModel.Reflection
{
    public class DotNetAssemblyInfo : ShellObjectInfo<IFileSystemObjectInfoProperties2, DotNetNamespaceInfoItemProvider, IBrowsableObjectInfoSelectorDictionary<DotNetNamespaceInfoItemProvider>, DotNetNamespaceInfoItemProvider>, IDotNetAssemblyInfo // <IFileSystemObjectInfoProperties>
    {
        #region Properties
        public override Predicate<DotNetNamespaceInfoItemProvider> RootItemsPredicate => item => !IsNullEmptyOrWhiteSpace(item.NamespaceName);

        public override Predicate<IBrowsableObjectInfo> RootItemsBrowsableObjectInfoPredicate => null;

        public static Action RegisterSelectors { get; private set; } = () =>
        {
            ShellObjectInfo.DefaultItemSelectorDictionary.Push(item => item.ShellObject is ShellFile shellFile && shellFile.Path.EndsWith(Exe, Dll), _item => new DotNetAssemblyInfo(ShellObjectInfo.GetInitInfo(_item.ShellObject).Path, _item.ShellObject, _item.ClientVersion.Value));

            RegisterSelectors = () => { /* Left empty. */ };
        };

        public override bool IsBrowsable => true;

        public sealed override bool IsRecursivelyBrowsable => false;

        public override bool IsBrowsableByDefault => false;

        public Assembly Assembly { get; private set; }

        public override IFileSystemObjectInfoProperties2 ObjectPropertiesGeneric => ;
        #endregion

        protected DotNetAssemblyInfo(in string path, in ShellObject shellObject, in ClientVersion clientVersion) : base(path, shellObject, clientVersion) { /* Left empty. */ }

        #region Methods
        public static DotNetAssemblyInfo From(in ShellObject shellObject, in ClientVersion clientVersion)
        {
            ShellObjectInitInfo initInfo = ShellObjectInfo.GetInitInfo(shellObject);

            return initInfo.FileType == FileType.File ? initInfo.Path.EndsWith(".exe", ".dll") ? new DotNetAssemblyInfo(initInfo.Path, shellObject, clientVersion) : throw new ArgumentException($"{nameof(shellObject)} must be an exe (.exe) or a dll (.dll).") : throw new ArgumentException($"{nameof(shellObject)} is not a file.");
        }

        public override IBrowsableObjectInfoSelectorDictionary<DotNetNamespaceInfoItemProvider> GetSelectorDictionary() => DefaultItemSelectorDictionary;

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

        public static System.Collections.Generic.IEnumerable<DotNetItemType> GetDefaultItemTypes() => new DotNetItemType[] { DotNetItemType.Namespace, DotNetItemType.Struct, DotNetItemType.Enum, DotNetItemType.Class, DotNetItemType.Interface, DotNetItemType.Delegate };

        protected virtual System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(System.Collections.Generic.IEnumerable<DotNetItemType> typesToEnumerate, Predicate<DotNetNamespaceInfoItemProvider> func) => DotNetNamespaceInfoEnumeration.From(this, typesToEnumerate, func);

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders(Predicate<DotNetNamespaceInfoItemProvider> predicate) => GetItemProviders(GetDefaultItemTypes(), predicate);

        protected override System.Collections.Generic.IEnumerable<DotNetNamespaceInfoItemProvider> GetItemProviders() => GetItemProviders(GetDefaultItemTypes(), null);
        #endregion
    }
}
