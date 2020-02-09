using System.Windows;
using System.Windows.Input;

namespace WinCopies.GUI.Controls.Themes
{
    public partial class Generic
    {

        public static ResourceDictionary ResourceDictionary { get; } = Util.Generic.AddNewDictionary("/WinCopies.GUI.Controls;component/Themes/Generic.xaml");

        public static T GetResource<T>(object key) => (T)ResourceDictionary[key];

        #region Resources

        public static Style ReadOnlyTextBoxStyle => GetResource<Style>(nameof(ReadOnlyTextBoxStyle));

        #endregion

        // todo: commands

        private void ScrollToLeftButton_Click(object sender, RoutedEventArgs e)
        {

            if (sender is TabControl) ((TabControl)sender).OnScrollToLeftButtonClickInternal();

        }

        private void ScrollToLeftButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)

        {

            if (sender is TabControl) ((TabControl)sender).OnScrollToLeftButtonMouseDoubleClickInternal();

        }

        private void ScrollToRightButton_Click(object sender, RoutedEventArgs e)

        {

            if (sender is TabControl) ((TabControl)sender).OnScrollToRightButtonClickInternal();

        }

        private void ScrollToRightButton_MouseDoubleClick(object sender, RoutedEventArgs e)

        {

            if (sender is TabControl) ((TabControl)sender).OnScrollToRightButtonMouseDoubleClickInternal();

        }
    }
}
