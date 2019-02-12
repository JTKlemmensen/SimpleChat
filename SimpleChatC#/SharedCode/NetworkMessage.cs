using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class NetworkMessage
    {
        public string Protocol { get; set; }
        public List<string> Arguments { get; set; }
    }
}