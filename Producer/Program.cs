using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Producer
{
    class Program
    {
        private static IPEndPoint consumerEndPoint;
        private static UdpClient udpClient = new UdpClient();
        static void Main(string[] args)
        {
            var consumrIp = ConfigurationManager.AppSettings.Get("consumerIp");
            var consumrPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            consumerEndPoint = new IPEndPoint(IPAddress.Parse(consumrIp), consumrPort);


            Console.WriteLine($"consumer: {consumerEndPoint}");

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
            Console.ReadKey();

        }

        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame,800,600);

            try
            {
                using (var ms=new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpClient.Send(bytes, bytes.Length, consumerEndPoint);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
