using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.Reflection;
using System.IO.Ports;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;

namespace WindowsFormsApplication1

{
    public partial class Zephyros : Form
    {

        PerformanceCounter[] _pCounter = null;


        static IPAddress ipAd = IPAddress.Parse("192.168.0.13");
        TcpListener myList = new TcpListener(ipAd, 8080);

        const byte CMD_THROTTLE = 0x30;
        const byte CMD_CTRL = 0x31;
        const byte CMD_REST = 0x0F;

        const byte PARAM_pitch = 0x0A;
        const byte PARAM_roll = 0x0B;
        const byte PARAM_yaw = 0x0C;
        const byte work_takeoff = 0x0D;

        const byte CMD_SERVO = 0x13;
        const byte CMD_SERVO_RUN = 0x14;
        const byte CMD_SERVO_STOP = 0x15;


        public byte[] MotorDATA = new byte[15] { 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  254 };

        public byte[] PARAM_RPY = new byte[15] { 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 254 };

        public byte[] ServoDATA = new byte[15] { 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 254 };

        Comport pconnect;

        PID_CTRL pidCTRL_M1 = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M2 = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M3 = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M4 = new PID_CTRL(0, 1, 0, 0, 1);


        PID_CTRL pidCTRL_M1g = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M2g = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M3g = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_M4g = new PID_CTRL(0, 1, 0, 0, 1);

        PID_CTRL pidCTRL_yaw = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_roll = new PID_CTRL(0, 1, 0, 0, 1);
        PID_CTRL pidCTRL_pitch = new PID_CTRL(0, 1, 0, 0, 1);


        PID_CTRL pidCTRL_M = new PID_CTRL(2, 1, 0, 0, 1);

        int Motor1_throttle = 1120;
        int Motor2_throttle = 1120;
        int Motor3_throttle = 1120;
        int Motor4_throttle = 1120;
        int Motor5_throttle = 1120;
        int Motor6_throttle = 1120;

        int ServoA1_throttle = 1500;
        int ServoA2_throttle = 1500;
        int ServoC1_throttle = 1500;
        int ServoC2_throttle = 1500;
        

        int Motor1_ctrl = 0;
        int Motor2_ctrl = 0;
        int Motor3_ctrl = 0;
        int Motor4_ctrl = 0;

        int rotation_roll1 = 0;
        int rotation_pitch1 = 0;
        int rotation_roll2 = 0;
        int rotation_pitch2 = 0;
        int rotation_roll3 = 0;
        int rotation_pitch3 = 0;
        int rotation_roll4 = 0;
        int rotation_pitch4 = 0;
        
        bool flag_auto = false;

        bool rotation_auto = false;

        double Motor1_dist = 0;
        double Motor2_dist = 0;
        double Motor3_dist = 0;
        double Motor4_dist = 0;
        

        double kp, ki, kd, outmin, outmax, kpg, kig, kdg, outming, outmaxg, kpy, kiy, kdy, outminy, outmaxy;

        double kpr, kir, kdr, kpp, kip, kdp, outminr, outmaxr, outminp, outmaxp;

        byte CMD_PARAM_RPY = 0x00;
        
        public string[] savewrite = new string[67];



        TcpClient pTcpClient;




        public Zephyros()
        {
            InitializeComponent();
            timer1.Start();
            setup_PID();


            _pCounter = new PerformanceCounter[2];

            _pCounter[0] = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _pCounter[1] = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            Text = " Zephyros";

        }

       
        

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] port_arr = SerialPort.GetPortNames();

            foreach (string portno in port_arr)
            {
                txt_COM.Items.Add(portno);
            }

            load_Data();

        }


        public void key_press(object sender, KeyPressEventArgs e)
        {
            if (rotation_auto)
            {

                if (e.KeyChar == 117) // front
                {

                    rotation_pitch1 = rotation_pitch1 - 5;
                    rotation_roll1 = rotation_roll1 - 5;


                    rotation_pitch2 = rotation_pitch2 + 5;
                    rotation_roll2 = rotation_roll2 + 5;
                }

                if (e.KeyChar == 106) // back
                {
                    rotation_pitch2 = rotation_pitch2 - 5;
                    rotation_roll2 = rotation_roll2 - 5;


                    rotation_pitch1 = rotation_pitch1 + 5;
                    rotation_roll1 = rotation_roll1 + 5;

                }
                if (e.KeyChar == 104) // left
                {
                    rotation_pitch3 = rotation_pitch3 - 5;
                    rotation_roll3 = rotation_roll3 - 5;


                    rotation_pitch4 = rotation_pitch4 + 5;
                    rotation_roll4 = rotation_roll4 + 5;

                }
                if (e.KeyChar == 107) // right
                {
                    rotation_pitch4 = rotation_pitch4 - 5;
                    rotation_roll4 = rotation_roll4 - 5;


                    rotation_pitch3 = rotation_pitch3 + 5;
                    rotation_roll3 = rotation_roll3 + 5;

                }
            }
        }

        /*
        public void rotation()
        {
            Graphics gObject = panel3.CreateGraphics();
            Pen Pens = new Pen(Color.Red, 5);
            gObject.Clear(Color.LightGray);

            if (rotation_pitch1 < 0)
            {

                gObject.DrawLine(Pens, 165, 125, 165, 60);
                gObject.DrawLine(Pens, 140, 85, 165, 60);   // front
                gObject.DrawLine(Pens, 190, 85, 165, 60);
            }
            if (rotation_pitch2 < 0)
            {

                gObject.DrawLine(Pens, 165, 125, 165, 190);
                gObject.DrawLine(Pens, 140, 165, 165, 190);  // back
                gObject.DrawLine(Pens, 190, 165, 165, 190);
            }
            if (rotation_pitch3 < 0)
            {

                gObject.DrawLine(Pens, 100, 125, 165, 125);
                gObject.DrawLine(Pens, 100, 125, 120, 100);  // left
                gObject.DrawLine(Pens, 100, 125, 120, 150);
            }
            if (rotation_pitch4 < 0)
            {

                gObject.DrawLine(Pens, 165, 125, 230, 125);
                gObject.DrawLine(Pens, 230, 125, 210, 100);  // right
                gObject.DrawLine(Pens, 230, 125, 210, 150);
            }
        }
        */

           
        


        

        private void GetLH_BYTES(ref byte Hbyte, ref byte Lbyte, int value)
        {
            Hbyte = (byte)(value >> 8);
            Lbyte = (byte)(value & 0x00FF);
        }

        private void SetMotorData(int cmd, int M1, int M2, int M3, int M4, int M5, int M6)
        {
            byte highbyte = 0;
            byte lowbyte = 0;

            MotorDATA[2] = (byte)cmd;

            GetLH_BYTES(ref highbyte, ref lowbyte, M1);
            MotorDATA[4] = highbyte;
            MotorDATA[5] = lowbyte;


            GetLH_BYTES(ref highbyte, ref lowbyte, M2);
            MotorDATA[6] = highbyte;
            MotorDATA[7] = lowbyte;


            GetLH_BYTES(ref highbyte, ref lowbyte, M3);
            MotorDATA[8] = highbyte;
            MotorDATA[9] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, M4);
            MotorDATA[10] = highbyte;
            MotorDATA[11] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, M5);
            MotorDATA[12] = highbyte;
            MotorDATA[13] = lowbyte;

            
            // GetLH_BYTES(ref MotorDATA[12], ref MotorDATA[13], M6);




        }
        private void SetServoData(int servo_cmd, int S1, int S2, int S3, int S4)
        {
            byte highbyte = 0;
            byte lowbyte = 0;

            ServoDATA[2] = (byte)servo_cmd;

            GetLH_BYTES(ref highbyte, ref lowbyte, S1);
            ServoDATA[4] = highbyte;
            ServoDATA[5] = lowbyte;


            GetLH_BYTES(ref highbyte, ref lowbyte, S2);
            ServoDATA[6] = highbyte;
            ServoDATA[7] = lowbyte;


            GetLH_BYTES(ref highbyte, ref lowbyte, S3);
            ServoDATA[8] = highbyte;
            ServoDATA[9] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, S4);
            ServoDATA[10] = highbyte;
            ServoDATA[11] = lowbyte;
            


        }

        private void SetParamRPY(int param_cmd, double param_Roll, double param_Pitch, double param_Yaw, int param_Min, int param_Max, int param_Setpoint)
        {
            
                byte highbyte_s = 0;
                byte lowbyte_s = 0;
                
                PARAM_RPY[2] = (byte)param_cmd;



                double param_roll_p = param_Roll - Math.Truncate(param_Roll);
                PARAM_RPY[3] = (byte)param_Roll;
                PARAM_RPY[4] = (byte)(param_roll_p*100); 
               
            
                double param_pitch_p = param_Pitch - Math.Truncate(param_Pitch);
                PARAM_RPY[5] = (byte)param_Pitch;
                PARAM_RPY[6] = (byte)(param_pitch_p*100);

            
                double param_yaw_p = param_Yaw - Math.Truncate(param_Yaw);
                PARAM_RPY[7] = (byte)param_Yaw;
                PARAM_RPY[8] = (byte)(param_yaw_p*100);




                GetLH_BYTES(ref highbyte_s, ref lowbyte_s, param_Min);
                PARAM_RPY[9] = highbyte_s;
                PARAM_RPY[10] = lowbyte_s;


                GetLH_BYTES(ref highbyte_s, ref lowbyte_s, param_Max);
                PARAM_RPY[11] = highbyte_s;
                PARAM_RPY[12] = lowbyte_s;

                PARAM_RPY[13] = (byte)(param_Setpoint);
                
                        
                        if(pTcpClient!=null)
            {
                    if (pTcpClient.flag_connect)
                    pTcpClient.write(PARAM_RPY);
            }
           

        }


        private void RequestDATA(int param_cmd, int index)
        {

            try
            {
                PARAM_RPY[2] = (byte)param_cmd;
                PARAM_RPY[3] = (byte)index;
                if (pTcpClient != null)
                {
                    if (pTcpClient.flag_connect)
                        pTcpClient.write(PARAM_RPY);
                }
                
            }
            catch { }

        }
        private void SendingThrottle(int cmd, int index)
        {

            try
            {
                MotorDATA[2] = (byte)cmd;
                MotorDATA[3] = (byte)index;


                if (pTcpClient != null)
                {
                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
                
            }
            catch { }

        }
        private void SendingServo(int cmd, int index)
        {

            try
            {
                ServoDATA[2] = (byte)cmd;
                ServoDATA[3] = (byte)index;



                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(ServoDATA);
                }
                

            }
            catch { }

        }

        double _yaw;
        double _pitch;
        double _roll;
        double _gx;
        double _gy;
        double _gz;

        string st = "";
        string st2 = "";

        private void throttle_M1_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor1_throttle = Convert.ToInt32(txt_throttle_M1.Text);
                txt_throttle_M1.Text = "" + throttle_M1.Value;
                SetMotorData(CMD_THROTTLE, Convert.ToInt32(txt_throttle_M1.Text), 0, 0, 0, 0, 0);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }

        private void throttle_M2_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor2_throttle = Convert.ToInt32(txt_throttle_M2.Text);
                txt_throttle_M2.Text = "" + throttle_M2.Value;
                SetMotorData(CMD_THROTTLE, 0, Convert.ToInt32(txt_throttle_M2.Text), 0, 0, 0, 0);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }

        private void throttle_M3_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor3_throttle = Convert.ToInt32(txt_throttle_M3.Text);
                txt_throttle_M3.Text = "" + throttle_M3.Value;
                SetMotorData(CMD_THROTTLE, 0, 0, Convert.ToInt32(txt_throttle_M3.Text), 0, 0, 0);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }

        private void throttle_M4_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor4_throttle = Convert.ToInt32(txt_throttle_M4.Text);
                txt_throttle_M4.Text = "" + throttle_M4.Value;
                SetMotorData(CMD_THROTTLE, 0, 0, 0, Convert.ToInt32(txt_throttle_M4.Text), 0, 0);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }

        private void throttle_M5_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor5_throttle = Convert.ToInt32(txt_throttle_M5.Text);
                txt_throttle_M5.Text = "" + throttle_M5.Value;
                SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, Convert.ToInt32(txt_throttle_M5.Text), Convert.ToInt32(txt_throttle_M6.Text));
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }

        private void throttle_M6_Scroll(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {
                Motor6_throttle = Convert.ToInt32(txt_throttle_M6.Text);
                txt_throttle_M6.Text = "" + throttle_M6.Value;
                SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, Convert.ToInt32(txt_throttle_M5.Text), Convert.ToInt32(txt_throttle_M6.Text));
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }
            }
        }



        
        









        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (pconnect == null)
            {
                try
                {
                    pconnect = new Comport();
                    pconnect.getsensorctrl += new Comport.GetSensorCTRL(GetSensorCTRL);
                    pconnect.getcmd += new Comport.GetCMD(GetCMD);
                    pconnect.Open(txt_COM.Text, Convert.ToInt32(txt_baud.Text), 0);
                  //  SetMotorData(CMD_REST, 0, 0, 0, 0, 0 ,0);
                 //   pconnect.Send_Byte(MotorDATA);

                }
                catch
                {
                    MessageBox.Show("Error: Connecting");
                }
            }
            else
            {
                MessageBox.Show("Connected");
            }

        }
        public void GetCMD(byte[] data)
        {
            this.Invoke((MethodInvoker)delegate
            {
                String st = "";

                for (int i = 0; i < data.Length; i++)
                {
                    st += "" + data[i];
                    st += " ";
                }

                //  txt_terminal_sensor.Text = st;
            });
        }
        public void GetSensorCTRL_TCP(string st)//(byte[] data)
        {

            this.Invoke((MethodInvoker)delegate
            {

                txt_terminal.Text = st;
            });
            }
        public void GetSensorCTRL(string st)//(byte[] data)
        {

            this.Invoke((MethodInvoker)delegate
            {

                txt_terminal.Text = st;
                /*
                _yaw = Math.Round((double)((SByte)data[5]) * 180/(Math.PI*100),3) ;
                _pitch = Math.Round((double)((SByte)data[4]) * 180 / (Math.PI * 100),3);
                _roll = Math.Round((double)((SByte)data[3]) * 180 / (Math.PI * 100) ,3);

                if (flag_auto)
                {
                    
                    pidCTRL_yaw.myInput = _yaw;
                    pidCTRL_roll.myInput = _roll;
                    pidCTRL_pitch.myInput = _pitch;

                    pidCTRL_yaw.Compute();
                    pidCTRL_roll.Compute();
                    pidCTRL_pitch.Compute();


                    txt_PIDout_m1.Text = ""  + (int)(pidCTRL_pitch.myOutput / 2)  + (int)(-pidCTRL_roll.myOutput / 2) + (int)(pidCTRL_yaw.myOutput);
                    txt_PIDout_m2.Text = "" + (int)(pidCTRL_pitch.myOutput / 2) + (int)(pidCTRL_roll.myOutput / 2) + (int)-pidCTRL_yaw.myOutput;
                    txt_PIDout_m3.Text = "" + (int)(-pidCTRL_pitch.myOutput / 2) + (int)(pidCTRL_roll.myOutput / 2) + (int)-pidCTRL_yaw.myOutput;
                    txt_PIDout_m4.Text = "" + (int)(-pidCTRL_pitch.myOutput / 2) + (int)(-pidCTRL_roll.myOutput / 2) + (int)pidCTRL_yaw.myOutput;




                    Motor1_ctrl = Motor1_throttle + (int)(pidCTRL_pitch.myOutput - pidCTRL_roll.myOutput) / 2 + (int)pidCTRL_yaw.myOutput;         //+ rotation_roll2 + rotation_pitch2 + rotation_roll3 + rotation_pitch3;
                    Motor2_ctrl = Motor2_throttle + (int)(pidCTRL_pitch.myOutput + pidCTRL_roll.myOutput)/2 - (int)pidCTRL_yaw.myOutput;         //+ rotation_roll2 + rotation_pitch2 + rotation_roll4 + rotation_pitch4;
                    Motor3_ctrl = Motor3_throttle + (int)(-pidCTRL_pitch.myOutput + pidCTRL_roll.myOutput)/2 - (int)pidCTRL_yaw.myOutput;            //+ rotation_roll1 + rotation_pitch1 + rotation_roll4 + rotation_pitch4;
                    Motor4_ctrl = Motor4_throttle + (int)(-pidCTRL_pitch.myOutput - pidCTRL_roll.myOutput)/2 + (int)pidCTRL_yaw.myOutput;            //+ rotation_roll1 + rotation_pitch1 + rotation_roll3 + rotation_pitch3;



                    
                   




                    SetMotorData(CMD_THROTTLE, Motor1_ctrl, Motor2_ctrl, Motor3_ctrl, Motor4_ctrl, Convert.ToInt32(txt_throttle_M5.Text), Convert.ToInt32(txt_throttle_M6.Text));
                    if (pconnect != null)
                    {
                        if (pconnect.IsOpen)
                        {
                            pconnect.Send_Byte(MotorDATA);
                        }
                    }

                }

                txt_terminal.Text = st;
                st = "";*/
            });
        }
        /*
        void checklimit()
        {
            if (Motor1_ctrl > 2150)
            {
                Motor1_ctrl = 2150;
            }
            else if (Motor1_ctrl < 1150)
            {
                Motor1_ctrl = 1150;
            }
            if (Motor2_ctrl > 2150)
            {
                Motor2_ctrl = 2150;
            }
            else if (Motor2_ctrl < 1150)
            {
                Motor2_ctrl = 1150;
            }
            if (Motor3_ctrl > 2150)
            {
                Motor3_ctrl = 2150;
            }
            else if (Motor3_ctrl < 1150)
            {
                Motor3_ctrl = 1150;
            }
            if (Motor4_ctrl > 2150)
            {
                Motor4_ctrl = 2150;
            }
            else if (Motor4_ctrl < 1150)
            {
                Motor4_ctrl = 1150;
            }
            
        }
        */

        public void getdistance(double pitch, double roll)
        {
            if (pitch > 0)
            {
                Motor1_dist = _pitch;
                Motor3_dist = -_pitch;
            }
            /* else if (pitch == 0)
             {
                 Motor1_dist = 0.0;
                 Motor3_dist = 0.0;
             }
             */
            else if (pitch < 0)
            {
                Motor1_dist = _pitch;
                Motor3_dist = -_pitch;
            }

            if (roll > 0)
            {
                Motor2_dist = roll;
                Motor4_dist = -roll;
            }
            else if (roll < 0)
            {
                Motor2_dist = roll;
                Motor4_dist = -roll;
            }


        }
        int cntchart = 0;
        int cnttchart = 0;
        


        private void timer1_Tick(object sender, EventArgs e)
        {
            /*

            txt_rot_front.Text = ""  + rotation_pitch1 +"  "+ rotation_roll1;
            txt_rot_back.Text = "" + rotation_pitch2 + "  " + rotation_roll2;
            txt_rot_left.Text = "" + rotation_pitch3 + "  " + rotation_roll3;
            txt_rot_right.Text = "" + rotation_pitch4 + "  " + rotation_roll4;

            
            if (autoctrl.Checked)
            {

                string splitvalue = txt_terminal.Text;
                string[] result = splitvalue.Split(',');

                try {
                    txt_roll.Text = result[0];
                    txt_pitch.Text = result[1];
                    txt_pitch.Text = result[2];
                    txt_PIDout_m1.Text = result[3];
                    txt_PIDout_m2.Text = result[4];
                    txt_PIDout_m3.Text = result[5];
                    txt_PIDout_m4.Text = result[6];
                }
                catch { }
            }
            


            txt_yaw_out.Text = "" + pidCTRL_yaw.myOutput;
            txt_roll_out.Text = "" + pidCTRL_roll.myOutput;
            txt_pitch_out.Text = "" + pidCTRL_pitch.myOutput;

            txt_PWM_m1.Text = "" + Motor1_ctrl;
            txt_PWM_m2.Text = "" + Motor2_ctrl;
            txt_PWM_m3.Text = "" + Motor3_ctrl;
            txt_PWM_m4.Text = "" + Motor4_ctrl;

            Pic1_M1.Text = "" + Motor1_ctrl;
            Pic1_M2.Text = "" + Motor2_ctrl;
            Pic1_M3.Text = "" + Motor3_ctrl;
            Pic1_M4.Text = "" + Motor4_ctrl;

            Pic2_M1.Text = "" + Motor1_dist;
            Pic2_M2.Text = "" + Motor2_dist;
            Pic2_M3.Text = "" + Motor3_dist;
            Pic2_M4.Text = "" + Motor4_dist;

            motor_graph.ChartAreas[0].AxisY.Minimum = 900;
            motor_graph.ChartAreas[0].AxisY.Maximum = 2300;
            motor_graph.ChartAreas[0].AxisX.Interval = 500;

            YRP_Graph.ChartAreas[0].AxisY.Minimum = -100;
            YRP_Graph.ChartAreas[0].AxisY.Maximum = 100;
            YRP_Graph.ChartAreas[0].AxisX.Interval = 5000;

          //  Gyro_Graph.ChartAreas[0].AxisY.Minimum = -500;
          //  Gyro_Graph.ChartAreas[0].AxisY.Maximum = 500;
          //  Gyro_Graph.ChartAreas[0].AxisX.Interval = 5000;

         //   dist_graph.ChartAreas[0].AxisY.Minimum = -2000;
         //   dist_graph.ChartAreas[0].AxisY.Maximum = 2000;
         //   dist_graph.ChartAreas[0].AxisX.Interval = 5000;

            

            this.Invoke((MethodInvoker)delegate
            {
                motor_graph.Series["M_Thr1"].Points.AddY(Motor1_ctrl);
                motor_graph.Series["M_Thr2"].Points.AddY(Motor2_ctrl);
                motor_graph.Series["M_Thr3"].Points.AddY(Motor3_ctrl);
                motor_graph.Series["M_Thr4"].Points.AddY(Motor4_ctrl);

                // YRP_Graph.Series["Yaw"].Points.AddY(_yaw * 100);

                YRP_Graph.Series["Roll"].Points.AddY(_roll );

                 YRP_Graph.Series["Pitch"].Points.AddY(_pitch );

           //     Gyro_Graph.Series["Gyro_X"].Points.AddY(_gx * 50);
           ////    Gyro_Graph.Series["Gyro_Y"].Points.AddY(_gy * 100);
            //    Gyro_Graph.Series["Gyro_Z"].Points.AddY(_gz * 100);

            //    dist_graph.Series["D_M1"].Points.AddY(Motor1_dist * 100);
             //   dist_graph.Series["D_M2"].Points.AddY(Motor2_dist * 100);
              //  dist_graph.Series["D_M3"].Points.AddY(Motor3_dist * 100);
              //  dist_graph.Series["D_M4"].Points.AddY(Motor4_dist * 100);



                if (cntchart++ > 200)
                {
                    motor_graph.Series["M_Thr1"].Points.RemoveAt(0);
                    motor_graph.Series["M_Thr2"].Points.RemoveAt(0);
                    motor_graph.Series["M_Thr3"].Points.RemoveAt(0);
                    motor_graph.Series["M_Thr4"].Points.RemoveAt(0);

                  //YRP_Graph.Series["Yaw"].Points.RemoveAt(0);
                    YRP_Graph.Series["Roll"].Points.RemoveAt(0);
                    YRP_Graph.Series["Pitch"].Points.RemoveAt(0);

                }


                if (cnttchart++ > 300)

                {
/*
                   Gyro_Graph.Series["Gyro_X"].Points.RemoveAt(0);
                   Gyro_Graph.Series["Gyro_Y"].Points.RemoveAt(0);
                   Gyro_Graph.Series["Gyro_Z"].Points.RemoveAt(0);


                    dist_graph.Series["D_M1"].Points.RemoveAt(0);
                    dist_graph.Series["D_M2"].Points.RemoveAt(0);
                    dist_graph.Series["D_M3"].Points.RemoveAt(0);
                    dist_graph.Series["D_M4"].Points.RemoveAt(0);
                }

            });

            canvas_Paint();
            distance_Paint();
            rotation();*/
        }
        

        private void btn_update_Click(object sender, EventArgs e)
        {
            setup_PID();
        }
        void setup_PID()
        {
            try
            {

                

                

                ///////// Yaw Roll Pitch ///////////
                ///////////////////////////////////


                pidCTRL_yaw.SetMode(1);
                kpy = Convert.ToDouble(txt_yaw_kp.Text);
                kiy = Convert.ToDouble(txt_yaw_ki.Text);
                kdy = Convert.ToDouble(txt_yaw_kd.Text);
                outminy = Convert.ToDouble(txt_yaw_outmin.Text);
                outmaxy = Convert.ToDouble(txt_yaw_outmax.Text);
                pidCTRL_yaw.SetOutputLimits(outminy, outmaxy);
                pidCTRL_yaw.SetTunings(kpy, kiy, 0);
                pidCTRL_yaw.mySetpoint = Convert.ToDouble(txt_sp_yaw.Text);

                pidCTRL_roll.SetMode(1);
                kpr = Convert.ToDouble(txt_roll_kp.Text);
                kir = Convert.ToDouble(txt_roll_ki.Text);
                kdr = Convert.ToDouble(txt_roll_kd.Text);
                outminr = Convert.ToDouble(txt_roll_outmin.Text);
                outmaxr = Convert.ToDouble(txt_roll_outmax.Text);
                pidCTRL_roll.SetOutputLimits(outminr, outmaxr);
                pidCTRL_roll.SetTunings(kpr, kir, kdr);
                pidCTRL_roll.mySetpoint = Convert.ToDouble(txt_sp_roll.Text);

                pidCTRL_pitch.SetMode(1);
                kpp = Convert.ToDouble(txt_pitch_kp.Text);
                kip = Convert.ToDouble(txt_pitch_ki.Text);
                kdp = Convert.ToDouble(txt_pitch_kd.Text);
                outminp = Convert.ToDouble(txt_pitch_outmin.Text);
                outmaxp = Convert.ToDouble(txt_pitch_outmax.Text);
                pidCTRL_pitch.SetOutputLimits(outminp, outmaxp);
                pidCTRL_pitch.SetTunings(kpp, kip, kdp);
                pidCTRL_pitch.mySetpoint = Convert.ToDouble(txt_sp_pitch.Text);

            }
            catch
            {

                MessageBox.Show("Error");
            }

             
        }

        private void btn_rest_Click(object sender, EventArgs e)
        {
            SetMotorData(CMD_REST, 0, 0, 0, 0,0,0);
            throttle_M1.Value = 1050;
            throttle_M2.Value = 1050;
            throttle_M3.Value = 1050;
            throttle_M4.Value = 1050;
            throttle_M5.Value = 1050;
            throttle_M6.Value = 1050;






            if (pTcpClient != null)
            {

                if (pTcpClient.flag_connect)
                    pTcpClient.write(MotorDATA);
            }
        }

        private void throttle_M1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void throttle_M1_MouseLeave(object sender, EventArgs e)
        {

        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        /*
        private void check_auto_CheckedChanged(object sender, EventArgs e)
        {
            Motor1_throttle = Convert.ToInt32(txt_throttle_M1.Text);
            Motor2_throttle = Convert.ToInt32(txt_throttle_M2.Text);
            Motor3_throttle = Convert.ToInt32(txt_throttle_M3.Text);
            Motor4_throttle = Convert.ToInt32(txt_throttle_M4.Text);
            Motor5_throttle = Convert.ToInt32(txt_throttle_M5.Text);
            Motor6_throttle = Convert.ToInt32(txt_throttle_M6.Text);

            if (check_auto.Checked)
            {
                flag_auto = true;
            }
            else
            {
                flag_auto = false;
            }
        }
        */
        private void txt_sp_M1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            Motor1_throttle = Convert.ToInt32(txt_throttle_M1.Text);
            Motor2_throttle = Convert.ToInt32(txt_throttle_M2.Text);
            Motor3_throttle = Convert.ToInt32(txt_throttle_M3.Text);
            Motor4_throttle = Convert.ToInt32(txt_throttle_M4.Text);
            try
            {
                SetMotorData(CMD_THROTTLE, Convert.ToInt32(txt_throttle_M1.Text), Convert.ToInt32(txt_throttle_M2.Text), Convert.ToInt32(txt_throttle_M3.Text), Convert.ToInt32(txt_throttle_M4.Text), Convert.ToInt32(txt_throttle_M5.Text), Convert.ToInt32(txt_throttle_M6.Text));
                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(MotorDATA);
                    }
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Motor1_throttle = Convert.ToInt32(txt_throttle_M1.Text);
            Motor2_throttle = Convert.ToInt32(txt_throttle_M2.Text);
            Motor3_throttle = Convert.ToInt32(txt_throttle_M3.Text);
            Motor4_throttle = Convert.ToInt32(txt_throttle_M4.Text);
            throttle_M1.Value = Motor1_throttle;
            throttle_M2.Value = Motor2_throttle;
            throttle_M3.Value = Motor3_throttle;
            throttle_M4.Value = Motor4_throttle;
            //  throttle_all


            try
            {
                SetMotorData(CMD_THROTTLE, Convert.ToInt32(txt_throttle_M1.Text), Convert.ToInt32(txt_throttle_M2.Text), Convert.ToInt32(txt_throttle_M3.Text), Convert.ToInt32(txt_throttle_M4.Text), Convert.ToInt32(txt_throttle_M5.Text), Convert.ToInt32(txt_throttle_M6.Text));
                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(MotorDATA);
                    }
                }
            }
            catch { }
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void txt_throttle_all_TextChanged(object sender, EventArgs e)
        {

        }

        private void danger_stop_Click(object sender, EventArgs e)
        {

            flag_auto = false;

            SetMotorData(CMD_REST, 0, 0, 0, 0, 0, 0);
            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(MotorDATA);
                }
            }

        }

        private void Zephyros_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("g");
        }

        public void throttle_all_Scroll_1(object sender, EventArgs e)
        {

            Int32 value = throttle_all.Value;
            try
            {
                throttle_M1.Value = 1200 + value;
                Motor1_throttle = throttle_M1.Value;
            }
            catch { }
            try
            {
                throttle_M2.Value = 1200 + value;
                Motor2_throttle = throttle_M2.Value;
            }
            catch { }
            try
            {
                throttle_M3.Value = 1200 + value;
                Motor3_throttle = throttle_M3.Value;
            }
            catch { }
            try
            {
                throttle_M4.Value = 1200 + value;
                Motor4_throttle = throttle_M4.Value;
            }
            catch { }
            try
            {
                throttle_M5.Value = 1200 + value;
                Motor5_throttle = throttle_M5.Value;
            }
            catch { }
            try
            {
                throttle_M6.Value = 1200 + value;
                Motor6_throttle = throttle_M6.Value;
            }

            catch { }
            

            txt_throttle_M1.Text = "" + throttle_M1.Value;
            txt_throttle_M2.Text = "" + throttle_M2.Value;
            txt_throttle_M3.Text = "" + throttle_M3.Value;
            txt_throttle_M4.Text = "" + throttle_M4.Value;
            txt_throttle_M5.Text = "" + throttle_M5.Value;
            txt_throttle_M6.Text = "" + throttle_M6.Value;

            SetMotorData(CMD_THROTTLE, throttle_M1.Value, throttle_M2.Value, throttle_M3.Value, throttle_M4.Value, throttle_M5.Value, throttle_M6.Value);

            SendingThrottle(0x30, 0xa0);
            if (pTcpClient != null)
            {

                if (pTcpClient.flag_connect)
                    pTcpClient.write(MotorDATA);
            }
        }
        

        ///// PID 게인값 조정 ////

        public void save_btn_Click(object sender, EventArgs e)
        {

            SaveFileDialog savePanel = new SaveFileDialog();
            savePanel.Filter = "Text File|*.txt";
            savePanel.FileName = "save";
            savePanel.Title = "Save Text File";

            if (savePanel.ShowDialog() == DialogResult.OK)
            {
                changedata();

                string path = savePanel.FileName;
                StreamWriter sw = new StreamWriter(File.Create(path));
                for (int x = 1; x < savewrite.Length; x++)
                {
                    sw.WriteLine(savewrite[x]);

                }
                sw.Close();
            }
        }

       

        private void btn_pitch_param_FB_Click(object sender, EventArgs e)
        {
         
                RequestDATA(0x11, 0xc0);
    
        }

        private void btn_roll_param_FB_Click(object sender, EventArgs e)
        {

            RequestDATA(0x11, 0xc1);

        }

        private void btn_yaw_param_FB_Click(object sender, EventArgs e)
        {

            RequestDATA(0x11, 0xc2);

        }
        

        private void button3_Click(object sender, EventArgs e)
        {

            RequestDATA(0x12, 0xd0);
        }

        private void check_throttle_on_CheckedChanged(object sender, EventArgs e)
        {
            if (check_throttle_on.Checked)
            {

                SendingThrottle(0x30, 0xa0);
            }
            else
            {
                SendingThrottle(0x30, 0xa4);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

            RequestDATA(0x30, 0xa2);
        }

        private void autoctrl_CheckedChanged(object sender, EventArgs e)
        {
            if(autoctrl.Checked)
            {
                RequestDATA(0x0d, 0xb0); // calculator pid

            }
            else
            {
                RequestDATA(0x0f, 0xb2);  // stop
            } 
        }

        private void txt_terminal2_TextChanged(object sender, EventArgs e)
        {

        }
        

        private void pitchup_btn_Click(object sender, EventArgs e)
        {
            
                try
                {
                    SetParamRPY(PARAM_pitch, Convert.ToDouble(txt_pitch_kp.Text), Convert.ToDouble(txt_pitch_ki.Text), Convert.ToDouble(txt_pitch_kd.Text), Convert.ToInt32(txt_pitch_outmin.Text), Convert.ToInt32(txt_pitch_outmax.Text), Convert.ToInt32(txt_sp_pitch.Text));
                }
                catch { }
            
        }



        private void rollup_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SetParamRPY(PARAM_roll, Convert.ToDouble(txt_roll_kp.Text), Convert.ToDouble(txt_roll_ki.Text), Convert.ToDouble(txt_roll_kd.Text), Convert.ToInt32(txt_roll_outmin.Text), Convert.ToInt32(txt_roll_outmax.Text), Convert.ToInt32(txt_sp_roll.Text));
            }
            catch { }
        
    }

        private void yawup_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SetParamRPY(PARAM_yaw, Convert.ToDouble(txt_yaw_kp.Text), Convert.ToDouble(txt_yaw_ki.Text), Convert.ToDouble(txt_yaw_kd.Text), Convert.ToInt32(txt_yaw_outmin.Text), Convert.ToInt32(txt_yaw_outmax.Text), Convert.ToInt32(txt_sp_yaw.Text));
            }
            catch
            {

            }
        }

        private void load_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openPanel = new OpenFileDialog();
            openPanel.Filter = "Text File|*.txt";
            openPanel.Title = "Open Text File";
            if (openPanel.ShowDialog() == DialogResult.OK)
            {

                string path = openPanel.FileName;
                StreamReader sr = new StreamReader(File.OpenRead(path));

                for (int x = 1; x < savewrite.Length; x++)
                {
                    savewrite[x] = sr.ReadLine();

                }

                sr.Close();

                load_Data();
                
            }
           
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            RequestDATA(0x12, 0xd1);
        }

        private void throttle_up_btn_Click(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {

                throttle_all.Value += 10;

                Int32 value = throttle_all.Value;



                try
                {
                    throttle_M1.Value = 1200 + value;
                    Motor1_throttle = throttle_M1.Value;
                }
                catch { }
                try
                {
                    throttle_M2.Value = 1200 + value;
                    Motor2_throttle = throttle_M2.Value;
                }
                catch { }
                try
                {
                    throttle_M3.Value = 1200 + value;
                    Motor3_throttle = throttle_M3.Value;
                }
                catch { }
                try
                {
                    throttle_M4.Value = 1200 + value;
                    Motor4_throttle = throttle_M4.Value;
                }
                catch { }
                try
                {
                    throttle_M5.Value = 1200 + value;
                    Motor5_throttle = throttle_M5.Value;
                }
                catch { }
                try
                {
                    throttle_M6.Value = 1200 + value;
                    Motor6_throttle = throttle_M6.Value;
                }

                catch { }


                txt_throttle_M1.Text = "" + throttle_M1.Value;
                txt_throttle_M2.Text = "" + throttle_M2.Value;
                txt_throttle_M3.Text = "" + throttle_M3.Value;
                txt_throttle_M4.Text = "" + throttle_M4.Value;
                txt_throttle_M5.Text = "" + throttle_M5.Value;
                txt_throttle_M6.Text = "" + throttle_M6.Value;

                SetMotorData(CMD_THROTTLE, throttle_M1.Value, throttle_M2.Value, throttle_M3.Value, throttle_M4.Value, throttle_M5.Value, throttle_M6.Value);

                SendingThrottle(0x30, 0xa1);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }

            }
        }

        private void throttle_down_btn_Click(object sender, EventArgs e)
        {
            if (autoctrl.Checked)
            {

                if (throttle_all.Value > 0)
                {
                    throttle_all.Value -= 10;

                    Int32 value = throttle_all.Value;



                    try
                    {
                        throttle_M1.Value = 1200 + value;
                        Motor1_throttle = throttle_M1.Value;
                    }
                    catch { }
                    try
                    {
                        throttle_M2.Value = 1200 + value;
                        Motor2_throttle = throttle_M2.Value;
                    }
                    catch { }
                    try
                    {
                        throttle_M3.Value = 1200 + value;
                        Motor3_throttle = throttle_M3.Value;
                    }
                    catch { }
                    try
                    {
                        throttle_M4.Value = 1200 + value;
                        Motor4_throttle = throttle_M4.Value;
                    }
                    catch { }
                    try
                    {
                        throttle_M5.Value = 1200 + value;
                        Motor5_throttle = throttle_M5.Value;
                    }
                    catch { }
                    try
                    {
                        throttle_M6.Value = 1200 + value;
                        Motor6_throttle = throttle_M6.Value;
                    }

                    catch { }


                    txt_throttle_M1.Text = "" + throttle_M1.Value;
                    txt_throttle_M2.Text = "" + throttle_M2.Value;
                    txt_throttle_M3.Text = "" + throttle_M3.Value;
                    txt_throttle_M4.Text = "" + throttle_M4.Value;
                    txt_throttle_M5.Text = "" + throttle_M5.Value;
                    txt_throttle_M6.Text = "" + throttle_M6.Value;

                    SetMotorData(CMD_THROTTLE, throttle_M1.Value, throttle_M2.Value, throttle_M3.Value, throttle_M4.Value, throttle_M5.Value, throttle_M6.Value);

                    SendingThrottle(0x30, 0xa1);
                    if (pTcpClient != null)
                    {

                        if (pTcpClient.flag_connect)
                            pTcpClient.write(MotorDATA);
                    }
                }
            }
            

        }

        private void kpy_scroll_Scroll(object sender, EventArgs e)
        {
            txt_yaw_kp.Text = "" + kpy_scroll.Value / 100.0;
        }



        private void Servo_bar_A1_Scroll(object sender, EventArgs e)
        {
            if (check_Servo_on.Checked)
            {
                ServoA1_throttle = Convert.ToInt32(txt_servo_a1.Text);
                txt_servo_a1.Text = "" + Servo_bar_A1.Value;

                SetServoData(CMD_SERVO, Convert.ToInt32(txt_servo_a1.Text), 1500, 1500, 1500);



                SendingServo(0x13, 0xa3);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(ServoDATA);
                    }
                }
            }
        }

        private void Servo_bar_A2_Scroll(object sender, EventArgs e)
        {
            if (check_Servo_on.Checked)
            {
                ServoA2_throttle = Convert.ToInt32(txt_servo_a2.Text);
                txt_servo_a2.Text = "" + Servo_bar_A2.Value;

                SetServoData(CMD_SERVO, 1500, Convert.ToInt32(txt_servo_a2.Text), 1500, 1500);



                SendingServo(0x13, 0xa3);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(ServoDATA);
                    }
                }
            }
        }

        private void Servo_bar_C1_Scroll(object sender, EventArgs e)
        {
            if (check_Servo_on.Checked)
            {
                ServoC1_throttle = Convert.ToInt32(txt_servo_c1.Text);
                txt_servo_c1.Text = "" + Servo_bar_C1.Value;

                SetServoData(CMD_SERVO, 1500, 1500, Convert.ToInt32(txt_servo_c1.Text), 1500);



                SendingServo(0x13, 0xa3);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(ServoDATA);
                    }
                }
            }
        }

        private void Servo_bar_C2_Scroll(object sender, EventArgs e)
        {
            if (check_Servo_on.Checked)
            {

                ServoC2_throttle = Convert.ToInt32(txt_servo_c2.Text);
                txt_servo_c2.Text = "" + Servo_bar_C2.Value;


                SetServoData(CMD_SERVO, 1500, 1500, 1500, Convert.ToInt32(txt_servo_c2.Text));



                SendingServo(0x13, 0xa3);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(ServoDATA);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetServoData(CMD_SERVO, 1500, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
        }

        private void check_Servo_on_CheckedChanged(object sender, EventArgs e)
        {
            if (check_Servo_on.Checked)
            {
                SendingServo(0x14, 0x00); // calculator pid


            }
        }

        private void button8_Click(object sender, EventArgs e)
        {


            SendingServo(0x12, 0xd0);
        }

        private void driving_forward_Click(object sender, EventArgs e)
        {
            
                ServoA1_throttle = 1550;
                ServoA2_throttle = 1450;

                SetServoData(CMD_SERVO, ServoA1_throttle, ServoA2_throttle, 1500, 1700);



                SendingServo(0x13, 0xa3);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(ServoDATA);
                    }
                }


            Thread.Sleep(300);

            ServoA1_throttle = 1500;

            SetServoData(CMD_SERVO, ServoA1_throttle, ServoA1_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }


        }

        private void driving_backward_Click(object sender, EventArgs e)
        {
            ServoA1_throttle = 1450;
            ServoA2_throttle = 1550;

            SetServoData(CMD_SERVO, ServoA1_throttle, ServoA2_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

            Thread.Sleep(300);

            ServoA1_throttle = 1500;

            SetServoData(CMD_SERVO, ServoA1_throttle, ServoA1_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
        }

        private void driving_right_Click(object sender, EventArgs e)
        {
            ServoC1_throttle = 2400;
            ServoC2_throttle = 2400;

            SetServoData(CMD_SERVO, 1500, 1500, ServoC1_throttle, ServoC2_throttle);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
        }

        private void driving_left_Click(object sender, EventArgs e)
        {
            ServoC1_throttle = 700;
            ServoC2_throttle = 700;

            SetServoData(CMD_SERVO, 1500, 1500, ServoC1_throttle, ServoC2_throttle);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
        }

        private void driving_stop_Click(object sender, EventArgs e)
        {
            ServoC1_throttle = 1540;
            ServoC2_throttle = 1460;

            SetServoData(CMD_SERVO, 1500, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);



            SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, 0, 0);

            SendingThrottle(0x30, 0xa1);
            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
            
            
        }

        private void servo1_all_cw_Click(object sender, EventArgs e)
        {
            

            ServoA1_throttle = 1548;

            SetServoData(CMD_SERVO, ServoA1_throttle, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

            

            Thread.Sleep(490);

            ServoC1_throttle = 1500;

            SetServoData(CMD_SERVO, ServoC1_throttle, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }


        }

        private void servo1_all_ccw_Click(object sender, EventArgs e)
        {
            ServoC1_throttle = 1462;

            SetServoData(CMD_SERVO, ServoC1_throttle, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

            Thread.Sleep(490);

            ServoC1_throttle = 1500;

            SetServoData(CMD_SERVO, ServoC1_throttle, 1500, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }
        }

        private void servo2_all_cw_Click(object sender, EventArgs e)
        {
            ServoC2_throttle = 1462;

            SetServoData(CMD_SERVO, 1500, ServoC2_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

            Thread.Sleep(490);

            ServoC2_throttle = 1500;

            SetServoData(CMD_SERVO, 1500, ServoC2_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

        }

        private void servo2_all_ccw_Click(object sender, EventArgs e)
        {
            ServoC2_throttle = 1560;

            SetServoData(CMD_SERVO, 1500, ServoC2_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

            Thread.Sleep(490);

            ServoC2_throttle = 1500;

            SetServoData(CMD_SERVO, 1500, ServoC2_throttle, 1500, 1700);



            SendingServo(0x13, 0xa3);

            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(ServoDATA);
                }
            }

        }

        private void driving_go_Click(object sender, EventArgs e)
        {
            
            
                SendingThrottle(0x30, 0xa1);

                
                SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, 1200, 1200);

                if (pconnect != null)
                {
                    if (pconnect.IsOpen)
                    {
                        pconnect.Send_Byte(MotorDATA);
                    }
                }
                
            
        }

        private void check_prop_on_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (check_throttle_on.Checked)
            {

                throttle_all.Value += 50;

                Int32 value = throttle_all.Value;



                try
                {
                    throttle_M1.Value = 1200 + value;
                    Motor1_throttle = throttle_M1.Value;
                }
                catch { }
                try
                {
                    throttle_M2.Value = 1200 + value;
                    Motor2_throttle = throttle_M2.Value;
                }
                catch { }
                try
                {
                    throttle_M3.Value = 1200 + value;
                    Motor3_throttle = throttle_M3.Value;
                }
                catch { }
                try
                {
                    throttle_M4.Value = 1200 + value;
                    Motor4_throttle = throttle_M4.Value;
                }
                catch { }
                try
                {
                    throttle_M5.Value = 1200 + value;
                    Motor5_throttle = throttle_M5.Value;
                }
                catch { }
                try
                {
                    throttle_M6.Value = 1200 + value;
                    Motor6_throttle = throttle_M6.Value;
                }

                catch { }


                txt_throttle_M1.Text = "" + throttle_M1.Value;
                txt_throttle_M2.Text = "" + throttle_M2.Value;
                txt_throttle_M3.Text = "" + throttle_M3.Value;
                txt_throttle_M4.Text = "" + throttle_M4.Value;
                txt_throttle_M5.Text = "" + throttle_M5.Value;
                txt_throttle_M6.Text = "" + throttle_M6.Value;

                SetMotorData(CMD_THROTTLE, throttle_M1.Value, throttle_M2.Value, throttle_M3.Value, throttle_M4.Value, throttle_M5.Value, throttle_M6.Value);

                SendingThrottle(0x30, 0xa0);
                if (pTcpClient != null)
                {

                    if (pTcpClient.flag_connect)
                        pTcpClient.write(MotorDATA);
                }

            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(check_prop_on.Checked)
            {


                throttle_prop.Value += 10;

                Int32 propvalue = throttle_prop.Value;
                
                try
                {
                    throttle_prop.Value = 1200 + propvalue;
   
                }
                catch { }

                txt_prop1.Text = "" + propvalue;
                txt_prop2.Text = "" + propvalue;
                SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, Convert.ToInt32(txt_prop1.Text), Convert.ToInt32(txt_prop2.Text));

                    SendingThrottle(0x30, 0xa1);

                    if (pconnect != null)
                    {
                        if (pconnect.IsOpen)
                        {
                            pconnect.Send_Byte(MotorDATA);
                        }
                    }

                }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (check_prop_on.Checked)
            {

                if (throttle_prop.Value > 0)
                {
                    throttle_prop.Value -= 10;

                    Int32 propvalue = throttle_prop.Value;

                    try
                    {
                        throttle_prop.Value = 1200 + propvalue;

                    }
                    catch { }


                    txt_prop1.Text = "" + propvalue;
                    txt_prop2.Text = "" + propvalue;

                    SetMotorData(CMD_THROTTLE, 0, 0, 0, 0, Convert.ToInt32(txt_prop1.Text), Convert.ToInt32(txt_prop2.Text));

                    SendingThrottle(0x30, 0xa1);

                    if (pconnect != null)
                    {
                        if (pconnect.IsOpen)
                        {
                            pconnect.Send_Byte(MotorDATA);
                        }
                    }

                }
            }
        }

        private void throttle_prop_Scroll(object sender, EventArgs e)
        {

        }

        private void btn_receiver_param_FB_Click(object sender, EventArgs e)
        {

            RequestDATA(0x17, 0xc3);
        }
        

        private void button9_Click(object sender, EventArgs e)
        {

            pTcpClient = new TcpClient();
            pTcpClient.CreateServer(1900);
            pTcpClient.getsensorctrl_TCP += GetSensorCTRL_TCP;

        }

        private void kiy_scroll_Scroll(object sender, EventArgs e)
        {

            txt_yaw_ki.Text = "" + kiy_scroll.Value / 100.0;
        }

        private void kdy_scroll_Scroll(object sender, EventArgs e)
        {

            txt_yaw_kd.Text = "" + kdy_scroll.Value / 100.0;
        }
        // Yaw Gain
        private void kpp_scroll_Scroll_1(object sender, EventArgs e)
        {

            txt_pitch_kp.Text = "" + kpp_scroll.Value / 100.0;
        }

        private void kip_scroll_Scroll_1(object sender, EventArgs e)
        {

            txt_pitch_ki.Text = "" + kip_scroll.Value / 100.0;
        }

        private void kdp_scroll_Scroll_1(object sender, EventArgs e)
        {
            
            txt_pitch_kd.Text = "" + kdp_scroll.Value / 100.0;
        }

        private void kpr_scroll_Scroll_1(object sender, EventArgs e)
        {

            txt_roll_kp.Text = "" + kpr_scroll.Value / 100.0;
        }

        private void kir_scroll_Scroll_1(object sender, EventArgs e)
        {

            txt_roll_ki.Text = "" + kir_scroll.Value / 100.0;
        }

        private void kdr_scroll_Scroll_1(object sender, EventArgs e)
        {

            txt_roll_kd.Text = "" + kdr_scroll.Value / 100.0;
        }
        
        


       
        

        private void chart1_Click(object sender, EventArgs e)
        {

        }


        private void throttle_start_CheckedChanged(object sender, EventArgs e)
        {



        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void group_PID_control_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox23_Enter(object sender, EventArgs e)
        {

        }


        private void motor_graph_Click(object sender, EventArgs e)
        {
        }

        

       
        private void wall_Ready_CheckedChanged(object sender, EventArgs e)
        {
            throttle_M1.Value = 1130;
            throttle_M2.Value = 1130;
            throttle_M3.Value = 1130;
            throttle_M4.Value = 1130;
        }
       
        /*
        private void climb_M34_Scroll(object sender, EventArgs e)
        {
            if (wall_Start.Checked)
            {
                flag_auto = true;
            }
            else
            {
                flag_auto = false;
            }

            Motor3_throttle = climb_M34.Value;
            Motor4_throttle = climb_M34.Value;
            txt_climb_M3.Text = "" + climb_M34.Value;
            txt_climb_M4.Text = "" + climb_M34.Value;
            txt_throttle_M1.Text = "" + 1130;
            txt_throttle_M2.Text = "" + 1130;
            txt_throttle_M3.Text = txt_climb_M3.Text;
            txt_throttle_M4.Text = txt_climb_M4.Text;
            //SetMotorData(CMD_THROTTLE, throttle_M1.Value, throttle_M2.Value, climb_M34.Value, climb_M34.Value);
            if (pconnect != null)
            {
                if (pconnect.IsOpen)
                {
                    pconnect.Send_Byte(MotorDATA);
                }
            }
        }*/
        /*private void image_animation()
          {
              if(_roll > 0)
              {
                  pictureBox1.BackgroundImage = Properties.Resources._1;
              }
              if(_roll < 0)
              {
                  pictureBox1.BackgroundImage = Properties.Resources._2;
              }
          }*/
          /*
        private void canvas_Paint()
        {
            Graphics gObject = panel2.CreateGraphics();
            Pen Pens = new Pen(Color.Red, 3);
            Pen Pens1 = new Pen(Color.Blue, 3);
            Pen Pens2 = new Pen(Color.Black, 3);
            gObject.Clear(Color.LightGray);

            if (pidCTRL_M1.myOutput > 0)
            {

                gObject.DrawLine(Pens, 285, 50, 285, 25);
                gObject.DrawLine(Pens, 285, 25, 265, 35);
                gObject.DrawLine(Pens, 285, 25, 305, 35); // motor 1 up
                gObject.DrawEllipse(Pens2, 255, 5, 60, 60);
                gObject.DrawLine(Pens2, 55, 190, 260, 50);
                gObject.DrawLine(Pens2, 60, 60, 265, 190);

            }
            else if (pidCTRL_M1.myOutput == 0)
            {
                gObject.DrawLine(Pens2, 265, 35, 305, 35);
                gObject.DrawEllipse(Pens2, 255, 5, 60, 60);
                gObject.DrawLine(Pens2, 55, 190, 260, 50);
                gObject.DrawLine(Pens2, 60, 60, 265, 190);
            }
            else
            {

                gObject.DrawLine(Pens1, 285, 25, 285, 50);
                gObject.DrawLine(Pens1, 285, 50, 265, 40);
                gObject.DrawLine(Pens1, 285, 50, 305, 40); // motor 1 dn
                gObject.DrawEllipse(Pens2, 255, 5, 60, 60);
                gObject.DrawLine(Pens2, 55, 190, 260, 50);
                gObject.DrawLine(Pens2, 60, 60, 265, 190);

            }
            if (pidCTRL_M2.myOutput > 0)
            {
                gObject.DrawLine(Pens, 40, 50, 40, 25);
                gObject.DrawLine(Pens, 40, 25, 20, 35);
                gObject.DrawLine(Pens, 40, 25, 60, 35);  // motor 2 up
                gObject.DrawEllipse(Pens2, 10, 5, 60, 60);
            }
            else if (pidCTRL_M2.myOutput == 0)
            {
                gObject.DrawLine(Pens2, 20, 35, 60, 35);
                gObject.DrawEllipse(Pens2, 10, 5, 60, 60);
            }
            else
            {

                gObject.DrawLine(Pens1, 40, 25, 40, 50);
                gObject.DrawLine(Pens1, 40, 50, 20, 40);
                gObject.DrawLine(Pens1, 40, 50, 60, 40); // motor 2 dn
                gObject.DrawEllipse(Pens2, 10, 5, 60, 60);

            }
            if (pidCTRL_M3.myOutput > 0)
            {
                gObject.DrawLine(Pens, 40, 235, 40, 210);
                gObject.DrawLine(Pens, 40, 210, 20, 225);
                gObject.DrawLine(Pens, 40, 210, 60, 225);  // motor 3 up
                gObject.DrawEllipse(Pens2, 10, 185, 60, 60);
            }

            else if (pidCTRL_M3.myOutput == 0)
            {
                gObject.DrawLine(Pens2, 20, 225, 60, 225);
                gObject.DrawEllipse(Pens2, 10, 185, 60, 60);
            }
            else
            {
                gObject.DrawLine(Pens1, 40, 210, 40, 235);
                gObject.DrawLine(Pens1, 40, 235, 20, 225);
                gObject.DrawLine(Pens1, 40, 235, 60, 225); // motor 3 dn
                gObject.DrawEllipse(Pens2, 10, 185, 60, 60);
            }
            if (pidCTRL_M4.myOutput > 0)
            {
                gObject.DrawLine(Pens, 285, 235, 285, 210);
                gObject.DrawLine(Pens, 285, 210, 265, 225);
                gObject.DrawLine(Pens, 285, 210, 305, 225); // motor 4 up
                gObject.DrawEllipse(Pens2, 255, 185, 60, 60);
            }

            else if (pidCTRL_M4.myOutput == 0)
            {
                gObject.DrawLine(Pens2, 265, 225, 305, 225);
                gObject.DrawEllipse(Pens2, 255, 185, 60, 60);
            }
            else
            {
                gObject.DrawLine(Pens1, 285, 210, 285, 235);
                gObject.DrawLine(Pens1, 285, 235, 265, 225);
                gObject.DrawLine(Pens1, 285, 235, 305, 225); // motor 4 dn
                gObject.DrawEllipse(Pens2, 255, 185, 60, 60);
            }

        }
        */
        /*
        private void Rotation_ctrl_CheckedChanged(object sender, EventArgs e)
        {
            if (Rotation_ctrl.Checked)
            {
                rotation_auto = true;
            }
            else
            {
                rotation_auto = false;
            }
        }
        private void distance_Paint()
        {
            Graphics gObject = panel1.CreateGraphics();

            Brush red = new SolidBrush(Color.Red);
            Brush blue = new SolidBrush(Color.Blue);
            Pen Pens3 = new Pen(Color.Red, 3);
            Pen Pens4 = new Pen(Color.Blue, 3);
            Pen Pens5 = new Pen(Color.Black, 3);
            gObject.Clear(Color.LightGray);
            if (Motor1_dist > 0)
            {

                gObject.DrawLine(Pens3, 285, 50, 285, 25);
                gObject.DrawLine(Pens3, 285, 25, 265, 35);
                gObject.DrawLine(Pens3, 285, 25, 305, 35); // motor 1 up
                gObject.DrawEllipse(Pens5, 255, 5, 60, 60);
                gObject.DrawLine(Pens5, 55, 190, 260, 50);
                gObject.DrawLine(Pens5, 60, 60, 265, 190);

            }
            else if (Motor1_dist == 0)
            {
                gObject.DrawLine(Pens5, 265, 35, 305, 35);
                gObject.DrawEllipse(Pens5, 255, 5, 60, 60);
                gObject.DrawLine(Pens5, 55, 190, 260, 50);
                gObject.DrawLine(Pens5, 60, 60, 265, 190);
            }
            else
            {

                gObject.DrawLine(Pens4, 285, 25, 285, 50);
                gObject.DrawLine(Pens4, 285, 50, 265, 40);
                gObject.DrawLine(Pens4, 285, 50, 305, 40); // motor 1 dn
                gObject.DrawEllipse(Pens5, 255, 5, 60, 60);
                gObject.DrawLine(Pens5, 55, 190, 260, 50);
                gObject.DrawLine(Pens5, 60, 60, 265, 190);

            }
            if (Motor2_dist > 0)
            {
                gObject.DrawLine(Pens3, 40, 50, 40, 25);
                gObject.DrawLine(Pens3, 40, 25, 20, 35);
                gObject.DrawLine(Pens3, 40, 25, 60, 35);  // motor 2 up
                gObject.DrawEllipse(Pens5, 10, 5, 60, 60);
            }
            else if (Motor2_dist == 0)
            {
                gObject.DrawLine(Pens5, 20, 35, 60, 35);
                gObject.DrawEllipse(Pens5, 10, 5, 60, 60);
            }
            else
            {

                gObject.DrawLine(Pens4, 40, 25, 40, 50);
                gObject.DrawLine(Pens4, 40, 50, 20, 40);
                gObject.DrawLine(Pens4, 40, 50, 60, 40); // motor 2 dn
                gObject.DrawEllipse(Pens5, 10, 5, 60, 60);

            }
            if (Motor3_dist > 0)
            {
                gObject.DrawLine(Pens3, 40, 235, 40, 210);
                gObject.DrawLine(Pens3, 40, 210, 20, 225);
                gObject.DrawLine(Pens3, 40, 210, 60, 225);  // motor 3 up
                gObject.DrawEllipse(Pens5, 10, 185, 60, 60);
            }

            else if (Motor3_dist == 0)
            {
                gObject.DrawLine(Pens5, 20, 225, 60, 225);
                gObject.DrawEllipse(Pens5, 10, 185, 60, 60);
            }
            else
            {
                gObject.DrawLine(Pens4, 40, 210, 40, 235);
                gObject.DrawLine(Pens4, 40, 235, 20, 225);
                gObject.DrawLine(Pens4, 40, 235, 60, 225); // motor 3 dn
                gObject.DrawEllipse(Pens5, 10, 185, 60, 60);
            }
            if (Motor4_dist > 0)
            {
                gObject.DrawLine(Pens3, 285, 235, 285, 210);
                gObject.DrawLine(Pens3, 285, 210, 265, 225);
                gObject.DrawLine(Pens3, 285, 210, 305, 225); // motor 4 up
                gObject.DrawEllipse(Pens5, 255, 185, 60, 60);
            }

            else if (Motor4_dist == 0)
            {
                gObject.DrawLine(Pens5, 265, 225, 305, 225);
                gObject.DrawEllipse(Pens5, 255, 185, 60, 60);
            }
            else
            {
                gObject.DrawLine(Pens4, 285, 210, 285, 235);
                gObject.DrawLine(Pens4, 285, 235, 265, 225);
                gObject.DrawLine(Pens4, 285, 235, 305, 225); // motor 4 dn
                gObject.DrawEllipse(Pens5, 255, 185, 60, 60);
            }

        }*/
        private void load_Data()
        {

            txt_yaw_kp.Text = savewrite[49];
            txt_yaw_ki.Text = savewrite[50];
            txt_yaw_kd.Text = savewrite[51];
            txt_yaw_outmin.Text = savewrite[52];
            txt_yaw_outmax.Text = savewrite[53];
            txt_sp_yaw.Text = savewrite[54];

            txt_pitch_kp.Text = savewrite[55];
            txt_pitch_ki.Text = savewrite[56];
            txt_pitch_kd.Text = savewrite[57];
            txt_pitch_outmin.Text = savewrite[58];
            txt_pitch_outmax.Text = savewrite[59];
            txt_sp_pitch.Text = savewrite[60];

            txt_roll_kp.Text = savewrite[61];
            txt_roll_ki.Text = savewrite[62];
            txt_roll_kd.Text = savewrite[63];
            txt_roll_outmin.Text = savewrite[64];
            txt_roll_outmax.Text = savewrite[65];
            txt_sp_roll.Text = savewrite[66];


        }
        public void changedata()
        {

            savewrite[49] = txt_yaw_kp.Text;
            savewrite[50] = txt_yaw_ki.Text;
            savewrite[51] = txt_yaw_kd.Text;
            savewrite[52] = txt_yaw_outmin.Text;
            savewrite[53] = txt_yaw_outmax.Text;
            savewrite[54] = txt_sp_yaw.Text;

            savewrite[55] = txt_pitch_kp.Text;
            savewrite[56] = txt_pitch_ki.Text;
            savewrite[57] = txt_pitch_kd.Text;
            savewrite[58] = txt_pitch_outmin.Text;
            savewrite[59] = txt_pitch_outmax.Text;
            savewrite[60] = txt_sp_pitch.Text;

            savewrite[61] = txt_roll_kp.Text;
            savewrite[62] = txt_roll_ki.Text;
            savewrite[63] = txt_roll_kd.Text;
            savewrite[64] = txt_roll_outmin.Text;
            savewrite[65] = txt_roll_outmax.Text;
            savewrite[66] = txt_sp_roll.Text;

        }

        public void splitdata()
        {
            
        }
    }
}




    

