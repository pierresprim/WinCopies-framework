using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WinCopies.Desktop;
using WinCopies.Util;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI
{
    [Flags]
    public enum MouseButtons : byte
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 4,
        XButton1 = 8,
        XButton2 = 16
    }

    public static class ExtraProperties
    {
        public static readonly DependencyProperty ListenedButtonsProperty = RegisterAttached<MouseButtons>("ListenedButtons", typeof(ExtraProperties));

        public static MouseButtons GetListenedButtons(DependencyObject target) => (MouseButtons)target.GetValue(ListenedButtonsProperty);

        public static void SetListenedButtons(DependencyObject target, MouseButtons value) => target.SetValue(ListenedButtonsProperty, value);
    }

    public static class ExtraEvents
    {
        public static readonly RoutedEvent ClickEvent = RegisterRoutedEvent<ClickEventHandler, ListViewItem>("Click", RoutingStrategy.Bubble);

        public static void AddClickHandler(DependencyObject d, Delegate handler) => AddAttachedRoutedEventHandler(d, ClickEvent, handler);

        public static void RemoveClickHandler(DependencyObject d, Delegate handler) => RemoveAttachedRoutedEventHandler(d, ClickEvent, handler);
    }

    public static class Helpers
    {
        public static bool CheckButton(in MouseButtonEventArgs e, in MouseButtons listenedButtons) => !(e == null || listenedButtons == MouseButtons.None) && listenedButtons.HasFlag(e.ChangedButton.ToMouseButtons());

        public static void OnMouseDown(in UIElement uiElement, in MouseButtonEventArgs e, in MouseButtons listenedButtons, in IValueObject<bool> isSelectedProperty)
        {
            if (CheckButton(e, listenedButtons))
            {
                _ = uiElement.CaptureMouse();

                if (!isSelectedProperty.Value)
                {
                    Panel itemsPanel = uiElement.GetParent<ItemsControl>(false)?.GetItemsPanel();

                    if (itemsPanel != null)
                    {
#if CS8
                        static
#endif
                        bool tryDeselect(in object _item)
                        {
                            if (_item is ListBoxItem listBoxItem)
                            {
                                listBoxItem.IsSelected = false;

                                return false;
                            }

                            return true;
                        }

                        foreach (object item in itemsPanel.Children)

                            if (tryDeselect(item) && item is GroupItem groupItem)

                                foreach (object subItem in groupItem.GetItemsPanel().Children)

                                    _ = tryDeselect(subItem);
                    }

                    isSelectedProperty.Value = true;
                }
            }
        }

        public static void OnMouseUp(in UIElement uiElement, in MouseButtonEventArgs e, in MouseButtons listenedButtons)
        {
            if (uiElement.IsMouseCaptured && CheckButton(e, listenedButtons))
            {
                uiElement.ReleaseMouseCapture();

                if (uiElement.IsMouseOver)

                    uiElement.RaiseEvent(new ClickEventArgs(ExtraEvents.ClickEvent
#if CS9
                )
                    {
                        Button =
#else
                ,
#endif
                e.ChangedButton
#if CS9
                    }
#else
                )
#endif
                );
            }
        }
    }

    public static class Extensions
    {
        public static MouseButtons ToMouseButtons(this MouseButton button)
#if CS8
            =>
#else
        {
            switch (
#endif
            button
#if CS8
            switch
#else
            )
#endif
            {
#if !CS8
                case
#endif
                MouseButton.Left
#if CS8
                =>
#else
                : return
#endif
                MouseButtons.Left
#if CS8
                ,
#else
                ; case
#endif
                MouseButton.Right
#if CS8
                =>
#else
                : return
#endif
                MouseButtons.Right
#if CS8
                ,
#else
                ; case
#endif
                MouseButton.Middle
#if CS8
                =>
#else
                : return
#endif
                MouseButtons.Middle
#if CS8
                ,
#else
                ; case
#endif
                MouseButton.XButton1
#if CS8
                =>
#else
                : return
#endif
                MouseButtons.XButton1
#if CS8
                ,
#else
                ; case
#endif
                MouseButton.XButton2
#if CS8
                =>
#else
                : return
#endif
                MouseButtons.XButton2
#if CS8
                ,
                _ =>
#else
                ; default:
#endif
                throw new ArgumentOutOfRangeException(nameof(button))
#if CS8
            };
#else
                ;
            }
        }
#endif
    }
}
