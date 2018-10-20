using ECAS.Servicebus.Configurator;
using ECAS.Servicebus.Consumers;
using ECAS.Servicebus.Contracts;

using MassTransit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Microservice.Recognizer.Agent
{
    public partial class Service : ServiceBase
    {
        IBusControl bus;

        string username = "guest";
        string password = "guest";
        string uriHost = "rabbitmq://localhost";
        string queuename = "QRecognizer";

        public Service()
        {
            InitializeComponent();
        }
        public void StartService()
        {
            OnStart(null);
        }
        public void StopService()
        {
            OnStop();
        }
        protected override void OnStart(string[] args)
        {

            bus = BusConfigurator.Create(uriHost, username, password, queuename,
                () => new RecognizerConsumer(bus));

            bus.Start();
            bus.Publish(new MessageInfo { Message = "ServiceBus Start" });
        }

        protected override void OnStop()
        {
            bus.Stop();
        }
    }
}
