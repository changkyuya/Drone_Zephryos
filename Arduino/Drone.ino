#include <PID_v1.h>
#include <I2Cdev.h>
#include <MPU6050_6Axis_MotionApps20.h>
#include <helper_3dmath.h>
#include <Servo.h>
#include <Wire.h>
#include <PinChangeInt.h>
#include <TimerOne.h>


#define CMD_THROTTLE 48 // 0x30
#define CMD_CTRL 49  // 0x31
#define CMD_REST 50  // 0x32

#define CMD_PARAM_PITCH_PID  10 // 0x1
#define CMD_PARAM_ROLL_PID   11  // 0x0b
#define CMD_PARAM_YAW_PID    12 // 0x0C
#define CMD_WORK_TAKEOFF     13 // 0x0D
#define CMD_WORK_LANDING     14 // 0x0E
#define CMD_WORK_STOP        15 // 0x0F
#define CMD_PARAM_THROTTLE   16
#define CMD_PARAM_PID_FEEDBACK   17 // 0x11
#define CMD_THROTTLE_FEEDBACK 18  // 0x12
#define CMD_SERVO 19 // 0x13
#define CMD_SERVO_RUN 20 // 0x14
#define CMD_SERVO_STOP 21 // 0x15
#define CMD_SERVO_PROP 22 // 0x16
#define CMD_RECEIVER_FEEDBACK 23 // 0x17
#define INITMPU 24 // 0x18

#define WORKING_STATE_TAKEOFF 100
#define WORKING_STATE_LANDING 101
#define WORKING_STATE_STOP    102
#define WORKING_SERVO_START 103
#define WORKING_SERVO_STOP 104
#define WORKING_SERVO_PROP 105

#define motor_arm 1050


int  WORKING_STATE = WORKING_STATE_STOP;
int WORKING_STATE_SERVO = WORKING_STATE_STOP;
int CMD_VALUE = 0;

int M1,M2,M3,M4,M5,M6;

int THROTTLE_M1;
int THROTTLE_M2;
int THROTTLE_M3;
int THROTTLE_M4;
int THROTTLE_M5;
int THROTTLE_M6;
int THROTTLE_ALL;

int S1=1500;
int S2=1500;
int S3=1500;
int S4=1700;
int S5=1500;

int data[16] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
byte bytedata[16] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

boolean flag_cmd = false;
boolean flag_throttle_workingstate = false;
boolean flag_servo_workingstate = false;
boolean flag_pid_workingstate = false;

int timecnt = 0;
int cnt = 0;
int cntwork=0;

//// PID OUTPUT VALUE ///
float  roll_out, pitch_out, yaw_out;
float myoutputyaw , myoutputpitch , myoutputroll ;

//// PID INPUT VALUE ///
double  roll_in, pitch_in, yaw_in;
float myinputyaw , myinputpitch , myinputroll ;

//// SetPoint VALUE ////
double roll_setpoint = 0, pitch_setpoint = 0, yaw_setpoint = 0;


///////// SETUP P,I,D GAIN /////

double PARAM_ROLL_KP = 0.0;
double PARAM_ROLL_KI = 0.0;
double PARAM_ROLL_KD = 0.0;
double PARAM_ROLL_OUTMAX = 0.0;
double PARAM_ROLL_OUTMIN = 0.0;

double PARAM_PITCH_KP = 0.0;
double PARAM_PITCH_KI = 0.0;
double PARAM_PITCH_KD = 0.0;
double PARAM_PITCH_OUTMAX = 0.0;
double PARAM_PITCH_OUTMIN = 0.0;

double PARAM_YAW_KP = 0.0;
double PARAM_YAW_KI = 0.0;
double PARAM_YAW_KD = 0.0;
double PARAM_YAW_OUTMAX = 0.0;
double PARAM_YAW_OUTMIN = 0.0;

double JOYSTICK_YAW = 0.0;
double JOYSTICK_ROLL = 0.0;
double JOYSTICK_PITCH = 0.0;

PID PID_Roll(&myinputroll, &roll_out, &roll_setpoint, 0, 0, 0, DIRECT);
PID PID_Pitch(&myinputpitch, &pitch_out, &pitch_setpoint, 0, 0, 0, DIRECT);
PID PID_Yaw(&myinputyaw, &yaw_out, &yaw_setpoint, 0, 0, 0, DIRECT);


void serialEvent() {
  if (Serial.available()) {
    Serial.readBytes(bytedata, 16);
    for (int i = 0; i < 16; i++)
    {
      data[i] = (int)bytedata[i];
    }
    if (data[0] == 255 && data[1] == 255 && data[15] == 254)
    {
      CMD_VALUE = data[2];
      flag_cmd = true;
      updateParams();
      
    }
  }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////               MOTOR SETUP              ////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

Servo Ma, Mb, Mc, Md, Me, Mf;
Servo fullfront,fullback,centerfront,centerback,control;

inline void initMotor()
{
  
  Ma.attach(3);
  Mb.attach(5);
  Mc.attach(6);
  Md.attach(9);
  Me.attach(10);
  Mf.attach(11);
  
  fullfront.attach(8); // 
  fullback.attach(7);  // 
  centerfront.attach(4);
  centerback.attach(2);
  control.attach(12);
  
}

inline void arming()
{
  Ma.writeMicroseconds(1000);
  Mb.writeMicroseconds(1000);
  Mc.writeMicroseconds(1000);
  Md.writeMicroseconds(1000);
  Me.writeMicroseconds(1000);
  Mf.writeMicroseconds(1000);
  
  fullfront.writeMicroseconds(1500); //360 BACK
  fullback.writeMicroseconds(1500); //360 FRONT
  
  centerfront.writeMicroseconds(1500); // 360 BACK
  centerback.writeMicroseconds(1700); // 360 FRONT
  control.writeMicroseconds(1500);    // control left right
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////               MOTOR SETUP              ////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////             MPU 6050 SETUP             ////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


MPU6050 mpu;


uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer
// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
VectorFloat gravity;    // [x, y, z]            gravity vector
int32_t g[3];              // [x, y, z]            gyro vector
float gyro[3];
float ypr[3] = {0.0f, 0.0f, 0.0f};     // yaw pitch roll values
float yprLast[3] = {0.0f, 0.0f, 0.0f};
int STATE_CMD = CMD_REST;

volatile bool mpuInterrupt = true;     // indicates whether MPU interrupt pin has gone high

boolean interruptLock = false;

int timer1_counter;
String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete

void mpu_offset()
{

  
}


void initMPU() {
  Wire.begin();
  mpu.initialize();
  devStatus = mpu.dmpInitialize();
  if (devStatus == 0) {

    mpu.setDMPEnabled(true);
    attachInterrupt(0, dmpDataReady, RISING);
    mpuIntStatus = mpu.getIntStatus();
    packetSize = mpu.dmpGetFIFOPacketSize();
  }

}


inline void dmpDataReady() {
  mpuInterrupt = true;
}

void GetYawRollPitch() {

  mpuInterrupt = false;
  mpuIntStatus = mpu.getIntStatus();
  fifoCount = mpu.getFIFOCount();

  if ((mpuIntStatus & 0x10) || fifoCount >= 1024) {

    mpu.resetFIFO();

  } else if (mpuIntStatus & 0x02) {

    while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

    mpu.getFIFOBytes(fifoBuffer, packetSize);

    fifoCount -= packetSize;

    mpu.dmpGetQuaternion(&q, fifoBuffer);
    mpu.dmpGetGravity(&gravity, &q);
    mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
    mpu.dmpGetGyro(g, fifoBuffer);
    for (int i = 0; i < 3; i++) {
      gyro[i]   = (float)(g[3 - i - 1]) / 131.0 / 360.0;
    }


  }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////             MPU 6050 SETUP             ////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 




////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////             RECEIVER SETUP           //////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define DEBUG_RECEIVER

#define THROTTLE  0
#define ROLL 1
#define PITCH 2
#define YAW   3
#define TRIG 4


uint8_t rc_pins[5] = { 14, 15, 16, 17,12};
uint8_t rc_flags[5] = {1, 2, 4, 5, 8};
uint16_t rc_values[5] = {0, 0, 0, 0, 0};


volatile uint8_t rc_shared_flags;
volatile uint16_t rc_shared_values[5];
volatile uint32_t rc_shared_ts[5];

void rc_channel_change(uint8_t id) {
  if (digitalRead(rc_pins[id]) == HIGH) {
    rc_shared_ts[id] = micros();
  }
  else {
    rc_shared_values[id] = (uint16_t)(micros() - rc_shared_ts[id]);
    rc_shared_flags |= rc_flags[id];
  }
}

void rc_throttle_change()  { rc_channel_change(THROTTLE);  }
void rc_roll_change() { rc_channel_change(ROLL); }
void rc_pitch_change() { rc_channel_change(PITCH); }
void rc_yaw_change()   { rc_channel_change(YAW);   }
void rc_trig_change()   { rc_channel_change(TRIG);   }

void rc_setup_interrupts() {
  PCintPort::attachInterrupt(rc_pins[THROTTLE],  &rc_throttle_change, CHANGE);
  PCintPort::attachInterrupt(rc_pins[ROLL], &rc_roll_change, CHANGE);
  PCintPort::attachInterrupt(rc_pins[PITCH], &rc_pitch_change, CHANGE);
  PCintPort::attachInterrupt(rc_pins[YAW],   &rc_yaw_change, CHANGE);
  PCintPort::attachInterrupt(rc_pins[TRIG],   &rc_trig_change, CHANGE);
}

void rc_process_channels() {
  static uint8_t flags;
  
  if (rc_shared_flags) {
    noInterrupts();
    flags = rc_shared_flags;
    
    if (flags & rc_flags[0]) rc_values[0] = rc_shared_values[0];
    if (flags & rc_flags[1]) rc_values[1] = rc_shared_values[1];
    if (flags & rc_flags[2]) rc_values[2] = rc_shared_values[2];
    if (flags & rc_flags[3]) rc_values[3] = rc_shared_values[3];
    if (flags & rc_flags[4]) rc_values[4] = rc_shared_values[4];
    
    rc_shared_flags = 0;
    interrupts(); 
  }

  flags = 0;
}

#ifdef DEBUG_RECEIVER
void rc_print_channels() {
  static char str[64];

  sprintf(str, "Throttle: %d, Roll: %d, Pitch: %d, Yaw: %d TRIG: %d\n",
    rc_values[0], rc_values[1]-1480, rc_values[2]-1480, rc_values[3]-1480, rc_values[4]
  );
 
  Serial.println(str); 
}
#endif

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
//////////////////////////////////////////////             RECEIVER SETUP           //////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////            UPDATE VALUE      ////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

void updateThrottle()
{
  THROTTLE_M1 = (data[4] << 8) |  data[5];
  THROTTLE_M2 = (data[6] << 8) |  data[7];
  THROTTLE_M3 = (data[8] << 8) |  data[9];
  THROTTLE_M4 = (data[10] << 8) |  data[11];
  THROTTLE_M5 = (data[12] << 8) |  data[13];
  THROTTLE_M6 = THROTTLE_M5;
    Serial.println("throttle Updated");
}

void updateServoValue()
{

  S1 = (data[4] << 8) |  data[5];       // full FRONT
  S2 = (data[6] << 8) |  data[7];       // full BACK
  S3 = (data[8] << 8) |  data[9];       // CENTER FRONT
  S4 = (data[10] << 8) |  data[11];     // CENTER BACK
  S5 = (data[12] << 8) | data[13];      // Control
  
  Serial.println("servo update value");

    fullfront.writeMicroseconds(S1); //360 BACK
  fullback.writeMicroseconds(S2); //360 FRONT
  centerfront.writeMicroseconds(S3); // 360 BACK
  centerback.writeMicroseconds(S4); // 360 FRONT
  control.writeMicroseconds(S5);
     /*
    Serial.print("Servo :");
    Serial.print(" S1: ");
    Serial.print(S1);
    Serial.print(" S2: ");
    Serial.print(S2);
    Serial.print(" S3: ");
    Serial.print(S3);
    Serial.print(" S4: ");
    Serial.print(S4);
    Serial.print(" S5: ");
    Serial.println(S5);
*/
}
void updatejoystick()
{
  JOYSTICK_YAW = (data[4] << 8) |  data[5];       
  JOYSTICK_ROLL = (data[6] << 8) |  data[7];       
  JOYSTICK_PITCH = (data[8] << 8) |  data[9];  
  Serial.println("joystick update");
       
}
void updataRollValue()
{
  PARAM_ROLL_KP = valueFloat(data[3], data[4]);
  PARAM_ROLL_KI = valueFloat(data[5], data[6]);
  PARAM_ROLL_KD = valueFloat(data[7], data[8]);
  PARAM_ROLL_OUTMIN = (data[9] << 8) |  data[10];
  PARAM_ROLL_OUTMAX = (data[11] << 8) | data[12];
  roll_setpoint = (data[13] << 8) | data[14];
  if(roll_setpoint>1024)
  {
    roll_setpoint = (1024 - roll_setpoint)/100; 
  }
  else
  {
    roll_setpoint /= 100; 
  }
  PID_Roll.SetMode(AUTOMATIC);
  PID_Roll.SetOutputLimits(-PARAM_ROLL_OUTMIN, PARAM_ROLL_OUTMAX);
  PID_Roll.SetTunings(PARAM_ROLL_KP, PARAM_ROLL_KI, PARAM_ROLL_KD);
      Serial.print("ROLL");
    Serial.print(" KP: ");
    Serial.print(PARAM_ROLL_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_ROLL_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_ROLL_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_ROLL_OUTMAX);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_ROLL_OUTMIN);
    Serial.print(" SETPOINT: ");
    Serial.println(roll_setpoint);
}

void updatePitchValue()
{
  PARAM_PITCH_KP = valueFloat(data[3], data[4]);
  PARAM_PITCH_KI = valueFloat(data[5], data[6]);
  PARAM_PITCH_KD = valueFloat(data[7], data[8]);
  PARAM_PITCH_OUTMIN = (data[9] << 8) |  data[10];
  PARAM_PITCH_OUTMAX = (data[11] << 8) | data[12];

  pitch_setpoint = (data[13] << 8) | data[14];
  
  if(pitch_setpoint>1024)
  {
    pitch_setpoint = (1024 - pitch_setpoint)/100; 
  }
  else
  {
    pitch_setpoint /= 100; 
  }
  PID_Pitch.SetMode(AUTOMATIC);
  PID_Pitch.SetOutputLimits(-PARAM_PITCH_OUTMIN, PARAM_PITCH_OUTMAX);
  PID_Pitch.SetTunings(PARAM_PITCH_KP, PARAM_PITCH_KI, PARAM_PITCH_KD);
     
     Serial.print("PITCH ");
    Serial.print(" KP: ");
    Serial.print(PARAM_PITCH_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_PITCH_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_PITCH_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_PITCH_OUTMAX);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_PITCH_OUTMIN);
    Serial.print(" SETPOINT: ");
    Serial.println(pitch_setpoint);
}

void updateYawValue()
{
  PARAM_YAW_KP = valueFloat(data[3], data[4]);
  PARAM_YAW_KI = valueFloat(data[5], data[6]);
  PARAM_YAW_KD = valueFloat(data[7], data[8]);
  PARAM_YAW_OUTMIN = (data[9] << 8) |  data[10];
  PARAM_YAW_OUTMAX = (data[11] << 8) | data[12]; 
  yaw_setpoint = (data[13] << 8) | data[14];
 if(yaw_setpoint>4096)
  {
    yaw_setpoint = (4096 - yaw_setpoint)/100; 
  }
  else
  {
    yaw_setpoint /= 100; 
  }
  PID_Yaw.SetMode(AUTOMATIC);
  PID_Yaw.SetOutputLimits(-PARAM_YAW_OUTMIN, PARAM_YAW_OUTMAX);
  PID_Yaw.SetTunings(PARAM_YAW_KP, PARAM_YAW_KI, PARAM_YAW_KD);
      Serial.print("YAW");
    Serial.print(" KP: ");
    Serial.print(PARAM_YAW_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_YAW_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_YAW_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_YAW_OUTMIN);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_YAW_OUTMAX);
    Serial.print(" SETPOINT: ");
    Serial.println(yaw_setpoint);
}



double valueFloat(byte v1, byte v2)
{
  double tmp = ((double)v1 + (double)v2 / 100.0);

  return tmp;
}


inline void updatemotor()
{
  Ma.writeMicroseconds(THROTTLE_M1);
  Mb.writeMicroseconds(THROTTLE_M2);
  Mc.writeMicroseconds(THROTTLE_M3);
  Md.writeMicroseconds(THROTTLE_M4);
  Me.writeMicroseconds(THROTTLE_M5);
  Mf.writeMicroseconds(THROTTLE_M6);

}


void updatepidmotor()
{
  
  Ma.writeMicroseconds(M1);
  Mb.writeMicroseconds(M2);
  Mc.writeMicroseconds(M3);
  Md.writeMicroseconds(M4);
  Me.writeMicroseconds(M5);
  Mf.writeMicroseconds(M6);
}


inline void updateservo()
{

  fullfront.writeMicroseconds(S1);
  fullback.writeMicroseconds(S2);
  centerfront.writeMicroseconds(S3);
  centerback.writeMicroseconds(S4);
  control.writeMicroseconds(S5);
  
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////            UPDATE VALUE      ////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////       PID       /////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


void CalculatorPID()
{
  if(flag_pid_workingstate)
  {
  double throttle,roll,pitch,yaw;
  
  roll = JOYSTICK_ROLL/30;
  pitch = JOYSTICK_PITCH/30;
  yaw = JOYSTICK_YAW/30;
  
  myinputyaw = ypr[0]*10;
  myinputpitch = ypr[1]*100;
  myinputroll = ypr[2]*100;

  PID_Roll.Compute();
  PID_Pitch.Compute();
  PID_Yaw.Compute();

  M1 = THROTTLE_M1   - pitch_out - pitch - roll_out - roll - yaw_out - yaw;
  M2 = THROTTLE_M2   - pitch_out - pitch + roll_out + roll + yaw_out + yaw;
  M3 = THROTTLE_M3   + pitch_out + pitch + roll_out + roll - yaw_out + yaw;
  M4 = THROTTLE_M4   + pitch_out + pitch - roll_out - roll + yaw_out - yaw;
  M5 = THROTTLE_M5   - pitch_out/20;
  M6 = THROTTLE_M6   + pitch_out/20;
updatepidmotor();


Serial.print(myinputyaw);
Serial.print(",");
Serial.print(myinputpitch);
Serial.print(",");
Serial.println(myinputroll);
/*
Serial.print("M1: ");
Serial.print(M1);
Serial.print(",");
Serial.print("M2: ");
Serial.print(M2);
Serial.print(",");
Serial.print("M3: ");
Serial.print(M3);
Serial.print(",");
Serial.print("M4: ");
Serial.print(M4);
Serial.print(",");
Serial.print("yaw: ");
Serial.print(myinputyaw);
Serial.print(",");
/*
Serial.print(" pitch: ");
Serial.print(pitch);
Serial.print(" roll: ");
Serial.print(roll);
Serial.print(" yaw: ");
Serial.println(yaw);
*/
  }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////       PID       /////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////      COMMAND LINE      //////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

void updateParams()
{
  switch (CMD_VALUE)
  {
    case CMD_PARAM_ROLL_PID:
      updataRollValue();
      Serial.println("CMD_PARAM_ROLL_PID");
      break;

    case CMD_PARAM_PITCH_PID:
      updatePitchValue();
      Serial.println("CMD_PARAM_PITCH_PID");
      break;

    case CMD_PARAM_YAW_PID:
      updateYawValue();
        Serial.println("CMD_PARAM_YAW_PID");
      break;

    case CMD_THROTTLE:
      throttle_control(data[3]);
       Serial.println("CMD_THROTTLE");
      break;

    case CMD_WORK_TAKEOFF:
      flag_throttle_workingstate = true;
      flag_pid_workingstate = true;
      WORKING_STATE = WORKING_STATE_TAKEOFF;
      //workingstate(data[3]);
     // Serial.println("CMD_WORK_TAKEOFF");
      break;

    case CMD_WORK_STOP:
      M1=0;
      M2=0;
      M3=0;
      M4=0;
      M5=0;
      M6=0;
          WORKING_STATE = WORKING_STATE_STOP;
      //workingstate(data[3]);
      
      Serial.println("CMD_WORK_STOP");
      break;

    case CMD_WORK_LANDING:
      WORKING_STATE = WORKING_STATE_LANDING;
      Serial.println("CMD_WORK_LANDING");
      flag_pid_workingstate = false;
      pitch_out =0;
      roll_out =0;
      yaw_out =0;
  myinputyaw = 0;
  myinputpitch = 0;
  myinputroll = 0;
 
  PID_Roll.Compute();
  PID_Pitch.Compute();
  PID_Yaw.Compute();
      break;
    case CMD_PARAM_PID_FEEDBACK :
      feedback_params(data[3]);
      break;

    case CMD_THROTTLE_FEEDBACK :
      feedback_params(data[3]);
      break;

    case CMD_RECEIVER_FEEDBACK :
      feedback_params(data[3]);
      break;

    case CMD_SERVO :
     throttle_control(data[3]);
      
    //  WORKING_STATE_SERVO = WORKING_SERVO_START;
    //  Serial.println("update servo");
      break;
      
    case CMD_SERVO_RUN : 
    
      flag_servo_workingstate = true;
      
   //   Serial.println("run servo");
        break;

    case CMD_SERVO_STOP :
     flag_servo_workingstate = false;
     
     WORKING_STATE_SERVO = WORKING_SERVO_STOP;
     
      Serial.println("stop servo");
     break;

     
     case CMD_SERVO_PROP :
     flag_servo_workingstate = true;
     WORKING_STATE_SERVO = WORKING_SERVO_PROP;
     break;

     case INITMPU :
     initMPU();      
      M1=0;
      M2=0;
      M3=0;
      M4=0;
     break;
  }

}

void workingstateF()
{
  switch (WORKING_STATE)
  {
    case WORKING_STATE_TAKEOFF:
    //   Serial.println("WORKING_STATE_TAKEOFF2");
      CalculatorPID();
      break;
      
    case WORKING_STATE_LANDING:
     //  Serial.println("WORKING_STATE_LANDING");
      break;
      
    case WORKING_STATE_STOP:
    
    updatepidmotor();
     // Serial.print("WORKING_STATE_STOP");
      flag_throttle_workingstate = false;
      break;
  }
}
void workingstateS()
{
    switch (WORKING_STATE_SERVO)
    {
    case WORKING_SERVO_START:
    
     updateservo();
     
     
   //  Serial.println("working servo start");
     break;

     case WORKING_SERVO_STOP:
     
      flag_servo_workingstate = false;

     //   Serial.println("working servo stop");
     break;

     case WORKING_SERVO_PROP:
  
  Ma.writeMicroseconds(0);
  Mb.writeMicroseconds(0);
  Mc.writeMicroseconds(0);
  Md.writeMicroseconds(0);
  Me.writeMicroseconds(THROTTLE_M5);
  Mf.writeMicroseconds(THROTTLE_M6);
break;
    }
  }


void throttle_control(int index)
{

if (index == 0xa0)
  {
    updateThrottle();
    
  Ma.writeMicroseconds(THROTTLE_M1);
  Mb.writeMicroseconds(THROTTLE_M2);
  Mc.writeMicroseconds(THROTTLE_M3);
  Md.writeMicroseconds(THROTTLE_M4);
  Me.writeMicroseconds(THROTTLE_M5);
  Mf.writeMicroseconds(THROTTLE_M6);
  Serial.println("all motor throttle");
  
  }
  else if (index == 0xa1)
  {
    updateThrottle();
    
  Ma.writeMicroseconds(0);
  Mb.writeMicroseconds(0);
  Mc.writeMicroseconds(0);
  Md.writeMicroseconds(0);
  Me.writeMicroseconds(THROTTLE_M5);
  Mf.writeMicroseconds(THROTTLE_M6);
  Serial.println("center motor throttle");
  }
  else if (index == 0xa2)
  {
    
  Ma.writeMicroseconds(1000);
  Mb.writeMicroseconds(1000);
  Mc.writeMicroseconds(1000);
  Md.writeMicroseconds(1000);
  Me.writeMicroseconds(1000);
  Mf.writeMicroseconds(1000);
  
THROTTLE_M1=1000;
THROTTLE_M2=1000;
THROTTLE_M3=1000;
THROTTLE_M4=1000;
THROTTLE_M5=1000;
THROTTLE_M6=1000;
  
  Serial.println("center motor stop");

  }
  else if (index == 0xa3)
  {
    updateServoValue();
    

  
  }
  else if (index == 0xa4)
  {
    updatejoystick();
    
  }
}


void feedback_params(int index)
{
  if (index == 0xC0)
  {
    
    Serial.print("PITCH ");
    Serial.print(" KP: ");
    Serial.print(PARAM_PITCH_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_PITCH_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_PITCH_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_PITCH_OUTMAX);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_PITCH_OUTMIN);
    Serial.print(" SETPOINT: ");
    Serial.println(pitch_setpoint);

  }
  else if (index == 0xC1)
  {
    Serial.print("ROLL");
    Serial.print(" KP: ");
    Serial.print(PARAM_ROLL_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_ROLL_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_ROLL_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_ROLL_OUTMAX);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_ROLL_OUTMIN);
    Serial.print(" SETPOINT: ");
    Serial.println(roll_setpoint);
  }
  else if (index == 0xC2)
  {
    Serial.print("YAW");
    Serial.print(" KP: ");
    Serial.print(PARAM_YAW_KP);
    Serial.print(" KI: ");
    Serial.print(PARAM_YAW_KI);
    Serial.print(" KD: ");
    Serial.print(PARAM_YAW_KD);
    Serial.print(" OUTMAX: ");
    Serial.print(PARAM_YAW_OUTMIN);
    Serial.print(" OUTMIN: ");
    Serial.print(PARAM_YAW_OUTMAX);
    Serial.print(" SETPOINT: ");
    Serial.println(yaw_setpoint);
  }
  else if (index == 0xC3)
  {
    
  Serial.print("Throttle :");
  Serial.print(rc_values[0]);
  Serial.print(" Roll : ");
  Serial.print(rc_values[1]);
  Serial.print(" Pitch : ");
  Serial.print(rc_values[2]);
  Serial.print(" Yaw : ");
  Serial.println(rc_values[3]);
  }
  else if (index == 0xD0)
  {
    
    Serial.print("Servo :");
    Serial.print(" S1: ");
    Serial.print(S1);
    Serial.print(" S2: ");
    Serial.print(S2);
    Serial.print(" S3: ");
    Serial.print(S3);
    Serial.print(" S4: ");
    Serial.print(S4);
    Serial.print(" S5: ");
    Serial.println(S5);
  
  }
  
  else if (index == 0xD1)
  {

    Serial.print("Throttle");
    Serial.print(" M1: ");
    Serial.print(M1);
    Serial.print(" M2: ");
    Serial.print(M2);
    Serial.print(" M3: ");
    Serial.print(M3);
    Serial.print(" M4: ");
    Serial.print(M4);
    Serial.print(" M5: ");
    Serial.print(M5);
    Serial.print(" M6: ");
    Serial.println(M6);
  }

}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////      COMMAND LINE      //////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
///////////////////////////////////////////////////////     SETUP     ////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

void setup() {
  // initialize serial:
  Serial.begin(115200);

  
  rc_setup_interrupts();
  initMotor();
  initMPU();
  
  Ma.writeMicroseconds(1000);
  Mb.writeMicroseconds(1000);
  Mc.writeMicroseconds(1000);
  Md.writeMicroseconds(1000);
  Me.writeMicroseconds(1000);
  Mf.writeMicroseconds(1000);

  
  fullfront.writeMicroseconds(1500); //360 BACK
  fullback.writeMicroseconds(1500); //360 FRONT
  
  centerfront.writeMicroseconds(1500); // 360 BACK
  centerback.writeMicroseconds(1700); // 360 FRONT

  control.writeMicroseconds(1500);


}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
///////////////////////////////////////////////////////     SETUP     ////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////    LOOP     /////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


void loop() {

//rc_process_channels();
GetYawRollPitch();
//rc_print_channels();
  /*
  if(timecnt++>10)
    {
      sensorDataReq();
      timecnt=0;
    }
*/
/*
 if(rc_values[4] > 2000)
 {
  Ma.writeMicroseconds(0);
  Mb.writeMicroseconds(0);
  Mc.writeMicroseconds(0);
  Md.writeMicroseconds(0);
  Me.writeMicroseconds(0);
  Mf.writeMicroseconds(0);
 }*/
 if (flag_throttle_workingstate)
  {
    
        workingstateF();
    
  }
  if (flag_servo_workingstate)
  {
    
        workingstateS();
    
  }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
////////////////////////////////////////////////////////    LOOP     /////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 















void sensorDataReq()
{
/*
  Serial.print("Throttle :");
  Serial.print(rc_values[0]);
  Serial.print(" Roll : ");
  Serial.print(rc_values[1]);
  Serial.print(" Pitch : ");
  Serial.print(rc_values[2]);
  Serial.print(" Yaw : ");
  Serial.println(rc_values[3]);
  
  Serial.print("");
  Serial.print(" Sensor: ");
  Serial.print(" Roll: ");
  Serial.print(ypr[2]*100);
  Serial.print(" Pitch: ");
  Serial.print(ypr[1]*100);
  Serial.print(" Yaw: ");
  Serial.println(ypr[0]*100);
  
    Serial.write((byte)gyro[0] * 100); //x
    Serial.write((byte)gyro[1] * 100); //y
    Serial.write((byte)gyro[2] * 100); //z
  */


  /*if( stringComplete)
    {

    //JoinData();
    //updatemotor();

    stringComplete=false;
    }*/

}








