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

using Microsoft.WindowsAPICodePack.Win32Native;
using System;

#if !CS8
using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO.Process
{
    public static partial class ProcessTypes<T> where T : IPath
    {
        public static class ProcessErrorTypes<TError, TAction>
        {
            public interface IProcessErrorItemFactory
            {
                IProcessErrorItem<T, TError, TAction> GetErrorItem(T item, IProcessError<TError, TAction> error);
            }

            public interface IProcessErrorFactories : IProcessErrorFactory<TError, TAction>, IProcessErrorItemFactory
            {
                // Left empty.
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

    public static class ProcessTypes<TPath, TError, TAction> where TPath : IPathInfo
    {
        public class ProcessErrorFactoryBase : IProcessErrorFactoryBase<TError, TAction>
        {
            public IProcessError<TError, TAction> GetError(TError error, Exception exception, ErrorCode errorCode) => new ProcessError<TError, TAction>(error, exception, errorCode);

            public IProcessError<TError, TAction> GetError(TError error, Exception exception, HResult hResult) => new ProcessError<TError, TAction>(error, exception, hResult);

            public IProcessError<TError, TAction> GetError(TError error, string message, ErrorCode errorCode) => new ProcessError<TError, TAction>(error, message, errorCode);

            public IProcessError<TError, TAction> GetError(TError error, string message, HResult hResult) => new ProcessError<TError, TAction>(error, message, hResult);

            public IProcessErrorItem<TPath, TError, TAction> GetErrorItem(TPath item, IProcessError<TError, TAction> error) => new ProcessErrorItem(item, error);

#if !CS8
            #region IProcessErrorFactoryBase Support
            private static TError GetError(in object error, in string argumentName) => error is TError _error ? _error : throw GetInvalidTypeArgumentException(argumentName);

            IProcessError IProcessErrorFactoryBase.GetError(object error, Exception exception, ErrorCode errorCode) => GetError(GetError(error, nameof(error)), exception, errorCode);

            IProcessError IProcessErrorFactoryBase.GetError(object error, Exception exception, HResult hResult) => GetError(GetError(error, nameof(error)), exception, hResult);

            IProcessError IProcessErrorFactoryBase.GetError(object error, string message, ErrorCode errorCode) => GetError(GetError(error, nameof(error)), message, errorCode);

            IProcessError IProcessErrorFactoryBase.GetError(object error, string message, HResult hResult) => GetError(GetError(error, nameof(error)), message, hResult);
            #endregion
#endif
        }

        public class ProcessErrorItem : IProcessErrorItem<TPath, TError, TAction>
        {
            public TPath Item { get; }

            public IProcessError<TError, TAction> Error { get; internal set; }

            string IPathCommon.RelativePath => Item.RelativePath;

            IPathCommon IPathCommon.Parent => Item.Parent;

            string IPathCommon.Path => Item.Path;

            bool IPath.IsDirectory => Item.IsDirectory;

            Size? IPath.Size => Item.Size;

            protected internal ProcessErrorItem(in TPath path) => Item = path;

            protected internal ProcessErrorItem(in TPath path, in IProcessError<TError, TAction> error) : this(path) => Error = error;

            public override string ToString() => PathHelper.ToString(this);

            public bool Equals(IO.IPathCommon other) => PathHelper.Equals(this, other);

            public override bool Equals(object obj) => PathHelper.Equals(this, obj);

            public override int GetHashCode() => PathHelper.GetHashCode(this);

#if !CS8
            #region IProcessErrorItem Support
            IProcessError IProcessErrorItem.Error => Error;
            #endregion
#endif
        }
    }

    public abstract class ProcessErrorFactory<T, TAction> : ProcessTypes<T, ProcessError, TAction>.ProcessErrorFactoryBase, ProcessTypes<T>.ProcessErrorTypes<ProcessError, TAction>.IProcessErrorFactories where T : IPathInfo
    {
        public ProcessError NoError => ProcessError.None;

        public ProcessError UnknownError => ProcessError.UnknownError;

        public ProcessError CancelledByUserError => ProcessError.CancelledByUser;

        public ProcessError WrongStatusError => ProcessError.WrongStatus;

        public abstract TAction IgnoreAction { get; }

#if !CS8
        object IProcessErrorFactoryData.NoError => NoError;

        object IProcessErrorFactoryData.UnknownError => UnknownError;

        object IProcessErrorFactoryData.CancelledByUserError => CancelledByUserError;

        object IProcessErrorFactoryData.WrongStatusError => WrongStatusError;
#endif
    }
}
