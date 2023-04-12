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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.GUI.IO.Process;
using WinCopies.IO;
using WinCopies.IO.Process;

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO.Controls.Process
{
    /// <summary>
    /// Represents a process control.
    /// </summary>
    public class ProcessControl : Control
    {
        private static DependencyProperty Register<T>(in string propertyName) => Util.Desktop.UtilHelpers.Register<T, ProcessControl>(propertyName);

        /// <summary>
        /// Identifies the <see cref="Paths"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathsProperty = Register<ProcessTypes<IPathInfo>.IProcessQueue>(nameof(Paths));

        public ProcessTypes<IPathInfo>.IProcessQueue Paths { get => (ProcessTypes<IPathInfo>.IProcessQueue)GetValue(PathsProperty); set => SetValue(PathsProperty, value); }

        // todo: IQueue InitialPaths { get; }

        /// <summary>
        /// Identifies the <see cref="ProcessName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessNameProperty = Register<string>(nameof(ProcessName));

        public string ProcessName { get => (string)GetValue(ProcessNameProperty); set => SetValue(ProcessNameProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Error"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorProperty = Register<IProcessError>(nameof(Error));

        public IProcessError Error { get => (IProcessError)GetValue(ErrorProperty); set => SetValue(ErrorProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ErrorPaths"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorPathsProperty = Register<ProcessTypes<IProcessErrorItem>.IProcessQueue>(nameof(ErrorPaths));

        public ProcessTypes<IProcessErrorItem>.IProcessQueue ErrorPaths { get => (ProcessTypes<IProcessErrorItem>.IProcessQueue)GetValue(ErrorPathsProperty); set => SetValue(ErrorPathsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="InitialTotalSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialTotalSizeProperty = Register<Size>(nameof(InitialTotalSize));

        public Size InitialTotalSize { get => (Size)GetValue(InitialTotalSizeProperty); set => SetValue(InitialTotalSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ActualRemainingSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualRemainingSizeProperty = Register<Size>(nameof(ActualRemainingSize));

        public Size ActualRemainingSize { get => (Size)GetValue(ActualRemainingSizeProperty); set => SetValue(ActualRemainingSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsCompleted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCompletedProperty = Register<bool>(nameof(IsCompleted));

        public bool IsCompleted { get => (bool)GetValue(IsCompletedProperty); set => SetValue(IsCompletedProperty, value); }

        /// <summary>
        /// Identifies the <see cref="IsPaused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPausedProperty = Register<bool>(nameof(IsPaused));

        public bool IsPaused { get => (bool)GetValue(IsPausedProperty); set => SetValue(IsPausedProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SourcePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourcePathProperty = Register<string>(nameof(SourcePath));

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        public string SourcePath { get => (string)GetValue(SourcePathProperty); set => SetValue(SourcePathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ArePathsLoaded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArePathsLoadedProperty = Register<bool>(nameof(ArePathsLoaded));

        /// <summary>
        /// Gets or sets a value that indicates whether the paths of the associated process are loaded.
        /// </summary>
        public bool ArePathsLoaded { get => (bool)GetValue(ArePathsLoadedProperty); set => SetValue(ArePathsLoadedProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Status"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = Register<ProcessStatus>(nameof(Status));

        public ProcessStatus Status { get => (ProcessStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        /// <summary>
        /// Identifies the <see cref="InitialItemSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialItemSizeProperty = Register<Size>(nameof(InitialItemSize));

        /// <summary>
        /// Gets or sets the initial item size.
        /// </summary>
        public Size InitialItemSize { get => (Size)GetValue(InitialItemSizeProperty); set => SetValue(InitialItemSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="InitialItemCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialItemCountProperty = Register<uint>(nameof(InitialItemCount));

        /// <summary>
        /// Gets or sets the initial item count.
        /// </summary>
        public uint InitialItemCount { get => (uint)GetValue(InitialItemCountProperty); set => SetValue(InitialItemCountProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RemainingItemSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RemainingItemSizeProperty = Register<Size>(nameof(RemainingItemSize));

        /// <summary>
        /// Gets or sets the Remaining item size.
        /// </summary>
        public Size RemainingItemSize { get => (Size)GetValue(RemainingItemSizeProperty); set => SetValue(RemainingItemSizeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="RemainingItemCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RemainingItemCountProperty = Register<int>(nameof(RemainingItemCount));

        /// <summary>
        /// Gets or sets the Remaining item count.
        /// </summary>
        public int RemainingItemCount { get => (int)GetValue(RemainingItemCountProperty); set => SetValue(RemainingItemCountProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CurrentPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPathProperty = Register<string>(nameof(CurrentPath));

        public string CurrentPath { get => (string)GetValue(CurrentPathProperty); set => SetValue(CurrentPathProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ProgressPercentage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProgressPercentageProperty = Register<uint>(nameof(ProgressPercentage));

        public uint ProgressPercentage { get => (uint)GetValue(ProgressPercentageProperty); set => SetValue(ProgressPercentageProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CurrentPathProgressPercentage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPathProgressPercentageProperty = Register<sbyte>(nameof(CurrentPathProgressPercentage));

        public sbyte CurrentPathProgressPercentage { get => (sbyte)GetValue(CurrentPathProgressPercentageProperty); set => SetValue(CurrentPathProgressPercentageProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Actions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActionsProperty = Register<IProcessActions>(nameof(Actions));

        public IProcessActions Actions { get => (IProcessActions)GetValue(ActionsProperty); set => SetValue(ActionsProperty, value); }

        /// <summary>
        /// Identifies the <see cref="CustomActions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomActionsProperty = Register<IEnumerable<WinCopies.Util.Commands.Primitives.ICommand<IProcessErrorItem>>>(nameof(CustomActions));

        public IEnumerable<WinCopies.Util.Commands.Primitives.ICommand<IProcessErrorItem>> CustomActions { get => (IEnumerable<WinCopies.Util.Commands.Primitives.ICommand<IProcessErrorItem>>)GetValue(CustomActionsProperty); set => SetValue(CustomActionsProperty, value); }

        static ProcessControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessControl), new FrameworkPropertyMetadata(typeof(ProcessControl)));

        protected virtual void OnAddCommandBindings() => CommandBindings.Add(new CommandBinding(Commands.Commands.CommonCommand, (object sender, ExecutedRoutedEventArgs e) => OnExecuteCommand(e), (object sender, CanExecuteRoutedEventArgs e) => OnCanExecuteCommand(e)));

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
