using System.Windows;
using System.Windows.Input;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.Controls
{
    public class TabItem : System.Windows.Controls.TabItem
    {
        public MouseButtons ListenedButtons { get => ExtraProperties.GetListenedButtons(this); set => ExtraProperties.SetListenedButtons(this, value); }

        public static readonly RoutedEvent ClickEvent = Register<ClickEventHandler, TabItem>(nameof(Click), RoutingStrategy.Bubble);

        public event ClickEventHandler Click { add => AddHandler(ClickEvent, value); remove => RemoveHandler(ClickEvent, value); }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            using
#if !CS8
                (
#endif
                IValueObject<bool> isSelectedProperty = new PropertyValueObject<bool>(GetType().GetProperty(nameof(IsSelected)), this)
#if CS8
                ;
#else
                )
#endif

            Helpers.OnMouseDown(this, e, ListenedButtons, isSelectedProperty);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            Helpers.OnMouseUp(this, e, ListenedButtons);
        }
    }
}
