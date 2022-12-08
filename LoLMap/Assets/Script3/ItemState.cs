using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemState
{
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
    public float SkillAcc;          // 스킬 가속(쿨감)(SkillAcceleration)
    public float DefPen;            // 방어관통력(defensePenetration)
    public float MagPen;            // 마법관통력(MagiPenetration)
    public float Ten;               // 강인함(Tenacity)
    public float Critical;          // 치명타
    public float LifeSteal;         // 생명력 흡수
    public int price;               // 가격
}
