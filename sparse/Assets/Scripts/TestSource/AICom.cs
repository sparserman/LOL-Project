using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICom : MonoBehaviour
{

    public BaseActor m_LinkActor;

    public MoveCom m_MoveCom;



    public void Attack()
    {

        m_LinkActor.Attack();
    }


    // Start is called before the first frame update
    void Start()
    {
        m_LinkActor = GetComponent<BaseActor>();
        m_MoveCom = GetComponent<MoveCom>();


        m_LinkActor.m_ActorDamageEvent += DamageChekc;
        m_LinkActor.m_ActorDieEvent += Die;
    }

    void Die()
    {

    }

    void DamageChekc(float p_val)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
