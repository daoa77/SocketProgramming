using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace MovingObjectClient
{
    public partial class FormClient : Form
    {
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        Pen red = new Pen(Color.Red);

        TcpClient client;

        public FormClient()
        {
            InitializeComponent();
            this.Paint += FormClient_Paint;  // hubungkan event Paint
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000);  // port sama dengan server
                var stream = client.GetStream();
                var reader = new StreamReader(stream, Encoding.UTF8);

                // langsung mulai loop menerima posisi
                _ = ReceivePositions(reader);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal konek ke server: " + ex.Message);
            }
        }

        private async Task ReceivePositions(StreamReader reader)
        {
            try
            {
                while (true)
                {
                    string line = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int x) &&
                            int.TryParse(parts[1], out int y))
                        {
                            rect.X = x;
                            rect.Y = y;

                            // repaint di UI thread
                            this.Invoke((MethodInvoker)(() => this.Invalidate()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke server putus: " + ex.Message);
            }
        }

        private void FormClient_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(red, rect);
            e.Graphics.FillRectangle(fillBlue, rect);
        }
    }
}
