using Microsoft.WindowsAPICodePack.Win32Native.Shell.DesktopWindowManager;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Collections.Generic;

using static System.Console;

using static WinCopies.ThrowHelper;
using static WinCopies.UtilHelpers;
using static WinCopies.Console.Console;

namespace WinCopies.Console
{
    public interface IScreen : IUIntCountableEnumerable<IControlBase>
    {

    }

    public class Screen : IScreen
    {
        private readonly ILinkedList<ControlBase> _controls = new Collections.DotNetFix.Generic.LinkedList<ControlBase>();
        private readonly ILinkedList<SelectableControl> _selectableControls = new Collections.DotNetFix.Generic.LinkedList<SelectableControl>();

        public IReadOnlyLinkedList<ControlBase> Controls { get; }

        public IReadOnlyLinkedList<SelectableControl> SelectableControls { get; }

        public SelectableControl SelectedItem { get; internal set; }

        public Console Console { get; internal set; }

        public uint Count => _controls.Count;

        internal Screen(in Console console)
        {
            Console = console;

            Controls = new ReadOnlyLinkedList<ControlBase>(_controls);

            SelectableControls = new ReadOnlyLinkedList<SelectableControl>(_selectableControls);
        }

        public int GetMaxLength() => Console.GetMaxLength();

        protected internal CursorPosition GetCursorPosition(in bool relative)
        {
            int getLastTopPosition() => _controls.Last.Value.RelativeCursorPosition.Top;

            return _controls.Count == 0u ? relative || Console == null ? new CursorPosition() : new CursorPosition(Console.CursorPosition.Left, Console.CursorPosition.Top) : relative || Console == null ? new CursorPosition(0, getLastTopPosition() + 1) : new CursorPosition(Console.CursorPosition.Left, getLastTopPosition() + Console.CursorPosition.Top + 1);
        }

        public T Add<T>(in T control) where T : ControlBase
        {
            ThrowIfNull(control, nameof(control));

            control.screen = this;

            control.RelativeCursorPosition = GetCursorPosition(true);

            control.Render();

            _ = _controls.AddLast(control);

            return control;
        }

        public Label AddLabel(in string text)
        {
            Label label = Add(new Label());

            label.Text = text;

            return label;
        }

        public T AddSelectable<T>(in T control) where T : SelectableControl
        {
            if (control.Parent == null)

                _ = Add(control);

            control.Node = _selectableControls.AddLast(control);

            return control;
        }

        public (ConsoleKeyInfo?, bool) SelectFirst() => _selectableControls.Count > 0u ? Select(_selectableControls.First.Value) : (null, false);

        public (ConsoleKeyInfo?, bool) Select(SelectableControl control)
        {
            if ((control ?? throw GetArgumentNullException(nameof(control))).Screen == this)
            {
                if (control.Node?.List != _selectableControls)

                    throw new InvalidOperationException("The control was not added to selectable controls.");

                control.Select();

                (ConsoleKeyInfo? key, bool? result) selectNext(in Func<System.Collections.Generic.IEnumerable<ControlElement>> func, in ConsoleKeyInfo _key)
                {
                    if (control.Parent != null)
                    {
                        System.Collections.Generic.IEnumerable<ControlElement> enumerable = func();

                        //foreach (ControlElement _control in enumerable)

                        //    if (_control is SelectableControl selectableControl && selectableControl.Node != null /*&& selectableControl.Index > control.Index*/)

                        //        return (Select(selectableControl), true);

                        foreach (ControlElement __control in enumerable)

                            if (__control is SelectableControl _selectableControl && _selectableControl.Node != null && _selectableControl != control)

                                return Select(_selectableControl);
                    }

                    return (_key, null);
                }

                ConsoleKeyInfo key;

                (ConsoleKeyInfo? __key, bool? result) tuple;

                void setTuple(in Func<System.Collections.Generic.IEnumerable<ControlElement>> _func) => tuple = selectNext(_func, key);

                while (true)

                    switch ((key = ReadKey(true)).Key)
                    {
                        case ConsoleKey.Tab:

                            control.Deselect();

                            return _selectableControls.Count > 1u ? Select(key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? control.Node.Previous == null ? _selectableControls.Last.Value : control.Node.Previous.Value : control.Node.Next == null ? _selectableControls.First.Value : control.Node.Next.Value) : (key, true);

                        case ConsoleKey.LeftArrow:

                            control.Deselect();

                            setTuple(() => new Enumerable<ControlElement>(() => control.Parent.Controls.GetReversedEnumerator()));

                            if (tuple.result == false || (tuple.__key.HasValue && tuple.__key.Value.Key == ConsoleKey.Escape))

                                return (tuple.__key, tuple.result.Value);

                            break;

                        case ConsoleKey.RightArrow:

                            control.Deselect();

                            setTuple(() => control.Parent.Controls);

                            if (tuple.result == false || (tuple.__key.HasValue && tuple.__key.Value.Key == ConsoleKey.Escape))

                                return (tuple.__key, tuple.result.Value);

                            break;

                        case ConsoleKey.Enter:
                        case ConsoleKey.Escape:

                            if (key.Key == ConsoleKey.Enter && Console?.Looping == true)

                                break;

                            control.Deselect();

                            SetCursorPosition(GetCursorPosition(false));

                            if (key.Key == ConsoleKey.Escape && Console?.Looping == false)
                            {
                                Console console = Console;

                                console.Commands[0].Select();

                                if (console != Console)
                                {
                                    console.Commands[0].Deselect();

                                    return (key, true);
                                }

                                ConsoleKeyInfo _key;

                                while ((_key = ReadKey()).Key != ConsoleKey.Escape)
                                {
                                    _ = Console.Commands[0].OnSelect(_key);

                                    if (console != Console)
                                    {
                                        console.Commands[0].Deselect();

                                        return (key, true);
                                    }
                                }

                                console.Commands[0].Deselect();
                            }

                            return (key, Console?.Looping == true);

                        default:

                            bool value = control.OnSelect(key);

                            control.Deselect();

                            if (!value)

                                return (key, value);

                            control.Select();

                            break;
                    }
            }

            return (null, false);
        }

        public IUIntCountableEnumerator<IControlBase> GetEnumerator() => _controls.GetEnumerator();

        internal virtual void Render()
        {
            foreach (IControlBase control in _controls)

                control.Render();
        }

        System.Collections.Generic.IEnumerator<IControlBase> System.Collections.Generic.IEnumerable<IControlBase>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed class Console
    {
        private bool _initialized;
        private Screen screen;

        public bool Looping { get; private set; }

        public Screen Screen
        {
            get => screen; set
            {
                if (!_initialized)

                    throw GetNotInitializedException();

                _ = UpdateValue(ref screen, value, (Screen oldScreen, Screen newScreen) =>
  {
      if (oldScreen != null)
      {
          oldScreen.Console = null;

          oldScreen.SelectedItem?.Deselect();
      }

      if (newScreen != null)
      {
          newScreen.Console = this;

          _Render();
      }
  });
            }
        }

        public ImmutableArray<Button> Commands { get; } = new Button[1] { new ActionButton(() => { Environment.Exit(0); return false; }) { Text = "Exit", maxLength = 6 } }.ToImmutableArray();

        public CursorPosition CursorPosition { get; } = new CursorPosition(0, 2);

        public int Margin { get; }

        public static Console Instance { get; } = new Console();

        private Console()
        {

        }

        public static void SetCursorPosition(in CursorPosition cursorPosition) => System.Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);

        public void Render()
        {
            ThrowIfNoScreen();

            _Render();
        }

        private void _Render()
        {
            Clear();

            Commands[0].Render();

            screen.Render();
        }

        private static InvalidOperationException GetNotInitializedException() => new
#if !CS9
            InvalidOperationException
#endif
            ("The console is not initialized.");

        public int GetMaxLength() => WindowWidth - CursorPosition.Left - Margin;

        public void Initialize(in CursorPosition cursorPosition)
        {
            if (_initialized)

                throw new InvalidOperationException("The console was already initialized.");

            _initialized = true;

            WindowHeight = cursorPosition.Top;
            WindowWidth = cursorPosition.Left;

            //DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            Microsoft.WindowsAPICodePack.Win32Native.Console.Console.DeleteConsoleMenu(new KeyValuePair<SystemMenuCommands, MenuFlags>(SystemMenuCommands.Maximize, MenuFlags.ByCommand), new KeyValuePair<SystemMenuCommands, MenuFlags>(SystemMenuCommands.Size, MenuFlags.ByCommand));

            //Console.WriteLine("Yes, its fixed!");
            //Console.ReadLine();

            // System.Console.SetWindowSize(300, 200);
        }

        public Screen GetScreen() => _initialized ? new Screen(this) : throw GetNotInitializedException();

        private void ThrowIfNoScreen()
        {
            if (screen == null)

                throw new InvalidOperationException("No screen registered.");
        }

        public void Loop(in SelectableControl control = null)
        {
            ThrowIfNoScreen();

            (ConsoleKeyInfo? key, bool result) tuple;

            try
            {
                Looping = true;

                while (true)
                {
                    if (screen == null)

                        return;

                    if (!(tuple = control == null ? screen.SelectFirst() : screen.Select(control)).result)

                        return;

                    if (screen == null)

                        return;

                    if (tuple.key.HasValue && tuple.key.Value.Key == ConsoleKey.Escape)
                    {
                        Commands[0].Select();

                        bool loop()
                        {
                            while (true)
                            {
                                if (screen == null)

                                    return true;

                                switch ((tuple.key = ReadKey(true)).Value.Key)
                                {
                                    case ConsoleKey.Escape:

                                        Commands[0].Deselect();

                                        return true;

                                    default:

                                        return Commands[0].OnSelect(tuple.key.Value);
                                }
                            }
                        }

                        if (!loop())

                            break;
                    }
                }
            }

            finally
            {
                Looping = false;
            }
        }
    }
}
