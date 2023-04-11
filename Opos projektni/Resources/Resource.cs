using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class Resource
    {
        private static Graph dependencyGraph = new Graph();

        private string name;
        [NonSerialized]
        private Task? owner;
        [NonSerialized]
        private int? ownerPriority = null;
        [NonSerialized]
        private PriorityQueue<Task, int> waitingTasks = new PriorityQueue<Task, int>(Comparer<int>.Create((x, y) => y - x));

        public Resource(String name)
        {
            this.name = name;
            this.owner = null;
        }

        public string Name { get { return this.name; } }


        public void TryLock(Task task)
        {
            bool status = false;
            lock (this)
            {
                if (this.owner != null)
                {
                    dependencyGraph.TryAddEdge(task, this.owner);
                    this.waitingTasks.Enqueue(task, task.Priority);
                    if (task.Priority > this.owner.Priority)
                    {
                        ownerPriority = owner.Priority;
                        this.owner.Priority = task.Priority;
                    }
                    status = true;

                }
                else
                    this.owner = task;
            }
            if (status)
            {
                task.Block();
                this.owner = task;
            }
        }


        public void Unlock()
        {
            lock (this)
            {
                if (this.owner != null)
                {
                    if (this.ownerPriority != null)
                    {
                        this.owner.Priority = this.ownerPriority.Value;
                        this.ownerPriority = null;
                    }
                    this.owner = null;
                    if (this.waitingTasks.Count != 0)
                    {
                        var task = this.waitingTasks.Dequeue();
                        dependencyGraph.RemoveEdge(task);
                        task.UnBlock();
                    }
                }
            }
        }



    }
}
