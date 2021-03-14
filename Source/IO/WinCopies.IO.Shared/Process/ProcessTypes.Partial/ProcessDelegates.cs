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

using System;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public class ProcessDelegates<TParam>
        {
            public Action<T> ProgressDelegate { get; }

            public Func<bool, bool> CheckPerformedDelegate { get; }

            public Func<bool> CancellationPendingDelegate { get; }

            public Func<TParam, bool> CommonDelegate { get; }

            public ProcessDelegates(in Action<T> progressDelegate, in Func<bool, bool> checkPerformedDelegate, in Func<bool> cancellationPendingDelegate, in Func<TParam, bool> commonDelegate)
            {
                ProgressDelegate = progressDelegate ?? throw GetArgumentNullException(nameof(progressDelegate));
                CheckPerformedDelegate = checkPerformedDelegate ?? throw GetArgumentNullException(nameof(checkPerformedDelegate));
                CancellationPendingDelegate = cancellationPendingDelegate ?? throw GetArgumentNullException(nameof(cancellationPendingDelegate));
                CommonDelegate = commonDelegate ?? throw GetArgumentNullException(nameof(commonDelegate));
            }
        }
    }
}
