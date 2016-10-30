using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WindowsFormsApplication1
{
    class TcpClient
    {
        Socket s;
        Thread pthreadDataReceive;
        private NetworkStream networkStream;
        public delegate void GetSensorCTRL_TCP(string data);
        public event GetSensorCTRL_TCP getsensorctrl_TCP;
        StreamReader streamReader;
        int port = 30000;
        public delegate void StreamData(String message);

        public bool flag_connect=false;
        public TcpClient()
        {
        }
        public void CreateServer(int _port)
        {
            port = _port;
            pthreadDataReceive = new Thread(DataReceiveEvent);
                pthreadDataReceive.Start();
      
        }
        public void DataReceiveEvent()
        {

            try
            {
                TcpListener myList = new TcpListener(IPAddress.Any, port);
                myList.Start();
                s = myList.AcceptSocket();
                networkStream = new NetworkStream(s);
                streamReader = new StreamReader(networkStream);
                flag_connect = true;
            }
            catch
            {
                return;
            }
            while (flag_connect)
            {
                String message = streamReader.ReadLine();
                getsensorctrl_TCP(message);
                Console.WriteLine(message);
            }
        }
        public void write(byte[] data)
        {
            if (networkStream != null)
            {
                networkStream.Write(data, 0, data.Length);
            }
            
        }
    }
}
