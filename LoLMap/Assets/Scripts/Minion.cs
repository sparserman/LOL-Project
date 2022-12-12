using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    GameManager gm;

    public Team team;

    public float MaxHP = 300;
    public float HP = 300;
    public float ATK = 35;

    public bool die = false;

    public GameObject hpBar;

    public GameObject blueNexus;
    public GameObject redNexus;

    private Vector3 dir;        // ����
    public float deltaRotation;     // ȸ�� �ӵ�

    public GameObject attackTarget;    // ���� ���
    public float attackRange;       // ���� ��Ÿ�

    public float speed;

    private NavMeshAgent agent;

    private Animator ani;

    private bool attackDelay = true;

    public bool aggro = false;

    void Start()
    {
        gm = GameManager.GetInstance;
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        gm.allList.Add(gameObject);

        hpBar = Instantiate(Resources.Load("Prefabs/" + "MinionHPBar") as GameObject);
        hpBar.transform.parent = GameObject.Find("GUI").transform;

        for (int i = 0; i < GameManager.GetInstance.playerList.Count; i++)
        {
            if (GameManager.GetInstance.playerList[i].GetComponent<ChampController>().inOperation)
            {
                if (GameManager.GetInstance.playerList[i].GetComponent<ChampController>().team == team)
                {
                    GetComponent<Minion>().hpBar.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 1, 1);
                }
            }
        }
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
        if (!attackDelay)
        {
            return;
        }

        // �̵�
        agent.speed = speed * 0.01f;
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        ani.SetBool("isAttack", false);
        // ������ȯ
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

        // è���� �׾��� �� üũ
        if (attackTarget.layer.Equals(9))
        {
            if (attackTarget.GetComponent<ChampController>().die)
            {
                // Ÿ�� ����
                if (team == Team.BLUE)
                {
                    attackTarget = redNexus;
                }
                else
                {
                    attackTarget = blueNexus;
                }
            }
        }
        // �̴Ͼ��� �׾��� �� üũ
        else if (attackTarget.layer.Equals(10))
        {
            if (attackTarget.GetComponent<Minion>().die)
            {
                // Ÿ�� ����
                if (team == Team.BLUE)
                {
                    attackTarget = redNexus;
                }
                else
                {
                    attackTarget = blueNexus;
                }
            }
        }


        // ������ ���� ��
        if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange)
        {
            ani.SetBool("isAttack", true);
            agent.speed = 0;
            attackDelay = false;
        }
        agent.SetDestination(attackTarget.transform.position);


        if (!aggro)
        {
            TargetChange();
        }
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
                    // è�Ǿ��̸�
                    if (gm.allList[i].layer.Equals(9))
                    {
                        if (team != gm.allList[i].GetComponent<ChampController>().team
                            && !gm.allList[i].GetComponent<ChampController>().die)
                        {
                            attackTarget = gm.allList[i];
                        }
                    }
                    // �̴Ͼ��̸�
                    else if (gm.allList[i].layer.Equals(10))
                    {
                        if (gm.allList[i].transform != gameObject)
                        {
                            if (team != gm.allList[i].GetComponent<Minion>().team
                                && !gm.allList[i].GetComponent<Minion>().die)
                            {
                                attackTarget = gm.allList[i];
                            }
                        }
                    }
                }
            }
        }

        if(Vector3.Distance(attackTarget.transform.position, transform.position) > 5f)
        {
            if(team == Team.BLUE)
            {
                attackTarget = redNexus;
            }
            else
            {
                attackTarget = blueNexus;
            }
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
            GetComponent<Collider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    private void DieEvent()
    {
        gm.allList.Remove(gameObject);
        Destroy(gameObject);
    }


}
