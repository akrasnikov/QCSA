using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Contracts
{
    public class MessageInfo:IMessageInfo
    {
        public int CommandId { get; set; }
        public string Message { get; set; }
        public string Comment { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
