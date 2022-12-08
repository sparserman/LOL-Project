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
			


			Manager_Room::getInstance().RoomEntrance(client);

			if (client->m_r_count == 0)
			{
				unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_SELECT, SUB_Poppy, 0);
				Manager_LOGJOIN::getInstance().PackPacket(client, pack, JOIN_SUCCESS_MSG);
			}
			else if (client->m_r_count == 1)
			{
				unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_SELECT, SUB_Rengar, 0);
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
	printf("STATE_GAME recv\n");

	Protocol protocol;
	protocol = Manager_Protocol::getInstance().Unpacking_prot(client->getProt());

	switch (protocol.main)
	{
	case GAME_Poppy:
		printf("���� ���������� �ǻ߿���\n");
		switch (protocol.sub)
		{
		case SUB_MOVE:
			float x = 0;
			float y = 0;
			float z = 0;
			Manager_GAME::getInstance().UnPacking(client->getBuf(), x, y, z);

			printf("%f, %f, %f", x, y, z);


			unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_Poppy, SUB_MOVE, 0);
			Manager_GAME::getInstance().MovePacking(client, pack, x, y, z);

			break;

		}//protocol.sub���� ����
		break;


	case GAME_Rengar:
		printf("���� ���������� ��������\n");
		switch (protocol.sub)
		{
		case SUB_MOVE:
			float x = 0;
			float y = 0;
			float z = 0;
			Manager_GAME::getInstance().UnPacking(client->getBuf(), x, y, z);

			printf("%f, %f, %f", x, y, z);


			unsigned int pack = Manager_Protocol::getInstance().Packing_prot(GAME_Rengar, SUB_MOVE, 0);
			Manager_GAME::getInstance().MovePacking(client, pack, x, y, z);

			break;

		}//protocol.sub���� ����
		break;


	}//protocol.main���� ����
}

void STATE_GAME::send_sta()
{
	printf("STATE_GAME send\n");
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


