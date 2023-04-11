using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    internal class TaskExecutor
    {
        private Task task;
        private Thread executeThread;

        public TaskExecutor(Task task)
        {
            this.task = task;
            this.executeThread = new Thread(Execute);
            this.executeThread.IsBackground = true;
        }

        internal void Start()
        {
            executeThread.Start();
        }

        private void Execute()
        {
            try
            {
                task.Action.Run(new CoopMechanism(task));
            }
            catch (Exception e) { }
            finally
            {
                task.Finish();
            }
        }
    }
}
