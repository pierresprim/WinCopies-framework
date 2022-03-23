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
using System.IO;

#if !WinCopies3
using static WinCopies.Util.Util;
#else
using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO
{
    public static class File
    {
        public static bool IsDuplicate(in string leftPath, in string rightPath, in int bufferLength)
        {
            FileStream leftFileStream = null, rightFileStream = null;

            try
            {
                leftFileStream = new FileStream(leftPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferLength, FileOptions.None);

                rightFileStream = new FileStream(rightPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferLength, FileOptions.None);

                bool result = IsDuplicate(leftFileStream, rightFileStream, bufferLength);

                return result;
            }

            finally
            {
                if (leftFileStream != null)
                {
                    leftFileStream.Dispose();

                    rightFileStream?.Dispose();
                }
            }
        }

        /// <summary>
        /// Determines whether two files are duplicates by checking their size and content. For the exceptions that can occur, see the doc of the stream class that you use.
        /// </summary>
        /// <param name="leftStream">The left <see cref="Stream"/>.</param>
        /// <param name="rightStream">The right <see cref="Stream"/>.</param>
        /// <param name="bufferLength">The buffer length to use to read data.</param>
        /// <returns><see langword="true"/> if the two files are duplicates; otherwise <see langword="false"/>.</returns>
        public static bool IsDuplicate(in System.IO.Stream leftStream, in System.IO.Stream rightStream, in int bufferLength)
        {
            ThrowIfNull(leftStream, nameof(leftStream));
            ThrowIfNull(rightStream, nameof(rightStream));

            if (leftStream.Length != rightStream.Length)

                return false;

            byte[] leftBuffer = new byte[bufferLength], rightBuffer = new byte[bufferLength];

            int leftReadSize, rightReadSize;

            while ((leftReadSize = leftStream.Read(leftBuffer, 0, bufferLength)) > 0 && (rightReadSize = rightStream.Read(rightBuffer, 0, bufferLength)) > 0)
            {
                if (leftReadSize == rightReadSize)
                {
                    for (int i = 0; i < bufferLength; i++)

                        if (leftBuffer[i] != rightBuffer[i])

                            return false;
                }

                else

                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether two files are duplicates by checking their size and content. For the exceptions that can occur, see the doc of the stream class that you use.
        /// </summary>
        /// <param name="leftStream">The left <see cref="Stream"/>.</param>
        /// <param name="rightStream">The right <see cref="Stream"/>.</param>
        /// <param name="bufferLength">The buffer length to use to read data.</param>
        /// <param name="callback">A delegate that is raised after each data block has been checked. If the returned value is <see langword="true"/>, then the process is stopped.</param>
        /// <returns><see langword="true"/> if the two files are duplicates; <see langword="null"/> if the process has been canceled; otherwise <see langword="false"/>.</returns>
        public static bool? IsDuplicate(in System.IO.Stream leftStream, in System.IO.Stream rightStream, in int bufferLength, Func<bool> callback)
        {
            ThrowIfNull(leftStream, nameof(leftStream));
            ThrowIfNull(rightStream, nameof(rightStream));

            if (leftStream.Length != rightStream.Length)

                return false;

#if CS8

            callback ??= () => false;

#else

            if (callback == null)

                callback = () => false;

#endif

            byte[] leftBuffer = new byte[bufferLength], rightBuffer = new byte[bufferLength];

            int leftReadSize, rightReadSize;

            while ((leftReadSize = leftStream.Read(leftBuffer, 0, bufferLength)) > 0 && (rightReadSize = rightStream.Read(rightBuffer, 0, bufferLength)) > 0)
            {
                if (leftReadSize == rightReadSize)
                {
                    for (int i = 0; i < bufferLength; i++)

                        if (leftBuffer[i] != rightBuffer[i])

                            return false;
                }

                else

                    return false;

                if (callback())

                    return null;
            }

            return true;
        }

        public static FileStream GetFileStream(in string path, in int bufferLength) => new
#if !CS9
            FileStream
#endif
            (path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferLength, FileOptions.None);
    }
}
