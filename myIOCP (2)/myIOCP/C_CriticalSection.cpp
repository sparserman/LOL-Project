#include "C_CriticalSection.h"

C_CriticalSection::C_CriticalSection()
    : m_bInit(FALSE)
{
    InitializeCriticalSection(&m_cs);
    m_bInit = TRUE;
}

C_CriticalSection::C_CriticalSection(DWORD dwSpinCount)
    : m_bInit(FALSE)
{
    m_bInit = InitializeCriticalSectionAndSpinCount(&m_cs, dwSpinCount);
}

C_CriticalSection::~C_CriticalSection()
{
    DeleteCriticalSection(&m_cs);
}
