using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECAS.DebugKit.Comport
{
    class Program
    {
        public static void DataComportIsAcepted(SerialPort sp, string a)
        {
            try
            {
                string buffer;
                Console.WriteLine($"comport.BytesToRead: {sp.BytesToRead}");
                buffer = sp.ReadTo("kg");
                Console.WriteLine($"buffer:{buffer}");
                sp.DiscardInBuffer();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException {ex.ToString()}");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"TimeoutException {ex.ToString()}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException {ex.ToString()}");
            }

        }
        //public static event EventHandler DataComportIsAcepted;
        static void Main(string[] args)
        {
            string txt = "=000.100(kg)";


            //buffer = sp.ReadTo("(kg)");
            ////string pattern = ".*?([+-]?\\d*\\.\\d+)(?![-+0-9\\.])";
            //string pattern = ".*?(\\d+)";	// Integer Number 1
            //string re1 = ".*?"; // Non-greedy match on filler
            string re2 = ".*?([+-]?\\d*\\.\\d+)(?![-+0-9\\.])";    // Float 1

            Regex r = new Regex(re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                String float1 = m.Groups[1].ToString();
                Console.Write("(" + float1.ToString() + ")" + "\n");
            }
            Console.ReadLine();
            return;

            var comport = new SerialPort();
            comport.PortName = "COM4";
            comport.BaudRate = 9600;
            comport.Parity = Parity.None;
            comport.StopBits = StopBits.One;
            comport.DataBits = 8;
            comport.Handshake = Handshake.None;

            if (!comport.IsOpen) comport.Open();
            try
            {
                Observable
                  .Interval(TimeSpan.FromMilliseconds(50))
                  .Subscribe(element =>
                  {
                      if (comport.IsOpen)
                          if (comport.BytesToRead > 0)
                              DataComportIsAcepted(comport, null);
                  });
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
            comport.Close();
        }
    }
}
