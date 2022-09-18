#pragma once
#include "Global.h"

// bind, listen, accept
// basic send, recv
class Socket
{
protected:
	WSAOVERLAPPED_EX r_overlapped;
	WSAOVERLAPPED_EX s_overlapped;

	SOCKET sock;
	SOCKADDR_IN addr;
public:
	Socket();
	Socket(SOCKET p_socket);
	//~Socket() { closesocket(sock); };

	void initSocket();

	// get, set
	void setSock(SOCKET p_sock) { this->sock = p_sock; };

	SOCKADDR_IN getAddr() { return this->addr; }
	int mygetpeername();
	
	//Socket& operator=(const Socket& p_sock);

	bool socket_soc();
	bool bind_soc(int p_port);
	bool listen_soc();
	// 소켓 핸들만 반환
	SOCKET accept_soc();

	bool send_soc(char* p_buf, int p_size);
	bool recv_soc(char* p_buf, int p_size);

	//최재욱 함수
	void create_IOport(HANDLE p_hcp);
	void overlappedInit(void* p_ptr);
};