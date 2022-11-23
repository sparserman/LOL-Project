﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ChamState
{
    public int LV;                  // 레벨
    public int Xp;                  // 경험치
    public float Attack;            // 공격력 
    public float Mag;               // 주문력
    public float AttackSpeed;       // 공격 속도 
    public float HPRegen;           // 체력 재생
    public float MPRegen;           // 마나 재생
    public float Def;               // 방어력
    public float MagDef;            // 마법 저항력
    public float HP;                // 체력
    public float MP;                // 마나
    public float Speed;             // 이동속도 
    public float AttackRange;       // 사거리(AttackRange)
    public float SkillAcc;          // 스킬 가속(쿨감)(SkillAcceleration)
    public float DefPen;            // 방어관통력(defensePenetration)
    public float MagPen;            // 마법관통력(MagiPenetration)
    public float Ten;               // 강인함(Tenacity)
    public float Critical;          // 치명타
    
    enum TEAM
    {
        RED = 1,
        BLUE,
        MONSTER
    };
    // 버프 디버프 
}
