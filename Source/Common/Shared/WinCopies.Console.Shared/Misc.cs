using System;

using WinCopies.Collections;

using static System.Console;

using static WinCopies.Console.Console;
using static WinCopies.ThrowHelper;

namespace WinCopies.Console
{
    public static class ConsoleExtensions
    {
        public static void UpdateProperty<T>(this IControlBase control, ref T value, in T newValue)
        {
            ThrowIfNull(control, nameof(control));

            if (!Equals(value, newValue))
            {
                value = newValue;

                control.Render();
            }
        }

        public static void ResetControlText(this IControlBase control, in int previousLength)
        {
            ThrowIfNull(control, nameof(control));

            SetCursorPosition(control.CursorPosition);

            ResetColor();

            Write(new string(' ', previousLength));

            SetCursorPosition(control.CursorPosition);
        }
    }

    public struct CursorPosition
    {
        public int Left { get; }

        public int Top { get; }

        public CursorPosition(in int left, in int top)
        {
            Left = left;
            Top = top;
        }
    }

    public class ScrollableSelect : SelectableControl, IControl
    {
        private ILoopEnumerator items;

        public ILoopEnumerator Items { get => items; set => UpdateProperty(ref items, value ?? throw GetArgumentNullException(nameof(value))); }

        public ScrollableSelect(in ConsoleColor foreground, in ConsoleColor background)
        {
            Foreground = foreground;

            Background = background;
        }

        public ScrollableSelect() : this(ConsoleColor.Black, ConsoleColor.Gray) { }

        protected override string RenderOverride2() => items?.Current.ToString();

        protected internal override bool OnSelect(ConsoleKeyInfo key)
        {
            void move(in Action action)
            {
                action();

                Render();
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:

                    move(items.MovePrevious);

                    break;

                case ConsoleKey.DownArrow:

                    move(items.MoveNext);

                    break;
            }

            return true;
        }
    }
}
