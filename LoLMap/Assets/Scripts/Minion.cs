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

    private Vector3 dir;        // 방향
    public float deltaRotation;     // 회전 속도

    public GameObject attackTarget;    // 공격 대상
    public float attackRange;       // 공격 사거리

    [SerializeField]
    private int patience = 0;     // 인내심

    public float speed;

    private NavMeshAgent agent;

    private Animator ani;

    private bool attackDelay = true;

    // 블루 : 1, 레드 : 2
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
        // 타겟 위치
        Vector3 attackPos = attackTarget.transform.position;
        // 이동
        agent.speed = speed * 0.01f;
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        ani.SetBool("isAttack", false);
        // 방향전환
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

        // 목적지 도착 시
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
                    // 블루 팀이면
                    if(type == 1)
                    {
                        if (gm.allList[i].transform.name != transform.name && gm.allList[i].tag == "Red")
                        {
                            attackTarget = gm.allList[i];
                        }
                    }
                    // 레드 팀 이면
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
        Debug.Log("미니언 공격");
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
