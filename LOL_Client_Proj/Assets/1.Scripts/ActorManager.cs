using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum E_PlayerType
{
    Atros = 0,
    Oriana,
    Poppy,

    Max

}

[System.Serializable]
public class PlayerInfoData
{
    public E_PlayerType m_CurrentActorType = E_PlayerType.Max;

}

public class ActorManager : MonoBehaviour
{
    public PlayerInfoData m_PlayerInfoDataCls = null;

    [NonReorderable]
    public List<BaseActor> m_PlayerfabsList = new List<BaseActor>((int)E_PlayerType.Max);



    void Start()
    {
        // 
        GameObject.Instantiate<BaseActor>(m_PlayerfabsList[(int)m_PlayerInfoDataCls.m_CurrentActorType]);

    }

    void Update()
    {
        
    }
}
