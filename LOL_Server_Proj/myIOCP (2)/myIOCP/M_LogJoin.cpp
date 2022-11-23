#include "M_LogJoin.h"
#include "Session.h"

void Manager_LOGJOIN::LoginProcess(Session* p_ptr)
{
	E_RESULT login_result = NODATA;
	str_log temp = p_ptr->getlogjoin();

	char msg[BUFSIZ];
	E_PROTOCOL protocol;

	for (int i = 0; i < 3; i++)
	{
		if (!strcmp(LoginInfo[i].id, temp.id))
		{
			if (!strcmp(LoginInfo[i].pw, temp.pw))
			{
				login_result = LOGIN_SUCCESS;
				strcpy(msg, LOGIN_SUCCESS_MSG);
			}
			else
			{
				login_result = PW_ERROR;
				strcpy(msg, PW_ERROR_MSG);
			}
			break;
		}
	}

	if (login_result == NODATA)
	{
		login_result = ID_ERROR;
		strcpy(msg, ID_ERROR_MSG);
	}

	if (login_result != LOGIN_SUCCESS)
	{
	}

	//p_ptr->setState(STA_LJ_RESULT_SEND);

	protocol = PROT_LJ_LOGIN_RESULT;

	classifyProt(p_ptr, protocol, login_result, msg);
}

void Manager_LOGJOIN::LogoutProcess(Session* p_ptr)
{
	E_RESULT login_result = NODATA;
	char msg[BUFSIZ];
	E_PROTOCOL protocol;

	p_ptr->getlogjoin().init_str_log();

	//p_ptr->setState(E_STATE::STA_LJ_RESULT_SEND);

	strcpy(msg, LOGOUT_MSG);
	protocol = PROT_LJ_LOGOUT_RESULT;
	login_result = LOGOUT_SUCCESS;

	classifyProt(p_ptr, protocol, login_result, msg);
}

void Manager_LOGJOIN::classifyProt(Session* p_ptr, E_PROTOCOL p_prot, E_RESULT p_rst, char* p_data)
{
	switch (p_prot)
	{
	case PROT_LJ_JOIN_INFO:
		break;
	case PROT_LJ_LOGIN_INFO:
		break;
	case PROT_LJ_JOIN_RESULT:
		//PackPacket(p_ptr, p_prot, p_rst, p_data);
		break;
	case PROT_LJ_LOGIN_RESULT:
		//PackPacket(p_ptr, p_prot, p_rst, p_data);
		break;
	case PROT_LJ_LOGOUT:
		break;
	case PROT_LJ_LOGOUT_RESULT:
		//PackPacket(p_ptr, p_prot, p_rst, p_data);
		break;
	default:
		break;
	}
}

void Manager_LOGJOIN::PackPacket(Session* p_ptr, unsigned int p_prot, const char* p_data)
{
	char* temp = new char[BUFSIZE];
	char* buf = temp;
	int datasize = 0;
	ZeroMemory(temp, sizeof(temp));


	// Data size
	int strsize = strlen(p_data);
	memcpy(buf, &strsize, sizeof(int));
	datasize += sizeof(int);
	buf += sizeof(int);

	// Data
	memcpy(buf, p_data, strsize);
	datasize += strsize;
	buf += strsize;

	buf = temp;
	// bool값 정해서 밑에 껄로 처리 하기
	p_ptr->Packet::send_pak(p_prot, buf, datasize);


	delete[] temp;
}

void Manager_LOGJOIN::UnPackPacket(Session* p_ptr)
{
	p_ptr->recv_pak();
}
