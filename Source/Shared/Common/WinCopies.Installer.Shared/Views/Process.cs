/* Copyright © Pierre Sprimont, 2022
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
using System.Windows.Input;

using WinCopies.Desktop;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.Installer.GUI
{
    public class Process : InstallerPageData
    {
        private static DependencyProperty Register<T>(in string propertyName) => Register<T, Process>(propertyName);

        public static readonly DependencyProperty OverallProgressProperty = Register<double>(nameof(OverallProgress));

        public double OverallProgress { get => (double)GetValue(OverallProgressProperty); set => SetValue(OverallProgressProperty, value); }

        public static readonly DependencyProperty CurrentItemProgressProperty = Register<double>(nameof(CurrentItemProgress));

        public double CurrentItemProgress { get => (double)GetValue(CurrentItemProgressProperty); set => SetValue(CurrentItemProgressProperty, value); }

        public static readonly DependencyProperty StepNameProperty = Register<string>(nameof(StepName));

        public string StepName { get => (string)GetValue(StepNameProperty); set => SetValue(StepNameProperty, value); }

        public static readonly DependencyProperty StepDataProperty = Register<byte>(nameof(StepData));

        public byte StepData { get => (byte)GetValue(StepDataProperty); set => SetValue(StepDataProperty, value); }

        public static readonly DependencyProperty CommandProperty = Register<ICommand>(nameof(Command));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty LogProperty = Register<string>(nameof(Log));

        public string Log { get => (string)GetValue(LogProperty); set => SetValue(LogProperty, value); }

        static Process() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<Process>();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _ = Command.TryExecute(null);
        }
    }
}
