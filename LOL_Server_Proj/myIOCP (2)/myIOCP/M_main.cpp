#include "M_main.h"


// 유니코드문자집합으로 다바꿀것
// 문자열을 WCHAR_T로 전부 바꿀것 클라이언트와 연동을위해 TCHAR도 가능

int Manager_main::initMain()
{
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) return 1;

	IOCP::initIOCP();

	// 멤버로 가지고 있기
	listen_soc.socket_soc();
	listen_soc.bind_soc(SERVERPORT);
	listen_soc.listen_soc();
}

int Manager_main::runMain()
{
	// 메인에서 따로따로
	initMain();


	while (1)
	{
		SOCKET cl_sock;
		cl_sock = listen_soc.accept_soc();
		AcceptPostQueuedCompletionStatus(cl_sock);

		//v_accept(listen_soc.accept_soc());
	}

	return 0;
}

bool Manager_main::v_accept(SOCKET p_sock)
{
	
	
	Session* ptr = Manager_Session::getInstance().AddClientInfo(p_sock);
	ptr->create_IOport(hcp);

	if (!ptr->Packet::recv_pak())
	{
		//ErrorPostQueuedCompletionStatus();
		return false;
	}
	log_file(TEXT("recv 선언"));
	return true;
}

bool Manager_main::v_recv(void* p_ptr, int p_cbTransferred)
{
	// session으로 캐스팅하고 패킷의 컴플릿트를 실핼하는 구조
	Session* temp = (Session*)p_ptr;

	int result = temp->Packet::CompleteRecv_pak(p_cbTransferred);
	switch (result)
	{
	case SOC_ERROR:
		return 0;
	case SOC_FALSE:
		return 0;
	case SOC_TRUE:
		break;
	}

	temp->recv_sta();
	//CompleteRecvProcess(temp);
	log_file(TEXT("complate recv 실행"));
	if (!temp->Packet::recv_pak())
	{
		//temp->setState(STA_DISCONNECTED);
		return false;
	}
	log_file(TEXT("recv 선언"));
	return true;
}

bool Manager_main::v_send(void* p_ptr, int p_cbTransferred)
{
	Session* temp = (Session*)p_ptr;

	int result = temp->Packet::CompleteSend_pak(p_cbTransferred);
	switch (result)
	{
	case SOC_ERROR:
		return 0;
	case SOC_EMPTY:
		return 0;
	case SOC_FALSE:
		return 0;
	case SOC_TRUE:
		break;
	}


	temp->send_sta();
	//CompleteSendProcess(temp);

	// 큐 다음 센드하는 함수 호출
	temp->Packet::send_que();
}

void Manager_main::CompleteRecvProcess(Session* p_ptr)
{
	
	
	// 스테이트에 따라 매니저 부르고
	// 프로토콜은 각 매니저에서 불러서 처리하기
	
	p_ptr->recv_sta();
	
}

void Manager_main::CompleteSendProcess(Session* p_ptr)
{

}
