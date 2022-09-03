#pragma once
#include "Global.h"
#include "Session.h"

// 추상 클래스
// IOCP 관련
// 완료 포트 쓰레드 생성
// 쓰레드 함수
// 억셉트 리슨 등등 가상함수로 만들기

class IOCP
{
protected:
	HANDLE hcp;

public:
	int initIOCP();

	virtual bool v_accept(SOCKET) = 0;
	virtual bool v_recv(void*, int) = 0;
	virtual bool v_send(void*, int) = 0;

	// 레지스터 함수로 IOCP소켓 등록하기
	void ErrorPostQueuedCompletionStatus(SOCKET p_sock);
	void AcceptPostQueuedCompletionStatus(SOCKET p_sock);

	static DWORD WINAPI WorkerThread(LPVOID arg);
};
