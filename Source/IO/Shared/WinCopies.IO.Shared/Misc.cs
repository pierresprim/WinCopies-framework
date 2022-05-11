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

using System;
using System.Reflection;

using WinCopies.Collections.Generic;

namespace WinCopies.IO
{
    public enum ExtensionCommand : byte
    {
        None = 0,

        CopyName = 1,

        CopyPath = 2
    }

    public enum FileSystemEntryEnumerationOrder : byte
    {
        /// <summary>
        /// Does not sort items.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enumerates files then directories.
        /// </summary>
        FilesThenDirectories = 1,

        /// <summary>
        /// Enumerates directories then files.
        /// </summary>
        DirectoriesThenFiles = 2
    }

    public struct ClientVersion
    {
        public string ClientName { get; }
        public uint MajorVersion { get; }
        public uint MinorVersion { get; }
        public uint Revision { get; }

        public ClientVersion(in string clientName, in uint majorVersion, in uint minorVersion, in uint revision)
        {
            ClientName = clientName;

            MajorVersion = majorVersion;

            MinorVersion = minorVersion;

            Revision = revision;
        }

        public ClientVersion(in string clientName, in Version version) : this(clientName, (uint)version.Major, (uint)version.Minor, (uint)version.Revision)
        {
            // Left empty.
        }

        public ClientVersion(in AssemblyName assemblyName) : this(assemblyName.Name, assemblyName.Version)
        {
            // Left empty.
        }

        public override bool Equals(object obj) => obj is ClientVersion _obj ? _obj.ClientName == ClientName && _obj.MajorVersion == MajorVersion && _obj.MinorVersion == MinorVersion && _obj.Revision == Revision : false;

        public override int GetHashCode() => ClientName.GetHashCode() ^ MajorVersion.GetHashCode() ^ MinorVersion.GetHashCode() ^ Revision.GetHashCode();

        public static bool operator ==(ClientVersion left, ClientVersion right) => left.Equals(right);

        public static bool operator !=(ClientVersion left, ClientVersion right) => !(left == right);
    }

    public interface IRecursiveEnumerable<T> : Collections.Generic.IRecursiveEnumerable<T>
    {
        new RecursiveEnumeratorAbstract<T> GetEnumerator();
    }

    public enum BrowsableObjectInfoCallbackReason
    {
        Added,

        Updated,

        Removed
    }

    internal class BrowsableObjectInfoCallbackQueue : System.IDisposable
    {
        private Collections.DotNetFix.Generic.LinkedList<Action<BrowsableObjectInfoCallbackArgs>> _list = new
#if !CS9
            Collections.DotNetFix.Generic.LinkedList<Action<BrowsableObjectInfoCallbackArgs>>
#endif
            ();

        public IBrowsableObjectInfoCallback Enqueue(in Action<BrowsableObjectInfoCallbackArgs> action) => new BrowsableObjectInfoCallback(this, _list.AddLast(action));

        internal void Remove(in BrowsableObjectInfoCallback callback) => _list.Remove(callback.Node);

        public void RaiseCallbacks(in BrowsableObjectInfoCallbackArgs a)
        {
            foreach (Action<BrowsableObjectInfoCallbackArgs> action in _list)

                action(a);
        }

        public void Dispose()
        {
            _list.Clear();

            _list = null;
        }
    }

    public interface IBrowsableObjectInfoCallback : DotNetFix.IDisposable
    {
        Action<BrowsableObjectInfoCallbackArgs> Action { get; }
    }

    internal class BrowsableObjectInfoCallback : IBrowsableObjectInfoCallback
    {
        private BrowsableObjectInfoCallbackQueue _callbackQueue;

        internal Collections.DotNetFix.Generic.LinkedList<Action<BrowsableObjectInfoCallbackArgs>>.LinkedListNode Node { get; private set; }

        public Action<BrowsableObjectInfoCallbackArgs> Action => IsDisposed ? throw ThrowHelper.GetExceptionForDispose(false) : Node.Value;

        public bool IsDisposed => _callbackQueue == null;

        public BrowsableObjectInfoCallback(in BrowsableObjectInfoCallbackQueue callbackQueue, Collections.DotNetFix.Generic.LinkedList<Action<BrowsableObjectInfoCallbackArgs>>.LinkedListNode node)
        {
            _callbackQueue = callbackQueue;

            Node = node;
        }

        public void Dispose()
        {
            if (_callbackQueue != null)
            {
                _callbackQueue.Remove(this);

                _callbackQueue = null;

                Node = null;

                GC.SuppressFinalize(this);
            }
        }

        ~BrowsableObjectInfoCallback() => Dispose();
    }
}
