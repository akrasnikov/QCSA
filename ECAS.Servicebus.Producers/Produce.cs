using ECAS.Servicebus.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Produces
{
    public class Produce
    {
        public async Task Send(ISendEndpoint endpoint)
        {
            await endpoint.Send<MessageInfo>(new
            {
                OrderId = "27",
                OrderDate = DateTime.UtcNow,
                OrderAmount = 123.45m
            });
        }
    }
}
