#ifndef FIRE_BASE_DRIVE_HEADER
#define FIRE_BASE_DRIVE_HEADER

//includes
#include <string>
#include <memory>


namespace _FireDrive_ {

	//function prototypes
	typedef void (* _OnCompletionCallBack) (bool);



	typedef enum _FireDriveExceptions {
		UNKNOWN_EXCEPTION = -1,
		NO_EXCEPTION = 0,
		FILE_NOT_FOUND_EXCEPTION = 20021,
		INVALID_ARGUMENT,
		NO_MEMORY
	}FireDriveExceptions, *PFireDriveExceptions;

	//class for FireDrive related exceptions
	class FireDriveException {

	public:
		FireDriveExceptions exception;
		std::string message;
		std::string customMessage;
		std::string fullMessage;

		FireDriveException();

		FireDriveException(FireDriveExceptions exception, const char* customMessage = NULL);

		FireDriveException(const char* customMessageFormat, FireDriveExceptions exception, ...);

		FireDriveException(const FireDriveException& exception);

		~FireDriveException();

		std::string getMessage();

		std::string getCustomMessage();

		std::string getFullMessage();

		void operator=(const FireDriveException& obj);
	};


	//class for FireDrive Utility Functions
	class FireDriveUtility {

	protected:

		//function to read the whole file provided the path of
		//remember to use free() to free the allocated buffer memory
		//len can be null
		//returns a shared pointer to the string
		std::shared_ptr<std::string> readWholeFile(const char* pathToFile, unsigned int* len = NULL);

	};


	//interface class for firebase authentication
	class FireDriveAuthentication {
	public:

		/*CreateUser Function
		* Creates a new User
		*
		* Parameters -
		* email = the email to be used to login
		* password = the password to be set
		* exception = refference to a FireDriveException object
		* callback = a callback to be called at the end of the operation completion. Can be NULL.
		*
		* Returns true if successful or false if not. Check the exception if false.
		*/
		virtual bool CreateUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback) = 0;


		/*SignInUser Function
		* Signs a user in, if provided correct password and email
		*
		* Parameters -
		* email = the email to be used while logging in
		* password = the password to be used while logging in
		* exception = refference to a FireDriveException object
		* callback = a callback function to be called at the end of the operation completion. Can be NULL.
		*/
		virtual std::shared_ptr<const std::string> SignInUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback = NULL) = 0;


	};


	class FireDrive : public FireDriveAuthentication, public FireDriveUtility {
	private:
		void* appOptions;
		void* app;

		std::shared_ptr<std::string> fileData;
		std::shared_ptr<std::string> userUID;

		//blocked functions
		FireDrive(const FireDrive& obj);
		void operator=(const FireDrive& obj);

	public:

		/*Constructor
		*
		* Creates the FireDrive object with the supplied arguments
		*
		* Parameters -
		* jsonConfigContent = the string content of the json config file
		* len = leave it as length of the string content
		*/
		FireDrive(const char* jsonConfigContent, int len);

		/*Constructor
		*
		* Creates the FireDrive object with the supplied config json
		*
		* Parameters -
		* pathToConfigJson = path to the config json file
		*/
		FireDrive(const char* pathToConfigJson);


		//destructor
		virtual ~FireDrive();


		//public functions

		/*getJsonConfigData Function
		*
		* Returns the json config file data as a shared pointer to a const string.
		*/
		std::shared_ptr<const std::string> getJsonConfigFileData();


		/*CreateUser Function
		* Creates a new User
		* 
		* Parameters -
		* email = the email to be used to login
		* password = the password to be set
		* exception = refference to a FireDriveException object
		* callback = a callback to be called at the end of the operation completion. Can be NULL.
		* 
		* Returns true if successful or false if not. Check the exception if false.
		*/
		bool CreateUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback = NULL) override;



		/*SignInUser Function
		* Signs a user in, if provided correct password and email
		* 
		* Parameters - 
		* email = the email to be used while logging in
		* password = the password to be used while logging in
		* exception = refference to a FireDriveException object
		* callback = a callback function to be called at the end of the operation completion. Can be NULL.
		*/
		std::shared_ptr<const std::string> SignInUser(const char* email, const char* password, FireDriveException& exception, _OnCompletionCallBack callback = NULL) override;
	};


}

#endif