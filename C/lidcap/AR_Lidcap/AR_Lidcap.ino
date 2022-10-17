/*
 Name:		AR_Lidcap.ino
 Created:	12/2/2021 3:51:54 AM
 Author:	amitk
*/

#include "EnvDefine.h"
#include "UnoInteractor.h"

//------------------------------------AR namespace Implementations-------------------------------------------------------
using namespace UNOI;
using namespace UNO_DS;


//-----------------------------DS namespace implementations------------------------------------------------------------------------------------------------------
template<class keyDataType, class datatype>
inline Map<keyDataType, datatype>::Map()
{
	//set to 0, initially
	next = NULL;
	previous = NULL;
	index = 0;
	count = 0;
}


template<class keyDataType, class datatype>
ARR_Errors UNO_DS::Map<keyDataType, datatype>::findIndexForKey(keyDataType key, int* retIndex, datatype* retData)
{
	//locals
	Map* temp = this;

	//check through the list
	for (int i = 0; i < this->count; i++) {
		if (temp->key == key) {
			if (retIndex) *retIndex = i;
			if (retData) *retData = temp->data;
			return SUCCESS;
		}
		temp = temp->next;
	}
	return FAIL;
}



template<class keyDataType, class datatype>
ARR_Errors UNO_DS::Map<keyDataType, datatype>::findKeyForIndex(int index, keyDataType* retKey, datatype* retData)
{
	Map* temp = this;

	//check
	if (index < 0 || index >= count)
		return FAIL;

	//calculate the pointer
	for (int i = 0; i < index; i++) {
		temp = temp->next;
	}

	if (retKey) retKey = temp->key;
	if (retData) retData = temp->data;

	return SUCCESS;
}



template<class keyDataType, class datatype>
ARR_Errors Map<keyDataType, datatype>::add(keyDataType key, datatype data, int* retIndex = 0)
{
	Map* addNode = this;

	//check parameters
	if (!key ||
		!data)
		return OPTIONERROR;

	//check if the key pre exists
	if (findIndexForKey(key) == FAIL)
		return OPTIONERROR;

	//add the data if is the first, or create a node
	if (count > 0) {
		//is not the first,create
		addNode = addNode + (count - 1);
		addNode->next = new Map();
		addNode->next->previous = addNode;
		addNode->next->next = this;
		this->previous = addNode->next;
		addNode = addNode->next;
	}

	//add data and key
	addNode->data = data;
	addNode->key = key;

	//increase the count at the base node
	this->count++;

	return SUCCESS;
}

//--------------DS namespace Impelementations--------------------------------------------------------------------------------------------------




//---------------AR namespace Implementations-------------------------------------------------------------------------------------------------

ARR_Errors UNOI::UnoInteractor::initWorkspace()
{
	//add the default event functions
	if (
		defaultFunctions.add(COMM_QUIT_CONN, quitConnection) ||
		defaultFunctions.add(COMM_MK_CONN_SL, makeConnectionSerial) ||
		defaultFunctions.add(COMM_MK_CONN_BT, makeConnectionBT) ||
		defaultFunctions.add(COMM_MK_CONN_WIFI, makeConnectionWifi) ||
		defaultFunctions.add(COMM_GET_STATUS, getStatus)
		) {
		return FAIL;
	}

	return SUCCESS;
}


ARR_Errors UNOI::UnoInteractor::mainLoopCallback() //TO-DO
{
	//take input


	//judge logic


	//execute function callback

	return SUCCESS;

}


bool UNOI::UnoInteractor::getActiveStatus()
{
	return activeStatus;
}


bool UNOI::UnoInteractor::getConnectionStatus()
{
	return isConnected;
}


UNOI::BlueToothInteractor::BlueToothInteractor(short enablePin, short statePin, short rxPin, short txPin)
{
	blModule = NULL;//init all to NULLs
	activeStatus = false; //set active status to false
	isConnected = false; //set the connected status to false

	//check and set
	if (!enablePin ||
		!statePin ||
		!rxPin ||
		!txPin) return;

	this->enablePin = enablePin;
	this->statePin = statePin;
	this->rxPin = rxPin;
	this->txPin = txPin;

	//initialize the library
	if (!Init())
		this->activeStatus = true;
}



ARR_Errors UNOI::BlueToothInteractor::Init()
{
	//check and return
	if (!enablePin ||
		!statePin ||
		!rxPin ||
		!txPin) return OPTIONERROR;

	//create a new software serial port
	blModule = new SoftwareSerial(rxPin, txPin);

	//check the pointer
	if (!blModule)
		return INITERROR;

	//begin the Software Serial Connection
	blModule->begin(DEFAULT_BBAUD_RATE);

	//set the status to active
	activeStatus = true;

	return SUCCESS;
}


ARR_Errors UNOI::BlueToothInteractor::getInteraction()
{
	return SUCCESS;
}



ARR_Errors UNOI::makeConnectionBT(EventParams eventParams, void** retData)
{
	//check for passed argument
	if (!retData)
		return OPTIONERROR;

	//get the params
	EventParams* params = (EventParams*) *retData;
	
	//check the params
	if (
		params->MAGIC != _MAGIC ||
		params->com != COMM_MK_CONN_BT ||
		!params->interactor
		) {
		return OPTIONERROR;
	}

	//init the UnoInteractor
	*params->interactor = new BlueToothInteractor(PIN_BT_ENABLE, PIN_BT_STATE, PIN_BT_RX, PIN_BT_TX);
	UnoInteractor* interactor = *params->interactor;

	//check the active status of the library
	if (!interactor->getActiveStatus()) 
		return FAIL;
	
	//check the connection status
	if (interactor->getConnectionStatus())
		return FAIL;

	return SUCCESS;
}


//global
bool runLoop = false; //flag indicating to run the loop or not

//Main Interactor Obejct
UnoInteractor* interactor; //the interactor


// the setup function runs once when you press reset or power the board
void setup() {
	//initiate the Uno Interactor Workspace
	if (UnoInteractor::initWorkspace()) {
		//error handle

	}

	//begin the serial communication
	Serial.begin(DEFAULT_SBAUD_RATE);
}

// the loop function runs over and over again until power down or reset
void loop() {
	
	//loop is not running, try to connect
	if (Serial.available()) {
		//Serial command available, execute at force mode, and return
		String com = Serial.readString();

		//check to see if its a command, at this stage only a few commands work
		if ((com.c_str())[0] == '-') {
			//is a command, try to execute and exit out of the function
			Serial.println("executing force command - '" + com + "'.");

			//try to execute the command

			//find the command, at this stage onlu default functions works
			if (!defaultFunctions->findIndexForKey(com)) {
				Serial.println("ERROR : Command Not Found.");
				//key not found return from here
				return;
			}

			//command found, execute
			EventParams params;
			params.com = com;
			params.source = InteractSources::SERIALCONN;


			//if the command is from defualt functions
			//create the default function parameters
			DefaultFunctionParamters dfp();

			
			

			//create the function pointer
			EventCallBackFunction func;

			//get the function to be executed
			//defaultFunctions.findIndexForKey(com, func);

			//execute the function
			func(params, NULL);

			return;
		}

	}

}
