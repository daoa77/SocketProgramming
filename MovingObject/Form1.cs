using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MovingObject
{
    public partial class Form1 : Form
    {
        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;

        TcpListener server;
        List<TcpClient> clients = new List<TcpClient>();
        NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 50;
            timer1.Enabled = true;

            // Jalankan server di thread terpisah
            Thread serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000); // listen di port 5000
            server.Start();

            while (true)
            {
                var client = server.AcceptTcpClient();
                lock (clients)
                {
                    clients.Add(client);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            back();
            rect.X += slide;
            Invalidate();

            // Kirim posisi ke semua client
            string msg = $"{rect.X},{rect.Y}\n";
            byte[] data = Encoding.UTF8.GetBytes(msg);

            lock (clients)
            {
                foreach (var c in clients.ToList())
                {
                    try
                    {
                        c.GetStream().Write(data, 0, data.Length);
                    }
                    catch
                    {
                        clients.Remove(c); // hapus client yang putus
                    }
                }
            }
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}
