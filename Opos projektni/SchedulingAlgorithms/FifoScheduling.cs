using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class FifoScheduling : SchedulingAlgorithm
    {

        [NonSerialized]
        private Queue<Task> tasks = new Queue<Task>();


        internal override void PushTask(Task task)
        {

            this.tasks.Enqueue(task);

        }

        internal override Task PopTask()
        {
            if (this.QueueIsEmpty())
                return null;
            return this.tasks.Dequeue();
        }

        internal override bool QueueIsEmpty()
        {
            return this.tasks.Count == 0;
        }

        public override void OnDeserialization(object? sender)
        {
            base.OnDeserialization(sender);
            this.tasks = new Queue<Task>();
        }


    }
}
