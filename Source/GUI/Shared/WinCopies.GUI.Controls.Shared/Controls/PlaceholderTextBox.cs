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

using WinCopies.GUI.Controls.Models;

namespace WinCopies.GUI.Controls
{
    //    public class PlaceholderSubTextBox : System.Windows.Controls.TextBox
    //    {
    //        internal PlaceholderTextBox _textBox;

    //        #region TextBoxBase
    //        internal void _OnContextMenuOpening(in ContextMenuEventArgs e) => base.OnContextMenuOpening(e);

    //        protected sealed override void OnContextMenuOpening(ContextMenuEventArgs e) => _textBox._OnContextMenuOpening(e);

    //        internal void _OnLostFocus(in RoutedEventArgs e) => base.OnLostFocus(e);

    //            protected sealed override void OnLostFocus(RoutedEventArgs e) => _textBox._OnLostFocus(e);

    //        internal void _OnLostKeyboardFocus(in KeyboardFocusChangedEventArgs e) => base.OnLostKeyboardFocus(e);

    //        protected sealed override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) => _textBox._OnLostKeyboardFocus(e);

    //        internal void _OnMouseDown(in MouseButtonEventArgs e) => base.OnMouseDown(e);

    //        protected sealed override void OnMouseDown(MouseButtonEventArgs e) => _textBox._OnMouseDown(e);

    //        internal void _OnMouseMove(in MouseEventArgs e) => base.OnMouseMove(e);

    //        protected sealed override void OnMouseMove(MouseEventArgs e) => _textBox._OnMouseMove(e);

    //        internal void _OnMouseUp(in MouseButtonEventArgs e) => base.OnMouseUp(e);

    //        protected sealed override void OnMouseUp(MouseButtonEventArgs e) => _textBox._OnMouseUp(e);

    //        internal void _OnMouseWheel(in MouseWheelEventArgs e) => base.OnMouseWheel(e);

    //        protected sealed override void OnMouseWheel(MouseWheelEventArgs e) => _textBox._OnMouseWheel(e);

    //        internal void _OnPreviewKeyDown(in KeyEventArgs e) => base.OnPreviewKeyDown(e);

    //        protected sealed override void OnPreviewKeyDown(KeyEventArgs e) => _textBox._OnPreviewKeyDown(e);

    //        internal void _OnQueryContinueDrag(in QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);

    //        protected sealed override void OnQueryContinueDrag(QueryContinueDragEventArgs e) => _textBox._OnQueryContinueDrag(e);

    //        internal void _OnQueryCursor(in QueryCursorEventArgs e) => base.OnQueryCursor(e);

    //        protected sealed override void OnQueryCursor(QueryCursorEventArgs e) => _textBox._OnQueryCursor(e);

    //        internal void _OnSelectionChanged(in RoutedEventArgs e) => base.OnSelectionChanged(e);

    //        protected sealed override void OnSelectionChanged(RoutedEventArgs e) => _textBox._OnSelectionChanged(e);

    //        internal void _OnTemplateChanged(in ControlTemplate oldTemplate, in ControlTemplate newTemplate) => base.OnTemplateChanged(oldTemplate, newTemplate);

    //        protected sealed override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) => _textBox._OnTemplateChanged(oldTemplate, newTemplate);

    //        internal void _OnTextChanged(in TextChangedEventArgs e) => base.OnTextChanged(e);

    //        protected sealed override void OnTextChanged(TextChangedEventArgs e) => _textBox._OnTextChanged(e);
    //        #endregion

    //        #region TextBox
    //        internal System.Windows.Size _MeasureOverride(in System.Windows.Size constraint) => base.MeasureOverride(constraint);

    //        protected sealed override System.Windows.Size MeasureOverride(System.Windows.Size constraint) => _textBox._MeasureOverride(constraint);

    //        internal AutomationPeer _OnCreateAutomationPeer() => base.OnCreateAutomationPeer();

    //        protected sealed override AutomationPeer OnCreateAutomationPeer() => _textBox._OnCreateAutomationPeer();

    //        internal void _OnPropertyChanged(in DependencyPropertyChangedEventArgs e) => base.OnPropertyChanged(e);

    //        protected sealed override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) => _textBox._OnPropertyChanged(e);
    //        #endregion
    //    }

    public enum PlaceholderMode
    {
        OnTextChanged = 0,

        OnFocus = 1
    }

    //    [System.Windows.TemplatePart(Name = PART_TextBox, Type = typeof(PlaceholderSubTextBox))]
    public class PlaceholderTextBox : ButtonTextBox /*Control, IAddChild*/
    {
        private static DependencyProperty Register<T>(in string propertyName, in PropertyMetadata propertyMetadata) => Util.Desktop.UtilHelpers.Register<T, PlaceholderTextBox>(propertyName, propertyMetadata);

        public static readonly DependencyProperty PlaceholderStyleProperty = Register<Style>(nameof(PlaceholderStyle), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            if (e.NewValue == null)

                d.SetValue(IsPlaceholderVisiblePropertyKey, false);

            else
            {
                bool value;

                switch ((PlaceholderMode)d.GetValue(PlaceholderModeProperty))
                {
                    case PlaceholderMode.OnTextChanged:

                        value = string.IsNullOrEmpty((string)d.GetValue(TextProperty));

                        break;

                    case PlaceholderMode.OnFocus:

                        value = !(bool)d.GetValue(IsFocusedProperty) && string.IsNullOrEmpty((string)d.GetValue(TextProperty));

                        break;

                    default:

                        value = false;

                        break;
                }

                d.SetValue(IsPlaceholderVisiblePropertyKey, value);
            }
        }));

        public Style PlaceholderStyle { get => (Style)GetValue(PlaceholderStyleProperty); set => SetValue(PlaceholderStyleProperty, value); }

        public static readonly DependencyProperty PlaceholderModeProperty = Register<PlaceholderMode>(nameof(PlaceholderMode), new PropertyMetadata(PlaceholderMode.OnTextChanged, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
#if CS8
            d.SetValue(IsPlaceholderVisiblePropertyKey,
#else
        {
            bool getValue()
            {
                switch (
#endif
                (PlaceholderMode)e.NewValue
#if CS8
            switch
#else
                )
#endif
                {
#if !CS8
                    case
#endif
                    PlaceholderMode.OnFocus
#if CS8
                    => 
#else
                    :
                        return
#endif
                        !(d.GetValue(PlaceholderStyleProperty) == null || (bool)d.GetValue(IsFocusedProperty))
#if CS8
                        ,
#else
                        ;
                    case
#endif
                        PlaceholderMode.OnTextChanged
#if CS8
                    => 
#else
                        :
                        return
#endif
                        d.GetValue(PlaceholderStyleProperty) != null && string.IsNullOrEmpty((string)d.GetValue(TextProperty))
#if CS8
                        ,
                        _ => 
#else
                        ;
                }

                return
#endif
            false
#if !CS8
            ;
#endif
            }
#if CS8
            )
#else
            d.SetValue(IsPlaceholderVisiblePropertyKey, getValue());
        }
#endif
            ));

        public PlaceholderMode PlaceholderMode { get => (PlaceholderMode)GetValue(PlaceholderModeProperty); set => SetValue(PlaceholderModeProperty, value); }

        private static readonly DependencyPropertyKey IsPlaceholderVisiblePropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPlaceholderVisible), typeof(bool), typeof(PlaceholderTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IsPlaceholderVisibleProperty = IsPlaceholderVisiblePropertyKey.DependencyProperty;

        public bool IsPlaceholderVisible { get => (bool)GetValue(IsPlaceholderVisibleProperty); private set => SetValue(IsPlaceholderVisiblePropertyKey, value); }

        static PlaceholderTextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (PlaceholderMode == PlaceholderMode.OnFocus)

                IsPlaceholderVisible = false;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (PlaceholderMode == PlaceholderMode.OnFocus)

                IsPlaceholderVisible = PlaceholderStyle != null && string.IsNullOrEmpty(Text);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (PlaceholderMode == PlaceholderMode.OnTextChanged)

                IsPlaceholderVisible = PlaceholderStyle != null && string.IsNullOrEmpty(Text);
        }

        //        const string PART_TextBox = "PART_TextBox";

        //        private PlaceholderSubTextBox _textBox;

        //        public override void OnApplyTemplate()
        //        {
        //            // No handlers to remove.

        //            base.OnApplyTemplate();

        //            _textBox = GetTemplateChild("PART_TextBox") as PlaceholderSubTextBox;

        //            _textBox._textBox = this;

        //            _textBox.SelectionChanged += SelectionChanged;

        //            _textBox.TextChanged += TextChanged;
        //        }

        //        #region TextBoxBase

        //        /// <summary>
        //        /// Identifies the <see cref="AcceptsReturn"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register(nameof(AcceptsReturn), typeof(bool), typeof(PlaceholderTextBox));

        //        /// <summary>
        //        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(PlaceholderTextBox), new PropertyMetadata(TextBoxBase.VerticalScrollBarVisibilityProperty.DefaultMetadata.DefaultValue));

        //        /// <summary>
        //        /// Identifies the <see cref="UndoLimit"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty UndoLimitProperty = DependencyProperty.Register(nameof(UndoLimit), typeof(int), typeof(PlaceholderTextBox), new PropertyMetadata(TextBoxBase.UndoLimitProperty.DefaultMetadata.DefaultValue));

        //        /// <summary>
        //        /// Identifies the <see cref="SelectionOpacity"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty SelectionOpacityProperty = DependencyProperty.Register(nameof(SelectionOpacity), typeof(double), typeof(PlaceholderTextBox), new PropertyMetadata(TextBoxBase.SelectionOpacityProperty.DefaultMetadata.DefaultValue));

        //        /// <summary>
        //        /// Identifies the <see cref="SelectionBrush"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(PlaceholderTextBox));

        //        /// <summary>
        //        /// Identifies the <see cref="IsUndoEnabled"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty IsUndoEnabledProperty = DependencyProperty.Register(nameof(IsUndoEnabled), typeof(bool), typeof(PlaceholderTextBox), new PropertyMetadata(TextBoxBase.IsUndoEnabledProperty.DefaultMetadata.DefaultValue));

        //        /// <summary>
        //        /// Identifies the <see cref="SelectionTextBrush"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty SelectionTextBrushProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty IsReadOnlyProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="IsReadOnlyCaretVisible"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty IsReadOnlyCaretVisibleProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="IsInactiveSelectionHighlightEnabled"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty IsInactiveSelectionHighlightEnabledProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="CaretBrush"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty CaretBrushProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="AutoWordSelection"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty AutoWordSelectionProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="AcceptsTab"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty AcceptsTabProperty;

        //        /// <summary>
        //        /// Identifies the <see cref="IsSelectionActive"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty IsSelectionActiveProperty;

        //        /// <summary>
        //        /// Gets or sets a value that indicates whether a horizontal scroll bar is shown.
        //        /// </summary>
        //        /// <value>
        //        /// <para>A value that is defined by the <see cref="ScrollBarVisibility"/> enumeration.</para>
        //        /// <para>The default value is <see cref="Visibility.Hidden"/>.</para>
        //        /// </value>
        //        public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }

        //        /// <summary>
        //        /// Gets or sets the horizontal scroll position.
        //        /// </summary>
        //        /// <value>
        //        /// <para>A floating-point value that specifies the horizontal scroll position, in device-independent units (1/96th inch per unit). Setting this property causes the text editing control to scroll to the specified horizontal offset.</para>
        //        /// <para>Reading this property returns the current horizontal offset.</para>
        //        /// <para>The value of this property is 0.0 if the text editing control is not configured to support scrolling.</para>
        //        /// <para>This property has no default value.</para>
        //        /// </value>
        //        /// <exception cref="System.ArgumentException">An attempt is made to set this property to a negative value.</exception>
        //        public double HorizontalOffset => _textBox.HorizontalOffset;
        //        //
        //        // Summary:
        //        //     Gets the horizontal size of the visible content area.
        //        //
        //        // Returns:
        //        //     A floating-point value that specifies the horizontal size of the visible content
        //        //     area, in device-independent units (1/96th inch per unit). The value of this property
        //        //     is 0.0 if the text editing control is not configured to support scrolling. This
        //        //     property has no default value.
        //        public double ExtentWidth => _textBox.ExtentWidth;
        //        //
        //        // Summary:
        //        //     Gets the vertical size of the visible content area.
        //        //
        //        // Returns:
        //        //     A floating-point value that specifies the vertical size of the visible content
        //        //     area, in device-independent units (1/96th inch per unit). The value of this property
        //        //     is 0.0 if the text-editing control is not configured to support scrolling. This
        //        //     property has no default value.
        //        public double ExtentHeight => _textBox.ExtentHeight;

        //        /// <summary>
        //        /// Gets or sets a value that determines whether when a user selects part of a word by dragging across it with the mouse, the rest of the word is selected.
        //        /// </summary>
        //        /// <value><see langword="true"/> if automatic word selection is enabled; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.</value>
        //        public bool AutoWordSelection { get => (bool)GetValue(AutoWordSelectionProperty); set => SetValue(AutoWordSelectionProperty, value); }

        //        /// <summary>
        //        /// Gets a value that indicates whether the most recent action can be undone.
        //        /// </summary>
        //        /// <value>
        //        /// <para><see langword="true"/> if the most recent action can be undone; otherwise, <see langword="false"/>.</para>
        //        /// <para>This property has no default value.</para>
        //        /// </value>
        //        public bool CanUndo => _textBox.CanUndo;

        //        /// <summary>
        //        /// Gets a value that indicates whether the most recent undo action can be redone.
        //        /// </summary>
        //        /// <value>
        //        /// <para><see langword="true"/> if the most recent undo action can be redone; otherwise, <see langword="false"/>.</para>
        //        /// <para>This property has no default value.</para>
        //        /// </value>
        //        public bool CanRedo => _textBox.CanRedo;
        //        //
        //        // Summary:
        //        //     Gets or sets a value that indicates how the text editing control responds when
        //        //     the user presses the TAB key.
        //        //
        //        // Returns:
        //        //     true if pressing the TAB key inserts a tab character at the current cursor position;
        //        //     false if pressing the TAB key moves the focus to the next control that is marked
        //        //     as a tab stop and does not insert a tab character. The default value is false.
        //        public bool AcceptsTab { get; set; }
        //        //
        //        // Summary:
        //        //     Gets or sets a value that indicates whether the text box displays selected text
        //        //     when the text box does not have focus.
        //        //
        //        // Returns:
        //        //     true if the text box displays selected text when the text box does not have focus;
        //        //     otherwise, false. The registered default is false. For more information about
        //        //     what can influence the value, see Dependency Property Value Precedence.
        //        public bool IsInactiveSelectionHighlightEnabled { get; set; }
        //        //
        //        // Summary:
        //        //     Gets or sets the brush that is used to paint the caret of the text box.
        //        //
        //        // Returns:
        //        //     The brush that is used to paint the caret of the text box.
        //        public Brush CaretBrush { get; set; }
        //        //
        //        // Summary:
        //        //     Gets or sets a value that indicates whether the text editing control is read-only
        //        //     to a user interacting with the control.
        //        //
        //        // Returns:
        //        //     true if the contents of the text editing control are read-only to a user; otherwise,
        //        //     the contents of the text editing control can be modified by the user. The default
        //        //     value is false.
        //        public bool IsReadOnly { get; set; }
        //        //
        //        // Summary:
        //        //     Gets or sets the vertical scroll position.
        //        //
        //        // Returns:
        //        //     A floating-point value that specifies the vertical scroll position, in device-independent
        //        //     units (1/96th inch per unit). Setting this property causes the text editing control
        //        //     to scroll to the specified vertical offset. Reading this property returns the
        //        //     current vertical offset. The value of this property is 0.0 if the text editing
        //        //     control is not configured to support scrolling. This property has no default
        //        //     value.
        //        //
        //        // Exceptions:
        //        //   T:System.ArgumentException:
        //        //     An attempt is made to set this property to a negative value.
        //        public double VerticalOffset { get; }

        //        /// <summary>
        //        /// Gets a value that indicates whether the text box has focus and selected text.
        //        /// </summary>
        //        /// <value>
        //        /// <para><see langword="true"/> if the text box has focus and selected text; otherwise, <see langword="false"/>.</para>
        //        /// <para>The registered default is <see langword="false"/>.</para>
        //        /// <para>For more information about what can influence the value, see Dependency Property Value Precedence.</para>
        //        /// </value>
        //        public bool IsSelectionActive => (bool)GetValue(IsSelectionActiveProperty);

        //        /// <summary>
        //        /// Gets or sets a value that indicates whether undo support is enabled for the text-editing control.
        //        /// </summary>
        //        /// <value><see langword="true"/> if undo support is enabled; otherwise, <see langword="false"/>. The default value is <see langword="true"/>.</value>
        //        public bool IsUndoEnabled { get => (bool)GetValue(IsUndoEnabledProperty); set => SetValue(IsUndoEnabledProperty, value); }

        //        /// <summary>
        //        /// Gets or sets the brush that highlights selected text.
        //        /// </summary>
        //        /// <value>The brush that highlights selected text.</value>
        //        public Brush SelectionBrush { get => (Brush)GetValue(SelectionBrushProperty); set => SetValue(SelectionBrushProperty, value); }

        //        /// <summary>
        //        /// Gets or sets the opacity of the <see cref="SelectionBrush"/>.
        //        /// </summary>
        //        /// <value>
        //        /// <para>The opacity of the <see cref="SelectionBrush"/>.</para>
        //        /// <para>The default is 0.4.</para>
        //        /// </value>
        //        public double SelectionOpacity { get => (double)GetValue(SelectionOpacityProperty); set => SetValue(SelectionOpacityProperty, value); }

        //        /// <summary>
        //        /// Gets or sets a value that defines the brush used for selected text.
        //        /// </summary>
        //        /// <value>The brush used for selected text.</value>
        //        public Brush SelectionTextBrush { get => (Brush)GetValue(SelectionTextBrushProperty); set => SetValue(SelectionTextBrushProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets a System.Windows.Controls.SpellCheck object that provides access to spelling
        //        //     errors in the text contents of a System.Windows.Controls.Primitives.TextBoxBase
        //        //     or System.Windows.Controls.RichTextBox.
        //        //
        //        // Returns:
        //        //     A System.Windows.Controls.SpellCheck object that provides access to spelling
        //        //     errors in the text contents of a System.Windows.Controls.Primitives.TextBoxBase
        //        //     or System.Windows.Controls.RichTextBox. This property has no default value.
        //        public SpellCheck SpellCheck => _textBox.SpellCheck;
        //        //
        //        // Summary:
        //        //     Gets or sets the number of actions stored in the undo queue.
        //        //
        //        // Returns:
        //        //     The number of actions stored in the undo queue. The default is -1, which means
        //        //     the undo queue is limited to the memory that is available.
        //        //
        //        // Exceptions:
        //        //   T:System.InvalidOperationException:
        //        //     System.Windows.Controls.Primitives.TextBoxBase.UndoLimit is set after calling
        //        //     System.Windows.Controls.Primitives.TextBoxBase.BeginChange and before calling
        //        //     System.Windows.Controls.Primitives.TextBoxBase.EndChange.
        //        public int UndoLimit { get => (int)GetValue(UndoLimitProperty); set => SetValue(UndoLimitProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets or sets a value that indicates how the text editing control responds when
        //        //     the user presses the ENTER key.
        //        //
        //        // Returns:
        //        //     true if pressing the ENTER key inserts a new line at the current cursor position;
        //        //     otherwise, the ENTER key is ignored. The default value is false for System.Windows.Controls.TextBox
        //        //     and true for System.Windows.Controls.RichTextBox.
        //        public bool AcceptsReturn { get => (bool)GetValue(AcceptsReturnProperty); set => SetValue(AcceptsReturnProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets or sets a value that indicates whether a vertical scroll bar is shown.
        //        //
        //        // Returns:
        //        //     A value that is defined by the System.Windows.Controls.ScrollBarVisibility enumeration.
        //        //     The default value is System.Windows.Visibility.Hidden.
        //        public ScrollBarVisibility VerticalScrollBarVisibility { get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); set => SetValue(VerticalScrollBarVisibilityProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets the vertical size of the scrollable content area.
        //        //
        //        // Returns:
        //        //     A floating-point value that specifies the vertical size of the scrollable content
        //        //     area, in device-independent units (1/96th inch per unit). The value of this property
        //        //     is 0.0 if the text editing control is not configured to support scrolling. This
        //        //     property has no default value.
        //        public double ViewportHeight => _textBox.ViewportHeight;
        //        //
        //        // Summary:
        //        //     Gets the horizontal size of the scrollable content area.
        //        //
        //        // Returns:
        //        //     A floating-point value that specifies the horizontal size of the scrollable content
        //        //     area, in device-independent units (1/96th inch per unit). The value of this property
        //        //     is 0.0 if the text editing control is not configured to support scrolling. This
        //        //     property has no default value.
        //        public double ViewportWidth => _textBox.ViewportWidth;

        //        /// <summary>
        //        /// Gets or sets a value that indicates whether a read-only text box displays a caret.
        //        /// </summary>
        //        /// <value>
        //        /// <para><see langword="true"/> if a read-only text box displays a caret; otherwise, <see langword="false"/>.</para>
        //        /// <para>The default is <see langword="false"/>.</para>
        //        /// </value>
        //        public bool IsReadOnlyCaretVisible { get => (bool)GetValue(IsReadOnlyCaretVisibleProperty); set => SetValue(IsReadOnlyCaretVisibleProperty, value); }

        //        /// <summary>
        //        /// Occurs when the text selection has changed.
        //        /// </summary>
        //        public event RoutedEventHandler SelectionChanged;

        //        /// <summary>
        //        /// Occurs when content changes in the text element.
        //        /// </summary>
        //        public event TextChangedEventHandler TextChanged;

        //        /// <summary>
        //        /// Appends a string to the contents of a text control.
        //        /// </summary>
        //        /// <param name="textData">A string that specifies the text to append to the current contents of the text control.</param>
        //        public void AppendText(in string textData) => _textBox.AppendText(textData);

        //        /// <summary>
        //        /// Begins a change block.
        //        /// </summary>
        //        public void BeginChange() => _textBox.BeginChange();

        //        /// <summary>
        //        /// Copies the current selection of the text editing control to the <see cref="System.Windows.Clipboard"/>.
        //        /// </summary>
        //        public void Copy() => _textBox.Copy();

        //        /// <summary>
        //        /// Removes the current selection from the text editing control and copies it to the <see cref="System.Windows.Clipboard"/>.
        //        /// </summary>
        //        public void Cut() => _textBox.Cut();

        //        /// <summary>
        //        /// Creates a change block.
        //        /// </summary>
        //        /// <returns>An <see cref="System.IDisposable"/> object that refers to a new change block.</returns>
        //        public System.IDisposable DeclareChangeBlock() => _textBox.DeclareChangeBlock();

        //        /// <summary>
        //        /// Ends a change block.
        //        /// </summary>
        //        public void EndChange() => _textBox.EndChange();

        //        /// <summary>
        //        /// Scrolls the contents of the control down by one line.
        //        /// </summary>
        //        public void LineDown() => _textBox.LineDown();

        //        /// <summary>
        //        /// Scrolls the contents of the control to the left by one line.
        //        /// </summary>
        //        public void LineLeft() => _textBox.LineLeft();

        //        /// <summary>
        //        /// Scrolls the contents of the control to the right by one line.
        //        /// </summary>
        //        public void LineRight() => _textBox.LineRight();

        //        /// <summary>
        //        /// Scrolls the contents of the control upward by one line.
        //        /// </summary>
        //        public void LineUp() => _textBox.LineUp();

        //        /// <summary>
        //        /// Locks the most recent undo unit of the undo stack of the application. This prevents the locked unit from being merged with undo units that are added subsequently.
        //        /// </summary>
        //        public void LockCurrentUndoUnit() => _textBox.LockCurrentUndoUnit();

        //        /// <summary>
        //        /// Scrolls the contents of the control down by one page.
        //        /// </summary>
        //        public void PageDown() => _textBox.PageDown();

        //        /// <summary>
        //        /// Scrolls the contents of the control to the left by one page.
        //        /// </summary>
        //        public void PageLeft() => _textBox.PageLeft();

        //        /// <summary>
        //        /// Scrolls the contents of the control to the right by one page.
        //        /// </summary>
        //        public void PageRight() => _textBox.PageRight();

        //        /// <summary>
        //        /// Scrolls the contents of the control up by one page.
        //        /// </summary>
        //        public void PageUp() => _textBox.PageUp();

        //        /// <summary>
        //        /// Pastes the contents of the Clipboard over the current selection in the text editing control.
        //        /// </summary>
        //        public void Paste() => _textBox.Paste();

        //        /// <summary>
        //        /// Undoes the most recent undo command. In other words, redoes the most recent undo unit on the undo stack.
        //        /// </summary>
        //        /// <returns><see langword="true"/> if the redo operation was successful; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if there is no undo command available (the undo stack is empty).</returns>
        //        public bool Redo() => _textBox.Redo();

        //        /// <summary>
        //        /// Scrolls the view of the editing control to the end of the content.
        //        /// </summary>
        //        public void ScrollToEnd() => _textBox.ScrollToEnd();

        //        /// <summary>
        //        /// Scrolls the view of the editing control to the beginning of the viewport.
        //        /// </summary>
        //        public void ScrollToHome() => _textBox.ScrollToHome();

        //        /// <summary>
        //        /// Scrolls the contents of the editing control to the specified horizontal offset.
        //        /// </summary>
        //        /// <param name="offset">A double value that specifies the horizontal offset to scroll to.</param>
        //        public void ScrollToHorizontalOffset(in double offset) => _textBox.ScrollToHorizontalOffset(offset);

        //        /// <summary>
        //        /// Scrolls the contents of the editing control to the specified vertical offset.
        //        /// </summary>
        //        /// <param name="offset">A double value that specifies the vertical offset to scroll to.</param>
        //        public void ScrollToVerticalOffset(in double offset) => _textBox.ScrollToVerticalOffset(offset);

        //        /// <summary>
        //        /// Selects all the contents of the text editing control.
        //        /// </summary>
        //        public void SelectAll() => _textBox.SelectAll();

        //        /// <summary>
        //        /// Undoes the most recent undo command. In other words, undoes the most recent undo unit on the undo stack.
        //        /// </summary>
        //        /// <returns><see langword="true"/> if the undo operation was successful; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if the undo stack is empty.</returns>
        //        public bool Undo() => _textBox.Undo();

        //        /// <summary>
        //        /// Called whenever an unhandled <see cref="System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event.
        //        /// </summary>
        //        /// <param name="e">Arguments of the event.</param>
        //        protected override void OnContextMenuOpening(ContextMenuEventArgs e) => _textBox._OnContextMenuOpening(e);

        //        internal void _OnContextMenuOpening(ContextMenuEventArgs e) => OnContextMenuOpening(e);

        //        /// <summary>
        //        /// Invoked whenever an unhandled <see cref="System.Windows.DragDrop.DragEnter"/> attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        /// </summary>
        //        /// <param name="e">Provides data about the event.</param>
        //        protected override void OnDragEnter(DragEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.DragDrop.DragLeave attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnDragLeave(DragEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.DragDrop.DragOver attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnDragOver(DragEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.DragDrop.DragEnter attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnDrop(DragEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.DragDrop.GiveFeedback attached routed event reaches an element derived from this class in its route. Implement thismethod to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnGiveFeedback(GiveFeedbackEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Keyboard.GotKeyboardFocus attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Keyboard.KeyDown attached
        //        //     routed event reaches an element derived from this class in its route. Implement
        //        //     this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnKeyDown(KeyEventArgs e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Keyboard.KeyUp attached routed
        //        //     event reaches an element derived from this class in its route. Implement this
        //        //     method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnKeyUp(KeyEventArgs e);

        //        /// <summary>
        //        /// Raises the <see cref="System.Windows.UIElement.LostFocus"/> event (using the provided arguments).
        //        /// </summary>
        //        /// <param name="e">Provides data about the event.</param>
        //        protected override void OnLostFocus(RoutedEventArgs e)=>_textBox._OnLostFocus(e);

        //        internal void _OnLostFocus(in RoutedEventArgs e) => OnLostFocus(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Keyboard.LostKeyboardFocus
        //        //     attached routed event reaches an element derived from this class in its route.
        //        //     Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) => _textBox._OnLostKeyboardFocus(e);

        //        internal void _OnLostKeyboardFocus(in KeyboardFocusChangedEventArgs e) => OnLostKeyboardFocus(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Mouse.MouseDown attached routed
        //        //     event reaches an element derived from this class in its route. Implement this
        //        //     method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnMouseDown(MouseButtonEventArgs e) => _textBox._OnMouseDown(e);

        //        internal void _OnMouseDown(in MouseButtonEventArgs e) => OnMouseDown(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Mouse.MouseMove attached routed
        //        //     event reaches an element derived from this class in its route. Implement this
        //        //     method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnMouseMove(MouseEventArgs e) => _textBox._OnMouseMove(e);

        //        internal void _OnMouseMove(in MouseEventArgs e) => OnMouseMove(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.Mouse.MouseUp event reaches
        //        //     an element derived from this class in its route. Implement this method to add
        //        //     class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Arguments of the event. These arguments will include details about which mouse
        //        //     button was depressed, and the handled state.
        //        protected override void OnMouseUp(MouseButtonEventArgs e) => _textBox._OnMouseUp(e);

        //        internal void _OnMouseUp(in MouseButtonEventArgs e) => OnMouseUp(e);
        //        //
        //        // Summary:
        //        //     Is called when a System.Windows.UIElement.MouseWheel event is routed to this
        //        //     class (or to a class that inherits from this class).
        //        //
        //        // Parameters:
        //        //   e:
        //        //     The mouse wheel arguments that are associated with this event.
        //        protected override void OnMouseWheel(MouseWheelEventArgs e) => _textBox._OnMouseWheel(e);

        //        internal void _OnMouseWheel(in MouseWheelEventArgs e) => OnMouseWheel(e);
        //        //
        //        // Summary:
        //        //     Called when the System.Windows.UIElement.KeyDown occurs.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     The event data.
        //        protected override void OnPreviewKeyDown(KeyEventArgs e) => _textBox._OnPreviewKeyDown(e);

        //        internal void _OnPreviewKeyDown(in KeyEventArgs e) => OnPreviewKeyDown(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.DragDrop.QueryContinueDrag attached
        //        //     routed event reaches an element derived from this class in its route. Implement
        //        //     this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) => _textBox._OnQueryContinueDrag(e);

        //        internal void _OnQueryContinueDrag(in QueryContinueDragEventArgs e) => OnQueryContinueDrag(e);

        //        /// <summary>
        //        /// Invoked whenever an unhandled System.Windows.Input.Mouse.QueryCursor attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        //        /// </summary>
        //        /// <param name="e">Provides data about the event.</param>
        //        protected override void OnQueryCursor(QueryCursorEventArgs e) => _textBox._OnQueryCursor(e);

        //        internal void _OnQueryCursor(in QueryCursorEventArgs e) => OnQueryCursor(e);

        //        /// <summary>
        //        /// Is called when the caret or current selection changes position.
        //        /// </summary>
        //        /// <param name="e">The arguments that are associated with the <see cref="SelectionChanged"/> event.</param>
        //        protected virtual void OnSelectionChanged(RoutedEventArgs e) => _textBox._OnSelectionChanged(e);

        //        internal void _OnSelectionChanged(in RoutedEventArgs e) => OnSelectionChanged(e);

        //        /// <summary>
        //        /// Is called when the control template changes.
        //        /// </summary>
        //        /// <param name="oldTemplate">A <see cref="ControlTemplate"/> object that specifies the control template that is currently active.</param>
        //        /// <param name="newTemplate">A <see cref="ControlTemplate"/> object that specifies a new control template to use.</param>
        //        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) => _textBox._OnTemplateChanged(oldTemplate, newTemplate);

        //        internal void _OnTemplateChanged(in ControlTemplate oldTemplate, in ControlTemplate newTemplate) => OnTemplateChanged(oldTemplate, newTemplate);

        //        /// <summary>
        //        /// Is called when content in this editing control changes.
        //        /// </summary>
        //        /// <param name="e">The arguments that are associated with the <see cref="TextChanged"/> event.</param>
        //        protected virtual void OnTextChanged(TextChangedEventArgs e) => _textBox._OnTextChanged(e);

        //        internal void _OnTextChanged(in TextChangedEventArgs e) => OnTextChanged(e);
        //        //
        //        // Summary:
        //        //     Invoked whenever an unhandled System.Windows.Input.TextCompositionManager.TextInput
        //        //     attached routed event reaches an element derived from this class in its route.
        //        //     Implement this method to add class handling for this event.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Provides data about the event.
        //        protected override void OnTextInput(TextCompositionEventArgs e);
        //        #endregion

        //        /// <summary>
        //        /// Initializes a new instance of the <see cref="PlaceholderTextBox"/> class.
        //        /// </summary>
        //        public PlaceholderTextBox() { /* Left empty. */ }

        //        #region TextBox
        //        #region Dependency Properties
        //        /// <summary>
        //        /// Identifies the <see cref="CharacterCasing"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.Register(nameof(CharacterCasing), typeof(CharacterCasing), typeof(PlaceholderTextBox), new PropertyMetadata(CharacterCasing.Normal));

        //        /// <summary>
        //        /// Identifies the <see cref="MaxLength"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(PlaceholderTextBox));

        //        /// <summary>
        //        /// Identifies the <see cref="MaxLines"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty MaxLinesProperty = DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(PlaceholderTextBox), new PropertyMetadata(int.MaxValue));

        //        /// <summary>
        //        /// Identifies the <see cref="MinLines"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty MinLinesProperty = DependencyProperty.Register(nameof(MinLines), typeof(int), typeof(PlaceholderTextBox), new PropertyMetadata(1));

        //        /// <summary>
        //        /// Identifies the <see cref="TextAlignment"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(PlaceholderTextBox));

        //        /// <summary>
        //        /// Identifies the <see cref="TextDecorations"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorationCollection), typeof(PlaceholderTextBox));

        //        /// <summary>
        //        /// Identifies the <see cref="Text"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(System.Windows.Controls.TextBox.TextProperty.DefaultMetadata.DefaultValue));

        //        /// <summary>
        //        /// Identifies the <see cref="TextWrapping"/> dependency property.
        //        /// </summary>
        //        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(PlaceholderTextBox), new PropertyMetadata(TextWrapping.NoWrap));
        //        #endregion

        //        #region Properties
        //        /// <summary>
        //        /// Gets or sets how characters are cased when they are manually entered into the text box.
        //        /// </summary>
        //        /// <value>One of the <see cref="System.Windows.Controls.CharacterCasing"/> values that specifies how manually entered characters are cased. The default is <see cref="System.Windows.Controls.CharacterCasing.Normal"/>.</value>
        //        public CharacterCasing CharacterCasing { get => (CharacterCasing)GetValue(CharacterCasingProperty); set => SetValue(CharacterCasingProperty, value); }

        //        //
        //        // Summary:
        //        //     Gets the total number of lines in the text box.
        //        //
        //        // Returns:
        //        //     The total number of lines in the text box, or -1 if layout information is not
        //        //     available.
        //        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public int LineCount => _textBox.LineCount;

        //        /// <summary>
        //        /// Gets or sets the maximum number of characters that can be manually entered into the text box.
        //        /// </summary>
        //        /// <value>
        //        /// <para>The maximum number of characters that can be manually entered into the text box.</para>
        //        /// <para>The default is 0, which indicates no limit.</para>
        //        /// </value>
        //        [DefaultValue(0)]
        //        [Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable)]
        //        public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }

        //        /// <summary>
        //        /// Gets or sets the maximum number of visible lines.
        //        /// </summary>
        //        /// <value>
        //        /// <para>The maximum number of visible lines.</para>
        //        /// <para>The default is <see cref="int.MaxValue"/>.</para>
        //        /// </value>
        //        /// <exception cref="System.Exception"><see cref="MaxLines"/> is less than <see cref="MinLines"/>.</exception>
        //        [DefaultValue(int.MaxValue)]
        //        public int MaxLines { get => (int)GetValue(MaxLinesProperty); set => SetValue(MaxLinesProperty, value); }

        //        /// <summary>
        //        /// Gets or sets the minimum number of visible lines.
        //        /// </summary>
        //        /// <value>
        //        /// <para>The minimum number of visible lines.</para>
        //        /// <para>The default is 1.</para>
        //        /// </value>
        //        /// <exception cref="Exception"><see cref="MinLines"/> is greater than <see cref="MaxLines"/>.</exception>
        //        [DefaultValue(1)]
        //        public int MinLines { get => (int)GetValue(MinLinesProperty); set => SetValue(MinLinesProperty, value); }

        //        /// <summary>
        //        /// Gets or sets the content of the current selection in the text box.
        //        /// </summary>
        //        /// <value>The currently selected text in the text box.</value>
        //        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public string SelectedText { get => _textBox.SelectedText; set => _textBox.SelectedText = value; }
        //        //
        //        // Summary:
        //        //     Gets or sets the horizontal alignment of the contents of the text box.
        //        //
        //        // Returns:
        //        //     One of the System.Windows.TextAlignment values that specifies the horizontal
        //        //     alignment of the contents of the text box. The default is System.Windows.TextAlignment.Left.
        //        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets or sets a character index for the beginning of the current selection.
        //        //
        //        // Returns:
        //        //     The character index for the beginning of the current selection.
        //        //
        //        // Exceptions:
        //        //   T:System.ArgumentOutOfRangeException:
        //        //     System.Windows.Controls.TextBox.SelectionStart is set to a negative value.
        //        [DefaultValue(0)]
        //        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public int SelectionStart { get => _textBox.SelectionStart; set => _textBox.SelectionStart = value; }
        //        //
        //        // Summary:
        //        //     Gets or sets the text contents of the text box.
        //        //
        //        // Returns:
        //        //     A string containing the text contents of the text box. The default is an empty
        //        //     string ("").
        //        [DefaultValue("")]
        //        [Localizability(LocalizationCategory.Text)]
        //        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        //        /// <summary>
        //        /// Gets the text decorations to apply to the text box.
        //        /// </summary>
        //        /// <value>A <see cref="System.Windows.TextDecorationCollection"/> collection that contains text decorations to apply to the text box. The default is <see langword="null"/> (no text decorations applied).</value>
        //        public TextDecorationCollection TextDecorations { get => (TextDecorationCollection)GetValue(TextDecorationsProperty); set => SetValue(TextDecorationsProperty, value); }

        //        /// <summary>
        //        /// Gets or sets a value indicating the number of characters in the current selection in the text box.
        //        /// </summary>
        //        /// <value>The number of characters in the current selection in the text box. The default is 0.</value>
        //        /// <exception cref="System.ArgumentOutOfRangeException"><see cref="SelectionLength"/> is set to a negative value.</exception>
        //        [DefaultValue(0)]
        //        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public int SelectionLength { get => _textBox.SelectionLength; set => _textBox.SelectionLength = value; }
        //        //
        //        // Summary:
        //        //     Gets or sets the insertion position index of the caret.
        //        //
        //        // Returns:
        //        //     The zero-based insertion position index of the caret.
        //        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public int CaretIndex { get => _textBox.CaretIndex; set => _textBox.CaretIndex = value; }
        //        //
        //        // Summary:
        //        //     Gets the currently effective typography variations for the text contents of the
        //        //     text box.
        //        //
        //        // Returns:
        //        //     A System.Windows.Documents.Typography object that specifies the currently effective
        //        //     typography variations. For a list of default typography values, see System.Windows.Documents.Typography.
        //        public Typography Typography => _textBox.Typography;
        //        //
        //        // Summary:
        //        //     Gets or sets how the text box should wrap text.
        //        //
        //        // Returns:
        //        //     One of the System.Windows.TextWrapping values that indicates how the text box
        //        //     should wrap text. The default is System.Windows.TextWrapping.NoWrap.
        //        public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }
        //        //
        //        // Summary:
        //        //     Gets an enumerator for the logical child elements of the System.Windows.Controls.TextBox.
        //        //
        //        // Returns:
        //        //     An enumerator for the logical child elements of the System.Windows.Controls.TextBox.
        //        protected internal override IEnumerator LogicalChildren { get; }
        //        #endregion

        //        #region Methods
        //        #region Public Methods
        //        /// <summary>
        //        /// Clears all the content from the text box.
        //        /// </summary>
        //        public void Clear() => _textBox.Clear();
        //        //
        //        // Summary:
        //        //     Returns the zero-based character index for the first character in the specified
        //        //     line.
        //        //
        //        // Parameters:
        //        //   lineIndex:
        //        //     The zero-based index of the line to retrieve the initial character index for.
        //        //
        //        // Returns:
        //        //     The zero-based index for the first character in the specified line.
        //        public int GetCharacterIndexFromLineIndex(in int lineIndex) => _textBox.GetCharacterIndexFromLineIndex(lineIndex);
        //        //
        //        // Summary:
        //        //     Returns the zero-based index of the character that is closest to the specified
        //        //     point.
        //        //
        //        // Parameters:
        //        //   point:
        //        //     A point in System.Windows.Controls.TextBox coordinate-space for which to return
        //        //     an index.
        //        //
        //        //   snapToText:
        //        //     true to return the nearest index if there is no character at the specified point;
        //        //     false to return -1 if there is no character at the specified point.
        //        //
        //        // Returns:
        //        //     The index of the character that is closest to the specified point, or -1 if no
        //        //     valid index can be found.
        //        public int GetCharacterIndexFromPoint(in System.Windows.Point point, in bool snapToText) => _textBox.GetCharacterIndexFromPoint(point, snapToText);
        //        //
        //        // Summary:
        //        //     Returns the line index for the first line that is currently visible in the text
        //        //     box.
        //        //
        //        // Returns:
        //        //     The zero-based index for the first visible line in the text box.
        //        public int GetFirstVisibleLineIndex() => _textBox.GetFirstVisibleLineIndex();
        //        //
        //        // Summary:
        //        //     Returns the line index for the last line that is currently visible in the text
        //        //     box.
        //        //
        //        // Returns:
        //        //     The zero-based index for the last visible line in the text box.
        //        public int GetLastVisibleLineIndex() => _textBox.GetLastVisibleLineIndex();
        //        //
        //        // Summary:
        //        //     Returns the zero-based line index for the line that contains the specified character
        //        //     index.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index for which to retrieve the associated line index.
        //        //
        //        // Returns:
        //        //     The zero-based index for the line that contains the specified character index.
        //        public int GetLineIndexFromCharacterIndex(in int charIndex) => _textBox.GetLineIndexFromCharacterIndex(charIndex);
        //        //
        //        // Summary:
        //        //     Returns the number of characters in the specified line.
        //        //
        //        // Parameters:
        //        //   lineIndex:
        //        //     The zero-based line index for which to return a character count.
        //        //
        //        // Returns:
        //        //     The number of characters in the specified line.
        //        public int GetLineLength(in int lineIndex) => _textBox.GetLineLength(lineIndex);
        //        //
        //        // Summary:
        //        //     Returns the text that is currently displayed on the specified line.
        //        //
        //        // Parameters:
        //        //   lineIndex:
        //        //     The zero-based line index for which to retrieve the currently displayed text.
        //        //
        //        // Returns:
        //        //     A string containing a copy of the text currently visible on the specified line.
        //        public string GetLineText(in int lineIndex) => _textBox.GetLineText(lineIndex);
        //        //
        //        // Summary:
        //        //     Returns the beginning character index for the next spelling error in the contents
        //        //     of the text box.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index indicating a position from which to search for
        //        //     the next spelling error.
        //        //
        //        //   direction:
        //        //     One of the System.Windows.Documents.LogicalDirection values that specifies the
        //        //     direction in which to search for the next spelling error, starting at the specified
        //        //     charIndex.
        //        //
        //        // Returns:
        //        //     The character index for the beginning of the next spelling error in the contents
        //        //     of the text box, or -1 if no next spelling error exists.
        //        public int GetNextSpellingErrorCharacterIndex(in int charIndex, in LogicalDirection direction) => _textBox.GetNextSpellingErrorCharacterIndex(charIndex, direction);
        //        //
        //        // Summary:
        //        //     Returns the rectangle for the leading or trailing edge of the character at the
        //        //     specified index.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index of the character for which to retrieve the rectangle.
        //        //
        //        //   trailingEdge:
        //        //     true to get the trailing edge of the character; false to get the leading edge
        //        //     of the character.
        //        //
        //        // Returns:
        //        //     A rectangle for an edge of the character at the specified character index, or
        //        //     System.Windows.Rect.Empty if a bounding rectangle cannot be determined.
        //        //
        //        // Exceptions:
        //        //   T:System.ArgumentOutOfRangeException:
        //        //     charIndex is negative or is greater than the length of the content.
        //        public Rect GetRectFromCharacterIndex(in int charIndex, in bool trailingEdge) => _textBox.GetRectFromCharacterIndex(charIndex, trailingEdge);
        //        //
        //        // Summary:
        //        //     Returns the rectangle for the leading edge of the character at the specified
        //        //     index.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index of the character for which to retrieve the rectangle.
        //        //
        //        // Returns:
        //        //     A rectangle for the leading edge of the character at the specified character
        //        //     index, or System.Windows.Rect.Empty if a bounding rectangle cannot be determined.
        //        public Rect GetRectFromCharacterIndex(in int charIndex) => _textBox.GetRectFromCharacterIndex(charIndex);
        //        //
        //        // Summary:
        //        //     Returns a System.Windows.Controls.SpellingError object associated with any spelling
        //        //     error at the specified character index.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index of a position in content to examine for a spelling
        //        //     error.
        //        //
        //        // Returns:
        //        //     A System.Windows.Controls.SpellingError object containing the details of the
        //        //     spelling error found at the character indicated by charIndex, or null if no spelling
        //        //     error exists at the specified character.
        //        public SpellingError GetSpellingError(in int charIndex) => _textBox.GetSpellingError(charIndex);
        //        //
        //        // Summary:
        //        //     Returns the length of any spelling error that includes the specified character.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index of a position in content to examine for a spelling
        //        //     error.
        //        //
        //        // Returns:
        //        //     The length of any spelling error that includes the character specified by charIndex,
        //        //     or 0 if the specified character is not part of a spelling error.
        //        public int GetSpellingErrorLength(in int charIndex) => _textBox.GetSpellingErrorLength(charIndex);
        //        //
        //        // Summary:
        //        //     Returns the beginning character index for any spelling error that includes the
        //        //     specified character.
        //        //
        //        // Parameters:
        //        //   charIndex:
        //        //     The zero-based character index of a position in content to examine for a spelling
        //        //     error.
        //        //
        //        // Returns:
        //        //     The beginning character index for any spelling error that includes the character
        //        //     specified by charIndex, or -1 if the specified character is not part of a spelling
        //        //     error.
        //        public int GetSpellingErrorStart(in int charIndex) => _textBox.GetSpellingErrorStart(charIndex);
        //        //
        //        // Summary:
        //        //     Scrolls the line at the specified line index into view.
        //        //
        //        // Parameters:
        //        //   lineIndex:
        //        //     The zero-based line index of the line to scroll into view.
        //        public void ScrollToLine(in int lineIndex) => _textBox.ScrollToLine(lineIndex);
        //        //
        //        // Summary:
        //        //     Selects a range of text in the text box.
        //        //
        //        // Parameters:
        //        //   start:
        //        //     The zero-based character index of the first character in the selection.
        //        //
        //        //   length:
        //        //     The length of the selection, in characters.
        //        //
        //        // Exceptions:
        //        //   T:System.ArgumentOutOfRangeException:
        //        //     start or length is negative.
        //        public void Select(in int start, in int length) => _textBox.Select(start, length);
        //        //
        //        // Summary:
        //        //     Returns a value that indicates whether the effective value of the System.Windows.Controls.TextBox.Text
        //        //     property should be serialized during serialization of the System.Windows.Controls.TextBox
        //        //     object.
        //        //
        //        // Parameters:
        //        //   manager:
        //        //     A serialization service manager object for this object.
        //        //
        //        // Returns:
        //        //     true if the System.Windows.Controls.TextBox.Text property should be serialized;
        //        //     otherwise, false.
        //        //
        //        // Exceptions:
        //        //   T:System.NullReferenceException:
        //        //     manager is null.
        //        [EditorBrowsable(EditorBrowsableState.Never)]
        //        public bool ShouldSerializeText(in XamlDesignerSerializationManager manager) => _textBox.ShouldSerializeText(manager);
        //        #endregion

        //        #region Protected Methods
        //        /// <summary>
        //        /// Sizes the text box to its content.
        //        /// </summary>
        //        /// <param name="constraint">A <see cref="System.Windows.Size"/> structure that specifies the constraints on the size of the text box.</param>
        //        /// <returns>A <see cref="System.Windows.Size"/> structure indicating the new size of the text box.</returns>
        //        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint) => _textBox._MeasureOverride(constraint);

        //        internal System.Windows.Size _MeasureOverride(in System.Windows.Size constraint) => MeasureOverride(constraint);
        //        //
        //        // Summary:
        //        //     Creates and returns an System.Windows.Automation.Peers.AutomationPeer object
        //        //     for the text box.
        //        //
        //        // Returns:
        //        //     An System.Windows.Automation.Peers.AutomationPeer object for the text box.
        //        protected override AutomationPeer OnCreateAutomationPeer() => _textBox._OnCreateAutomationPeer();

        //        internal AutomationPeer _OnCreateAutomationPeer() => OnCreateAutomationPeer();
        //        //
        //        // Summary:
        //        //     Called when one or more of the dependency properties that exist on the element
        //        //     have had their effective values changed.
        //        //
        //        // Parameters:
        //        //   e:
        //        //     Arguments for the associated event.
        //        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) => _textBox._OnPropertyChanged(e);

        //        internal void _OnPropertyChanged(DependencyPropertyChangedEventArgs e) => OnPropertyChanged(e);
        //        #endregion
    }

    #region (View)Models
    public interface IPlaceholderTextBoxModel : IButtonTextBoxModel
    {
        string Placeholder { get; set; }

        PlaceholderMode PlaceholderMode { get; set; }
    }

    public class PlaceholderTextBoxModelTextOriented : ButtonTextBoxModelTextOriented, IPlaceholderTextBoxModel
    {
        public string Placeholder { get; set; }

        public PlaceholderMode PlaceholderMode { get; set; }
    }

    public class PlaceholderTextBoxViewModelTextOriented<T> : ButtonTextBoxViewModelTextOriented<T>, IPlaceholderTextBoxModel where T : IPlaceholderTextBoxModel, ITextBoxModelTextOriented
    {
        public string Placeholder { get => ModelGeneric.Placeholder; set { ModelGeneric.Placeholder = value; OnPropertyChanged(nameof(Placeholder)); } }

        public PlaceholderMode PlaceholderMode { get => ModelGeneric.PlaceholderMode; set { ModelGeneric.PlaceholderMode = value; OnPropertyChanged(nameof(PlaceholderMode)); } }

        public PlaceholderTextBoxViewModelTextOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class PlaceholderTextBoxModelSelectionOriented : ButtonTextBoxModelSelectionOriented, IPlaceholderTextBoxModel
    {
        public string Placeholder { get; set; }

        public PlaceholderMode PlaceholderMode { get; set; }
    }

    public class PlaceholderTextBoxViewModelSelectionOriented<T> : ButtonTextBoxViewModelSelectionOriented<T>, IPlaceholderTextBoxModel where T : IPlaceholderTextBoxModel, ITextBoxModelSelectionOriented
    {
        public string Placeholder { get => ModelGeneric.Placeholder; set { ModelGeneric.Placeholder = value; OnPropertyChanged(nameof(Placeholder)); } }

        public PlaceholderMode PlaceholderMode { get => ModelGeneric.PlaceholderMode; set { ModelGeneric.PlaceholderMode = value; OnPropertyChanged(nameof(PlaceholderMode)); } }

        public PlaceholderTextBoxViewModelSelectionOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class PlaceholderTextBoxModelTextEditingOriented : ButtonTextBoxModelTextEditingOriented, IPlaceholderTextBoxModel
    {
        public string Placeholder { get; set; }

        public PlaceholderMode PlaceholderMode { get; set; }
    }

    public class PlaceholderTextBoxViewModelTextEditingOriented<T> : ButtonTextBoxViewModelTextEditingOriented<T>, IPlaceholderTextBoxModel where T : IPlaceholderTextBoxModel, ITextBoxModelTextEditingOriented
    {
        public string Placeholder { get => ModelGeneric.Placeholder; set { ModelGeneric.Placeholder = value; OnPropertyChanged(nameof(Placeholder)); } }

        public PlaceholderMode PlaceholderMode { get => ModelGeneric.PlaceholderMode; set { ModelGeneric.PlaceholderMode = value; OnPropertyChanged(nameof(PlaceholderMode)); } }

        public PlaceholderTextBoxViewModelTextEditingOriented(T model) : base(model) { /* Left empty. */ }
    }

    public class PlaceholderTextBoxModel : ButtonTextBoxModel, IPlaceholderTextBoxModel
    {
        public string Placeholder { get; set; }

        public PlaceholderMode PlaceholderMode { get; set; }
    }

    public class PlaceholderTextBoxViewModel<T> : ButtonTextBoxViewModel<T>, IPlaceholderTextBoxModel where T : ITextBoxModel, IPlaceholderTextBoxModel, Models.ITextBoxModel
    {
        public string Placeholder { get => ModelGeneric.Placeholder; set { ModelGeneric.Placeholder = value; OnPropertyChanged(nameof(Placeholder)); } }

        public PlaceholderMode PlaceholderMode { get => ModelGeneric.PlaceholderMode; set { ModelGeneric.PlaceholderMode = value; OnPropertyChanged(nameof(PlaceholderMode)); } }

        public PlaceholderTextBoxViewModel(T model) : base(model) { /* Left empty. */ }
    }

    public interface IPlaceholderTextBoxModel2 : IPlaceholderTextBoxModel, IButtonTextBoxModel2
    {
        // Left empty.
    }
    #endregion
}
