using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    internal class ExecutionTimeCheckerThread
    {
        private Task task;
        private Stopwatch stopwatch;
        private long executionTime;
        private Thread thread;

        public ExecutionTimeCheckerThread(Task task, Stopwatch stopwatch, long executionTime)
        {
            this.task = task;
            this.stopwatch = stopwatch;
            this.executionTime = executionTime;
            this.thread = new Thread(Run);
        }

        public void Start()
        {
            this.thread.Start();
        }

        private void Run()
        {
            while(true)
            {
                bool status = false;
                Thread.Sleep(100);
                lock(task.taskLock)
                {
                    if (task.state == Task.TaskState.TERMINATED)
                        break;
                    if(task.state==Task.TaskState.PAUSED)
                    {
                        status = true;
                    }
                }
                if(status)
                {
                    lock(task.pauseLock)
                    {
                        Monitor.Wait(task.pauseLock);
                    }
                }
                if (stopwatch.ElapsedMilliseconds >= executionTime)
                {
                    task.RequestTerminate();
                    break;
                }
            }
        }
    }
}
