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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using WinCopies.Desktop;
using WinCopies.Util.Data;

using static WinCopies.Commands.TextCommands;
using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public class UpDownButton : Control
    {
        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.Register(nameof(UpCommand), typeof(ICommand), typeof(UpDownButton));

        public ICommand UpCommand { get => (ICommand)GetValue(UpCommandProperty); set => SetValue(UpCommandProperty, value); }

        public static readonly DependencyProperty UpCommandParameterProperty = DependencyProperty.Register(nameof(UpCommandParameter), typeof(object), typeof(UpDownButton));

        public object UpCommandParameter { get => GetValue(UpCommandParameterProperty); set => SetValue(UpCommandParameterProperty, value); }

        public static readonly DependencyProperty UpCommandTargetProperty = DependencyProperty.Register(nameof(UpCommandTarget), typeof(IInputElement), typeof(UpDownButton));

        public IInputElement UpCommandTarget { get => (IInputElement)GetValue(UpCommandTargetProperty); set => SetValue(UpCommandTargetProperty, value); }



        public static readonly DependencyProperty DownCommandProperty = DependencyProperty.Register(nameof(DownCommand), typeof(ICommand), typeof(UpDownButton));

        public ICommand DownCommand { get => (ICommand)GetValue(DownCommandProperty); set => SetValue(DownCommandProperty, value); }

        public static readonly DependencyProperty DownCommandParameterProperty = DependencyProperty.Register(nameof(DownCommandParameter), typeof(object), typeof(UpDownButton));

        public object DownCommandParameter { get => GetValue(DownCommandParameterProperty); set => SetValue(DownCommandParameterProperty, value); }

        public static readonly DependencyProperty DownCommandTargetProperty = DependencyProperty.Register(nameof(DownCommandTarget), typeof(IInputElement), typeof(UpDownButton));

        public IInputElement DownCommandTarget { get => (IInputElement)GetValue(DownCommandTargetProperty); set => SetValue(DownCommandTargetProperty, value); }



        static UpDownButton() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<UpDownButton>();
    }

    public class NumericUpDown : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, NumericUpDown>(propertyName);
        private static DependencyProperty Register<T>(in string propertyName, in T defaultValue) => Register<T, NumericUpDown>(propertyName, defaultValue);
        private static DependencyProperty Register<T>(in string propertyName, in decimal defaultValue, FuncIn<NumericUpDown, decimal, bool> func, ActionIn<NumericUpDown, decimal, decimal, FuncIn<NumericUpDown, decimal, bool>> action) => Register<T, NumericUpDown>(propertyName, new PropertyMetadata(defaultValue, (DependencyObject d, DependencyPropertyChangedEventArgs e) => action((NumericUpDown)d, (decimal)e.OldValue, (decimal)e.NewValue, func)));
        private static DependencyProperty Register<T>(in string propertyName, in decimal defaultValue, in FuncIn<NumericUpDown, decimal, bool> func) => Register<T>(propertyName, defaultValue, func, DependencyProperty_ValueChanged);

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = Register<bool>(nameof(IsReadOnly));

        public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsReadOnlyCaretVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyCaretVisibleProperty = Register<bool>(nameof(IsReadOnlyCaretVisible));

        public bool IsReadOnlyCaretVisible { get => (bool)GetValue(IsReadOnlyCaretVisibleProperty); set => SetValue(IsReadOnlyCaretVisibleProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = Register<ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility));

        public ScrollBarVisibility HorizontalScrollBarVisibility { get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); set => SetValue(HorizontalScrollBarVisibilityProperty, value); }

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = Register<ScrollBarVisibility>(nameof(VerticalScrollBarVisibility));

        public ScrollBarVisibility VerticalScrollBarVisibility { get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); set => SetValue(VerticalScrollBarVisibilityProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsUndoEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUndoEnabledProperty = Register<bool>(nameof(IsUndoEnabled), true);

        public bool IsUndoEnabled { get => (bool)GetValue(IsUndoEnabledProperty); set => SetValue(IsUndoEnabledProperty, value); }

        /// <summary>
        /// Identifies the <see cref="UndoLimit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UndoLimitProperty = Register<int>(nameof(UndoLimit));

        public int UndoLimit { get => (int)GetValue(UndoLimitProperty); set => SetValue(UndoLimitProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectionBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty = Register(nameof(SelectionBrush), SystemColors.HighlightBrush);

        public Brush SelectionBrush { get => (Brush)GetValue(SelectionBrushProperty); set => SetValue(SelectionBrushProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectionTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionTextBrushProperty = Register<Brush>(nameof(SelectionTextBrush));

        public Brush SelectionTextBrush { get => (Brush)GetValue(SelectionTextBrushProperty); set => SetValue(SelectionTextBrushProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SelectionOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionOpacityProperty = Register(nameof(SelectionOpacity), 0.4);

        public double SelectionOpacity { get => (double)GetValue(SelectionOpacityProperty); set => SetValue(SelectionOpacityProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CaretBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty = Register<Brush>(nameof(CaretBrush));

        public Brush CaretBrush { get => (Brush)GetValue(CaretBrushProperty); set => SetValue(CaretBrushProperty, value); }

        /// <summary>
        /// Identifies the <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = Register<decimal>(nameof(MinValue), decimal.MinValue, (in NumericUpDown obj, in decimal value) => value > obj.Value);

        public decimal MinValue { get => (decimal)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = Register<decimal>(nameof(Value), 0m, (in NumericUpDown obj, in decimal value) => value > obj.MaxValue, (in NumericUpDown d, in decimal oldValue, in decimal newValue, in FuncIn<NumericUpDown, decimal, bool> func) =>
        {
            DependencyProperty_ValueChanged(d, oldValue, newValue, func);

            d.RaiseEvent(new ValueChangedRoutedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent));
        });

        public decimal Value { get => (decimal)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        /// <summary>
        /// Identifies the <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = Register<decimal>(nameof(MaxValue), decimal.MaxValue, (in NumericUpDown obj, in decimal value) => value < obj.Value);

        public decimal MaxValue { get => (decimal)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepProperty = Register<decimal, NumericUpDown>(nameof(Step), new PropertyMetadata(1m, (DependencyObject d, DependencyPropertyChangedEventArgs e) => { if ((decimal)e.NewValue < 0m) throw new ArgumentOutOfRangeException(); }));

        public decimal Step { get => (decimal)GetValue(StepProperty); set => SetValue(StepProperty, value); }

        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = Register<TextAlignment>(nameof(TextAlignment));

        public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }

        /// <summary>
        /// Identifies the <see cref="TextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty = Register<TextDecorationCollection>(nameof(TextDecorations));

        public TextDecorationCollection TextDecorations { get => (TextDecorationCollection)GetValue(TextDecorationsProperty); set => SetValue(TextDecorationsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ValueChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = Register<ValueChangedRoutedEventHandler<decimal>, NumericUpDown>(nameof(ValueChanged), RoutingStrategy.Bubble);

        public event ValueChangedRoutedEventHandler<decimal> ValueChanged { add => AddHandler(ValueChangedEvent, value); remove => RemoveHandler(ValueChangedEvent, value); }



        static NumericUpDown() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<NumericUpDown>();

        public NumericUpDown() => AddCommandBindings();



        private void UpdateValue(in decimal valueToTest, in Func<decimal> func, in FuncIn<decimal, decimal, bool> predicate)
        {
            decimal value;

            try
            {
                if (predicate(value = func(), valueToTest))

                    value = valueToTest;
            }

            catch (OverflowException)
            {
                value = valueToTest;
            }

            Value = value;
        }

        private static void DependencyProperty_ValueChanged(in NumericUpDown d, in decimal oldValue, in decimal newValue, in FuncIn<NumericUpDown, decimal, bool> func)
        {
            if (func(d, newValue))

                throw new InvalidOperationException("The new value is out of range.");
        }

        protected virtual void Increment() => UpdateValue(MaxValue, () => Value + Step, (in decimal newValue, in decimal maxValue) => newValue > maxValue);

        protected virtual void Decrement() => UpdateValue(MinValue, () => Value - Step, (in decimal newValue, in decimal minValue) => newValue < minValue);

        protected virtual void AddCommandBindings()
        {
            void add(in ICommand command, Action action, Func<bool> func) => CommandBindings.Add(command, (object sender, ExecutedRoutedEventArgs e) => { action(); e.Handled = true; }, (object sender, CanExecuteRoutedEventArgs e) => { e.CanExecute = func(); e.Handled = true; });

            add(Upper, Increment, () => Value < MaxValue);
            add(Lower, Decrement, () => Value > MinValue);
        }
    }

    public interface IUpDownButtonModel : Models.IControlModel
    {
        ICommand UpCommand { get; set; }

        object UpCommandParameter { get; set; }

        IInputElement UpCommandTarget { get; set; }



        ICommand DownCommand { get; set; }

        object DownCommandParameter { get; set; }

        IInputElement DownCommandTarget { get; set; }
    }

    public class UpDownButtonModel : Models.ControlModel, IUpDownButtonModel
    {
        public ICommand UpCommand { get; set; }

        public object UpCommandParameter { get; set; }

        public IInputElement UpCommandTarget { get; set; }



        public ICommand DownCommand { get; set; }

        public object DownCommandParameter { get; set; }

        public IInputElement DownCommandTarget { get; set; }
    }

    public class UpDownButtonViewModel<T> : ViewModels.ControlViewModel<T>, IUpDownButtonModel where T : IUpDownButtonModel
    {
        public ICommand UpCommand { get => ModelGeneric.UpCommand; set { ModelGeneric.UpCommand = value; OnPropertyChanged(nameof(UpCommand)); } }

        public object UpCommandParameter { get => ModelGeneric.UpCommandParameter; set { ModelGeneric.UpCommandParameter = value; OnPropertyChanged(nameof(UpCommandParameter)); } }

        public IInputElement UpCommandTarget { get => ModelGeneric.UpCommandTarget; set { ModelGeneric.UpCommandTarget = value; OnPropertyChanged(nameof(UpCommandTarget)); } }



        public ICommand DownCommand { get => ModelGeneric.DownCommand; set { ModelGeneric.DownCommand = value; OnPropertyChanged(nameof(DownCommand)); } }

        public object DownCommandParameter { get => ModelGeneric.DownCommandParameter; set { ModelGeneric.DownCommandParameter = value; OnPropertyChanged(nameof(DownCommandParameter)); } }

        public IInputElement DownCommandTarget { get => ModelGeneric.DownCommandTarget; set { ModelGeneric.DownCommandTarget = value; OnPropertyChanged(nameof(DownCommandTarget)); } }

        public UpDownButtonViewModel(T model) : base(model) { /* Left empty. */ }
    }
}
