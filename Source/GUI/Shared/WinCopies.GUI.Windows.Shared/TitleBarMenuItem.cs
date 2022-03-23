using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

using WinCopies.Collections;
using WinCopies.Desktop;
using WinCopies.Util.Data;

using static WinCopies.ThrowHelper;

namespace WinCopies.GUI.Windows
{
    public enum XButton : sbyte
    {
        One = 1,

        Two = 2
    }

    public enum XButtonClick : sbyte
    {
        Down = 1,

        Up = 2,

        DoubleClick = 3
    }

    public class TitleBarMenuItem : ViewModelBase, ICommandSource, DotNetFix.IDisposable
    {
        private ICommand _command;
        private object _commandParameter;
        private IInputElement _commandTarget;
        private string _header;
        private bool _isEnabled = true;

        public uint Id { get; internal set; }

        public TitleBarMenuItemQueue Collection { get; internal set; }

        public string Header { get => _header; set => UpdateValue(ref _header, value, nameof(Header)); }

        public bool IsEnabled { get => _isEnabled; set => _IsEnabled = Command == null ? value : throw new InvalidOperationException("Command is not null."); }

        protected bool _IsEnabled { set => UpdateValue(ref _isEnabled, value, nameof(IsEnabled)); }

        public ICommand Command
        {
            get => _command;

            set
            {
                ICommand oldValue = _command;

#if WinCopies4
                if (
#endif
                UpdateValue(ref _command, value, nameof(Command))
#if WinCopies4
                    )
#else
                    ;

                if (oldValue != value)
#endif
                    OnCommandChanged(oldValue);
            }
        }

        public object CommandParameter { get => _commandParameter; set => UpdateValue(ref _commandParameter, value, nameof(CommandParameter)); }

        public IInputElement CommandTarget { get => _commandTarget; set => UpdateValue(ref _commandTarget, value, nameof(CommandTarget)); }

        public bool IsDisposed => Command == null;

        public event EventHandler Click;

        internal virtual void OnClick()
        {
            _ = Command?.TryExecute(CommandParameter, CommandTarget);

            Click?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCommandChanged(ICommand oldValue)
        {
            void onCommandCanExecuteChanged(object sender, EventArgs e) => OnCommandCanExecuteChanged(e);

            if (oldValue != null)

                oldValue.CanExecuteChanged -= onCommandCanExecuteChanged;

            if (_command != null)

                _command.CanExecuteChanged += onCommandCanExecuteChanged;

            _OnCommandCanExecuteChanged();
        }

        private void _OnCommandCanExecuteChanged() => _IsEnabled = _command?.CanExecute(_commandParameter, _commandTarget)??true;

        protected virtual void OnCommandCanExecuteChanged(EventArgs e) => _OnCommandCanExecuteChanged();

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)

                return;

            if (disposing)
            {
                if (Collection == null)

                    Id = 0u;

                _command = null;
                _commandParameter = null;
                _commandTarget = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }

    public class TitleBarMenuItemQueue : Collections.DotNetFix.Generic.EnumerableQueueCollection<TitleBarMenuItem>
    {
        public uint LastId { get; private set; }

        protected override void EnqueueItem(TitleBarMenuItem item)
        {
            ThrowIfNull(item, nameof(item));

            if (item.Collection == null)
            {
                item.Collection = this;
                item.Id = ++LastId;

                base.EnqueueItem(item);

                LastId = item.Id;
            }

            else

                throw new ArgumentException("The given item is already in a collection.");
        }

        private static void ResetItem(in TitleBarMenuItem item)
        {
            item.Collection = null;
            item.Id = 0u;
        }

        protected override bool TryDequeueItem(
#if CS8
            [MaybeNullWhen(false)]
#endif
            out TitleBarMenuItem result)
        {
            if (base.TryDequeueItem(out result))
            {
                ResetItem(result);

                return true;
            }

            return false;
        }

        protected override TitleBarMenuItem DequeueItem()
        {
            TitleBarMenuItem item = base.DequeueItem();

            ResetItem(item);

            return item;
        }

        protected override void ClearItems()
        {
            while (((IUIntCountable)InnerQueue).Count > 0)

                _ = DequeueItem();
        }
    }
}
