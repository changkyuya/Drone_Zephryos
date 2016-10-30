package com.example.administrator.zephyros_ck;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewGroup.LayoutParams;

public class FlightControl2 {
    public static final int STICK_NONE = 10;
    public static final int STICK_UP = 11;
    public static final int STICK_UPRIGHT = 12;
    public static final int STICK_RIGHT = 13;
    public static final int STICK_DOWNRIGHT =14;
    public static final int STICK_DOWN = 15;
    public static final int STICK_DOWNLEFT = 16;
    public static final int STICK_LEFT = 17;
    public static final int STICK_UPLEFT = 18;

    private int STICK_ALPHA2 = 200;
    private int LAYOUT_ALPHA2 = 200;
    private int OFFSET2 = 0;

    private Context mContext2;
    private ViewGroup mLayout2;
    private LayoutParams params2;
    private int stick_width2, stick_height2;

    private int position_x2 = 0, position_y2 = 0, min_distance2 = 0;
    private float distance2 = 0, angle2 = 0;

    private DrawCanvas draw2;
    private Paint paint2;
    private Bitmap stick2;

    private boolean touch_state2 = false;

    public FlightControl2(Context context, ViewGroup layout, int stick_res_id) {
        mContext2 = context;

        stick2 = BitmapFactory.decodeResource(mContext2.getResources(), stick_res_id);

        stick_width2 = stick2.getWidth();
        stick_height2 = stick2.getHeight();

        draw2 = new DrawCanvas(mContext2);
        paint2 = new Paint();
        mLayout2 = layout;
        params2 = mLayout2.getLayoutParams();
    }

    public void drawStick(MotionEvent arg3) {
        position_x2 = (int) (arg3.getX() - (params2.width / 2));
        position_y2 = (int) (arg3.getY() - (params2.height / 2));
        distance2 = (float) Math.sqrt(Math.pow(position_x2, 2) + Math.pow(position_y2, 2));
        angle2 = (float) cal_angle(position_x2, position_y2);


        if(arg3.getAction() == MotionEvent.ACTION_DOWN) {
            if(distance2 <= (params2.width / 2) - OFFSET2) {
                draw2.position(arg3.getX(), arg3.getY());
                draw();
                touch_state2 = true;
            }
        } else if(arg3.getAction() == MotionEvent.ACTION_MOVE && touch_state2) {
            if(distance2 <= (params2.width / 2) - OFFSET2) {
                draw2.position(arg3.getX(), arg3.getY());
                draw();
            } else if(distance2 > (params2.width / 2) - OFFSET2){
                float x = (float) (Math.cos(Math.toRadians(cal_angle(position_x2, position_y2)))
                        * ((params2.width / 2) - OFFSET2));
                float y = (float) (Math.sin(Math.toRadians(cal_angle(position_x2, position_y2)))
                        * ((params2.height / 2) - OFFSET2));
                x += (params2.width / 2);
                y += (params2.height / 2);
                draw2.position(x, y);
                draw();
            }
        } else if(arg3.getAction() == MotionEvent.ACTION_UP) {
            draw2.position(250, 250);
            position_x2 =0;
            position_y2 =0;
            draw();
            touch_state2 = false;


        }

    }

    public int[] getPosition() {
        if(distance2 > min_distance2 && touch_state2) {
            return new int[] { position_x2, position_y2 };
        }
        return new int[] { 0, 0 };
    }

    public int getX() {
        if(distance2 > min_distance2 && touch_state2) {
            return position_x2;
        }
        else{
            position_x2 = 0;
        }
        return 0;
    }

    public int getY() {
        if(distance2 > min_distance2 && touch_state2) {
            return position_y2;
        }
        else{
             position_y2 =0;
        }
        return 0;
    }

    public float getAngle() {
        if(distance2 > min_distance2 && touch_state2) {
            return angle2;
        }
        return 0;
    }

    public float getDistance() {
        if(distance2 > min_distance2 && touch_state2) {
            return distance2;
        }
        return 0;
    }

    public void setMinimumDistance(int minDistance) {
        min_distance2 = minDistance;
    }

    public int getMinimumDistance() {
        return min_distance2;
    }

    public int get8Direction() {
        if(distance2 > min_distance2 && touch_state2) {
            if(angle2 >= 247.5 && angle2 < 292.5 ) {
                return STICK_UP;
            } else if(angle2 >= 292.5 && angle2 < 337.5 ) {
                return STICK_UPRIGHT;
            } else if(angle2 >= 337.5 || angle2 < 22.5 ) {
                return STICK_RIGHT;
            } else if(angle2 >= 22.5 && angle2 < 67.5 ) {
                return STICK_DOWNRIGHT;
            } else if(angle2 >= 67.5 && angle2 < 112.5 ) {
                return STICK_DOWN;
            } else if(angle2 >= 112.5 && angle2 < 157.5 ) {
                return STICK_DOWNLEFT;
            } else if(angle2 >= 157.5 && angle2 < 202.5 ) {
                return STICK_LEFT;
            } else if(angle2 >= 202.5 && angle2 < 247.5 ) {
                return STICK_UPLEFT;
            }
        } else if(distance2 <= min_distance2 && touch_state2) {
            return STICK_NONE;
        }
        return 0;
    }

    public int get4Direction() {
        if(distance2 > min_distance2 && touch_state2) {
            if(angle2 >= 225 && angle2 < 315 ) {
                return STICK_UP;
            } else if(angle2 >= 315 || angle2 < 45 ) {
                return STICK_RIGHT;
            } else if(angle2 >= 45 && angle2 < 135 ) {
                return STICK_DOWN;
            } else if(angle2 >= 135 && angle2 < 225 ) {
                return STICK_LEFT;
            }
        } else if(distance2 <= min_distance2 && touch_state2) {
            return STICK_NONE;
        }
        return 0;
    }

    public void setOffset(int offset) {
        OFFSET2 = offset;
    }

    public int getOffset() {
        return OFFSET2;
    }

    public void setStickAlpha(int alpha) {
        STICK_ALPHA2 = alpha;
        paint2.setAlpha(alpha);
    }

    public int getStickAlpha() {
        return STICK_ALPHA2;
    }

    public void setLayoutAlpha(int alpha) {
        LAYOUT_ALPHA2 = alpha;
        mLayout2.getBackground().setAlpha(alpha);
    }

    public int getLayoutAlpha() {
        return LAYOUT_ALPHA2;
    }

    public void setStickSize(int width, int height) {
        stick2 = Bitmap.createScaledBitmap(stick2, width, height, false);
        stick_width2 = stick2.getWidth();
        stick_height2 = stick2.getHeight();
    }

    public void setStickWidth(int width) {
        stick2 = Bitmap.createScaledBitmap(stick2, width, stick_height2, false);
        stick_width2 = stick2.getWidth();
    }

    public void setStickHeight(int height) {
        stick2 = Bitmap.createScaledBitmap(stick2, stick_width2, height, false);
        stick_height2 = stick2.getHeight();
    }

    public int getStickWidth() {
        return stick_width2;
    }

    public int getStickHeight() {
        return stick_height2;
    }

    public void setLayoutSize(int width, int height) {
        params2.width = width;
        params2.height = height;
    }

    public int getLayoutWidth() {
        return params2.width;
    }

    public int getLayoutHeight() {
        return params2.height;
    }

    private double cal_angle(float x, float y) {
        if(x >= 0 && y >= 0)
            return Math.toDegrees(Math.atan(y / x));
        else if(x < 0 && y >= 0)
            return Math.toDegrees(Math.atan(y / x)) + 180;
        else if(x < 0 && y < 0)
            return Math.toDegrees(Math.atan(y / x)) + 180;
        else if(x >= 0 && y < 0)
            return Math.toDegrees(Math.atan(y / x)) + 360;
        return 0;
    }

    private void draw() {
        try {
            mLayout2.removeView(draw2);
        } catch (Exception e) { }
        mLayout2.addView(draw2);
    }

    private class DrawCanvas extends View{
        float x, y;

        private DrawCanvas(Context mContext) {
            super(mContext);
        }

        public void onDraw(Canvas canvas) {
            canvas.drawBitmap(stick2, x, y, paint2);
        }

        private void position(float pos_x, float pos_y) {
            x = pos_x - (stick_width2 / 2);
            y = pos_y - (stick_height2 / 2);
        }
    }
}