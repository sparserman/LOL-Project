using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCom : MonoBehaviour
{
    [SerializeField]
    protected BaseActor m_LinkActor = null;
    [SerializeField]
    protected MoveControl m_LinkMoveControl = null;

    public event Action<KeyCode> m_KeyClickEvent;

    private void Awake()
    {
        m_LinkActor = GetComponent<BaseActor>();
        m_LinkMoveControl = GetComponent<MoveControl>();
    }

    //void Start()
    //{
        
    //}

    
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Q) )
        {
            m_LinkActor.Click_QSkillEvent();

            if(m_KeyClickEvent != null)
            {
                m_KeyClickEvent(KeyCode.Q);
            }
        }


        if(Input.GetMouseButtonDown(0))
        {
            Vector3 wpos = new Vector3();
            m_LinkMoveControl.TargetMove(wpos);
        }

        
    }
}
