using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Configurator
{

    public class BusBuilder<T> where T : class, IConsumer
    {
        private BusAttribute<T> attribute;
        public BusBuilder()
        {
            attribute = new BusAttribute<T>();
        }
       
       

        public BusBuilder<T> ConsumerMethod(Func<T> consumerMethod)
        {
            attribute.ConsumerMethod = consumerMethod;
            return this;
        }
        public static implicit operator BusAttribute<T>(BusBuilder<T> builder)
        {
            return builder.attribute;
        }
        public BusAttribute<T> Build()
        {
            return attribute;
        }
    }   
}
