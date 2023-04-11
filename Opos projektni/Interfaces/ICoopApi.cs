using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    public interface ICoopApi
    {
        public void CheckForPause();
        public void CheckForStop();

        public void CheckForContextSwitch();
        public int GetMaxDegreeOfParalellism();

        public void SetProgress(double progress);

        public void TryLock(string resource);

        public void Unlock(string resource);


    }
}
