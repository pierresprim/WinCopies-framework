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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Collections.Generic;
using WinCopies.Commands;
using WinCopies.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public class CommandSourceControl<T> : Control, ICommandSource<T>
    {
        public static readonly DependencyProperty CommandProperty = Register<ICommand, CommandSourceControl<T>>(nameof(Command));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty CommandParameterProperty = Register<T, CommandSourceControl<T>>(nameof(CommandParameter));

        public T CommandParameter { get => (T)GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = Register<IInputElement, CommandSourceControl<T>>(nameof(CommandParameter));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

#if !CS8
        object ICommandSource.CommandParameter => CommandParameter;
#endif
    }

    public class ItemsControl<TEnumerable> : ItemsControl where TEnumerable : System.Collections.IEnumerable
    {
        public new TEnumerable ItemsSource { get => base.ItemsSource == null ? default : (TEnumerable)base.ItemsSource; set => base.ItemsSource = value; }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue == null)

                base.OnItemsSourceChanged(oldValue, null);

            else

                base.OnItemsSourceChanged(oldValue, newValue is TEnumerable enumerable ? enumerable : throw new ArgumentException($"The given value is not an {typeof(TEnumerable).Name}.", nameof(newValue)));
        }
    }

    public class CommandItemsControl<TEnumerable, TCommandParameter> : ItemsControl<TEnumerable>, ICommandSource<TCommandParameter> where TEnumerable : System.Collections.IEnumerable
    {
        public static readonly DependencyProperty CommandProperty = Register<ICommand, CommandItemsControl<TEnumerable, TCommandParameter>>(nameof(Command));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty CommandParameterProperty = Register<TCommandParameter, CommandItemsControl<TEnumerable, TCommandParameter>>(nameof(CommandParameter));

        public TCommandParameter CommandParameter { get => (TCommandParameter)GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = Register<IInputElement, CommandItemsControl<TEnumerable, TCommandParameter>>(nameof(CommandTarget));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

#if !CS8
        object ICommandSource.CommandParameter => CommandParameter;
#endif
    }

    public class NavigationButton : CommandItemsControl<ILinkedListEnumerable, object>
    {
        public static readonly DependencyProperty GoBackButtonStyleProperty = Register<Style, NavigationButton>(nameof(GoBackButtonStyle));

        public Style GoBackButtonStyle { get => (Style)GetValue(GoBackButtonStyleProperty); set => SetValue(GoBackButtonStyleProperty, value); }

        public static readonly DependencyProperty GoForwardButtonStyleProperty = Register<Style, NavigationButton>(nameof(GoForwardButtonStyle));

        public Style GoForwardButtonStyle { get => (Style)GetValue(GoForwardButtonStyleProperty); set => SetValue(GoForwardButtonStyleProperty, value); }

        public NavigationButton()
        {
            _ = CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, (object sender, ExecutedRoutedEventArgs e) => OnBrowseBack(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanBrowseBack(e)));

            _ = CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward, (object sender, ExecutedRoutedEventArgs e) => OnBrowseForward(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanBrowseForward(e)));

            _ = CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, (object sender, ExecutedRoutedEventArgs e) => OnGoToPage(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanGoToPage(e)));
        }

        protected virtual void OnCanBrowseBack(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ItemsSource?.Current?.CanMovePreviousFromCurrent == true;

            e.Handled = true;
        }

        protected virtual void OnBrowseBack(ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (ItemsSource == null)

                return;

            if (ItemsSource.MovePrevious())

                _ = Command?.TryExecute(CommandParameter, CommandTarget);
        }

        protected virtual void OnCanBrowseForward(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ItemsSource?.Current?.CanMoveNextFromCurrent == true;

            e.Handled = true;
        }

        protected virtual void OnBrowseForward(ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (ItemsSource == null)

                return;

            if (ItemsSource.MoveNext())

                _ = Command?.TryExecute(CommandParameter, CommandTarget);
        }

        protected virtual void OnCanGoToPage(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        protected virtual void OnGoToPage(ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (ItemsSource == null)

                return;

            ItemsSource.UpdateCurrent(((ILinkedListNodeEnumerable)e.Parameter).Node);

            _ = Command?.TryExecute(CommandParameter, CommandTarget);
        }
    }
}
