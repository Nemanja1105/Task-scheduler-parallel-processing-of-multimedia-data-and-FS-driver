using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni.SchedulingAlgorithms
{
    [Serializable]
    internal class PriorityRoundRobinScheduling : RoundRobinScheduling
    {
        public PriorityRoundRobinScheduling(int timeSlice) : base(timeSlice) { }

        protected override int getInterval(Task t)
        {
            return (t.Priority + 1) * this.timeSlice;
        }
    }
}
