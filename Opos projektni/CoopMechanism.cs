using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    internal class CoopMechanism : ICoopApi
    {
        private Task task;


        public CoopMechanism(Task task)
        {
            this.task = task;

        }

        public void TryLock(string resource)
        {
            task.onResourceLock(resource, task);
        }
        public void Unlock(string resource)
        {
            task.onResourceUnlock(resource);
        }

        public void CheckForPause()
        {
            bool status = false;
            lock (task.taskLock)
            {
                if (task.state == Task.TaskState.RUNNING_WITH_PAUSE_REQUEST)
                {
                    task.State = Task.TaskState.PAUSED;
                    this.task.onTaskPaused(this.task);
                    status = true;
                }
            }
            if (status)
            {

                lock (task.pauseLock)
                {
                    Monitor.Wait(this.task.pauseLock);
                }
            }

        }

        public int GetMaxDegreeOfParalellism() { return this.task.specification.MaxDegreeOfParallelism; }

        public void SetProgress(double value) { this.task.Progress = value; }

        public void CheckForStop()
        {
            lock (task.taskLock)
            {
                if (task.state == Task.TaskState.RUNNING_WITH_TERMINATE_REQUEST)
                {
                    throw new InterruptException();
                }
            }
        }

        public void CheckForContextSwitch()
        {
            bool status = false;
            lock (task.taskLock)
            {
                if (task.state == Task.TaskState.RUNNING_WITH_CONTEXT_SWITH_REQUEST)
                {
                    task.State = Task.TaskState.SWAPPED;
                    this.task.onTaskPaused(this.task);
                    status = true;
                }
            }
            if (status)
            {
                lock (task.taskLock)
                {
                    if (task.state != Task.TaskState.SWAPPED)
                        return;
                }
                lock (task.pauseLock)
                {
                    Monitor.Wait(this.task.pauseLock);
                }
            }
        }
    }
}
