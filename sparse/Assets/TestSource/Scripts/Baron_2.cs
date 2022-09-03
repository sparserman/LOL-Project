using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baron_2 : MonoBehaviour
{

    [SerializeField]
    protected Animator m_LinkAIFSM;


    [SerializeField]
    public Animator m_ActAnimator;


    public GameObject attackTarget;    // ���� ��ǥ

    public bool hit = false;    // ���� ���� ����
    public bool m_ISAttackCheck = true;     // ���� ���� ����

    [SerializeField]
    protected BaronShotObj m_AttackEffect;    // ���� ����Ʈ

    [SerializeField]
    protected Transform m_ThroatPos;        // ���� �߻� ��ġ


    //public bool ISAttack
    //{
    //    get { return m_ISAttackCheck; }
    //    set { m_ISAttackCheck = value; } 
    //}


    [field:SerializeField]
    public bool ISHit { get; set; }

    [SerializeField]
    private BaronShotObj AttackEffect = null;     // ���� ����Ʈ

    [SerializeField]
    public int patience = 100;     // �γ���

    [SerializeField]
    public int attackCount = 0;    // ���� Ƚ��

    [SerializeField]
    public Transform ThroatPos;    // ���� ������ ��ġ


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
