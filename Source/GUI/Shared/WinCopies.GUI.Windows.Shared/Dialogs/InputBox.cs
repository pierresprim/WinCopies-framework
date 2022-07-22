/*Copyright © Pierre Sprimont, 2019
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

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WinCopies.GUI.Windows
{
    public class InputBox : DialogWindow
    {
        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(InputBox), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Controls.Orientation"/> of the label and the text box. This is a dependency property.
        /// </summary>
        public Orientation Orientation { get => (Orientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(InputBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the text of the text box. This is a dependency property.
        /// </summary>
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Placeholder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(InputBox));

        /// <summary>
        /// Gets or sets the placeholder for the text box. This is a dependency property.
        /// </summary>
        public string
#if CS8
                ?
#endif
                Placeholder
        { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        static InputBox() =>
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindow), new FrameworkPropertyMetadata(typeof(DialogWindow)));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputBox), new FrameworkPropertyMetadata(typeof(InputBox))); // InputBox.StyleProperty.OverrideMetadata(typeof(InputBox), new FrameworkPropertyMetadata((Style) Application.Current. Resources["abcd"]));

        public static bool? ShowDialog(in string title, in DialogButton? dialogButton, in object content, in string
#if CS8
            ?
#endif
            text, in string
#if CS8
            ?
#endif
            placeholder,
#if CS8
            [MaybeNullWhen(false)][NotNullWhen(true)]
#endif
            out string result, in BitmapSource
#if CS8
            ?
#endif
            icon = null)
        {
            var dialog = new InputBox() { Title = title, DialogButton = dialogButton, Content = content, Text = text, Placeholder = placeholder, Icon = icon };

            bool? _result = dialog.ShowDialog();

            if (_result == true)
            {
                result = dialog.Text;

                return _result;
            }

            result = null;

            return _result;
        }

        public static bool? ShowDialog(in string title, in DialogButton? dialogButton, in object content, in string placeholder, out string result) => ShowDialog(title, dialogButton, content, placeholder, out result);

        ///// <summary>
        ///// Initializes a new instance of the <see cref="InputBox"/> class.
        ///// </summary>
        //public InputBox() => Content = /*new Label { Content = "a" }; new Control { Template = (ControlTemplate)ResourcesHelper.Instance.ResourceDictionary["InputBoxTemplate"] };*/

        // /// <summary>
        // /// Initialize a new instance of the <see cref="InputBox"/> window.
        // /// </summary>
        // public InputBox() =>
        // InitializeComponent();

        // CommandBindings.Add(new CommandBinding(Util.Util.CommonCommand, OnTextChangedInternal));

        ///// <summary>
        ///// Is called when content in this editing control changes.
        ///// </summary>
        ///// <param name="e">The arguments that are associated with the <see cref="TextChanged"/> event.</param>
        ///// <remarks>
        ///// This method raises a <see cref="TextChanged"/> event.
        ///// </remarks>
        //protected virtual void OnTextChanged(TextChangedEventArgs e)
        //{
        //    ThrowIfNull(e, nameof(e));

        //    _ = Command?.CanExecute(CommandParameter, CommandTarget);

        //    e.RoutedEvent = TextChangedEvent;

        //    RaiseEvent(e);
        //}

        //protected override void OnCommandExecuted(ExecutedRoutedEventArgs e)
        //{
        //    if ((e ?? throw GetArgumentNullException(nameof(e))).Parameter is TextChangedEventArgs _e)

        //        OnTextChanged(_e);

        //    else

        //        base.OnCommandExecuted(e);
        //}
    }
}
