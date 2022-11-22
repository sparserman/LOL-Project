#include "M_main.h"


// �����ڵ幮���������� �ٹٲܰ�
// ���ڿ��� WCHAR_T�� ���� �ٲܰ� Ŭ���̾�Ʈ�� ���������� TCHAR�� ����

int Manager_main::initMain()
{
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) return 1;

	IOCP::initIOCP();

	// ����� ������ �ֱ�
	listen_soc.socket_soc();
	listen_soc.bind_soc(SERVERPORT);
	listen_soc.listen_soc();
}

int Manager_main::runMain()
{
	// ���ο��� ���ε���
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
	log_file(TEXT("recv ����"));
	return true;
}

bool Manager_main::v_recv(void* p_ptr, int p_cbTransferred)
{
	// session���� ĳ�����ϰ� ��Ŷ�� ���ø�Ʈ�� �����ϴ� ����
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
	log_file(TEXT("complate recv ����"));
	if (!temp->Packet::recv_pak())
	{
		//temp->setState(STA_DISCONNECTED);
		return false;
	}
	log_file(TEXT("recv ����"));
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

	// ť ���� �����ϴ� �Լ� ȣ��
	temp->Packet::send_que();
}

void Manager_main::CompleteRecvProcess(Session* p_ptr)
{
	
	
	// ������Ʈ�� ���� �Ŵ��� �θ���
	// ���������� �� �Ŵ������� �ҷ��� ó���ϱ�
	
	p_ptr->recv_sta();
	
}

void Manager_main::CompleteSendProcess(Session* p_ptr)
{

}
