#pragma once
#include "Session.h"
//#include "T_LinkedList.h"
#include "T_Singleton.h"

// client list
class Manager_Session : public Singleton<Manager_Session>
{
protected:
	list<Session*> list_client;
public:
	Session* AddClientInfo(SOCKET p_sock);
	void RemoveClientInfo(Session* p_client);
};