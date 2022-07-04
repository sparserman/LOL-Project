using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove : MonoBehaviour
{
    public Camera cam;
    private Animator ani;
    private ChamState state;
    private NavMeshAgent agent;

    private Vector3 destination;    // ������ ��ǥ

    private GameObject attackTarget;    // ���ݸ�ǥ

    public GameObject QSkillObj;    // Q ��ų ����Ʈ

    public int attackCount = 0;     // ���� ��� ��ȣ
    public bool attackCheck = true;     // ���� ���� ����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        ani = GetComponent<Animator>();

        state.Speed = 325;
        state.AttackSpeed = 1.5f;
    }
    void Update()
    {
        Move();
        QSkill();
        Attack();
        if (Input.GetKeyDown(KeyCode.T))
        {
            state.AttackSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            state.AttackSpeed -= 0.1f;
        }
    }

    private void Move()
    {
        ani.SetFloat("AttackSpeed", 1.5f / state.AttackSpeed);
        // ���콺 ��Ŭ��
        if (Input.GetMouseButton(1) && !ani.GetBool("isQSkill"))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    attackTarget = hit.collider.gameObject;
                }
                else
                {
                    attackTarget = null;
                }

                ChampSetDestination(hit.point);
                StopCoroutine(MoveStop());
                if (attackTarget == null)
                {
                    ani.SetBool("isAttack", false);  // �̵� �� ���� ��� ����
                    ani.SetInteger("AttackMotion", 0);  // �̵� �� ���� ��� ����
                }

            }
        }



        if (!ani.GetBool("isQSkill"))
        {
            // ������ȯ�� �̵�
            if (ani.GetInteger("isMove") == 2)
            {
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
                transform.forward = dir;

                // ������ ���� ��
                if (Vector3.Distance(transform.position, destination) <= 0.5f)
                {
                    ani.SetInteger("isMove", 1);
                    agent.speed = 0;
                    //StopCoroutine(MoveStop());
                    StartCoroutine(MoveStop());
                    return;
                }
            }
        }

    }

    private void Attack()
    {
        if (attackTarget != null)
        {
            ani.SetInteger("isMove", 2);
            ani.SetBool("isAttack", true);
            agent.speed = state.Speed * 0.01f;
            var dir = new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z) - transform.position;
            transform.forward = dir;

            // ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 3f)
            {
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                if (attackCheck)
                {
                    if (attackCount == 3)
                    {
                        attackCount = 1;
                    }
                    else
                    {
                        attackCount++;
                    }
                    ani.SetInteger("AttackMotion", attackCount);
                    StartCoroutine(AttackHit());
                    StartCoroutine(AttackDelay());
                    if (ani.GetInteger("AttackMotion") > 0)
                    {
                        attackCheck = false;
                        ani.SetInteger("AttackMotion", attackCount);
                    }

                }
            }
        }
    }

    // ���� ��Ʈ
    IEnumerator AttackHit()
    {
        yield return new WaitForSeconds(0.5f);
    }

    // ���� ������ (���ݼӵ�)
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(state.AttackSpeed);
        attackCheck = true;
    }

    // �̵� ���� �� (��������� ����)
    IEnumerator MoveStop()
    {
        yield return new WaitForSeconds(1.3f);
        if(ani.GetInteger("isMove") == 1)
        {
            ani.SetInteger("isMove", 0);
        }
    }

    // Q ��ų
    private void QSkill()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !ani.GetBool("isQSkill"))
        {
            ani.SetBool("isQSkill", true);
            ani.SetInteger("isMove", 0);
            agent.speed = 0;

            RaycastHit hit;
            
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                SetDir(hit.point);
            }

            // �÷��̾� ����
            transform.forward = new Vector3(destination.x, -90, destination.z);

            StartCoroutine(QSkillStop());
        }
    }

    // Q ��ų ����Ʈ ����
    IEnumerator QSkillStop()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject cpyObj;
        cpyObj = Instantiate(QSkillObj, transform.position, Quaternion.identity);
        cpyObj.transform.forward = new Vector3(destination.x, -180, destination.z);
        cpyObj.SetActive(true);
        // 1�ʵ� ����
        Destroy(cpyObj, 1);
        ani.SetBool("isQSkill", false);
    }

    // �̵� ������ ����
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        ani.SetInteger("isMove", 2);
    }

    // �������� ���� (���⸸ �ٲٱ� ����)
    private void SetDir(Vector3 dir)
    {
        destination = dir - transform.position;
    }
}
