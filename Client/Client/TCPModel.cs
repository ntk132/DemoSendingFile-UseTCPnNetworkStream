using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class TCPModel
    {
        private TcpClient tcp;
        private Stream stm;
        //private NetworkStream networkStream;
        private byte[] dataIn;
        private byte[] dataOut;

        public void ConnectToServer(String ip, int port)
        {
            // Create client
            tcp = new TcpClient();
            
            // start connection
            tcp.Connect(ip, port);
            // set stream for this client
            stm = tcp.GetStream();
            //networkStream = tcp.GetStream();
        }

        /******** Receive data ********/
        public String Receive_Data(object obj)
        {
            try
            {
                String message = null;
                dataIn = new byte[100];

                // get message from server
                int k = stm.Read(dataIn, 0, 100);

                // fill each character of message
                for (int i = 0; i < k; i++)
                    message += Convert.ToChar(dataIn[i]);

                /**** TO DO ****/
                return message;
            }
            catch
            {
                return "";
            }
        }

        /******** Send data ********/
        public bool Send_Data(String str)
        {
            try
            {
                // Create buffer
                dataOut = new byte[100];

                // Encode the message to byte[]
                ASCIIEncoding asen = new ASCIIEncoding();
                dataOut = asen.GetBytes(str);

                // Send the request to server
                stm.Write(dataOut, 0, dataOut.Length);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public void Sending(byte[] output)
        {
            try
            {
                stm.Write(output, 0, output.Length);
            }
            catch
            {

            }
        }

        public void SendFile(String pathFile)
        {
            byte[] outFile = File.ReadAllBytes(pathFile);
            /*
            using (NetworkStream ns = tcp.GetStream())
            {
                //ns.Write(outFile, 0, outFile.Length);
                //ns.Flush();                
            }
            */

            stm.Write(outFile, 0, outFile.Length);
            stm.Flush();

        }

        public int ReceiveFile(String savePath)
        {
            int thisRead = 0;
            int blockSize = 1024;
            Byte[] dataByte = new Byte[blockSize];

            var ms = new MemoryStream();

            using (NetworkStream ns = tcp.GetStream())
            {
                try
                {
                    while (true)
                    {
                        if (!ns.DataAvailable)
                            break;

                        thisRead = ns.Read(dataByte, 0, blockSize);
                        ms.Write(dataByte, 0, thisRead);
                    }

                    File.WriteAllBytes(savePath, ms.ToArray());

                    ns.Close();

                    return 1;
                }
                catch
                {
                    ns.Close();

                    return -1;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                stm.Close();
                tcp.Close();
            }
            catch
            {
                return;
            }
        }
    }
}
