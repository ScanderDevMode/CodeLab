#ifndef BLUEAR_HEADER_INCLUDED
#define BLUEAR_HEADER_INCLUDED


typedef class _BlueAR {
private:
	void* session;

public:
	/*
	* Constructor
	* Inits and sets the basic requirements for the trasmission
	* Params -
	* RX_pin = pin connected to the RX pin of module
	* TX_pin = pin connected to the TX pin of module
	* enable_pin = pin connected to the enable pin of module [optional]
	* state_pin = pin connected to the state pin of module [optional]
	*/
	_BlueAR(int RX_pin, int TX_pin, int enable_pin = -1, int state_pin = -1);

	/*
	* Is Module Active
	* Function to check if the module is active or not for work.
	* 
	* Returns :
	* true - if active
	* false - if not active
	*/
	bool isMoudleActive();

	/*
	* Set Command Mode
	* Sets the bluetooth module to command mode
	* 
	* Returns :
	* true - if set successfully
	*/
	bool setCommandMode();

	/*
	* Set Communication Mode
	* Sets the bluetooth module to communication mode
	*
	* Returns :
	* true - if set successfully
	*/
	bool setCommunicationMode();


}BlueAR, *PBlueAR;

#endif