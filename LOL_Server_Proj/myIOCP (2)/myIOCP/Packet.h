#pragma once
#include "Socket.h"
#include "T_Queue.h"

// packing/unpacking
// send queue
// recv
class Packet : public Socket
{
protected:
	class str_pack
	{
	public:
		char* buf;  // 동적할당
		int bytes;
		int comp_bytes;

		str_pack()
			: buf(nullptr), bytes(0), comp_bytes(0)
		{};

		str_pack(char* p_buf,int p_size)
			: bytes(0), comp_bytes(0)
		{
			buf = new char[p_size + 1];
			ZeroMemory(buf, sizeof(buf));
			memcpy(buf, p_buf, p_size);
			this->bytes = p_size;
		};

		str_pack(str_pack* p_pack)
		{
			this->buf = p_pack->buf;
			this->bytes = p_pack->bytes;
			this->comp_bytes = p_pack->comp_bytes;
		};

		~str_pack()
		{
			if (this->buf != nullptr)
			{
				delete[] buf;
			}
		};
	};

	myQueue<str_pack*> queue;

	int serialR;
	int serialS;

	WSABUF r_wsabuf;
	char recvbuf[BUFSIZE];
	bool r_sizeflag;
	int recvbytes;
	int comp_recvbytes;

public:
	Packet() { initPack(); }
	Packet(SOCKET p_socket);
	//~Packet() {}

	void setSizeflag(bool p_bool) { this->r_sizeflag = p_bool; };

	void Packing(unsigned int p_prot, char* p_data, int p_size);
	//push
	void Push(char* p_buf, int p_size);
	void UnPacking(char* p_buf, char* p_data);

	// 프로토콜이랑 버퍼위치를 제공하는 함수 만들기
	char* getBuf() { return recvbuf; };
	E_PROTOCOL getProt();

	void send_que();

	//샌드에서 패킹 호출
	bool send_pak(unsigned int p_prot, char* p_data, int p_size);
	bool recv_pak();

	int CompleteRecv_pak(int p_completebyte);
	int CompleteSend_pak(int p_completebyte);

	void initPack();
};