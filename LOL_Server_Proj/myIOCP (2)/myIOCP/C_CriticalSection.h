#pragma once
#include <Windows.h>

// 크리티컬섹션을 클래스화
// 지역변수로써 생성 삭제 하기 (생성자, 소멸자에서 처리)
// 패킷에도 매니저들이 각각 들고 있기

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

// 패킷이 가질것
// 크리티컬을 멤버로 소유한 클래스 만들기
// 새로운 객체가 생성자 소멸자에서 락/언락

class C_MyCritSection
{
private:
    C_CriticalSection* ptr;

    C_MyCritSection() :ptr(nullptr) {}
public:
    C_MyCritSection(C_CriticalSection* p_cirt) :ptr(p_cirt) { ptr->Lock(); }
    ~C_MyCritSection() { ptr->Unlock(); };
};
