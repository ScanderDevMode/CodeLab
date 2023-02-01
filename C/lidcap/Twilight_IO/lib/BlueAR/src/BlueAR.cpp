#include <BlueAR.h>
#include <BlueAR_Internals.h>

//using namespace BLUEARINTERNAL;




//internal defines

//drs(deref session)
#define _drs(x) BLUEARINTERNAL::deref_session(x)



//Function defines for BlueARInternals-----------------------------------------------------------------------------------

const BLUEARINTERNAL::PBLESession& BLUEARINTERNAL::internal_deref_BLE_Session(const void* _session) {
	if (((BLUEARINTERNAL::PBLESession)_session)->_getMagicNumber() == 0x83CA9C)
		return ((BLUEARINTERNAL::PBLESession)_session);
	else
		return NULL;
}


BLUEARINTERNAL::_BLESession::_BLESession(const int RX_pin, const int TX_pin, const int enable_pin, const int state_pin) :
	rx(RX_pin), tx(TX_pin), enable(enable_pin), state(state_pin), bluetooth_TransmissionPort(NULL), isReady(false)
{
	if (initSession())
		isReady = true;
}


bool BLUEARINTERNAL::_BLESession::moduleStateQuery(int waitTime)
{
	while (waitTime != 0) {		
		if (digitalRead(state) == HIGH)
			return true;
		delay(1);
		waitTime--;
	}

	return false;
}

long BLUEARINTERNAL::_BLESession::_getMagicNumber()
{
	return _magic_;
}


bool BLUEARINTERNAL::_BLESession::initSession() {
	
	//init the software serial port for the transmission
	bluetooth_TransmissionPort = new SoftwareSerial(tx, rx);

	//set the modes for the enable and state pin
	pinMode(enable, OUTPUT);
	pinMode(state, INPUT);

	//check the state of the module
	if (!moduleStateQuery())
		return false;

	return true;
}


bool BLUEARINTERNAL::_BLESession::setEnableKey(uint8_t state) {
	if (isReady) {
		digitalWrite(enable, state);
		return true;
	}
	return false;
}




//Function defines for BlueAR--------------------------------------------------------------------------------------

_BlueAR::_BlueAR(int RX_pin, int TX_pin, int enable_pin, int state_pin)
{
	//init every thing here
	session = (void *)new BLUEARINTERNAL::BLESession(RX_pin, TX_pin, enable_pin, state_pin);
	return;
}


bool _BlueAR::isMoudleActive() {
	return _drs(session)->moduleStateQuery();
}


bool _BlueAR::setCommandMode() {
	if (_drs(session)->setEnableKey(HIGH))
		return true;
	return false;
}

bool _BlueAR::setCommunicationMode() {
	if (_drs(session)->setEnableKey(LOW))
		return true;
	return false;
}