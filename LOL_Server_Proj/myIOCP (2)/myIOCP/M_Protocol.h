#include "Global.h"
#include <stdarg.h>

// 
#define PROT_BIT_SIZE 8

// main 1~16
#define MAIN_LOGJOIN 2

// sub 1~16
#define SUB_LOGJOIN_LOGIN 1

// detail 2의 16승까지 배수만
#define DETALI_LOGIN_RESULT 1
#define DETALI_JOIN_RESULT 2
#define DETALI_LOGIN_SUCCESS 4
#define DETALI_JOIN_SUCCESS 8

class Manager_Protocol : public Singleton<Manager_Protocol>
{
private:
    bool* detail_list;
    int list_size;
public:
    Manager_Protocol() :detail_list(nullptr), list_size(0) {};
    ~Manager_Protocol() { cleanList(); };

    void cleanList() { if (detail_list != nullptr)delete[] detail_list; list_size = 0; };
    void Packing_prot(unsigned int p_main, unsigned int p_sub, unsigned int p_args, ...);
    void Unpacking_prot(unsigned int p_prot);
    bool* UnpackingDetail(unsigned int args);
};

void 최재욱()
{
    Manager_Protocol::getInstance().Packing_prot(MAIN_LOGJOIN, SUB_LOGJOIN_LOGIN, 2, DETALI_LOGIN_RESULT, DETALI_LOGIN_SUCCESS);
}

