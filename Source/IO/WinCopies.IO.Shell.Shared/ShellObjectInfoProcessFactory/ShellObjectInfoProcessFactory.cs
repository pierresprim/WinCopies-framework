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
using System.Collections.Specialized;
using System.IO;
using System.Windows;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.IO.Shell.Process;
using WinCopies.Linq;
using WinCopies.Util;

using static Microsoft.WindowsAPICodePack.Shell.KnownFolders;

using static WinCopies.IO.ObjectModel.ShellObjectInfo;
using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;

using SystemPath = System.IO.Path;

namespace WinCopies.IO
{
    public partial class ShellObjectInfoProcessFactory : IProcessFactory
    {
        #region Fields
        private IShellObjectInfoBase2 _path;
        private ProcessFactoryTypes<IShellObjectInfoBase2>.Copy _copy;
        private ProcessFactoryTypes<IShellObjectInfoBase2>.Copy _cut;
        private ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion _recycling;
        private ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion _deletion;
        private ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion _clearing;
        private IProcessCommands _newItemProcessCommands;
        private const string PreferredDropEffect = "Preferred DropEffect";
        #endregion

        #region Properties
        public IRunnableProcessFactoryProcessInfo Copy => _copy;

        public IRunnableProcessFactoryProcessInfo Cut => _cut;

        public IDirectProcessFactoryProcessInfo Recycling => _recycling;

        public IDirectProcessFactoryProcessInfo Deletion => _deletion;

        public IDirectProcessFactoryProcessInfo Clearing => _clearing;

        public bool IsDisposed => _path == null;

        public IProcessCommands NewItemProcessCommands => IsDisposed ? throw GetExceptionForDispose(false) : _newItemProcessCommands;
        #endregion Properties

        public ShellObjectInfoProcessFactory(in IShellObjectInfo<IFileSystemObjectInfoProperties> path)
        {
            _path = path;

            _copy = new
#if !CS9
                    ProcessFactoryTypes<IShellObjectInfoBase2>.Copy
#endif
                    (path, false);

            _cut = new
#if !CS9
                    ProcessFactoryTypes<IShellObjectInfoBase2>.Copy
#endif
                    (path, true);

            _recycling = new
#if !CS9
                ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion
#endif
                (path, RemoveOption.Recycle);

            _deletion = new
#if !CS9
                ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion
#endif
                (path, RemoveOption.Delete);

            _clearing = new
#if !CS9
                ProcessFactoryTypes<IShellObjectInfoBase2>.Deletion
#endif
                (path, RemoveOption.Clear);

            _newItemProcessCommands = new _NewItemProcessCommands(path);
        }

        #region Methods
        public static ProcessParameters GetProcessParameters(in string processGuid, in string sourcePath, in string destinationPath, in bool move, in System.Collections.Generic.IEnumerable<string> paths) => new ProcessParameters(processGuid, paths.PrependValues(sourcePath, destinationPath, move ? "1" : "0"));

        public static ProcessParameters GetProcessParameters(in string processGuid, in string sourcePath, in RemoveOption removeOption, in System.Collections.Generic.IEnumerable<string> paths) => new ProcessParameters(processGuid, paths.PrependValues(sourcePath, removeOption.GetNumValue().ToString()));

        public static string GetSourcePath(in string path, in bool updatePredicate, ref Predicate<string> predicate)
        {
            ThrowIfNullEmptyOrWhiteSpace(path, nameof(path));

            string sourcePath = null;

            if (path.EndsWith(":\\") || path.EndsWith(":\\\\"))
            {
                if (updatePredicate)

                    predicate = _path => sourcePath == From(_path).Parent.Path;

                sourcePath = From(ShellObjectFactory.Create(Computer.ParsingName)).Path;
            }

            else
            {
                if (updatePredicate)

                    predicate = _path => sourcePath == SystemPath.GetDirectoryName(_path);

                sourcePath = SystemPath.GetDirectoryName(path);
            }

            return sourcePath;
        }

        public static StringCollection TryGetFileDropList(uint count, out bool move)
        {
            uint i = 0;

            StringCollection sc = null;

            bool _move = false;

            _ = For(() => i < count, () =>
            {
                if (System.Windows.Clipboard.ContainsFileDropList())

                    sc = System.Windows.Clipboard.GetFileDropList();

                if (System.Windows.Clipboard.ContainsData(PreferredDropEffect))

                    _move = ((DragDropEffects)((MemoryStream)System.Windows.Clipboard.GetData(PreferredDropEffect)).ReadByte()) == DragDropEffects.Move;
            }, () => i++);

            move = _move;

            return sc;
        }

        public static string CanPaste(in ShellObject shellObject, in uint count, out bool move, out StringCollection sc)
        {
            if (!(shellObject ?? throw GetArgumentNullException(nameof(shellObject))).IsFileSystemObject)
            {
                move = false;

                sc = null;

                return null;
            }

            sc = TryGetFileDropList(count, out move);

            if (sc == null || sc.Count == 0)

                return null;

            string sourcePath = null;

            Predicate<string> predicate = null;

            StringCollection _sc = sc;

            string getSourcePath(in bool updatePredicate) => GetSourcePath(_sc[0], updatePredicate, ref predicate);

            if (sc.Count == 1)

                return getSourcePath(false);

            predicate = path =>
            {
                sourcePath = getSourcePath(true);

                return true;
            };

            return new Enumerable<string>(() => new Collections.StringEnumerator(_sc, Collections.DotNetFix.EnumerationDirection.FIFO)).ForEachANDALSO(predicate) ? sourcePath : null;
        }

        bool IProcessFactory.CanPaste(uint count) => CanPaste(_path.InnerObject, count, out _, out _) != null;

        IProcess IProcessFactory.GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => GetProcess(processParameters);

        IProcess IProcessFactory.TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => TryGetProcess(processParameters);

        public void Dispose()
        {
            if (IsDisposed)

                return;

            _path = null;

            _copy.Dispose();
            _copy = null;

            _cut.Dispose();
            _cut = null;

            _recycling.Dispose();
            _recycling = null;

            _deletion.Dispose();
            _deletion = null;

            _clearing.Dispose();
            _clearing = null;

            _newItemProcessCommands.Dispose();
            _newItemProcessCommands = null;
        }
        #endregion

        ~ShellObjectInfoProcessFactory() => Dispose();
    }
}
