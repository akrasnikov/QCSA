using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Contracts
{
    interface IMessageInfo
    {
        int CommandId { get; set; }
        string Message { get; set; }
        string Comment { get; set; }
        DateTime Timestamp { get; set; }
    }
}
