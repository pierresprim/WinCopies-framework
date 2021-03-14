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

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public static class ProcessErrorTypes<TError>
        {
            public class ProcessErrorItem : IProcessErrorItem<T, TError>
            {
                public T Path { get; }

                public IProcessError<TError> Error { get; }

                public ProcessErrorItem(in T path, in IProcessError<TError> error)
                {
                    Path = path;

                    Error = error;
                }
            }

            public interface IProcessErrorItemFactory
            {
                IProcessErrorItem<T, TError> GetErrorItem(T item, IProcessError<TError> error);
            }

            public interface IProcessErrorFactories : IProcessErrorFactory<TError>, IProcessErrorItemFactory
            {
#if CS8
                IProcessErrorItem<T, TError> GetErrorItem(T item, TError error, Exception exception) => GetErrorItem(item, GetError(error, exception));

                IProcessErrorItem<T, TError> GetErrorItem(T item, TError error, string message) => GetErrorItem(item, GetError(error, message));
#endif
            }

            public class ProcessErrorFactory : IProcessErrorFactories
            {
                public IProcessError<TError> GetError(TError error, Exception exception) => new ProcessError<TError>(error, exception);

                public IProcessError<TError> GetError(TError error, string message) => new ProcessError<TError>(error, message);

                public IProcessErrorItem<T, TError> GetErrorItem(T item, IProcessError<TError> error) => new ProcessErrorItem(item, error);
            }

            public class ProcessOptions
            {
                public Predicate<T> PathLoadedDelegate { get; }

                public bool ClearOnError { get; }

                public ProcessOptions(in Predicate<T> pathLoadedDelegate, in bool clearOnError)
                {
                    PathLoadedDelegate = pathLoadedDelegate;

                    ClearOnError = clearOnError;
                }
            }

#if !CS8
    public static class ProcessErrorFactoryHelper
    {
        public static IProcessErrorItem GetErrorItem(this IProcessErrorFactories<TError> factory, in TItem item, in TError error, in Exception exception) => factory.GetErrorItem(item, factory.GetError(error, exception));

        public static IProcessErrorItem GetErrorItem(this IProcessErrorFactories<TError> factory, in TItem item, in TError error, in string message) => factory.GetErrorItem(item, factory.GetError(error, message));
    }
#endif
        }
    }
}
