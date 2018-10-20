using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECAS.DebugKit.RabbitMQ
{
    class Configurator
    {
        static IBusControl bus;
        public static IBusControl Create<T>() where T : class, IConsumer, new ()
        {

            bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                var host = configure.Host(new Uri(""), h =>
                {
                    h.Username("");
                    h.Password("");
                });
                configure.ReceiveEndpoint(host, "", endpoint =>
                {
                    endpoint.Consumer(() => new T());
                });

            });

            return bus;
        }

        internal static void Create<T>(Func<T> p) where T : class, IConsumer
        {
            var sdc = p;
            var sd = p();
            
            bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                var host = configure.Host(new Uri(""), h =>
                {
                    h.Username("");
                    h.Password("");
                });
                configure.ReceiveEndpoint(host, "", endpoint =>
                {
                    endpoint.Consumer( p );
                });

            });

           
        }


        //internal static void Create<T>(Func<T> p) where T : class, IConsumer, new()
        //{
        //    bus = Bus.Factory.CreateUsingRabbitMq(configure =>
        //    {
        //        var host = configure.Host(new Uri(""), h =>
        //        {
        //            h.Username("");
        //            h.Password("");
        //        });
        //        configure.ReceiveEndpoint(host, "", endpoint =>
        //        {
        //            endpoint.Consumer(() => new T());
        //        });

        //    });
        //}
    }
}
