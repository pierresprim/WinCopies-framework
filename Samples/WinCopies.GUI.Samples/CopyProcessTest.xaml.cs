using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinCopies.GUI.IO;
using WinCopies.GUI.IO.Process;

namespace WinCopies.GUI.Samples
{
    /// <summary>
    /// Interaction logic for CopyTest.xaml
    /// </summary>
    public partial class CopyTest : Window
    {
        private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Items), typeof(ObservableCollection<Copy>), typeof(CopyTest), new PropertyMetadata(new ObservableCollection<Copy>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        public ObservableCollection<Copy> Items => (ObservableCollection<Copy>)GetValue(ItemsProperty);

        public CopyTest()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var addNew = new AddNewCopy();

            if (addNew.ShowDialog() == true)
            {
                var Copy = IO.Process.Copy.From(new CopyProcessPathCollection(addNew.SourcePath), addNew.DestPath
#if DEBUG
, null
#endif
                    ) ;

                Copy.WorkerReportsProgress = true;
                Copy.WorkerSupportsCancellation = true;

                Items.Add(Copy);

                Copy.RunWorkerAsync();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Copy> items = Items;

            while (items.Count > 0)
            {
                items[0].CancelAsync();

                items.RemoveAt(0);
            }
        }
    }
}
