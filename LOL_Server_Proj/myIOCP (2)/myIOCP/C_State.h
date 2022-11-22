#pragma once
class Session;
#ifndef __C_STATE_H_
#define __C_STATE_H_
#include "Global.h"
#include "M_LogJoin.h"


class C_State
{
public:
	virtual void recv_sta() = 0;
	virtual void send_sta() = 0;
};

class STATE_INIT : public C_State
{
protected:
	Session* client;
	bool go_lobby;
public:
	STATE_INIT(Session* p_client) : client(p_client), go_lobby(false){}
	void recv_sta();

	void send_sta();
};

class STATE_GAME : public C_State
{
protected:
	Session* client;
public:
	STATE_GAME(Session* p_client) : client(p_client) {}
	void recv_sta()
	{
		printf("STATE_GAME recv\n");
	}
	void send_sta()
	{
		printf("STATE_GAME send\n");
	}
};

class STATE_LOGIN : public C_State
{
protected:
	Session* client;
public:
	STATE_LOGIN(Session* p_client) : client(p_client) {}

	void recv_sta();
	void send_sta();

};

class STATE_LOGJOIN : public C_State
{
protected:
	Session* client;
public:
	STATE_LOGJOIN(Session* p_client) : client(p_client) {}
	
	void recv_sta();
	void send_sta();
	
};

class STATE_LOBBY : public C_State
{
protected:
	Session* client;
public:
	STATE_LOBBY(Session* p_client) : client(p_client) {}

	void recv_sta();
	void send_sta();

};

class STATE_DISCONNECTED : public C_State
{
protected:
	Session* client;
public:
	STATE_DISCONNECTED(Session* p_client) : client(p_client) {}

	void recv_sta();
	void send_sta();

};
#endif