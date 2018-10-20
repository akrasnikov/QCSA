using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Contracts
{
    interface IMessageRecognize
    {
        string CommandId { get; set; }
        string CameraId { get; set; }
        string ImagePath { get; set; }
        string Comment { get; set; }
        string Timestamp { get; set; }
    }
}
