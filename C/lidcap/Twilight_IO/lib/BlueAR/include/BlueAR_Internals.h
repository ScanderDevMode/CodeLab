#ifndef BLUEARINTERNALS_HEADER_INCLUDED
#define BLUEARINTERNALS_HEADER_INCLUDED

//internal includes
#include <Arduino.h>
#include <SoftwareSerial.h>



namespace BLUEARINTERNAL {
	//Internal Defines
	#define deref_session(x) internal_deref_BLE_Session(x)



	//internal structures

	/*
	* Internal structure used to hold the metadata of the current session.
	* A session here means the time from when the Arduino boots up till when it shutsdown
	*/
	typedef class _BLESession {
	private:

		//MAGIC INT
		static const long _magic_ = 0x83CA9C;

		//connection pins
		const int rx; //RX pin on Arduino
		const int tx; //TX pin on Arduino
		const int enable; //Enable pin on Arduino, -1 value means no enable pin
		const int state; //State pin on Arduino, -1 value means no state pin

		//SoftwareSerial port
		SoftwareSerial* bluetooth_TransmissionPort;

		//Library State to perform other tasks
		//If this is not active, then only moduleStateQuerry will work
		bool isReady;


	public:
		//Contructor
		_BLESession(const int RX_pin, const int TX_pin, const int enable_pin = -1, const int state_pin = -1);

		//Function to get the magic number from stored in the class
		long _getMagicNumber();
		
		
		
		/*
		* Module State Query
		* Function to enquire for the active state of the bluetooth module plugged in.
		* returns true if active
		* basically checks the state pin of the module
		*
		* Params :
		* waitTime - time to be waited for the module to be active (Default - 5000 MS or 5 Secs)
		* 
		* Returns :
		* True - If module active
		* False - If module not active
		*/
		bool moduleStateQuery(int waitTime = 5000);


		/*
		* Set Enable Key
		* Function to set the enable key on the bluetooth module high or low
		* 
		* Params :
		* state - HIGH / LOW, HIGH sets the module to command mode. LOW sets to communication mode
		* 
		* Returns :
		* True - If command mode set successfully
		* False - If command mode not set successfully
		*/
		bool setEnableKey(uint8_t state);


	private:
		bool initSession();

	}BLESession, * PBLESession;


	//Internal Global Functions

	//Internal function to deref the session pointer and check the integrity,
	//and then returns the casted object
	const PBLESession& internal_deref_BLE_Session(const void *_session);
}

#endif