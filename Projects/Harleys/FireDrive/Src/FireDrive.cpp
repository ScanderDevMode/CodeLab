/*
*	Do Not Amend this section.
*
*/


//includes
//c headers
#include <stdio.h>
#include <stdlib.h>
//cpp headers
#include <cstdarg>
#include <string>
#include <iostream>
#include <fstream>
#include <streambuf>
//firebase headers
#include "FireDrive.h"
#include "firebase/app.h"
#include "firebase/auth.h"
#include "firebase/future.h"


namespace _FireDrive_ {

	//using the firebase namespace
	using namespace firebase;


	//internal defines
#define AO_V2A(x) ((AppOptions*)x) //caster for void pointer to AppOptions Pointer
#define AP_V2A(x) ((App*)x) //caster for void pointer to App Pointer
#define CDEL(x) if(x != NULL)delete x //deletes the x if it exists




//---------------------------------------------------FIREDRIVE---------------------------------------------------------------

	FireDrive::FireDrive(const char* pathToConfigJson) {
		//read file data
		unsigned int len = 0;
		fileData = readWholeFile(pathToConfigJson, &len);
		if (fileData->length() <= 10) {
			//error in file
			std::cerr << "Error Reading file." << std::endl;
			return;
		}

		//create the appOptions object
		appOptions = (void*)new AppOptions();

		//load from the file data into appOptions
		//AppOptions::LoadFromJsonConfig(fileData->c_str(), &appOpts);
		AppOptions::LoadFromJsonConfig(fileData->c_str(), (AppOptions*)appOptions);
		if (AO_V2A(appOptions)->api_key() == NULL || AO_V2A(appOptions)->api_key() == "") {
			//error in file
			std::cerr << "Error loading from json data!" << std::endl;
			return;
		}

		//create the App instance
		app = (void*)App::Create(*AO_V2A(appOptions), "Harleys");
		if (!AP_V2A(app)) {
			//error creating the app object
			std::cerr << "Error creating the app object!" << std::endl;
			return;
		}
	}

	FireDrive::FireDrive(const char* jsonConfigContent, int len)
	{
		//store the json data
		std::string str = jsonConfigContent;
		fileData = std::make_shared<std::string>(str);
		if (fileData->length() <= 10) {
			//error in file
			std::cerr << "Error Reading file." << std::endl;
			return;
		}

		//create the appOptions object
		appOptions = (void*)new AppOptions();

		//load from the file data into appOptions
		AppOptions::LoadFromJsonConfig(fileData->c_str(), (AppOptions*)appOptions);
		if (AO_V2A(appOptions)->api_key() == NULL || AO_V2A(appOptions)->api_key() == "") {
			//error in file
			std::cerr << "Error loading from json data!" << std::endl;
			return;
		}

		//create the App instance
		app = (void*)App::Create(*AO_V2A(appOptions), "Harleys");
		if (!AP_V2A(app)) {
			//error creating the app object
			std::cerr << "Error creating the app object!" << std::endl;
			return;
		}
	}

	FireDrive::FireDrive(const FireDrive& obj) {}

	void FireDrive::operator=(const FireDrive& obj) {}

	FireDrive::~FireDrive() {
		//delete the appOptions
		CDEL(AO_V2A(appOptions));
		CDEL(AP_V2A(app));
	}

	std::shared_ptr<const std::string> FireDrive::getJsonConfigFileData() {
		return fileData;
	}

	bool FireDrive::CreateUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback) {

		//get Auth class object
		auth::Auth* _auth = auth::Auth::GetAuth(AP_V2A(app));

		//create the user
		Future<auth::User*> result = _auth->CreateUserWithEmailAndPassword(email, password);

		//pre check if failed
		if (result.status() == kFutureStatusComplete || result.status() == kFutureStatusInvalid) {
			if (result.error() == auth::kAuthErrorNone) {

				return true;
			}
			else {
				//already failed for some reason
				//handle exception
				FireDriveException fde(UNKNOWN_EXCEPTION, result.error_message());
				exception = fde;
				return false;
			}
		}
		

		//declare lamda function for the call back - only registers when pending
		auto callBFunc = [callback, &exception](const Future<auth::User*>& _result) -> void {
			std::string customessage = "";
			bool res = false;

			//store the function pointer as we are accessing it as a refference to the original which is in another scope
			_OnCompletionCallBack userCallBack = callback;

			if (_result.status() == kFutureStatusComplete) {
				//check now
				if (_result.error() == auth::kAuthErrorNone) {
					//success
					res = true;
				}
				else {
					//error
					res = false;
					customessage.append(_result.error_message());
				}
			}
			else {
				//should only enter this function if the operation is complete
				//hence error!!!!
				res = false;
				customessage.append("Callback called OUT OF ORDINARY!!!");
			}

			//prepare the exception if any
			if (res == false) {
				FireDriveException fde(UNKNOWN_EXCEPTION, customessage.c_str());
				exception = fde;
			}

			//call the callback provided by user if not NULL
			if(callback)
				userCallBack(res);
		};

		//resgister the call back function
		result.OnCompletion(callBFunc);

		return true; //return true for pending
	}

	std::shared_ptr<const std::string> FireDrive::SignInUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback) {
		
		//get Auth
		auth::Auth* _auth = auth::Auth::GetAuth(AP_V2A(app));

		//sign in
		Future<auth::User*> result = _auth->SignInWithEmailAndPassword(email, password);

		//pre check and verify
		if (result.status() == kFutureStatusComplete || result.status() == kFutureStatusInvalid) {
			if (result.error() == auth::kAuthErrorNone) {
				auth::User* user = result.result();
				return ;
			}
			else {
				//already failed for some reason
				//handle exception
				FireDriveException fde(UNKNOWN_EXCEPTION, result.error_message());
				exception = fde;
				return NULL;
			}
		}


		return NULL;
	}


	//----------------------------------------------------FIREDRIVEEXCEPTION----------------------------------------------------

	FireDriveException::FireDriveException() : customMessage(""), fullMessage(""), message(""), exception(NO_EXCEPTION) {}

	FireDriveException::FireDriveException(FireDriveExceptions exception, const char* customMessage) :
		customMessage(""), fullMessage(""), message("")
	{
		//store the messages
		this->exception = exception;

		//store the message
		switch (this->exception) {
		case NO_EXCEPTION:
			break;
		case FILE_NOT_FOUND_EXCEPTION:
			message = "File Not Found. Please check the path to file.";
			break;
		case INVALID_ARGUMENT:
			message = "Invalid Arguments Passed.";
			break;
		case NO_MEMORY:
			message = "Fatal! No more free memory.";
			break;
		default:
			message = "Fatal! Unknown Exception.";
			break;
		}

		//if custom message exists
		if (customMessage != NULL) {
			//int len = strlen(customMessage) + strlen(message) + 5;
			this->customMessage = customMessage;
			//fullMessage = (char*)malloc(len); //create full message
			//sprintf_s(fullMessage, len, "%s : %s", message, customMessage);

			fullMessage = message + " : " + customMessage;
		}
	}

	//TO-DO
	FireDriveException::FireDriveException(const char* customMessageFormat, FireDriveExceptions exception, ...) :
		customMessage(""), fullMessage(""), message("")
	{
		//store the messages
		this->exception = exception;

		//store the message
		switch (this->exception) {
		case NO_EXCEPTION:
			break;
		case FILE_NOT_FOUND_EXCEPTION:
			message = "File Not Found. Please check the path to file.";
			break;
		case INVALID_ARGUMENT:
			message = "Invalid Arguments Passed.";
			break;
		case NO_MEMORY:
			message = "Fatal! No more free memory.";
			break;
		default:
			message = "Fatal! Unknown Exception.";
			break;
		}

		//if custom message exists
		if (customMessageFormat != NULL) {
			//read the varargs and prepare the message string
			//va_list list;

			//int len = strlen(customMessage) + strlen(message) + 5;
			//this->customMessage = strdup(customMessage);
			//fullMessage = (char*)malloc(len); //create full message
			//sprintf_s(fullMessage, len, "%s : %s", message, customMessage);
		}
	}

	FireDriveException::FireDriveException(const FireDriveException& exception) :
		customMessage(NULL), fullMessage(NULL), message(NULL)
	{
		this->customMessage = exception.customMessage;
		this->exception = exception.exception;
		this->fullMessage = exception.fullMessage;
		this->message = exception.message;
	}

	void FireDriveException::operator=(const FireDriveException& obj)
	{
		this->customMessage = obj.customMessage;
		this->exception = obj.exception;
		this->fullMessage = obj.fullMessage;
		this->message = obj.message;
	}

	FireDriveException::~FireDriveException()
	{
		//get rid of the memory
		/*free(message);
		if (customMessage) free(customMessage);
		if (fullMessage) free(fullMessage);*/
	}

	std::string FireDriveException::getMessage()
	{
		return message;
	}

	std::string FireDriveException::getCustomMessage()
	{
		return customMessage;
	}

	std::string FireDriveException::getFullMessage()
	{
		return customMessage;
	}

	



	//---------------------------------------------------FIREDRIVEUTILITY-------------------------------------------------------

	std::shared_ptr<std::string> FireDriveUtility::readWholeFile(const char* pathToFile, unsigned int* len)
	{
		//open and read the JSON CONFIG file
		try {
			//using string
			std::ifstream ifs;
			std::shared_ptr<std::string> str; //this might be null
			std::streampos length = 0;

			//open the file
			ifs.open(pathToFile);
			if (!ifs.is_open()) {
				//error
				throw FireDriveException(FireDriveExceptions::FILE_NOT_FOUND_EXCEPTION, "Not able to find the given file.");
			}

			//read the string into the stream
			ifs.seekg(0, std::ios::end); //seek end

			length = ifs.tellg(); //get the length

			//reserve the space needed
			str = std::make_shared<std::string>();
			str->reserve((int)length);
			if (str->capacity() < length) {
				//error
				ifs.close();
				throw FireDriveException(FireDriveExceptions::NO_MEMORY, "Not able to reserve more memory!");
			}

			ifs.seekg(0, std::ios::beg); //reverse back to begining

			//assign h
			str->assign(std::istreambuf_iterator<char>(ifs), std::istreambuf_iterator<char>());
			if (str->length() != length) {
				//error
				ifs.close();
				throw FireDriveException(FireDriveExceptions::UNKNOWN_EXCEPTION, "Unknown error occured while reading the file!");
			}

			//close the file and return the string pointer and len
			ifs.close();
			if (len) *len = (int)length;
			return str;
		}
		catch (FireDriveException exception) {
			//print the error for now
			switch (exception.exception) {
			case UNKNOWN_EXCEPTION:
			case INVALID_ARGUMENT:
			case NO_MEMORY:
			case FILE_NOT_FOUND_EXCEPTION:
				std::cerr << exception.fullMessage << std::endl;
				break;
			}

			return NULL;
		}
	}
}