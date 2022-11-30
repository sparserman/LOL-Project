#include "C_State.h"
#include "Session.h"


//STATE_INIT ���ú� ����
void STATE_INIT::recv_sta()
{
	printf("STATE_INIT recv\n");
	
	Manager_Protocol::getInstance().Unpacking_prot(client->getProt());

	
	char msg[BUFSIZE];
	memset(msg, 0, sizeof(msg));
	client->UnPacking(client->getBuf(), msg);

	printf("%s\n", msg);
	
	
}

void STATE_INIT::send_sta()
{
	printf("STATE_INIT send\n");

	unsigned int pack = Manager_Protocol::getInstance().Packing_prot(MAIN_LOGJOIN, SUB_LOGJOIN_LOGIN, 1, DETALI_LOGIN_RESULT);
	Manager_LOGJOIN::getInstance().PackPacket(client, pack, JOIN_SUCCESS_MSG);

	client->setState(client->getsta_login());
}

//STATE_LOGIN ���ú� ����
void STATE_LOGIN::recv_sta()
{

}

void STATE_LOGIN::send_sta()
{

}


//STATE_LOGJOIN ���ú� ����
void STATE_LOGJOIN::recv_sta()
{
}

void STATE_LOGJOIN::send_sta()
{
}


//STATE_LOBBY ���ú� ����
void STATE_LOBBY::recv_sta()
{
}

void STATE_LOBBY::send_sta()
{
}


//STATE_DISCONNECTED ���ú� ����
void STATE_DISCONNECTED::recv_sta()
{
}

void STATE_DISCONNECTED::send_sta()
{
}