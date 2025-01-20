using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;



namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private string fileName = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(fileName);
            }
            else
            {
                MessageBox.Show("Картинка не выбрана", "Выберите картинку", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {


            YoloWrapper yolo = new YoloWrapper("yolov3.cfg", "yolov3.weights", "coco.names");
            MemoryStream memoryStream = new MemoryStream();
            pictureBox1.Image.Save(memoryStream, ImageFormat.Jpeg);

            List<YoloItem> items = yolo.Detect(memoryStream.ToArray()).ToList<YoloItem>();
            Image finalImage = pictureBox1.Image;

            Graphics graph = Graphics.FromImage(finalImage);


            Font font = new Font("Consolas", 10, FontStyle.Bold);

            SolidBrush brush = new SolidBrush(Color.Red);

            foreach (YoloItem item in items)
            {
                Point rectPoint = new Point(item.X, item.Y);
                Size rectSize = new Size(item.Width, item.Height);
                Rectangle rect = new Rectangle(rectPoint, rectSize);
                Pen pen = new Pen(Color.Red, 2);
                graph.DrawRectangle(pen, rect);
                graph.DrawString(item.Type, font, brush, rectPoint);
            }
            pictureBox1.Image = finalImage;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var port = int.Parse(ConfigurationManager.AppSettings.Get("port"));
            var client = new UdpClient(port);
            while (true)
            {
                var data = await client.ReceiveAsync();
                using (var ms = new MemoryStream(data.Buffer))
                {

                    YoloWrapper yolo = new YoloWrapper("yolov3.cfg", "yolov3.weights", "coco.names");
                    List<YoloItem> items = yolo.Detect(ms.ToArray()).ToList<YoloItem>();
                    Image finalImage = new Bitmap(ms);
                    Graphics graph = Graphics.FromImage(finalImage);
                    Font font = new Font("Consolas", 10, FontStyle.Bold);
                    SolidBrush brush = new SolidBrush(Color.Red);

                    foreach (YoloItem item in items)
                    {
                        Point rectPoint = new Point(item.X, item.Y);
                        Size rectSize = new Size(item.Width, item.Height);
                        Rectangle rect = new Rectangle(rectPoint, rectSize);
                        Pen pen = new Pen(Color.Red, 2);
                        graph.DrawRectangle(pen, rect);
                        graph.DrawString(item.Type, font, brush, rectPoint);
                    }
                    pictureBox1.Image = finalImage;
                }
                Text = $"Bytes recieved: {data.Buffer.Length * sizeof(byte)}";
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            MessageBox.Show(string.Join("\n", host.AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork)));
        }
    }
}
