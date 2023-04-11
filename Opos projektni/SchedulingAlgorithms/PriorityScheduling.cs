using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class PriorityScheduling:SchedulingAlgorithm
    {
        [NonSerialized]
        private PriorityQueue<Task, int> tasks = new PriorityQueue<Task, int>(Comparer<int>.Create((x, y) => y - x));


     

        internal override void PushTask(Task task)
        {
            this.tasks.Enqueue(task, (int)task.Priority);
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
            this.tasks= new PriorityQueue<Task, int>(Comparer<int>.Create((x, y) => y - x));
        }

    }
}
