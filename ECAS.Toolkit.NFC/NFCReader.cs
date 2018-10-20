using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PCSC;
using PCSC.Iso7816;
using PCSC.Exceptions;
using PCSC.Monitoring;
using PCSC.Utils;


namespace ECAS.Toolkit.NFC
{
    public class NFCReader
    {

        public delegate void ActionCardHandler(string eventName, SCRState state, string readerName);
        public delegate void StatusCardHandler(SCRState newstate, SCRState laststate, string readername);
        public delegate void ExceptionCardHandler(string scardError);

        public event ActionCardHandler EventCardAction;
        public event StatusCardHandler EventCardStatus;
        public event ExceptionCardHandler EventCardException;



        private const byte MSB = 0x00;
        private byte LSB = 0x08;

        MifareCard card;

        public ISCardMonitor Monitor { get; }

        public NFCReader(byte block = 0x08)
        {

            LSB = block;
            var contextFactory = ContextFactory.Instance;
            context = contextFactory.Establish(SCardScope.System);

            var monitorFactory = MonitorFactory.Instance;
            Monitor = monitorFactory.Create(SCardScope.System);

            var readerNames = context.GetReaders();

            AttachToAllEvents(Monitor);

            if (NoReaderAvailable(readerNames))
                new Exception("You need at least one reader in order to run this application.");

            readerName = ChooseReader(readerNames);
            if (readerName == null)
                new Exception("The reader is not correctly selected.");

            Monitor.Start(readerName);
        }



        ISCardContext context;
        string readerName;
        private void AccessCard()
        {

            var isoReader = new IsoReader(context: context, readerName: readerName, mode: SCardShareMode.Shared,
                                                   protocol: SCardProtocol.Any, releaseContextOnDispose: false);

            card = new MifareCard(isoReader);
            var loadKeySuccessful = card.LoadKey(KeyStructure.VolatileMemory, 0x00, // first key slot
                                                  new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } /* key */ );

            if (!loadKeySuccessful)
                throw new Exception("LOAD KEY failed.");

            var authSuccessful = card.Authenticate(MSB, LSB, KeyType.KeyA, 0x00);
            if (!authSuccessful)
                throw new Exception("AUTHENTICATE failed.");
        }

        private void AttachToAllEvents(ISCardMonitor monitor)
        {
            // Point the callback function(s) to the anonymous & static defined methods below.
            monitor.CardInserted += (sender, args) => CardEvent("CardInserted", args);
            monitor.CardRemoved += (sender, args) => CardEvent("CardRemoved", args);
            monitor.Initialized += (sender, args) => CardEvent("Initialized", args);
            monitor.StatusChanged += StatusChanged;
            monitor.MonitorException += MonitorException;
        }

        private void CardEvent(string eventName, CardStatusEventArgs unknown)
        {
            EventCardAction?.Invoke(eventName, unknown.State, unknown.ReaderName);
            Debug.WriteLine($">> {eventName} Event for reader: {unknown.ReaderName}", "Card Event");
            Debug.WriteLine($"State: {unknown.State}\n", "Card Event");
        }

        private void StatusChanged(object sender, StatusChangeEventArgs args)
        {
            EventCardStatus?.Invoke(args.NewState, args.LastState, args.ReaderName);
            Debug.WriteLine($">> StatusChanged Event for reader: {args.ReaderName}", "Card Status");
            Debug.WriteLine($"Last state: {args.LastState}\nNew state: {args.NewState}\n", "Card Status");
        }

        private void MonitorException(object sender, PCSCException ex)
        {
            EventCardException?.Invoke(SCardHelper.StringifyError(ex.SCardError));
            Debug.WriteLine($"Monitor exited due an error:", "MonitorException");
            Debug.WriteLine(SCardHelper.StringifyError(ex.SCardError), "MonitorException");
        }

        public byte[] ConvertToBytes(string s)
        {
            try
            {
                if (s.Length < 10)
                {
                    return BitConverter.GetBytes(Convert.ToInt32(s));
                }

                else
                {
                    return Enumerable.Range(0, s.Length / 2).Select(x =>
                        Convert.ToByte(s.Substring(x * 2, 2), 16)).ToArray();
                }

            }
            catch { return new byte[16]; }
        }


        public string GetRancherId(int length)
        {
            AccessCard();

            byte[] sector = card.ReadBinary(MSB, LSB, 16);
            byte[] rancherid = card.ReadBinary(MSB, LSB, 16);
            Array.Resize(ref rancherid, length);
            return BitConverter.ToInt32(rancherid, 0).ToString();
        }

        public string GetRancherId()
        {
            AccessCard();

            byte[] sector = card.ReadBinary(MSB, LSB, 16);
            byte[] rancherid = card.ReadBinary(MSB, LSB, 16);
            Debug.WriteLine($"Result: {(rancherid != null ? BitConverter.ToString(rancherid) : null)}", "Get Rancher ID");
            return (rancherid.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2)).ToLower();
        }
        public string GetRancherString(byte customLSB )
        {
            AccessCard();

            byte[] sector = card.ReadBinary(MSB, customLSB, 16);
            byte[] rancherid = card.ReadBinary(MSB, customLSB, 16);
            Debug.WriteLine($"Result: {(rancherid != null ? System.Text.Encoding.ASCII.GetString(rancherid) : null)}", "Get Rancher String");
            return System.Text.Encoding.ASCII.GetString(rancherid);
        }
        public string UpdateRancherId(string number)
        {
            var block = new byte[16];
            var hexId = ConvertToBytes(number);

            //array.ToList().ForEach(x =>
            //{
            //    int index = array.ToList().IndexOf(x);
            //    if (index < 4) x = id[index];
            //    else x = 0;
            //});

            var blockEnumerator = block.GetEnumerator();
            for (var i = 0; blockEnumerator.MoveNext() == true; i++)
            {
                try
                {
                    block[i] = hexId[i];
                }
                catch
                { }
            }

            return UpdateRancherId(block);
        }

        public string UpdateRancherId(byte[] block)
        {
            AccessCard();
            var updateSuccessful = card.UpdateBinary(MSB, LSB, block);
            if (!updateSuccessful) throw new Exception("UPDATE BINARY failed.");

            var result = card.ReadBinary(MSB, LSB, 16);
            Debug.WriteLine($"Result (after BINARY UPDATE): {(result != null ? BitConverter.ToString(result) : null)}", "Update Rancher ID");
            return BitConverter.ToString(result);
        }
        public string UpdateRancherId(byte[] block, byte customLSB)
        {
            AccessCard();
            var updateSuccessful = card.UpdateBinary(MSB, customLSB, block);
            if (!updateSuccessful) throw new Exception("UPDATE BINARY failed.");

            var result = card.ReadBinary(MSB, customLSB, 16);
            Debug.WriteLine($"Result (after BINARY UPDATE): {(result != null ? System.Text.Encoding.ASCII.GetString(result) : null)}", "Update Rancher ID");
            return System.Text.Encoding.ASCII.GetString(result);
        }
        /// <summary>
        /// Asks the user to select a smartcard reader containing the Mifare chip
        /// </summary>
        /// <param name="readerNames">Collection of available smartcard readers</param>
        /// <returns>The selected reader name or <c>null</c> if none</returns>
        private string ChooseReader(IList<string> readerNames)
        {
            // Show available readers.
            Debug.WriteLine("Available readers: ", "ChooseReader"); 
            for (var i = 0; i < readerNames.Count; i++)
                Debug.WriteLine($"[{i}] {readerNames[i]}");
            var choice = 0;
            var line = "0"; /*select default reader*/

            if (int.TryParse(line, out choice) && (choice >= 0) && (choice <= readerNames.Count))
                return readerNames[choice];

            return null;
        }

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }
    }
}


//private  void Init()
//{
//    var contextFactory = ContextFactory.Instance;
//    using (var context = contextFactory.Establish(SCardScope.System))
//    {
//        var readerNames = context.GetReaders();
//        if (NoReaderAvailable(readerNames))
//            new Exception("You need at least one reader in order to run this application.");

//        var readerName = ChooseReader(readerNames);
//        if (readerName == null)
//            new Exception("The reader is not correctly selected.");

//        using (var isoReader = new IsoReader( context: context, readerName: readerName,  mode: SCardShareMode.Shared,
//                                              protocol: SCardProtocol.Any,    releaseContextOnDispose: false))
//        {
//            card = new MifareCard(isoReader);
//            var loadKeySuccessful = card.LoadKey( KeyStructure.VolatileMemory,   0x00, // first key slot
//                                                  new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } /* key */ );

//            if (!loadKeySuccessful)
//                throw new Exception("LOAD KEY failed.");

//            var authSuccessful = card.Authenticate(MSB, LSB, KeyType.KeyA, 0x00);
//            if (!authSuccessful)
//                throw new Exception("AUTHENTICATE failed.");

//        }
//    }
//}
