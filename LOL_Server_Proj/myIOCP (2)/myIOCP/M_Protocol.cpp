#include "M_Protocol.h"

unsigned int Manager_Protocol::Packing_prot(unsigned int p_main, unsigned int p_sub, unsigned int p_args, ...)
{
    // �������� ��ŷ
    unsigned int prot = 0;
    unsigned int temp = 0;

    va_list ap;    // ���� ���� ��� ������

    // main
    temp = temp | p_main << PROT_BIT_SIZE * 3;
    prot = prot | temp;
    //printf("temp : %d\n", temp);
    temp = 0;

    // sub
    temp = temp | p_sub << PROT_BIT_SIZE * 2;
    prot = prot | temp;
    //printf("temp : %d\n", temp);
    temp = 0;

    // detail
    va_start(ap, p_args);    // ���� ���� ��� ������ ����
    for (int i = 0; i < p_args; i++)    // ���� ���� ������ŭ �ݺ�
    {
        unsigned int num = va_arg(ap, unsigned int);    // unsigned int ũ�⸸ŭ ���� ���� ��� �����Ϳ��� ���� ������
                                                        // ap�� unsigned int ũ�⸸ŭ ���������� �̵�
        temp = temp | num;
    }
    va_end(ap);    // ���� ���� ��� �����͸� NULL�� �ʱ�ȭ
    prot = prot | temp;
    temp = 0;

    //printf("prot : %d\n", prot);

    

    return prot;
}

void Manager_Protocol::Unpacking_prot(unsigned int p_prot)
{
    unsigned int temp = 0;
    unsigned int main = 0;
    unsigned int sub = 0;
    unsigned int detail = 0;

    temp = 0xff00000 & p_prot;
    main = temp >> PROT_BIT_SIZE * 3;
    temp = 0;

    temp = 0xff000 & p_prot;
    sub = temp >> PROT_BIT_SIZE * 2;
    temp = 0;

    temp = 0xffff & p_prot;
    detail = temp;

    printf("main : %d\nsub : %d\ndetail : %d\n", main, sub, detail);

    cleanList();
    detail_list = UnpackingDetail(detail);
}

bool* Manager_Protocol::UnpackingDetail(unsigned int args)
{
    bool* temp = new bool[PROT_BIT_SIZE * 2];
    int complement = 1;

    for (int i = 0; i < PROT_BIT_SIZE * 2; i++)
    {

        if (args & complement)
        {
            temp[i] = true;
            list_size++;
        }
        else
        {
            temp[i] = false;
        }

        //printf("%d. %d\n", complement, temp[i]);

        complement *= 2;
    }

    return temp;
}