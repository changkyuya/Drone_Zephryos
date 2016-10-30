package com.example.administrator.zephyros_ck;

import android.app.Activity;
import android.content.SharedPreferences;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.os.Handler;
import android.os.StrictMode;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.math.BigDecimal;
import java.net.ServerSocket;
import java.net.Socket;

public class PIDtune extends AppCompatActivity {


    TextView inputdata;
    EditText pitch_kp, pitch_ki, pitch_kd, roll_kp, roll_ki, roll_kd, yaw_kp, yaw_ki, yaw_kd, pitch_min, pitch_max, pitch_set, roll_min, roll_max, roll_set, yaw_min, yaw_max, yaw_set;
    public ServerSocket serverSocket;
    public Socket socket;
    public DataOutputStream writeSocket;
    public DataInputStream readSocket;
    public Handler mHandler = new Handler();
    public ConnectivityManager cManager;
    public NetworkInfo wifi;
    public View header;
    public InputStream in_stream;
    BufferedReader in;
    private boolean mRun = false;


    public static final String KEY_MY_PREFERENCE = "IP";
    public static final String KEY_MY_PREFERENCE2 = "PORT";

    public static final String KEY_PITCH_P = "PP";
    public static final String KEY_PITCH_I = "PI";
    public static final String KEY_PITCH_D = "PD";

    public static final String KEY_ROLL_P = "RP";
    public static final String KEY_ROLL_I = "RI";
    public static final String KEY_ROLL_D = "RD";

    public static final String KEY_YAW_P = "YP";
    public static final String KEY_YAW_I = "YI";
    public static final String KEY_YAW_D = "YD";


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pidtune);

        getSupportActionBar().setTitle("Zephyros - PID TUNE");

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);

        Button pitch_click = (Button) findViewById(R.id.pitch_btn);
        pitch_click.setOnClickListener(pitch_click_Listener);

        Button roll_click = (Button) findViewById(R.id.roll_btn);
        roll_click.setOnClickListener(roll_click_Listener);

        Button yaw_click = (Button) findViewById(R.id.yaw_btn);
        yaw_click.setOnClickListener(yaw_click_Listener);


        // findViewById(R.id.roll_btn).setOnClickListener(RollBtnClick);
        //    findViewById(R.id.yaw_btn).setOnClickListener(YawBtnClick);

        pitch_kp = (EditText) findViewById(R.id.pitch_kp);
        pitch_ki = (EditText) findViewById(R.id.pitch_ki);
        pitch_kd = (EditText) findViewById(R.id.pitch_kd);
        pitch_min = (EditText) findViewById(R.id.pitch_min);
        pitch_max = (EditText) findViewById(R.id.pitch_max);
        pitch_set = (EditText) findViewById(R.id.pitch_sp);

        roll_kp = (EditText) findViewById(R.id.roll_kp);
        roll_ki = (EditText) findViewById(R.id.roll_ki);
        roll_kd = (EditText) findViewById(R.id.roll_kd);
        roll_min = (EditText) findViewById(R.id.roll_min);
        roll_max = (EditText) findViewById(R.id.roll_max);
        roll_set = (EditText) findViewById(R.id.roll_sp);

        yaw_kp = (EditText) findViewById(R.id.yaw_kp);
        yaw_ki = (EditText) findViewById(R.id.yaw_ki);
        yaw_kd = (EditText) findViewById(R.id.yaw_kd);
        yaw_min = (EditText) findViewById(R.id.yaw_min);
        yaw_max = (EditText) findViewById(R.id.yaw_max);
        yaw_set = (EditText) findViewById(R.id.yaw_sp);
        inputdata = (TextView) findViewById(R.id.textView7);


        SharedPreferences ppiidd = getSharedPreferences("pids", Activity.MODE_WORLD_READABLE | MODE_WORLD_WRITEABLE);

        String pitch_pp = ppiidd.getString(KEY_PITCH_P, "");
        String pitch_ii = ppiidd.getString(KEY_PITCH_I, "");
        String pitch_dd = ppiidd.getString(KEY_PITCH_D, "");
        String roll_pp = ppiidd.getString(KEY_ROLL_P, "");
        String roll_ii = ppiidd.getString(KEY_ROLL_I, "");
        String roll_dd = ppiidd.getString(KEY_ROLL_D, "");
        String yaw_pp = ppiidd.getString(KEY_YAW_P, "");
        String yaw_ii = ppiidd.getString(KEY_YAW_I, "");
        String yaw_dd = ppiidd.getString(KEY_YAW_D, "");


        final EditText Pp = (EditText) findViewById(R.id.pitch_kp);
        Pp.setText(pitch_pp);
        final EditText Pi = (EditText) findViewById(R.id.pitch_ki);
        Pi.setText(pitch_ii);
        final EditText Pd = (EditText) findViewById(R.id.pitch_kd);
        Pd.setText(pitch_dd);

        final EditText Rp = (EditText) findViewById(R.id.roll_kp);
        Rp.setText(roll_pp);
        final EditText Ri = (EditText) findViewById(R.id.roll_ki);
        Ri.setText(roll_ii);
        final EditText Rd = (EditText) findViewById(R.id.roll_kd);
        Rd.setText(roll_dd);

        final EditText Yp = (EditText) findViewById(R.id.yaw_kp);
        Yp.setText(yaw_pp);
        final EditText Yi = (EditText) findViewById(R.id.yaw_ki);
        Yi.setText(yaw_ii);
        final EditText Yd = (EditText) findViewById(R.id.yaw_kd);
        Yd.setText(yaw_dd);


    }

    protected void onStop() {
        super.onStop();

        EditText pitch_kp = (EditText) findViewById(R.id.pitch_kp);
        EditText pitch_ki = (EditText) findViewById(R.id.pitch_ki);
        EditText pitch_kd = (EditText) findViewById(R.id.pitch_kd);
        EditText roll_kp = (EditText) findViewById(R.id.roll_kp);
        EditText roll_ki = (EditText) findViewById(R.id.roll_ki);
        EditText roll_kd = (EditText) findViewById(R.id.roll_kd);
        EditText yaw_kp = (EditText) findViewById(R.id.yaw_kp);
        EditText yaw_ki = (EditText) findViewById(R.id.yaw_ki);
        EditText yaw_kd = (EditText) findViewById(R.id.yaw_kd);


        String pitch_pp = pitch_kp.getText().toString();
        String pitch_ii = pitch_ki.getText().toString();
        String pitch_dd = pitch_kd.getText().toString();
        String roll_pp = roll_kp.getText().toString();
        String roll_ii = roll_ki.getText().toString();
        String roll_dd = roll_kd.getText().toString();
        String yaw_pp = yaw_kp.getText().toString();
        String yaw_ii = yaw_ki.getText().toString();
        String yaw_dd = yaw_kd.getText().toString();


        // 데이타를저장합니다.

        SharedPreferences ppiidd = getSharedPreferences("pids", Activity.MODE_WORLD_READABLE | MODE_WORLD_WRITEABLE);
        SharedPreferences.Editor PIDS = ppiidd.edit();
        PIDS.putString(KEY_PITCH_P, pitch_pp);
        PIDS.putString(KEY_PITCH_I, pitch_ii);
        PIDS.putString(KEY_PITCH_D, pitch_dd);
        PIDS.putString(KEY_ROLL_P, roll_pp);
        PIDS.putString(KEY_ROLL_I, roll_ii);
        PIDS.putString(KEY_ROLL_D, roll_dd);
        PIDS.putString(KEY_YAW_P, yaw_pp);
        PIDS.putString(KEY_YAW_I, yaw_ii);
        PIDS.putString(KEY_YAW_D, yaw_dd);
        PIDS.commit();
    }

/*
    Button.OnClickListener connect_click_Listener = new View.OnClickListener() {

        public void onClick(View v) {

try {

    SharedPreferences prefs = getSharedPreferences("PrefName", Activity.MODE_PRIVATE);

    String text = prefs.getString(KEY_MY_PREFERENCE, "");
    String text2 = prefs.getString(KEY_MY_PREFERENCE2, "");

    int port = Integer.parseInt(text2);
    serverSocket = new ServerSocket(port);

    final String result = "서버 포트 " + port + " 가 준비되었습니다.";
    // socket = new Socket(text, 1900);
    socket = serverSocket.accept();
    try {


        writeSocket = new DataOutputStream(socket.getOutputStream());
        readSocket = new DataInputStream(socket.getInputStream());

        mHandler.post(new Runnable() {

            @Override
            public void run() {
                // TODO Auto-generated method stub
                Toast.makeText(PIDtune.this, "연결에 성공하였습니다.", Toast.LENGTH_SHORT).show();
            }

        });

    } catch (Exception e) {

        Toast.makeText(PIDtune.this, "연결되지 않았습니다.", Toast.LENGTH_SHORT).show();
    }
    serverSocket.close();
    socket.close();
}catch (Exception e){

}
        }
        };
*/

    Button.OnClickListener disconnect_click_Listener = new View.OnClickListener() {
        public void onClick(View v) {
            try {
                if (socket != null) {
                    socket.close();
                    mHandler.post(new Runnable() {

                        @Override
                        public void run() {
                            Toast.makeText(PIDtune.this, "연결을 끊었습니다.", Toast.LENGTH_SHORT).show();

                        }
                    });

                }
            } catch (Exception e) {
                Toast.makeText(PIDtune.this, "연결이 끊기지 않았습니다.", Toast.LENGTH_SHORT).show();
            }

        }
    };


    public static double round(double d, int decimalPlace) {
        BigDecimal bd = new BigDecimal(d);
        bd = bd.setScale(decimalPlace, BigDecimal.ROUND_HALF_UP);
        return bd.doubleValue();
    }


    Button.OnClickListener pitch_click_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            //EditText내용을 가져오기
            String kp = pitch_kp.getText().toString();
            String ki = pitch_ki.getText().toString();
            String kd = pitch_kd.getText().toString();
            String outmin = pitch_min.getText().toString();
            String outmax = pitch_max.getText().toString();
            String setpoint = pitch_set.getText().toString();

            double kpp = Double.valueOf(kp);
            double kip = Double.valueOf(ki);
            double kdp = Double.valueOf(kd);
            int kminp = Integer.parseInt(outmin);
            int kmaxp = Integer.parseInt(outmax);
            int ksetp = Integer.parseInt(setpoint);



            byte[] data1 = new byte[15];

            byte PARAM_Pitch = 0x0a;

            double newp =  (kpp - (int)kpp)*100;
            double newi =  (kip - (int)kip)*100;
            double newd =  (kdp - (int)kdp)*100;


            data1[0] = (byte) 0xff;
            data1[1] = (byte) 0xff;
            data1[2] = PARAM_Pitch;
            data1[3] = (byte) kpp;
            data1[4] = (byte) newp;
            data1[5] = (byte) kip;
            data1[6] = (byte) newi;
            data1[7] = (byte) kdp;
            data1[8] = (byte) newd;
            data1[9] = (byte) ((kminp >> 8) & 0xFF);
            data1[10] = (byte) (kminp & 0xFF);
            data1[11] = (byte) ((kmaxp >> 8) & 0xFF);
            data1[12] = (byte) (kmaxp & 0xFF);
            data1[13] = (byte) ksetp;
            data1[14] = (byte) 0xfe;


            if (kp.getBytes().length <= 0 || ki.getBytes().length <= 0 || kd.getBytes().length <= 0) {

                Toast.makeText(PIDtune.this, "값을 정확히 입력하세요.", Toast.LENGTH_SHORT).show();
            } else {


                try {

                    Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();


                    OutputStream out2 = mySocket.getOutputStream();
                    DataOutputStream dos = new DataOutputStream(out2);


                    dos.write(data1);
                    dos.flush();
                    //dos.write(data);
                    /*BufferedWriter bufOut = new BufferedWriter( new OutputStreamWriter( out2 ) );
                    for(int i=0;i<15;i++) {
                        bufOut.write(data[i]);
                    }*/
                    Toast.makeText(PIDtune.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {

                    Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
                    Toast.makeText(PIDtune.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
                }


                //Toast.makeText(PIDtune.this, "Pitch: Kp"+, Toast.LENGTH_SHORT).show();

            }
        }


    };

    Button.OnClickListener roll_click_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            String kp = roll_kp.getText().toString();
            String ki = roll_ki.getText().toString();
            String kd = roll_kd.getText().toString();
            String outmin = roll_min.getText().toString();
            String outmax = roll_max.getText().toString();
            String setpoint = roll_set.getText().toString();

            double kpr = Double.valueOf(kp);
            double kir = Double.valueOf(ki);
            double kdr = Double.valueOf(kd);
            int kminp = Integer.parseInt(outmin);
            int kmaxp = Integer.parseInt(outmax);
            int ksetp = Integer.parseInt(setpoint);

            byte[] data2 = new byte[15];

            byte PARAM_roll = 0x0b;

            double newp =  (kpr - (int)kpr)*100;
            double newi =  (kir - (int)kir)*100;
            double newd =  (kdr - (int)kdr)*100;


            data2[0] = (byte) 0xff;
            data2[1] = (byte) 0xff;
            data2[2] = PARAM_roll;
            data2[3] = (byte) kpr;
            data2[4] = (byte) newp;
            data2[5] = (byte) kir;
            data2[6] = (byte) newi;
            data2[7] = (byte) kdr;
            data2[8] = (byte) newd;
            data2[9] = (byte) ((kminp >> 8) & 0xFF);
            data2[10] = (byte) (kminp & 0xFF);
            data2[11] = (byte) ((kmaxp >> 8) & 0xFF);
            data2[12] = (byte) (kmaxp & 0xFF);
            data2[13] = (byte) ksetp;
            data2[14] = (byte) 0xfe;


            if (kp.getBytes().length <= 0 || ki.getBytes().length <= 0 || kd.getBytes().length <= 0) {

                Toast.makeText(PIDtune.this, "값을 정확히 입력하세요.", Toast.LENGTH_SHORT).show();
            } else {


                Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();
                try {


                    OutputStream out2 = mySocket.getOutputStream();
                    DataOutputStream dos = new DataOutputStream(out2);


                    dos.write(data2);
                    dos.flush();

                    Toast.makeText(PIDtune.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {
                    Toast.makeText(PIDtune.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
                }
            }
        }


    };

    Button.OnClickListener yaw_click_Listener = new View.OnClickListener() {
        public void onClick(View v) {


            //EditText내용을 가져오기
            String kp = yaw_kp.getText().toString();
            String ki = yaw_ki.getText().toString();
            String kd = yaw_kd.getText().toString();
            String outmin = yaw_min.getText().toString();
            String outmax = yaw_max.getText().toString();
            String setpoint = yaw_set.getText().toString();

            double kpy = Double.valueOf(kp);
            double kiy = Double.valueOf(ki);
            double kdy = Double.valueOf(kd);
            int kminp = Integer.parseInt(outmin);
            int kmaxp = Integer.parseInt(outmax);
            int ksetp = Integer.parseInt(setpoint);

            byte[] data3 = new byte[15];
            byte PARAM_yaw = 0x0c;

            double newp =  (kpy - (int)kpy)*100;
            double newi =  (kiy - (int)kiy)*100;
            double newd =  (kdy - (int)kdy)*100;


            data3[0] = (byte) 0xff;
            data3[1] = (byte) 0xff;
            data3[2] = PARAM_yaw;
            data3[3] = (byte) kpy;
            data3[4] = (byte) newp;
            data3[5] = (byte) kiy;
            data3[6] = (byte) newi;
            data3[7] = (byte) kdy;
            data3[8] = (byte) newd;
            data3[9] = (byte) ((kminp >> 8) & 0xFF);
            data3[10] = (byte) (kminp & 0xFF);
            data3[11] = (byte) ((kmaxp >> 8) & 0xFF);
            data3[12] = (byte) (kmaxp & 0xFF);
            data3[13] = (byte) ksetp;
            data3[14] = (byte) 0xfe;

            if (kp.getBytes().length <= 0 || ki.getBytes().length <= 0 || kd.getBytes().length <= 0) {

                Toast.makeText(PIDtune.this, "값을 정확히 입력하세요.", Toast.LENGTH_SHORT).show();
            } else {

                Socket mySocket = MainActivity.SocketHolder.getInstance().getSocket();

                try {

                    OutputStream out2 = mySocket.getOutputStream();
                    DataOutputStream dos = new DataOutputStream(out2);


                    dos.write(data3);
                    dos.flush();

                    Toast.makeText(PIDtune.this, "값이 전송되었습니다.", Toast.LENGTH_SHORT).show();
                } catch (Exception e) {
                    Toast.makeText(PIDtune.this, "값이 전송되지 않았습니다.", Toast.LENGTH_SHORT).show();
                }
            }
        }


    };


}










