using System.Windows;
using System.Windows.Controls;

using WinCopies.Util.Commands.Primitives;

namespace WinCopies.GUI.Templates
{
    public class MenuItemTemplateSelector : ItemContainerTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl) => item is ICommand 
                ? (DataTemplate)Application.Current.Resources["MenuItemDataTemplate"]
                : item == null
                ? (DataTemplate)Application.Current.Resources["SeparatorDataTemplate"]
                : base.SelectTemplate(item, parentItemsControl);
    }
}
