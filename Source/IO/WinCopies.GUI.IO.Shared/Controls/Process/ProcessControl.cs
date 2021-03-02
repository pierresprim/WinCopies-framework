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

using System.Windows;

using WinCopies.GUI.Controls;

using Size = WinCopies.IO.Size;

namespace WinCopies.GUI.IO.Controls.Process
{
    public enum ProcessStatus : sbyte
    {
        None,

        InProgress,

        Succeeded,

        CancelledByUser,

        Error
    }

    /// <summary>
    /// Represents an I/O process control.
    /// </summary>
    public class ProcessControl : HeaderedControl
    {
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
        public static readonly DependencyProperty InitialItemCountProperty = DependencyProperty.Register(nameof(InitialItemCount), typeof(int), typeof(ProcessControl));

        /// <summary>
        /// Gets or sets the initial item count.
        /// </summary>
        public int InitialItemCount { get => (int)GetValue(InitialItemCountProperty); set => SetValue(InitialItemCountProperty, value); }

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
        public static readonly DependencyProperty CurrentPathProperty = DependencyProperty.Register(nameof(CurrentPath), typeof(IPathInfo), typeof(ProcessControl));

        public IPathInfo CurrentPath { get => (IPathInfo)GetValue(CurrentPathProperty); set => SetValue(CurrentPathProperty, value); }

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

        static ProcessControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessControl), new FrameworkPropertyMetadata(typeof(ProcessControl)));
    }
}
