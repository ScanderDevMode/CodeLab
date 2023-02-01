#include <Arduino.h>


//private libs
#include <BlueAR.h>


//Globals
PBlueAR blue;




//function to print during DEBUG build only
//with some equipments for the same

//some defines for prefixes
#define prefix_NOTE ">>>>"
#define prefix_ERROR "!!!!"
#define prefix_IMP "@@@@"

void D_print(const char* msg, const char* prefix = NULL, bool newLine = false) {
#ifdef DEBUG
	if (prefix) {
		Serial.write(prefix);
		Serial.write(" ");
	}
	Serial.write(msg);
	if (newLine)
		Serial.write('\n');
#endif
}

void D_println(const char* msg, const char* prefix = NULL) {
	D_print(msg, prefix, true);
}

#ifdef DEBUG
void printSerialMonitorHelp() {
	D_print("Commands : \n");
}
#endif









void setup()
{
	//only enable serial reading when in debugging mode
	//this to debug and control from the serial monitor
#ifdef DEBUG
	Serial.begin(9600); //start the serial port communication
	D_println("Hello World from Arduino Board, PlatformIO Invaded Successfully!!!");
	printSerialMonitorHelp();
#endif

	pinMode(4, OUTPUT);
	digitalWrite(4, LOW);


	//init the BlueAR class
	//blue = new BlueAR(2, 3, 4, 5);
	//D_println((blue->isMoudleActive()) ? "Bluetooth Module Found" : "No Bluetooth Module Found", ">>>>");

	//set the module to command mode
	/*if (blue->isMoudleActive())
		D_println((blue->setCommandMode()) ? "Module Set to \'Command Mode\'" : "Module NOT Set to \'Command Mode\'", ">>>>");*/

	//blue->setCommandMode();
}



void loop()
{
		
}