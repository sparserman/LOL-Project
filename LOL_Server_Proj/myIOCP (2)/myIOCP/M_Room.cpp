#include "M_Room.h"
#include "Session.h"

Manager_Room::Manager_Room()
{
	m_roomCount = 0;

	m_room[m_roomCount] = new Room;
	m_room[m_roomCount]->roomNum = m_roomCount;
	m_room[m_roomCount]->count = 0;
}

void Manager_Room::RoomEntrance(Session* p_ptr)
{
	m_room[m_roomCount]->summoner[m_room[m_roomCount]->count] = p_ptr;
	p_ptr->my_room = m_room[m_roomCount];
	p_ptr->m_r_count = m_room[m_roomCount]->count;
	printf("플레이어가 들어왔습니다.\n");
	m_room[m_roomCount]->count++;

	if (m_room[m_roomCount]->count >= 2)
	{
		
	}
}
