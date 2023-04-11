using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskSchedulerGui
{
    /// <summary>
    /// Interaction logic for SavedContextsChoiseFrame.xaml
    /// </summary>
    public partial class SavedContextsChoiseFrame : Window
    {
        public SavedContextsChoiseFrame(String[] str)
        {
            InitializeComponent();
            ListView.ItemsSource = str;
        }



        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //String data = (String)(((ListViewItem)sender).Content);
            String data = (String)ListView.SelectedItem;
            if (data != null)
            {
                try
                {
                    var scheduler = Opos_projektni.Serialization.SchedulerSerialization.Deserialize(data);
                    TaskSchedulerMainFrame frame = new TaskSchedulerMainFrame(scheduler);
                    this.Hide();
                    frame.Show();
                }
                catch (Exception)
                {
                    MessageBox.Show("Greska", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
