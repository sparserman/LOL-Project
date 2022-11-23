#pragma once
#include "T_Singleton.h"
//#include "Session.h"
#include "Global.h"
class Session;

enum E_RESULT
{
	NODATA = -1,
	ID_EXIST = 1,
	ID_ERROR,
	PW_ERROR,
	JOIN_SUCCESS,
	LOGIN_SUCCESS,
	LOGOUT_SUCCESS
};

class Manager_LOGJOIN : public Singleton<Manager_LOGJOIN>
{
protected:
	list<Session*> list_join;

	// ¶«»§¿ë
	struct LoginInfo
	{
		char id[512];
		char pw[512];
	}LoginInfo[3] = { {"kja0204","1234"},{"aaa","111"},{"bbb","222"} };
public:

	void LoginProcess(Session* p_ptr);
	void LogoutProcess(Session* p_ptr);

	void classifyProt(Session* p_ptr, E_PROTOCOL p_prot, E_RESULT p_rst, char* p_data);

	// LOGIN RESULT
	void PackPacket(Session* p_ptr, unsigned int p_prot ,const char* p_data);
	void UnPackPacket(Session* p_ptr);
};

