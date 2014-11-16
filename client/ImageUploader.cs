using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.IO;

namespace Screencapture
{
    public class ImageUploader
    {
        public string Address
        {
            private set;
            get;
        }

        public event UploadDataCompletedEventHandler Uploaded;

        private List<WebClient> Clients = new List<WebClient>();

        public ImageUploader(string address)
        {
            Address = address;
        }

        public void Upload(Bitmap image)
        {
            WebClient client = new WebClient();
            client.UploadDataCompleted += delegate(object sender, UploadDataCompletedEventArgs e)
            {
                lock (Clients)
                {
                    Clients.Remove((WebClient)sender);
                } 

                UploadDataCompletedEventHandler eh = Uploaded;

                if (eh != null)
                {
                    eh(this, e);
                }
            };

            lock (Clients)
            {
                Clients.Add(client);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                client.UploadDataAsync(new Uri(Address), stream.ToArray());
            }
        }
    }
}
