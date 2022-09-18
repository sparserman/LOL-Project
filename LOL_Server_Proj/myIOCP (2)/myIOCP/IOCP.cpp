#include "IOCP.h"
//Ŭ���̾�Ʈ�� ������ ����ü�� ����� ��ó������� ���� �� ����Ʈ�� ���־ߵ�
//C# ��Ʈ��ũ���δ� �����ؾߵ�
//C# ���� �����带 ���� RECV������, SAND�����带 ����
//RECV������� ������ �����带 �̾��ִ� Queae, SAND������� ������ �����带 �̾��ִ� Queae�� ����
//������ �����忡�� �̺�Ʈ�� �߻��ϸ� SAND, RECV�����带 ����

int IOCP::initIOCP()
{
	log_file(TEXT("IOCP init����"));
	// ����� �Ϸ� ��Ʈ ����
	hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (hcp == NULL) return 1;

	// CPU ���� Ȯ��
	SYSTEM_INFO si;
	GetSystemInfo(&si);

	// (CPU ���� * 2)���� �۾��� ������ ����
	HANDLE hThread;
	for (int i = 0; i < (int)si.dwNumberOfProcessors * 2; i++)
	{
		hThread = CreateThread(NULL, 0, WorkerThread, this, 0, NULL);
		log_file(TEXT("������ ����"));
		if (hThread == NULL) return 1;
		CloseHandle(hThread);
	}
}

void IOCP::ErrorPostQueuedCompletionStatus(SOCKET p_sock)
{
	WSAOVERLAPPED_EX* overlapped = new WSAOVERLAPPED_EX;
	memset(overlapped, 0, sizeof(WSAOVERLAPPED_EX));

	overlapped->type = E_IO_TYPE::IO_DISCONNECT;
	overlapped->ptr = (Session*)p_sock;

	PostQueuedCompletionStatus(hcp, E_IO_TYPE::IO_ERROR, p_sock, (LPOVERLAPPED)overlapped);
}

void IOCP::AcceptPostQueuedCompletionStatus(SOCKET p_sock)
{
	WSAOVERLAPPED_EX* overlapped = new WSAOVERLAPPED_EX;
	memset(overlapped, 0, sizeof(WSAOVERLAPPED_EX));

	overlapped->type = E_IO_TYPE::IO_ACCEPT;
	overlapped->ptr = (Session*)p_sock;

	PostQueuedCompletionStatus(hcp, E_IO_TYPE::IO_ACCEPT, p_sock, (LPOVERLAPPED)overlapped);
}

DWORD __stdcall IOCP::WorkerThread(LPVOID arg)
{
	int retval;
	IOCP* ptr = (IOCP*)arg;


	while (1) 
	{
		// �񵿱� ����� �Ϸ� ��ٸ���
		DWORD cbTransferred;
		SOCKET client_sock;
		WSAOVERLAPPED_EX* overlapped;		//ĳ���ÿ�
		//PostQueuedCompletionStatus(hcp, 100, (ULONG_PTR)client_sock, (LPOVERLAPPED)ptr);
		retval = GetQueuedCompletionStatus(ptr->hcp, &cbTransferred,
			(ULONG_PTR*)&client_sock, (LPOVERLAPPED*)&overlapped, INFINITE);
				
		// Ŭ���̾�Ʈ ���� ���
		SOCKADDR_IN clientaddr;
		int addrlen = sizeof(clientaddr);
		getpeername(client_sock, (SOCKADDR*)&clientaddr, &addrlen);

		// �񵿱� ����� ��� Ȯ��
		if (retval == 0 || cbTransferred == 0)
		{
			if (retval == 0)
			{
				DWORD temp1, temp2;
				WSAGetOverlappedResult(client_sock, &overlapped->overlapped,
					&temp1, FALSE, &temp2);		//�����ڵ�
				err_display(TEXT("WSAGetOverlappedResult()"));
			}
			closesocket(client_sock);
			printf_s("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
				inet_ntoa(clientaddr.sin_addr), ntohs(clientaddr.sin_port));
			delete ptr;
			continue;
		}


		int result;
		
		switch (overlapped->type)
		{
		case IO_RECV:
			ptr->v_recv(overlapped->ptr, cbTransferred);
			break;
		case IO_SEND:
			ptr->v_send(overlapped->ptr, cbTransferred);
			break;
		case IO_DISCONNECT:
			break;
		case IO_ACCEPT:
			ptr->v_accept(client_sock);
			log_file(TEXT("accept"));
			break;
		case IO_ERROR:
			break;
		default:
			break;
		}
	}
	return 0;
}
