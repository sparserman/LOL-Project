#pragma once
#include "IOCP.h"
#include "M_Session.h"
#include "M_LogJoin.h"
#include "T_Singleton.h"

// �α� �Ŵ��� ����� �������� ����Ʈ�Լ��� "����"�� ���

// SetConsoleCtrlHandler Class �����
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