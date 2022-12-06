#pragma once
class Session;
#include "Global.h"

struct Room
{
	int roomNum;
	Session* summoner[2];
	int count;
};

class Manager_Room : public Singleton<Manager_Room>
{
public:
	Manager_Room();

	void RoomEntrance(Session* p_ptr);
protected:
	int m_roomCount;
	Room* m_room[5];
};

