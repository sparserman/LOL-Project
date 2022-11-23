#pragma once
#include "Global.h"

#define MAX_X 20
#define MAX_Y 20

#define SEC_SIZE 3

class Sector;

class M_Sector : public Singleton< M_Sector>
{


private:
	list<Sector> m_sector_list[SEC_SIZE][SEC_SIZE];
};