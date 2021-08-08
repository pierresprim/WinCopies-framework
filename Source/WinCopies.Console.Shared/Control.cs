using System;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Temp;

using static System.Console;

using static WinCopies.ThrowHelper;
using static WinCopies.Console.Console;

namespace WinCopies.Console
{
    public interface IControlBase
    {
        void Render();

        CursorPosition RelativeCursorPosition { get; }

        CursorPosition CursorPosition { get; }
    }

    public interface IControl : IControlBase
    {
        ConsoleColor Foreground { get; set; }

        ConsoleColor Background { get; set; }

        string Text { get; }

        event EventHandler<ControlElement> Rendered;
    }

    public abstract class ControlBase : IControlBase
    {
        internal int? maxLength;
        internal Screen screen;

        public Screen Screen => screen ?? (this as ControlElement)?.Parent?.screen;

        public CursorPosition RelativeCursorPosition { get; internal set; }

        public CursorPosition CursorPosition => Screen == null ? RelativeCursorPosition : new CursorPosition(Screen.Console.CursorPosition.Left + RelativeCursorPosition.Left, Screen.Console.CursorPosition.Top + RelativeCursorPosition.Top);

        public int GetMaxLength() => Screen == null ? maxLength ?? WindowWidth : Screen.GetMaxLength();

        public void Render()
        {
            if (!maxLength.HasValue && (Screen == null || Screen.Console == null))

                return;

            RenderOverride();
        }

        protected abstract void RenderOverride();
    }

    public abstract class ControlElement : ControlBase, IControl
    {
        private int _previousLength;
        private string _text;

        public Control Parent { get; internal set; }

        public string Text { get => _text; protected set => UpdateProperty(ref _text, value); }

        public virtual ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

        public virtual ConsoleColor Background { get; set; } = ConsoleColor.Black;

        public event EventHandler<ControlElement> Rendered;

        protected void UpdateProperty<T>(ref T value, in T newValue) => ConsoleExtensions.UpdateProperty(this, ref value, newValue);

        protected sealed override void RenderOverride() => Render(null);

        internal void Render(int? previousLength)
        {
            if (Parent != null && !Parent.Rendering)
            {
                Rendered?.Invoke(this, EventArgs.Empty);

                return;
            }

            string text = RenderOverride2();

            if (text != null)
            {
                foreach (char c in text)

                    if (c == '\n')

                        throw new InvalidOperationException("The rendered text contains \\n character.");

                int maxLength = GetMaxLength();

                if (previousLength.HasValue)
                {
                    if (text.Length > maxLength - previousLength.Value)

                        throw new InvalidOperationException("The rendered text is too big.");
                }

                else

                    _ = UtilHelpers.TruncateIfLonger(ref text, maxLength);

                //if (text.Length > maxLength)

                //    text = text.Truncate(maxLength);
            }

            this.ResetControlText(_previousLength);

            ForegroundColor = Foreground;

            BackgroundColor = Background;

            _previousLength = text == null ? 0 : text.Length;

            SetCursorPosition(CursorPosition);

            Write(_text = text);

            if (Parent == null)

                WriteLine();

            _ = this is SelectableControl _selectable && _selectable.IsSelected;

            SetCursorPosition(this is SelectableControl selectable && selectable.IsSelected ? CursorPosition : Screen == null ? new CursorPosition(0, CursorPosition.Top + 1) : Screen.GetCursorPosition(false));

            ResetColor();
        }

        protected abstract string RenderOverride2();
    }

    public class Control : ControlBase
    {
        private Collections.Generic.IEnumerable<ControlElement> _controls;

        protected internal Collections.Generic.IEnumerable<ControlElement> Controls => _controls;

        public bool Rendering { get; private set; }

        public Control(in Collections.Generic.IEnumerable<ControlElement> controls)
        {
            ThrowIfNull(controls, nameof(controls));

            foreach (ControlElement control in controls)
            {
                if (control == null)

                    throw new InvalidOperationException("At least one of the given controls is null.");

                if (control.Parent != null)

                    throw new InvalidOperationException("The given control already has a parent.");

                void renderedItem(ControlElement sender, EventArgs e) => OnRenderedItem(sender);

                control.Parent = this;

                control.Rendered += renderedItem;

                control.screen = Screen;
            }

            _controls = controls;
        }

        protected virtual void OnRenderedItem(ControlElement control) => Render(control);

        protected sealed override void RenderOverride() => Render(null);

        public void Render(ControlElement control)
        {
            Rendering = true;

            int length = 0;

            foreach (ControlElement _control in _controls)

                if (_control?.Text != null)

                    length += _control.Text.Length;

            this.ResetControlText(length);

            int previousLength = 0;

            try
            {
                foreach (ControlElement _control in _controls)

                    if (_control != null)
                    {
                        _control.RelativeCursorPosition = new CursorPosition(previousLength, RelativeCursorPosition.Top);

                        _control.Render(previousLength);

                        previousLength += _control.Text == null ? 0 : _control.Text.Length;
                    }
            }

            finally
            {
                Rendering = false;
            }

            if (control != null)

                SetCursorPosition(control.CursorPosition);
        }
    }

    public abstract class SelectableControl : ControlElement
    {
        internal ILinkedListNode<SelectableControl> Node { get; set; }

        public bool IsSelected { get; private set; }

        internal void Select()
        {
            SetCursorPosition(CursorPosition);

            if (Screen != null)

                Screen.SelectedItem = this;

            IsSelected = true;

            OnSelect();
        }

        protected virtual void OnSelect() { }

        protected internal abstract bool OnSelect(ConsoleKeyInfo key);

        internal void Deselect()
        {
            SetCursorPosition(CursorPosition);

            if (Screen?.SelectedItem == this)

                Screen.SelectedItem = null;

            IsSelected = false;

            OnDeselect();
        }

        protected internal virtual void OnDeselect() { }
    }
}
