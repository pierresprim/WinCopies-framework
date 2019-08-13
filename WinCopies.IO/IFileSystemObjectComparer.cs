﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCopies.Util;

namespace WinCopies.IO
{
    public interface IFileSystemObjectComparer< in T> : IComparer<T>, IDeepCloneable where T : IFileSystemObject
    {
    }
}
