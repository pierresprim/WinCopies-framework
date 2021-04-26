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

#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Data;

using WinCopies.Util.Data;
using WinCopies.Collections.DotNetFix
#if WinCopies3
    .Generic
#endif
    ;
using static WinCopies.
#if WinCopies3
    ThrowHelper
#else
    Util.Util;

using static WinCopies.Util.ThrowHelper
#endif
    ;

using WinCopies.Collections;
using WinCopies.Collections.Abstraction.Generic;
using WinCopies.Collections.Generic;
using WinCopies.Linq;

using static WinCopies.Util.Data.ConverterHelper;
using WinCopies.Collections.DotNetFix;
using System.Collections.Specialized;
using System.Windows.Input;

#if !WinCopies3
using System.Collections;
using WinCopies.Util;
#endif

namespace WinCopies
{
    public static class Temp
    {
        [DllImport(Microsoft.WindowsAPICodePack.NativeAPI.Consts.DllNames.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateDirectoryW([In, MarshalAs(UnmanagedType.LPWStr)] string lpPathName, [In] IntPtr lpSecurityAttributes);
    }
}
#endif
