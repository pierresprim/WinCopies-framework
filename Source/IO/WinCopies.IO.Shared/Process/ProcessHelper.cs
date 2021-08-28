using Microsoft.WindowsAPICodePack.Win32Native;

using System;
using System.Linq;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;
using WinCopies.IO.Enumeration;

namespace WinCopies.IO.Process
{
    public static class ProcessHelper
    {
        public static IProcessProgressDelegateParameter GetDefaultNotifyCompletionParameters() => new ProcessProgressDelegateParameter(100u, null);

        public static IRecursiveEnumerable<IPathInfo> GetDefaultEnumerable(in IPathInfo path, in RecursiveEnumerationOrder recursiveEnumerationOrder) => ProcessHelper<IPathInfo>.GetDefaultEnumerable(path, recursiveEnumerationOrder, __path => __path);

        public static string GetDestinationPath(in IPathInfo x, in IPathInfo y) => $"{x.Path}{WinCopies.IO.Path.PathSeparator}{y.GetPath(true)}";

        public static void GetDefaultPathsLoadingErrorParameters<TPath, TError, TErrorAction, TFactory>(in TError error, in string message, in ErrorCode errorCode, in ProcessOptionsCommon<TPath> options, in TFactory factory, out IProcessError<TError, TErrorAction> _error, out bool clearOnError) where TPath : IPath where TFactory : ProcessErrorTypes<TPath, TError, TErrorAction>.IProcessErrorFactories
        {
            _error = factory.GetError(error, message, errorCode);

            clearOnError = options.ClearOnError;
        }
    }

    public static class ProcessHelper<T> where T : IPathInfo
    {
        public static ProcessDelegateTypes<T, IProcessProgressDelegateParameter>.ProcessDelegates GetDefaultProcessDelegates() => new
#if !CS9
            ProcessDelegateTypes<T, IProcessProgressDelegateParameter>.ProcessDelegates
#endif
                (new EventDelegate<T>(),
                 EventAndQueryDelegate<bool>.GetANDALSO_Delegate(true),
                 EventAndQueryDelegate<object>.GetANDALSO_Delegate(false),
                 EventAndQueryDelegate<IProcessProgressDelegateParameter>.GetANDALSO_Delegate(true));

        public static EnumerableQueueCollection<T> GetInitialPaths(System.Collections.Generic.IEnumerator<string> enumerator, IPathInfo sourcePath, Converter<PathTypes<IPathInfo>.PathInfoBase, T> converter) => new
#if !CS9
            EnumerableQueueCollection<T>
#endif
            (new Enumerable<string>(
                () => enumerator
                ).Select(
                    path => path.EndsWith(":\\") || path.EndsWith(":\\\\")
                        ? converter(new PathTypes<IPathInfo>.RootPath(path, true))
                        : converter(new PathTypes<IPathInfo>.PathInfo(System.IO.Path.GetFileName(path), sourcePath)))
                    .ToEnumerableQueue());

        public static IRecursiveEnumerable<T> GetDefaultEnumerable(in T path, in RecursiveEnumerationOrder recursiveEnumerationOrder, in Func<PathTypes<IPathInfo>.PathInfo, T> func) => new RecursivelyEnumerablePath<T>(path, null, null
#if CS8
                    , null
#endif
                    , FileSystemEntryEnumerationOrder.FilesThenDirectories, recursiveEnumerationOrder, func, true
                    //#if DEBUG
                    //                    , null
                    //#endif
                    );

        public static class ProcessHelper2<TError, TAction, TProcessDelegateParam, TProcessEventDelegates> where TProcessEventDelegates : ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessEventDelegates where TProcessDelegateParam : IProcessProgressDelegateParameter
        {
            public static bool OnPathLoaded(in T path, in ProcessOptionsCommon<T> options, in ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in object cancellationPendingDelegateParam, in Action<T> action)
            {
                if ((options ?? throw ThrowHelper.GetArgumentNullException(nameof(options))).PathLoadedDelegate(path) && !processDelegates.CancellationPendingDelegate.RaiseEvent(cancellationPendingDelegateParam))
                {
                    action(path);

                    return true;
                }

                return false;
            }

            public static bool OnPathLoaded(in T path, in ProcessOptionsCommon<T> options, in ProcessDelegateTypes<T, TProcessDelegateParam>.IProcessDelegates<TProcessEventDelegates> processDelegates, in Action<T> action) => OnPathLoaded(path, options, processDelegates, null, action);
        }

    }
}
