//* Copyright © Pierre Sprimont, 2020
// *
// * This file is part of the WinCopies Framework.
// *
// * The WinCopies Framework is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * The WinCopies Framework is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

//using System;
//using System.Windows;
//using System.Windows.Input;

//namespace WinCopies.GUI.Controls
//{
//    public class InlineTextEditor : System.Windows.Controls.Control
//    {
//        /// <summary>
//        /// Identifies the <see cref="IsEditing"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(InlineTextEditor), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
//        {
//            d.SetValue(EditProperty, (bool)e.NewValue ? d.GetValue(TextProperty) : null);
//        }));

//        public bool IsEditing { get => (bool)GetValue(IsEditingProperty); set => SetValue(IsEditingProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="Edit"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty EditProperty = DependencyProperty.Register(nameof(Edit), typeof(string), typeof(InlineTextEditor), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
//        {
//            if (!(bool)d.GetValue(IsEditingProperty))

//                throw new InvalidOperationException($"The {nameof(InlineTextEditor)} is not currently in edit mode.");
//        }));

//        public string Edit { get => (string)GetValue(EditProperty); set => SetValue(EditProperty, value); }

//        /// <summary>
//        /// Identifies the <see cref="Text"/> dependency property.
//        /// </summary>
//        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(InlineTextEditor));

//        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }



//        static InlineTextEditor() => DefaultStyleKeyProperty.OverrideMetadata(typeof(InlineTextEditor), new FrameworkPropertyMetadata(typeof(InlineTextEditor)));

//        public InlineTextEditor()
//        {
//            CommandBindings.Add(new CommandBinding(WinCopies.Commands.Commands.CommonCommand, (object sender, ExecutedRoutedEventArgs e) =>
//        {
//            OnCommandExecuted(e);

//            OnEdit();
//        }, (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true));
//        }



//        protected virtual void OnEdit() => IsEditing = true;

//        protected virtual void OnValidate()
//        {
//            Text = Edit;

//            IsEditing = false;
//        }

//        protected virtual void OnCancelEdit()
//        {
//            IsEditing = false;
//        }

//        protected virtual void OnCommandExecuted(ExecutedRoutedEventArgs e) => e.Handled = true;

//        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
//        {
//            base.OnMouseDoubleClick(e);

//            OnEdit();
//        }
//    }
//}
