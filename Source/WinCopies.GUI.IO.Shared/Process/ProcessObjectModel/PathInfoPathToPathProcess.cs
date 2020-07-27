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

using System.ComponentModel;

namespace WinCopies.GUI.IO.Process
{
    public abstract class PathInfoPathToPathProcess<T
#if DEBUG
         , TSimulationParameters
#endif
        > : PathToPathProcess<T
#if DEBUG
             , TSimulationParameters
#endif
            > where T : WinCopies.IO.IPathInfo
#if DEBUG
        where TSimulationParameters : ProcessSimulationParameters
#endif
    {
        protected PathInfoPathToPathProcess(in PathCollection<T> pathsToExtract, in string destPath
#if DEBUG
             , in TSimulationParameters simulationParameters
#endif
            ) : base(pathsToExtract, destPath
#if DEBUG
                 , simulationParameters
#endif
                )
        { }

        protected override ProcessError OnLoadPaths(DoWorkEventArgs e)
        {
            foreach (IPathInfo path in PathCollection)

                _Paths.Enqueue(path);

            return ProcessError.None;
        }
    }
}
