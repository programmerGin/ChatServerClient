using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream;
        string readData;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //Connect
        {
            clientSocket.Connect("127.0.0.1", 7000); //서버 포트번호
            readData = "Connect To server...";
            msg();

            serverStream = clientSocket.GetStream();
            byte[] outBytes = Encoding.ASCII.GetBytes(textBox1.Text + "$");
            serverStream.Write(outBytes, 0, outBytes.Length);
            serverStream.Flush();

            Thread clThread = new Thread(getMessage);
            clThread.Start();
        }

        private void getMessage()
        {
            byte[] inBytes = new byte[clientSocket.ReceiveBufferSize];
            string returnData;
            while(true)
            {
                
                serverStream.Read(inBytes, 0, clientSocket.ReceiveBufferSize);
                returnData = Encoding.ASCII.GetString(inBytes);
               // readData = returnData.Substring(0, returnData.IndexOf("$"));
                readData = " " + returnData;
                msg();
            }
        }
        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                textBox2.Text += Environment.NewLine + "<<" + readData;
        }

        private void button2_Click(object sender, EventArgs e) //Send
        {
            byte[] outBytes = Encoding.ASCII.GetBytes(textBox3.Text + "$");
            serverStream.Write(outBytes, 0, outBytes.Length);
            serverStream.Flush();

        }
    }
}
