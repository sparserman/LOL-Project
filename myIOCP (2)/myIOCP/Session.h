#pragma once
#include "Packet.h"
#include "C_State.h"
#include "Global.h"
#ifndef _SESSION_H_
#define _SESSION_H_

// 각 매니저들의 모든 스테이트를 들고 있고 현재 스태이트가지는 변수 만들기

// client
class Session : public Packet
{
protected:
	C_State* state;
	STATE_INIT* sta_init;
	STATE_LOGIN* sta_login;
	STATE_LOGJOIN* sta_logjoin;
	STATE_LOBBY* sta_lobby;
	STATE_GAME* sta_game;
	STATE_DISCONNECTED* sta_dis;

	str_log logjoin;

public:
	Session();
	Session(SOCKET p_socket);


	void setState(C_State* p_state) { state = p_state; }
	
	void recv_sta();
	void send_sta();


	C_State* getsta_init() { return sta_init; }
	C_State* getsta_login() { return sta_login; }
	C_State* getsta_logjoin() { return sta_logjoin; }
	C_State* getsta_lobby() { return sta_lobby; }
	C_State* getsta_game() { return sta_game; }
	C_State* getsta_dis() { return sta_dis; }
	
	str_log getlogjoin() { return logjoin; };

	int CompleteRecv_ses(void* p_ptr, int p_completebyte);
	int CompleteSend_ses(void* p_ptr, int p_completebyte);

	void initSession();
};

#endif