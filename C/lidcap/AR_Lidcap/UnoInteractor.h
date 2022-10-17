#ifndef AR_LIDCAP_HEADER_FILE_XX
#define AR_LIDCAP_HEADER_FILE_XX

//includes
#include "SoftwareSerial.h" //includes for Serial Connection with the bluetooth module 
#include "AR_Err.h" //include for the error types

//defines
#define DEFAULT_SBAUD_RATE 9600
#define DEFAULT_BBAUD_RATE 9600

namespace UNO_DS {

	template <class keyDataType, class datatype>
	class Map {
	private:
		//locals
		int count;
		Map* previous;
		Map* next;
		int index;
		datatype data;
		keyDataType key;
		
	public:
		//constructor
		Map();

		//function to add a key data pair into the map
		ARR_Errors add(keyDataType key, datatype data, int* retIndex = NULL);

		//function to find data and index by key
		ARR_Errors findIndexForKey(keyDataType key, int* retIndex = NULL, datatype* retData = NULL);

		//function to find the key and data by index
		ARR_Errors findKeyForIndex(int index, keyDataType* retKey = NULL, datatype* retData = NULL);
	};
}




namespace UNOI {
	//prototypes
	class UnoInteractor;


	//enum containing the InteractSources
	typedef enum _InteractSources {
		SERIALCONN,
		BLUETOOTHCONN,
		WIFICONN
	}InteractSources;

	//structure for return from default functions
	typedef struct _DefaultFunctionReturns {
		void* ret; //TO-DO
	}DefaultFunctionReturns;


	//stucture to contain the Default Functions Parameters
	typedef struct _DefaultFunctionParameters {
	public:
		const int MAGIC = _MAGIC;
		UnoInteractor** interactor; //interactor obejct to be operated on

		//Connection specific information
		union Connection{
			union _BTConn {
				String ssid;	 //the ssid to set for the medium.
				String password; //the password to set for the medium. [Bluetooth, WIFI]
				String baudRate; //the baud rate to be used for the Serial Connection. [Serial]
			}BTConn;
			union _WiFiConn {
				String ssid; //the ssid to set for the medium. Can be any name without space
				String password; //the password to set for the medium. [Bluetooth, WIFI] can be anything, without space
			}WiFiConn;
		}connection;

		DefaultFunctionReturns* retData; //pointer to the return data
	}DefaultFunctionParamters;


	//structure containing the event params, 
	//is passed in the callback function with the context in it
	typedef struct _EventParams {
	public:
		const int MAGIC = _MAGIC;
		String com;
		InteractSources source;
		DefaultFunctionParamters* dfp;
	}EventParams;


	//event callback function typedef
	typedef ARR_Errors(*EventCallBackFunction)(EventParams eventParams, void** retData);

	//Map to conatain the default functions
	using namespace UNO_DS;
	Map<String, EventCallBackFunction> defaultFunctions;


	//event default functions----------------------------------------------------

	//function to quit the ongoing connection, cannot happen until already connected
	ARR_Errors quitConnection(EventParams eventParams, void** retData);
	
	//function to make a new connection via Serial, cannot happen until disconneted
	//
	ARR_Errors makeConnectionSerial(EventParams eventParams, void** retData);
	
	//function to make a new connection via Bluetooth, cannot happen until disconneted
	ARR_Errors makeConnectionBT(EventParams eventParams, void** retData);
	//function to a new connection via WiFi, cannot happen until disconnected
	ARR_Errors makeConnectionWifi(EventParams eventParams, void** retData);
	//function to get the current status of the connection
	ARR_Errors getStatus(EventParams eventParams, void** retData);
	//------------------------------------------------------------------------







	//Main Uno Interactor Class
	class UnoInteractor {
	protected:
		bool isConnected; //indicates if currently is connected via a medium
		bool activeStatus; //indicates if the library is active or not

	private:
		//function to Initiate the UnoIntractor
		virtual ARR_Errors Init() = 0;

		//function to take the input
		virtual ARR_Errors getInteraction() = 0;

	public:

		//function to go in the main loop
		ARR_Errors mainLoopCallback();

		//function to get the status of the library
		bool getActiveStatus();

		//function to get the Connection Status
		bool getConnectionStatus();

		//function to initiate the UnoInteractor Workspace
		//This function must be called before starting to do anything with the UnoInteractor,
		//better to call it in the setup function before anything else.
		static ARR_Errors initWorkspace();
	};


	class BlueToothInteractor : public UnoInteractor {
	private:
		SoftwareSerial* blModule;
		short enablePin, statePin, rxPin, txPin;

		//@override
		ARR_Errors getInteraction();

	public:

		//Constructor
		BlueToothInteractor(short enablePin, short statePin, short rxPin, short txPin);

		//@override
		ARR_Errors Init();
	};

}












#endif
