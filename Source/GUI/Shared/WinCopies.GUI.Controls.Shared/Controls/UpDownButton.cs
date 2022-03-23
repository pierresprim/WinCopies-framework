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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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



        static UpDownButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(UpDownButton), new FrameworkPropertyMetadata(typeof(UpDownButton)));
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
