/*
 Name:		UnoInteract.h
 Created:	3/6/2022 5:12:42 PM
 Author:	amitk
 Editor:	http://www.visualmicro.com
*/

#ifndef _UnoInteract_h
#define _UnoInteract_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif


namespace AR {

	//uno interactor Abstarct base class
	class UnoInteractor {

	public:
		//function for initiating the UnoInteraction
		void initiateUnoInteraction();

		//function to call in the loop to handle all the other things
		void mainLoopCall();
	};
}






#endif

