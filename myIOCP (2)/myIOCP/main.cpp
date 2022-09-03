#pragma once
#include "M_main.h"
#include "T_Singleton.h"

int main()
{
	//Manager_main* m1 = new Manager_main;
	//m1->runMain();
	Manager_main::getInstance().runMain();
}