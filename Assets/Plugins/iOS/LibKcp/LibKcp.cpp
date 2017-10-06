//
//  LibKcp.cpp
//  LibKcp
//
//  Created by 杨 帆 on 2017/10/2.
//  Copyright © 2017年 yang fan. All rights reserved.
//

#include "LibKcp.h"
#include <unistd.h>
#include <sys/time.h>
#include <cstring>
#include <cstdio>
#include "sess.h"

IUINT32 iClock();
UDPSession *session;

extern "C"
{
    bool _InitKcp(const char *ip, uint16_t port,
                  int mtu,int wndSize,
                  size_t dataShards, size_t parityShards,
                  bool useStreamMode)
    {
        struct timeval time;
        gettimeofday(&time, NULL);
        srand((time.tv_sec * 1000) + (time.tv_usec / 1000));

        session = UDPSession::DialWithOptions(ip, port, dataShards,parityShards);
        session->NoDelay(1, 20, 2, 1);
        session->WndSize(wndSize, wndSize);
        session->SetMtu(mtu);
        session->SetStreamMode(useStreamMode);
        session->SetDSCP(46);

        return (session != nullptr);
    }
    
    void _SendData(char *buf,size_t sz)
    {
        session->Write(buf, sz);
    }
    
    ssize_t _ReceiveData(char *buf,size_t sz)
    {
        ssize_t n = 0;
        n = session->Read(buf, sz);
        return n;
    }
    
    void _Update()
    {
        session->Update(iClock());
    }
    
    void _Destroy()
    {
        UDPSession::Destroy(session);
    }
}

void TimeOfDay(long *sec, long *usec)
{
    struct timeval time;
    gettimeofday(&time, NULL);
    if (sec) *sec = time.tv_sec;
    if (usec) *usec = time.tv_usec;
}

IUINT64 iClock64(void)
{
    long s, u;
    IUINT64 value;
    TimeOfDay(&s, &u);
    value = ((IUINT64) s) * 1000 + (u / 1000);
    return value;
}

IUINT32 iClock()
{
    return (IUINT32) (iClock64() & 0xfffffffful);
}



