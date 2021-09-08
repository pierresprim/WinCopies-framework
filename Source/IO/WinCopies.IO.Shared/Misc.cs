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
using WinCopies.IO.ObjectModel;

namespace WinCopies.IO
{
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
        private Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>> _list = new
#if !CS9
            Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>
#endif
            ();

        public BrowsableObjectInfoCallback Enqueue(in Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> action)
        {
            Collections.DotNetFix.Generic.LinkedList<Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason>>.LinkedListNode node = _list.AddLast(action);

            return new BrowsableObjectInfoCallback(() => _list.Remove(node));
        }

        public void RaiseCallbacks(IBrowsableObjectInfo browsableObjectInfo, BrowsableObjectInfoCallbackReason callbackReason)
        {
            foreach (Action<IBrowsableObjectInfo, BrowsableObjectInfoCallbackReason> action in _list)

                action(browsableObjectInfo, callbackReason);
        }

        public void Dispose()
        {
            _list.Clear();

            _list = null;
        }
    }

    internal class BrowsableObjectInfoCallback : DotNetFix.IDisposable
    {
        private Action _action;

        public bool IsDisposed => _action == null;

        public BrowsableObjectInfoCallback(in Action action) => _action = action;

        public void Dispose()
        {
            if (_action != null)
            {
                _action();

                _action = null;

                GC.SuppressFinalize(this);
            }
        }

        ~BrowsableObjectInfoCallback() => Dispose();
    }
}
