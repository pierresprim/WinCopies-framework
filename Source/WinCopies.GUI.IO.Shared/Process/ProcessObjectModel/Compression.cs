﻿/* Copyright © Pierre Sprimont, 2020
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

using System.ComponentModel;

using SevenZip;

namespace WinCopies.GUI.IO.Process
{
    public class Compression : ArchiveProcess<WinCopies.IO.IPathInfo>
    {
        protected SevenZipCompressor ArchiveCompressor { get; }

        public Compression(in PathCollection<WinCopies.IO.IPathInfo> pathsToExtract, in string destPath, in SevenZipCompressor archiveCompressor) : base(pathsToExtract, destPath) => ArchiveCompressor = archiveCompressor;

        private void ArchiveCompressor_FileCompressionStarted(object sender, FileNameEventArgs e)
        {
            if (OnFileProcessStarted(e.PercentDone))

                e.Cancel = true;
        }

        private void ArchiveCompressor_FileCompressionFinished(object sender, System.EventArgs e) => _ = OnFileProcessCompleted();

        protected override ProcessError OnProcess(DoWorkEventArgs e)
        {
            ArchiveCompressor.FileCompressionStarted += ArchiveCompressor_FileCompressionStarted;

            ArchiveCompressor.FileCompressionFinished += ArchiveCompressor_FileCompressionFinished;

            return ProcessError.None;
        }
    }
}
