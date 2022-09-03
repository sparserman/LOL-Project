#include "Socket.h"

#pragma region 기본

#pragma endregion

Socket::Socket()
{
	ZeroMemory(&r_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&s_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&sock, sizeof(SOCKET));
	ZeroMemory(&addr, sizeof(SOCKADDR_IN));

	r_overlapped.ptr = this;
	r_overlapped.type = E_IO_TYPE::IO_RECV;

	s_overlapped.ptr = this;
	s_overlapped.type = E_IO_TYPE::IO_SEND;
}

Socket::Socket(SOCKET p_socket)
	:Socket()
{
	sock = p_socket;

}

void Socket::initSocket()
{
	ZeroMemory(&r_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&s_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&sock, sizeof(SOCKET));
	ZeroMemory(&addr, sizeof(SOCKADDR_IN));

	r_overlapped.type = E_IO_TYPE::IO_RECV;
	s_overlapped.type = E_IO_TYPE::IO_SEND;
}

int Socket::mygetpeername()
{
	int addrlen = sizeof(addr);
	getpeername(sock, (SOCKADDR*)&addr, &addrlen);

	return sock;
}
/*
Socket& Socket::operator=(const Socket& p_sock)
{
	Socket temp;

	temp.r_overlapped = p_sock.r_overlapped;
	temp.s_overlapped = p_sock.s_overlapped;
	temp.sock = p_sock.sock;
	temp.addr = p_sock.addr;

	return temp;
}
*/
bool Socket::socket_soc()
{
	sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET)
	{
		err_quit(TEXT("socket()"));

		return false;
	}

	return true;
}

bool Socket::bind_soc(int p_port)
{
	int retval;

	ZeroMemory(&addr, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(p_port);
	retval = bind(sock, (SOCKADDR*)&addr, sizeof(addr));
	if (retval == SOCKET_ERROR)
	{
		err_quit(TEXT("bind()"));

		return false;
	}

	return true;
}

bool Socket::listen_soc()
{
	int retval;

	retval = listen(sock, SOMAXCONN);
	if (retval == SOCKET_ERROR)
	{
		err_quit(TEXT("listen()"));

		return false;
	}

	return true;
}

// 리슨이 억셉해주는 구조 리턴값으로
SOCKET Socket::accept_soc()
{
	int addrlen;

	//Socket m_sock;
	addrlen = sizeof(addr);

	SOCKET cl_sock;
	SOCKADDR_IN clientaddr;

	cl_sock = accept(this->sock, (SOCKADDR*)&clientaddr, &addrlen);
	if (cl_sock == INVALID_SOCKET)
	{
		err_display(TEXT("accept()"));
		return cl_sock;
	}
	
	return cl_sock;
}

bool Socket::send_soc(char* p_buf, int p_size)
{
	int retval;
	DWORD sendbytes;
	WSABUF temp;

	ZeroMemory(&s_overlapped.overlapped, sizeof(s_overlapped.overlapped));

	temp.buf = p_buf;
	temp.len = p_size;


	
	retval = WSASend(sock, &temp, 1, &sendbytes, 0, &s_overlapped.overlapped, NULL);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			err_display(TEXT("soc_Send()"));
			return false;
		}
	}

	return true;
}

bool Socket::recv_soc(char* p_buf, int p_size)
{
	int retval;
	DWORD flags = 0;
	DWORD recvbytes;
	WSABUF temp;

	ZeroMemory(&r_overlapped.overlapped, sizeof(r_overlapped.overlapped));

	temp.buf = p_buf;
	temp.len = p_size;

	retval = WSARecv(sock, &temp, 1, &recvbytes, &flags, &r_overlapped.overlapped, NULL);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			err_display(TEXT("soc_Recv()"));
			return false;
		}
	}

	return true;
}

void Socket::create_IOport(HANDLE p_hcp)
{
	CreateIoCompletionPort((HANDLE)sock, p_hcp, sock, 0);
}


void Socket::overlappedInit(void* p_ptr)
{
	//r_sizeflag = false;
	/*
	ZeroMemory(&r_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&s_overlapped, sizeof(WSAOVERLAPPED_EX));
	ZeroMemory(&sock, sizeof(SOCKET));
	ZeroMemory(&addr, sizeof(SOCKADDR_IN));
	*/
	r_overlapped.ptr = p_ptr;
	r_overlapped.type = E_IO_TYPE::IO_RECV;

	s_overlapped.ptr = p_ptr;
	s_overlapped.type = E_IO_TYPE::IO_SEND;
}