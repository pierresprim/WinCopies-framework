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
using System.ComponentModel;
using System.Security;
using SevenZip;
using WinCopies.Util;

namespace WinCopies.GUI.IO.Process
{
    //public interface IExtractionProcessPathInfo : WinCopies.IO.IPathInfo
    //{
    //    Func<SevenZipExtractor> GetArchiveExtractorDelegate { get; }
    //}

    //public struct ExtractionProcessPathInfo : IExtractionProcessPathInfo
    //{
    //    public string Path { get; }

    //    public bool IsDirectory => false;

    //    public Func<SevenZipExtractor> GetArchiveExtractorDelegate { get; }

    //    public ExtractionProcessPathInfo(string path, Func<SevenZipExtractor> getArchiveExtractorDelegate)
    //    {
    //        Path = path;

    //        GetArchiveExtractorDelegate = getArchiveExtractorDelegate;
    //    }
    //}

    public class Extraction : ArchiveProcess<IPathInfo, ProcessQueueCollection, ReadOnlyProcessQueueCollection, ProcessErrorPathQueueCollection, ReadOnlyProcessErrorPathQueueCollection
#if DEBUG
         , ProcessSimulationParameters
#endif
        >
    {
        protected SevenZipExtractor ArchiveExtractor { get; }

        public static Extraction From(in PathInfoPathCollection pathsToExtract, in string destPath, in SevenZipExtractor archiveExtractor
#if DEBUG
             , ProcessSimulationParameters simulationParameters
#endif
            )
        {
            var processQueueCollection = new ProcessQueueCollection();
            var processErrorPathQueueCollection = new ProcessErrorPathQueueCollection();

            return new Extraction(pathsToExtract, destPath, archiveExtractor, processQueueCollection, new ReadOnlyProcessQueueCollection(processQueueCollection), processErrorPathQueueCollection, new ReadOnlyProcessErrorPathQueueCollection(processErrorPathQueueCollection)
#if DEBUG
                 , simulationParameters
#endif
                );
        }

        private Extraction(in PathInfoPathCollection pathsToExtract, in string destPath, in SevenZipExtractor archiveExtractor, in ProcessQueueCollection pathCollection, in ReadOnlyProcessQueueCollection readOnlyPathCollection, in ProcessErrorPathQueueCollection errorPathCollection, ReadOnlyProcessErrorPathQueueCollection readOnlyErrorPathCollection
#if DEBUG
             , ProcessSimulationParameters simulationParameters
#endif
            ) : base(pathsToExtract, destPath, pathCollection, readOnlyPathCollection, errorPathCollection, readOnlyErrorPathCollection
#if DEBUG
                 , simulationParameters
#endif
                ) =>

            // ValidateDestPath(destPath);

            ArchiveExtractor = archiveExtractor;

        private void ArchiveExtractor_FileExtractionStarted(object sender, FileInfoEventArgs e)
        {
            if (OnFileProcessStarted(e.PercentDone))

                e.Cancel = true;
        }

        private void ArchiveExtractor_FileExtractionFinished(object sender, FileInfoEventArgs e) => _ = OnFileProcessCompleted();

        protected override ProcessError OnProcess(DoWorkEventArgs e)
        {
            ArchiveExtractor.FileExtractionStarted += ArchiveExtractor_FileExtractionStarted;

            ArchiveExtractor.FileExtractionFinished += ArchiveExtractor_FileExtractionFinished;

            ArchiveExtractor.ExtractFiles(DestPath, PathsToStringArray());

            return ProcessError.None;
        }
    }
}
