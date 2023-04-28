/* Copyright © Pierre Sprimont, 2022
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

namespace WinCopies.Installer
{
    public interface IFile
    {
        bool IsMainApp { get; }

        string Name { get; }

        System.IO.Stream Stream { get; }
    }

    public readonly struct File : IFile
    {
        public bool IsMainApp { get; }

        public string Name { get; }

        public System.IO.Stream Stream { get; }

        public File(in bool isMainApp, in string name, in System.IO.Stream stream)
        {
            IsMainApp = isMainApp;
            Name = name;
            Stream = stream;
        }
    }

    public readonly struct DefaultFile : IFile
    {
        public bool IsMainApp => false;

        public string Name { get; }

        public System.IO.Stream Stream { get; }

        public DefaultFile(in string name, in System.IO.Stream stream)
        {
            Name = name;
            Stream = stream;
        }
    }

    /*public struct ValidableFile : IValidableFile
    {
        private readonly /*FuncOut<ulong?, */ /*Func<System.IO.Stream
#if CS8
              ?
#endif
              >
#if CS8
              ?
#endif
              _remoteValidationStreamProvider;

        public IFile File { get; }

        bool IFile.IsMainApp => File.IsMainApp;
        string IFile.Name => File.Name;
        System.IO.Stream IFile.Stream => File.Stream;

        public ValidableFile(in IFile file, in /*FuncOut<ulong?, */ /*Func<System.IO.Stream
#if CS8
            ?
#endif
            >
#if CS8
            ?
#endif
            remoteValidationStreamProvider)
        {
            File = file;
            _remoteValidationStreamProvider = remoteValidationStreamProvider;
        }
    }*/

    public enum Error : byte
    {
        Succeeded = 0,

        RecoveredError = 1,

        NotRecoveredError,

        FatalError,

        SuperFatalError
    }
}
