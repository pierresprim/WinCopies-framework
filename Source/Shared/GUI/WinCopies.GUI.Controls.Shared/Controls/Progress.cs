using System;
using System.Windows;
using System.Windows.Controls;

using WinCopies.Desktop;

namespace WinCopies.GUI.Controls
{
    public class ProgressBar : ContentControl
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, ProgressBar>(propertyName);

        public static readonly DependencyProperty MinValueProperty = Register<double>(nameof(MinValue));

        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }

        public static readonly DependencyProperty ValueProperty = Register<double>(nameof(Value));

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public static readonly DependencyProperty MaxValueProperty = Register<double>(nameof(MaxValue));

        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

        static ProgressBar() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<ProgressBar>();
    }

    public class Progress : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, Progress>(propertyName);
        private static DependencyProperty RegisterProgressProperty<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, Progress>(propertyName, new PropertyMetadata(0d, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            var newValue = (double)e.NewValue;

            if (newValue < 0d || newValue > 100d)

                throw new InvalidOperationException("The value must be a percentage.");
        }));

        public static readonly DependencyProperty OverallProgressProperty = RegisterProgressProperty<double>(nameof(OverallProgress));

        public double OverallProgress { get => (double)GetValue(OverallProgressProperty); set => SetValue(OverallProgressProperty, value); }

        public static readonly DependencyProperty OverallProgressHeaderProperty = Register<object>(nameof(OverallProgressHeader));

        public object OverallProgressHeader { get => GetValue(OverallProgressHeaderProperty); set => SetValue(OverallProgressHeaderProperty, value); }

        public static readonly DependencyProperty CurrentItemProgressProperty = RegisterProgressProperty<double>(nameof(CurrentItemProgress));

        public double CurrentItemProgress { get => (double)GetValue(CurrentItemProgressProperty); set => SetValue(CurrentItemProgressProperty, value); }

        public static readonly DependencyProperty CurrentItemProgressHeaderProperty = Register<object>(nameof(CurrentItemProgressHeader));

        public object CurrentItemProgressHeader { get => GetValue(CurrentItemProgressHeaderProperty); set => SetValue(CurrentItemProgressHeaderProperty, value); }

        static Progress() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Progress>();
    }
}
