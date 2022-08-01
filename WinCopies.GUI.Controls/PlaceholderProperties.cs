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

using System;
using System.ComponentModel;
using System.Windows.Markup;
using WinCopies.Util;

namespace WinCopies.GUI.Controls
{

    // todo: to add the other xml doc tags.

    [MarkupExtensionReturnType(typeof(PlaceholderProperties))]
    public class PlaceholderProperties : MarkupExtension, INotifyPropertyChanged
    {

        /// <summary>
        /// The method that is called to set a value to a property. If succeeds, then call the <see cref="OnPropertyChanged(string, object, object)"/> method. See the Remarks section.
        /// </summary>
        /// <param name="propertyName">The name of the property to set in a new value into</param>
        /// <param name="fieldName">The name of the field to store the new value. This must the field that is called by the property accessors (get and set).</param>
        /// <param name="newValue">The value to set in the property</param>
        /// <param name="declaringType">The declaring type of both the property and its associated field</param>
        /// <remarks>To use this method, you need to work with the WinCopies Framework Property changed notification pattern. See the website of the WinCopies Framework for more details.</remarks>
        protected virtual void OnPropertyChanged(string propertyName, string fieldName, object newValue, Type declaringType)

        {

            var (propertyChanged, oldValue) = ((INotifyPropertyChanged)this).SetProperty(propertyName, fieldName, newValue, declaringType);

            if (propertyChanged) OnPropertyChanged(propertyName, oldValue, newValue);

        }

        /// <summary>
        /// The method that is called to notify that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property which has changed.</param>
        /// <param name="oldValue">The old value of the property</param>
        /// <param name="newValue">The new value of the property</param>
        /// <remarks>To use this method, you need to work with the WinCopies Framework Property changed notification pattern. See the website of the WinCopies Framework for more details.</remarks>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

        private readonly string text = null;

        /// <summary>
        /// Gets or sets the text contents of the text box.
        /// </summary>
        /// <value>
        /// A <see langword="string"/> containing the text contents of the text box. The default is an empty string ("").
        /// </value>
        public string Text
        {
            get => text; set => OnPropertyChanged(nameof(Text), nameof(text), value, typeof(PlaceholderProperties));
        }

        private readonly bool acceptsReturn = false;

        /// <summary>
        /// Gets or sets a value that indicates how the text editing control responds when the user presses the ENTER key.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if pressing the ENTER key inserts a new line at the current cursor position; otherwise, the ENTER key is ignored. The default value is <see langword="false"/> for TextBox and true for RichTextBox.
        /// </value>
        /// <remarks>
        /// The ENTER key corresponds to VK_RETURN virtual-key code.
        /// </remarks>
        public bool AcceptsReturn
        {
            get => acceptsReturn; set => OnPropertyChanged(nameof(AcceptsReturn), nameof(acceptsReturn), value, typeof(PlaceholderProperties));
        }

        private readonly bool acceptsTab = false;

        /// <summary>
        /// Gets or sets a value that indicates how the text editing control responds when the user presses the TAB key.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if pressing the TAB key inserts a tab character at the current cursor position; <see langword="false"/> if pressing the TAB key moves the focus to the next control that is marked as a tab stop and does not insert a tab character.
        /// <br />
        /// The default value is <see langword="false"/>.
        /// </value>
        public bool AcceptsTab
        {
            get => acceptsTab; set => OnPropertyChanged(nameof(AcceptsTab), nameof(acceptsTab), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily();

        /// <summary>
        /// Gets or sets the font family of the control.
        /// </summary>
        public System.Windows.Media.FontFamily FontFamily
        {
            get => fontFamily; set => OnPropertyChanged(nameof(FontFamily), nameof(fontFamily), value, typeof(PlaceholderProperties));
        }

        private readonly double fontSize = 12;

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public double FontSize
        {
            get => fontSize; set => OnPropertyChanged(nameof(FontSize), nameof(fontSize), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.FontStretch fontStretch = new System.Windows.FontStretch();

        /// <summary>
        /// Gets or sets the degree to which a font is condensed or expanded on the screen.
        /// </summary>
        public System.Windows.FontStretch FontStretch
        {
            get => fontStretch; set => OnPropertyChanged(nameof(FontStretch), nameof(fontStretch), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.FontStyle fontStyle = new System.Windows.FontStyle();

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public System.Windows.FontStyle FontStyle
        {
            get => fontStyle; set => OnPropertyChanged(nameof(FontStyle), nameof(fontStyle), value, typeof(PlaceholderProperties));
        }

        private System.Windows.FontWeight fontWeight = new System.Windows.FontWeight();

        /// <summary>
        /// Gets or sets the weight or thickness of the specified font.
        /// </summary>
        public System.Windows.FontWeight FontWeight
        {
            get => fontWeight; set => OnPropertyChanged(nameof(FontWeight), nameof(fontWeight), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.Media.Brush foreground = System.Windows.Media.Brushes.Black;

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        public System.Windows.Media.Brush Foreground
        {
            get => foreground; set => OnPropertyChanged(nameof(Foreground), nameof(foreground), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.TextAlignment textAlignment = System.Windows.TextAlignment.Left;

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents of the text box.
        /// </summary>
        public System.Windows.TextAlignment TextAlignment
        {
            get => textAlignment; set => OnPropertyChanged(nameof(TextAlignment), nameof(textAlignment), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.TextDecorationCollection textDecorations = null;

        /// <summary>
        /// Gets the text decorations to apply to the text box.
        /// </summary>
        public System.Windows.TextDecorationCollection TextDecorations
        {
            get => textDecorations; set => OnPropertyChanged(nameof(TextDecorations), nameof(textDecorations), value, typeof(PlaceholderProperties));
        }

        private readonly System.Windows.TextWrapping textWrapping = System.Windows.TextWrapping.NoWrap;

        /// <summary>
        /// Gets or sets how the text box should wrap text.
        /// </summary>
        public System.Windows.TextWrapping TextWrapping
        {
            get => textWrapping; set => OnPropertyChanged(nameof(TextWrapping), nameof(textWrapping), value, typeof(PlaceholderProperties));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PlaceholderProperties() { }

        public PlaceholderProperties(string text) => Text = text;

        public PlaceholderProperties(string text, bool acceptsReturn, bool acceptsTab, System.Windows.Media.FontFamily fontFamily,
            double fontSize, System.Windows.FontStretch fontStretch, System.Windows.FontStyle fontStyle, System.Windows.FontWeight fontWeight,
            System.Windows.Media.Brush foreground, System.Windows.TextAlignment textAlignment, System.Windows.TextDecorationCollection textDecorations,
            System.Windows.TextWrapping textWrapping)

        {

            this.text = text;

            this.acceptsReturn = acceptsReturn;

            this.acceptsTab = acceptsTab;

            this.fontFamily = fontFamily;

            this.fontSize = fontSize;

            this.fontStretch = fontStretch;

            this.fontStyle = fontStyle;

            this.fontWeight = fontWeight;

            this.foreground = foreground;

            this.textAlignment = textAlignment;

            this.textDecorations = textDecorations;

            this.textWrapping = textWrapping;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
