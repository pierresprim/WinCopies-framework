using System;
using System.Collections.Generic;
using System.Text;

namespace WinCopies.IO.Process
{
    public interface IProcessErrorBase
    {
        string ErrorMessage { get; }
    }

    public interface IProcessError : IProcessErrorBase
    {
        object Error { get; }
    }

    public interface IProcessError<TError> : IProcessError
    {
        TError Error { get; }

#if CS8
        object IProcessError.Error => Error;
#endif
    }

    public interface IProcessErrorItem : IPathInfo
    {
        IProcessError Error { get; }
    }

    public interface IProcessErrorItem<TInnerItem, TError> : IProcessErrorItem
    {
        TInnerItem Path { get; }

        IProcessError<TError> Error { get; }

#if CS8
        IProcessError IProcessErrorItem.Error => Error;
#endif
    }

}
