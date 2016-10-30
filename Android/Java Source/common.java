package com.example.administrator.zephyros_ck;

/**
 * Created by Administrator on 2016-08-31.
 */

import android.app.Application;

/**
 * Created by Administrator on 2016-08-18.
 */
public class common extends Application {

    private String getx;
    private String gety;
    private String getx2;
    private String gety2;
    private String pitch_p;
    private String pitch_i;
    private String pitch_d;
    private String roll_p;
    private String roll_i;
    private String roll_d;
    private String yaw_p;
    private String yaw_i;
    private String yaw_d;
    private boolean joystick = false;
    private boolean pitch_bool = false;
    private boolean roll_bool = false;
    private boolean yaw_bool = false;



    static String[] value = new String[13];
    static String[] pitch = new String[3];

    public String getgetx()
    {
        return getx;
    }

    public void setgetx(String getx)
    {
        this.getx = getx;
        this.value[0] = getx+",";
    }
    ////////////////////////////////////////////
    public String getgety()
    {
        return gety;
    }

    public void setgety(String gety)
    {
        this.gety = gety;
        this.value[1] = gety+",";
    }
    /////////////////////////////////////////////
    public String getgetx2()
    {
        return getx2;
    }
    public void setgetx2(String getx2)
    {
        this.getx2 = getx2;
        this.value[2] = getx2+",";
    }

    ///////////////////////////////////////

    public String getgety2()
    {
        return gety2;
    }
    public void setgety2(String gety2)
    {
        this.gety2 = gety2;
        this.value[3] = gety2+",";
    }

    ///////////////////////////////////////
    public String getpkp()
    {
        return pitch_p;
    }

    public void setpkp(String pkp)
    {
        this.pitch_p = pkp;
        this.value[4] = pkp+",";
        this.pitch[0] = pkp;
    }

    /////////////////////////////////////////////////
    public String getpki()
    {
        return pitch_i;
    }

    public void setpki(String pki)
    {
        this.pitch_i = pki;
        this.value[5] = pki+",";
        this.pitch[1] = pki;
    }

    /////////////////////////////////////////////////    public double getpkp()
    public String getpkd()
    {
        return pitch_d;
    }

    public void setpkd(String pkd)
    {
        this.pitch_d = pkd;
        this.value[6] = pkd+",";
        this.pitch[2] = pkd;

    }

    /////////////////////////////////////////////////

    public String getrkp()
    {
        return roll_p;
    }

    public void setrkp(String rkp)
    {
        this.roll_p = rkp;
        this.value[7] = rkp+",";
    }

    /////////////////////////////////////////////////

    public String getrki()
    {
        return roll_i;
    }

    public void setrki(String rki)
    {
        this.roll_i = rki;
        this.value[8] = rki+",";
    }

    /////////////////////////////////////////////////

    public String getrkd()
    {
        return roll_d;
    }

    public void setrkd(String rkd)
    {
        this.roll_d = rkd;
        this.value[9] = rkd+",";
    }

    /////////////////////////////////////////////////

    public String getykp()
    {
        return yaw_p;
    }

    public void setykp(String ykp)
    {
        this.yaw_p = ykp;
        this.value[10] = ykp+",";
    }

    /////////////////////////////////////////////////

    public String getyki()
    {
        return yaw_i;
    }

    public void setyki(String yki)
    {
        this.yaw_i = yki;
        this.value[11] = yki+",";
    }

    /////////////////////////////////////////////////

    public String getykd()
    {
        return yaw_d;
    }

    public void setykd(String ykd)
    {
        this.yaw_d = ykd;
        this.value[12] = ykd;
    }
    //////////////////////////////////////////////
    public boolean joystick()
    {
        return joystick;
    }

    public void joystick(boolean joystick)
    {
        this.joystick = false;
    }
    //////////
    /////////////////////////////////////////////////

   public common(){

    }

    private volatile static common instance = null;
    public static common getInstance(){
        if(instance==null){
            synchronized (common.class){
                if(instance==null){
                    instance = new common();
                }
            }
        }
        return instance;
    }

}



