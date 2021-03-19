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

#if !CS8
using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public static class ProcessErrorTypes<TError>
        {
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
        }
    }

    public static class ProcessTypes<TPath, TError> where TPath : IPathInfo
    {
        public class ProcessErrorFactory : IProcessErrorFactoryBase<TError>
        {
            public IProcessError<TError> GetError(TError error, Exception exception) => new ProcessError<TError>(error, exception);

            public IProcessError<TError> GetError(TError error, string message) => new ProcessError<TError>(error, message);

            public IProcessErrorItem<TPath, TError> GetErrorItem(TPath item, IProcessError<TError> error) => new ProcessErrorItem(item, error);

#if !CS8
            #region IProcessErrorFactoryBase Support
            private static TError GetError(in object error, in string argumentName) => error is TError _error ? _error : throw GetInvalidTypeArgumentException(argumentName);

            IProcessError IProcessErrorFactory.GetError(object error, Exception exception) => GetError(GetError(error, nameof(error)), exception);

            IProcessError IProcessErrorFactory.GetError(object error, string message) => GetError(GetError(error, nameof(error)), message);
            #endregion
#endif
        }

        public class ProcessErrorItem : IProcessErrorItem<TPath, TError>
        {
            public TPath Path { get; }

            public IProcessError<TError> Error { get; }

            string IPathCommon.RelativePath => Path.RelativePath;

            IPathCommon IPathCommon.Parent => Path.Parent;

            string IPathCommon.Path => Path.Path;

            bool IPath.IsDirectory => Path.IsDirectory;

            Size? IPath.Size => Path.Size;

            public ProcessErrorItem(in TPath path, in IProcessError<TError> error)
            {
                Path = path;

                Error = error;
            }

#if !CS8
            #region IProcessErrorItem Support
            IProcessError IProcessErrorItem.Error => Error;
            #endregion
#endif
        }
    }
}
