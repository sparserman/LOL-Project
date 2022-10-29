using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputActor : MonoBehaviour
{

    public BaseActor m_LinkActor;

    public void Attack()
    {

        m_LinkActor.Attack();
    }

    void Start()
    {
        m_LinkActor = GetComponent<BaseActor>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_LinkActor.Attack();
        }
    }
}
