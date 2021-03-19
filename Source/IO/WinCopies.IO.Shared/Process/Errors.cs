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
using System.Linq;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    public static class ProcessErrorHelper
    {
        public static IProcessError<ProcessError> GetError(in ProcessError error, in IProcessErrorFactory<ProcessError> factory) => factory.GetError(error, GetErrorMessageFromProcessError(error));

        public static string GetErrorMessageFromProcessError(ProcessError error)
        {
            System.Reflection.PropertyInfo property = typeof(Resources.ExceptionMessages).GetProperties().FirstOrDefault(p => p.Name == error.ToString());

            return property == null ? error.ToString() : (string)property.GetValue(null);
        }
    }

    public sealed class ProcessError<TError> : IProcessError<TError>
    {
        private readonly string _message;

        public TError Error { get; }

        public string ErrorMessage => Exception?.Message ?? _message;

        public Exception Exception { get; }

        private ProcessError(in TError error) => Error = error;

        public ProcessError(in TError error, in Exception exception) : this(error) => Exception = exception;

        public ProcessError(in TError error, in string message) : this(error) => _message = message;

#if !CS8
        #region IProcessError Support
        object IProcessError.Error => Error;
        #endregion
#endif
    }

    public interface IProcessErrorFactoryData
    {
        object NoError { get; }

        object UnknownError { get; }

        object CancelledByUserError { get; }

        object WrongStatusError { get; }
    }

    public interface IProcessErrorFactoryData<T> : IProcessErrorFactoryData
    {
        T NoError { get; }

        T UnknownError { get; }

        T CancelledByUserError { get; }

        T WrongStatusError { get; }

#if CS8
        object IProcessErrorFactoryData.NoError => NoError;

        object IProcessErrorFactoryData.UnknownError => UnknownError;

        object IProcessErrorFactoryData.CancelledByUserError => CancelledByUserError;

        object IProcessErrorFactoryData.WrongStatusError => WrongStatusError;
#endif
    }

    public interface IProcessErrorFactory
    {
        IProcessError GetError(object error, Exception exception);

        IProcessError GetError(object error, string message);
    }

    public interface IProcessErrorFactoryBase<T> : IProcessErrorFactory
    {
        IProcessError<T> GetError(T error, Exception exception);

        IProcessError<T> GetError(T error, string message);

#if CS8
        private static T GetError(in object error, in string argumentName) => error is T _error ? _error : throw GetInvalidTypeArgumentException(argumentName);

        IProcessError IProcessErrorFactory.GetError(object error, Exception exception) => GetError(GetError(error, nameof(error)), exception);

        IProcessError IProcessErrorFactory.GetError(object error, string message) => GetError(GetError(error, nameof(error)), message);
#endif
    }

    public interface IProcessErrorFactory<T> : IProcessErrorFactoryData<T>, IProcessErrorFactoryBase<T>
    {
        // Left empty.
    }
}
