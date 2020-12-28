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

#if !(NETCORE || NET5)
using System.Diagnostics;
#endif
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using static WinCopies.
#if WinCopies3
    UtilHelpers;

    using WinCopies.Commands;
#else
    Util.Util;

using WinCopies.Util
#endif
    ;

namespace WinCopies.GUI.Controls
{
    /// <summary>
    /// Provides some properties to define a link <see cref="Run"/>. See the remarks section.
    /// </summary>
    /// <remarks>
    /// Usage of <see cref="TextElement.Foreground"/> and <see cref="Inline.TextDecorations"/> is deprecated, particularly to set a new value, for the foreground and text decorations for this type. Consider using the <see cref="NormalForeground"/> property instead for the <see cref="TextElement.Foreground"/> property.
    /// </remarks>
    public class LinkRun : Run, ICommandSource
    {
        //private static readonly DependencyPropertyKey IsActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsActive), typeof(bool), typeof(LinkRun), new PropertyMetadata(false)); 

        //public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty; 

        //public bool IsActive { get => (bool)GetValue(IsActiveProperty); } 

        /// <summary>
        /// Identifies the <see cref="UnderliningMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnderliningModeProperty = DependencyProperty.Register(nameof(UnderliningMode), typeof(LinkUnderliningMode), typeof(LinkRun), new PropertyMetadata(LinkUnderliningMode.UnderlineWhenMouseOverOrFocused, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((LinkRun)d).OnUnderliningModeChanged(e)));

        /// <summary>
        /// Gets or sets a <see cref="LinkUnderliningMode"/> value that describes how to underline this <see cref="LinkRun"/>. This is a dependency property.
        /// </summary>
        public LinkUnderliningMode UnderliningMode { get => (LinkUnderliningMode)GetValue(UnderliningModeProperty); set => SetValue(UnderliningModeProperty, value); }

        /// <summary>
        /// Identifies the <see cref="NormalForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalForegroundProperty = DependencyProperty.Register(nameof(NormalForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.Blue));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> foreground to use when this <see cref="LinkRun"/> is in a normal state (not highlighted, active or seen). This is a dependency property.
        /// </summary>
        public Brush NormalForeground { get => (Brush)GetValue(NormalForegroundProperty); set => SetValue(NormalForegroundProperty, value); }

        /// <summary>
        /// Identifies the <see cref="HighlightedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedForegroundProperty = DependencyProperty.Register(nameof(HighlightedForeground), typeof(Brush), typeof(LinkRun), new PropertyMetadata(Brushes.Aqua));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> foreground to use when this <see cref="LinkRun"/> is highlighted. See the remarks section. This is dependency property.
        /// </summary>
        /// <remarks>
        /// The following table show the possibility for a <see cref="LinkRun"/> to be highlighted:
        /// |---------------------------------------------------------------------------------------|
        /// | Is mouse over                 |   *   | true  | true  | false | false | true  | false |
        /// | Is mouse left button pressed  | false | true  | false | true  | false | true  | true  |
        /// | Is focused                    | true  | true  | false | true  | false | false | false |
        /// | 
        /// | Is highlighted                | true  | false | true  | true  | false |   /   | false |
        /// |---------------------------------------------------------------------------------------|
        /// </remarks>
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
        public static readonly DependencyProperty SeenProperty = DependencyProperty.Register(nameof(Seen), typeof(bool), typeof(LinkRun), new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((LinkRun)d).OnSeenChanged(e)));

        public bool Seen { get => (bool)GetValue(SeenProperty); set => SetValue(SeenProperty, value); }

        /// <summary>
        /// Identifies the <see cref="Uri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UriProperty = DependencyProperty.Register(nameof(Uri), typeof(string), typeof(LinkRun), new PropertyMetadata(null));

        public string Uri { get => (string)GetValue(UriProperty); set => SetValue(UriProperty, value); }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(LinkRun));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(LinkRun));

        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(LinkRun));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinkRun));

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);

            remove => RemoveHandler(ClickEvent, value);
        }

        static LinkRun()
        {
            // Run.ForegroundProperty.OverrideMetadata(typeof(LinkRun), new FrameworkPropertyMetadata(Brushes.Blue)); 

            CursorProperty.OverrideMetadata(typeof(LinkRun), new FrameworkPropertyMetadata(Cursors.Hand));

            FocusableProperty.OverrideMetadata(typeof(LinkRun), new FrameworkPropertyMetadata(true));
        }

        public LinkRun()
        {
            _ = BindingOperations.SetBinding(this, ForegroundProperty, new Binding(nameof(NormalForeground))
            {
                Source = this,

                Mode = BindingMode.TwoWay
            });

            Click += LinkRun_Click;
        }

        private void LinkRun_Click(object sender, RoutedEventArgs e) => OnClick(e);

        protected virtual void RemoveUnderlining()=> TextDecorations = new TextDecorationCollection();

        protected virtual void Underline() =>                        TextDecorations = new TextDecorationCollection() { new TextDecoration(TextDecorationLocation.Underline, null, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended) };

        protected virtual void OnUnderliningModeChanged(DependencyPropertyChangedEventArgs e) => OnChangeUnderlining((LinkUnderliningMode)e.NewValue);

            protected virtual void OnChangeUnderlining(LinkUnderliningMode underliningMode)
        {
            switch (underliningMode)
            {
                case LinkUnderliningMode.None:

                    RemoveUnderlining();

                    break;

                case LinkUnderliningMode.UnderlineWhenMouseOverOrFocused:

                    if ((IsMouseOver && Mouse.LeftButton == MouseButtonState.Released) || IsFocused)

                        Underline();

                    else

                        RemoveUnderlining();

                    break;

                case LinkUnderliningMode.UnderlineWhenNotMouseOverNorFocused:

                    if (!(IsMouseOver || IsFocused))

                        Underline();

                    else

                        RemoveUnderlining();

                    break;

                case LinkUnderliningMode.AlwaysUnderline:

                    Underline();

                    break;
            }
        }

        protected virtual void OnSeenChanged(DependencyPropertyChangedEventArgs e)
        {
             if ((bool)e.NewValue)
            
                OnHasBeenSeen();
            
            else             if ((IsMouseOver && Mouse.LeftButton == MouseButtonState.Released) || IsFocused)

                Highlight();

            else

                Dehighlight();
        }

        protected virtual void Highlight()
        {
            _ = BindingOperations.SetBinding(this, ForegroundProperty, new Binding(nameof(HighlightedForeground))
            {
                Source = this,

                Mode = BindingMode.TwoWay
            });

            OnChangeUnderlining(UnderliningMode);
        }

        protected virtual void Activate() => BindingOperations.SetBinding(this, ForegroundProperty, new Binding(nameof(ActiveForeground))
        {
            Source = this,

            Mode = BindingMode.TwoWay
        });

        protected virtual void OnHasBeenSeen()
        {
            BindingOperations.SetBinding(this, ForegroundProperty, new Binding(nameof(SeenForeground)) { Source = this, Mode = BindingMode.TwoWay });

            OnChangeUnderlining(UnderliningMode);
        }

        protected virtual void Dehighlight()
        {
            _ = BindingOperations.SetBinding(this, ForegroundProperty, new Binding(nameof(NormalForeground))
            {
                Source = this,

                Mode = BindingMode.TwoWay
            });

            OnChangeUnderlining(UnderliningMode);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (Mouse.LeftButton == MouseButtonState.Released && !IsFocused)

                Highlight();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            Highlight();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Activate();

            _ = CaptureMouse();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter && !IsMouseCaptured)
            {
                Highlight();

                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();

                if (IsMouseOver)
                {
                    Highlight();

                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsFocused)

                if (Seen)

                    OnHasBeenSeen();

                else

                    Dehighlight();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (!IsMouseOver)

                if (Seen)

                    OnHasBeenSeen();

                else

                    Dehighlight();
        }

        protected virtual void OnClick(RoutedEventArgs e)
        {
            Seen = true;

            Command?.TryExecute(CommandParameter, CommandTarget);

            if (IsNullEmptyOrWhiteSpace(Uri))

                return;

#if NETCORE || NET5
            Temp.StartProcessNetCore(Uri);
#else
            _ = Process.Start(Uri);
#endif
        }
    }
}
