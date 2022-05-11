using System.Windows;
using System.Windows.Input;

namespace WinCopies.GUI
{
    public class ClickEventArgs : RoutedEventArgs
    {
        public MouseButton Button
        {
            get;
#if CS9
            init;
#endif
        }

        public ClickEventArgs(
#if CS9
            )
        { /* Left empty. */ }
#else
            in MouseButton button = MouseButton.Left) => Button = button;
#endif

        public ClickEventArgs(in RoutedEvent routedEvent
#if !CS9
            , in MouseButton button = MouseButton.Left
#endif
            ) : base(routedEvent)
#if CS9
        { /* Left empty. */ }
#else
            => Button = button;
#endif

        public ClickEventArgs(in RoutedEvent routedEvent, in object source
#if !CS9
            , in MouseButton button = MouseButton.Left
#endif
            ) : base(routedEvent, source)
#if CS9
        { /* Left empty. */ }
#else
            => Button = button;
#endif
    }

    public delegate void ClickEventHandler(object sender, ClickEventArgs e);
}
