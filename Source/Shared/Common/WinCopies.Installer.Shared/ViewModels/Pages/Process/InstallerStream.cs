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

/*using System;
using System.IO;

using ICustomStream = WinCopies.DotNetFix.IWriterStream;

namespace WinCopies.Installer
{
    public interface IInstallerStream : ICustomStream
    {
        Action GetDeleter();

        System.IO.Stream AsSystemStream();
    }

    public class InstallerStream : DotNetFix.FileStream, IInstallerStream
    {
        public InstallerStream(in string path) : base(GetStream(path)) { /* Left empty. */ /*}

        /*protected static Action GetAction(string path) => () => System.IO.File.Delete(path);

        protected static FileStream GetStream(in string path)
        {
            string
#if CS8
                ?
#endif
                directory = Path.GetDirectoryName(path);

            if (!(directory == null || Directory.Exists(directory)))

                Directory.CreateDirectory(directory);

            return new FileStream(path, FileMode.CreateNew);
        }

        public Action GetDeleter() => GetAction(Path);

        public System.IO.Stream AsSystemStream() => this;
    }
}*/
