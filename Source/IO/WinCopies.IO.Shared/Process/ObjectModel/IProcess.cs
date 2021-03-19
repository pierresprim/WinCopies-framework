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

using WinCopies.Collections.DotNetFix;

namespace WinCopies.IO.Process.ObjectModel
{
    public interface IProcess : IPropertyObservable
    {
        IProcessErrorFactory Factory { get; }

        IProcessEventDelegates ProcessEventDelegates { get; }

        bool ArePathsLoaded { get; }

        IPathCommon SourcePath { get; }

        ProcessTypes<IPathInfo>.IProcessCollection Paths { get; }

        // todo: IQueue InitialPaths { get; }

        string Name { get; }

        IProcessError Error { get; }

        IProcessErrorFactoryData ProcessErrorFactoryData { get; }

        ProcessTypes<IProcessErrorItem>.IProcessCollection ErrorPaths { get; }

        Size InitialTotalSize { get; }

        uint InitialItemCount { get; }

        Size ActualRemainingSize { get; }

        bool LoadPaths();

        bool Start();

        void Reset();
    }

    public interface IDestinationProcess : IProcess
    {
        IPathCommon DestinationPath { get; }
    }
}
