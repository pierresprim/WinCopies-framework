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
using System.Management;
using System.Threading;
using WinCopies.Collections;
using WinCopies.Util;

namespace WinCopies.IO
{
    public interface IWMILoader : IBrowsableObjectInfoLoader
    {

        /// <summary>
        /// Gets or sets the WMI item types to load.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="IWMILoader"/> is busy.</exception>
        WMIItemTypes WMIItemTypes { get; set; }

    }

    /// <summary>
    /// A class for easier <see cref="ManagementBaseObject"/> items loading.
    /// </summary>
    public class WMILoader<TPath, TItems, TSubItems, TFactory, TItemsFactory> : BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory>, IWMILoader where TPath : WMIItemInfo where TItems : WMIItemInfo where TSubItems : WMIItemInfo where TFactory : BrowsableObjectInfoFactory, IWMIItemInfoFactory where TItemsFactory : BrowsableObjectInfoFactory, IWMIItemInfoFactory
    {

        protected override BrowsableObjectInfoLoader<TPath, TItems, TSubItems, TFactory> DeepCloneOverride() => new WMILoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>(default, WMIItemTypes, (IFileSystemObjectComparer<IFileSystemObject>)FileSystemObjectComparer.DeepClone(), WorkerReportsProgress, WorkerSupportsCancellation);

        private readonly WMIItemTypes _wmiItemTypes;

        /// <summary>
        /// Gets or sets the WMI item types to load.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception thrown when this property is set while the <see cref="WMILoader{TPath, TItems, TSubItems, TFactory}"/> is busy.</exception>
        public WMIItemTypes WMIItemTypes { get => _wmiItemTypes; set => this.SetBackgroundWorkerProperty(nameof(WMIItemTypes), nameof(_wmiItemTypes), value, typeof(WMILoader<TPath, TItems, TSubItems, TFactory, TItemsFactory>), true); }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="wmiItemTypes">The WMI item types to load.</param>
        public WMILoader(BrowsableObjectTreeNode<TPath, TItems, TFactory> path, WMIItemTypes wmiItemTypes, bool workerReportsProgress, bool workerSupportsCancellation) : this(path, wmiItemTypes, new FileSystemObjectComparer<IFileSystemObject>(), workerReportsProgress, workerSupportsCancellation) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowsableObjectInfoLoader{TPath, TItems, TSubItems, TFactory}"/> class using a custom comparer.
        /// </summary>
        /// <param name="workerReportsProgress">Whether the thread can notify of the progress.</param>
        /// <param name="workerSupportsCancellation">Whether the thread supports the cancellation.</param>
        /// <param name="fileSystemObjectComparer">The comparer used to sort the loaded items.</param>
        /// <param name="wmiItemTypes">The WMI item types to load.</param>
        public WMILoader(BrowsableObjectTreeNode<TPath, TItems, TFactory> path, WMIItemTypes wmiItemTypes, IFileSystemObjectComparer<IFileSystemObject> fileSystemObjectComparer, bool workerReportsProgress, bool workerSupportsCancellation) : base(path, (IFileSystemObjectComparer<IFileSystemObject>)fileSystemObjectComparer, workerReportsProgress, workerSupportsCancellation) => _wmiItemTypes = wmiItemTypes;

        //public override bool CheckFilter(string path) => throw new NotImplementedException();

        protected override void OnDoWork(DoWorkEventArgs e)
        {

            var paths = new ArrayBuilder<PathInfo>();

            string _path;

            bool dispose = false;

            // #pragma warning disable IDE0019 // Pattern Matching
            var managementClass = Path.Value.ManagementObject as ManagementClass;
            // #pragma warning restore IDE0019 // Pattern Matching

            if (managementClass == null)

            {

                dispose = true;

                // #pragma warning disable IDE0067 // Dispose objects before losing scope
                managementClass = new ManagementClass(new ManagementScope(Path.Value.Path, Path.Factory.Options?.ConnectionOptions), new ManagementPath(Path.Value.Path), Path.Factory.Options?.ObjectGetOptions);
                // #pragma warning restore IDE0067 // Dispose objects before losing scope

            }

            if (Path.Value.WMIItemType == WMIItemType.Namespace)

            {

                // managementClass = Path.ManagementObject as ManagementClass ?? new ManagementClass(new ManagementScope(Path.Path, Path.Factory?.Options?.ConnectionOptions), new ManagementPath(Path.Path), Path.Factory?.Options?.ObjectGetOptions);

                if (WMIItemTypes.HasFlag(WMIItemTypes.Namespace))

                    try
                    {

                        managementClass.Get();

                        using (ManagementObjectCollection.ManagementObjectEnumerator instances = (Path.Factory.Options?.EnumerationOptions == null ? managementClass.GetInstances() : managementClass.GetInstances(Path.Factory.Options?.EnumerationOptions)).GetEnumerator())
                        {

                            ManagementBaseObject instance;

                            while (instances.MoveNext())

                                try

                                {

                                    do

                                    {

                                        instance = instances.Current;

                                        _path = WMIItemInfo.GetPath(instance, WMIItemType.Namespace);

                                        if (CheckFilter(_path))

                                            _ = paths.AddLast(new PathInfo(_path, _path.RemoveAccents(), WMIItemInfo.GetName(instance, WMIItemType.Namespace), WMIItemType.Namespace, instance, managementBaseObject => WMIItemInfo.DefaultManagementObjectDeepClone(managementBaseObject as ManagementClass ?? throw new NotSupportedException("The object is not a ManagementObject."), null)));

                                    }

                                    while (instances.MoveNext());

                                }

                                // #pragma warning disable CA1031 // Do not catch general exception types
                                catch (Exception ex) when (!(ex is ThreadAbortException)) { }
                            // #pragma warning restore CA1031 // Do not catch general exception types

                        }

                    }

                    // #pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex) when (!(ex is ThreadAbortException)) { }
                // #pragma warning restore CA1031 // Do not catch general exception types

                if (WMIItemTypes.HasFlag(WMIItemTypes.Class))

                    try

                    {

                        // MessageBox.Show(wmiItemInfo.Path.Substring(0, wmiItemInfo.Path.Length - ":__NAMESPACE".Length));
                        // managementClass = new ManagementClass(new ManagementScope(Path.Path, Path.Factory?.Options?.ConnectionOptions), new ManagementPath(Path.Path.Substring(0, Path.Path.Length - ":__NAMESPACE".Length)), Path.Factory?.Options?.ObjectGetOptions);

                        //#if DEBUG
                        //                        if (Path.Path.Contains("CIM"))

                        //                            MessageBox.Show(instances.Count.ToString());
                        //#endif

                        ManagementBaseObject instance;

                        using (ManagementObjectCollection.ManagementObjectEnumerator instances = (Path.Factory?.Options?.EnumerationOptions == null ? managementClass.GetSubclasses() : managementClass.GetSubclasses(Path.Factory?.Options?.EnumerationOptions)).GetEnumerator())

                        {

                            while (instances.MoveNext())

                                try

                                {

                                    do

                                    {

                                        instance = instances.Current;

                                        _path = WMIItemInfo.GetPath(instance, WMIItemType.Class);

                                        if (CheckFilter(_path))

                                            _ = paths.AddLast(new PathInfo(_path, _path.RemoveAccents(), WMIItemInfo.GetName(instance, WMIItemType.Class), WMIItemType.Class, instance, managementBaseObject => WMIItemInfo.DefaultManagementObjectDeepClone(managementBaseObject as ManagementClass ?? throw new NotSupportedException("The object is not a ManagementObject."), null)));

                                    } while (instances.MoveNext());

                                }

#pragma warning disable CA1031 // Do not catch general exception types
                                catch (Exception ex) when (!(ex is ThreadAbortException)) { }
#pragma warning restore CA1031 // Do not catch general exception types

                        }

                    }

#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex) when (!(ex is ThreadAbortException)) { }
#pragma warning restore CA1031 // Do not catch general exception types

            }

            else if (Path.Value.WMIItemType == WMIItemType.Class && WMIItemTypes.HasFlag(WMIItemTypes.Instance))

            {

                try
                {

                    managementClass.Get();

                    ManagementBaseObject instance;

                    using (ManagementObjectCollection.ManagementObjectEnumerator instances = (Path.Factory?.Options?.EnumerationOptions == null ? managementClass.GetInstances() : managementClass.GetInstances(Path.Factory?.Options?.EnumerationOptions)).GetEnumerator())

                        while (instances.MoveNext())

                            try

                            {

                                do

                                {

                                    instance = instances.Current;

                                    _path = WMIItemInfo.GetPath(instance, WMIItemType.Instance);

                                    if (CheckFilter(_path))

                                        _ = paths.AddLast(new PathInfo(_path, _path.RemoveAccents(), WMIItemInfo.GetName(instance, WMIItemType.Instance), WMIItemType.Instance, instance, managementBaseObject => WMIItemInfo.DefaultManagementObjectDeepClone(managementBaseObject as ManagementClass ?? throw new NotSupportedException("The object is not a ManagementObject."), null)));

                                } while (instances.MoveNext());

                            }

                            // #pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex) when (!(ex is ThreadAbortException)) { }
                    // #pragma warning restore CA1031 // Do not catch general exception types

                }

                // #pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex) when (!(ex is ThreadAbortException)) { }
                // #pragma warning restore CA1031 // Do not catch general exception types

                if (dispose)

                    managementClass.Dispose();

            }



            IEnumerable<PathInfo> pathInfos;



            if (FileSystemObjectComparer == null)

                pathInfos = paths;

            else

            {

                var _paths = paths.ToList();

                _paths.Sort((System.Collections.Generic.IComparer<PathInfo>)FileSystemObjectComparer);

                pathInfos = _paths;

            }



            PathInfo path_;



            using (IEnumerator<PathInfo> _paths = pathInfos.GetEnumerator())

                while (_paths.MoveNext())

                    try

                    {

                        do

                        {

                            path_ = _paths.Current;

                            // new_Path.LoadThumbnail();

                            ReportProgress(0, new BrowsableObjectTreeNode<TItems, TSubItems, TItemsFactory>((TItems)(IWMIItemInfo)Path.Factory.GetBrowsableObjectInfo(path_.Path, path_.WMIItemType, path_.ManagementObject, path_.ManagementObjectDelegate /*managementObject => WMIItemInfo.DefaultManagementObjectDeepClone( (ManagementObject) path_.ManagementObject, null )*/), (TItemsFactory)Path.Factory.DeepClone()));

                        } while (_paths.MoveNext());

                    }

#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex) when (!(ex is ThreadAbortException)) { }
#pragma warning restore CA1031 // Do not catch general exception types

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

            public DeepClone<ManagementBaseObject> ManagementObjectDelegate { get; }

            public ManagementBaseObject ManagementObject { get; }

            public WMIItemType WMIItemType { get; }

            public PathInfo(string path, string normalizedPath, string name, WMIItemType wmiItemType, ManagementBaseObject managementObject, DeepClone<ManagementBaseObject> managementObjectDelegate) : base(path, normalizedPath)
            {

                Name = name;

                ManagementObject = managementObject;

                ManagementObjectDelegate = managementObjectDelegate;

                WMIItemType = wmiItemType;

            }

        }

    }

}
