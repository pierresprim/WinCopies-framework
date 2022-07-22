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
using System.Collections.Generic;
using System.Windows;

using WinCopies.Collections.Generic;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.Process.ObjectModel;

using static WinCopies.ThrowHelper;

namespace WinCopies.IO.Process
{
    public interface IProcessFactoryProcessInfoBase
    {
        bool UserConfirmationRequired { get; }

        string GetUserConfirmationText();
    }

    public interface IProcessFactoryProcessInfo : IProcessFactoryProcessInfoBase, IProcessInfoBase
    {
        // Left empty.
    }

    public interface IDirectProcessInfo : IProcessFactoryProcessInfo
    {
        IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);

        IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
    }

    public interface IRunnableProcessInfo : IProcessFactoryProcessInfo
    {
        bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        IProcessParameters GetProcessParameters(uint count);

        IProcessParameters TryGetProcessParameters(uint count);
    }

    public interface IDragDropProcessInfo : IProcessInfoBase
    {
        bool CanRun(IDictionary<string, object> data);

        IDictionary<string, object> TryGetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects);

        IDictionary<string, object> GetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects);

        IProcessParameters TryGetProcessParameters(IDictionary<string, object> data);

        IProcessParameters GetProcessParameters(IDictionary<string, object> data);
    }

    public static class ProcessFactoryProcessInfo
    {
        private static Exception GetParametersGenerationException(in bool process) => new InvalidOperationException($"An unknown error occurred during {(process ? "process " : null)}parameters generation.");

        public static Exception GetProcessParametersGenerationException() => GetParametersGenerationException(true);

        public static Exception GetParametersGenerationException() => GetParametersGenerationException(false);
    }

    public abstract class ProcessFactoryProcessInfoBase<T>
    {
        protected T Path { get; private set; }

        protected ProcessFactoryProcessInfoBase(in T path) => Path = path;

        protected virtual void Dispose() => Path = default;

        ~ProcessFactoryProcessInfoBase() => Dispose();
    }

    public abstract class ProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfoBase<T>, IProcessFactoryProcessInfo where T : IBrowsableObjectInfo
    {
        public abstract bool UserConfirmationRequired { get; }

        protected ProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

        public abstract string GetUserConfirmationText();

        protected abstract bool CanRun(EmptyCheckEnumerator<IBrowsableObjectInfo> enumerator);

        public virtual bool CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => CanRun(new EmptyCheckEnumerator<IBrowsableObjectInfo>((paths ?? throw GetArgumentNullException(nameof(paths))).GetEnumerator()));
    }

    public abstract class DirectProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IDirectProcessInfo where T : IBrowsableObjectInfo
    {
        protected DirectProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

        public virtual IProcessParameters GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => TryGetProcessParameters(paths) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

        public abstract IProcessParameters TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths);
    }

    public abstract class RunnableProcessFactoryProcessInfo<T> : ProcessFactoryProcessInfo<T>, IRunnableProcessInfo where T : IBrowsableObjectInfo
    {
        protected RunnableProcessFactoryProcessInfo(in T path) : base(path) { /* Left empty. */ }

        public abstract void Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        public abstract bool TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count);

        public virtual IProcessParameters GetProcessParameters(uint count) => TryGetProcessParameters(count) ?? throw ProcessFactoryProcessInfo.GetProcessParametersGenerationException();

        public abstract IProcessParameters TryGetProcessParameters(uint count);
    }

    public interface IProcessFactory : DotNetFix.IDisposable
    {
        IProcessCommand
#if CS8
            ?
#endif
            NewItemProcessCommand
        { get; }

        IProcessCommand
#if CS8
            ?
#endif
            RenameItemProcessCommand
        { get; }

        IRunnableProcessInfo
#if CS8
            ?
#endif
            Copy
        { get; }

        IRunnableProcessInfo
#if CS8
            ?
#endif
            Cut
        { get; }

        IDirectProcessInfo
#if CS8
            ?
#endif
            Recycling
        { get; }

        IDirectProcessInfo
#if CS8
            ?
#endif
            Deletion
        { get; }

        IDirectProcessInfo
#if CS8
            ?
#endif
            Clearing
        { get; }

        IDragDropProcessInfo
#if CS8
            ?
#endif
            DragDrop
        { get; }

        bool CanPaste(uint count);

        IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters);

        IProcess
#if CS8
            ?
#endif
            TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters);
    }

    public abstract class ProcessInfoNullableProcessFactory : IProcessFactory
    {
        public abstract IProcessCommand
#if CS8
            ?
#endif
            NewItemProcessCommand
        { get; }

        public abstract IProcessCommand
#if CS8
            ?
#endif
            RenameItemProcessCommand
        { get; }

        public abstract bool IsDisposed { get; }

        public virtual IRunnableProcessInfo
#if CS8
            ?
#endif
            Copy => null;

        public virtual IRunnableProcessInfo
#if CS8
            ?
#endif
            Cut => null;

        public virtual IDirectProcessInfo
#if CS8
            ?
#endif
            Recycling => null;

        public virtual IDirectProcessInfo
#if CS8
            ?
#endif
            Deletion => null;

        public virtual IDirectProcessInfo
#if CS8
            ?
#endif
            Clearing => null;

        public virtual IDragDropProcessInfo
#if CS8
            ?
#endif
            DragDrop => null;

        public abstract bool CanPaste(uint count);
        public abstract IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters);
        public abstract IProcess
#if CS8
            ?
#endif
            TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters);

        protected abstract void DisposeManaged();
        protected abstract void DisposeUnmanaged();
        
        public void Dispose()
        {
            if (IsDisposed)

                return;

            DisposeUnmanaged();
            DisposeManaged();
            GC.SuppressFinalize(this);
        }

        ~ProcessInfoNullableProcessFactory() => DisposeUnmanaged();
    }

    /*public static class ProcessFactory
    {
        public static IDirectProcessInfo DefaultProcessInfo { get; } = new _DefaultProcessFactory.DefaultDirectProcessInfo();

        public static IRunnableProcessInfo DefaultRunnableProcessFactoryProcessInfo { get; } = new _DefaultProcessFactory.DefaultRunnableProcessFactoryProcessInfo();

        public static IDragDropProcessInfo DefaultDragDropProcessInfo { get; } = new _DefaultProcessFactory.DragDropProcessInfo();

        private class _DefaultProcessFactory : IProcessFactory
        {
            internal class DefaultProcessInfoBase : IProcessInfoBase
            {
                bool IProcessInfoBase.CanRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => false;
            }

            internal class DefaultProcessInfo : DefaultProcessInfoBase, IProcessFactoryProcessInfo
            {
                bool IProcessFactoryProcessInfoBase.UserConfirmationRequired => false;

                string IProcessFactoryProcessInfoBase.GetUserConfirmationText() => null;
            }

            internal class DefaultDirectProcessInfo : DefaultProcessInfo, IDirectProcessInfo
            {
                IProcessParameters IDirectProcessInfo.GetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => throw new NotSupportedException();

                IProcessParameters IDirectProcessInfo.TryGetProcessParameters(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths) => null;
            }

            internal class DefaultRunnableProcessFactoryProcessInfo : DefaultProcessInfo, IRunnableProcessInfo
            {
                IProcessParameters IRunnableProcessInfo.GetProcessParameters(uint count) => throw new NotSupportedException();

                IProcessParameters IRunnableProcessInfo.TryGetProcessParameters(uint count) => null;

                void IRunnableProcessInfo.Run(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => throw new NotSupportedException();

                bool IRunnableProcessInfo.TryRun(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, uint count) => false;
            }

            internal class DragDropProcessInfo : DefaultProcessInfoBase, IDragDropProcessInfo
            {
                bool IDragDropProcessInfo.CanRun(IDictionary<string, object> data) => false;

                IDictionary<string, object> IDragDropProcessInfo.GetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects) => throw new NotSupportedException();

                IProcessParameters IDragDropProcessInfo.GetProcessParameters(IDictionary<string, object> data) => throw new NotSupportedException();

                IDictionary<string, object> IDragDropProcessInfo.TryGetData(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> paths, out DragDropEffects dragDropEffects)
                {
                    dragDropEffects = DragDropEffects.None;

                    return null;
                }

                IProcessParameters IDragDropProcessInfo.TryGetProcessParameters(IDictionary<string, object> data) => null;
            }

            bool DotNetFix.IDisposable.IsDisposed => false;

            IProcessCommand IProcessFactory.NewItemProcessCommand => null;

            IProcessCommand IProcessFactory.RenameItemProcessCommand => null;

            IRunnableProcessInfo IProcessFactory.Copy => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

            IRunnableProcessInfo IProcessFactory.Cut => ProcessFactory.DefaultRunnableProcessFactoryProcessInfo;

            IDirectProcessInfo IProcessFactory.Recycling => ProcessFactory.DefaultProcessInfo;

            IDirectProcessInfo IProcessFactory.Deletion => ProcessFactory.DefaultProcessInfo;

            IDirectProcessInfo IProcessFactory.Clearing => ProcessFactory.DefaultProcessInfo;

            IDragDropProcessInfo IProcessFactory.DragDrop => DefaultDragDropProcessInfo;

            bool IProcessFactory.CanPaste(uint count) => false;

            IProcess IProcessFactory.GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => throw new NotSupportedException();

            IProcess IProcessFactory.TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => null;

            void System.IDisposable.Dispose() { /* Left empty. */ /*}
        }

        public static IProcessFactory DefaultProcessFactory { get; } = new _DefaultProcessFactory();
    }*/

    public interface IProcessPathCollectionFactory
    {
        ProcessTypes<T>.IProcessQueue GetProcessCollection<T>() where T : IPathInfo;

        IProcessLinkedList<TItems, TError, TErrorItems, TAction> GetProcessLinkedList<TItems, TError, TErrorItems, TAction>() where TItems : IPathInfo where TErrorItems : IProcessErrorItem<TItems, TError, TAction>;
    }
}
