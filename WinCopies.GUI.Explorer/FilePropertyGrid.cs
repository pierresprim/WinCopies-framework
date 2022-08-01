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

using AttachedCommandBehavior;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinCopies.IO;

namespace WinCopies.GUI.Explorer
{
    public partial class FilePropertyGrid : Control
    {

        /// <summary>
        /// Identifies the <see cref="FileProperties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilePropertiesProperty = DependencyProperty.Register(nameof(FileProperties), typeof(ReadOnlyObservableCollection<ShellPropertyContainer>), typeof(FilePropertyGrid), new PropertyMetadata(null));

        public ReadOnlyObservableCollection<ShellPropertyContainer> FileProperties { get => (ReadOnlyObservableCollection<ShellPropertyContainer>)GetValue(FilePropertiesProperty); set => SetValue(FilePropertiesProperty, value); }

        static FilePropertyGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FilePropertyGrid), new FrameworkPropertyMetadata(typeof(FilePropertyGrid)));

        public FilePropertyGrid()

        {

            DataContextChanged += FilePropertyGrid_DataContextChanged;

            CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.EditProperty, EditProperty_Executed, EditProperty_CanExecute));

        }

        protected virtual void OnDataContextChanged(DependencyPropertyChangedEventArgs e)

        {

            if (e.NewValue is IO.ShellObjectInfo selectedItem)

            {

                ObservableCollection<ShellPropertyContainer> properties = new ObservableCollection<ShellPropertyContainer>();

                if (selectedItem.ShellObject.Properties.DefaultPropertyCollection != null)

                    foreach (IShellProperty property in selectedItem.ShellObject.Properties.DefaultPropertyCollection)

                        properties.Add(new ShellPropertyContainer(property));

                FileProperties = new ReadOnlyObservableCollection<ShellPropertyContainer>(properties);

                //Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellProperty<ulong?> shellProperty = selectedItem.ShellObject.Properties.System.Size;

                //shellProperty.Description.DisplayName;

#if DEBUG 

                // WinCopies.IO.RegistryInterop.test(selectedItem.ShellObject);

#endif 

            }

            else

                FileProperties = null;

        }

        private void FilePropertyGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => OnDataContextChanged(e);

        private void EditProperty_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void EditProperty_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

            Windows.Dialogs.FilePropertyEditorDialog filePropertyEditor = new Windows.Dialogs.FilePropertyEditorDialog();

            filePropertyEditor.FileProperties = (Array)((IShellProperty)e.Parameter).ValueAsObject;

#if DEBUG 

            filePropertyEditor.Command = new DelegateCommand { ExecuteDelegate = (object o) => Debug.WriteLine("Je passe par ici...") };

            Debug.WriteLine(((IShellProperty)e.Parameter).ValueAsObject.GetType().ToString());

#endif 

            filePropertyEditor.ShowDialog();

        }
    }
}
