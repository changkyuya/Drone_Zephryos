using System;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace WindowsFormsApplication1
{

    public class  Comport
    {
        private SerialPort _serialPort;
        public bool IsOpen = false;
        public int seldev;
        public int num;
        public EventHandler StatusChanged;
        public delegate void GetSensorCTRL(string data);
        public event GetSensorCTRL getsensorctrl;
        public delegate void GetCMD(byte[] data);
        public event GetCMD getcmd;
        public bool flag_threadterminal = false;
        public Thread pthreadterminal;
        private const int NUM_BYTE_READ =16; // pitch, roll ,yaw
        byte[] readBuffer1 = new byte[NUM_BYTE_READ];
         public Comport()
        {
            _serialPort = new SerialPort();
            pthreadterminal = new Thread(DataReceivedEvent);
            seldev = 0;
        }
         ~Comport()
         {
             try
             {
                 if (pthreadterminal != null)
                     pthreadterminal = null;
                 if (_serialPort != null)
                     _serialPort = null;
             }
             catch { }
         }
         public void Close()
         {
             if (IsOpen)
             {
                 if (flag_threadterminal)
                 {
                     flag_threadterminal = false;
                     IsOpen = false;
                     pthreadterminal.Join();
                     pthreadterminal.Abort();
                 }
                 _serialPort.Close();
             }
         }
        public void DataReceivedEvent()
        {
            byte[] readBuffer = new byte[NUM_BYTE_READ];
            while (flag_threadterminal)
            {

                /*  int cntByte = _serialPort.BytesToRead;

                  if (cntByte > NUM_BYTE_READ - 1)
                  {

                      int count = _serialPort.Read(readBuffer, 0, NUM_BYTE_READ);

                     if (readBuffer[0] == 255 & readBuffer[1] == 255 && readBuffer[14] == 254)
                      {

                              getsensorctrl(readBuffer);

                      }
                      _serialPort.DiscardInBuffer();
                      Array.Clear(readBuffer, 0, readBuffer.Length);

                  }*/
                try
                {
                    String data = _serialPort.ReadLine();
                    getsensorctrl(data);
                }
                catch { }

            }
        }

        public bool Open(String COM, Int32 baudrate,int device)
        {
            bool val = true;
            try
            {
                _serialPort.PortName = COM;
                _serialPort.BaudRate = baudrate;
                _serialPort.Encoding = Encoding.Default;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.ReadTimeout = 5000;
                //   the read/write timeouts
                seldev = device;
                _serialPort.Open();
                pthreadterminal.Start();
                flag_threadterminal = true;
                IsOpen = true;
            }
            catch (IOException ex)
            {
               // StatusChanged(String.Format("{0} does not exist", Settings.Port.PortName));
                MessageBox.Show("Error at IOException:" + ex.ToString());
                val = false;
            }
            catch (UnauthorizedAccessException exAuth)
            {
               // StatusChanged(String.Format("{0} already in use", Settings.Port.PortName));
                MessageBox.Show("Error at UnauthorizedAccessException:" + exAuth.ToString());
                val = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at Connected PORT:" + ex.ToString());
                val = false;
            }
            return val;
            // Update the status

        }


        /// <summary> Get the status of the serial port. </summary>
        public void Send(string data)
        {
            if (IsOpen)
            {
                string lineEnding = "";
                _serialPort.Write(data + lineEnding);
            }
        }
        public void Send_Char(char ch)
        {
            if (IsOpen)
            {
                char []wrt=new char[1];
                wrt[0] =ch;
                _serialPort.Write(wrt,0,1);
            }
        }

        public void Send_Byte(byte[] data)
        {
            
                    _serialPort.Write(data, 0, data.Length);
         }
        public void SendCMDpacket_text(String str)
        {
            try
            {
                if (IsOpen)
                {
                    _serialPort.Write(str);
                }
            }
            catch { }
        }

    }
}
