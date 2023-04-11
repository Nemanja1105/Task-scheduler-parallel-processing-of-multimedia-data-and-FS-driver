using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class PriorityPreemptiveScheduling : PriorityScheduling
    {
        internal override void PushTask(Task task)
        {

            //potrazi zadatak sa najmanjim prioritetom u running
            var swappingTask = FindTaskWithMinPriority();
            if (swappingTask != null && swappingTask.Priority < task.Priority)
                swappingTask.RequestContextSwitch();
            base.PushTask(task);
            //kada prihravati zahtjev izvrsava isti kod kao za pauzu i uzima zadatak najveceg prioriteta

        }

        internal override void ActionAfterFinish(Task t)
        {
            base.ActionAfterFinish(t);
            if (t.state != Task.TaskState.TERMINATED && t.state != Task.TaskState.PAUSED && t.state != Task.TaskState.BLOCKED_WAITING_FOR_RESOURCE)
            {
                this.PushTask(t);
            }
        }

        private Task? FindTaskWithMinPriority()
        {
            var t = this.runningTasks.MinBy((t) => t.Priority);
            return t;
        }


    }
}
