using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFS210.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        PacketSerializer ps;
        public Form1()
        {
            InitializeComponent();
            ps = new PacketSerializer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            WFS210.IO.Message msg = new WFS210.IO.Message(Convert.ToByte(txtCMD.Text,16));
            var payload = StringToByteArray(txtPayload.Text);
            if(payload.Length == 0 && numLength.Value > 6)
            {
                payload = new byte[((int)numLength.Value - 6)];
                msg.Payload = payload;
            }
            FakeStream stream = new FakeStream(new MemoryStream(),new ComplementChecksum());
            ps.Serialize(stream, msg);
            var i = stream.Length;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            foreach (var b in buffer)
            {
                var s = Convert.ToString(b, 16);
                txtData.Text += s;
            }
        }
    }
}
