using Opos_projektni;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
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
    /// Interaction logic for TaskSchedulerMainFrame.xaml
    /// </summary>
    public partial class TaskSchedulerMainFrame : Window
    {
        private Opos_projektni.TaskScheduler scheduler;


        public TaskSchedulerMainFrame(Opos_projektni.TaskScheduler scheduler)
        {
            InitializeComponent();
            this.scheduler = scheduler;
            new Opos_projektni.Serialization.AutoSaveThread(scheduler);
            listView.ItemsSource = scheduler.allTask;

        }

        public TaskSchedulerMainFrame(int maxNumOfTask, SchedulingAlgorithm schedulingAlgorithm)
        {
            InitializeComponent();
            this.scheduler = new Opos_projektni.TaskScheduler(maxNumOfTask, schedulingAlgorithm);
            new Opos_projektni.Serialization.AutoSaveThread(scheduler);   
            listView.ItemsSource = scheduler.allTask;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var windows = new AddNewTaskFrame();
            windows.ShowDialog();
            windows.Focus();
            if (windows.taskSpecification != null)
            {
                try
                {
                    scheduler.ScheduleWithoutStarting(windows.taskSpecification);
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message);
                    return;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = (Opos_projektni.Task)((Button)sender).DataContext;
                lock (task.taskLock)
                {
                    if (task.State == Opos_projektni.Task.TaskState.SCHEDULED_NOT_STARTED)
                        task.StartTaskScheduling();
                    else if (task.State == Opos_projektni.Task.TaskState.PAUSED || task.State == Opos_projektni.Task.TaskState.RUNNING_WITH_PAUSE_REQUEST)
                        task.RequestContinue();
                }
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message, "Invalid task state", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                var task = (Opos_projektni.Task)((Button)sender).DataContext;
                task.RequestPause();
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message, "Invalid task state", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = (Opos_projektni.Task)((Button)sender).DataContext;
                task.RequestTerminate();
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message, "Invalid task state", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var task = (Opos_projektni.Task)((Button)sender).DataContext;
            scheduler.allTask.Remove(task);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Opos_projektni.Serialization.SchedulerSerialization.Serialize(this.scheduler);
        }
    }
}
