/* Copyright © Pierre Sprimont, 2020
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

using WinCopies.GUI.Controls;
using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.Process;

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO.Controls.Process
{
    /// <summary>
    /// Represents an I/O process control.
    /// </summary>
    public class ProcessControl : HeaderedControl
    {
        public static readonly DependencyProperty PathsProperty = DependencyProperty.Register(nameof(Paths), typeof(ProcessTypes<IPathInfo>.IProcessQueue), typeof(ProcessControl));

        ProcessTypes<IPathInfo>.IProcessQueue Paths { get => (ProcessTypes<IPathInfo>.IProcessQueue)GetValue(PathsProperty); set => SetValue(PathsProperty, value); }

        // todo: IQueue InitialPaths { get; }

        public static readonly DependencyProperty ProcessNameProperty = DependencyProperty.Register(nameof(ProcessName), typeof(string), typeof(ProcessControl));

        public string ProcessName { get => (string)GetValue(ProcessNameProperty); set => SetValue(ProcessNameProperty, value); }

        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(nameof(Error), typeof(IProcessError), typeof(ProcessControl));

        public IProcessError Error { get => (IProcessError)GetValue(ErrorProperty); set => SetValue(ErrorProperty, value); }

        public static readonly DependencyProperty ErrorPathsProperty = DependencyProperty.Register(nameof(ErrorPaths), typeof(ProcessTypes<IProcessErrorItem>.IProcessQueue), typeof(ProcessControl));

        public ProcessTypes<IProcessErrorItem>.IProcessQueue ErrorPaths { get => (ProcessTypes<IProcessErrorItem>.IProcessQueue)GetValue(ErrorPathsProperty); set => SetValue(ErrorPathsProperty, value); }

        public static readonly DependencyProperty InitialTotalSizeProperty = DependencyProperty.Register(nameof(InitialTotalSize), typeof(Size), typeof(ProcessControl));

        public Size InitialTotalSize { get => (Size)GetValue(InitialTotalSizeProperty); set => SetValue(InitialTotalSizeProperty, value); }

        public static readonly DependencyProperty ActualRemainingSizeProperty = DependencyProperty.Register(nameof(ActualRemainingSize), typeof(Size), typeof(ProcessControl));

        public Size ActualRemainingSize { get => (Size)GetValue(ActualRemainingSizeProperty); set => SetValue(ActualRemainingSizeProperty, value); }

        public static readonly DependencyProperty IsCompletedProperty = DependencyProperty.Register(nameof(IsCompleted), typeof(bool), typeof(ProcessControl));

        public bool IsCompleted { get => (bool)GetValue(IsCompletedProperty); set => SetValue(IsCompletedProperty, value); }

        public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register(nameof(IsPaused), typeof(bool), typeof(ProcessControl));

        public bool IsPaused { get => (bool)GetValue(IsPausedProperty); set => SetValue(IsPausedProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SourcePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourcePathProperty = DependencyProperty.Register(nameof(SourcePath), typeof(string), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        public string SourcePath { get => (string)GetValue(SourcePathProperty); set => SetValue(SourcePathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ArePathsLoaded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArePathsLoadedProperty = DependencyProperty.Register(nameof(ArePathsLoaded), typeof(bool), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets a value that indicates whether the paths of the associated process are loaded.
        /// </summary>
        public bool ArePathsLoaded { get => (bool)GetValue(ArePathsLoadedProperty); set => SetValue(ArePathsLoadedProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Status"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(ProcessStatus), typeof(ProcessControl));

        public ProcessStatus Status { get => (ProcessStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        /// <summary>
        /// Identifies the <see cref="InitialItemSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialItemSizeProperty = DependencyProperty.Register(nameof(InitialItemSize), typeof(Size), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the initial item size.
        /// </summary>
        public Size InitialItemSize { get => (Size)GetValue(InitialItemSizeProperty); set => SetValue(InitialItemSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="InitialItemCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialItemCountProperty = DependencyProperty.Register(nameof(InitialItemCount), typeof(uint), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the initial item count.
        /// </summary>
        public uint InitialItemCount { get => (uint)GetValue(InitialItemCountProperty); set => SetValue(InitialItemCountProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RemainingItemSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RemainingItemSizeProperty = DependencyProperty.Register(nameof(RemainingItemSize), typeof(Size), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the Remaining item size.
        /// </summary>
        public Size RemainingItemSize { get => (Size)GetValue(RemainingItemSizeProperty); set => SetValue(RemainingItemSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RemainingItemCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RemainingItemCountProperty = DependencyProperty.Register(nameof(RemainingItemCount), typeof(int), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the Remaining item count.
        /// </summary>
        public int RemainingItemCount { get => (int)GetValue(RemainingItemCountProperty); set => SetValue(RemainingItemCountProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CurrentPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPathProperty = DependencyProperty.Register(nameof(CurrentPath), typeof(IPathCommon), typeof(ProcessControl));

        public IPathCommon CurrentPath { get => (IPathCommon)GetValue(CurrentPathProperty); set => SetValue(CurrentPathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ProgressPercentage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProgressPercentageProperty = DependencyProperty.Register(nameof(ProgressPercentage), typeof(sbyte), typeof(ProcessControl));

        public sbyte ProgressPercentage { get => (sbyte)GetValue(ProgressPercentageProperty); set => SetValue(ProgressPercentageProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CurrentPathProgressPercentage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPathProgressPercentageProperty = DependencyProperty.Register(nameof(CurrentPathProgressPercentage), typeof(sbyte), typeof(ProcessControl));

        public sbyte CurrentPathProgressPercentage { get => (sbyte)GetValue(CurrentPathProgressPercentageProperty); set => SetValue(CurrentPathProgressPercentageProperty, value); }

        public static readonly DependencyProperty RunActionProperty = DependencyProperty.Register(nameof(RunAction), typeof(Action), typeof(ProcessControl));

        public Action RunAction { get => (Action)GetValue(RunActionProperty); set => SetValue(RunActionProperty, value); }

        public static readonly DependencyProperty PauseActionProperty = DependencyProperty.Register(nameof(PauseAction), typeof(Action), typeof(ProcessControl));

        public Action PauseAction { get => (Action)GetValue(PauseActionProperty); set => SetValue(PauseActionProperty, value); }

        public static readonly DependencyProperty CancelActionProperty = DependencyProperty.Register(nameof(CancelAction), typeof(Action), typeof(ProcessControl));

        public Action CancelAction { get => (Action)GetValue(CancelActionProperty); set => SetValue(CancelActionProperty, value); }

        static ProcessControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessControl), new FrameworkPropertyMetadata(typeof(ProcessControl)));

        protected virtual void OnAddCommandBindings() => CommandBindings.Add(new CommandBinding(WinCopies.Commands.Commands.CommonCommand, (object sender, ExecutedRoutedEventArgs e) => OnExecuteCommand(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanExecuteCommand(e)));

        protected virtual void OnCanExecuteCommand(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;

            e.Handled = true;
        }

        protected virtual void OnExecuteCommand(ExecutedRoutedEventArgs e)
        {
            ((Action)e.Parameter)();

            e.Handled = true;
        }
    }
}
