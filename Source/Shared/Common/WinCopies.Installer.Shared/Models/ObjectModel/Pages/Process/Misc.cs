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
using System.Collections.Generic;

namespace WinCopies.Installer
{
    public interface IFileValidationProvider
    {

        Func<System.IO.Stream>
#if CS8
            ?
#endif
            GetLocalValidationStream(string file);

        System.IO.Stream
#if CS8
           ?
#endif
           GetRemoteValidationStream(string file, out ulong? length);

        byte[]
#if CS8
            ?
#endif
            GetValidationData(System.IO.Stream stream);
    }

    public interface IFileEnumerable :
#if CS8
        Collections.DotNetFix.Generic.
#endif
        IEnumerable<KeyValuePair<string, IFile>>
    {
        string
#if CS8
            ?
#endif
            RelativeDirectory
        { get; }

        string
#if CS8
            ?
#endif
            OldRelativeDirectory
        { get; }

        IEnumerable<KeyValuePair<string, string>>
#if CS8
            ?
#endif
            Resources
        { get; }

        System.IO.Stream GetWriter(string path);
    }

    public delegate void Copier(System.IO.Stream reader, System.IO.Stream writer, Action<uint> progressReporter, out ulong? length);

    public interface ITemporaryFileEnumerable : IFileValidationProvider, System.IDisposable
    {
        string ActionName { get; }

        IFileEnumerable Files { get; }

        //IEnumerable<KeyValuePair<string, IValidableFile>> ValidationFiles { get; }

        IRemoteFileEnumerable Paths { get; }

        IEnumerable<string> PhysicalFiles { get; }

        Copier Copier { get; }

        /*System.IO.Stream
#if CS8
            ?
#endif
            GetValidationFileWriter(string relativePath);*/

        void Delete(string relativePath);
    }
}
