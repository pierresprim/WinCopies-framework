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

using WinCopies.GUI.IO.Process;

namespace WinCopies.GUI.IO.Controls.Process
{
    public class ProcessWindow : Windows.Window
    {
        public static readonly DependencyProperty ProcessesProperty = DependencyProperty.Register(nameof(Processes), typeof(System.Collections.Generic.IEnumerable<IProcess>), typeof(ProcessWindow));

        public System.Collections.Generic.IEnumerable<IProcess> Processes { get => (System.Collections.Generic.IEnumerable<IProcess>)GetValue(ProcessesProperty); set => SetValue(ProcessesProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(IProcess), typeof(ProcessWindow));

        public IProcess SelectedItem { get => (IProcess)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        static ProcessWindow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessWindow), new FrameworkPropertyMetadata(typeof(ProcessWindow)));
    }
}
