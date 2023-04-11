using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public abstract class SchedulingAlgorithm : IDeserializationCallback
    {
        [NonSerialized]
        internal List<Task> runningTasks = new List<Task>();
        internal abstract void PushTask(Task task);
        internal abstract Task PopTask();

        internal abstract bool QueueIsEmpty();

        internal virtual void ActionBeforeStarting(Task task)
        {
            this.runningTasks.Add(task);
        }

        internal virtual void ActionAfterFinish(Task task)
        {
            this.runningTasks.Remove(task);
        }

        public virtual void OnDeserialization(object? sender)
        {
            this.runningTasks = new List<Task>();
        }
    }
}

