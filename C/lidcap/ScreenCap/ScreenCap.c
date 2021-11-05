#include <stdio.h>
#include <stdlib.h>
#include <Ole2.h>
#include <OleCtl.h>
#include "ScreenCap.h"

//internal functions
//function to push monitors in list, used by EnumDisplayCount function
BOOL CALLBACK pushMonitorsInList(HMONITOR hMon, HDC hdc, LPRECT lprcMonitor, LPARAM pData) {
	//revert into the monitor list
	PHMonitorList monitorList = (PHMonitorList)pData;

	//create the space and add into the list
	if (!monitorList->hmonitor)
		monitorList->hmonitor = (HMONITOR *)calloc(1, sizeof(HMONITOR));
	else {
		void* newMem = (void *)realloc(monitorList->hmonitor, sizeof(HMONITOR) * (monitorList->monitorCount + 1));
		if (!newMem)
			return FALSE;
		monitorList->hmonitor = (HMONITOR *)newMem;
	}

	if (!monitorList->hmonitor)
		return FALSE;

	monitorList->hmonitor[monitorList->monitorCount] = hMon;

	monitorList->monitorCount++;

	return TRUE;
}

//function to get the HDC for a HMONITOR
HDC getHDCFromHMON(HMONITOR hMon) {

	MONITORINFOEXA monInfo = { 0 };
	monInfo.cbSize = sizeof(MONITORINFOEXA);

	GetMonitorInfoA(hMon, (LPMONITORINFO)&monInfo);

	HDC dc = CreateDCA(NULL, monInfo.szDevice, NULL, NULL);

	return dc;
}


//function to find the index of a color in the histogram list, this searches the first match, 
//assuming no single item will repeat in a histogram.
BOOL findIndexHist(PColorHistogram hist, int histSize, SCColor color, int* index, PColorHistogram* tupple) {
	if (!hist || !histSize)
		return FALSE;
	for (int i = 0; i < histSize; i++) {
		if (
			hist[i].color.R == color.R ||
			hist[i].color.G == color.G ||
			hist[i].color.B == color.B
		) {	
			if (index)
				*index = i;
			if (tupple)
				*tupple = &hist[i];

			return TRUE;
		}
	}
	return FALSE;
}


//function to prepare the histogram of the colors in a HBITMAP
int getHistogramOfCols(HBITMAP hBitmap, int width, int height, PColorHistogram *colorHist, int *histSize) {
	HDC hdc;
	COLORREF col;
	SCColor color;
	HBITMAP oldBitmap;

	PColorHistogram hist = NULL;
	int size = 0;

	//create a device context same as the screen
	hdc = CreateCompatibleDC(NULL);

	//select the HBITMAP into that device context
	oldBitmap = SelectObject(hdc, hBitmap);

	//read the color one by one and store
	for (int h = 0; h < height; h++) {
		for (int w = 0; w < width; w++ ) {
			int ind = 0;

			col = GetPixel(hdc, w, h);
			
			color.R = GetRValue(col);
			color.G = GetGValue(col);
			color.B = GetBValue(col);

			if (findIndexHist(hist, size, color, &ind, NULL)) {
				//color exists in histogram, increase the count
				hist[ind].count++;
				continue;
			}
			
			//color does not exists in the histogram create a new block for that color
			if (size == 0)
				hist = (PColorHistogram)calloc(1, sizeof(ColorHistogram));
			else
				hist = (PColorHistogram)realloc(hist, sizeof(ColorHistogram) * (size + 1));
			
			size++;

			hist[size - 1].color = color;
			hist[size - 1].count = 1;
		}
	}

	if (histSize)
		*histSize = size;
	if (colorHist)
		*colorHist = hist;
	else if (!colorHist && hist)
		free(hist);

	SelectObject(hdc, oldBitmap);

	DeleteDC(hdc);

	return size;
}


//function to find the dominant color
int* find4DomCol(PColorHistogram colHist, int histSize) {
	int*	hInd = (int *)calloc(4, sizeof(int));
	long*	colCounts = (long*)calloc(4, sizeof(long));

	//find the 4 top dom colors
	for (int i = 0; i < histSize; i++) {
		if (colHist[i].count > (unsigned long)colCounts[0]) {
			colCounts[0] = colHist[i].count;
			hInd[0] = i;
		}
		else if (colHist[i].count > (unsigned long)colCounts[1]) {
			colCounts[1] = colHist[i].count;
			hInd[1] = i;
		}
		else if (colHist[i].count > (unsigned long)colCounts[2]) {
			colCounts[2] = colHist[i].count;
			hInd[2] = i;
		}
		else if (colHist[i].count > (unsigned long)colCounts[3]) {
			colCounts[3] = colHist[i].count;
			hInd[3] = i;
		}
	}

	free(colCounts);
	return hInd;
}


//Internal function to resize and stream the monitor image, for faster processing
HBITMAP capForStream(int monitorIndex, unsigned int compIntensity) {
	HDC hScreenDC;
	HDC hMemoryDC;
	HBITMAP hBitmap;
	HBITMAP hOldBitmap;
	int width, height;


	//get the display context (dc) of the display device
	if (monitorIndex == 0)
		hScreenDC = GetDC(NULL); //get the primary display/device context
	else if (monitorIndex == -1)
		hScreenDC = CreateDC(TEXT("DISPLAY"), NULL, NULL, NULL); //create a dc for all the monitors
	else {
		PHMonitorList monList;
		int count = queryMonitorCount(&monList);

		if (monitorIndex > count)
			return NULL;

		hScreenDC = getHDCFromHMON(monList->hmonitor[monitorIndex - 1]);

		freeHMonitorList(monList);
	}

	hMemoryDC = CreateCompatibleDC(hScreenDC); //create a compatible dc buffer

	//get the height and width of the screen
	width = (monitorIndex == -1) ? GetDeviceCaps(hScreenDC, HORZRES) * 2 : GetDeviceCaps(hScreenDC, HORZRES);
	height = GetDeviceCaps(hScreenDC, VERTRES);

	//create a compatible Bitmap to that DC with the height and width
	hBitmap = CreateCompatibleBitmap(hScreenDC, width / compIntensity, height / compIntensity);
	hOldBitmap = SelectObject(hMemoryDC, hBitmap); //select and replace the newly created bitmap in the DC, and hold onto the old one

	//copy the image data from the Screen DC, to memoryDC which will store it into the seleced hBitmap bitmap, this time we stretch it to a smaller scale
	//BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, 0, 0, SRCCOPY);
	StretchBlt(hMemoryDC, 0, 0, width / compIntensity, height / compIntensity, hScreenDC, 0, 0, width, height, SRCCOPY);

	hBitmap = SelectObject(hMemoryDC, hOldBitmap); //reverse the selection in the hMemoryDC, and get back the hBitMap in which the image data is there

	//debug - remove
	writeDIB("E:/Work/Codes/C/lidcap/ScreenCap/test/index_N1.bmp", hBitmap, NULL);

	//delete unwanted DC's
	DeleteDC(hMemoryDC);
	DeleteDC(hScreenDC);

	return hBitmap; //return the acquired bmp data
}


//Exposed Function --------------------------------------------------------------------------------------------------------------------------------------

BOOL freeHMonitorList(PHMonitorList monList) {

	if (!monList)
		return FALSE;

	free(monList->hmonitor);
	free(monList);
	return TRUE;
}

int queryMonitorCount(HMonitorList** monList) {
	//create the monitor rect structure
	HMonitorList* monList_t = (HMonitorList *)calloc(1, sizeof(HMonitorList));
	//call the enum display monitor function
	EnumDisplayMonitors(0, 0, pushMonitorsInList, (LPARAM)monList_t);

	if (!monList) {
		int count = monList_t->monitorCount;
		freeHMonitorList(monList_t); //free the hdc list
		return count;
	}

	*monList = monList_t;

	return monList_t->monitorCount;
}

HBITMAP captureScreenImage() {
	return captureScreenImageEx(0, NULL, FALSE);
}

HBITMAP captureScreenImageEx(int monitorIndex, char* fileName, BOOL freeAtEnd) {
	HDC hScreenDC;
	HDC hMemoryDC;
	HBITMAP hBitmap;
	HBITMAP hOldBitmap;
	int width, height;
	

	//get the display context (dc) of the display device
	if (monitorIndex == 0)
		hScreenDC = GetDC(NULL); //get the primary display/device context
	else if (monitorIndex == -1)
		hScreenDC = CreateDC(TEXT("DISPLAY"), NULL, NULL, NULL); //create a dc for all the monitors
	else {
		PHMonitorList monList;
		int count = queryMonitorCount(&monList);

		if (monitorIndex > count)
			return NULL;

		hScreenDC = getHDCFromHMON(monList->hmonitor[monitorIndex - 1]);

		freeHMonitorList(monList);
	}
	
	hMemoryDC = CreateCompatibleDC(hScreenDC); //create a compatible dc buffer

	//get the height and width of the screen
	width = (monitorIndex == -1) ? GetDeviceCaps(hScreenDC, HORZRES) * 2 : GetDeviceCaps(hScreenDC, HORZRES);
	height = GetDeviceCaps(hScreenDC, VERTRES);

	//create a compatible Bitmap to that DC with the height and width
	hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
	hOldBitmap = SelectObject(hMemoryDC, hBitmap); //select and replace the newly created bitmap in the DC, and hold onto the old one

	//copy the image data from the Screen DC, to memoryDC which will store it into the seleced hBitmap bitmap
	BitBlt(hMemoryDC, 0, 0, width, height, hScreenDC, 0, 0, SRCCOPY);

	hBitmap = SelectObject(hMemoryDC, hOldBitmap); //reverse the selection in the hMemoryDC, and get back the hBitMap in which the image data is there

	//delete unwanted DC's
	DeleteDC(hMemoryDC);
	DeleteDC(hScreenDC);

	if (fileName) {
		//save the file
		writeDIB(fileName, hBitmap, NULL);
	}

	if (freeAtEnd) {
		DeleteObject(hBitmap);
	}

	return hBitmap; //return the acquired bmp data
}

short writeDIB(char* fileName, HBITMAP hBitmap, HPALETTE palette) {
	
	PICTDESC pd; //picture description	
	HRESULT res;
	LPPICTURE picture;
	LPSTREAM stream;
	HANDLE file;
	HGLOBAL mem = 0;
	LPVOID data;
	DWORD bytes_written = 0;
	LONG bytes_streamed = 0;
	BOOL result = FALSE;

	//fill the picture description
	pd.cbSizeofstruct = sizeof(PICTDESC);
	pd.picType = PICTYPE_BITMAP;
	pd.bmp.hbitmap = hBitmap;
	pd.bmp.hpal = palette;

	//create the picture
	res = OleCreatePictureIndirect(
		&pd,
		&IID_IPicture,
		FALSE,
		&picture
	);
	if (!SUCCEEDED(res))
		return -1;

	//create the byte stream, to save in a file
	res = CreateStreamOnHGlobal(0, TRUE, &stream);
	if (!SUCCEEDED(res)) {
		picture->lpVtbl->Release(picture); //release the picture
		return -1;
	}
	
	//stream the bytes to a file
	res = picture->lpVtbl->SaveAsFile(picture, stream, TRUE, &bytes_streamed); //get the stream
	//create the file
	file = CreateFileA(fileName, GENERIC_WRITE, FILE_SHARE_READ, 0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
	if (!SUCCEEDED(res) || !file) {
		stream->lpVtbl->Release(stream);
		picture->lpVtbl->Release(picture);
		return -1;
	}

	//get the dat from the stream
	res = GetHGlobalFromStream(stream, &mem);
	data = GlobalLock(mem); //lock the mem to write

	result = WriteFile(file, data, bytes_streamed, &bytes_written, 0); //write to the file created
	result &= (bytes_written == bytes_streamed); //verify

	//unlock the memory and close handles
	GlobalUnlock(mem);
	CloseHandle(file);

	//release the strean and picture
	stream->lpVtbl->Release(stream);
	picture->lpVtbl->Release(picture);

	return (result) ? 0 : -1;
}

SCColor getDominantColor(HBITMAP hBitmap, PColorHistogram* colHist, int* histSize, int** domIndex) {
	SCColor noCol = { 0 };
	BITMAP bitmap; //for getting the bitmap details
	PColorHistogram hist = NULL;
	int size = 0;
	int* index;

	if (!hBitmap) //check if the handle is there or not
		return noCol;

	//get the bitmap object out of HBITMAP
	GetObject(hBitmap, sizeof(BITMAP), &bitmap); //from this we can get the size of the image

	//get the histogram of the colors
	getHistogramOfCols(hBitmap, bitmap.bmWidth, bitmap.bmHeight, &hist, &size);

	index = find4DomCol(hist, size); //find the dominant color

	noCol = hist[index[0]].color;

	if (colHist)
		*colHist = hist;
	else
		free(hist);
	if (histSize)
		*histSize = size;
	if (domIndex)
		*domIndex = index;

	return noCol;
}

SCColor getDominantColorFromScreen(int monitorIndex, PColorHistogram* colHist, int* histSize, int** domIndex) {
	//get the bitmap
	//HBITMAP hBitmap = captureScreenImageEx(monitorIndex, NULL, FALSE);
	HBITMAP hBitmap = capForStream(monitorIndex, 5); //using a default compression value for now.
	//get the dominant color
	SCColor color = getDominantColor(hBitmap, colHist, histSize, domIndex);
	//delete the captured object
	DeleteObject(hBitmap);
	//return the color
	return color;
}


int main() {
	//capture the screen and save the file
	//HBITMAP screen = captureScreenImage();
	//writeDIB("E:/Work/Codes/C/lidcap/ScreenCap/test/sahidScreenshot.bmp", screen, NULL);
	//DeleteObject(screen);

	//qerrying the count of monitor
	//int i = queryMonitorCount(NULL);

	//capturing the screen and saving the file in one go
	captureScreenImageEx(2, "E:/Work/Codes/C/lidcap/ScreenCap/test/index_N1.bmp", TRUE);

	//finding the top 4 dominant colors in the screen 2
	/*int* domIndex;
	PColorHistogram hist;
	int histSize;
	SCColor  domColor = getDominantColorFromScreen(2, &hist, &histSize, &domIndex);
		
	free(domIndex);*/


	return 0;
}