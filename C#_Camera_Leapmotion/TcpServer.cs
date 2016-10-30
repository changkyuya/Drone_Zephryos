using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace WFA_LM_test1
{
    class TcpServer
    {
        Socket s;
        Thread pthreadDataReceive;
        private NetworkStream networkStream;
        StreamReader streamReader;
        int port = 8080;
        public delegate void StreamData(String message);

        public bool flag_connect = false;
        public TcpServer()
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
                try
                {
                    String message = streamReader.ReadLine();
                }
                catch { }
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
