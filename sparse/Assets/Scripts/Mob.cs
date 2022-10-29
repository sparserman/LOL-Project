using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField]
    private float hp;

    [SerializeField]
    private Vector3 firstPos;        // 초기 위치
    [SerializeField]
    private float speed;        // 이동 속도

    public GameObject attackTarget = null;  // 공격 목표

    public bool hit = false;    // 공격 받은 상태
    private bool attackCheck = true;     // 공격 장전 상태

    private float time = 0;
    [SerializeField]
    private int patience = 100;     // 인내심

    private Animator ani;
    private NavMeshAgent agent;

    public float deltaRotation;     // 회전 속도

    public float attackRange;
    public float attackSpeed;

    public int mobType;     // 타입

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        ani = GetComponent<Animator>();

        transform.position = firstPos;
    }

    void Update()
    {
        Attack();
    }

    // 공격 받음
    public void HitCheck()
    {
        ani.SetTrigger("isHit");
    }

    private void Attack()
    {
        if (attackTarget != null && patience != 0)
        {
            // 기본 공격 목표에 도착 시
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange)
            {
                // 이동 중지
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // 적 방향 쳐다보기
                transform.forward = attackTarget.transform.position - transform.position;

                // 공격 가능 상태 일때
                if (attackCheck)
                {
                    ani.SetInteger("AttackMotion", 1);

                    if (m_AttackDelayCoroutine != null)
                    {
                        StopCoroutine(m_AttackDelayCoroutine);
                        m_AttackDelayCoroutine = null;
                    }

                    m_AttackDelayCoroutine = StartCoroutine(AttackDelay());

                    attackCheck = false;

                }
            }
            else if (ani.GetInteger("AttackMotion") == 0)
            {
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

                // 방향전환
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

                ani.SetInteger("isMove", 2);
                agent.speed = speed * 0.01f;

                ChampSetDestination(attackTarget.transform.position);
            }

            if(Vector3.Distance(firstPos, attackTarget.transform.position) >= 7f)
            {
                time += Time.deltaTime;
                if(time >= 0.5f)
                {
                    patience -= 10;
                    time = 0;
                }
            }
            else
            {
                time = 0;
            }
        }

        if (patience == 0)
        {
            attackTarget = null;

            // 방향전환
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(firstPos - transform.position), deltaRotation);

            ani.SetInteger("isMove", 2);
            ani.SetInteger("AttackMotion", 0);
            agent.speed = speed * 0.01f;

            ani.Play("Run");

            ChampSetDestination(firstPos);

            if(Vector3.Distance(transform.position, firstPos) <= 0.5f)
            {
                patience = 100;
                transform.forward = new Vector3(-1,0,-1);
                ani.SetInteger("isMove", 0);
                if(mobType == 10)
                {
                    ani.Play("model|Landing_model");
                }
            }

        }

    }

    // 공격 딜레이 (공격속도)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackSpeed);
        attackCheck = true;

        m_AttackDelayCoroutine = null;
    }

    public void AttackEvent(int n)
    {
        switch(n)
        {
            case 0:
                Debug.Log("hit");
                break;
            case 1:
                ani.SetInteger("AttackMotion", 0);
                break;
        }
    }

    // 이동 목적지 설정
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        ani.SetInteger("isMove", 2);
    }
}
