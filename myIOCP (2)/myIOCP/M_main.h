#pragma once
#include "IOCP.h"
#include "M_Session.h"
#include "M_LogJoin.h"
#include "T_Singleton.h"

// 로그 매니저 만들기 가변인자 리스트함수로 "파일"에 찍기

// SetConsoleCtrlHandler Class 만들기
class Manager_main : public IOCP, public Singleton<Manager_main>
{
protected:
	WSADATA wsa;
	Socket listen_soc;
public:
	int initMain();

	int runMain();

	virtual bool v_accept(SOCKET p_sock);
	virtual bool v_recv(void* p_ptr, int p_cbTransferred);
	virtual bool v_send(void* p_ptr, int p_cbTransferred);

	void CompleteRecvProcess(Session* p_ptr);
	void CompleteSendProcess(Session* p_ptr);
};