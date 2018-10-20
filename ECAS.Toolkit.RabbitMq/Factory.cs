using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Toolkit.RabbitMq
{
    public class Factory:IDisposable
    {
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;
        EventingBasicConsumer basicConsumer;
        private string queueName;

        public event EventHandler Received;

        public Factory(string hostname, string username, string password)
        {
            try
            {
                factory = new ConnectionFactory();
                factory.HostName = hostname;
                factory.UserName = username;
                factory.Password = password;
                factory.VirtualHost = "/";
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                throw new InvalidOperationException("RabbitMQ Connection -> Incorrect login or password", ex.InnerException);
            }
            finally { }
        }


        public void CreateConsume()
        {
            basicConsumer = new EventingBasicConsumer(channel);
            basicConsumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Received(message, ea);
            };
            channel.BasicConsume(queue: queueName,
                                autoAck: true,
                                consumer: basicConsumer);
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }

        public void CreateQueue(string queueName)
        {
            this.queueName = queueName;
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
            channel.Close();
            channel.Dispose();
        }
    }
}
