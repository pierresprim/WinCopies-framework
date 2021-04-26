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

using Microsoft.WindowsAPICodePack.Win32Native;

namespace WinCopies.IO.Process
{
    public interface IProcessErrorBase
    {
        string ErrorMessage { get; }

        ErrorCode? ErrorCode { get; }

        HResult? HResult { get; }
    }

    public interface IProcessError : IProcessErrorBase
    {
        object Error { get; }
    }

    public interface IProcessError<TError> : IProcessError
    {
        TError Error { get; }

#if CS8
        object IProcessError.Error => Error;
#endif
    }

    public interface IProcessErrorItem : IPathInfo
    {
        IProcessError Error { get; }
    }

    public interface IProcessErrorItem<TInnerItem, TError> : IProcessErrorItem
    {
        TInnerItem Item { get; }

        IProcessError<TError> Error { get; }

#if CS8
        IProcessError IProcessErrorItem.Error => Error;
#endif
    }

}
