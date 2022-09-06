using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baron_2 : MonoBehaviour
{

    [SerializeField]
    protected Animator m_LinkAIFSM;


    [SerializeField]
    public Animator m_ActAnimator;


    public GameObject attackTarget;    // 공격 목표

    public bool hit = false;    // 공격 받은 상태
    public bool m_ISAttackCheck = true;     // 공격 장전 상태

    [SerializeField]
    protected BaronShotObj m_AttackEffect;    // 공격 이펙트

    [SerializeField]
    protected Transform m_ThroatPos;        // 공격 발사 위치


    //public bool ISAttack
    //{
    //    get { return m_ISAttackCheck; }
    //    set { m_ISAttackCheck = value; } 
    //}


    [field:SerializeField]
    public bool ISHit { get; set; }

    [SerializeField]
    private BaronShotObj AttackEffect = null;     // 공격 이펙트

    [SerializeField]
    public int patience = 100;     // 인내심

    [SerializeField]
    public int attackCount = 0;    // 공격 횟수

    [SerializeField]
    public Transform ThroatPos;    // 공격 나오는 위치


    private void Awake()
    {
        m_LinkAIFSM = GetComponent<Animator>();

        //m_ActAnimator = this.GetComponentInChildren<Animator>();


        BaronBase[] baselist = m_LinkAIFSM.GetBehaviours<BaronBase>();

        foreach (var item in baselist)
        {
            item.Initilize(this);
        }

        BaronIdle idle = m_LinkAIFSM.GetBehaviour<BaronIdle>();
        m_LinkAIFSM.GetBehaviour<BaronChease>();

        //idle.SetAttackTarget(attackTarget);
    }

    public void Attack()
    {
        m_ActAnimator.SetTrigger("Attack");
    }

    void Start()
    {
        


    }

    void Update()
    {
        
    }

    public void AttackEvent(int n)
    {
        switch (n)
        {
            case 0:
                Debug.Log("shot");
                BaronShotObj cpyObj;
                cpyObj = Instantiate<BaronShotObj>(m_AttackEffect, m_ThroatPos.position, Quaternion.identity);
                cpyObj.gameObject.SetActive(true);
                cpyObj.SetTarget(attackTarget);
                break;
        }
    }
}
