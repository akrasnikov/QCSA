using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Contracts
{
    interface IMessageTrailerNumber
    {
        string CommandId { get; set; }
        string CameraId { get; set; }
        string TrailerPlateNumber { get; set; }      
        string Comment { get; set; }
        string Status { get; set; }
        string Timestamp { get; set; }
    }
}
