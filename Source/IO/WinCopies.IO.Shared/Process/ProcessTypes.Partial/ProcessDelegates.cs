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
        int Progress { get; }
    }

    public struct ProcessProgressDelegateParameter : IProcessProgressDelegateParameter
    {
        public int Progress { get; }

        public ProcessProgressDelegateParameter(int progress) => Progress = progress;
    }

    public interface IProcessEventDelegates
    {
        void AddProgressDelegate(Action<IPathCommon> action);

        void AddCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates);

        void RemoveCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates);

        void AddCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action);

        void RemoveCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action);

        void AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action);
    }

    public static class ProcessDelegateTypes<T, TParam> where T : IPathInfo where TParam : IProcessProgressDelegateParameter
    {
        public interface IProcessDelegates
        {
            EventDelegate<T> ProgressDelegate { get; }

            ANDALSO_EventAndQueryDelegate<bool> CheckPerformedDelegate { get; }

            ANDALSO_EventAndQueryDelegate<object> CancellationPendingDelegate { get; }

            ANDALSO_EventAndQueryDelegate<TParam> CommonDelegate { get; }
        }

        public class ProcessDelegatesBase : IProcessDelegates
        {
            public EventDelegate<T> ProgressDelegate { get; }

            public ANDALSO_EventAndQueryDelegate<bool> CheckPerformedDelegate { get; }

            public ANDALSO_EventAndQueryDelegate<object> CancellationPendingDelegate { get; }

            public ANDALSO_EventAndQueryDelegate<TParam> CommonDelegate { get; }

            public ProcessDelegatesBase(in EventDelegate<T> progressDelegate, in ANDALSO_EventAndQueryDelegate<bool> checkPerformedDelegate, in ANDALSO_EventAndQueryDelegate<object> cancellationPendingDelegate, in ANDALSO_EventAndQueryDelegate<TParam> commonDelegate)
            {
                ProgressDelegate = progressDelegate ?? throw GetArgumentNullException(nameof(progressDelegate));
                CheckPerformedDelegate = checkPerformedDelegate ?? throw GetArgumentNullException(nameof(checkPerformedDelegate));
                CancellationPendingDelegate = cancellationPendingDelegate ?? throw GetArgumentNullException(nameof(cancellationPendingDelegate));
                CommonDelegate = commonDelegate ?? throw GetArgumentNullException(nameof(commonDelegate));
            }
        }

        public abstract class ProcessDelegatesAbstract<TProcessEventDelegates> : ProcessDelegatesBase where TProcessEventDelegates : IProcessEventDelegates
        {
            public ProcessDelegatesAbstract(in EventDelegate<T> progressDelegate, in ANDALSO_EventAndQueryDelegate<bool> checkPerformedDelegate, in ANDALSO_EventAndQueryDelegate<object> cancellationPendingDelegate, in ANDALSO_EventAndQueryDelegate<TParam> commonDelegate) : base(progressDelegate, checkPerformedDelegate, cancellationPendingDelegate, commonDelegate)
            {
                // Left empty.
            }

            public abstract TProcessEventDelegates GetProcessEventDelegates();
        }

        public class ProcessDelegates : ProcessDelegatesAbstract<IProcessEventDelegates>
        {
            public ProcessDelegates(in EventDelegate<T> progressDelegate, in ANDALSO_EventAndQueryDelegate<bool> checkPerformedDelegate, in ANDALSO_EventAndQueryDelegate<object> cancellationPendingDelegate, in ANDALSO_EventAndQueryDelegate<TParam> commonDelegate) : base(progressDelegate, checkPerformedDelegate, cancellationPendingDelegate, commonDelegate)
            {
                // Left empty.
            }

            public override IProcessEventDelegates GetProcessEventDelegates() => new ProcessEventDelegates(this);
        }

        public interface IProcessEventDelegates : Process.IProcessEventDelegates
        {
            void AddProgressDelegate(Action<T> action);

            void RemoveProgressDelegate(Action<T> action);

            void AddCommonDelegate(IQueryDelegateDelegate<TParam, bool> action);

            void RemoveCommonDelegate(IQueryDelegateDelegate<TParam, bool> action);

#if CS8
            private static Action<T> GetDelegate(Action<IPathCommon> action) => obj => action(obj);

            void Process.IProcessEventDelegates.AddProgressDelegate(Action<IPathCommon> action) => AddProgressDelegate(GetDelegate(action));

            public Action<T> AddProgressDelegate(Action<IPathCommon> action)
            {
                Action<T> _action = GetDelegate(action);

                AddProgressDelegate(_action);

                return _action;
            }

            private static QueryDelegateDelegate<TParam, bool> GetDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) => new
#if !CS9
QueryDelegateDelegate<TParam, bool>
#endif
            (obj => action.FirstAction(obj), (x, y) => action.OtherAction(x, y));

            void Process.IProcessEventDelegates.AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) => AddCommonDelegate(GetDelegate(action));

            public IQueryDelegateDelegate<TParam, bool> AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                IQueryDelegateDelegate<TParam, bool> _action = GetDelegate(action);

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

            public void AddCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates) => Delegates.CheckPerformedDelegate.Add(delegates);

            public void RemoveCheckPerformedDelegate(IQueryDelegateDelegate<bool, bool> delegates) => Delegates.CheckPerformedDelegate.Remove(delegates);

            public void AddCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action) => Delegates.CancellationPendingDelegate.Add(action);

            public void RemoveCancellationPendingDelegate(IQueryDelegateDelegate<object, bool> action) => Delegates.CancellationPendingDelegate.Remove(action);

            public void AddCommonDelegate(IQueryDelegateDelegate<TParam, bool> action) => Delegates.CommonDelegate.Add(action);

            public void RemoveCommonDelegate(IQueryDelegateDelegate<TParam, bool> action) => Delegates.CommonDelegate.Remove(action);

#if !CS8
            private static Action<T> GetDelegate(Action<IPathCommon> action) => obj => action(obj);

            void Process.IProcessEventDelegates.AddProgressDelegate(Action<IPathCommon> action) => AddProgressDelegate(GetDelegate(action));

            public Action<T> AddProgressDelegate(Action<IPathCommon> action)
            {
                Action<T> _action = GetDelegate(action);

                AddProgressDelegate(_action);

                return _action;
            }

            private static QueryDelegateDelegate<TParam, bool> GetDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) => new
#if !CS9
QueryDelegateDelegate<TParam, bool>
#endif
            (obj => action.FirstAction(obj), (x, y) => action.OtherAction(x, y));

            void Process.IProcessEventDelegates.AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action) => AddCommonDelegate(GetDelegate(action));

            public IQueryDelegateDelegate<TParam, bool> AddCommonDelegate(IQueryDelegateDelegate<IProcessProgressDelegateParameter, bool> action)
            {
                IQueryDelegateDelegate<TParam, bool> _action = GetDelegate(action);

                AddCommonDelegate(_action);

                return _action;
            }
#endif
        }
    }
}
