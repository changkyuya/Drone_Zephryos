using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WFA_LM_test1
{
    class VideoStreaming
    {
        public byte[] bytes = new byte[1024 * 1024];
        public static String _IpAddress = "192.168.0.33";
        public static int _port = 8080;
        public VideoStreaming(String IpAddress, int port)
        {
            _IpAddress = IpAddress;
            _port = port;
        }
        public VideoStreaming()
        {
        }
        public Bitmap BStreaming()
        {
            int count = 0;
            Array.Clear(bytes, 0, bytes.Length);
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            try
            {
                clientSocket.Connect("192.168.0.33", _port);
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("SREAMING\n");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                byte[] inStream = new byte[10];
                serverStream.Read(inStream, 0, 10);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                while (count < Convert.ToInt32(returndata))
                {
                    count += serverStream.Read(bytes, count, Convert.ToInt32(returndata) - (count));
                }
                MemoryStream ms = new MemoryStream(bytes);
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(ms);
                clientSocket.Close();

                return b;
            }
            catch (Exception)
            {
                if (clientSocket != null)
                    if (clientSocket.Connected)
                        clientSocket.Close();
            }
            return null;


        }
        public Image IStreaming()
        {
            int count = 0;
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            try
            {
                clientSocket.Connect(_IpAddress, _port);
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("SREAMINGwn");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                byte[] inStream = new byte[10];
                serverStream.Read(inStream, 0, 10);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);

                while (count < Convert.ToInt32(returndata))
                {
                    count += serverStream.Read(bytes, count, Convert.ToInt32(returndata) - count);
                }
                Image image = Image.FromStream(new MemoryStream(bytes));
                Array.Clear(bytes, 0, bytes.Length);
                clientSocket.Close();
                return image;
            }
            catch (Exception)
            {
                if (clientSocket != null)
                    if (clientSocket.Connected)
                        clientSocket.Close();
            }
            return null;


        }
    }
}
