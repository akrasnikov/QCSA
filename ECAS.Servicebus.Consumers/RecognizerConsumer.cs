using ECAS.Servicebus.Contracts;
using MassTransit;
using System;
using System.Threading.Tasks;
using ECAS.Toolkit.Recognizer;


namespace ECAS.Servicebus.Consumers
{
    public class RecognizerConsumer : IConsumer<MessageRecognize>
    {
        readonly Action<MessageRecognize, string> action;
        private IRecognizer recognizer;
        readonly IBusControl busControl;

        public RecognizerConsumer(IBusControl busControl)
        {
            
            this.busControl = busControl;
        }

        public RecognizerConsumer(IRecognizer recognizer, IBusControl busControl, Action<MessageRecognize, string> action)
        {
            this.action = action;
            this.recognizer = recognizer;
            this.busControl = busControl;
        }



        public Task Consume(ConsumeContext<MessageRecognize> context)
        {
            MessageRecognize message = context.Message;
            return Task.Run(() =>
            {
                try
                {
                    
                    string number = string.Empty;
                    recognizer.OnStartRecognition(message.ImagePath, x => { number = x; });
                    action?.Invoke(message, number);

                    busControl.Publish(new MessageInfo() { Message = $"ECA System LPR -> Agent -> licensePlateNumber: {number}" });
                    busControl.Publish(new MessageTrailerNumber()
                    {
                        CameraId = message.CameraId,
                        CommandId = message.CommandId,
                        Status = "Ok",
                        TrailerPlateNumber = number,
                        Timestamp = DateTime.Now.ToString()
                    });
                }
                catch (Exception ex)
                {
                    busControl.Publish(new MessageInfo() { Message = $"Exception for RecognizerConsumer {ex.ToString()}" });
                }

            });
        }
    }
}
