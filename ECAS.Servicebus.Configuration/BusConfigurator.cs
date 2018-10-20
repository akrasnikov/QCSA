using ECAS.Servicebus.Consumers;
using ECAS.Servicebus.Contracts;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Servicebus.Configurator
{
    public class BusConfigurator
    {

        /// <summary>
        /// Метод создает Service Bus
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="queuename"></param>
        /// <param name="_host"> Получает доступ к IRabbitMqHost через делегат </param>
        /// <returns>IBusControl</returns>
        public static IBusControl Create(string hostAddress, string username, string password, string queuename, Action<IRabbitMqHost> _host)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                IRabbitMqHost host = configure.Host(new Uri(hostAddress), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
                _host(host);
            });

            return bus;
        }

        public static IRabbitMqHost CreateHost(string hostAddress, string username, string password)
        {
            IRabbitMqHost host = null;
            var bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                host = configure.Host(new Uri(hostAddress), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });

            return host;
        }
        public static HostReceiveEndpointHandle CreateQueue<T>(IRabbitMqHost host, string queuename, Func<T> consumerFactoryMethod) where T : class, IConsumer
        {
            var handle = host.ConnectReceiveEndpoint(queuename, x =>
           {
               x.Consumer(consumerFactoryMethod);
           });

            return handle;
        }

        ///// <summary>
        ///// C конструктором без параметров
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="hostAddress"></param>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <param name="queuename"></param>
        ///// <param name="consumerFactoryMethod"></param>
        ///// <returns></returns>
        //public static IBusControl Create<T>(string hostAddress, string username, string password, string queuename, Func<T> consumerFactoryMethod) where T : class, IConsumer, new()
        //{

        //    bus = Bus.Factory.CreateUsingRabbitMq(configure =>
        //    {
        //        var host = configure.Host(new Uri(hostAddress), h =>
        //        {
        //            h.Username(username);
        //            h.Password(password);
        //        });
        //        configure.ReceiveEndpoint(host, queuename, endpoint =>
        //        {
        //            endpoint.Consumer(consumerFactoryMethod);
        //        });

        //    });

        //    return bus;
        //}


        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hostAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="queuename"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <returns></returns>
        public static IBusControl Create<T>(string hostAddress, string username, string password, string queuename, Func<T> consumerFactoryMethod) where T : class, IConsumer
        {

            var bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                var host = configure.Host(new Uri(hostAddress), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
                configure.ReceiveEndpoint(host, queuename, endpoint =>
                {
                    endpoint.Consumer(consumerFactoryMethod);
                });

            });

            return bus;
        }


        public static IBusControl Create<T>(string hostAddress, string username, string password, string queuename) where T : class, IConsumer, new()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                var host = configure.Host(new Uri(hostAddress), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
                configure.ReceiveEndpoint(host, queuename, endpoint =>
                {
                    endpoint.Consumer(() => new T());
                });

            });

            return bus;
        }
    }


}
