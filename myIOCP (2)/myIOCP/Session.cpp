#include "Session.h"



Session::Session()
{
    //state = STA_INIT;
    sta_init = new STATE_INIT(this);
    sta_logjoin = new STATE_LOGJOIN(this);
    sta_lobby = new STATE_LOBBY(this);
    sta_game = new STATE_GAME(this);
    sta_dis = new STATE_DISCONNECTED(this);

    state = sta_init;
    logjoin.init_str_log();
}

Session::Session(SOCKET p_socket)
{
    Packet::Packet(p_socket);
}

void Session::recv_sta()
{
    state->recv_sta();
}

void Session::send_sta()
{
    state->send_sta();
}

int Session::CompleteRecv_ses(void* p_ptr, int p_completebyte)
{
    Session* temp = (Session*)p_ptr;

    return temp->Packet::CompleteRecv_pak(p_completebyte);
}

int Session::CompleteSend_ses(void* p_ptr, int p_completebyte)
{
    Session* temp = (Session*)p_ptr;

    return temp->Packet::CompleteSend_pak(p_completebyte);
}

void Session::initSession()
{
    sta_init = new STATE_INIT(this);
    sta_login = new STATE_LOGIN(this);
    sta_logjoin = new STATE_LOGJOIN(this);
    sta_lobby = new STATE_LOBBY(this);
    sta_game = new STATE_GAME(this);
    sta_dis = new STATE_DISCONNECTED(this);


    state = sta_init;

    //Packet::initPack();

    //state = STA_INIT;
    logjoin.init_str_log();
}


