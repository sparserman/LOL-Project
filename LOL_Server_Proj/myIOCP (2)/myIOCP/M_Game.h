#pragma once
#include "Global.h"
class Session;

class Manager_GAME : public Singleton<Manager_GAME>
{
public:
	void UnPacking(char* p_buf, float& p_x, float& p_y, float& p_z);
	void MovePacking(Session* p_ptr, unsigned int p_prot, float p_x, float p_y, float p_z);
};

