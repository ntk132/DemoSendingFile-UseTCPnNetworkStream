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

namespace Client
{
    public partial class Form1 : Form
    {
        TCPModel client = new TCPModel();

        //private String pathBook = @"D:\VS 2015\SendingFile\Client\Client\Download\";
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

            /**** Auto create the connection to server ****/
            // Try to connect to the server
            try
            {
                // get ip address
                String ip = "127.0.100.6";
                // get port number
                int port = 6060;

                client.ConnectToServer(ip, port);

                Thread t = new Thread(Listener);
                t.Start();
            }
            catch
            {
                // if connecting is failed the turn on the offline mode
                MessageBox.Show("You are offline now!");

                return;
            }
        }

        // this methos always listen any message from server
        private void Listener(object obj)
        {
            String filename = "";
            while (true)
            {
                if (!isSending)
                {
                    // Get data
                    String dataIn = client.Receive_Data(obj);

                    if (dataIn == "Start")
                    {
                        filename = client.Receive_Data(obj);

                        MessageBox.Show(filename);
                        isSending = true;
                        continue;
                    }
                }
                else
                {
                    if (client.ReceiveFile(textBox3.Text + @"\" + filename) == 1)
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
            //byte[] bytes = File.ReadAllBytes(textBox2.Text);

            // service only client
            //client.Sending(bytes);

            // Send the start signal
            client.Send_Data("Start");

            // Send the file
            client.Send_Data(filename);

            // service only client
            client.SendFile(textBox2.Text);

            // Send the end signal
            client.Send_Data("End");
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
            client.Disconnect();
        }
    }
}
