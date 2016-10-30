package com.example.administrator.zephyros_ck;

import android.content.Context;
import android.content.SharedPreferences;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.text.format.Formatter;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.ServerSocket;
import java.net.Socket;

public class TCPClient extends AppCompatActivity {

    private EditText et1, et2, et3;
    private TextView tv4;
    private Socket socket;
    private DataOutputStream writeSocket;
    private DataInputStream readSocket;
    private Handler mHandler = new Handler();
    private ConnectivityManager cManager;
    private NetworkInfo wifi;
    private ServerSocket serverSocket;

    boolean flagtrue = false;


    public static final String KEY_MY_PREFERENCE = "IP";
    public static final String KEY_MY_PREFERENCE2 = "PORT";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tcpclient);


        getSupportActionBar().setTitle("Zephyros - TCP Setting");


        SharedPreferences prefs = getSharedPreferences("PrefName",0);
        String text = prefs.getString(KEY_MY_PREFERENCE, "");
        String text2 = prefs.getString(KEY_MY_PREFERENCE2, "");
        final EditText edit = (EditText) findViewById(R.id.editText1);
        edit.setText(text);

        final EditText edit2 = (EditText) findViewById(R.id.editText2);
        edit2.setText(text2);

        et1 = (EditText) findViewById(R.id.editText1);
        et2 = (EditText) findViewById(R.id.editText2);
        et3 = (EditText) findViewById(R.id.editText3);

        tv4 = (TextView) findViewById(R.id.textView4);

        cManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);

        String ip = null;
        int port = 0;



    }

    protected void onStop() {
        super.onStop();
        EditText editText = (EditText) findViewById(R.id.editText1);
        EditText editText1 = (EditText) findViewById(R.id.editText2);
        String text = editText.getText().toString();
        String text2 = editText1.getText().toString();
        // 데이타를저장합니다.

        SharedPreferences prefs = getSharedPreferences("PrefName",0);
        SharedPreferences.Editor editor = prefs.edit();

        editor.putString(KEY_MY_PREFERENCE, text);
        editor.putString(KEY_MY_PREFERENCE2, text2);
        editor.commit();

    }
    @SuppressWarnings("deprecation")
    public void OnClick(View v) throws Exception {
        switch (v.getId()) {
            case R.id.button1:
                (new Connect()).start();
                break;
            case R.id.button2:
                (new Disconnect()).start();
                break;
            case R.id.button3:
                (new SetServer()).start();
                break;
            case R.id.button4:
                (new CloseServer()).start();
                break;
            case R.id.button5:
                wifi = cManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
                if (wifi.isConnected()) {
                    WifiManager wManager = (WifiManager) getSystemService(Context.WIFI_SERVICE);
                    WifiInfo info = wManager.getConnectionInfo();
                    tv4.setText("IP Address : " + Formatter.formatIpAddress(info.getIpAddress()));
                } else {
                    tv4.setText("Disconnected");
                }
                break;
            case R.id.button6:
                (new sendMessage()).start();
        }
    }

    class Connect extends Thread {
        public void run() {
            Log.d("Connect", "Run Connect");
            String ip = null;
            int port = 0;

            try {
                ip = et1.getText().toString();
                port = Integer.parseInt(et2.getText().toString());
            } catch (Exception e) {
                final String recvInput = "정확히 입력하세요!";
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }
                });
            }
            try {
                socket = new Socket(ip, port);
                writeSocket = new DataOutputStream(socket.getOutputStream());
                readSocket = new DataInputStream(socket.getInputStream());

                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast("연결에 성공하였습니다.");
                    }

                });
                (new recvSocket()).start();
            } catch (Exception e) {
                final String recvInput = "연결에 실패하였습니다.";
                Log.d("Connect", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    class Disconnect extends Thread {
        public void run() {
            try {
                if (socket != null) {
                    socket.close();
                    mHandler.post(new Runnable() {

                        @Override
                        public void run() {
                            // TODO Auto-generated method stub
                            setToast("연결이 종료되었습니다.");
                        }
                    });

                }

            } catch (Exception e) {
                final String recvInput = "연결에 실패하였습니다.";
                Log.d("Connect", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    class SetServer extends Thread {

        public void run() {
            try {
                int port = Integer.parseInt(et2.getText().toString());
                serverSocket = new ServerSocket(port);
                final String result = "서버 포트 " + port + " 가 준비되었습니다.";

                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(result);
                    }
                });

                socket = serverSocket.accept();
                writeSocket = new DataOutputStream(socket.getOutputStream());
                readSocket = new DataInputStream(socket.getInputStream());


                while (true) {
                    byte[] b = new byte[100];
                    int ac = readSocket.read(b, 0, b.length);
                    String input = new String(b, 0, b.length);
                    final String recvInput = input.trim();

                    if(ac==-1)
                        break;

                    mHandler.post(new Runnable() {

                        @Override
                        public void run() {
                            // TODO Auto-generated method stub
                            setToast(recvInput);
                        }

                    });
                }

                serverSocket.close();
                socket.close();
            } catch (Exception e) {
                final String recvInput = "서버 준비에 실패하였습니다.";
                Log.d("SetServer", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    class recvSocket extends Thread {

        public void run() {
            try {
                readSocket = new DataInputStream(socket.getInputStream());

                while (true) {
                    byte[] b = new byte[100];
                    int ac = readSocket.read(b, 0, b.length);
                    String input = new String(b, 0, b.length);
                    final String recvInput = input.trim();

                    if(ac==-1)
                        break;

                    mHandler.post(new Runnable() {

                        @Override
                        public void run() {
                            // TODO Auto-generated method stub
                            setToast(recvInput);
                        }

                    });
                }
                mHandler.post(new Runnable(){

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast("연결이 종료되었습니다.");
                    }

                });
            } catch (Exception e) {
                final String recvInput = "연결에 문제가 발생하여 종료되었습니다..";
                Log.d("SetServer", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    class CloseServer extends Thread {
        public void run() {
            try {
                if (serverSocket != null) {
                    serverSocket.close();
                    socket.close();

                    mHandler.post(new Runnable() {

                        @Override
                        public void run() {
                            // TODO Auto-generated method stub
                            setToast("서버가 종료되었습니다..");
                        }
                    });
                }
            } catch (Exception e) {
                final String recvInput = "서버 준비에 실패하였습니다.";
                Log.d("SetServer", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    class sendMessage extends Thread {
        public void run() {
            try {
                byte[] b = new byte[100];
               // b = "HelloHello, World!".getBytes();
               // writeSocket.write(b);

            } catch (Exception e) {
                final String recvInput = "메시지 전송에 실패하였습니다.";
                Log.d("SetServer", e.getMessage());
                mHandler.post(new Runnable() {

                    @Override
                    public void run() {
                        // TODO Auto-generated method stub
                        setToast(recvInput);
                    }

                });

            }

        }
    }

    void setToast(String msg) {
        Toast.makeText(this, msg, Toast.LENGTH_SHORT).show();
    }





    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private volatile static TCPClient instance = null;
    public static TCPClient getInstance(){
        if(instance==null){
            synchronized (TCPClient.class){
                if(instance==null){
                    instance = new TCPClient();
                }
            }
        }
        return instance;
    }
}
