using FastYolo.Model;
using FastYolo;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public static List<YoloItem> itemsYolo = new List<YoloItem>();
        private Thread _thread;
        MemoryStream ms;
        public Form1()
        {
            InitializeComponent();
            _thread = new Thread(DetectObjects);

        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            var port = int.Parse(ConfigurationManager.AppSettings.Get("port"));
            var client = new UdpClient(port);
            _thread.Start();
            while (true)
            {
                var data = await client.ReceiveAsync();
                using (ms = new MemoryStream(data.Buffer))
                {

                    //   Image finalImage = new Bitmap(ms); 172.30.115.4
                    Image finalImage = new Bitmap(ms);
                    if (itemsYolo.Count>0)
                    {
                        Graphics graph = Graphics.FromImage(finalImage);
                        Font font = new Font("Consolas", 10, FontStyle.Bold);
                        SolidBrush brush = new SolidBrush(System.Drawing.Color.Red);

                        foreach (YoloItem item in itemsYolo)
                        {
                            Point rectPoint = new Point(item.X, item.Y);
                            Size rectSize = new Size(item.Width, item.Height);
                            Rectangle rect = new Rectangle(rectPoint, rectSize);
                            Pen pen = new Pen(System.Drawing.Color.Red, 2);
                            graph.DrawRectangle(pen, rect);
                            graph.DrawString(item.Type, font, brush, rectPoint);
                        }
                    }
                    
                    pictureBox1.Image = finalImage;
                }
                Text = $"Bytes recieved: {data.Buffer.Length * sizeof(byte)}";
            }
        }
        private void DetectObjects()
        {
            while (true)
            {
                //Bitmap bmp = new Bitmap(pictureBox1.Image);
                //var ms = new MemoryStream();
                //bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                YoloWrapper yolo = new YoloWrapper("yolov3.cfg", "yolov3.weights", "coco.names");
                itemsYolo = yolo.Detect(ms.ToArray()).ToList<YoloItem>();
                Thread.Sleep(500);
            }
          
            
        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            MessageBox.Show(string.Join("\n", host.AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork)));
        }
    }
}