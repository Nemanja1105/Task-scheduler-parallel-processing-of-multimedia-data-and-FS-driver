using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class Task : INotifyPropertyChanged
    {
        public enum TaskState
        {
            NOT_SCHEDULED, SCHEDULED, SCHEDULED_NOT_STARTED, RUNNING_WITH_PAUSE_REQUEST, RUNNING_WITH_TERMINATE_REQUEST, RUNNING, WAITING_TO_RESUME,
            PAUSED, TERMINATED, RUNNING_WITH_CONTEXT_SWITH_REQUEST, SWAPPED, BLOCKED_WAITING_FOR_RESOURCE, UNBLOCKED_WAITING_FOR_RESUME
        };

        internal TaskState state;//
        public object taskLock = new object();
        [NonSerialized]
        internal TaskExecutor executor;

        internal TaskSpecification specification;

        [NonSerialized]
        internal Action<Task> onStartTaskScheduling;
        [NonSerialized]
        internal Action<Task> onTaskFinished;
        [NonSerialized]
        internal Action<Task> onTaskPaused;
        [NonSerialized]
        internal Action<Task> onTaskContinue;
        [NonSerialized]
        internal Action<String, Task> onResourceLock;
        [NonSerialized]
        internal Action<String> onResourceUnlock;

        private double progress = 0.0;

        private object waitLock = new object();
        internal object pauseLock = new object();
        internal object resourseLock = new object();

        public Task()
        {

        }

        public Task(TaskSpecification specification, Action<Task> onStartTaskScheduling, Action<Task> onTaskFinish, Action<Task> onTaskPaused, Action<Task> onTaskContinue, Action<string, Task> onResourceLock, Action<string> onResourceUnlock)
        {
            this.specification = specification;

            this.onStartTaskScheduling = onStartTaskScheduling;
            this.onTaskFinished = onTaskFinish;
            this.onTaskPaused = onTaskPaused;
            this.onTaskContinue = onTaskContinue;


            this.executor = new TaskExecutor(this);
            this.state = TaskState.NOT_SCHEDULED;
            this.onResourceLock = onResourceLock;
            this.onResourceUnlock = onResourceUnlock;
        }


        public int Priority
        {
            get { return this.specification.Priority; }
            set { this.specification.Priority = value; }
        }

        public double Progress
        {
            get { return this.progress; }
            set { this.progress = value; NotifyPropertyChanged(); }

        }

        public String Name
        {
            get { return this.specification.Name; }
        }

        public TaskState State
        {
            get { return this.state; }
            set
            {
                this.state = value;
                NotifyPropertyChanged();
                //nije najbolje rjesenje da stalno radimo notify
                NotifyPropertyChanged(nameof(IsPausable));
                NotifyPropertyChanged(nameof(IsStartable));
                NotifyPropertyChanged(nameof(IsStoppable));
            }
        }


        internal IAction Action
        {
            get { return this.specification.Action; }
        }



        public void StartTaskScheduling()
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.SCHEDULED_NOT_STARTED)
                {
                    this.State = TaskState.SCHEDULED;
                    this.onStartTaskScheduling(this);
                }
                else if (this.state == TaskState.NOT_SCHEDULED)
                    throw new InvalidOperationException("Zadatak se mora prvo postaviti na rasporedjivanje");
                else
                    throw new InvalidOperationException("Zadatak je vec stavljen na rasporedjivanje");
            }
        }

        public void RequestPause()//zahtjev za terminiranje ima prioritet
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.RUNNING)
                {
                    this.State = TaskState.RUNNING_WITH_PAUSE_REQUEST;
                }
            }
        }

        public void RequestContinue()
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.PAUSED)
                {
                    this.State = TaskState.WAITING_TO_RESUME;
                    this.onTaskContinue(this);

                }
                else if (this.state == TaskState.RUNNING_WITH_PAUSE_REQUEST)
                {
                    this.State = TaskState.RUNNING;
                }
                else if (this.state == TaskState.SCHEDULED_NOT_STARTED)
                    this.StartTaskScheduling();
            }
        }

        internal void RequestContextSwitch()
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.RUNNING)
                {
                    this.State = TaskState.RUNNING_WITH_CONTEXT_SWITH_REQUEST;
                }
            }
        }

        internal void Block()
        {
            this.setTaskState(Task.TaskState.BLOCKED_WAITING_FOR_RESOURCE);
            this.onTaskPaused(this);//handler iz rasporedjivaca da ukloni trenutni iz running i uzme drugi zadatak
            lock (this.resourseLock)
            {
                Monitor.Wait(this.resourseLock);
            }
        }

        internal void UnBlock()
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.BLOCKED_WAITING_FOR_RESOURCE)
                {
                    this.State = TaskState.UNBLOCKED_WAITING_FOR_RESUME;
                    this.onTaskContinue(this);
                }
            }
        }



        public void RequestTerminate()
        {
            lock (this.taskLock)
            {
                if (this.state == TaskState.RUNNING || this.state == TaskState.RUNNING_WITH_PAUSE_REQUEST || this.state == TaskState.RUNNING_WITH_CONTEXT_SWITH_REQUEST)
                {
                    this.State = TaskState.RUNNING_WITH_TERMINATE_REQUEST;
                }
                else if (this.state == TaskState.NOT_SCHEDULED)
                    throw new InvalidOperationException("Zadatak nije pokrenut");
                else if (this.state == TaskState.TERMINATED)
                    throw new InvalidOperationException("Zadatak je vec zavrsen");
            }
        }

        public void Wait()
        {
            bool status = false;
            lock (this.taskLock)
            {
                if (this.state == TaskState.TERMINATED)
                    return;
                else
                {
                    status = true;
                }

            }
            if (status)
            {
                lock (this.waitLock)
                {
                    Monitor.Wait(this.waitLock);
                }
            }
        }

        internal void Start()
        {
            bool status = false;
            bool status2 = false;
            lock (this.taskLock)
            {
                if (this.state == TaskState.SCHEDULED)
                {
                    this.State = TaskState.RUNNING;
                    this.executor.Start();
                }
                else if (this.state == TaskState.WAITING_TO_RESUME || this.state == TaskState.SWAPPED)
                {
                    this.State = TaskState.RUNNING;
                    status = true;
                }
                else if (this.state == TaskState.UNBLOCKED_WAITING_FOR_RESUME)
                {
                    this.State = TaskState.RUNNING;
                    status2 = true;
                }
                else
                    throw new InvalidOperationException("Invalid task state");
            }
            if (status)
            {
                lock (this.pauseLock)
                {
                    Monitor.PulseAll(this.pauseLock);

                }
            }
            else if (status2)
            {
                lock (this.resourseLock)
                {
                    Monitor.Pulse(this.resourseLock);
                }
            }
        }

        internal void Finish()//finish poziva executor tako da on samo pristupa Finish
        {

            bool status = false;
            lock (this.taskLock)
            {

                if (this.state == TaskState.RUNNING || this.state == TaskState.RUNNING_WITH_PAUSE_REQUEST || this.state == TaskState.RUNNING_WITH_TERMINATE_REQUEST || this.state == TaskState.RUNNING_WITH_CONTEXT_SWITH_REQUEST)
                {

                    this.State = TaskState.TERMINATED;
                    NotifyPropertyChanged(nameof(IsTerminated));
                    this.onTaskFinished(this);
                    status = true;
                }
            }
            if (status)
            {
                lock (this.waitLock)
                {
                    Monitor.PulseAll(this.waitLock);
                }
            }
        }

        internal void setTaskState(TaskState state)
        {
            lock (this.taskLock)
            {
                this.State = state;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool IsPausable
        {
            get { return this.state == TaskState.RUNNING; }
        }

        public bool IsTerminated
        {
            get { return this.state == TaskState.TERMINATED; }
        }

        public bool IsStartable
        {
            get { return this.state == TaskState.SCHEDULED_NOT_STARTED || this.state == TaskState.PAUSED || this.state == TaskState.RUNNING_WITH_PAUSE_REQUEST; }
        }

        public bool IsStoppable
        {
            get { return this.state == TaskState.RUNNING || this.state == TaskState.RUNNING_WITH_PAUSE_REQUEST; }
        }

    }
}
