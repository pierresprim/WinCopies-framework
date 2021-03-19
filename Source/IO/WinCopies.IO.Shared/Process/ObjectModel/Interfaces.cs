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

using WinCopies.Collections.AbstractionInterop.Generic;
using WinCopies.Collections.DotNetFix;
using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO.Process.ObjectModel
{
    public static partial class ProcessInterfaceModelTypes<TItems, TError> where TItems : IPathInfo
    {
        public interface IProcess<TParam,TProcessEventDelegates> : IProcess where TParam : IProcessProgressDelegateParameter where TProcessEventDelegates : ProcessDelegateTypes<TItems,TParam>.IProcessEventDelegates
        {
            new IProcessErrorFactory<TError> Factory { get; }

            new TProcessEventDelegates ProcessEventDelegates { get; }

            new TItems SourcePath { get; }

            new ProcessTypes<TItems>.IProcessCollection Paths { get; }

            new IProcessError<TError> Error { get; }

            new IProcessErrorFactoryData<TError> ProcessErrorFactoryData { get; }

            new ProcessTypes<IProcessErrorItem<TItems, TError>>.IProcessCollection ErrorPaths { get; }

#if CS8
            IProcessErrorFactory IProcess.Factory => Factory;

            IProcessEventDelegates IProcess.ProcessEventDelegates => ProcessEventDelegates;

            IPathCommon IProcess.SourcePath => SourcePath;

            ProcessTypes<IPathInfo>.IProcessCollection IProcess.Paths => new AbstractionProcessCollection<TItems, IPathInfo>(Paths);

            IProcessError IProcess.Error => Error;

            IProcessErrorFactoryData IProcess.ProcessErrorFactoryData => ProcessErrorFactoryData;

            ProcessTypes<IProcessErrorItem>.IProcessCollection IProcess.ErrorPaths => new AbstractionProcessCollection<IProcessErrorItem<TItems, TError>, IProcessErrorItem>(ErrorPaths);
#endif
        }

        public interface IDestinationProcess : IProcess, ObjectModel.IDestinationProcess
        {
            new TItems DestinationPath { get; }

#if CS8
            IPathCommon ObjectModel.IDestinationProcess.DestinationPath => DestinationPath;
#endif
        }
    }
}
