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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using WinCopies.Desktop;

using static System.Windows.Input.NavigationCommands;

using static WinCopies.Util.Desktop.UtilHelpers;

using Application = System.Windows.Application;

namespace WinCopies.Installer.GUI
{
    public class InstallerWindow : Window
    {
        private static DependencyPropertyKey RegisterReadOnly<T>(in string propertyName, in T defaultValue) => RegisterReadOnly<T, InstallerWindow>(propertyName, defaultValue);

        private static readonly DependencyPropertyKey InstallerPropertyKey = RegisterReadOnly<IInstallerModel
#if CS8
            ?
#endif
            >(nameof(Installer), null);

        public static readonly DependencyProperty InstallerProperty = InstallerPropertyKey.DependencyProperty;

        public IInstallerModel Installer { get => (IInstallerModel)GetValue(InstallerProperty); private set => SetValue(InstallerPropertyKey, value); }

        static InstallerWindow() => DefaultStyleKeyProperty.OverrideDefaultStyleKey<InstallerWindow>();

        public InstallerWindow(in IInstallerModel installer)
        {
            SetResourceReference(StyleProperty, typeof(InstallerWindow));

            Installer = installer;

            AddCommandBindings();
        }

        protected virtual void AddCommandBindings()
        {
            void add(in ICommand command, Action action, Func<bool> condition) => CommandBindings.Add(command, (object sender, ExecutedRoutedEventArgs e) =>
            {
                action?.Invoke();
                e.Handled = true;
            }, (object sender, CanExecuteRoutedEventArgs e) =>
            {
                e.CanExecute = condition();
                e.Handled = true;
            });

            void updateInstaller(in ICommand command, Func<Action
#if CS8
                ?
#endif
                > func, in Func<bool> condition) => add(command, () => func()?.Invoke(), condition);

            updateInstaller(BrowseBack, () => Installer.Current.MovePrevious, () => Installer.Current.CanBrowseBack);
            updateInstaller(BrowseForward, () =>
            {
                IInstallerModel installer = Installer;
                IInstallerPageViewModel page = installer.Current;

                if (installer.Completed && !page.CanBrowseForward)
                {
                    Application.Current.Shutdown(0);

                    return null;
                }

                if (page is ICommonPage commonPage)
                {
                    string
#if CS8
                        ?
#endif
                        error = commonPage.Data.Error;

                    if (error != null)
                    {
                        _ = MessageBox.Show(error, "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        return null;
                    }
                }

                return page.MoveNext;
            }, () =>
            {
                IInstallerModel installer = Installer;

                return installer.Current.CanBrowseForward || installer.Completed;
            });

            add(Commands.DialogCommands.Cancel, Close, () => Installer.Current.CanCancel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            IInstallerModel installer = Installer;
            IInstallerPageViewModel current = installer.Current;

            if (installer.Completed)
            {
                if (current.CanBrowseForward)

                    e.Cancel = true;

                return;
            }

            if ((!current.CanBrowseForward || MessageBox.Show($"Are you sure you want to cancel the installation of {installer.ProgramName}?", "Installation Cancellation - Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No))

                e.Cancel = true;
        }
    }
}
