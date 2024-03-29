﻿/* Copyright © Pierre Sprimont, 2021
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
using Microsoft.WindowsAPICodePack.Win32Native;
using Microsoft.WindowsAPICodePack.Win32Native.Shell;

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Shell.Process;
using WinCopies.Linq;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.Shell.Resources.ExceptionMessages;
using static WinCopies.ThrowHelper;

namespace WinCopies.IO
{
    public partial class ShellObjectInfoProcessFactory
    {
        public static class ProcessFactoryTypes<T> where T : IShellObjectInfoBase2
        {
            public class Copy : RunnableProcessFactoryProcessInfo<T>
            {
                public override bool UserConfirmationRequired => false;

                public bool Move { get; }

                public override string GetUserConfirmationText() => null;

                public Copy(in T path, in bool move) : base(path) => Move = move;

                new internal void Dispose() => base.Dispose();

                protected override bool CanRun(EmptyCheckEnumerator<IBrowsableObjectInfo> enumerator) => Shell.Process.ProcessHelper.CanRun(Path.Path, enumerator);

                private bool _Copy(in System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, in uint count)
                {
                    ThrowIfNull(paths, nameof(paths));

                    var sc = new StringCollection();

                    sc.AddRange(new ArrayBuilder<string>(paths.OfType<IShellObjectInfoBase2>().Select(path => path.Path)).ToArray());

                    for (int i = 0; i < count; i++)

                        try
                        {
                            var dropEffect = new MemoryStream(4);
                            dropEffect.Write(new byte[] { (byte)(Move ? DragDropEffects.Move : DragDropEffects.Copy | DragDropEffects.Link), 0, 0, 0 }, 0, 4);
                            dropEffect.Flush();

                            var dataObject = new DataObject();
                            dataObject.SetData(PreferredDropEffect, dropEffect);
                            dataObject.SetFileDropList(sc);

                            System.Windows.Clipboard.Clear();
                            System.Windows.Clipboard.SetDataObject(dataObject, true);

                            return true;
                        }

                        catch
                        {
                            // Left empty.
                        }

                    return false;
                }

                public override bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => CanRun(paths) && _Copy(paths, count);

                public override void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count)
                {
                    if (!TryRun(paths, count))

                        throw new InvalidOperationException(CopyDidNotSucceeded);
                }

                public override IProcessParameters TryGetProcessParameters(uint count)
                {
                    string sourcePath = CanPaste(Path.InnerObject, count, out bool move, out StringCollection sc);

                    return sourcePath == null
                        ? null
                        : GetCopyProcessParameters(sourcePath, Path.Path, move, new Enumerable<string>(() => new Collections.StringEnumerator(sc, Collections.DotNetFix.EnumerationDirection.FIFO)));
                }
            }

            public class Deletion : DirectProcessFactoryProcessInfo<T>
            {
                public bool Recycle => RemoveOption == RemoveOption.Recycle;

                public override bool UserConfirmationRequired => !Recycle;

                public RemoveOption RemoveOption { get; }

                public override string GetUserConfirmationText()
                {
                    switch (RemoveOption)
                    {
                        case RemoveOption.Clear:

                            return "Are you sure you want to empty the Recycle Bin?";

                        case RemoveOption.Delete:

                            return string.Format(Shell.Properties.Resources.UserConfirmationText, "permanently delete");
                    }

                    return null;
                }

                public Deletion(in T path, in RemoveOption removeOption) : base(path) => RemoveOption = removeOption == RemoveOption.None || !removeOption.IsValidEnumValue() ? throw new ArgumentOutOfRangeException(nameof(removeOption)) : removeOption;

                new internal void Dispose() => base.Dispose();

                protected bool CheckRecycleBin()
                {
                    switch (RemoveOption)
                    {
                        case RemoveOption.Delete:

                            return true;

                        case RemoveOption.Recycle:

                            if (Path.InnerObject.ParsingName == RecycleBin.ParsingName)

                                return false;

                            var recycleBinInfo = new SHQUERYRBINFO() { cbSize = Marshal.SizeOf<SHQUERYRBINFO>() };

                            return Path.InnerObject.IsFileSystemObject && CoreErrorHelper.Succeeded(Microsoft.WindowsAPICodePack.Win32Native.Shell.Shell.SHQueryRecycleBin(Path.Path, ref recycleBinInfo));

                        case RemoveOption.Clear:

                            return Path.InnerObject.ParsingName == RecycleBin.ParsingName;
                    }

                    return false;
                }

                protected override bool CanRun(EmptyCheckEnumerator<IBrowsableObjectInfo> enumerator) => RemoveOption == RemoveOption.Clear ? !enumerator.HasItems : Shell.Process.ProcessHelper.CanRun(Path.Path, enumerator);

                public override bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => CheckRecycleBin() && base.CanRun(paths);

                public override IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths)
                {
                    if (CheckRecycleBin())
                    {
                        System.Collections.Generic.IEnumerable<string> _paths = paths.Select(path => path.Path);

                        var enumerator = new EmptyCheckEnumerator<string>(_paths.GetEnumerator());

                        if (enumerator.HasItems)
                        {
                            if (RemoveOption == RemoveOption.Clear) // We have already checked that the current directory is the Recycle Bin when RemoveOption is Clear in CheckRecycleBin().

                                return null;

                            string sourcePath = null;

                            Predicate<string> predicate = null;

                            predicate = path =>
                            {
                                sourcePath = GetSourcePath(enumerator.Current, true, ref predicate);

                                return true;
                            };

                            return new Enumerable<string>(() => enumerator).ForEachANDALSO(predicate) ? ShellObjectInfoProcessFactory.GetProcessParameters(Consts.Guids.Shell.Process.Shell.Deletion, sourcePath, RemoveOption, _paths) : null;
                        }

                        else if (RemoveOption == RemoveOption.Clear)

                            return ShellObjectInfoProcessFactory.GetProcessParameters(Consts.Guids.Shell.Process.Shell.Deletion, ShellObjectFactory.Create(RecycleBin.ParsingName).Properties.System.ItemName.Value, RemoveOption, new CustomEnumeratorEnumerable<string, EmptyEnumerator<string>>(new EmptyEnumerator<string>()));
                    }

                    return null;
                }
            }
        }
    }
}
