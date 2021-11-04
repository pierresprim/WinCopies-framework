//* Copyright © Pierre Sprimont, 2021
//*
//* This file is part of the WinCopies Framework.
//*
//* The WinCopies Framework is free software: you can redistribute it and/or modify
//* it under the terms of the GNU General Public License as published by
//* the Free Software Foundation, either version 3 of the License, or
//* (at your option) any later version.
//*
//* The WinCopies Framework is distributed in the hope that it will be useful,
//* but WITHOUT ANY WARRANTY; without even the implied warranty of
//* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//* GNU General Public License for more details.
//*
//* You should have received a copy of the GNU General Public License
//* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using Microsoft.WindowsAPICodePack.Taskbar;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.Desktop;
using WinCopies.IO.Process;

namespace WinCopies.GUI.IO.Process
{
    public abstract class ProcessWindow : Windows.Window
    {
        protected struct NotificationIconData
        {
            public Icon Icon;
            public string ToolTip;

            public NotificationIconData(in Icon icon, in string toolTip)
            {
                Icon = icon;

                ToolTip = toolTip;
            }
        }

        private NotificationIcon _icon;
        private MenuItem _showWindowMenuItem;

        protected ProcessWindow(in ObservableCollection<IProcess> processes)
        {
            (processes ?? throw ThrowHelper.GetArgumentNullException(nameof(processes))).CollectionChanged += Processes_CollectionChanged;

            CommandBindings.Add(NotificationIconCommands.ShowWindow, ChangeWindowVisibilityState);

            ContentTemplateSelector = new InterfaceDataTemplateSelector();

            Content = new ProcessManager<IProcess>() { Processes = processes };
        }

        protected ProcessWindow() : this(new ObservableCollection<IProcess>()) { /* Left empty. */ }

        protected abstract NotificationIconData GetNotificationIconData();

        private void UpdateShowWindowMenuItemHeader(in string header)
        {
            if (_showWindowMenuItem != null)

                _showWindowMenuItem.Header = header;
        }

        private void HideWindow()
        {
            Hide();

            UpdateShowWindowMenuItemHeader("Show window");
        }

        private void ChangeWindowVisibilityState()
        {
            if (Visibility == Visibility.Visible)

                HideWindow();

            else
            {
                Show();

                _ = Activate();

                UpdateShowWindowMenuItemHeader("Minimize window");
            }
        }

        protected MenuItem AddMenuItem(in ContextMenu contextMenu, in ICommand command, in string header)
        {
            var menuItem = new MenuItem() { Command = command, CommandTarget = this };

            _ = contextMenu.Items.Add(menuItem);

            if (header != null)

                menuItem.Header = header;

            return menuItem;
        }

        protected virtual MenuItem GetShowWindowMenuItem(in ContextMenu contextMenu) => AddMenuItem(contextMenu, NotificationIconCommands.ShowWindow, "Minimize window");

        protected virtual ContextMenu GetNotificationIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            _showWindowMenuItem = GetShowWindowMenuItem(contextMenu);

            _ = AddMenuItem(contextMenu, NotificationIconCommands.Close, null);

            return contextMenu;
        }

        protected virtual NotificationIcon GetNotificationIcon()
        {
            NotificationIconData notificationIconData = GetNotificationIconData();

            var contextMenu = GetNotificationIconContextMenu();

            if (contextMenu != null)

                contextMenu.ContextMenuOpening += (object sender, ContextMenuEventArgs _e) => CommandManager.InvalidateRequerySuggested();

            return new NotificationIcon(this, Guid.NewGuid(), notificationIconData.Icon /*Properties.Resources.WinCopies*/, notificationIconData.ToolTip, contextMenu, true);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _icon = GetNotificationIcon();

            if (_icon != null)
            {
                _icon.LeftButtonUp += (object sender, EventArgs _e) => ChangeWindowVisibilityState();

                _ = _icon.Initialize();
            }
        }

        private void Processes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)

                foreach (object process in e.NewItems)

                    ((IProcess)process).RunWorkerAsync();
        }

        protected virtual void OnClosingCancelled() { /* Left empty. */ }

        protected abstract bool ValidateClosing();

        protected virtual void OnClosingValidated() {/* Left empty. */ }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ValidateClosing())
            {
                ObservableCollection<IProcess> processes = ((ProcessManager<IProcess>)Content).Processes;

                void cancel(in Action action)
                {
                    action?.Invoke();

                    OnClosingCancelled();

                    e.Cancel = true;
                }

                for (int i = 0; i < processes.Count; i++)
                {
                    if (processes[i].IsBusy)
                    {
                        cancel(() => MessageBox.Show("There are running processes. All processes have to complete or to be cancelled in order to close the application.", "WinCopies", MessageBoxButton.OK, MessageBoxImage.Information));

                        return;
                    }

                    else if (processes[i].Status == ProcessStatus.Error)

                        if (MessageBox.Show("There are errors in some processes. Are you sure you want to cancel they?", "WinCopies", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)

                            break;

                        else
                        {
                            cancel(null);

                            return;
                        }
                }

                _icon?.Dispose();

                OnClosingValidated();
            }

            else
            {
                HideWindow();

                e.Cancel = true;
            }
        }
    }
}
