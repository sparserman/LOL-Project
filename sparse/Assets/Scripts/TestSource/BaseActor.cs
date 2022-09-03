using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActor : MonoBehaviour
{

    public event Action<float> m_ActorDamageEvent;
    public event Action m_ActorDieEvent;

    public virtual void Attack()
    {

    }

    public virtual void Skill(E_SkillTYPE p_type)
    {

    }

}
