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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using WinCopies.GUI.IO.Process;
using WinCopies.IO.Process;
using WinCopies.Util.Commands.Primitives;

using static WinCopies.Commands.ProcessCommands;
using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.IO.Controls.Process
{
    public class ProcessErrorItemControl : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, ProcessErrorItemControl>(propertyName);

        public static readonly DependencyProperty IconProperty = Register<ImageSource>(nameof(Icon));

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public static readonly DependencyProperty PathProperty = Register<string>(nameof(Path));

        public string Path { get => (string)GetValue(PathProperty); set => SetValue(PathProperty, value); }

        public static readonly DependencyProperty ErrorMessageProperty = Register<string>(nameof(ErrorMessage));

        public string ErrorMessage { get => (string)GetValue(ErrorMessageProperty); set => SetValue(ErrorMessageProperty, value); }

        public static readonly DependencyProperty ApplyToAllProperty = Register<bool>(nameof(ApplyToAllProperty));

        public bool ApplyToAll { get => (bool)GetValue(ApplyToAllProperty); set => SetValue(ApplyToAllProperty, value); }

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(nameof(Actions), typeof(IProcessActions), typeof(ProcessErrorItemControl), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ProcessErrorItemControl)d).RegisterCommandBindings()));

        public IProcessActions Actions { get => (IProcessActions)GetValue(ActionsProperty); set => SetValue(ActionsProperty, value); }

        public static readonly DependencyProperty CustomActionsProperty = Register<IEnumerable<ICommand<IProcessErrorItem>>>(nameof(CustomActions));

        public IEnumerable<ICommand<IProcessErrorItem>> CustomActions { get => (IEnumerable<ICommand<IProcessErrorItem>>)GetValue(CustomActionsProperty); set => SetValue(CustomActionsProperty, value); }

        static ProcessErrorItemControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessErrorItemControl), new FrameworkPropertyMetadata(typeof(ProcessErrorItemControl)));

        protected virtual CommandBinding GetCommandBinding(in System.Windows.Input. ICommand command, Action<bool> action) => new CommandBinding(command, (object sender, ExecutedRoutedEventArgs e) =>
        {
            e.Handled = true;

            action(ApplyToAll);
        }, Desktop.Delegates.CanExecute);

        protected virtual void AddCommandBinding(in System.Windows.Input. ICommand command, in Action<bool> action) => CommandBindings.Add(GetCommandBinding(command, action));

        protected virtual void RegisterCommandBindings()
        {
            if (Actions == null)

                return;

            AddCommandBinding(TryAgain, Actions.TryAgain);

            AddCommandBinding(Ignore, Actions.Ignore);

            _ = CommandBindings.Add(new CommandBinding(Commands.Commands.CommonCommand, (object sender, ExecutedRoutedEventArgs e) =>
              {
                  e.Handled = true;

                  ((Util.Commands.Primitives.ICommand)e.Parameter).Execute(DataContext);

                  Actions.TryAgain(ApplyToAll);
              }, (object sender, CanExecuteRoutedEventArgs e) =>
              {
                  e.Handled = true;

                  e.CanExecute = DataContext != null && ((Util.Commands.Primitives.ICommand)e.Parameter).CanExecute(DataContext);
              }));
        }
    }
}
