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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WinCopies.Collections.DotNetFix;

namespace WinCopies.GUI.IO.Process
{
    public enum DuplicateFindingContentMatchingOption : byte
    {
        None = 0,

        Size = 1,

        Content = 2,

        Both = 3
    }

    public struct DuplicateFindingMatchingOptions
    {
        public bool Name { get; }

        public DuplicateFindingContentMatchingOption ContentMatchingOption { get; }

        public bool ModifiedDate { get; }

        public DuplicateFindingMatchingOptions(in bool name, in DuplicateFindingContentMatchingOption contentMatchingOption, in bool modifiedDate)
        {
            Name = name;

            ContentMatchingOption = contentMatchingOption;

            ModifiedDate = modifiedDate;
        }
    }

    public struct DuplicateFindingIgnoreOptions
    {
        public bool ZeroByteFiles { get; }

        public bool SystemFiles { get; }

        public bool HiddenFiles { get; }

        public string[] FileExtensions { get; }

        public string[] Paths { get; }

        public uint FileSizeUnder { get; }

        public uint FileSizeOver { get; }

        public DuplicateFindingIgnoreOptions(in bool zeroByteFiles, in bool systemFiles, in bool hiddenFiles, in string[] fileExtensions, in string[] paths, in uint fileSizeUnder, in uint fileSizeOver)
        {
            ZeroByteFiles = zeroByteFiles;

            SystemFiles = systemFiles;

            HiddenFiles = hiddenFiles;

            FileExtensions = fileExtensions;

            Paths = paths;

            FileSizeUnder = fileSizeUnder;

            FileSizeOver = fileSizeOver;
        }
    }

    public struct DuplicateFindingOptions
    {
        public DuplicateFindingMatchingOptions MatchingOptions { get; }

        public DuplicateFindingIgnoreOptions IgnoreOptions { get; }

        public DuplicateFindingOptions(in DuplicateFindingMatchingOptions matchingOptions, in DuplicateFindingIgnoreOptions ignoreOptions)
        {
            MatchingOptions = matchingOptions;

            IgnoreOptions = ignoreOptions;
        }
    }

    public class DuplicateFinding : Process<WinCopies.IO.IPathInfo
#if DEBUG
         , ProcessSimulationParameters
#endif
        >
    {
        public uint Buffer { get; }

        public PathCollection<WinCopies.IO.IPathInfo> PathsToCheckCollection { get; }

        protected ObservableQueueCollection<WinCopies.IO.IPathInfo> _PathsToCheck { get; }

        public ReadOnlyObservableQueueCollection<WinCopies.IO.IPathInfo> PathsToCheck { get; }
    }
}
