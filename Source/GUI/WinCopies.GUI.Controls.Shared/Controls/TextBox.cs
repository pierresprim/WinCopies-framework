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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.GUI.Controls.Models;
using WinCopies.Util.Commands;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Represents a <see cref="System.Windows.Controls.TextBox"/> that can display items on the left and right of the text.
    /// </summary>
    public class TextBox : System.Windows.Controls.TextBox
    {
        /// <summary>
        /// Identifies the <see cref="LeftItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsProperty = DependencyProperty.Register(nameof(LeftItems), typeof(IEnumerable), typeof(TextBox));

        public IEnumerable LeftItems { get => (IEnumerable)GetValue(LeftItemsProperty); set => SetValue(LeftItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightItemsProperty = DependencyProperty.Register(nameof(RightItems), typeof(IEnumerable), typeof(TextBox));

        public IEnumerable RightItems { get => (IEnumerable)GetValue(RightItemsProperty); set => SetValue(RightItemsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LeftItemsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsStyleProperty = DependencyProperty.Register(nameof(LeftItemsStyle), typeof(Style), typeof(TextBox));

        public Style LeftItemsStyle { get => (Style)GetValue(LeftItemsStyleProperty); set => SetValue(LeftItemsStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItemsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightItemsStyleProperty = DependencyProperty.Register(nameof(RightItemsStyle), typeof(Style), typeof(TextBox));

        public Style RightItemsStyle { get => (Style)GetValue(RightItemsStyleProperty); set => SetValue(RightItemsStyleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="LeftItemsTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftItemsTemplateSelectorProperty = DependencyProperty.Register(nameof(LeftItemsTemplateSelector), typeof(DataTemplateSelector), typeof(TextBox));

        public DataTemplateSelector LeftItemsTemplateSelector { get => (DataTemplateSelector)GetValue(LeftItemsTemplateSelectorProperty); set => SetValue(LeftItemsTemplateSelectorProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RightItemsTemplateSelector"/> dependency property.
        /// </summary>

        public static readonly DependencyProperty RightItemsTemplateSelectorProperty = DependencyProperty.Register(nameof(RightItemsTemplateSelector), typeof(DataTemplateSelector), typeof(TextBox));

        public DataTemplateSelector RightItemsTemplateSelector { get => (DataTemplateSelector)GetValue(RightItemsTemplateSelectorProperty); set => SetValue(RightItemsTemplateSelectorProperty, value); }

        static TextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
    }

    public class ButtonTextBox : TextBox
    {
        /// <summary>
        /// Identifies the <see cref="Buttons"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(IEnumerable<IButtonModel>), typeof(ButtonTextBox));

        public IEnumerable<IButtonModel> Buttons { get => (IEnumerable<IButtonModel>)GetValue(ButtonsProperty); set => SetValue(ButtonsProperty, value); }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonTextBox"/>.
        /// </summary>
        public ButtonTextBox() => AddButtons();

        /// <summary>
        /// Adds the default buttons to the <see cref="Buttons"/> property.
        /// </summary>
        protected virtual void AddButtons()
        {
            Buttons = new ObservableCollection<IButtonModel>() { new ButtonModel<Bitmap>(Icons.Properties.Resources.cancel) { Command = DialogCommands.Cancel, CommandTarget = this } };

            _ = CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, (object sender, ExecutedRoutedEventArgs e) => OnCancel(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanCancel(e)));
        }

        /// <summary>
        /// Determines whether the <see cref="DialogCommands.Cancel"/> command can be executed. This method handles the command.
        /// </summary>
        /// <param name="e">The event args of the command.</param>
        protected virtual void OnCanCancel(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(IsReadOnly || string.IsNullOrEmpty(Text));

            e.Handled = true;
        }

        /// <summary>
        /// Clears <see cref="System.Windows.Controls.TextBox.Text"/>. This method is called on <see cref="DialogCommands.Cancel"/>. This method handles the command.
        /// </summary>
        /// <param name="e">The event args of the command.</param>
        protected virtual void OnCancel(ExecutedRoutedEventArgs e)
        {
            Clear();

            e.Handled = true;
        }
    }
}
