#include "Global.h"
#include <stdarg.h>

// 
#define PROT_BIT_SIZE 8

// main 1~16
#define MAIN_LOGJOIN 1
#define GAME_SELECT 2
#define GAME_MAIN 3

#define GAME_Poppy 4
#define GAME_Rengar 5
#define GAME_Ezreal 6
#define GAME_Orianna 7


//  GAME_Champion sub 1~16
#define SUB_MOVE 1
#define SUB_ATTACK 2
#define SUB_SKILL_Q 3
#define SUB_SKILL_W 4
#define SUB_SKILL_E 5
#define SUB_SKILL_R 6
#define SUB_SKILL_PV 7



// MAIN_LOGJOIN sub 1~16
#define SUB_LOGJOIN_LOGIN 1



// GAME_SELECT sub 1~16
#define SUB_Poppy 1
#define SUB_Rengar 2
#define SUB_Ezreal 3
#define SUB_Orianna 4

// GAME_MAIN sub 1~16


//MAIN_LOGJOIN detail 2의 16승까지 배수만
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
    unsigned int Packing_prot(unsigned int p_main, unsigned int p_sub, unsigned int p_args, ...);
    Protocol Unpacking_prot(unsigned int p_prot);
    bool* UnpackingDetail(unsigned int args);
};