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

namespace WinCopies.GUI.IO
{
    /// <summary>
    /// This enum defines which content properties to check on a duplicate finding process.
    /// </summary>
    public enum DuplicateFindingContentMatchingOption : byte
    {
        /// <summary>
        /// Does not check any content properties.
        /// </summary>
        None = 0,

        /// <summary>
        /// Checks only size.
        /// </summary>
        Size = 1,

        /// <summary>
        /// Checks content and size.
        /// </summary>
        Content = 2
    }

    /// <summary>
    /// Defines matching options for a duplicate finding process.
    /// </summary>
    public class DuplicateFindingMatchingOptions
    {
        /// <summary>
        /// Gets a value that indicates whether the name should be checked.
        /// </summary>
        public bool Name { get; }

        /// <summary>
        /// Gets a value that indicates the content matching check option.
        /// </summary>
        public DuplicateFindingContentMatchingOption ContentMatchingOption { get; }

        /// <summary>
        /// Gets a value that indicates whether the modified date should be checked.
        /// </summary>
        public bool ModifiedDate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateFindingMatchingOptions"/> class.
        /// </summary>
        /// <param name="name">A value that indicates whether the name should be checked.</param>
        /// <param name="contentMatchingOption">A value that indicates the content matching check option.</param>
        /// <param name="modifiedDate">A value that indicates whether the modified date should be checked.</param>
        public DuplicateFindingMatchingOptions(in bool name, in DuplicateFindingContentMatchingOption contentMatchingOption, in bool modifiedDate)
        {
            Name = name;

            ContentMatchingOption = contentMatchingOption;

            ModifiedDate = modifiedDate;
        }
    }

    [Flags]
    public enum DuplicateFindingIgnoreValues : uint
    {
        None=0,

        ZeroByteFiles = 1,

        SystemFiles = 2,

        HiddenFiles = 4
    }

    /// <summary>
    /// Defines ignore-options for a duplicate finding process.
    /// </summary>
    public class DuplicateFindingIgnoreOptions
    {
        public DuplicateFindingIgnoreValues IgnoreValues { get; }

        public IReadOnlyList<string> FileNames { get; }

        public IReadOnlyList<string> FileExtensions { get; }

        public IReadOnlyList<string> Paths { get; }

        public uint? FileSizeUnder { get; }

        public uint? FileSizeOver { get; }

        public DuplicateFindingIgnoreOptions(in DuplicateFindingIgnoreValues ignoreValues, in IReadOnlyList<string> fileNames, in IReadOnlyList<string> fileExtensions, in IReadOnlyList<string> paths, in uint fileSizeUnder, in uint fileSizeOver)
        {
            IgnoreValues = ignoreValues;

            FileNames = fileNames;

            FileExtensions = fileExtensions;

            Paths = paths;

            FileSizeUnder = fileSizeUnder;

            FileSizeOver = fileSizeOver;
        }
    }

    public class DuplicateFindingOptions
    {
        public DuplicateFindingMatchingOptions MatchingOptions { get; }

        public DuplicateFindingIgnoreOptions IgnoreOptions { get; }

        public DuplicateFindingOptions(in DuplicateFindingMatchingOptions matchingOptions, in DuplicateFindingIgnoreOptions ignoreOptions)
        {
            MatchingOptions = matchingOptions;

            IgnoreOptions = ignoreOptions;
        }
    }
}
