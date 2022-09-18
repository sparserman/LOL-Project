#include "Global.h"

#include <fstream>
#include <cstdarg>



using namespace std;

void Manager_LogFile::ClearFileLog(const char* pszFileName)
{
	unlink(pszFileName);
}

void Manager_LogFile::FileLog(const char* pszLog, ...)
{
	fstream _streamOut;
	//_streamOut.open(pszFileName, ios::out | ios::app);
	_streamOut.open(LOG_FILE_NAME, ios::out | ios::app);

	va_list argList;
	char cbuffer[1024];
	va_start(argList, pszLog);
	int i = 0;

	vsnprintf(cbuffer, 1024, pszLog, argList);
	//vsnprintf(cbuffer, 1024, pszLog, argList);
	va_end(argList);

	_streamOut << cbuffer << endl;
	_streamOut.close();
}