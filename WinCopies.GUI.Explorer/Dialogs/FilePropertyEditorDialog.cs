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
using WinCopies.GUI.Explorer;

namespace WinCopies.GUI.Windows.Dialogs
{
    public class FilePropertyEditorDialog : DialogWindow
    {

        public static readonly DependencyProperty FilePropertiesProperty = DependencyProperty.Register(nameof(FileProperties), typeof(Array), typeof(FilePropertyEditorDialog));

        public Array FileProperties

        {

            get => (Array)GetValue(FilePropertiesProperty);

            set => SetValue(FilePropertiesProperty, value);

        }

        static FilePropertyEditorDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FilePropertyEditorDialog), new FrameworkPropertyMetadata(typeof(FilePropertyEditorDialog)));

        // /// <summary>
        // /// Initializes a new instance of the <see cref="FilePropertyEditorDialog"/> class.
        // /// </summary>
        // public FilePropertyEditorDialog() => Content = new Control { Template = (ControlTemplate)ResourcesHelper.Instance.ResourceDictionary["FilePropertyEditorDialogTemplate"] };
    }
}
