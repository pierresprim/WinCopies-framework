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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinCopies.GUI.Controls
{
    public class Link : Button
    {

        /// <summary>
        /// Identifies the <see cref="UnderliningMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnderliningModeProperty = DependencyProperty.Register(nameof(UnderliningMode), typeof(LinkUnderliningMode), typeof(LinkRun), new PropertyMetadata(LinkUnderliningMode.UnderlineWhenMouseOverOrFocused));

        public LinkUnderliningMode UnderliningMode { get => (LinkUnderliningMode)GetValue(UnderliningModeProperty); set => SetValue(UnderliningModeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="NormalForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalForegroundProperty = DependencyProperty.Register(nameof(NormalForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.Blue));

        public Brush NormalForeground { get => (Brush)GetValue(NormalForegroundProperty); set => SetValue(NormalForegroundProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HighlightedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedForegroundProperty = DependencyProperty.Register(nameof(HighlightedForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.Aqua));

        public Brush HighlightedForeground { get => (Brush)GetValue(HighlightedForegroundProperty); set => SetValue(HighlightedForegroundProperty, value); }

        /// <summary>
        /// Identifies the <see cref="ActiveForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveForegroundProperty = DependencyProperty.Register(nameof(ActiveForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.RoyalBlue));

        public Brush ActiveForeground { get => (Brush)GetValue(ActiveForegroundProperty); set => SetValue(ActiveForegroundProperty, value); }

        /// <summary>
        /// Identifies the <see cref="SeenForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeenForegroundProperty = DependencyProperty.Register(nameof(SeenForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.MediumPurple));

        public Brush SeenForeground { get => (Brush)GetValue(SeenForegroundProperty); set => SetValue(SeenForegroundProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Seen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeenProperty = DependencyProperty.Register(nameof(Seen), typeof(bool), typeof(LinkRun), new PropertyMetadata(false));

        public bool Seen { get => (bool)GetValue(SeenProperty); set => SetValue(SeenProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Uri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UriProperty = DependencyProperty.Register(nameof(Uri), typeof(string), typeof(Link), new PropertyMetadata(null));

        public string Uri { get => (string)GetValue(UriProperty); set => SetValue(UriProperty, value); }

        static Link() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Link), new FrameworkPropertyMetadata(typeof(Link)));

        public Link()

        {



        }
    }
}
