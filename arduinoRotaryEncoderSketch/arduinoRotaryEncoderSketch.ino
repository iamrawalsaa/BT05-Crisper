#include <Bounce2.h>

//these pins can not be changed 2/3 are special pins
int encoderPin1 = 2;
int encoderPin2 = 3;

int BUTTON_PIN = 8;
int LANGUAGE_TOGGLE_PIN = 10;

volatile int lastEncoded = 0;
volatile long encoderValue = 0;

long lastencoderValue = 0;
long sentEncoderValue = 0;

int lastMSB = 0;
int lastLSB = 0;

Bounce debouncer = Bounce();
Bounce debouncerLanguage = Bounce();

void setup() {
  Serial.begin (9600);
  pinMode(encoderPin1, INPUT);
  pinMode(encoderPin2, INPUT);

  digitalWrite(encoderPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderPin2, HIGH); //turn pullup resistor on

  //call updateEncoder() when any high/low changed seen
  //on interrupt 0 (pin 2), or interrupt 1 (pin 3)

  attachInterrupt(0, updateEncoder, CHANGE);
  attachInterrupt(1, updateEncoder, CHANGE);

  debouncer.attach(BUTTON_PIN, INPUT_PULLUP); // Attach the debouncer to a pin with INPUT_PULLUP mode
  debouncer.interval(25);                     // Use a debounce interval of 25 milliseconds

  debouncerLanguage.attach(LANGUAGE_TOGGLE_PIN, INPUT_PULLUP);
  debouncerLanguage.interval(25);                     // Use a debounce interval of 25 milliseconds


  //pinMode(BUTTON_PIN, INPUT_PULLUP);

  Serial.flush();
  Serial.println();
  Serial.println("Rotary Encoded Launched. Version #1.05. 2023-10-19 Added language toggle");
  Serial.flush();
}

void loop(){
  // Do stuff here
  
  CheckForButtonPress();
  CheckForLanguageToggle();
//  delay(100); //just here to slow down the output, and show it will work  even during a delay
  SendRotaryEncoder();
}

void SendRotaryEncoder()
{
  //Serial.print("ROTATE|");
  if ( sentEncoderValue != encoderValue )
  {
    Serial.print("ROTATE|");
    Serial.println(encoderValue);
    Serial.flush();

    sentEncoderValue = encoderValue;
  }
}

void updateEncoder(){
  int MSB = digitalRead(encoderPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderPin2); //LSB = least significant bit

  int encoded = (MSB << 1) |LSB; //converting the 2 pin value to single number
  int sum  = (lastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderValue --;

  lastEncoded = encoded; //store this value for next time
}

void CheckForLanguageToggle()
{
  debouncerLanguage.update();

    if (debouncerLanguage.fell())
  {
    Serial.println("LANGUAGE|ENGLISH");
  }

  if (debouncerLanguage.rose())
  {
    Serial.println("LANGUAGE|HINDI");
  }
}

void CheckForButtonPress()
{
  //if ( digitalRead( BUTTON))

  debouncer.update(); // Update the Bounce instance

  // Serial.println("Button Check");

  if (debouncer.fell())
  {
    //ButtonPressed();
    Serial.println("SCISSOR|CLOSE");
  }

  if (debouncer.rose())
  {
    //ButtonReleased();
    Serial.println("SCISSOR|OPEN");
  }
}
