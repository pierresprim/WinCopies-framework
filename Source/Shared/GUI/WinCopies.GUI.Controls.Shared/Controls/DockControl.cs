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

using System;
using System.Windows;
using System.Windows.Controls;

namespace WinCopies.GUI.Controls
{
    [Flags]
    public enum TopDocking : byte
    {
        None = 0,

        TopOverLeft = 1,

        TopOverRight = 2,

        TopOverLeftRight = TopOverLeft | TopOverRight
    }

    [Flags]
    public enum BottomDocking : byte
    {
        None = 0,

        BottomOverLeft = 1,

        BottomOverRight = 2,

        BottomOverLeftRight = BottomOverLeft | BottomOverRight
    }

    [TemplatePart(Name = PART_Top, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Left, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Right, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Bottom, Type = typeof(ContentPresenter))]
    public class DockControl : ContentControl
    {
        public const string PART_Top = "PART_Top";
        public const string PART_Left = "PART_Left";
        public const string PART_Right = "PART_Right";
        public const string PART_Bottom = "PART_Bottom";

        private ContentPresenter _top;
        private ContentPresenter _left;
        private ContentPresenter _right;
        private ContentPresenter _bottom;

        public static readonly DependencyProperty TopDockingProperty = DependencyProperty.Register(nameof(TopDocking), typeof(TopDocking), typeof(DockControl), new PropertyMetadata(TopDocking.TopOverLeftRight, (DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockControl)d).OnTopDockingChanged((TopDocking)e.OldValue, (TopDocking)e.NewValue)));

        public TopDocking TopDocking { get => (TopDocking)GetValue(TopDockingProperty); set => SetValue(TopDockingProperty, value); }



        public static readonly DependencyProperty BottomDockingProperty = DependencyProperty.Register(nameof(BottomDocking), typeof(BottomDocking), typeof(DockControl), new PropertyMetadata(BottomDocking.BottomOverLeftRight, (DependencyObject d, DependencyPropertyChangedEventArgs e)=>((DockControl)d).OnBottomDockingChanged((BottomDocking)e.OldValue, (BottomDocking)e.NewValue)));

        public BottomDocking BottomDocking { get => (BottomDocking)GetValue(BottomDockingProperty); set => SetValue(BottomDockingProperty, value); }

        #region Left
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(object), typeof(DockControl));

        public static readonly DependencyProperty LeftStringFormatProperty = DependencyProperty.Register(nameof(LeftStringFormat), typeof(string), typeof(DockControl));

        public static readonly DependencyProperty LeftTemplateProperty = DependencyProperty.Register(nameof(LeftTemplate), typeof(DataTemplate), typeof(DockPanel));

        public static readonly DependencyProperty LeftTemplateSelectorProperty = DependencyProperty.Register(nameof(LeftTemplateSelector), typeof(DataTemplateSelector), typeof(DockPanel));



        public object Left { get => GetValue(LeftProperty); set => SetValue(LeftProperty, value); }

        public string LeftStringFormat { get => (string)GetValue(LeftStringFormatProperty); set => SetValue(LeftStringFormatProperty, value); }

        public DataTemplate LeftTemplate { get => (DataTemplate)GetValue(LeftTemplateProperty); set => SetValue(LeftTemplateProperty, value); }

        public DataTemplateSelector LeftTemplateSelector { get => (DataTemplateSelector)GetValue(LeftTemplateSelectorProperty); set => SetValue(LeftTemplateSelectorProperty, value); }
        #endregion

        #region Right
        public static readonly DependencyProperty RightProperty = DependencyProperty.Register(nameof(Right), typeof(object), typeof(DockControl));

        public static readonly DependencyProperty RightStringFormatProperty = DependencyProperty.Register(nameof(RightStringFormat), typeof(string), typeof(DockControl));

        public static readonly DependencyProperty RightTemplateProperty = DependencyProperty.Register(nameof(RightTemplate), typeof(DataTemplate), typeof(DockPanel));

        public static readonly DependencyProperty RightTemplateSelectorProperty = DependencyProperty.Register(nameof(RightTemplateSelector), typeof(DataTemplateSelector), typeof(DockPanel));



        public object Right { get => GetValue(RightProperty); set => SetValue(RightProperty, value); }

        public string RightStringFormat { get => (string)GetValue(RightStringFormatProperty); set => SetValue(RightStringFormatProperty, value); }

        public DataTemplate RightTemplate { get => (DataTemplate)GetValue(RightTemplateProperty); set => SetValue(RightTemplateProperty, value); }

        public DataTemplateSelector RightTemplateSelector { get => (DataTemplateSelector)GetValue(RightTemplateSelectorProperty); set => SetValue(RightTemplateSelectorProperty, value); }
        #endregion

        #region Top
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(object), typeof(DockControl));

        public static readonly DependencyProperty TopStringFormatProperty = DependencyProperty.Register(nameof(TopStringFormat), typeof(string), typeof(DockControl));

        public static readonly DependencyProperty TopTemplateProperty = DependencyProperty.Register(nameof(TopTemplate), typeof(DataTemplate), typeof(DockPanel));

        public static readonly DependencyProperty TopTemplateSelectorProperty = DependencyProperty.Register(nameof(TopTemplateSelector), typeof(DataTemplateSelector), typeof(DockPanel));



        public object Top { get => GetValue(TopProperty); set => SetValue(TopProperty, value); }

        public string TopStringFormat { get => (string)GetValue(TopStringFormatProperty); set => SetValue(TopStringFormatProperty, value); }

        public DataTemplate TopTemplate { get => (DataTemplate)GetValue(TopTemplateProperty); set => SetValue(TopTemplateProperty, value); }

        public DataTemplateSelector TopTemplateSelector { get => (DataTemplateSelector)GetValue(TopTemplateSelectorProperty); set => SetValue(TopTemplateSelectorProperty, value); }
        #endregion

        #region Bottom
        public static readonly DependencyProperty BottomProperty = DependencyProperty.Register(nameof(Bottom), typeof(object), typeof(DockControl));

        public static readonly DependencyProperty BottomStringFormatProperty = DependencyProperty.Register(nameof(BottomStringFormat), typeof(string), typeof(DockControl));

        public static readonly DependencyProperty BottomTemplateProperty = DependencyProperty.Register(nameof(BottomTemplate), typeof(DataTemplate), typeof(DockPanel));

        public static readonly DependencyProperty BottomTemplateSelectorProperty = DependencyProperty.Register(nameof(BottomTemplateSelector), typeof(DataTemplateSelector), typeof(DockPanel));



        public object Bottom { get => GetValue(BottomProperty); set => SetValue(BottomProperty, value); }

        public string BottomStringFormat { get => (string)GetValue(BottomStringFormatProperty); set => SetValue(BottomStringFormatProperty, value); }

        public DataTemplate BottomTemplate { get => (DataTemplate)GetValue(BottomTemplateProperty); set => SetValue(BottomTemplateProperty, value); }

        public DataTemplateSelector BottomTemplateSelector { get => (DataTemplateSelector)GetValue(BottomTemplateSelectorProperty); set => SetValue(BottomTemplateSelectorProperty, value); }
        #endregion



        protected virtual void OnTopDockingChanged(TopDocking oldValue, TopDocking newValue)
        {
            if (newValue == TopDocking.None)
            {
                SetLeftOverTop(true);
                SetRightOverTop(true);
            }

            else if (newValue == TopDocking.TopOverLeft)
            {
                SetRightOverTop(false);
                SetTopOverLeft(false);
            }

            else if (newValue == TopDocking.TopOverRight)
            {
                SetLeftOverTop(false);
                SetTopOverRight(false);
            }

            else if (newValue == TopDocking.TopOverLeftRight)
            {
                SetTopOverLeft(true);
                SetTopOverRight(true);
            }
        }

        protected virtual void OnBottomDockingChanged(BottomDocking oldValue, BottomDocking newValue)
        {
            if (newValue == BottomDocking.None)
            {
                SetLeftOverBottom(true);
                SetRightOverBottom(true);
            }

            else if (newValue == BottomDocking.BottomOverLeft)
            {
                SetRightOverBottom(false);
                SetBottomOverLeft(false);
            }

            else if (newValue == BottomDocking.BottomOverRight)
            {
                SetLeftOverBottom(false);
                SetBottomOverRight(false);
            }

            else if (newValue == BottomDocking.BottomOverLeftRight)
            {
                SetBottomOverLeft(true);
                SetBottomOverRight(true);
            }
        }

        private static void SetColumnAndColumnSpan(in ContentPresenter contentPresenter, in int column, in int columnSpan)
        {
            Grid.SetColumn(contentPresenter, column);
            Grid.SetColumnSpan(contentPresenter, columnSpan);
        }

        private static void SetRowAndRowSpan(in ContentPresenter contentPresenter, in int row, in int rowSpan)
        {
            Grid.SetRow(contentPresenter, row);
            Grid.SetRowSpan(contentPresenter, rowSpan);
        }

        private void SetLeftOrRightOverTop(in ContentPresenter contentPresenter, in BottomDocking bottomDocking) => SetRowAndRowSpan(contentPresenter, 0, BottomDocking.HasFlag(bottomDocking) ? 2 : 3);

        private void SetTopOverLeftOrRight(in ContentPresenter contentPresenter, in BottomDocking bottomDocking) => SetRowAndRowSpan(contentPresenter, 1, BottomDocking.HasFlag(bottomDocking) ? 1 : 2);

        protected virtual void SetLeftOverTop(bool rightOverTop)
        {
            SetLeftOrRightOverTop(_left, BottomDocking.BottomOverLeft);

            SetColumnAndColumnSpan(_top, 1, rightOverTop ? 1 : 2);
        }

        protected virtual void SetRightOverTop(bool leftOverTop)
        {
            SetLeftOrRightOverTop(_right, BottomDocking.BottomOverRight);

            if (leftOverTop)

                SetColumnAndColumnSpan(_top, 1, 1);

            else

                SetColumnAndColumnSpan(_top, 0, 2);
        }

        protected virtual void SetTopOverLeft(bool topOverRight)
        {
            SetColumnAndColumnSpan(_top, 0, topOverRight ? 3 : 2);

            SetTopOverLeftOrRight(_left, BottomDocking.BottomOverLeft);
        }

        protected virtual void SetTopOverRight(bool topOverLeft)
        {
            if (topOverLeft)

                SetColumnAndColumnSpan(_top, 0, 3);

            else

                SetColumnAndColumnSpan(_top, 1, 2);

            SetTopOverLeftOrRight(_right, BottomDocking.BottomOverRight);
        }

        private void SetLeftOrRightOverBottom(in ContentPresenter contentPresenter, in TopDocking topDocking)
        {
            if (TopDocking.HasFlag(topDocking))

                SetRowAndRowSpan(contentPresenter, 1, 2);

            else

                SetRowAndRowSpan(contentPresenter, 0, 3);
        }

        private void SetBottomOverLeftOrRight(in ContentPresenter contentPresenter, in TopDocking topDocking)
        {
            if (TopDocking.HasFlag(topDocking))

                SetRowAndRowSpan(contentPresenter, 1, 1);

            else

                SetRowAndRowSpan(contentPresenter, 0, 2);
        }

        protected virtual void SetLeftOverBottom(bool rightOverBottom)
        {
            SetLeftOrRightOverBottom(_left, TopDocking.TopOverLeft);

            SetColumnAndColumnSpan(_bottom, 1, rightOverBottom ? 1 : 2);
        }

        protected virtual void SetRightOverBottom(bool leftOverBottom)
        {
            SetLeftOrRightOverBottom(_right, TopDocking.TopOverRight);

            if (leftOverBottom)

                SetColumnAndColumnSpan(_bottom, 1, 1);

            else

                SetColumnAndColumnSpan(_bottom, 0, 2);
        }

        protected virtual void SetBottomOverLeft(bool bottomOverRight)
        {
            SetColumnAndColumnSpan(_bottom, 0, bottomOverRight ? 3 : 2);

            SetBottomOverLeftOrRight(_left, TopDocking.TopOverLeft);
        }

        protected virtual void SetBottomOverRight(bool bottomOverLeft)
        {
            if (bottomOverLeft)

                SetColumnAndColumnSpan(_bottom, 0, 3);

            else

                SetColumnAndColumnSpan(_bottom, 1, 2);

            SetBottomOverLeftOrRight(_right, TopDocking.TopOverRight);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _top = GetTemplateChild(PART_Top) as ContentPresenter;

            _left = GetTemplateChild(PART_Left) as ContentPresenter;

            _right = GetTemplateChild(PART_Right) as ContentPresenter;

            _bottom = GetTemplateChild(PART_Bottom) as ContentPresenter;
        }
    }
}
