#include "M_Game.h"
#include "Session.h"


void Manager_GAME::MoveUnpacking(char* p_buf, float& p_x, float& p_y, float& p_z)
{
	int number;

	//��������, �ø��� �ѹ�, ������
	char* ptr = p_buf + sizeof(unsigned int);  //�������� ������ +


	memcpy(&number, ptr, sizeof(int));
	ptr = ptr + sizeof(int);		//�ø��� �ѹ� +

	printf("%d\n", number);

	memcpy(&p_x, ptr, sizeof(float));
	ptr = ptr + sizeof(float);

	memcpy(&p_y, ptr, sizeof(float));
	ptr = ptr + sizeof(float);

	memcpy(&p_z, ptr, sizeof(float));
	ptr = ptr + sizeof(float);
}

void Manager_GAME::MovePacking(Session* p_ptr, unsigned int p_prot, float p_x, float p_y, float p_z)
{
	char* temp = new char[BUFSIZE];
	char* buf = temp;
	int datasize = 0;
	ZeroMemory(temp, sizeof(temp));


	memcpy(buf, &p_x, sizeof(float));
	datasize += sizeof(float);
	buf += sizeof(float);

	memcpy(buf, &p_y, sizeof(float));
	datasize += sizeof(float);
	buf += sizeof(float);

	memcpy(buf, &p_z, sizeof(float));
	datasize += sizeof(float);
	buf += sizeof(float);

	

	buf = temp;
	// bool�� ���ؼ� �ؿ� ���� ó�� �ϱ�
	for (int i = 0; i < p_ptr->my_room->count; i++)
	{
		p_ptr->my_room->summoner[i]->Packet::send_pak(p_prot, buf, datasize);
	}
	



	delete[] temp;
}




void Manager_GAME::AttackUnpacking(char* p_buf, int& p_champnum)
{
	int number;

	//��������, �ø��� �ѹ�, ������
	char* ptr = p_buf + sizeof(unsigned int);  //�������� ������ +


	memcpy(&number, ptr, sizeof(int));
	ptr = ptr + sizeof(int);		//�ø��� �ѹ� +

	printf("%d\n", number);

	memcpy(&p_champnum, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void Manager_GAME::AttackPacking(Session* p_ptr, unsigned int p_prot, int p_cham)
{
	char* temp = new char[BUFSIZE];
	char* buf = temp;
	int datasize = 0;
	ZeroMemory(temp, sizeof(temp));


	memcpy(buf, &p_cham, sizeof(float));
	datasize += sizeof(float);
	buf += sizeof(float);

	



	buf = temp;
	// bool�� ���ؼ� �ؿ� ���� ó�� �ϱ�
	for (int i = 0; i < p_ptr->my_room->count; i++)
	{
		p_ptr->my_room->summoner[i]->Packet::send_pak(p_prot, buf, datasize);
	}




	delete[] temp;
}