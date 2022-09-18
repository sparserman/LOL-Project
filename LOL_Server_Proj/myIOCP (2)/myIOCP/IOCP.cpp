#include "IOCP.h"
//클라이언트랑 서버랑 구조체를 쓸경우 전처리기등을 통해 빈 바이트를 없애야됨
//C# 네트워크공부는 따로해야됨
//C# 에도 쓰레드를 만들어서 RECV쓰레드, SAND쓰레드를 만듬
//RECV쓰레드와 렌더링 쓰레드를 이어주는 Queae, SAND쓰레드와 렌더링 쓰레드를 이어주는 Queae를 만듬
//렌더링 쓰레드에서 이벤트가 발생하면 SAND, RECV쓰레드를 실행

int IOCP::initIOCP()
{
	log_file(TEXT("IOCP init실행"));
	// 입출력 완료 포트 생성
	hcp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
	if (hcp == NULL) return 1;

	// CPU 개수 확인
	SYSTEM_INFO si;
	GetSystemInfo(&si);

	// (CPU 개수 * 2)개의 작업자 스레드 생성
	HANDLE hThread;
	for (int i = 0; i < (int)si.dwNumberOfProcessors * 2; i++)
	{
		hThread = CreateThread(NULL, 0, WorkerThread, this, 0, NULL);
		log_file(TEXT("쓰레드 생성"));
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
		// 비동기 입출력 완료 기다리기
		DWORD cbTransferred;
		SOCKET client_sock;
		WSAOVERLAPPED_EX* overlapped;		//캐스팅용
		//PostQueuedCompletionStatus(hcp, 100, (ULONG_PTR)client_sock, (LPOVERLAPPED)ptr);
		retval = GetQueuedCompletionStatus(ptr->hcp, &cbTransferred,
			(ULONG_PTR*)&client_sock, (LPOVERLAPPED*)&overlapped, INFINITE);
				
		// 클라이언트 정보 얻기
		SOCKADDR_IN clientaddr;
		int addrlen = sizeof(clientaddr);
		getpeername(client_sock, (SOCKADDR*)&clientaddr, &addrlen);

		// 비동기 입출력 결과 확인
		if (retval == 0 || cbTransferred == 0)
		{
			if (retval == 0)
			{
				DWORD temp1, temp2;
				WSAGetOverlappedResult(client_sock, &overlapped->overlapped,
					&temp1, FALSE, &temp2);		//오류코드
				err_display(TEXT("WSAGetOverlappedResult()"));
			}
			closesocket(client_sock);
			printf_s("[TCP 서버] 클라이언트 종료: IP 주소=%s, 포트 번호=%d\n",
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
