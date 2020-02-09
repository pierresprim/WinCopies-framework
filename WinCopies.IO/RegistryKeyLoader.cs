/* Copyright © Pierre Sprimont, 2019
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
using System.ComponentModel;
using System.Reflection;
using System.Security;

using Microsoft.Win32;
using WinCopies.Collections;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO
{

    [Flags]
    public enum RegistryItemTypes

    {

        None = 0,

        RegistryKey = 1,

        RegistryValue = 2

    }

    public interface IRegistryKeyLoader : IBrowsableObjectInfoLoader
    {

        RegistryItemTypes RegistryItemTypes { get; set; }

    }

    public class RegistryKeyLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory> : BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory>, IRegistryKeyLoader where TPath : RegistryItemInfo where TItems : RegistryItemInfo where TSubItems : RegistryItemInfo where TFactory : BrowsableObjectInfoFactory, IRegistryItemInfoFactory where TItemsFactory : BrowsableObjectInfoFactory, IRegistryItemInfoFactory
    {

        protected override BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> DeepCloneOverride() => new RegistryKeyLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>(default, RegistryItemTypes, (IFileSystemObjectComparer<IFileSystemObject>)FileSystemObjectComparer.DeepClone(), WorkerReportsProgress, WorkerSupportsCancellation);

        private readonly RegistryItemTypes _registryItemTypes = RegistryItemTypes.None;

        public RegistryItemTypes RegistryItemTypes
        {

            get => _registryItemTypes;

            set => _ = this.SetBackgroundWorkerProperty(nameof(RegistryItemTypes), nameof(_registryItemTypes), value, typeof(RegistryKeyLoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>), true);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryKeyLoader{TPath, TItems, TSubItems, TFactory, TItemsFactory}"/> class.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="registryItemTypes">The registry item types to load.</param>
        public RegistryKeyLoader(BrowsableObjectTreeNode<TPath, TItems, TFactory> path, RegistryItemTypes registryItemTypes, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, registryItemTypes, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) => RegistryItemTypes = registryItemTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryKeyLoader{TPath, TItems, TSubItems, TFactory, TItemsFactory}"/> class using a custom comparer.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
        /// <param name="registryItemTypes">The registry item types to load.</param>
        public RegistryKeyLoader(BrowsableObjectTreeNode<TPath, TItems, TFactory> path, RegistryItemTypes registryItemTypes, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer, bool workerReportsProgress, bool workerSupportsCancellation) : base(path, (IFileSystemObjectComparer<IFileSystemObject>)fileSystemObjectComparer, workerReportsProgress, workerSupportsCancellation) => _registryItemTypes = registryItemTypes;

        //public override bool CheckFilter(string path)

        //{

        //    if (Filter == null) return true;

        //    foreach (string filter in Filter)

        //    {

        //        bool checkFilters(string[] filters)

        //        {

        //            foreach (string _filter in filters)

        //            {

        //                if ( string.IsNullOrEmpty( _filter ) ) continue;

        //                if (path.Length >= _filter.Length && path.Contains(_filter))

        //                    path = path.Substring(path.IndexOf(_filter) + _filter.Length);

        //                else return false;

        //            }

        //            return true;

        //        }

        //        return checkFilters(filter.Split('*'));

        //    }

        //    return true;

        //}

        protected override void OnDoWork(DoWorkEventArgs e)
        {

            if (RegistryItemTypes == RegistryItemTypes.None)

                return;

            // todo: 'if' to remove if not necessary:

            // if (Path is IRegistryItemInfo registryItemInfo)

            // {

            var paths = new ArrayBuilder<PathInfo>();

            PathInfo pathInfo;

            void checkAndAppend(string pathWithoutName, string name, bool isValue)

            {

                string path = pathWithoutName + IO.Path.PathSeparator + name;

                if (CheckFilter(path))

                    _ = paths.AddLast(pathInfo = new PathInfo(path, path.RemoveAccents(), name, null, RegistryItemInfo.DefaultRegistryKeyDeepClone, isValue));

            }

            switch (Path.Value.RegistryItemType)

            {

                case RegistryItemType.RegistryRoot:

                    if (RegistryItemTypes.HasFlag(RegistryItemTypes.RegistryKey))

                    {

                        FieldInfo[] _registryKeyFields = typeof(Microsoft.Win32.Registry).GetFields();

                        string name;

                        foreach (FieldInfo fieldInfo in _registryKeyFields)

                        {

                            name = ((RegistryKey)fieldInfo.GetValue(null)).Name;

                            checkAndAppend(name, name, false);

                        }

                    }

                    break;

                case RegistryItemType.RegistryKey:

                    string[] items;

                    if (RegistryItemTypes.HasFlag(RegistryItemTypes.RegistryKey))

                        try

                        {

                            items = Path.Value.RegistryKey.GetSubKeyNames();

                            foreach (string item in items)

                                checkAndAppend(item.Substring(0, item.LastIndexOf(IO.Path.PathSeparator)), item.Substring(item.LastIndexOf(IO.Path.PathSeparator) + 1), false);

                        }

                        catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { }

                    if (RegistryItemTypes.HasFlag(RegistryItemTypes.RegistryValue))

                        try

                        {

                            items = Path.Value.RegistryKey.GetValueNames();

                            foreach (string item in items)

                                checkAndAppend(Path.Value.RegistryKey.Name, item, true);

                        }

                        catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { }

                    break;

            }



            IEnumerable<PathInfo> pathInfos;



            if (FileSystemObjectComparer == null)

                pathInfos = (IEnumerable<PathInfo>)paths;

            else

            {

                var _paths = paths.ToList();

                _paths.Sort(FileSystemObjectComparer);

                pathInfos = (IEnumerable<PathInfo>)_paths;

            }



            using (IEnumerator<PathInfo> pathsEnum = pathInfos.GetEnumerator())



                while (pathsEnum.MoveNext())

                    try

                    {

                        do

                            ReportProgress(0, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>((TItems)(pathsEnum.Current.IsValue ? ((IRegistryItemInfoFactory)Path.Factory).GetBrowsableObjectInfo(pathsEnum.Current.Path.Substring(0, pathsEnum.Current.Path.Length - pathsEnum.Current.Name.Length - 1 /* We remove one more character to remove the backslash between the registry key path and the registry key value name. */ ), pathsEnum.Current.Name) : Path.Factory.GetBrowsableObjectInfo(pathsEnum.Current.Path)), (TItemsFactory) Path.Factory.DeepClone()));

                        while (pathsEnum.MoveNext());

                    }

                    catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(IOException), typeof(UnauthorizedAccessException))) { }



            // }

        }

        protected class PathInfo : IO.PathInfo
        {

            /// <summary>
            /// Gets the localized name of this <see cref="PathInfo"/>.
            /// </summary>
            public override string LocalizedName => Name;

            /// <summary>
            /// Gets the name of this <see cref="PathInfo"/>.
            /// </summary>
            public override string Name { get; }

            public bool IsValue { get; }

            public RegistryKey RegistryKey { get; }

            public DeepClone<RegistryKey> RegistryKeyDelegate { get; }

            public PathInfo(string path, string normalizedPath, string name, RegistryKey registryKey, DeepClone<RegistryKey> registryKeyDelegate, bool isValue) : base(path, normalizedPath)
            {

                Name = name;

                RegistryKey = registryKey;

                RegistryKeyDelegate = registryKeyDelegate;

                IsValue = isValue;

            }
        }
    }
}
