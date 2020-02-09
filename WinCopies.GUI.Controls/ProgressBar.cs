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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinCopies.GUI.Controls
{
    public partial class ProgressBar : Control
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ProgressBar), new PropertyMetadata(0d, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            ((ProgressBar)d).MinimumValueChanged?.Invoke(d, new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue));

        }));

        public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ProgressBar), new PropertyMetadata(100d, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        {

            ((ProgressBar)d).MaximumValueChanged?.Invoke(d, new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue));

        }));

        public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar), new PropertyMetadata(0d, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {

            // var d_ = ((LightProgressBar)d);

            var value = ((double)e.NewValue);

            var maximum = (double)d.GetValue(MaximumProperty);

            if (value > maximum) throw new Exception(

 "Invalid value assignment exception:'The value was upper than the maximum.' The value must be less than or equal to the maximum.");

            ((ProgressBar)d).ValueChanged?.Invoke(d, new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue));

            // ValueProperty = value

            //if (d_.ProcessStatus != ProcessStatuss.Normal) return;

            //double size = (d_.ActualWidth / d_.Maximum) * value;

            //d_.ProgressRectangle.Width = size;

        }));

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        // todo:

        public static readonly DependencyProperty ProcessStatusProperty = DependencyProperty.Register(nameof(ProcessStatus), typeof(ProcessStatus), typeof(ProgressBar), new PropertyMetadata(WinCopies.GUI.Controls. ProcessStatus.None, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>

        { 

            // var d_ = ((LightProgressBar)d);

            var value = ((ProcessStatus)e.NewValue);

            switch (value)

            {

                case ProcessStatus.Indeterminate:



                    var linearGradientBrush = new LinearGradientBrush()
                    {

                        EndPoint = new Point(0.5, 1),

                        StartPoint = new Point(0.5, 0),

                        Opacity = 0.8

                    };



                    linearGradientBrush.GradientStops.Add(

                                   new GradientStop((Color)new ColorConverter().ConvertFrom("#FFCDFFC1"), 0));

                    linearGradientBrush.GradientStops.Add(

                     new GradientStop((Color)new ColorConverter().ConvertFrom("#FF86EC6E"), 0.5));

                    linearGradientBrush.GradientStops.Add(

                         new GradientStop((Color)new ColorConverter().ConvertFrom("#FFCDFFC1"), 1));



                    // IndeterminateProgress_Timer.Start() 



                    break;



                    // Case ProcessStatuss.Normal 



                    // Me.Value = ValueProperty 



                    // End Select 

            }



        }));

        public ProcessStatus ProcessStatus { get => (ProcessStatus)GetValue(ProcessStatusProperty); set => SetValue(ProcessStatusProperty, value); }



        // todo: to add a event for the process status

        public event RoutedPropertyChangedEventHandler<double> MinimumValueChanged;

        public event RoutedPropertyChangedEventHandler<double> MaximumValueChanged;

        public event RoutedPropertyChangedEventHandler<double> ValueChanged;



        static ProgressBar() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBar), new FrameworkPropertyMetadata(typeof(ProgressBar)));



        public ProgressBar()
        {

            // InitializeComponent();

        }



        //public void this_SizeChanged(object sender, SizeChangedEventArgs e)

        //{

        //    if (ProcessStatus != ProcessStatuss.Normal) return;

        //    double size = (ActualWidth / Maximum) * Value;

        //    ProgressRectangle.Width = size;

        //}
    }
}
