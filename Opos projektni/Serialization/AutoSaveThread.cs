using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni.Serialization
{
    public class AutoSaveThread
    {
        private static readonly int autosaveInterval = 5000;

        private TaskScheduler scheduler;
        private string prevFolder;

        public AutoSaveThread(TaskScheduler scheduler)
        {
            this.scheduler = scheduler;
            var thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(autosaveInterval);
                    if (!string.IsNullOrEmpty(prevFolder))
                        Directory.Delete(prevFolder, true);
                    this.prevFolder = SchedulerSerialization.Serialize(scheduler);
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
        }


    }
}
