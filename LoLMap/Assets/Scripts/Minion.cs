using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    GameManager gm;

    public float MaxHP = 300;
    public float HP = 300;
    public float ATK = 15;

    public bool die = false;

    public GameObject hpBar;

    [SerializeField]
    private GameObject nexus;

    private Vector3 dir;        // ����
    public float deltaRotation;     // ȸ�� �ӵ�

    public GameObject attackTarget;    // ���� ���
    public float attackRange;       // ���� ��Ÿ�

    [SerializeField]
    private int patience = 0;     // �γ���

    public float speed;

    private NavMeshAgent agent;

    private Animator ani;

    private bool attackDelay = true;

    // ��� : 1, ���� : 2
    public int type;

    void Start()
    {
        gm = GameManager.GetInstance;
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        gm.allList.Add(gameObject);

        hpBar = Instantiate(Resources.Load("Prefabs/" + "MinionHPBar") as GameObject);
        hpBar.transform.parent = GameObject.Find("GUI").transform;
    }

    void Update()
    {
        if (!die)
        {
            MinionControl();
            HPBarControl();
        }
    }

    private void HPBarControl()
    {
        hpBar.transform.GetChild(0).GetComponent<Image>().fillAmount = HP / MaxHP;
    }

    private void MinionControl()
    {
        if(!attackDelay)
        {
            return;
        }
        // Ÿ�� ��ġ
        Vector3 attackPos = attackTarget.transform.position;
        // �̵�
        agent.speed = speed * 0.01f;
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        ani.SetBool("isAttack", false);
        // ������ȯ
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

        // ������ ���� ��
        if (Vector3.Distance(transform.position, attackPos) <= attackRange)
        {
            ani.SetBool("isAttack", true);
            agent.speed = 0;
            attackDelay = false;
        }
        agent.SetDestination(attackPos);

        TargetChange();
    }

    private void TargetChange()
    {
        for (int i = 0; i < gm.allList.Count; i++)
        {
            if (Vector3.Distance(gm.allList[i].transform.position, transform.position) <= 5f)
            {
                if(Vector3.Distance(attackTarget.transform.position, transform.position)
                    > Vector3.Distance(gm.allList[i].transform.position, transform.position))
                {
                    // ��� ���̸�
                    if(type == 1)
                    {
                        if (gm.allList[i].transform.name != transform.name && gm.allList[i].tag == "Red")
                        {
                            attackTarget = gm.allList[i];
                        }
                    }
                    // ���� �� �̸�
                    else if (type == 2)
                    {
                        if (gm.allList[i].transform.name != transform.name && gm.allList[i].tag == "Blue")
                        {
                            attackTarget = gm.allList[i];
                        }
                    }
                }
            }
        }

        if(Vector3.Distance(attackTarget.transform.position, transform.position) > 5f)
        {
            attackTarget = nexus;
        }
    }

    public void MinionAttackEvent()
    {
        Debug.Log("�̴Ͼ� ����");
        attackDelay = true;
        if(attackTarget.layer.Equals(9))
        {
            attackTarget.GetComponent<ChampController>().Damaged(ATK);
        }
        else if (attackTarget.layer.Equals(10))
        {
            attackTarget.GetComponent<Minion>().Damaged(ATK);
        }
    }

    public void Damaged(float p_damage)
    {
        HP -= p_damage;
        if(HP <= 0)
        {
            die = true;
            ani.Play("Die");
            hpBar.SetActive(false);
        }
    }
}
