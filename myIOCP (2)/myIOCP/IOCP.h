#pragma once
#include "Global.h"
#include "Session.h"

// �߻� Ŭ����
// IOCP ����
// �Ϸ� ��Ʈ ������ ����
// ������ �Լ�
// ���Ʈ ���� ��� �����Լ��� �����

class IOCP
{
protected:
	HANDLE hcp;

public:
	int initIOCP();

	virtual bool v_accept(SOCKET) = 0;
	virtual bool v_recv(void*, int) = 0;
	virtual bool v_send(void*, int) = 0;

	// �������� �Լ��� IOCP���� ����ϱ�
	void ErrorPostQueuedCompletionStatus(SOCKET p_sock);
	void AcceptPostQueuedCompletionStatus(SOCKET p_sock);

	static DWORD WINAPI WorkerThread(LPVOID arg);
};
