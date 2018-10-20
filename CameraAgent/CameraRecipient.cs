using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using System.Reactive.Linq;

namespace CameraAgent
{
    public class CameraRecipient : IJob
    {

        public Bitmap loadedBitmap;

        public async Task Execute(IJobExecutionContext context)
        {
            Task[] myTasks = new Task[2]
              {
                      Task.Run(() => {RequestFrame("admin", "admin123", "192.168.1.1", null);}),
                      Task.Run(() => {RequestFrame("admin", "admin123", "192.168.1.1", null);})                     
              };
            await  Task.WhenAll(myTasks);
        }
        private void RequestFrame(string cameraLogin, string cameraPassword, string cameraAdress, string imagePath)
        {
            string cameraUrl = $"http://{cameraLogin}:{cameraPassword}@{cameraAdress}/ISAPI/Streaming/channels/101/picture?snapShotImageType=JPEG";/*TODO*/

            var request = WebRequest.Create(cameraUrl);
            request.Credentials = new NetworkCredential(cameraLogin, cameraPassword);
            request.Proxy = null;
            request.BeginGetResponse(new AsyncCallback(SaveImage), request);
        }

        void SaveImage(IAsyncResult result)
        {
            using (HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (Bitmap frame = new Bitmap(responseStream))
                    {
                        if (frame != null)
                        {
                            loadedBitmap = (Bitmap)frame.Clone();
                            string imagePath = Directory.GetCurrentDirectory() + @"\1.jpg"; /*TODO*/
                            loadedBitmap.Save(imagePath, ImageFormat.Jpeg);
                        }
                    }
                }
            }
        }


        public async void ParallelExecutionTest()
        {
            var o = Observable.CombineLatest(
                Observable.Start(() => { return "Result A"; }),
                Observable.Start(() => { return "Result B"; })
            ).Finally(() => Console.WriteLine("Done!"));

            await o.FirstAsync();
            //foreach (string r in await o.FirstAsync())
            //    Console.WriteLine(r);
        }
    }
}
