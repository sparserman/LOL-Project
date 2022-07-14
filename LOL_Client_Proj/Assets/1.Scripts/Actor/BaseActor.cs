using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum E_SkillClickType
{
    Q = 0,
    W,
    E,
    R,
}

public abstract class BaseActor : MonoBehaviour
{
    public abstract void Click_QSkillEvent();
    public abstract void Click_WSkillEvent();
    public abstract void Click_ESkillEvent();
    public abstract void Click_RSkillEvent();


    protected NavMeshAgent m_LinkNavMeshAgent = null;
    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        m_LinkNavMeshAgent = GetComponent<NavMeshAgent>();
    }
    public virtual void TargetMove(Vector3 p_targepos)
    {
        m_LinkNavMeshAgent.SetDestination(p_targepos);

    }
}
