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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WinCopies.GUI.Controls
{

    // todo : not fully tested yet.

    /// <summary>
    /// An improved <see cref="System.Windows.Controls.TextBox"/> with placeholder.
    /// </summary>
    public partial class TextBox : System.Windows.Controls.TextBox
    {
        private bool _isAutomaticallyTextChanging = false;

        /// <summary>
        /// Identifies the <see cref="PlaceholderProperties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderPropertiesProperty = DependencyProperty.Register("PlaceholderProperties", typeof(PlaceholderProperties), typeof(TextBox), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
#if DEBUG 
            System.Console.WriteLine("PlaceholderPropertiesProperty value changed.");
#endif 

            PlaceholderMode placeholderMode = (PlaceholderMode)d.GetValue(PlaceholderModeProperty);

            bool isFocused = (bool)d.GetValue(IsFocusedProperty);

            string realText = (string)d.GetValue(RealTextProperty);

            if (placeholderMode == PlaceholderMode.OnFocus && !isFocused && (string.IsNullOrEmpty(realText) || string.IsNullOrWhiteSpace(realText)))

            {
#if DEBUG 
                Debug.WriteLine(1);
#endif 

                ((TextBox)d).SetIsPlaceholderActiveValue(true);
            }

            else if (placeholderMode == PlaceholderMode.OnTextChange && (string.IsNullOrEmpty(realText) || string.IsNullOrWhiteSpace(realText)))

            {
#if DEBUG 
                System.Console.WriteLine(2);
#endif 

                ((TextBox)d).SetIsPlaceholderActiveValue(true);
            }

        }));

        /// <summary>
        /// Gets or sets the <see cref="Controls.PlaceholderProperties"/>. This is a dependency property.
        /// </summary>
        public PlaceholderProperties PlaceholderProperties { get => (PlaceholderProperties)GetValue(PlaceholderPropertiesProperty); set => SetValue(PlaceholderPropertiesProperty, value); }

        /// <summary>
        /// Identifies the <see cref="PlaceholderMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderModeProperty = DependencyProperty.Register("PlaceholderMode", typeof(PlaceholderMode), typeof(TextBox), new PropertyMetadata(PlaceholderMode.OnFocus));

        /// <summary>
        /// Gets or sets the <see cref="Controls.PlaceholderMode"/>. This is a dependency property.
        /// </summary>
        public PlaceholderMode PlaceholderMode { get => (PlaceholderMode)GetValue(PlaceholderModeProperty); set => SetValue(PlaceholderModeProperty, value); }

        private static readonly DependencyPropertyKey IsPlaceholderActivePropertyKey = DependencyProperty.RegisterReadOnly("IsPlaceholderActive", typeof(bool), typeof(TextBox), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsPlaceholderActive"/> depencency property.
        /// </summary>
        public static readonly DependencyProperty IsPlaceholderActiveProperty = IsPlaceholderActivePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicates if the placeholder is active. This is a dependency property.
        /// </summary>
        public bool IsPlaceholderActive { get => (bool)GetValue(IsPlaceholderActiveProperty); private set => SetValue(IsPlaceholderActivePropertyKey, value); }

        /// <summary>
        /// Identifies the <see cref="RealText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RealTextProperty = DependencyProperty.Register("RealText", typeof(string), typeof(TextBox)

#if DEBUG

            , new PropertyMetadata("", (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {

                Debug.WriteLine("RealTextProperty value changed : " + e.NewValue);

            })

#endif

            );

        /// <summary>
        /// Gets or sets the real text. This is a dependency property.
        /// </summary>
        public string RealText { get => (string)GetValue(RealTextProperty); set => SetValue(RealTextProperty, value); }

        static TextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));

        /// <summary>
        /// Initialize a new instance of the <see cref="TextBox"/> control.
        /// </summary>
        public TextBox()
        {
            // InitializeComponent();
        }

        void SetIsPlaceholderActiveValue(bool value)
        {

            _isAutomaticallyTextChanging = true;
            IsPlaceholderActive = value;
            _isAutomaticallyTextChanging = false;

        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {

            base.OnGotFocus(e);

            if (PlaceholderMode == PlaceholderMode.OnFocus)

                SetIsPlaceholderActiveValue(false);

        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    OnGotFocus(e);
        //}

        /// <summary>
        /// Raises the <see cref="UIElement.LostFocus"/> event (using the provided arguments).
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {

            base.OnLostFocus(e);

            if (PlaceholderMode == PlaceholderMode.OnFocus && (string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text)))

                SetIsPlaceholderActiveValue(true);

        }

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    OnLostFocus(e);
        //}

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        /// <param name="e">The arguments that are associated with the <see cref="System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {

            base.OnTextChanged(e);

            System.Console.WriteLine("IsPlaceholderActive: " + IsPlaceholderActive.ToString());

            if (_isAutomaticallyTextChanging) return;

            if (!IsPlaceholderActive && (string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text)) && PlaceholderMode == PlaceholderMode.OnTextChange)
            {

                // RealText = Text;
#if DEBUG
                Debug.WriteLine(3);
#endif 
                SetIsPlaceholderActiveValue(true);

            }

            else if (IsPlaceholderActive)
            {

                bool textRemoved = false;

                foreach (TextChange textChange in e.Changes)

                    if (textChange.RemovedLength > 0)
                    {

                        textRemoved = true;

                        break;

                    }

                if (textRemoved)
                {

                    System.Media.SystemSounds.Beep.Play();

                    e.Handled = true;

                }

                else
                {

#if DEBUG 
                    Debug.WriteLine(4);
#endif 
                    SetIsPlaceholderActiveValue(false);

                }

            }

            // else RealText = Text;
        }

        //private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    OnTextChanged(e);
        //}

        /// <summary>
        /// Is called when the caret or current selection changes position.
        /// </summary>
        /// <param name="e">The arguments that are associated with the <see cref="System.Windows.Controls.Primitives.TextBoxBase.SelectionChanged"/>event.</param>
        protected override void OnSelectionChanged(RoutedEventArgs e)
        {

            base.OnSelectionChanged(e);

            if (IsPlaceholderActive)

                e.Handled = true;

        }

        //private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    OnSelectionChanged(e);
        //}

    }

}
