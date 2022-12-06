#include "Packet.h"


Packet::Packet(SOCKET p_socket)
	//:Packet()
{
	Socket::Socket(p_socket);
}



unsigned int Packet::getProt()
{
	unsigned int temp;

	memcpy(&temp, recvbuf, sizeof(unsigned int));

	return temp;
}

void Packet::send_que()
{
	if (queue.queue_size() != -1)
	{
		str_pack*  temp = queue.queue_front();
		Socket::send_soc(temp->buf + temp->comp_bytes, temp->bytes - temp->comp_bytes);
	}
}

bool Packet::send_pak(unsigned int p_prot, char* p_data, int p_size)
{
	Packing(p_prot, p_data, p_size);

	if (queue.queue_size() == 1)
	{
		// send
		str_pack* temp = queue.queue_front();
		Socket::send_soc(temp->buf, temp->bytes);
	}
	else
	{
		// �׳� �Ѿ�� ���߿� Ȯ�� ���� �Ŀ� Ȯ��
	}

	return true;
}

bool Packet::recv_pak()
{
	/*
	int retval;
	DWORD recvbytes = 0;
	DWORD flags = 0;

	r_wsabuf.buf = recvbuf + comp_recvbytes;

	if (r_sizeflag)
	{
		r_wsabuf.len = recvbytes - comp_recvbytes;
	}
	else
	{
		r_wsabuf.len = sizeof(int) - comp_recvbytes;
	}

    return Socket::recv_soc(r_wsabuf.buf, r_wsabuf.len);
	*/

	int retval;
	//DWORD recvbytes = 0;
	DWORD flags = 0;

	ZeroMemory(&r_overlapped.overlapped, sizeof(r_overlapped.overlapped));

	r_wsabuf.buf = recvbuf + comp_recvbytes;

	if (r_sizeflag)
	{
		r_wsabuf.len = recvbytes - comp_recvbytes;
	}
	else
	{
		recvbytes = sizeof(int) - comp_recvbytes;
		r_wsabuf.len = recvbytes;
	}

	retval = WSARecv(sock, &r_wsabuf, 1, (LPDWORD)&recvbytes,
		&flags, &r_overlapped.overlapped, NULL);
	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			err_display(TEXT("WSARecv()"));
			return false;
		}
	}

	return true;
}






void Packet::Packing(unsigned int p_prot, char* p_data, int p_size)
{
	char* temp = new char[BUFSIZE];
	char* ptr = temp;
	int totalsize = 0;
	
	ZeroMemory(temp, BUFSIZE);

	ptr = ptr + sizeof(int);


	// ��������
	memcpy(ptr, &p_prot, sizeof(unsigned int));
	ptr = ptr + sizeof(unsigned int);
	totalsize = totalsize + sizeof(unsigned int);


	// �ø��� �ѹ�
	memcpy(ptr, &serialS, sizeof(int));
	ptr = ptr + sizeof(int);
	totalsize = totalsize + sizeof(int);
	serialS++;

	// ������ ������
	//memcpy(ptr, &p_size, sizeof(int));
	//ptr = ptr + sizeof(int);
	//totalsize = totalsize + sizeof(int);

	// ������
	memcpy(ptr, p_data, p_size);
	ptr = ptr + p_size;
	totalsize = totalsize + p_size;

	// �� ������
	ptr = temp;
	memcpy(ptr, &totalsize, sizeof(int));

	totalsize = totalsize + sizeof(int);

	// ť�� Ǫ��
	Push(ptr, totalsize);

	// ���� �Ҵ� ���ֱ� ������ ��� x
	delete[] temp;
}

void Packet::Push(char* p_buf, int p_size)
{
	queue.push(new str_pack(p_buf, p_size));
}

void Packet::UnPacking(char* p_buf, char* p_data)
{
	int strsize1;
	int number;

	//��������, �ø��� �ѹ�, ������
	char* ptr = p_buf + sizeof(unsigned int);  //�������� ������ +

	
	memcpy(&number, ptr, sizeof(int));
	ptr = ptr + sizeof(int);		//�ø��� �ѹ� +

	printf("%d\n", number);

	memcpy(&strsize1, ptr, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);

	memcpy(p_data, ptr, strsize1);
	ptr = ptr + strsize1;

}

int Packet::CompleteRecv_pak(int p_completebyte)
{
	comp_recvbytes += p_completebyte;
	
	if (comp_recvbytes == recvbytes)
	{
		if (!r_sizeflag)
		{
			memcpy(&recvbytes, recvbuf, sizeof(int));
			comp_recvbytes = 0;
			r_sizeflag = true;

			if (!Socket::recv_soc(recvbuf + comp_recvbytes, recvbytes - comp_recvbytes))
			{
				return SOC_ERROR;
			}

			return SOC_FALSE;
		}

		comp_recvbytes = 0;
		recvbytes = 0;
		r_sizeflag = false;

		return SOC_TRUE;
	}
	else
	{
		if (!Socket::recv_soc(recvbuf + comp_recvbytes, recvbytes - comp_recvbytes))
		{
			return SOC_ERROR;
		}

		return SOC_FALSE;
	}
}

int Packet::CompleteSend_pak(int p_completebyte)
{
	// ť���� ����Ʈ�ؼ� ��������
	str_pack* temp = queue.queue_front();

	if (temp == nullptr)
	{
		return SOC_EMPTY;
	}

	// �����ͼ� Ȯ���ϰ� ������ ����Ʈ�� ����Ʈ ������ ��
	temp->comp_bytes += p_completebyte;
	if (temp->comp_bytes == temp->bytes)
	{
		delete temp;
		queue.pop();

		//// ť�� ������ Ȯ���ϰ� ���Ҵٸ� �¸� ���� �ϴ�
		//// �Լ��� �и��� ���ο��� 
		//if (queue.queue_size() != 0)
		//{
		//	temp = queue.queue_front();
		//	Socket::send_soc(temp->buf + temp->comp_bytes, temp->bytes - temp->comp_bytes);
		//}

		return SOC_TRUE;
	}

	if (!Socket::send_soc(temp->buf + temp->comp_bytes, temp->bytes - temp->comp_bytes))
	{
		return SOC_ERROR;
	}

	return SOC_FALSE;
}

void Packet::initPack()
{
	//ZeroMemory(&queue, sizeof(myQueue<str_pack*>));
	ZeroMemory(&r_wsabuf, sizeof(WSABUF));
	ZeroMemory(recvbuf, BUFSIZ);
	r_sizeflag = false;
	comp_recvbytes = 0;
	serialR = 0;
	serialS = 3;
	recvbytes = 0;

}
