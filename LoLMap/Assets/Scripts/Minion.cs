using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    GameManager gm;

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

    public int type;

    void Start()
    {
        gm = GameManager.GetInstance;
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        gm.allList.Add(gameObject);
        attackTarget = nexus;
    }

    void Update()
    {
        TargetClear();
        MinionControl();
    }

    private void MinionControl()
    {
        // 타겟 위치
        Vector3 attackPos = attackTarget.transform.position;

        if (Vector3.Distance(transform.position, attackPos) < 1f)
        {
            Debug.Log("도착");
        }
        // 이동
        agent.speed = speed * 0.01f;
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

        // 방향전환
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

        // 목적지 도착 시
        if (Vector3.Distance(transform.position, attackPos) <= attackRange)
        {
            ani.SetInteger("isMove", 0);
            agent.speed = 0;
        }
        agent.SetDestination(attackPos);
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
                    if(type == 1)
                    {
                        
                    }
                    else
                    {
                        if (gm.allList[i].transform.name != transform.name && gm.allList[i].tag == "Blue")
                        {
                            attackTarget = gm.allList[i];
                            patience = 100;
                        }
                        
                    }
                }
            }
        }
    }

    private float currentTime = 0;
    private void TargetClear()
    {
        if (patience <= 0)
        {
            if (attackTarget != null)
            {
                TargetChange();
            }
        }
        if (attackTarget != null && attackTarget != nexus)
        {
            if (Vector3.Distance(transform.position, attackTarget.transform.position) >= 3)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= 1f)
                {
                    currentTime = 0;
                    patience -= 20;
                }
            }
        }
    }

    public void MinionAttackEvent()
    {
        Debug.Log("미니언 공격");
    }
}
