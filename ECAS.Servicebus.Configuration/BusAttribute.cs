using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Configurator
{
    public class BusAttribute<T> where T : class, IConsumer
    {
       public string QueueName { get; set; }

        public Func<T> ConsumerMethod { get; set; }

        public static BusBuilder<T> Create()
        {
            return new BusBuilder<T>();
        }
    }
}
