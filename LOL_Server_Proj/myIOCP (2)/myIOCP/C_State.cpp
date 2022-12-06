#include "C_State.h"
#include "Session.h"
#include "M_Room.h"

//STATE_INIT ���ú� ����
void STATE_INIT::recv_sta()
{
	printf("STATE_INIT recv\n");
	Protocol protocol;
	protocol = Manager_Protocol::getInstance().Unpacking_prot(client->getProt());

	switch (protocol.main)
	{
	case MAIN_LOGJOIN:
		switch (protocol.sub)
		{
		case SUB_LOGJOIN_LOGIN:
			char msg[BUFSIZE];
			memset(msg, 0, sizeof(msg));
			client->UnPacking(client->getBuf(), msg);

			printf("%s\n", msg);


			Manager_Room::getInstance().RoomEntrance(client);

			if (client->m_r_count == 0)
			{
				unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_SELECT, SUB_Poppy, 0);
				Manager_LOGJOIN::getInstance().PackPacket(client, pack, JOIN_SUCCESS_MSG);
			}
			else if (client->m_r_count == 1)
			{
				unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_SELECT, SUB_Ezreal, 0);
				Manager_LOGJOIN::getInstance().PackPacket(client, pack, JOIN_SUCCESS_MSG);
			}
			
			break;

		}//protocol.sub���� ����
		break;


	}//protocol.main���� ����
	
}

void STATE_INIT::send_sta()
{
	printf("STATE_INIT send\n");

	//Manager_Room::getInstance().RoomEntrance(client);
	client->setState(client->getsta_game());

}

void STATE_GAME::recv_sta()
{

}

void STATE_GAME::send_sta()
{

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


