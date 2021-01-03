
#include <SoftwareSerial.h>
#include <ChainableLED.h>

SoftwareSerial mySerial(8, 7); // RX, TX
ChainableLED leds(2, 3, 1);
static String cmdReturn="";

void setup() {
  // Open serial communications and wait for port to open:
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }
  //leds.init()
  leds.setColorRGB(0, 0, 0, 0);

  // set the data rate for the SoftwareSerial port
  mySerial.begin(9600);

  cmdReturn = char(0x02);
  cmdReturn += char(0x00);
  cmdReturn += char(0x00);
  cmdReturn += char(0x01);
  cmdReturn += char(0x00);
  cmdReturn += char(0x33);
  cmdReturn += char(0x31);
  
}

void loop() { // run over and over
  static String buffer="";
  static String cmdbuf="";
  
  if (mySerial.available()) {
    int rec = mySerial.read();
   
    if(rec == 13)
      {
        Serial.print(buffer+"\r");
        buffer = "";
      } else {
        buffer+=char(rec);
      }
      if(buffer==cmdReturn)
      {
        leds.setColorRGB(0, 0, 255, 128);
        buffer = "";
      }
  }
  
  if (Serial.available()) {
    char result = char(Serial.read());
  
    if(result == '\r')
    {
      if(cmdbuf.length() == 4 && cmdbuf[0] == 'c')
      {
        // make color led change
        int c1 = cmdbuf[1];
        int c2 = cmdbuf[2];
        int c3 = cmdbuf[3];
        leds.setColorRGB(0, c1, c2, c3);
       
      }
      if(cmdbuf.length() == 1 && cmdbuf[0] == 's')
      {
        // start scanner command.
        mySerial.write(char(0x7E));
        mySerial.write(char(0x00));
        mySerial.write(char(0x08));
        mySerial.write(char(0x01));
        mySerial.write(char(0x00));
        mySerial.write(char(0x02));
        mySerial.write(char(0x01));
        mySerial.write(char(0xAB));
        mySerial.write(char(0xCD));
        leds.setColorRGB(0, 0, 0, 128);
        
      }
      cmdbuf = "";
    } else {
      cmdbuf+=result;
    }
    
  }
}
