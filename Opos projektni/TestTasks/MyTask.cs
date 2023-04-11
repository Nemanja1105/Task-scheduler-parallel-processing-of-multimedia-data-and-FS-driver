using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Opos_projektni
{
    [Serializable]
    public class MyTask : IAction
    {
        private static int globalId = 1;
        private int id = globalId++;
        [JsonIgnore]
        private int currentIteration = 0;

        public int NumOfIteration { get; set; }
        public String Message { get; set; }

        public MyTask()
        {
            this.NumOfIteration = 3;
            this.Message = "Message...";
        }

        public MyTask(int numOfIteration, String message)
        {
            this.NumOfIteration = numOfIteration;
            this.Message = message;
        }

        public void Run(ICoopApi api)
        {
            api.TryLock("fajl1");
            Console.WriteLine("Pocetak zadatka:" + this.id);
            for (; currentIteration < NumOfIteration; currentIteration++)
            {
                api.CheckForStop();
                api.CheckForPause();
                api.CheckForContextSwitch();
                Console.WriteLine("Iteracija:" + currentIteration);
                Thread.Sleep(1000);
                api.SetProgress((double)(currentIteration + 1) / NumOfIteration);
            }
            Console.WriteLine("Kraj zadatka:" + this.id);
            api.Unlock("fajl1");
        }
    }
}
