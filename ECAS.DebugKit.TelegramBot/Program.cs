using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ECAS.DebugKit.TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient("683303941:AAG6fKpEl7a_Wyh1exIp9aoiOCLw-3hq2z4");
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            Console.ReadLine();
        }
    }
}
