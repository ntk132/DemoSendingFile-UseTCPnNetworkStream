using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        TCPModel server = new TCPModel();
        private String filename = "";
        bool isSending = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            button3.Enabled = false;
            textBox3.Text = Environment.CurrentDirectory;

            try
            {
                server.InitServer("127.0.100.6", "6060");

                Thread t = new Thread(SetConnection);
                t.Start();
            }
            catch
            {
                server.StopServer();
            }
        }

        private void SetConnection(object obj)
        {
            while (true)
            {
                if (server.AcceptConnection())
                {
                    textBox1.AppendText(server.remotEndPoint);

                    Thread t = new Thread(Listener);
                    t.Start(server.counter - 1);
                }
            }
        }

        private void Listener(object obj)
        {
            int index = (int)obj;
            String filename = "";
            while (true)
            {
                if (!isSending)
                {
                    // Get data
                    String dataIn = server.ReceiveData(obj);
                    
                    if (dataIn == "Start")
                    {
                        filename = server.ReceiveData(obj);

                        MessageBox.Show(filename);
                        isSending = true;
                        continue;
                    }
                }
                else
                {
                    if (server.ReceiveFile(textBox3.Text + @"\" + filename, index) == 1)
                    {
                        MessageBox.Show("Download successfully!");

                        isSending = false;
                    }
                    else
                    {
                        MessageBox.Show("Download Failed!");
                    }

                }
            }
        }

        // Load file
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;

                FileInfo f = new FileInfo(ofd.FileName);

                filename = f.Name;
            }
        }


        // Setting saving path
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }
        }


        // Sending file
        private void button3_Click(object sender, EventArgs e)
        {
            // Send the start signal
            server.SendDataToClient("Start", server.counter - 1);

            // Send the file
            server.SendDataToClient(filename, server.counter - 1);

            // service only client
            server.SendFile(textBox2.Text, 0);

            // Send the end signal or the last connected
            server.SendDataToClient("End", server.counter - 1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox2.Text == null)
                button3.Enabled = false;
            else
                button3.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.StopServer();
        }
    }
}
