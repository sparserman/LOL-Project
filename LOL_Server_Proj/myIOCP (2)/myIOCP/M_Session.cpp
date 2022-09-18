#include "M_Session.h"

Session* Manager_Session::AddClientInfo(SOCKET p_sock)
{
	Session* ptr = new Session;


	ptr->setSock(p_sock);
	ptr->mygetpeername();
	//EnterCriticalSection(&cs);

	//ptr->initSession();
	//ptr->initPack();
	//ptr->overlappedInit(&ptr);

	// list 추가
	list_client.push_back(ptr);


	//LeaveCriticalSection(&cs);
	TCHAR msg[1024];

	
	wsprintf(msg ,TEXT("\n[TCP 서버] 클라이언트 접속: IP 주소=%s, 포트 번호=%d\n"),
		inet_ntoa(ptr->getAddr().sin_addr), ntohs(ptr->getAddr().sin_port));

	//log_file(msg);

	return ptr;
}

void Manager_Session::RemoveClientInfo(Session* p_client)
{

	TCHAR msg[1024];
	wsprintf(msg , TEXT("[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n"),
		inet_ntoa(p_client->getAddr().sin_addr), ntohs(p_client->getAddr().sin_port));

	//EnterCriticalSection(&cs);

	// list 만들거나 find 함수 만들기
	list_client.remove(p_client);

	//log_file(msg);
	//LeaveCriticalSection(&cs);
}
