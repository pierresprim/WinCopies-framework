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

using System.Collections.Generic;
using WinCopies.Collections.DotNetFix.Generic;

namespace WinCopies.IO.Process
{
    public interface IProcessError<TError>
    {
        TError Error { get; }

        string ErrorMessage { get; }
    }

    public interface IProcessErrorItem<TInnerItem, TError> : IPathInfo
    {
        TInnerItem Path { get; }

        IProcessError<TError> Error { get; }
    }

    namespace ObjectModel
    {
        public interface IProcess<TItems, TError>
        {
            TItems SourcePath { get; }

            IQueue<TItems> Paths { get; }

            string Name { get; }

            TError NoError { get; }

            TError UnknownError { get; }

            TError WrongStatus { get; }

            IProcessError<TError> Error { get; }

            IQueue<IProcessErrorItem<TItems, TError>> ErrorPaths { get; }

            IList<IProcessData> ProcessData { get; }

            bool LoadPaths();

            bool Start();

            void Reset();
        }

        public interface IDestinationProcess<TItems, TError> : IProcess<TItems, TError>
        {
            TItems DestinationPath { get; }
        }
    }
}
