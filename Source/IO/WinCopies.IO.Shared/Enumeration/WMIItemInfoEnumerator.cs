//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using Microsoft.WindowsAPICodePack.PortableDevices;
//using System.Management;

//using WinCopies.Collections.Generic;
//using WinCopies.IO.AbstractionInterop;
//using WinCopies.IO.PropertySystem;

//namespace WinCopies.IO.Enumeration
//{
//    public sealed class WMIItemInfoEnumerator : Enumerator<ManagementBaseObject, WMIItemInfoItemProvider>
//    {
//        private readonly bool _resetInnerEnumerator = false;

//        public WMIItemType ItemType { get; }

//        public IWMIItemInfoOptions Options { get; }

//        public ClientVersion ClientVersion { get; }

//#if WinCopies3
//        private WMIItemInfoItemProvider _current = null;

//        protected override WMIItemInfoItemProvider CurrentOverride => _current;

//        public override bool? IsResetSupported => _resetInnerEnumerator;

//        protected override void ResetCurrent() => _current = null;
//#endif

//        public WMIItemInfoEnumerator(in System.Collections.Generic.IEnumerable<ManagementBaseObject> enumerable, in bool resetEnumerator, in WMIItemType itemType, in IWMIItemInfoOptions options, in ClientVersion clientVersion) : base(enumerable)
//        {
//            _resetInnerEnumerator = resetEnumerator;

//            ItemType = itemType;

//            Options = options;

//            ClientVersion = clientVersion;
//        }

//        protected override bool MoveNextOverride()
//        {
//            if (InnerEnumerator.MoveNext())
//            {

//                // if (CheckFilter(_path))

//#if !WinCopies3
//Current 
//#else
//                _current
//#endif
//            = new WMIItemInfoItemProvider(null, InnerEnumerator.Current, ItemType, Options, ClientVersion);

//                return true;
//            }

//            return false;
//        }

//        protected override void ResetOverride()
//        {
//            base.ResetOverride();

//            if (_resetInnerEnumerator)

//                InnerEnumerator.Reset();
//        }

//        #region IDisposable Support

//        protected override void
//#if !WinCopies3
//            Dispose(bool disposing)
//#else
//            DisposeManaged()
//#endif
//        {
//            Reset();

//#if !WinCopies3
//            base.Dispose(disposing);
//#else
//            base.DisposeManaged();
//#endif
//        }
//        #endregion
//    }
//}
