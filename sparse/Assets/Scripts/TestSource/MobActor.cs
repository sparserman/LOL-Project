using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_SkillTYPE
{
    Q = 0,
    W,
    E,
    R,


}


[System.Serializable]
public class ActorStat
{
    public int HP;


    //public byte[] GetSerialze()
    //{

    //}

    //public void Deserilize(byte[] p_data)
    //{
    //    HP = 
    //}
}


public class MobActor : BaseActor
{
    public ActorStat ActorStatData;

    public Animator m_LinkAnimator;


    public void Attack()
    {

    }

    public void Skill(E_SkillTYPE p_type)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
