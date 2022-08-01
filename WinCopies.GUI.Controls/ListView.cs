/* Copyright © Pierre Sprimont, 2019
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
using WinCopies.Util;
using WinCopies.Util.Commands;

namespace WinCopies.GUI.Controls
{
    public class ListView : System.Windows.Controls.ListView, IScrollable, ISelectionIndexableSettableSelector
    {
        public ScrollViewer ScrollHost { get; private set; } = null;

        ///// <summary>
        ///// Identifies the <see cref="Command"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ListView), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        //{

        //    RoutedCommand command = null;

        //    ListView listView = ((ListView)d);

        //    void tryRemove()

        //    {

        //        InputBindingCollection inputBindings = listView.InputBindings;

        //        for (int i = 0; i < inputBindings.Count; i++)

        //        {

        //            if (((InputBinding)inputBindings[i]).Command == command)

        //            {

        //                inputBindings.RemoveAt(i);

        //                break;

        //            }

        //        }

        //    }

        //    if ((command = e.OldValue as RoutedCommand) != null)

        //        tryRemove();

        //    if ((command = e.NewValue as RoutedCommand) != null)

        //        foreach (InputGesture inputGesture in command.InputGestures)

        //            listView.InputBindings.Add(new InputBinding(command, inputGesture));

        //}));

        ///// <summary>
        ///// Gets or sets the command of this <see cref="ListView"/>. This is a dependency property.
        ///// </summary>
        //public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        ///// <summary>
        ///// Identifies the <see cref="CommandParameter"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ListView), new PropertyMetadata(null));

        ///// <summary>
        ///// Gets or sets the command parameter for the <see cref="Command"/> property. This is a dependency property.
        ///// </summary>
        //public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        ///// <summary>
        ///// Identifies the <see cref="CommandTarget"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(ListView), new PropertyMetadata(null));

        //public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        static ListView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(typeof(ListView)));// ViewProperty.OverrideMetadata(typeof(ListView), new FrameworkPropertyMetadata(ViewProperty.DefaultMetadata.DefaultValue, (DependencyObject d, DependencyPropertyChangedEventArgs e) => { if (e.OldValue != null) ((GridView)e.OldValue).ListView = null; ((GridView)e.NewValue).ListView = (ListView)d; }));

        //public ListView()
        //{
        //MouseDoubleClick += ListView_MouseDoubleClick;

        //KeyDown += ListView_KeyDown;
        //}

        //private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (Command == null) return;

        //    Command.TryExecute(CommandParameter, CommandTarget);
        //}

        //private void ListView_KeyDown(object sender, KeyEventArgs e)
        //{

        //    if (Command == null) return;

        //    if (KeyCommandHelper.CanRaiseCommand(this, e))

        //        Command.Execute(CommandParameter, CommandTarget);

        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollHost = Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
        }
    }
}
