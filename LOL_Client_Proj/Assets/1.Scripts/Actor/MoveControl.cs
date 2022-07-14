using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveControl : MonoBehaviour
{
    protected NavMeshAgent m_LinkNavMesh = null;
    protected Vector3 m_TargetPos;
    public void TargetMove(Vector3 p_wpos)
    {
        m_TargetPos = p_wpos;
        UpdateNavMesh();
    }

    protected void UpdateNavMesh()
    {
        m_LinkNavMesh.SetDestination(m_TargetPos);
    }



    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
