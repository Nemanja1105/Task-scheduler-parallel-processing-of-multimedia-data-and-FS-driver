using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni.SchedulingAlgorithms
{
    [Serializable]
    public class RoundRobinScheduling : FifoScheduling
    {
        protected int timeSlice;
        public RoundRobinScheduling(int timeSlice)
        {
            this.timeSlice = timeSlice;
        }
        internal override void ActionAfterFinish(Task t)
        {
            base.ActionAfterFinish(t);
            if (t.state != Task.TaskState.TERMINATED && t.state != Task.TaskState.PAUSED)
            {
                this.PushTask(t);
            }
        }

        protected virtual int getInterval(Task t) { return this.timeSlice; }

        internal override void ActionBeforeStarting(Task task)
        {
            base.ActionBeforeStarting(task);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = getInterval(task);
            timer.AutoReset = false;
            timer.Elapsed += (o, e) =>
            {
                task.RequestContextSwitch();
                lock (timer)
                {
                    Monitor.Pulse(timer);
                }
            };
            Thread t = new Thread(() =>
            {
                timer.Start();
                lock (timer)
                {
                    Monitor.Wait(timer);
                }
            });
            t.Start();
        }


    }
}
