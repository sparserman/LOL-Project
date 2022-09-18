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

	// list �߰�
	list_client.push_back(ptr);


	//LeaveCriticalSection(&cs);
	TCHAR msg[1024];

	
	wsprintf(msg ,TEXT("\n[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n"),
		inet_ntoa(ptr->getAddr().sin_addr), ntohs(ptr->getAddr().sin_port));

	//log_file(msg);

	return ptr;
}

void Manager_Session::RemoveClientInfo(Session* p_client)
{

	TCHAR msg[1024];
	wsprintf(msg , TEXT("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n"),
		inet_ntoa(p_client->getAddr().sin_addr), ntohs(p_client->getAddr().sin_port));

	//EnterCriticalSection(&cs);

	// list ����ų� find �Լ� �����
	list_client.remove(p_client);

	//log_file(msg);
	//LeaveCriticalSection(&cs);
}
