package com.example.administrator.zephyros_ck;

import android.app.Activity;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.OutputStream;
import java.net.Socket;

public class FlightControl_2 extends Activity  {

    RelativeLayout layout_joystick, layout_joystick2;
    ImageView image_joystick, image_border;
    TextView textView1, textView2, textView3, textView4, textView5, textView6, textView7, textView8, textView9, textView10;
    CheckBox pidonoff_check, padonoff_check;
    public SeekBar Throttle; // SeekBar 선언
    public Socket socket;
    public DataOutputStream writeSocket;
    public DataInputStream readSocket;
    public Handler mHandler = new Handler();
    public int ServoA1_throttle = 1500;
    public int ServoA2_throttle = 1500;
    public int ServoC1_throttle = 1500;
    public int ServoC2_throttle = 1700;
    public int ServoF1_throttle = 1450;
    public int Center_throttle = 1100;
    public int Throttle_all = 1100;
    public static final String KEY_MY_PREFERENCE = "IP";


    FlightControl js;
    FlightControl2 js2;
    private int jsgetx;
    private int jsgety;
    private int js2getx;
    private int js2gety;
    public boolean pad_flag = false;


    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);


        setContentView(R.layout.activity_flight_control_2);


        Thread time_thread = new Thread(); //자식쓰레드 생성
        time_thread.start(); // 쓰레드 시작

        Throttle = (SeekBar)findViewById(R.id.throttle); // 앞의 activity_main.xml에서 정의한 SeekBar와 연결

        Button forward_click = (Button) findViewById(R.id.forward);
        forward_click.setOnClickListener(forward_Listener);

        Button backward_click = (Button) findViewById(R.id.backward);
        backward_click.setOnClickListener(backward_Listener);

        Button f_cw_click = (Button) findViewById(R.id.F_CW);
        f_cw_click.setOnClickListener(f_cw_Listener);

        Button f_ccw_click = (Button) findViewById(R.id.F_CCW);
        f_ccw_click.setOnClickListener(f_ccw_Listener);

        Button b_cw_click = (Button) findViewById(R.id.B_CW);
        b_cw_click.setOnClickListener(b_cw_Listener);

        Button b_ccw_click = (Button) findViewById(R.id.B_CCW);
        b_ccw_click.setOnClickListener(b_ccw_Listener);

        Button c_right_click = (Button) findViewById(R.id.c_right);
        c_right_click.setOnClickListener(c_right_Listener);

        Button c_left_click = (Button) findViewById(R.id.c_left);
        c_left_click.setOnClickListener(c_left_Listener);

        Button driving_left_click = (Button) findViewById(R.id.driving_left);
        driving_left_click.setOnClickListener(driving_left_Listener);

        Button driving_right_click = (Button) findViewById(R.id.driving_right);
        driving_right_click.setOnClickListener(driving_right_Listener);

        Button driving_go_click = (Button) findViewById(R.id.driving_go);
        driving_go_click.setOnClickListener(driving_go_Listener);

        Button driving_stop_click = (Button) findViewById(R.id.driving_stop);
        driving_stop_click.setOnClickListener(driving_stop_Listener);

        Button emergency_click = (Button) findViewById(R.id.emergency);
        emergency_click.setOnClickListener(emergency_Listener);


        Button runservo_click = (Button) findViewById(R.id.runservo);
        runservo_click.setOnClickListener(runservo_Listener);

        Button servo_init_click = (Button) findViewById(R.id.servo_init);
        servo_init_click.setOnClickListener(servo_init_Listener);

        Button servo_stop_click = (Button) findViewById(R.id.servo_stop);
        servo_stop_click.setOnClickListener(servo_stop_Listener);

        pidonoff_check = (CheckBox) findViewById(R.id.pidonoff);
        pidonoff_check.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {

            @Override
            public void onCheckedChanged(CompoundButton buttonView,
                                         boolean isChecked) {
                if (buttonView.getId() == R.id.pidonoff) {
                    if (isChecked) {

                        Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();

                        byte[] data = new byte[15];
                        data[0] = (byte) 0xff;
                        data[1] = (byte) 0xff;
                        data[2] = (byte) 0x0d;
                        data[3] = (byte) 0x00;
                        data[3] = (byte) 0x00;
                        data[4] = (byte) 0x00;
                        data[5] = (byte) 0x00;
                        data[6] = (byte) 0x00;
                        data[7] = (byte) 0x00;
                        data[8] = (byte) 0x00;
                        data[9] = (byte) 0x00;
                        data[10] = (byte) 0x00;
                        data[12] = (byte) 0x00;
                        data[13] = (byte) 0x00;
                        data[14] = (byte) 0xfe;

                        try {

                            OutputStream out2 = mySocket.getOutputStream();
                            DataOutputStream dos = new DataOutputStream(out2);


                            dos.write(data);
                            dos.flush();


                            //  Toast.makeText(FlightControl_2.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                        } catch (Exception e) {

                        }
                        Toast.makeText(FlightControl_2.this, "PID On", Toast.LENGTH_SHORT).show();

                        } else {

                        Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();

                        byte[] data = new byte[15];
                        data[0] = (byte) 0xff;
                        data[1] = (byte) 0xff;
                        data[2] = (byte) 0x0e;
                        data[3] = (byte) 0x00;
                        data[3] = (byte) 0x00;
                        data[4] = (byte) 0x00;
                        data[5] = (byte) 0x00;
                        data[6] = (byte) 0x00;
                        data[7] = (byte) 0x00;
                        data[8] = (byte) 0x00;
                        data[9] = (byte) 0x00;
                        data[10] = (byte) 0x00;
                        data[12] = (byte) 0x00;
                        data[13] = (byte) 0x00;
                        data[14] = (byte) 0xfe;

                        try {

                            OutputStream out2 = mySocket.getOutputStream();
                            DataOutputStream dos = new DataOutputStream(out2);


                            dos.write(data);
                            dos.flush();


                            //  Toast.makeText(FlightControl_2.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                        } catch (Exception e) {

                        }
                        Toast.makeText(FlightControl_2.this, "PID Off", Toast.LENGTH_SHORT).show();
                    }
                }
            }
        });


/*
        pidonoff = (Switch) findViewById(R.id.pidonoff);
*/

        textView1 = (TextView) findViewById(R.id.textView1);
        textView2 = (TextView) findViewById(R.id.textView2);
        textView3 = (TextView) findViewById(R.id.textView3);
        textView4 = (TextView) findViewById(R.id.textView4);
        textView5 = (TextView) findViewById(R.id.textView5);

        layout_joystick = (RelativeLayout) findViewById(R.id.layout_joystick);
        layout_joystick2 = (RelativeLayout) findViewById(R.id.layout_joystick2);

        js = new FlightControl(getApplicationContext(), layout_joystick, R.drawable.button2);
        js.setStickSize(150, 150);
        js.setLayoutSize(500, 500);
        js.setLayoutAlpha(150);
        js.setStickAlpha(100);
        js.setOffset(90);
        js.setMinimumDistance(50);

        js2 = new FlightControl2(getApplicationContext(), layout_joystick2, R.drawable.button);
        js2.setStickSize(150, 150);
        js2.setLayoutSize(500, 500);
        js2.setLayoutAlpha(150);
        js2.setStickAlpha(100);
        js2.setOffset(90);
        js2.setMinimumDistance(50);


        layout_joystick.setOnTouchListener(new OnTouchListener() {
            public boolean onTouch(View arg0, MotionEvent arg1) {


                js.drawStick(arg1);
                if (arg1.getAction() == MotionEvent.ACTION_DOWN
                        || arg1.getAction() == MotionEvent.ACTION_MOVE) {
                    /*common.getInstance().setgetx(String.valueOf(js.getX()));
                    common.getInstance().setgety(String.valueOf(js.getY()));*/
                    jsgetx = js.getX();
                    jsgety = js.getY();

                } else if (arg1.getAction() == MotionEvent.ACTION_UP) {
                    jsgetx = js.getX();
                    jsgety = js.getY();
                }

                //
                return true;
            }

        });
        layout_joystick2.setOnTouchListener(new OnTouchListener() {
            public boolean onTouch(View arg2, MotionEvent arg3) {
                js2.drawStick(arg3);
                if (arg3.getAction() == MotionEvent.ACTION_DOWN
                        || arg3.getAction() == MotionEvent.ACTION_MOVE) {

                    js2getx = js2.getX();
                    js2gety = js2.getY();
                } else if (arg3.getAction() == MotionEvent.ACTION_UP) {

                    js2getx = js2.getX();
                    js2gety = js2.getY();
                }


                   /* common.getInstance().setgetx2(String.valueOf(js2.getX()));
                    common.getInstance().setgety2(String.valueOf(js2.getY()));*/

                return true;
            }


        });


        padonoff_check = (CheckBox) findViewById(R.id.padonoff);
        padonoff_check.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {

            @Override
            public void onCheckedChanged(CompoundButton buttonView,
                                         boolean isChecked) {
                if (buttonView.getId() == R.id.padonoff) {
                    if (isChecked) {
                        pad_flag = true;

                    }

                    Toast.makeText(FlightControl_2.this, "Joystick On", Toast.LENGTH_SHORT).show();

                } else {

                    pad_flag = false;
                    Toast.makeText(FlightControl_2.this, "Joystick Off", Toast.LENGTH_SHORT).show();
                }
            }


        });

        Throttle = (SeekBar)findViewById(R.id.throttle); // 앞의 activity_main.xml에서 정의한 SeekBar와 연결



    }

    @Override
    protected void onStart() {
        super.onStart();
        Thread myThread = new Thread(new Runnable() {
            public void run() {
                while (true) {
                    try {

                        handler.sendMessage(handler.obtainMessage());
                        Thread.sleep(50);

                    } catch (Throwable t) {
                    }
                }
            }
        });

        myThread.start();
    }
    Handler handler = new Handler() {
        @Override
        public void handleMessage(Message msg) {
            updateThread();
            updatethrottle();
        }
    };

    private void updateThread() {
        Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
        if (padonoff_check.isChecked()) {
            if (pad_flag) {

                byte[] data = new byte[15];
                data[0] = (byte) 0xff;
                data[1] = (byte) 0xff;
                data[2] = (byte) 0x30;
                data[3] = (byte) 0xa4;
                data[3] = (byte) (jsgetx & 0xFF);
                data[4] = (byte) ((jsgetx >> 8) & 0xFF);
                data[5] = (byte) (js2getx & 0xFF);
                data[6] = (byte) ((js2getx >> 8) & 0xFF);
                data[7] = (byte) (js2gety & 0xFF);
                data[8] = (byte) ((js2gety >> 8) & 0xFF);
                data[9] = (byte) 0x00;
                data[10] = (byte) 0x00;
                data[12] = (byte) 0x00;
                data[13] = (byte) 0x00;
                data[14] = (byte) 0xfe;

                try {

                    OutputStream out2 = mySocket.getOutputStream();
                    DataOutputStream dos = new DataOutputStream(out2);


                    dos.write(data);
                    dos.flush();


                    //  Toast.makeText(FlightControl_2.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {

                }
            }
        }
   }
    private void updatethrottle() {
        Throttle.setMax(1100);
        Throttle.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                Throttle_all = progress+1100; // 생성된 SeekBar의 thumb(설정 부분)을 움직여 설정한 progress 값을 mSeekBarVal에 저장


                byte[] throttle = new byte[15];
                throttle[0] = (byte) 0xff;
                throttle[1] = (byte) 0xff;
                throttle[2] = (byte) 0x30;
                throttle[3] = (byte) 0xa0;
                throttle[4] = (byte) (Throttle_all & 0xFF);
                throttle[5] = (byte) ((Throttle_all >> 8) & 0xFF);
                throttle[6] = (byte) (Throttle_all & 0xFF);
                throttle[7] = (byte) ((Throttle_all >> 8) & 0xFF);
                throttle[8] = (byte) (Throttle_all & 0xFF);
                throttle[9] = (byte) ((Throttle_all >> 8) & 0xFF);
                throttle[10] = (byte) (Throttle_all & 0xFF);
                throttle[11] = (byte) ((Throttle_all >> 8) & 0xFF);
                throttle[12] = (byte) (Throttle_all & 0xFF);
                throttle[13] = (byte) ((Throttle_all >> 8) & 0xFF);
                throttle[14] = (byte) 0xfe;

                Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
                try {

                    OutputStream out2 = mySocket.getOutputStream();
                    DataOutputStream dos = new DataOutputStream(out2);

                    dos.write(throttle);
                    dos.flush();


                    //  Toast.makeText(FlightControl_2.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {

                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar Throttle) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar Throttle) {

                String numStr2 = String.valueOf(Throttle_all);
                Toast.makeText(FlightControl_2.this, numStr2, Toast.LENGTH_SHORT).show();
            }
        });


    }


    Button.OnClickListener forward_Listener = new View.OnClickListener() {
        public void onClick(View v) {

            ServoA1_throttle = 750;
            ServoA2_throttle = 2200;

            byte[] foward = new byte[15];

            byte CMD_SERVO = 0x13;

            foward[0] = (byte) 0xff;
            foward[1] = (byte) 0xff;
            foward[2] = CMD_SERVO;
            foward[3] = (byte) 0xa3;
            foward[4] = (byte) ((ServoA1_throttle >> 8) & 0xFF);
            foward[5] = (byte) (ServoA1_throttle & 0xFF);
            foward[6] = (byte) ((ServoA2_throttle >> 8) & 0xFF);
            foward[7] = (byte) (ServoA2_throttle & 0xFF);
            foward[8] = (byte) ((ServoC1_throttle >> 8) & 0xFF);
            foward[9] = (byte) (ServoC1_throttle & 0xFF);
            foward[10] = (byte) ((ServoC2_throttle >> 8) & 0xFF);
            foward[11] = (byte) (ServoC2_throttle & 0xFF);
            foward[12] = (byte) ((ServoF1_throttle >> 8) & 0xFF);
            foward[13] = (byte) (ServoF1_throttle & 0xFF);
            foward[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(foward);
                dos.flush();


            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener backward_Listener = new View.OnClickListener() {
        public void onClick(View v) {

            ServoA1_throttle = 2210;
            ServoA2_throttle = 750;

            byte[] backward = new byte[15];

            byte CMD_SERVO = 0x13;

            backward[0] = (byte) 0xff;
            backward[1] = (byte) 0xff;
            backward[2] = CMD_SERVO;
            backward[3] = (byte) 0xa3;
            backward[4] = (byte) ((ServoA1_throttle >> 8) );
            backward[5] = (byte) (ServoA1_throttle & 0xFF);
            backward[6] = (byte) ((ServoA2_throttle >> 8) );
            backward[7] = (byte) (ServoA2_throttle & 0xFF);
            backward[8] = (byte) ((ServoC1_throttle >> 8) );
            backward[9] = (byte) (ServoC1_throttle & 0xFF);
            backward[10] = (byte) ((ServoC2_throttle >> 8) );
            backward[11] = (byte) (ServoC2_throttle & 0xFF);
            backward[12] = (byte) ((ServoF1_throttle >> 8) );
            backward[13] = (byte) (ServoF1_throttle & 0xFF);
            backward[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(backward);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener f_cw_Listener = new View.OnClickListener() {
        public void onClick(View v) {

            ServoA1_throttle = 2210;


            byte[] f_cw = new byte[15];

            byte CMD_SERVO = 0x13;

            f_cw[0] = (byte) 0xff;
            f_cw[1] = (byte) 0xff;
            f_cw[2] = CMD_SERVO;
            f_cw[3] = (byte) 0xa3;
            f_cw[4] = (byte) ((ServoA1_throttle >> 8) );
            f_cw[5] = (byte) (ServoA1_throttle & 0xFF);
            f_cw[6] = (byte) ((ServoA2_throttle >> 8) );
            f_cw[7] = (byte) (ServoA2_throttle & 0xFF);
            f_cw[8] = (byte) ((ServoC1_throttle >> 8) );
            f_cw[9] = (byte) (ServoC1_throttle & 0xFF);
            f_cw[10] = (byte) ((ServoC2_throttle >> 8) );
            f_cw[11] = (byte) (ServoC2_throttle & 0xFF);
            f_cw[12] = (byte) ((ServoF1_throttle >> 8) );
            f_cw[13] = (byte) (ServoF1_throttle & 0xFF);
            f_cw[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(f_cw);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener f_ccw_Listener = new View.OnClickListener() {
        public void onClick(View v) {

            ServoA1_throttle = 750;


            byte[] f_ccw = new byte[15];

            byte CMD_SERVO = 0x13;

            f_ccw[0] = (byte) 0xff;
            f_ccw[1] = (byte) 0xff;
            f_ccw[2] = CMD_SERVO;
            f_ccw[3] = (byte) 0xa3;
            f_ccw[4] = (byte) ((ServoA1_throttle >> 8) );
            f_ccw[5] = (byte) (ServoA1_throttle & 0xFF);
            f_ccw[6] = (byte) ((ServoA2_throttle >> 8) );
            f_ccw[7] = (byte) (ServoA2_throttle & 0xFF);
            f_ccw[8] = (byte) ((ServoC1_throttle >> 8) );
            f_ccw[9] = (byte) (ServoC1_throttle & 0xFF);
            f_ccw[10] = (byte) ((ServoC2_throttle >> 8) );
            f_ccw[11] = (byte) (ServoC2_throttle & 0xFF);
            f_ccw[12] = (byte) ((ServoF1_throttle >> 8) );
            f_ccw[13] = (byte) (ServoF1_throttle & 0xFF);
            f_ccw[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(f_ccw);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }


    };

    Button.OnClickListener b_cw_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            ServoA2_throttle = 2200;

            byte[] b_cw = new byte[15];

            byte CMD_SERVO = 0x13;

            b_cw[0] = (byte) 0xff;
            b_cw[1] = (byte) 0xff;
            b_cw[2] = CMD_SERVO;
            b_cw[3] = (byte) 0xa3;
            b_cw[4] = (byte) ((ServoA1_throttle >> 8) );
            b_cw[5] = (byte) (ServoA1_throttle & 0xFF);
            b_cw[6] = (byte) ((ServoA2_throttle >> 8) );
            b_cw[7] = (byte) (ServoA2_throttle & 0xFF);
            b_cw[8] = (byte) ((ServoC1_throttle >> 8) );
            b_cw[9] = (byte) (ServoC1_throttle & 0xFF);
            b_cw[10] = (byte) ((ServoC2_throttle >> 8) );
            b_cw[11] = (byte) (ServoC2_throttle & 0xFF);
            b_cw[12] = (byte) ((ServoF1_throttle >> 8) );
            b_cw[13] = (byte) (ServoF1_throttle & 0xFF);
            b_cw[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(b_cw);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener b_ccw_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            ServoA2_throttle = 750;

            byte[] b_ccw = new byte[15];

            byte CMD_SERVO = 0x13;

            b_ccw[0] = (byte) 0xff;
            b_ccw[1] = (byte) 0xff;
            b_ccw[2] = CMD_SERVO;
            b_ccw[3] = (byte) 0xa3;
            b_ccw[4] = (byte) ((ServoA1_throttle >> 8) );
            b_ccw[5] = (byte) (ServoA1_throttle & 0xFF);
            b_ccw[6] = (byte) ((ServoA2_throttle >> 8) );
            b_ccw[7] = (byte) (ServoA2_throttle & 0xFF);
            b_ccw[8] = (byte) ((ServoC1_throttle >> 8) );
            b_ccw[9] = (byte) (ServoC1_throttle & 0xFF);
            b_ccw[10] = (byte) ((ServoC2_throttle >> 8) );
            b_ccw[11] = (byte) (ServoC2_throttle & 0xFF);
            b_ccw[12] = (byte) ((ServoF1_throttle >> 8) );
            b_ccw[13] = (byte) (ServoF1_throttle & 0xFF);
            b_ccw[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(b_ccw);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener c_right_Listener = new View.OnClickListener() {
        public void onClick(View v) {


                    ServoC1_throttle=2400;
                    ServoC2_throttle=2400;

            byte[] c_right = new byte[15];

            byte CMD_SERVO = 0x13;

            c_right[0] = (byte) 0xff;
            c_right[1] = (byte) 0xff;
            c_right[2] = CMD_SERVO;
            c_right[3] = (byte) 0xa3;
            c_right[4] = (byte) ((ServoA1_throttle >> 8) );
            c_right[5] = (byte) (ServoA1_throttle & 0xFF);
            c_right[6] = (byte) ((ServoA2_throttle >> 8) );
            c_right[7] = (byte) (ServoA2_throttle & 0xFF);
            c_right[8] = (byte) ((ServoC1_throttle >> 8) );
            c_right[9] = (byte) (ServoC1_throttle & 0xFF);
            c_right[10] = (byte) ((ServoC2_throttle >> 8) );
            c_right[11] = (byte) (ServoC2_throttle & 0xFF);
            c_right[12] = (byte) ((ServoF1_throttle >> 8) );
            c_right[13] = (byte) (ServoF1_throttle & 0xFF);
            c_right[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(c_right);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener c_left_Listener = new View.OnClickListener() {
        public void onClick(View v) {




            ServoC1_throttle=700;
            ServoC2_throttle=700;

            byte[] c_left = new byte[15];

            byte CMD_SERVO = 0x13;

            c_left[0] = (byte) 0xff;
            c_left[1] = (byte) 0xff;
            c_left[2] = CMD_SERVO;
            c_left[3] = (byte) 0xa3;
            c_left[4] = (byte) ((ServoA1_throttle >> 8) );
            c_left[5] = (byte) (ServoA1_throttle & 0xFF);
            c_left[6] = (byte) ((ServoA2_throttle >> 8) );
            c_left[7] = (byte) (ServoA2_throttle & 0xFF);
            c_left[8] = (byte) ((ServoC1_throttle >> 8) );
            c_left[9] = (byte) (ServoC1_throttle & 0xFF);
            c_left[10] = (byte) ((ServoC2_throttle >> 8) );
            c_left[11] = (byte) (ServoC2_throttle & 0xFF);
            c_left[12] = (byte) ((ServoF1_throttle >> 8) );
            c_left[13] = (byte) (ServoF1_throttle & 0xFF);
            c_left[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(c_left);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener driving_left_Listener = new View.OnClickListener() {
        public void onClick(View v) {



            ServoF1_throttle =700;


            byte[] d_left = new byte[15];

            byte CMD_SERVO = 0x13;

            d_left[0] = (byte) 0xff;
            d_left[1] = (byte) 0xff;
            d_left[2] = CMD_SERVO;
            d_left[3] = (byte) 0xa3;
            d_left[4] = (byte) ((ServoA1_throttle >> 8) );
            d_left[5] = (byte) (ServoA1_throttle & 0xFF);
            d_left[6] = (byte) ((ServoA2_throttle >> 8) );
            d_left[7] = (byte) (ServoA2_throttle & 0xFF);
            d_left[8] = (byte) ((ServoC1_throttle >> 8) );
            d_left[9] = (byte) (ServoC1_throttle & 0xFF);
            d_left[10] = (byte) ((ServoC2_throttle >> 8) );
            d_left[11] = (byte) (ServoC2_throttle & 0xFF);
            d_left[12] = (byte) ((ServoF1_throttle >> 8) );
            d_left[13] = (byte) (ServoF1_throttle & 0xFF);
            d_left[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(d_left);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener driving_right_Listener = new View.OnClickListener() {
        public void onClick(View v) {




            ServoF1_throttle =2400;

            byte[] d_right= new byte[15];

            byte CMD_SERVO = 0x13;

            d_right[0] = (byte) 0xff;
            d_right[1] = (byte) 0xff;
            d_right[2] = CMD_SERVO;
            d_right[3] = (byte) 0xa3;
            d_right[4] = (byte) ((ServoA1_throttle >> 8) );
            d_right[5] = (byte) (ServoA1_throttle & 0xFF);
            d_right[6] = (byte) ((ServoA2_throttle >> 8) );
            d_right[7] = (byte) (ServoA2_throttle & 0xFF);
            d_right[8] = (byte) ((ServoC1_throttle >> 8) );
            d_right[9] = (byte) (ServoC1_throttle & 0xFF);
            d_right[10] = (byte) ((ServoC2_throttle >> 8) );
            d_right[11] = (byte) (ServoC2_throttle & 0xFF);
            d_right[12] = (byte) ((ServoF1_throttle >> 8) );
            d_right[13] = (byte) (ServoF1_throttle & 0xFF);
            d_right[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(d_right);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener driving_go_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            Center_throttle += 50;
            String numStr2 = String.valueOf(Center_throttle);
            Toast.makeText(FlightControl_2.this, numStr2, Toast.LENGTH_SHORT).show();
            byte[] d_go = new byte[15];

            byte CMD_THROTTLE = 0x30;

            d_go[0] = (byte) 0xff;
            d_go[1] = (byte) 0xff;
            d_go[2] = CMD_THROTTLE;
            d_go[3] = (byte) 0xa1;
            d_go[4] = (byte) ((0 >> 8) );
            d_go[5] = (byte) (0 & 0xFF);
            d_go[6] = (byte) ((0 >> 8) );
            d_go[7] = (byte) (0 & 0xFF);
            d_go[8] = (byte) ((0 >> 8) );
            d_go[9] = (byte) (0 & 0xFF);
            d_go[10] = (byte) ((0 >> 8) );
            d_go[11] = (byte) (0 & 0xFF);
            d_go[12] = (byte) ((Center_throttle >> 8) );
            d_go[13] = (byte) (Center_throttle & 0xFF);
            d_go[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(d_go);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener driving_stop_Listener = new View.OnClickListener() {
        public void onClick(View v) {



            Center_throttle = 1000;

            byte[] d_stop = new byte[15];

            byte CMD_THROTTLE = 0x30;

            d_stop[0] = (byte) 0xff;
            d_stop[1] = (byte) 0xff;
            d_stop[2] = CMD_THROTTLE;
            d_stop[3] = (byte) 0xa1;
            d_stop[4] = (byte) ((0 >> 8) );
            d_stop[5] = (byte) (0 & 0xFF);
            d_stop[6] = (byte) ((0 >> 8) );
            d_stop[7] = (byte) (0 & 0xFF);
            d_stop[8] = (byte) ((0 >> 8) );
            d_stop[9] = (byte) (0 & 0xFF);
            d_stop[10] = (byte) ((0 >> 8) );
            d_stop[11] = (byte) (0 & 0xFF);
            d_stop[12] = (byte) ((Center_throttle >> 8) );
            d_stop[13] = (byte) (Center_throttle & 0xFF);
            d_stop[14] = (byte) 0xfe;



            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(d_stop);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };













    ////////////////////////////////////////////////
    ////////////////////////////////////////////////


    Button.OnClickListener servo_init_Listener = new View.OnClickListener() {
        public void onClick(View v) {





              ServoA1_throttle = 1500;
              ServoA2_throttle = 1500;
              ServoC1_throttle = 1500;
              ServoC2_throttle = 1700;
              ServoF1_throttle = 1450;

            byte[] init= new byte[15];

            byte CMD_SERVO = 0x13;

            init[0] = (byte) 0xff;
            init[1] = (byte) 0xff;
            init[2] = CMD_SERVO;
            init[3] = (byte) 0xa3;
            init[4] = (byte) ((ServoA1_throttle >> 8) );
            init[5] = (byte) (ServoA1_throttle & 0xFF);
            init[6] = (byte) ((ServoA2_throttle >> 8) );
            init[7] = (byte) (ServoA2_throttle & 0xFF);
            init[8] = (byte) ((ServoC1_throttle >> 8) );
            init[9] = (byte) (ServoC1_throttle & 0xFF);
            init[10] = (byte) ((ServoC2_throttle >> 8) );
            init[11] = (byte) (ServoC2_throttle & 0xFF);
            init[12] = (byte) ((ServoF1_throttle >> 8) );
            init[13] = (byte) (ServoF1_throttle & 0xFF);
            init[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(init);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener servo_stop_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            byte[] stopservo = new byte[15];

            byte CMD_SERVO_STOP = 0x15;

            stopservo[0] = (byte) 0xff;
            stopservo[1] = (byte) 0xff;
            stopservo[2] = CMD_SERVO_STOP;
            stopservo[3] = (byte) 0x00;
            stopservo[4] = (byte) 0x00;
            stopservo[5] = (byte) 0x00;
            stopservo[6] = (byte) 0x00;
            stopservo[7] = (byte) 0x00;
            stopservo[8] = (byte) 0x00;
            stopservo[9] = (byte) 0x00;
            stopservo[10] = (byte)0x00;
            stopservo[11] = (byte) 0x00;
            stopservo[12] = (byte) 0x00;
            stopservo[13] = (byte) 0x00;
            stopservo[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(stopservo);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

    Button.OnClickListener runservo_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            byte[] runservo = new byte[15];

            byte CMD_SERVO_RUN = 0x14;

            runservo[0] = (byte) 0xff;
            runservo[1] = (byte) 0xff;
            runservo[2] = CMD_SERVO_RUN;
            runservo[3] = (byte) 0x00;
            runservo[4] = (byte) 0x00;
            runservo[5] = (byte) 0x00;
            runservo[6] = (byte) 0x00;
            runservo[7] = (byte) 0x00;
            runservo[8] = (byte) 0x00;
            runservo[9] = (byte) 0x00;
            runservo[10] = (byte)0x00;
            runservo[11] = (byte) 0x00;
            runservo[12] = (byte) 0x00;
            runservo[13] = (byte) 0x00;
            runservo[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(runservo);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };


    Button.OnClickListener emergency_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            byte[] emergency = new byte[15];

            byte CMD_ALL_STOP = 0x15;

            emergency[0] = (byte) 0xff;
            emergency[1] = (byte) 0xff;
            emergency[2] = CMD_ALL_STOP;
            emergency[3] = (byte) 0x00;
            emergency[4] = (byte) 0x00;
            emergency[5] = (byte) 0x00;
            emergency[6] = (byte) 0x00;
            emergency[7] = (byte) 0x00;
            emergency[8] = (byte) 0x00;
            emergency[9] = (byte) 0x00;
            emergency[10] = (byte)0x00;
            emergency[11] = (byte) 0x00;
            emergency[12] = (byte) 0x00;
            emergency[13] = (byte) 0x00;
            emergency[14] = (byte) 0xfe;


            Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
            try {


                OutputStream out2 = mySocket.getOutputStream();
                DataOutputStream dos = new DataOutputStream(out2);


                dos.write(emergency);
                dos.flush();

            } catch (Exception e) {
                Toast.makeText(FlightControl_2.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
            }
        }




    };

}



