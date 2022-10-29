using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField]
    private float hp;

    [SerializeField]
    private Vector3 firstPos;        // �ʱ� ��ġ
    [SerializeField]
    private float speed;        // �̵� �ӵ�

    public GameObject attackTarget = null;  // ���� ��ǥ

    public bool hit = false;    // ���� ���� ����
    private bool attackCheck = true;     // ���� ���� ����

    private float time = 0;
    [SerializeField]
    private int patience = 100;     // �γ���

    private Animator ani;
    private NavMeshAgent agent;

    public float deltaRotation;     // ȸ�� �ӵ�

    public float attackRange;
    public float attackSpeed;

    public int mobType;     // Ÿ��

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

    // ���� ����
    public void HitCheck()
    {
        ani.SetTrigger("isHit");
    }

    private void Attack()
    {
        if (attackTarget != null && patience != 0)
        {
            // �⺻ ���� ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= attackRange)
            {
                // �̵� ����
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // �� ���� �Ĵٺ���
                transform.forward = attackTarget.transform.position - transform.position;

                // ���� ���� ���� �϶�
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

                // ������ȯ
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

            // ������ȯ
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

    // ���� ������ (���ݼӵ�)
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

    // �̵� ������ ����
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        ani.SetInteger("isMove", 2);
    }
}
