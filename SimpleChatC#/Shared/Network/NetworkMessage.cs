using Newtonsoft.Json;
using Shared.Network.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Network
{
    public class NetworkMessage
    {
        public MessageProtocols Protocol { get; set; }
        public string Message { get; set; }

        public bool TryGetObject<T>(out T obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject<T>(Message);
                return true;
            }
            catch (Exception)
            {
                obj = default(T);
                return false;
            }
        }
    }
}