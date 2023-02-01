#include <stdio.h>

#include "FireDrive.h"





using namespace _FireDrive_;


//test call back function
void onCreateCallBack(bool res) {
	if (res)
		printf("User created \n");
}


int main(int argc, char** argv) {

	//you can hide the data like this - if you are going for a dll build
	const char* fileData = "{\"project_info\":{\"project_number\":\"944286589208\",\"project_id\":\"harleys-4e431\",\"storage_bucket\":\"harleys-4e431.appspot.com\"},\"client\":[{\"client_info\":{\"mobilesdk_app_id\":\"1:944286589208:android:a38fcecfed8f888900be6d\",\"android_client_info\":{\"package_name\":\"com.ThreeDSSofts.Harleys\"}},\"oauth_client\":[{\"client_id\":\"944286589208-jktrtfp0ir11hbhhmpbqlffjp6q2abk9.apps.googleusercontent.com\",\"client_type\":3}],\"api_key\":[{\"current_key\":\"AIzaSyCduPJviqlQn1S-zbPm56Zrwt-QXL2xYCk\"}],\"services\":{\"appinvite_service\":{\"other_platform_oauth_client\":[{\"client_id\":\"944286589208-jktrtfp0ir11hbhhmpbqlffjp6q2abk9.apps.googleusercontent.com\",\"client_type\":3},{\"client_id\":\"944286589208-rl1q45qndnofe3oe0vi94e8ufhop5pas.apps.googleusercontent.com\",\"client_type\":2,\"ios_info\":{\"bundle_id\":\"com.3dsSofts.Harleys\"}}]}}}],\"configuration_version\":\"1\"}";
	FireDriveException exception;



	//FireDrive *fd = new FireDrive("", "AIzaSyCUaPyjW2gvvrXBqIGP3QoRXpsIurUAiWU", "harleys-4e431", "9442866589208");

	FireDrive* fd = new FireDrive(fileData, strlen(fileData));

	//get the json config file data
	auto fileStr = fd->getJsonConfigFileData();

	//create a new user
	fd->CreateUser("amitkrdas19@gmail.com", "am_p0qpl", exception, onCreateCallBack);



	//delete and see the FireDrive class exiting
	delete fd;
}