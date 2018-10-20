using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Toolkit.Recognizer
{
    public interface IRecognizer:IDisposable
    {
        void OnStartRecognition(string imagePath, Action<string> action);
        string OnStartRecognition(string imagePath);
    }
}
