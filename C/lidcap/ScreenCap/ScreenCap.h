/*
* Screen Capture Library 1.02
* Author - Amit Kr. Das
* 
* A small and simple library to do some screen capturing and saving as bmp Image.
* Provides a small ammount of other functionalities involving captured images or capturing images.
* Uses the wingdi library to capture and ole library for file streaming.
*/



#ifndef SCREENCAP_H
#define SCREENCAP_H

#include <Windows.h>


//structures

/*Structure
* Monitor HDC List -
* structure to list the monitor HDCs.
* 
* Members -
* monitorCount - Count of the displays.
* hmonitor - an array of HMONITORs for the displays
*/
typedef struct _HMonitorList {
	int monitorCount;
	HMONITOR* hmonitor;
}HMonitorList, * PHMonitorList;


/*Structure 
* Screen Cap Color -
* RGB containing strcture.
* 
* Members -
* R - Intensity of Red.
* G - Intensity of Green.
* B - Intensity of Blue.
*/
typedef struct _SCColor {
	DWORD	R;
	DWORD	G;
	DWORD	B;
}SCColor, *PSCColor;


/*Structure
* Color Histogram
* structure for making the histogram of colors, used for getting the dominant color.
* 
* Members -
* color - A SCColor structure containing the color.
* count - Total count of the color.
*/
typedef struct _ColorHistogram {
	SCColor color;
	DWORD count;
}ColorHistogram, *PColorHistogram;




//functions

/*Function
* Capture Screen Image Ex -
* This function is used for capturing the screen, with additional argumented operations.
* 
* Params - 
* monitorIndex - The index of the monitor to be captured. Index of monitors starts from 1.
* >>> 0 - passing 0 will capture the primary screen.
* >>>-1 - passing -1 will capture all the screens together. <TODO - Buggy>
* fileName - Name of the file, the captured image is to be saved as. If NULL, no file will be saved.
* freeAtEnd - Boolean to free the bitmap data in buffer after the function ends. If this is set to true, the return will be useless. Switch it to True if you dont need the return.
* 
* Return - returns the HBITMAP handle to the bitmap data, if free at end is set to false.
* */
HBITMAP captureScreenImageEx(int monitorIndex, char* fileName, BOOL freeAtEnd);


/*Function
* Capture  Screen Image -
* This function directly captures the primary monitor and returns the HBITMAP handle of the bitmap data.
* 
* Returns - returns the HBITMAP handle to the bitmap data. 
*/
HBITMAP captureScreenImage();


/*Function
* Write DIB -
* Writes a HBITMAP DIb data into a bmp file.
* 
* Params -
* fileName - The name of the file to be saved.
* hBitmap - HBITMAP pointer to DIB data.
* palette - Color Pallete if any, have to be prepared, can be NULL in case of images over 8 bits
* 
* Returns - -1 if something fails, 0 if all goes well
*/
short writeDIB(char* fileName, HBITMAP hBitmap, HPALETTE palette);


/*Function
* Query Monitor Count -
* Used to get the count of monitors, if pointer to PHMonitorList is passed,
* it returns the hdc list to all the display contexts.
* 
* Params -
* hdcList - Pointer to the PHMonitorList.
* 
* Returns - returns the count of the monitor.
*/
int queryMonitorCount(HMonitorList** monList);


/*Function
* Free HMonitor List -
* Used to free an aquired HMonitorList.
* 
* Params -
* monList - pointer to the HMonitorList to be freed.
* 
* Returns - True if ok, FALSE if something goes wrong.
*/
BOOL freeHMonitorList(PHMonitorList monList);


/*Function
* Get Dominant Color -
* Used to get a dominant color out of a given HBITMAP.
* 
* Params -
* hBitmap - Handle to the bitmap data.
* colHist - An histogram of Colors, can be NULL if not needed.
* histSize - Pointer to an integer where the size of the Color Histogram will be returned.
* domIndex - Pointer to an integer Array where the index of the top 4 dominant color in the histogram Array will be returned.
* 
* Returns - returns the dominant color as RGB is SCColor structure.
*/
SCColor getDominantColor(HBITMAP hBitmap, PColorHistogram* colHist, int* histSize, int** domIndex);


/*Function
* Get Dominant Color From Screen -
* Used to get a dominant color out of a given Monitor index.
*
* Params -
* monitorIndex - Index of the monitor to be searched for dominant color.
* colHist - An histogram of Colors, can be NULL if not needed.
* histSize - Pointer to an integer where the size of the Color Histogram will be returned.
* domIndex - Pointer to an integer Array where the index of the top 4 dominant color in the histogram Array will be returned.
*
* Returns - returns the dominant color as RGB is SCColor structure.
*/
SCColor getDominantColorFromScreen(int monitorIndex, PColorHistogram* colHist, int* histSize, int** domIndex);

#endif