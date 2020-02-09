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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.IO
{
    public class IOException : System.IO.IOException
    {

        public IBrowsableObjectInfo Path { get; }

        public IOException(IBrowsableObjectInfo path) : base() => Path = path;

        public IOException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public IOException(string message, int hresult, IBrowsableObjectInfo path) : base(message, hresult) => Path = path;

        public IOException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        protected IOException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class DirectoryNotFoundException : System.IO.DirectoryNotFoundException

    {

        public IBrowsableObjectInfo Path { get; }

        public DirectoryNotFoundException(IBrowsableObjectInfo path) : base() => Path = path;

        public DirectoryNotFoundException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public DirectoryNotFoundException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class DriveNotFoundException : System.IO.DriveNotFoundException

    {

        public IBrowsableObjectInfo Path { get; }

        public DriveNotFoundException(IBrowsableObjectInfo path) : base() => Path = path;

        public DriveNotFoundException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public DriveNotFoundException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        protected DriveNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class EndOfStreamException : System.IO.EndOfStreamException

    {

        public IBrowsableObjectInfo Path { get; }

        public EndOfStreamException(IBrowsableObjectInfo path) : base() => Path = path;

        public EndOfStreamException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public EndOfStreamException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        protected EndOfStreamException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class FileLoadException : System.IO.FileLoadException

    {

        public IBrowsableObjectInfo Path { get; }

        public FileLoadException(IBrowsableObjectInfo path) : base() => Path = path;

        public FileLoadException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public FileLoadException(string message, Exception inner, IBrowsableObjectInfo path) : base(message, inner) => Path = path;

        public FileLoadException(string message, string fileName, IBrowsableObjectInfo path) : base(message, fileName) => Path = path;

        public FileLoadException(string message, string fileName, Exception inner, IBrowsableObjectInfo path) : base(message, fileName, inner) => Path = path;

        protected FileLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class FileNotFoundException : System.IO.FileNotFoundException

    {

        public IBrowsableObjectInfo Path { get; }

        public FileNotFoundException(IBrowsableObjectInfo path) : base() => Path = path;

        public FileNotFoundException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public FileNotFoundException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        public FileNotFoundException(string message, string fileName, IBrowsableObjectInfo path) : base(message, fileName) => Path = path;

        public FileNotFoundException(string message, string fileName, Exception innerException, IBrowsableObjectInfo path) : base(message, fileName, innerException) => Path = path;

        protected FileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class PathTooLongException : System.IO.PathTooLongException

    {

        public IBrowsableObjectInfo Path { get; }

        public PathTooLongException(IBrowsableObjectInfo path) : base() => Path = path;

        public PathTooLongException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public PathTooLongException(string message, Exception innerException, IBrowsableObjectInfo path) : base(message, innerException) => Path = path;

        protected PathTooLongException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

    public class PipeException : System.IO.PipeException

    {

        public IBrowsableObjectInfo Path { get; }

        public PipeException(IBrowsableObjectInfo path) : base() => Path = path;

        public PipeException(string message, IBrowsableObjectInfo path) : base(message) => Path = path;

        public PipeException(string message, int errorCode, IBrowsableObjectInfo path) : base(message, errorCode) => Path = path;

        public PipeException(string message, Exception inner, IBrowsableObjectInfo path) : base(message, inner) => Path = path;

        protected PipeException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
