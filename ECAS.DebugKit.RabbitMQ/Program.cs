using ECAS.Servicebus.Contracts;
using ECAS.Toolkit.Logger;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.DebugKit.RabbitMQ
{
    public class Message1 { public string Text { get; set; } }
    public class Message2 { public string Text { get; set; } }
    class Program
    {
        static void Main(string[] args)
        {

            //Logger.InitLogger();
            Logger.Log.Info("Logger Info ->Ok");
            Logger.Log.Error("Logger Error ->Ok");

            //Recognizer r = new Recognizer();

            //r.OnStartRecognition(string.Format(@"C:\Users\Aleksey\Desktop\fdsfasdf\true1.jpg"), x => { Console.WriteLine(x); });
            //Console.ReadLine();
            //var bus1 = Bus.Factory.CreateUsingRabbitMq(sbc =>
            //{
            //    var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
            //    {
            //        h.Username("guest");
            //        h.Password("guest");
            //    });

            //    sbc.ReceiveEndpoint(host, "queue-MessageInfo", ep =>
            //    {
            //        ep.Handler<MessageInfo>(context =>
            //        {
            //            StringBuilder sb = new StringBuilder();
            //            sb.Append(context.Message.CommandId); sb.AppendLine();
            //            sb.Append(context.Message.Message); sb.AppendLine();
            //            sb.Append(context.Message.Timestamp); sb.AppendLine();

            //            return Console.Out.WriteLineAsync($"MessageInfo: {sb.ToString()}");
            //        });
            //    });

            //});
            //var recognizer = new Recognizer();
            //Configurator.Create(() => new RecognizerConsumer(recognizer, bus1));

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "QMessageInfo", ep =>
                {
                    ep.Handler<MessageInfo>(context =>
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(context.Message.CommandId); sb.AppendLine();
                        sb.Append(context.Message.Message); sb.AppendLine();
                        sb.Append(context.Message.Timestamp); sb.AppendLine();

                        return Console.Out.WriteLineAsync($"MessageInfo: {sb.ToString()}");
                    });
                });
                sbc.ReceiveEndpoint(host, "QRecognizer", ep =>
                {
                    ep.Handler<MessageTrailerNumber>(context =>
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(context.Message.CommandId); sb.AppendLine();
                        sb.Append(context.Message.TrailerPlateNumber); sb.AppendLine();
                        sb.Append(context.Message.Status); sb.AppendLine();
                        sb.Append(context.Message.Timestamp); sb.AppendLine();
                        return Console.Out.WriteLineAsync($"MessageTrailerNumber: {sb.ToString()}");
                    });
                });
            });

            bus.Start(); // This is important!
            bus.Publish(new MessageInfo { Message = "Message Send" });
            bus.Publish(new MessageRecognize
            {
                CameraId = "1",
                ImagePath = string.Format(@"C:\Users\Aleksey\Desktop\fdsfasdf\true1.jpg"),
                CommandId = "2",
                Timestamp = DateTime.Now.ToString()
            });


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();


        }
    }
}
