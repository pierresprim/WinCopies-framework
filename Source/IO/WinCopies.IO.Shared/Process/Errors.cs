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
using System.Linq;

using WinCopies.IO.Resources;

#if CS8
using static WinCopies.ThrowHelper;
#endif

namespace WinCopies.IO.Process
{
    public static class ProcessErrorHelper
    {
        public static IProcessError<ProcessError, TAction> GetError<TAction>(in ProcessError error, in ErrorCode errorCode, in IProcessErrorFactory<ProcessError, TAction> factory) => factory.GetError(error, GetErrorMessageFromProcessError(error), errorCode);

        public static IProcessError<ProcessError, TAction> GetError<TAction>(in ProcessError error, in HResult hResult, in IProcessErrorFactory<ProcessError, TAction> factory) => factory.GetError(error, GetErrorMessageFromProcessError(error), hResult);

        public static string GetErrorMessageFromProcessError(ProcessError error)
        {
            System.Reflection.PropertyInfo property = typeof(ExceptionMessages).GetProperties().FirstOrDefault(p => p.Name == error.ToString());

            return property == null ? error.ToString() : (string)property.GetValue(null);
        }

#if !CS8
        public static IProcessError GetNoErrorError(this IProcessErrorFactory factory) => factory.GetError(factory.NoError, ExceptionMessages.NoError, ErrorCode.NoError);

        public static IProcessError<T, TAction> GetNoErrorError<T, TAction>(this IProcessErrorFactory<T, TAction> factory) => factory.GetError(factory.NoError, ExceptionMessages.NoError, ErrorCode.NoError);
#endif
    }

    public sealed class ProcessError<TError, TAction> : IProcessError<TError, TAction>
    {
        private readonly string _message;

        public TAction Action { get; set; }

        public TError Error { get; }

        public string ErrorMessage => Exception?.Message ?? _message;

        public Exception Exception { get; }

        public ErrorCode? ErrorCode { get; }

        public HResult? HResult { get; }

        private ProcessError(in TError error) => Error = error;

        private ProcessError(in TError error, in Exception exception) : this(error) => Exception = exception;

        public ProcessError(in TError error, in Exception exception, in ErrorCode errorCode) : this(error, exception) => ErrorCode = errorCode;

        public ProcessError(in TError error, in Exception exception, in HResult hResult) : this(error, exception) => HResult = hResult;

        private ProcessError(in TError error, in string message) : this(error) => _message = message;

        public ProcessError(in TError error, in string message, in ErrorCode errorCode) : this(error, message) => ErrorCode = errorCode;

        public ProcessError(in TError error, in string message, in HResult hResult) : this(error, message) => HResult = hResult;

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

    public interface IProcessErrorFactoryData<T, TAction> : IProcessErrorFactoryData<T>
    {
        TAction IgnoreAction { get; }
    }

    public interface IProcessErrorFactoryBase
    {
        IProcessError GetError(object error, Exception exception, ErrorCode errorCode);

        IProcessError GetError(object error, Exception exception, HResult hResult);

        IProcessError GetError(object error, string message, ErrorCode errorCode);

        IProcessError GetError(object error, string message, HResult hResult);
    }

    public interface IProcessErrorFactoryBase<T, TAction> : IProcessErrorFactoryBase
    {
        IProcessError<T, TAction> GetError(T error, Exception exception, ErrorCode errorCode);

        IProcessError<T, TAction> GetError(T error, Exception exception, HResult hResult);

        IProcessError<T, TAction> GetError(T error, string message, ErrorCode errorCode);

        IProcessError<T, TAction> GetError(T error, string message, HResult hResult);

#if CS8
        private static T GetError(in object error, in string argumentName) => error is T _error ? _error : throw GetInvalidTypeArgumentException(argumentName);

        IProcessError IProcessErrorFactoryBase.GetError(object error, Exception exception, ErrorCode errorCode) => GetError(GetError(error, nameof(error)), exception, errorCode);

        IProcessError IProcessErrorFactoryBase.GetError(object error, Exception exception, HResult hResult) => GetError(GetError(error, nameof(error)), exception, hResult);

        IProcessError IProcessErrorFactoryBase.GetError(object error, string message, ErrorCode errorCode) => GetError(GetError(error, nameof(error)), message, errorCode);

        IProcessError IProcessErrorFactoryBase.GetError(object error, string message, HResult hResult) => GetError(GetError(error, nameof(error)), message, hResult);
#endif
    }

    public interface IProcessErrorFactory : IProcessErrorFactoryData, IProcessErrorFactoryBase
    {
#if CS8
        IProcessError GetNoErrorError() => GetError(NoError, ExceptionMessages.NoError, ErrorCode.NoError);
#endif
    }

    public interface IProcessErrorFactory<T, TAction> : IProcessErrorFactoryData<T, TAction>, IProcessErrorFactoryBase<T, TAction>, IProcessErrorFactory
    {
#if CS8
        new IProcessError<T, TAction> GetNoErrorError() => GetError(NoError, ExceptionMessages.NoError, ErrorCode.NoError);
#endif
    }
}
