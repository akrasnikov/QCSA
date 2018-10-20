using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Contracts
{
    public class MessageTrailerNumber:IMessageTrailerNumber
    {
        public string CommandId { get; set; }
        public string CameraId { get; set; }
        public string TrailerPlateNumber { get; set; }        
        public string Comment { get; set; }
        public string Status { get; set; }
        public string Timestamp { get; set; }
    }
}
