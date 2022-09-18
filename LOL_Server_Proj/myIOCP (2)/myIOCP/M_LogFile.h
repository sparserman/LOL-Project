#pragma once
#include "T_Singleton.h"

#ifndef __FILE_LOG_H_
#define __FILE_LOG_H_

#endif	// #ifndef __FILE_LOG

class Manager_LogFile : public Singleton<Manager_LogFile>
{
public:
	void ClearFileLog(const char* pszFileName);
	void FileLog(const char* pszLog, ...);
};

