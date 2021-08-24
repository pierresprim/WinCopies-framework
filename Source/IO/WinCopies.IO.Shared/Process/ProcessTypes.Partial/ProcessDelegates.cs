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

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    public interface IProcessProgressDelegateParameter
    {
        uint Progress { get; }

        uint? CurrentPathProgress { get; }
    }

    public struct ProcessProgressDelegateParameter : IProcessProgressDelegateParameter
    {
        public uint Progress { get; }

        public uint? CurrentPathProgress { get; }

        public ProcessProgressDelegateParameter(in uint progress, in uint? currentPathProgress)
        {
            Progress = progress;

            CurrentPathProgress = currentPathProgress;
        }
    }

    public interface IProcessEventDelegates
    {
        object AddProgressDelegate(Action<IPathCommon> action);

        void RemoveProgressDelegate(object action);

        void AddCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates);

        void RemoveCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates);

        void AddCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action);

        void RemoveCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action);

        object AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action);

        void RemoveCommonDelegate(object action);
    }

    public static class ProcessDelegateTypes<T, TParam> where T : IPathInfo where TParam : IProcessProgressDelegateParameter
    {
        public interface IProcessDelegates
        {
            EventDelegate<T> ProgressDelegate { get; }

            EventAndQueryDelegate<bool, bool> CheckPerformedDelegate { get; }

            EventAndQueryDelegate<object, bool> CancellationPendingDelegate { get; }

            EventAndQueryDelegate<TParam, bool> CommonDelegate { get; }
        }

        public interface IProcessDelegates<TProcessEventDelegates> : IProcessDelegates
        {
            TProcessEventDelegates GetProcessEventDelegates();
        }

        public class ProcessDelegatesBase : IProcessDelegates
        {
            public EventDelegate<T> ProgressDelegate { get; }

            public EventAndQueryDelegate<bool, bool> CheckPerformedDelegate { get; }

            public EventAndQueryDelegate<object, bool> CancellationPendingDelegate { get; }

            public EventAndQueryDelegate<TParam, bool> CommonDelegate { get; }

            public ProcessDelegatesBase(in EventDelegate<T> progressDelegate, in EventAndQueryDelegate<bool, bool> checkPerformedDelegate, in EventAndQueryDelegate<object, bool> cancellationPendingDelegate, in EventAndQueryDelegate<TParam, bool> commonDelegate)
            {
                ProgressDelegate = progressDelegate ?? throw GetArgumentNullException(nameof(progressDelegate));
                CheckPerformedDelegate = checkPerformedDelegate ?? throw GetArgumentNullException(nameof(checkPerformedDelegate));
                CancellationPendingDelegate = cancellationPendingDelegate ?? throw GetArgumentNullException(nameof(cancellationPendingDelegate));
                CommonDelegate = commonDelegate ?? throw GetArgumentNullException(nameof(commonDelegate));
            }
        }

        public abstract class ProcessDelegatesAbstract<TProcessEventDelegates> : ProcessDelegatesBase, IProcessDelegates<TProcessEventDelegates> where TProcessEventDelegates : IProcessEventDelegates
        {
            public ProcessDelegatesAbstract(in EventDelegate<T> progressDelegate, in EventAndQueryDelegate<bool, bool> checkPerformedDelegate, in EventAndQueryDelegate<object, bool> cancellationPendingDelegate, in EventAndQueryDelegate<TParam, bool> commonDelegate) : base(progressDelegate, checkPerformedDelegate, cancellationPendingDelegate, commonDelegate)
            {
                // Left empty.
            }

            public abstract TProcessEventDelegates GetProcessEventDelegates();
        }

        public class ProcessDelegates : ProcessDelegatesAbstract<IProcessEventDelegates>
        {
            public ProcessDelegates(in EventDelegate<T> progressDelegate, in EventAndQueryDelegate<bool, bool> checkPerformedDelegate, in EventAndQueryDelegate<object, bool> cancellationPendingDelegate, in EventAndQueryDelegate<TParam, bool> commonDelegate) : base(progressDelegate, checkPerformedDelegate, cancellationPendingDelegate, commonDelegate)
            {
                // Left empty.
            }

            public override IProcessEventDelegates GetProcessEventDelegates() => new ProcessEventDelegates(this);
        }

        internal class QueryDelegateDelegate : QueryDelegateDelegate<TParam, bool>
        {
            private readonly IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> _action;

            public IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> Action => _action;

            public QueryDelegateDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) : base(obj => action.FirstAction(obj), (x, y) => action.OtherAction(x, y)) => _action = action;

            public static QueryDelegateDelegate GetDelegate(in IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) => new
#if !CS9
QueryDelegateDelegate
#endif
            (action);

            public static IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> GetDelegate(in QueryDelegateDelegate action) => action.Action;
        }

        public interface IProcessEventDelegates : Process.IProcessEventDelegates
        {
            void AddProgressDelegate(Action<T> action);

            void RemoveProgressDelegate(Action<T> action);

            void AddCommonDelegate(IQueryDelegateDelegate<TParam, bool> action);

            void RemoveCommonDelegate(IQueryDelegateDelegate<TParam, bool> action);

#if CS8
            private static Action<T> GetDelegate(Action<IPathCommon> action) => obj => action(obj);

            object Process.IProcessEventDelegates.AddProgressDelegate(Action<IPathCommon> action) 
            {
                Action<T> _action = GetDelegate(action);
                
                AddProgressDelegate(_action);

                return _action;
            }

            public Action<T> AddProgressDelegate(Action<IPathCommon> action)
            {
                Action<T> _action = GetDelegate(action);

                AddProgressDelegate(_action);

                return _action;
            }

            object Process.IProcessEventDelegates.AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                var _action = QueryDelegateDelegate.GetDelegate(action);

                AddCommonDelegate(_action);

                return _action;
            }

            public IQueryDelegateDelegate<TParam, bool> AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                IQueryDelegateDelegate<TParam, bool> _action = QueryDelegateDelegate.GetDelegate(action);

                AddCommonDelegate(_action);

                return _action;
            }
#endif
        }

        public class ProcessEventDelegates : IProcessEventDelegates
        {
            protected ProcessDelegatesBase Delegates { get; }

            public ProcessEventDelegates(in ProcessDelegatesBase delegates) => Delegates = delegates;

            public void AddProgressDelegate(Action<T> action) => Delegates.ProgressDelegate.Add(action);

            public void RemoveProgressDelegate(Action<T> action) => Delegates.ProgressDelegate.Remove(action);

            protected static ArgumentException GetNotContainedActionException(in string argumentName) => new ArgumentException("The given action is not contained in the common delegates.", argumentName);

            void Process.IProcessEventDelegates.RemoveProgressDelegate(object action) => Delegates.ProgressDelegate.Remove(action is Action<T> _action ? _action : throw GetNotContainedActionException(nameof(action)));

            public void AddCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates) => Delegates.CheckPerformedDelegate.Add(delegates);

            public void RemoveCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates) => Delegates.CheckPerformedDelegate.Remove(delegates);

            public void AddCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action) => Delegates.CancellationPendingDelegate.Add(action);

            public void RemoveCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action) => Delegates.CancellationPendingDelegate.Remove(action);

            public void AddCommonDelegate(IQueryDelegateDelegate<TParam, bool> action) => Delegates.CommonDelegate.Add(action);

            public void RemoveCommonDelegate(IQueryDelegateDelegate<TParam, bool> action) => Delegates.CommonDelegate.Remove(action);

            void Process.IProcessEventDelegates.RemoveCommonDelegate(object action) => Delegates.CommonDelegate.Remove(action is QueryDelegateDelegate _action ? _action : throw GetNotContainedActionException(nameof(action)));

#if !CS8
            private static Action<T> GetDelegate(Action<IPathCommon> action) => obj => action(obj);

            object Process.IProcessEventDelegates.AddProgressDelegate(Action<IPathCommon> action)
            {
                var _action = GetDelegate(action);

                AddProgressDelegate(_action);

                return _action;
            }

            public Action<T> AddProgressDelegate(Action<IPathCommon> action) => AddProgressDelegate(action);

            object Process.IProcessEventDelegates.AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                var _action = QueryDelegateDelegate.GetDelegate(action);

                AddCommonDelegate(_action);

                return _action;
            }

            public IQueryDelegateDelegate<TParam, bool> AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                IQueryDelegateDelegate<TParam, bool> _action = QueryDelegateDelegate.GetDelegate(action);

                AddCommonDelegate(_action);

                return _action;
            }
#endif
        }
    }
}
