using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    internal class InterruptException : Exception
    {
        public InterruptException() : base("Prekid") { }
        public InterruptException(string message) : base(message) { }
    }
}
