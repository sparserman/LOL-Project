#pragma once
#include "Global.h"

class Session;


class Sector
{
private:
	list<Sector*> m_view_list;
	list<Session*> m_player_list;
	//list<Minion*> m_minion_list;
	
};

