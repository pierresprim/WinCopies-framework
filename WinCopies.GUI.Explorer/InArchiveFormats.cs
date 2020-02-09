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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCopies.GUI.Explorer
{
    [Flags]
    public enum InArchiveFormats
    {

        Zip = 1,

        SevenZip = 2,

        Arj = 4,

        BZip2 = 8,

        Cab = 16,

        Chm = 32,

        Compound = 64,

        Cpio = 128,

        CramFS = 256,

        Deb = 512,

        Dmg = 1024,

        Elf = 2048,

        Fat = 4096,

        Flv = 8192,

        GZip = 16384,

        Hfs = 32768,

        Iso = 65536,

        Lzh = 131072,

        Lzma = 262144,

        Lzma86 = 524288,

        Lzw = 1048576,

        MachO = 2097152,

    }
}
