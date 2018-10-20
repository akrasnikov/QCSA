using SimpleLPR3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace ECAS.Toolkit.Recognizer
{
    public class Recognizer : IRecognizer
    {

        ISimpleLPR simpleLPR;
        IProcessor processor;
        
        public event EventHandler EventHandlerIsRecognized;


        public void OnStartRecognition(string imagePath, Action<string> actionIsRecognized)
        {
            var location = Assembly.GetEntryAssembly().Location;
            string productKeyPath = Path.GetDirectoryName(location) + "\\key.xml";

            Console.WriteLine("productKeyPath: " + productKeyPath);

            EngineSetupParms cpuUse;
            cpuUse.cudaDeviceId = -1;
            simpleLPR = SimpleLPR.Setup(cpuUse);

            if (productKeyPath == null) throw new ArgumentNullException("License", "license file for LPR not found");
            simpleLPR.set_productKey(productKeyPath);

            Console.WriteLine("license file for LPR = OK " );
            if (processor == null)
            {
                processor = simpleLPR.createProcessor();
            }
            string country = "numerical_6-digit";
            for (uint i = 0; i < simpleLPR.numSupportedCountries; ++i)
            {
                string sCountry = simpleLPR.get_countryCode(i);
                simpleLPR.set_countryWeight(sCountry, sCountry == country ? 1.0f : 0.0f);
            }
            simpleLPR.realizeCountryWeights();

            if (imagePath == null) throw new ArgumentNullException("Image", "no image found");

            List<Candidate> currentCandidate = null;
            currentCandidate = processor.analyze(imagePath);

            if (currentCandidate != null && currentCandidate.Count > 0)
            {
                CountryMatch bestMatch;
                bestMatch.confidence = -1.0f;
                bestMatch.text = "";

                for (int i = 0; i < currentCandidate.Count; ++i)
                {
                    if (currentCandidate[i].matches.Count > 1)
                    {
                        if (currentCandidate[i].matches[0].confidence > bestMatch.confidence)
                            bestMatch = currentCandidate[i].matches[0];
                    }
                }
                if (bestMatch.confidence > 0)
                {
                    actionIsRecognized(bestMatch.text);
                }
            }

        }

        public void Dispose()
        {
            simpleLPR.Dispose();
            processor.Dispose();
            EventHandlerIsRecognized = null;
        }

        public string OnStartRecognition(string imagePath)
        {
            string number = string.Empty;
            OnStartRecognition(imagePath, x => { number = x; });
            EventHandlerIsRecognized?.Invoke(number, null);
            return number;
        }
    }
}
