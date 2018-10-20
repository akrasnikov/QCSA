using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECAS.Toolkit.Camera
{
    public class CameraApi
    {
        public async Task<Bitmap> GetFotoFromCamera(string cameraUrl, string cameraLogin, string cameraPassword, string fotoPath)
        {
            Bitmap fotoBitmap = null;
            var request = WebRequest.Create(cameraUrl);
            request.Credentials = new NetworkCredential(cameraLogin, cameraPassword);
            request.Proxy = null;

            using (WebResponse response = await request.GetResponseAsync())
            {
                if (response == null) throw new ArgumentNullException("Не получилось установить соедениние с камерой");
                using (Stream stream = response.GetResponseStream())
                {
                    fotoBitmap = new Bitmap(stream);
                    if (fotoBitmap == null) throw new ArgumentNullException("Не получилось скачать фото");
                    fotoBitmap.Save(fotoPath, ImageFormat.Jpeg);
                }
            }
            return fotoBitmap;
        }
    }
}
