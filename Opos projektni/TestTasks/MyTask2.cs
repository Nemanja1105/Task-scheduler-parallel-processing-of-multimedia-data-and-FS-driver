using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    public class MyTask2 : IAction
    {
        public void Run(ICoopApi api)
        {

            Console.WriteLine("Pocetak zadatka:");
            try
            {
                api.TryLock("R1");
            }
            catch (InvalidOperationException e)
            {
                api.SetProgress(-1);
                return;
            }
            for (int i = 0; i < 7; i++)
            {
                api.CheckForStop();
                api.CheckForPause();
                api.CheckForContextSwitch();
                Console.WriteLine("Iteracija:" + i);
                Thread.Sleep(1000);
            }
            api.Unlock("R1");
            Console.WriteLine("Kraj zadatka:");

        }
    }
}
