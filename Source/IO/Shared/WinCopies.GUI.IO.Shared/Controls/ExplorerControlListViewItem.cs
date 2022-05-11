/* Copyright © Pierre Sprimont, 2020
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Desktop;

using static System.Windows.Input.Key;
using static System.Windows.Input.Keyboard;

namespace WinCopies.GUI.IO.Controls
{
    public class ContextMenuRequestedEventArgs : RoutedEventArgs
    {
        public bool Ctrl { get; }

        public bool Shift { get; }

        public ContextMenuRequestedEventArgs(in bool ctrl = false, in bool shift = false)
        {
            Ctrl = ctrl;

            Shift = shift;
        }
    }

    public delegate void ContextMenuRequestedEventHandler(object sender, ContextMenuRequestedEventArgs e);

    public class ExplorerControlListViewItem : GUI.Controls.ListViewItem, ICommandSource
    {
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ExplorerControlListViewItem));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ExplorerControlListViewItem));

        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(ExplorerControlListViewItem));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        public static readonly RoutedEvent ContextMenuRequestedEvent = Util.Desktop.UtilHelpers.RegisterRoutedEvent<ContextMenuRequestedEventHandler, ExplorerControlListView>(nameof(ContextMenuRequested), RoutingStrategy.Bubble);

        public event ContextMenuRequestedEventHandler ContextMenuRequested { add => AddHandler(ContextMenuRequestedEvent, value); remove => RemoveHandler(ContextMenuRequestedEvent, value); }

        //static ExplorerControlListViewItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExplorerControlListViewItem), new FrameworkPropertyMetadata(typeof(ExplorerControlListViewItem)));

        private bool TryExecuteCommand() => CommandTarget == null ? Command.TryExecute(CommandParameter) : Command.TryExecute(CommandParameter, CommandTarget);

        /// <summary>
        /// Raises the <see cref="Control.MouseDoubleClick"/> routed event, tries to execute the command and, if succeeded, handles the event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (TryExecuteCommand())

                e.Handled = true;
        }

        protected virtual ContextMenuRequestedEventArgs GetContextMenuRequestedEventArgs()
        {
#if CS8
            static
#endif
            bool isKeyDown(in Key leftKey, in Key rightKey) => IsKeyDown(leftKey) || IsKeyDown(rightKey);

            return new
#if !CS9
            ContextMenuRequestedEventArgs
#endif
            (isKeyDown(LeftCtrl, RightCtrl), isKeyDown(LeftShift, RightShift))
            { RoutedEvent = ContextMenuRequestedEvent };
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            _ = CaptureMouse();
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();

                RaiseEvent(GetContextMenuRequestedEventArgs());
            }
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyDown attached event reaches an element in its route that is derived from this class. If the <see cref="KeyEventArgs.Key"/> property of <paramref name="e"/> is defined to <see cref="Key.Enter"/>, tries to execute the command and, if succeeded, handles the event. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Enter:

                    if (TryExecuteCommand())

                        e.Handled = true;

                    break;

                case Apps:

                    RaiseEvent(GetContextMenuRequestedEventArgs());

                    break;
            }
        }
    }
}
