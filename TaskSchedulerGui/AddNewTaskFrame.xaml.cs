using Opos_projektni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace TaskSchedulerGui
{
    /// <summary>
    /// Interaction logic for AddNewTaskFrame.xaml
    /// </summary>
    public partial class AddNewTaskFrame : Window
    {
        private static List<Type> taskTypes;

        public TaskSpecification? taskSpecification;


        static AddNewTaskFrame()
        {
            var type = typeof(IAction);
            taskTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != type).ToList();
        }
        public AddNewTaskFrame()
        {
            InitializeComponent();
            TaskTypeCombo.DataContext = new { TaskTypes = taskTypes };

        }


        private void DeadLineCheck_Checked(object sender, RoutedEventArgs e)
        {

            DeadLinePanel.IsEnabled = true;

        }

        private void StartTimeCheck_Checked(object sender, RoutedEventArgs e)
        {
            StartPanel.IsEnabled = true;
        }

        private void StartTimeCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            StartPanel.IsEnabled = false;

        }

        private void DeadLineCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            DeadLinePanel.IsEnabled = false;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "Enter")
                textBox.Text = "";
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "Enter";
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(nameTxt.Text) || nameTxt.Text == "Enter")
            {
                MessageBox.Show("Invalid task name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                nameTxt.Clear();
                nameTxt.Text = "Enter";
                return;
            }
            String name = nameTxt.Text;
            int priority = 0;
            if (!string.IsNullOrEmpty(priorityTxt.Text) && priorityTxt.Text != "Enter")
            {
                try
                {
                    priority = int.Parse(priorityTxt.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid priority", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    priorityTxt.Clear();
                    priorityTxt.Text = "Enter";
                    return;
                }
            }
            int? executionTime = null;
            if (!string.IsNullOrEmpty(maxExecTimeTxt.Text) && maxExecTimeTxt.Text != "Enter")
            {
                try
                {
                    executionTime = int.Parse(maxExecTimeTxt.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid maximum execution time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    maxExecTimeTxt.Clear();
                    maxExecTimeTxt.Text = "Enter";
                    return;
                }
            }
            int maxDegPar = 1;
            try
            {
                maxDegPar = int.Parse(maxParalelTxt.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid degree of parallelism", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                maxParalelTxt.Clear();
                maxParalelTxt.Text = "Enter";
                return;
            }
            DateTime? deadline = null;
            if (DeadLineCheck.IsChecked == true)
            {
                try
                {
                    var date = datePickerDeadLine.SelectedDate;
                    var tmp = timePickerDeadLineTxt.Text.Split(":");
                    int hh = int.Parse(tmp[0]);
                    int mm = int.Parse(tmp[1]);
                    int ss = int.Parse(tmp[2]);
                    deadline = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hh, mm, ss);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid deadline time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    timePickerDeadLineTxt.Clear();
                    timePickerDeadLineTxt.Text = "HH:mm:ss";
                    return;
                }
            }
            DateTime? startTime = null;
            if (StartTimeCheck.IsChecked == true)
            {
                try
                {
                    var date = datePickerStartTime.SelectedDate;
                    var tmp = timePickerStartTimeTxt.Text.Split(":");
                    int hh = int.Parse(tmp[0]);
                    int mm = int.Parse(tmp[1]);
                    int ss = int.Parse(tmp[2]);
                    startTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hh, mm, ss);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid start time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    timePickerStartTimeTxt.Clear();
                    timePickerStartTimeTxt.Text = "HH:mm:ss";
                    return;
                }
            }
            Type? taskType = (Type?)TaskTypeCombo.SelectedItem;
            if (taskType == null)
            {
                MessageBox.Show("Please select task type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            IAction? action = null;
            try
            {
                action = (IAction)JsonSerializer.Deserialize(taskConfigTxt.Text, taskType, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception)
            {
                MessageBox.Show("Please rewrite task config");
                this.TaskTypeCombo_SelectionChanged(null, null);
                return;
            }


            taskSpecification = new TaskSpecification(action, name, startTime, deadLine: deadline, executionTime, maxDegPar, priority);
            this.Close();
        }
        private String getJsonString(Type? type)
        {
            IAction action = (IAction)Activator.CreateInstance(type);
            return JsonSerializer.Serialize(action, action.GetType(), new JsonSerializerOptions { WriteIndented = true });
        }

        private void timePickerStartTimeTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "HH:mm:ss")
                textBox.Text = "";
        }

        private void timePickerStartTimeTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "HH:mm:ss";
        }

        private void TaskTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type? taskType = (Type?)TaskTypeCombo.SelectedItem;
            if (taskType != null)
            {
                string taskConfig = this.getJsonString(taskType);
                taskConfigTxt.Text = taskConfig;
            }
        }
    }
}
