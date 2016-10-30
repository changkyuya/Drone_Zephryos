using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Leap;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.UI;
using Emgu.Util.TypeEnum;
namespace WFA_LM_test1
{
    public partial class LeapZephyros : Form
    {
        Controller controller = new Controller();
        private  Listener _listener;
        Thread pthread;
        TcpServer pTcpClient;
        static IPAddress ipAd = IPAddress.Parse("192.168.0.13");
        TcpListener myList = new TcpListener(ipAd, 8080);

        public float wristxval;
        public float wristyval;
        public float wristzval;

        bool flag_right = false;
        bool flag_left = false;

        byte[] JOINTDATA = new byte[20];
        public byte[] Leap_motion = new byte[16] { 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 254 };


        private void GetLH_BYTES(ref byte Hbyte, ref byte Lbyte, int value)
        {
            Hbyte = (byte)(value >> 8);
            Lbyte = (byte)(value & 0x00FF);
        }

        private void SetLeapData(int cmd,int cmd2, int M1, int M2, int M3, int M4)
        {
            byte highbyte = 0;
            byte lowbyte = 0;

            Leap_motion[2] = (byte)cmd;
            Leap_motion[3] = (byte)cmd2;


            GetLH_BYTES(ref highbyte, ref lowbyte, M1);
            Leap_motion[4] = highbyte;
            Leap_motion[5] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, M2);
            Leap_motion[6] = highbyte;
            Leap_motion[7] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, M3);
            Leap_motion[8] = highbyte;
            Leap_motion[9] = lowbyte;

            GetLH_BYTES(ref highbyte, ref lowbyte, M4);
            Leap_motion[10] = highbyte;
            Leap_motion[11] = lowbyte;
        }

        private void create_server_btn_Click(object sender, EventArgs e)
        {
            pTcpClient = new TcpServer();
            pTcpClient.CreateServer(8080);
            MessageBox.Show("Create Server! " + ipAd + " : " + "8080");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPEINVALID);
            controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
            controller.SetPolicy(Controller.PolicyFlag.POLICY_IMAGES);
            //  _listener = new Listener();
            // controller.AddListener(_listener);
            pthread = new Thread(process);
            pthread.Start();
        }

        List<Rectangle> faces = new List<Rectangle>(); //사각형 위치를 나타내는걸 저장
        /*
        public void calculating()
        {

            Frame frame = controller.Frame();

            if (frame.Hands.Rightmost.GrabStrength == 1.0) // grabbed
            {
                if(wristyval<50)
                {
                    wristyval = 50;
                }
                /*wristxval;
                wristyval;
                wristzval;
            }
        }*/



        public LeapZephyros()
        {
            InitializeComponent();
            Text = " Leap Motion With Zephyros";
        }


        ///////////////////////////////////////////////////////////////////
        ////////////////////// Processing Thread //////////////////////////
        ///////////////////////////////////////////////////////////////////
        Leap.Image imageLA;
        Leap.Image imageRA;
        Bitmap bitmapLA;
        public Bitmap ConvertImg2BitmapLeapmotion(Leap.Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            ColorPalette grayscale = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                grayscale.Entries[i] = Color.FromArgb((int)255, i, i, i);
            }
            bitmap.Palette = grayscale;
            Rectangle lockArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(lockArea, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte[] rawImageData = image.Data;
            System.Runtime.InteropServices.Marshal.Copy(rawImageData, 0, bitmapData.Scan0, image.Width * image.Height);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
        public void process()
        {
            int Counter = 0;
            while (true)
            {
               // Thread.Sleep(10);
                try
                {
                   // Thread.Sleep(50);
                    Frame frame = controller.Frame();
                    flag_right = false;
                    flag_left = false;
                    bitmapLA = null;
                    if (!controller.Images.IsEmpty)
                    {
                        imageLA = controller.Images[0];
                        if (imageLA.IsValid)
                        {
                            bitmapLA = ConvertImg2BitmapLeapmotion(imageLA);
                            this.Invoke((MethodInvoker)delegate
                            {
                                pictureBox2.Image = bitmapLA;

                            });
                        }
                        else
                        {
                            bitmapLA = null;
                        }
                    }
                        GestureList gestures = frame.Gestures();

                     foreach (Gesture gesture in gestures)
                     {
                         switch (gesture.Type)
                         {
                             case Gesture.GestureType.TYPE_CIRCLE:
                                // CircleGesture circle = new CircleGesture(gesture);
                                 Counter++;
                                 this.Invoke((MethodInvoker)delegate
                                 {
                                     this.txt_circle.Text = "index"+Counter;

                                 });
                                 break;
                         }
                     }
                     
                    if (!frame.Hands.IsEmpty)
                    {
                        if (frame.Hands.Rightmost.GrabStrength == 1.0)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.txt_grab.Text = "grabbed";

                            });
                        }
                        else if (frame.Hands.Rightmost.GrabStrength == 0.0)
                        {
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.txt_grab.Text = "not grabbed";

                            });

                        }

                        

                         
                 
                        if (frame.Hands.Rightmost.IsRight)
                        {
                           Vector wrist_right = frame.Hands.Rightmost.WristPosition;
                            wristxval = wrist_right.x;
                            wristyval = wrist_right.y;
                            wristzval = wrist_right.z;
                            if (wristxval < -90)
                            {
                                wristxval = -90;
                            }
                            if (wristxval > 90)
                            {
                                wristxval = 90;
                            }
                            if (wristyval < 50)
                            {
                                wristyval = 50;
                            }
                            if(wristyval > 450)
                            {
                                wristyval = 450;
                            }
                            if(wristzval > 90)
                            {
                                wristzval = 90;
                            }
                            if(wristzval < -90)
                            {
                                wristzval = 90;
                            }

                            if(wristxval < -30 && wristxval>-80 && wristyval > 130 && wristyval < 200 && wristzval > -30 && wristzval < 40 ) // 왼쪽
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "left";

                                });
                                
                            }
                            if(wristxval > 30 && wristxval < 80 && wristyval > 130 && wristyval < 200 && wristzval > -30 && wristzval < 40)  // 오른쪽
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "right";

                                });
                                
                            }

                            if(wristyval < 130 && wristyval > 50 && wristzval > -30 && wristzval < 40 && wristxval > -30 && wristxval < 30)  // 아래
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "down";

                                });
                                
                            }
                            if (wristyval > 200 && wristyval <300 && wristzval > -30 && wristzval < 40 && wristxval > -30 && wristxval < 30)  // 위
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "up";

                                });
                            }

                            if (wristzval < -30 && wristzval > -90 && wristyval > 130 && wristyval < 200 && wristxval > -30 && wristxval < 30)
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "forward";

                                });
                                
                            } 
                            if (wristzval > 40 && wristzval < 90 && wristyval > 130 && wristyval < 200 && wristxval > -30 && wristxval < 30)
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.txt_move.Text = "backward";

                                });
                                
                            }
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.txt_wrist_X.Text = "" + wristxval;
                                this.txt_wrist_Y.Text = "" + wristyval;
                                this.txt_wrist_Z.Text = "" + wristzval;
                            });


                        }
                        if (checkBox1.Checked)
                        {
                            SetLeapData(1, 2, Convert.ToInt32(wristxval), Convert.ToInt32(wristyval), Convert.ToInt32(wristzval), Convert.ToInt32(frame.Hands.Rightmost.GrabStrength));
                            if (pTcpClient != null)
                            {

                                if (pTcpClient.flag_connect)
                                    pTcpClient.write(Leap_motion);
                            }
                        }
                    }

                }
                catch
                {
                    Console.WriteLine("--");
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        /*  public static void Detect(Mat image, String faceFileName, List<Rectangle> faces)
          {
              //Read the HaarCascade objects
              using (CascadeClassifier face = new CascadeClassifier("haarcascade_frontalface_default.xml")) //face에 harrcascade를 덮어준다.
              {
                  using (UMat ugray = new UMat())
                  {
                      CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray); //그레이변환

                      //normalizes brightness and increases contrast of the image
                      CvInvoke.EqualizeHist(ugray, ugray);//히스토그램 평활화   화소값이 한곳에 뭉치지않고 골고로 되게 해준다

                      //Detect the faces  from the gray scale image and store the locations as rectangle
                      //The first dimensional is the channel
                      //The second dimension is the index of the rectangle in the specific channel
                      Rectangle[] facesDetected = face.DetectMultiScale(ugray, 1.1, 10, new Size(20, 20)); //20 X20 윈도우 크기를 1.1 배율로 늘리면서 계속해서 얼굴검출
                                                                                                           //10은 네모 만들때 구성요소
                                                                                                           //harrcascade 연산자를 이용한다.
                                                                                                           //이렇게 얼굴 검출 을 하면 Rectangle[] facesDetected에 저장
                      faces.AddRange(facesDetected); //facedetected를 다시 faces <list>에 추가한다.
                  }
              }
          }*/

        public static void Detect(Mat image, String faceFileName, List<Rectangle> faces)
        {
            using (CascadeClassifier face = new CascadeClassifier("haarcascade_frontalface_default.xml")) //face에 harrcascade를 덮어준다.
            {
                using (UMat ugray = new UMat())
                {
                    CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray); //그레이변환

                    //normalizes brightness and increases contrast of the image
                    CvInvoke.EqualizeHist(ugray, ugray);//히스토그램 평활화   화소값이 한곳에 뭉치지않고 골고로 되게 해준다

                    //Detect the faces  from the gray scale image and store the locations as rectangle
                    //The first dimensional is the channel
                    //The second dimension is the index of the rectangle in the specific channel
                    Rectangle[] facesDetected = face.DetectMultiScale(ugray, 1.1, 10, new Size(20, 20)); //20 X20 윈도우 크기를 1.1 배율로 늘리면서 계속해서 얼굴검출
                                                                                                         //10은 네모 만들때 구성요소
                                                                                                         //harrcascade 연산자를 이용한다.
                                                                                                         //이렇게 얼굴 검출 을 하면 Rectangle[] facesDetected에 저장
                    faces.AddRange(facesDetected); //facedetected를 다시 faces <list>에 추가한다.
                }
            }
        }


        public Mat ConvertBitmapToMat(Bitmap bmp)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite,bmp.PixelFormat);

            // data = scan0 is a pointer to our memory block.
            IntPtr data = bmpData.Scan0;

            // step = stride = amount of bytes for a single line of the image
            int step = bmpData.Stride;

            // So you can try to get you Mat instance like this:
            Mat mat = new Mat(bmp.Height, bmp.Width, DepthType.Cv32F, 10, data, step);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return mat;
        }


        private void Streamvideo()
        {
            Bitmap Color_Image = null;

            Color_Image = _vStreaming.BStreaming();
            while (true)
            {


                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {

                        Mat preBitmap = ConvertBitmapToMat(Color_Image);

                        Detect(preBitmap, "haarcascade_frontalface_default.xml", faces); //얼굴검출
                    });
                }
                catch { }

            }
        }
        VideoStreaming _vStreaming;
        Thread pthreadstreaM;
        private void button2_Click(object sender, EventArgs e)
        {
            _vStreaming = new VideoStreaming();
            pthreadstreaM = new Thread(Streamvideo);
            pthreadstreaM.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pthreadstreaM.Abort();
        }
    }
   
}
