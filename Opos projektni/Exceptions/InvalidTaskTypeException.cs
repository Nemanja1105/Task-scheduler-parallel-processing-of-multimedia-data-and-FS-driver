using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos_projektni
{
    public class InvalidTaskTypeException : Exception
    {
        public InvalidTaskTypeException() : base("Tip zadatka se ne podudara sa algoritmom rasporedjivaca")
        {

        }

        public InvalidTaskTypeException(string message) : base(message)
        {

        }
    }
}
