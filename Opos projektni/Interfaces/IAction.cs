using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    
    public interface IAction
    {
        public void Run(ICoopApi api);
    }
}
