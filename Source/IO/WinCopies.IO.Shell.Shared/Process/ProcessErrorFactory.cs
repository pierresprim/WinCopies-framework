namespace WinCopies.IO.Process
{
    public abstract class ProcessErrorFactory<T, TAction> : ProcessTypes<T, ProcessError, TAction>.ProcessErrorFactoryBase, ProcessErrorTypes<T, ProcessError, TAction>.IProcessErrorFactories where T : IPathInfo
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

    public class DefaultProcessErrorFactory<T> : ProcessErrorFactory<T, ErrorAction> where T : IPathInfo
    {
        public override ErrorAction IgnoreAction => ErrorAction.Ignore;
    }
}
