#pragma once
#include "Global.h"


time_t curTime = time(NULL);

void err_display(LPCWSTR msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	printf("[%s] %s", msg, (TCHAR*)lpMsgBuf);
	LocalFree(lpMsgBuf);
}

void err_quit(LPCWSTR msg)
{
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf, 0, NULL);
	MessageBox(NULL, (LPCTSTR)lpMsgBuf, msg, MB_ICONERROR);
	LocalFree(lpMsgBuf);
	exit(1);
}

void log_file(LPCWSTR msg)
{
	struct tm* pLocal = localtime(&curTime);

	Manager_LogFile::getInstance().FileLog("[%02d-%02d] [%02d:%02d:%02d] : %s", pLocal->tm_mon + 1, pLocal->tm_mday,
		pLocal->tm_hour, pLocal->tm_min, pLocal->tm_sec, msg);
}