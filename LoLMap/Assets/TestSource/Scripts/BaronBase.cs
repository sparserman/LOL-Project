using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaronBase : StateMachineBehaviour
{
    [Header("[바론베이스용들]")]
    [SerializeField]
    protected GameObject m_AttackTarget;


    [SerializeField]
    protected GameObject m_LookTarget;

    [SerializeField]
    protected Baron_2 m_LinkMob;


    public void Initilize(Baron_2 p_LinkBaron)
    {
        m_LinkMob = p_LinkBaron;

    }
}
