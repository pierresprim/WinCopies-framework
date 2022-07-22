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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

using WinCopies.Extensions;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Shell.PropertySystem;
using WinCopies.Util;

namespace WinCopies.IO
{
    public partial class ShellObjectInfoProcessFactory
    {
        public class DragDropProcessFactory<T> : ProcessFactoryProcessInfoBase<T>, IDragDropProcessInfo where T : IShellObjectInfo<PropertySystem.IFileSystemObjectInfoProperties>
        {
            public DragDropProcessFactory(in T path) : base(path) { /* Left empty. */ }

            protected virtual bool SupportsDragDrop(IBrowsableObjectInfo path) => path.InnerObject is ShellObject shellObject && shellObject.IsFileSystemObject && path.ObjectProperties is IFileSystemObjectInfoProperties properties && properties.FileType != FileType.Drive;

            public bool CanRun(IEnumerable<IBrowsableObjectInfo> paths)
            {
                foreach (IBrowsableObjectInfo path in paths)

                    if (!SupportsDragDrop(path))

                        return false;

                return true;
            }

            protected virtual bool CanRunOverride(IDictionary<string, object> data, out DragDropEffects dragDropEffects, out string sourcePath, out string[] paths, out bool? sameRoot)
            {
                if (SupportsDragDrop(Path) && data.ContainsKey(DataFormats.FileDrop) && data.ContainsKey(PreferredDropEffect) && data[PreferredDropEffect] is DragDropEffects _dragDropEffects)

                    if ((dragDropEffects = _dragDropEffects).HasFlag(DragDropEffects.Copy) || _dragDropEffects.HasFlag())
                    {
                        var _paths = (string[])data[DataFormats.FileDrop];

                        string parent = null;

                        Predicate<string> predicate = path =>
                        {
                            parent = WinCopies.IO.Path.GetParent2(path);

                            predicate = _path => _path.Validate(parent, parent.EndsWith(System.IO.Path.DirectorySeparatorChar
#if !CS8
                                .ToString()
#endif
                                ) ? 1 : 0, null, null, 1, "\\");

                            return true;
                        };

                        paths = _paths;

                        foreach (string path in _paths)

                            if (!predicate(path))
                            {
                                sourcePath = parent;

                                sameRoot = null;

                                return false;
                            }

                        sourcePath = parent;

                        return (sameRoot = System.IO.Path.GetPathRoot(parent) == System.IO.Path.GetPathRoot(Path.Path)) == true || _dragDropEffects.HasFlag(DragDropEffects.Copy);
                    }

                dragDropEffects = DragDropEffects.None;

                sourcePath = null;

                paths = null;

                sameRoot = null;

                return false;
            }

            public bool CanRun(IDictionary<string, object> data) => CanRunOverride(data, out _, out _, out _, out _);

            public IDictionary<string, object> TryGetData(IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects)
            {
                if (CanRun(paths))
                {
                    StringCollection sc = Shell.Path.GetStringCollection(paths);

                    dragDropEffects = DragDropEffects.Copy | DragDropEffects.Move;

                    return new Dictionary<string, object>(1) { { DataFormats.FileDrop, sc } };
                }

                dragDropEffects = DragDropEffects.None;

                return null;
            }

            public IDictionary<string, object> GetData(IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects) => TryGetData(paths, out dragDropEffects) ?? throw ProcessFactoryProcessInfo.GetParametersGenerationException();

            public IProcessParameters TryGetProcessParameters(IDictionary<string, object> data)
            {
                if (CanRunOverride(data, out DragDropEffects dragDropEffects, out string sourcePath, out string[] paths, out bool? sameRoot))
                {
                    IProcessParameters getProcessParameters(in bool move) => GetCopyProcessParameters(sourcePath, Path.Path, move, paths);

                    return sameRoot.Value ? getProcessParameters(dragDropEffects.HasFlag(DragDropEffects.Move)) : getProcessParameters(false);
                }

                return null;
            }

            public IProcessParameters GetProcessParameters(IDictionary<string, object> data) => TryGetProcessParameters(data) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

            new internal void Dispose() => base.Dispose();
        }
    }
}
