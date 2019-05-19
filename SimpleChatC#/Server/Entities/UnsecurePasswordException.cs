using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    public class UnsecurePasswordException : Exception
    {
        public UnsecurePasswordException() : base("A password must contain at least 6 characters, contain at least 1 number and 1 letter")
        {

        }
    }
}