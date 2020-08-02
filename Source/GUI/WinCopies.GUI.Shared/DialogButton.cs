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

namespace WinCopies.GUI.Windows.Dialogs
{
    public enum DialogButton : uint
    {

        /// <summary>
        /// This field corresponds to the Win32 MB_OK flag.
        /// </summary>
        OK = 0x00000000,

        /// <summary>
        /// This field corresponds to the Win32 MB_OKCANCEL flag.
        /// </summary>
        OKCancel = 0x00000001,

        /// <summary>
        /// This field corresponds to the Win32 MB_ABORTRETRYIGNORE flag.
        /// </summary>
        AbortRetryIgnore = 0x000002,

        /// <summary>
        /// This field corresponds to the Win32 MB_YESNOCANCEL flag.
        /// </summary>
        YesNoCancel = 0x000003,

        /// <summary>
        /// This field corresponds to the Win32 MB_YESNO flag.
        /// </summary>
        YesNo = 0x000004,

        /// <summary>
        /// This field corresponds to the Win32 MB_RETRYCANCEL flag.
        /// </summary>
        RetryCancel = 0x000005,

        /// <summary>
        /// This field corresponds to the Win32 MB_CANCELTRYCONTINUE flag.
        /// </summary>
        CancelTryContinue = 0x000006,

        CancelRetryContinue = CancelTryContinue,

        ContinueIgnoreCancel=0x000007,

        OKApplyCancel = 0x000008,

        RetryIgnoreCancel = 0x000009,

        IgnoreCancel = 0x000010,

        YesToAllNoToAllCancel = 0x000011,

        YesToAllNoToAll = 0x000012

    }

    public enum DefaultButton
    {

        None = 0,

        OK = 1,

        Cancel = 2,

        Abort = 3,

        Retry = 4,

        Ignore = 5,

        Yes = 6,

        No = 7,

        YesToAll=8,

        NoToAll=9,

        Apply = 10,

        Continue = 11

    }

    public enum MessageBoxResult

    {

        None = 0,

        OK = 1,

        Cancel = 2,

        Abort = 3,

        Retry = 4,

        Ignore = 5,

        Yes = 6,

        No = 7,

        YesToAll=8,

        NoToAll=9,

        Continue=10

    }
}
