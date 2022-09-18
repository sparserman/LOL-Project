#pragma once
#include <Windows.h>

// ũ��Ƽ�ü����� Ŭ����ȭ
// ���������ν� ���� ���� �ϱ� (������, �Ҹ��ڿ��� ó��)
// ��Ŷ���� �Ŵ������� ���� ��� �ֱ�

class C_CriticalSection
{
private:
    CRITICAL_SECTION m_cs;
    BOOL m_bInit;

public:
    C_CriticalSection();
    C_CriticalSection(DWORD dwSpinCount);
    ~C_CriticalSection();

    void Lock() { EnterCriticalSection(&m_cs); }
    void Unlock() { LeaveCriticalSection(&m_cs); }
    BOOL IsInit() { return m_bInit; }
};

// ��Ŷ�� ������
// ũ��Ƽ���� ����� ������ Ŭ���� �����
// ���ο� ��ü�� ������ �Ҹ��ڿ��� ��/���

class C_MyCritSection
{
private:
    C_CriticalSection* ptr;

    C_MyCritSection() :ptr(nullptr) {}
public:
    C_MyCritSection(C_CriticalSection* p_cirt) :ptr(p_cirt) { ptr->Lock(); }
    ~C_MyCritSection() { ptr->Unlock(); };
};
