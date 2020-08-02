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

#if DEBUG
using Microsoft.WindowsAPICodePack.Win32Native.Shell;
using System;
using WinCopies.IO;

namespace WinCopies.GUI.IO.Process
{

    public delegate bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref bool pbCancel, CopyFileFlags dwCopyFlags);

    public enum PathDirectoryType
    {
        Source,

        Destination
    }

    public abstract class ProcessSimulationParameters
    {
        private bool? _sourcePathRootExists;
        private bool? _sourceDriveReady;

        protected InvalidOperationException GetInvalidOperationException() => new InvalidOperationException("Value cannot be null.");

        public bool SourcePathRootExists { get => _sourcePathRootExists ?? throw GetInvalidOperationException(); set => _sourcePathRootExists = value; }

        public bool SourceDriveReady { get => _sourceDriveReady ?? throw GetInvalidOperationException(); set => _sourceDriveReady = value; }
    }

    public abstract class PathToPathProcessSimulationParameters : ProcessSimulationParameters 
    {
        private bool? _destPathRootExists;
        private bool? _destDriveReady;
        private long? _destDriveTotalFreeSpace;

        public bool DestPathRootExists { get => _destPathRootExists ?? throw GetInvalidOperationException(); set => _destPathRootExists = value; }

        public bool DestDriveReady { get => _destDriveReady ?? throw GetInvalidOperationException(); set => _destDriveReady = value; }

        public long DestDriveTotalFreeSpace { get => _destDriveTotalFreeSpace ?? throw GetInvalidOperationException(); set => _destDriveTotalFreeSpace = value; }
    }

    public class CopyProcessSimulationParameters : PathToPathProcessSimulationParameters 
    {
        private FileSystemEntryEnumeratorProcessSimulation _fileSystemEntryEnumeratorProcessSimulation;
        private Func<WinCopies.IO.IPathInfo, IPathInfo> _ioPathInfoToGUIIOPathInfoAction;
        private Func<string, string> _renameOnDuplicateAction;
        private CopyFileEx _copyFileExAction;
        private Func<string, bool> _createDirectoryWAction;
        private Func<string, bool> _destPathExistsAction;
        private Func<string, PathDirectoryType, Exception> _creatingFileStreamSucceedsAction;
        private Func<string, string, Func<bool>, bool?> _isDuplicateAction;

        public FileSystemEntryEnumeratorProcessSimulation FileSystemEntryEnumeratorProcessSimulation { get => _fileSystemEntryEnumeratorProcessSimulation ?? throw GetInvalidOperationException(); set => _fileSystemEntryEnumeratorProcessSimulation = value ?? throw GetInvalidOperationException(); }

        public Func<WinCopies.IO.IPathInfo, IPathInfo> IOPathInfoToGUIIOPathInfoAction { get => _ioPathInfoToGUIIOPathInfoAction ?? throw GetInvalidOperationException(); set => _ioPathInfoToGUIIOPathInfoAction = value ?? throw GetInvalidOperationException(); }

        public Func<string, string> RenameOnDuplicateAction { get => _renameOnDuplicateAction ?? throw GetInvalidOperationException(); set => _renameOnDuplicateAction = value ?? throw GetInvalidOperationException(); }

        public CopyFileEx CopyFileExAction { get => _copyFileExAction ?? throw GetInvalidOperationException(); set => _copyFileExAction = value ?? throw GetInvalidOperationException(); }

        public Func<string, bool> CreateDirectoryWAction { get => _createDirectoryWAction ?? throw GetInvalidOperationException(); set => _createDirectoryWAction = value ?? throw GetInvalidOperationException(); }

        public Func<string, bool> DestPathExistsAction { get => _destPathExistsAction ?? throw GetInvalidOperationException(); set => _destPathExistsAction = value ?? throw GetInvalidOperationException(); }

        public Func<string, PathDirectoryType, Exception> CreatingFileStreamSucceedsAction { get => _creatingFileStreamSucceedsAction ?? throw GetInvalidOperationException(); set => _creatingFileStreamSucceedsAction = value ?? throw GetInvalidOperationException(); }

        public Func<string, string, Func<bool>, bool?> IsDuplicateAction { get => _isDuplicateAction ?? throw GetInvalidOperationException(); set => _isDuplicateAction = value ?? throw GetInvalidOperationException(); }
    }
}
#endif 
