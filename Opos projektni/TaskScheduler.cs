using Opos_projektni;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class TaskScheduler : IDeserializationCallback
    {
        private const int DEFAULT_MAX_NUM_OF_THREADS = 2;
        private int maxNumOfThreads;
        private SchedulingAlgorithm algorithm;//zadaci koji cekaju na rasporedjivanje running

        [NonSerialized]
        private List<Task> notstartedTasks = new List<Task>();
        [NonSerialized]
        public Dictionary<Task, Stopwatch> stopWatches = new Dictionary<Task, Stopwatch>();
        [NonSerialized]
        private Dictionary<Task, ExecutionTimeCheckerThread> executionContexts = new Dictionary<Task, ExecutionTimeCheckerThread>();
        [NonSerialized]
        public ObservableCollection<Task> allTask = new ObservableCollection<Task>();

        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();



        public TaskScheduler(int maxNumOfThreads, SchedulingAlgorithm algorithm)
        {
            this.maxNumOfThreads = maxNumOfThreads;
            this.algorithm = algorithm;
        }

        public void AddResource(Resource resource)
        {
            lock (this)
            {
                this.resources.Add(resource.Name, resource);
            }
        }

        public void RemoveResource(Resource resource)
        {
            lock (this)
            {
                this.resources.Remove(resource.Name);
            }
        }




        private void TryLockResource(String name, Task task)
        {
            var status = this.resources.TryGetValue(name, out var resource);
            if (status)
                resource.TryLock(task);
        }

        private void UnlockResource(String name)
        {
            var status = this.resources.TryGetValue(name, out var resource);
            if (status)
                resource.Unlock();
        }

        public Task Schedule(TaskSpecification taskSpecification)
        {
            if (taskSpecification == null)
                throw new ArgumentNullException();
            Task task = new Task(taskSpecification, startTaskHandler, finishTaskHandler, pauseTaskHandler, continueRequestHandler, TryLockResource, UnlockResource);
            lock (this)
            {
                allTask.Add(task);
            }
            if (taskSpecification.DeadLine != null)
                setTaskDeadLine(task, taskSpecification);
            if (taskSpecification.ExecutionTime != null)
            {
                Stopwatch stopwatch = new Stopwatch();
                this.stopWatches.Add(task, stopwatch);
                ExecutionTimeCheckerThread checker = new ExecutionTimeCheckerThread(task, stopwatch, taskSpecification.ExecutionTime.Value);
                executionContexts.Add(task, checker);

            }
            if (taskSpecification.StartingDateTime != null)
            {
                startTaskOnSpecificDateTime(task, taskSpecification);
            }
            else
            {
                task.setTaskState(Task.TaskState.SCHEDULED);
                this.startTask(task);
            }
            return task;
        }

        public Task ScheduleWithoutStarting(TaskSpecification taskSpecification)
        {
            if (taskSpecification == null)
                throw new ArgumentNullException();
            Task task = new Task(taskSpecification, startTaskHandler, finishTaskHandler, pauseTaskHandler, continueRequestHandler, TryLockResource, UnlockResource);

            task.setTaskState(Task.TaskState.SCHEDULED_NOT_STARTED);
            if (taskSpecification.DeadLine != null)
                setTaskDeadLine(task, taskSpecification);
            if (taskSpecification.ExecutionTime != null)
            {
                Stopwatch stopwatch = new Stopwatch();
                this.stopWatches.Add(task, stopwatch);
                ExecutionTimeCheckerThread checker = new ExecutionTimeCheckerThread(task, stopwatch, taskSpecification.ExecutionTime.Value);
                executionContexts.Add(task, checker);

            }
            if (taskSpecification.StartingDateTime != null)
            {
                startTaskOnSpecificDateTime(task, taskSpecification);
            }
            lock (this)
            {
                allTask.Add(task);
                if (!this.notstartedTasks.Contains(task))
                    this.notstartedTasks.Add(task);
            }
            return task;
        }



        private void startTaskOnSpecificDateTime(Task task, TaskSpecification taskSpecification)
        {
            if (taskSpecification.StartingDateTime < DateTime.Now)
                throw new ArgumentException("Datum mora biti veci od trenutnog");

            task.setTaskState(Task.TaskState.SCHEDULED_NOT_STARTED);
            lock (this)
            {
                this.notstartedTasks.Add(task);
                System.Timers.Timer timer = new System.Timers.Timer();
                TimeSpan interval = taskSpecification.StartingDateTime.GetValueOrDefault() - DateTime.Now;
                timer.Interval = Math.Max(interval.TotalMilliseconds, 1);
                timer.AutoReset = false;
                timer.Elapsed += (o, e) =>
                {
                    task.StartTaskScheduling();
                };

                timer.Start();
            }
        }

        private void setTaskDeadLine(Task task, TaskSpecification specification)
        {
            if (specification.DeadLine < DateTime.Now)
                throw new ArgumentException("Datum mora biti veci od trenutnog");
            System.Timers.Timer timer = new System.Timers.Timer();
            TimeSpan interval = specification.DeadLine.Value - DateTime.Now;
            timer.Interval = Math.Max(interval.TotalMilliseconds, 1);
            timer.AutoReset = false;
            timer.Elapsed += (o, e) =>
            {
                if (task.state != Task.TaskState.TERMINATED)
                    task.RequestTerminate();

            };

            timer.Start();
        }

        private void startTask(Task t)
        {
            lock (this)
            {
                if (this.algorithm.runningTasks.Count < this.maxNumOfThreads)
                {
                    if (stopWatches.ContainsKey(t))
                        stopWatches[t].Start();
                    this.algorithm.ActionBeforeStarting(t);

                    if (executionContexts.ContainsKey(t) && (t.state == Task.TaskState.SCHEDULED))
                        executionContexts[t].Start();
                    t.Start();
                }
                else
                {
                    this.algorithm.PushTask(t);
                }
            }
        }

        private void startTaskHandler(Task t)
        {
            lock (this)
            {
                this.notstartedTasks.Remove(t);
                this.startTask(t);
            }
        }

        private void finishTaskHandler(Task t)
        {

            lock (this)
            {
                if (stopWatches.ContainsKey(t))
                    stopWatches[t].Stop();
                this.algorithm.ActionAfterFinish(t);
                if (!this.algorithm.QueueIsEmpty())
                {
                    var task = this.algorithm.PopTask();
                    this.algorithm.ActionBeforeStarting(task);
                    task.Start();
                }
            }
        }

        private void continueRequestHandler(Task t)
        {
            this.startTask(t);
        }

        private void pauseTaskHandler(Task t)
        {
            this.finishTaskHandler(t);
        }


        public void OnDeserialization(object? sender)
        {
            this.notstartedTasks = new List<Task>();
            this.executionContexts = new Dictionary<Task, ExecutionTimeCheckerThread>();
            this.allTask = new ObservableCollection<Task>();
            this.stopWatches = new();
        }
    }






}

