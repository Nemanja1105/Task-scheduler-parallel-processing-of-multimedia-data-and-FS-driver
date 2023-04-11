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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Opos_projektni;
using Opos_projektni.SchedulingAlgorithms;

namespace TaskSchedulerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.checkSaved();

            var list = typeof(SchedulingAlgorithm).Assembly.GetTypes().Where((t) => t.IsSubclassOf(typeof(SchedulingAlgorithm))).ToList();
            SchedulingCombo.DataContext = new { Algorithms = list };
        }

        private void checkSaved()
        {
            var list = Opos_projektni.Serialization.SchedulerSerialization.GetSavedContexts();
            if (list.Length != 0)
            {
                if (MessageBox.Show("Detektovan je sacuvani kontekst!\n Da li zelite da ga ucitate?", "Detektovan sacuvani kontekst", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SavedContextsChoiseFrame saved = new SavedContextsChoiseFrame(list);
                    this.Hide();
                    saved.Show();
                }
            }
        }

        private void MaxNumTask_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "Enter")
                textBox.Text = "";
        }

        private void MaxNumTask_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "Enter";
        }

        private void getSubClasses()
        {
            var list = typeof(SchedulingAlgorithm).Assembly.GetTypes().Where((t) => t.IsSubclassOf(typeof(SchedulingAlgorithm)));



        }

        private void SchedulingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var alg = (Type)SchedulingCombo.SelectedItem;
            if (alg.IsSubclassOf(typeof(RoundRobinScheduling)) || alg.Equals(typeof(RoundRobinScheduling)))
                RoundRobinPanel.Visibility = Visibility.Visible;
            else
                RoundRobinPanel.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int maxNumOfTask = 1;
            SchedulingAlgorithm algorithm;
            try
            {
                maxNumOfTask = int.Parse(MaxNumTask.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid maximum number of concurrent taks", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MaxNumTask.Clear();
                MaxNumTask.Text = "Enter";
                return;
            }
            Type alg = (Type)SchedulingCombo.SelectedItem;
            if (alg == null)
            {
                MessageBox.Show("Please select scheduling algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int timeSlice = 1000;
            if (alg.IsSubclassOf(typeof(RoundRobinScheduling)) || alg.Equals(typeof(RoundRobinScheduling)))
            {
                try
                {
                    timeSlice = int.Parse(TimeSliceTextBox.Text);
                    algorithm = (SchedulingAlgorithm)Activator.CreateInstance(alg, timeSlice);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid time slice value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    TimeSliceTextBox.Clear();
                    TimeSliceTextBox.Text = "Enter";
                    return;
                }
            }
            else
                algorithm = (SchedulingAlgorithm)Activator.CreateInstance(alg);
            TaskSchedulerMainFrame mainFrame = new TaskSchedulerMainFrame(maxNumOfTask, algorithm);
            this.Hide();
            mainFrame.Show();

        }

        private void MaxNumTask_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
