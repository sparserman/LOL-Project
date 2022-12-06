#pragma once
#define SERVERPORT 9000
#define BUFSIZE 1024
#pragma comment(lib, "ws2_32")
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <winsock2.h>
#include <ws2tcpip.h>
#include <list>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <time.h>
#include <Windows.h>
#include <tchar.h>


#pragma warning(disable:4996)
#include "M_LogFile.h"


using namespace std;

#define ID_ERROR_MSG "없는 아이디입니다\n"
#define PW_ERROR_MSG "패스워드가 틀렸습니다.\n"
#define LOGIN_SUCCESS_MSG "로그인에 성공했습니다.\n"
#define ID_EXIST_MSG "이미 있는 아이디 입니다.\n"
#define JOIN_SUCCESS_MSG "가입에 성공했습니다.\n"
#define LOGOUT_MSG "로그아웃되었습니다.\n"

#define LOG_FILE_NAME "DebugingLog.txt"

enum E_PROTOCOL
{
	PROT_LJ_JOIN_INFO,
	PROT_LJ_LOGIN_INFO,
	PROT_LJ_JOIN_RESULT,
	PROT_LJ_LOGIN_RESULT,
	PROT_LJ_LOGOUT,
	PROT_LJ_LOGOUT_RESULT,
};

enum E_STATE
{
	STA_DISCONNECTED = -1,
	STA_INIT = 1,
	STA_INTRO,
	STA_LOGJOIN,
	STA_LJ_MENU_SELECT,
	STA_LJ_RESULT_SEND,
};

enum
{
	SOC_ERROR = 1,
	SOC_TRUE,
	SOC_FALSE,
	SOC_EMPTY,
};

enum E_IO_TYPE
{
	IO_RECV = 1,
	IO_SEND,
	IO_DISCONNECT,
	IO_ACCEPT = -100,
	IO_ERROR = -200,
};

class WSAOVERLAPPED_EX
{
public:
	WSAOVERLAPPED overlapped;
	void* ptr;
	E_IO_TYPE type;
};

struct Protocol
{
	int main;
	int sub;
	int detail;
};

struct str_log
{
	char id[BUFSIZ];
	char pw[BUFSIZ];
	char nick[BUFSIZ];
	bool isLog;
	
	void init_str_log() 
	{
		ZeroMemory(id, sizeof(id)); 
		ZeroMemory(id, sizeof(pw));
		ZeroMemory(id, sizeof(nick));
		//isLog = false; 
	};
};

void err_display(LPCWSTR msg);

void err_quit(LPCWSTR msg);


void log_file(LPCWSTR msg);