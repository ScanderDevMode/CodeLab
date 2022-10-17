#ifndef _ENV_DEFINE_HEADER_
#define _ENV_DEFINE_HEADER_

/*
	Header to contain the predefined static resources.
	These can be modified per project. <Project Specific>
*/


#define _MAGIC 789563545 //MAGIC number for UNO


//Default function commands
#define COMM_QUIT_CONN "" //quit connection
#define COMM_MK_CONN_BT "" //make connection Bluetooth
#define COMM_MK_CONN_WIFI "" //make connection WIFI
#define COMM_MK_CONN_SL "" //make connection Serial
#define COMM_GET_STATUS "-" //Get Status



//Pin defines

//led pin
#define PIN_LED 13

//bluetooth Pins
#define PIN_BT_ENABLE	4
#define PIN_BT_STATE	5
#define PIN_BT_RX		2
#define PIN_BT_TX		3









#endif
