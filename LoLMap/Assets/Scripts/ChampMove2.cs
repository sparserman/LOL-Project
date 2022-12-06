using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove2 : MonoBehaviour
{
    GameManager gm;

    public Camera cam;              // ���� ī�޶�
    private Animator ani;
    private ChamState state;        // è�Ǿ� ����
    private NavMeshAgent agent;

    private Vector3 destination;    // ������ ��ǥ
    private Transform deInfo;         // ������ ����

    private bool clickCheck = true;        // �� ������ õõ�� �ݺ��ϱ� ����
    private float clickTime = 0;            // Ŭ�� ������

    public GameObject attackTarget;    // ���ݸ�ǥ

    private GameObject lastAttackTarget;    // ���� �ֱ� ���ݸ�ǥ

    
    public GameObject QSkillObj;        // Q ��ų ����Ʈ
    public GameObject WSkillObj;        // W ��ų ����Ʈ
    public GameObject ESkillObj;        // E ��ų ����Ʈ

    private int attackCount = 0;     // ���� ��� ��ȣ
    [SerializeField]
    private bool attackCheck = true;     // ���� ���� ����
    private bool hitCheck = false;      // ���� �Ǿ����� üũ

    private bool RSkillCheck = false;       // R ��ų�� ��������� üũ

    [SerializeField]
    private bool PSkillCool = true;           // �нú� ��� ���� ���� (��Ÿ��)
    private bool QSkillOn = false;           // Q ��ų ��� ���� ����

    private bool QSkillCool = true;          // Q ��ų ��� ���� ���� (��Ÿ��)
    private bool WSkillCool = true;          // W ��ų ��� ���� ���� (��Ÿ��)
    private bool ESkillCool = true;          // E ��ų ��� ���� ���� (��Ÿ��)
    private bool RSkillCool = true;          // R ��ų ��� ���� ���� (��Ÿ��)

    [SerializeField]
    private LayerMask layerMask;        // �� ����
    [SerializeField]
    private LayerMask enemyMask;        // �� ����
    [SerializeField]
    private LayerMask MapMask;        // �� ����


    public float deltaRotation;     // ȸ�� �ӵ�

    [SerializeField]
    private bool moveStop = false;      // �̵� ����

    private bool hide = false;      // ���� ����

    // ��Ÿ�� �ð�
    public float PCoolTime;
    public float QCoolTime;
    public float WCoolTime;
    public float ECoolTime;
    public float RCoolTime;


    void Start()
    {
        gm = GameManager.GetInstance;
        gm.allList.Add(gameObject);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        ani = GetComponent<Animator>();

        
        state.Speed = 340;
        state.AttackSpeed = 1.5f;
    }
    void Update()
    {
        Move();
        QSkill();
        WSkill();
        ESkill();
        RSkill();
        Attack();
        Stop();

        Test();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            PSkillCool = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            PSkillCool = false;
        }
    }

    private void Test()
    {
        // ���� ����
        if (Input.GetKeyDown(KeyCode.T))
        {
            state.AttackSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            state.AttackSpeed -= 0.1f;
        }

        // ���콺 ��
        if(Input.GetMouseButtonUp(1))
        {
            clickCheck = true;
        }

        if(moveStop)
        {
            agent.speed = 0;
        }

        if(RSkillCheck)
        {
            hide = true;
            PSkillCool = true;
        }
    }

    private void Stop()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // ���� ����
            ani.SetBool("isAttack", false);
            attackTarget = null;

            // �̵� ����
            ani.SetInteger("isMove", 0);
            agent.speed = 0;
        }

        if (clickTime >= 0.2f)
        {
            // Ŭ�� ���� ���·� �ٲٱ�
            clickCheck = true;
            clickTime = 0;
        }
        
        if(!clickCheck)
        {
            clickTime += Time.deltaTime;
        }
    }

    // �̵�
    private void Move()
    {
        if (clickCheck)
        {
            ani.SetFloat("AttackSpeed", 1.5f / state.AttackSpeed);
            // ���콺 ��Ŭ�� ��
            if (Input.GetMouseButton(1) && !moveStop)
            {
                // Ŭ�� Ȯ��
                clickCheck = false;
                // ���콺 �̵��� ��ġ ã��
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
                {
                    // ��ǥ�� ����
                    ChampSetDestination(hit.point);
                    deInfo = hit.transform;
                    
                }

                // ���ݿ� ��ġ ã��
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
                {
                    // ��ǥ ����
                    attackTarget = hit.collider.gameObject;
                }
                else
                {
                    // �̵� �� ��ǥ ����
                    attackTarget = null;
                    ani.SetBool("isAttack", false);  // �̵� �� ���� ��� ����
                    ani.SetInteger("AttackMotion", 0);  // �̵� �� ���� ��� ����

                    // ���� ĵ�� ��
                    if (!hitCheck)
                    {
                        attackCheck = true;
                        if (m_AttackDelayCoroutine != null)
                        {
                            Debug.Log("��ž3");
                            StopCoroutine(m_AttackDelayCoroutine);
                            m_AttackDelayCoroutine = null;
                        }
                    }
                }
            }
        }

        if (!ani.GetBool("isQSkill"))
        {
            // ������ȯ�� �̵�
            if (ani.GetInteger("isMove") == 2)
            {
                // �̵�
                moveStop = false;
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

                // ������ȯ
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

                // ������ ���� ��
                if (Vector3.Distance(transform.position, destination) <= 0.5f)
                {
                    ani.SetInteger("isMove", 0);
                    agent.speed = 0;
                }
            }
        }

        // ���콺�� ���� �ٽ� Ŭ�����ɻ��·� �ٲٱ�
        if (Input.GetMouseButtonUp(1))
        {
            clickCheck = true;
            clickTime = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.CompareTag("Wall") && deInfo.CompareTag("Wall"))
        {
            ani.SetInteger("isMove", 0);
            moveStop = true;
        }
    }

    // ����
    private void Attack()
    {
        if (attackTarget != null && !moveStop)
        {
            lastAttackTarget = attackTarget;

            // �нú� ���� ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 8f && PSkillCool)
            {
                PassiveAttack();
            }

            // �⺻ ���� ��ǥ�� ���� ��
            else if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 2.5f && !PSkillCool)
            {
                // �̵� ����
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // �� ���� �Ĵٺ���
                transform.forward = attackTarget.transform.position - transform.position;

                // ���� ���� ���� �϶�
                if (attackCheck)
                {
                    // Q ��ų�� ����� ���¶��
                    if (QSkillOn)
                    {
                        ani.SetBool("isQSkill", true);
                        ani.SetBool("isAttack", false);
                    }
                    else
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
                        ani.SetBool("isAttack", true);
                    }

                    if (m_AttackDelayCoroutine == null)
                    {
                        m_AttackDelayCoroutine = StartCoroutine(AttackDelay());
                    }

                    attackCheck = false;
                }
            }
            else if (!ani.GetBool("isPassive"))
            {
                ani.SetInteger("isMove", 2);
                ani.SetInteger("AttackMotion", 0);
                ani.SetBool("isAttack", false);
                agent.speed = state.Speed * 0.01f;

                if (!hitCheck)
                {
                    attackCheck = true;
                    if (m_AttackDelayCoroutine != null)
                    {
                        StopCoroutine(m_AttackDelayCoroutine);
                        m_AttackDelayCoroutine = null;
                    }
                }

                ChampSetDestination(attackTarget.transform.position);
            }
        }
    }

    // ���� ����
    public void AttackEvent()
    {
        Debug.Log("��Ʈ!");
        Aggro();    // ��׷�
        hitCheck = true;

        if (QSkillOn)
        {
            QSkillOn = false;
            ani.SetBool("isQSkill", false);
            StartCoroutine(SkillCooltime(1));
            QSkillObj.SetActive(false);

            if (m_QSkillEffectCoroutine != null)
            {
                StopCoroutine(m_QSkillEffectCoroutine);
                m_QSkillEffectCoroutine = null;
            }
        }

        if (RSkillCheck)
        {
            state.Speed -= 200;
            RSkillCheck = false;
            hide = false;
            PSkillCool = false;
        }
    }

    public void Aggro()
    {
        // ��׷� ����
        if (lastAttackTarget.tag == "Mob")
        {
            Mob mob = lastAttackTarget.GetComponent<Mob>();
            if (mob.attackTarget == null)
            {
                mob.attackTarget = gameObject;
                mob.HitCheck();
            }
        }
        else if (lastAttackTarget.name == "Baron")
        {
            Baron baron = lastAttackTarget.GetComponent<Baron>();
            if (baron.attackTarget == null)
            {
                baron.attackTarget = gameObject;
            }
        }
        else if (lastAttackTarget.tag == "Dragon")
        {
            Dragon mob = lastAttackTarget.GetComponent<Dragon>();
            if (mob.attackTarget == null)
            {
                mob.attackTarget = gameObject;
                mob.HitCheck();
            }
        }
        lastAttackTarget = null;
    }

    // ���� ������ (���ݼӵ�)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(state.AttackSpeed);
        Debug.Log("������");
        if (!attackCheck)
        {
            attackCheck = true;
            hitCheck = false;
        }

        m_AttackDelayCoroutine = null;
    }

    // Q ��ų
    private void QSkill()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !moveStop && QSkillCool)
        {
            lastAttackTarget = attackTarget;
            QSkillOn = true;
            QSkillCool = false;
            QSkillObj.SetActive(true);
            attackCheck = true;

            if(m_QSkillEffectCoroutine != null)
            {
                StopCoroutine(m_QSkillEffectCoroutine);
                m_QSkillEffectCoroutine = null;
            }

            m_QSkillEffectCoroutine = StartCoroutine(QSkillEffect());
        }
    }

    Coroutine m_QSkillEffectCoroutine = null;
    IEnumerator QSkillEffect()
    {
        yield return new WaitForSeconds(6f);

        QSkillOn = false;
        StartCoroutine(SkillCooltime(1));
        QSkillObj.SetActive(false);
    }

    // W ��ų
    private void WSkill()
    {
        if (Input.GetKeyDown(KeyCode.W) && WSkillCool)
        {
            ani.SetBool("isAttack", false);
            Debug.Log("W");
            GameObject cpyObj;
            cpyObj = Instantiate(WSkillObj, transform.position, Quaternion.identity);
            cpyObj.transform.position += new Vector3(0, 1, 0);
            cpyObj.SetActive(true);
            Destroy(cpyObj, 0.4f);

            WSkillCool = false;
            
            StartCoroutine(SkillCooltime(2));

            if (m_MotionEndCoroutine != null)
            {
                StopCoroutine(m_MotionEndCoroutine);
                m_MotionEndCoroutine = null;
            }
            m_MotionEndCoroutine = StartCoroutine(MotionEnd(2, 0.3f));
        }
    }

    // ��� ��
    Coroutine m_MotionEndCoroutine = null;
    IEnumerator MotionEnd(int p_skill, float p_sec)
    {
        switch(p_skill)
        {
            case 1:
                ani.SetBool("isQSkill", true);
                break;
            case 2:
                ani.SetBool("isWSkill", true);
                break;
            case 3:
                ani.SetBool("isESkill", true);
                break;
            case 4:
                ani.SetBool("isRSkill", true);
                break;
        }

        yield return new WaitForSeconds(p_sec);

        switch (p_skill)
        {
            case 1:
                ani.SetBool("isQSkill", false);
                break;
            case 2:
                ani.SetBool("isWSkill", false);
                break;
            case 3:
                ani.SetBool("isESkill", false);
                break;
            case 4:
                ani.SetBool("isRSkill", false);
                break;
        }

        m_MotionEndCoroutine = null;
    }


    // E ��ų
    private void ESkill()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && ESkillCool)
        {
            moveStop = true;
            ani.SetInteger("isMove", 0);
            ani.SetBool("isAttack", false);
            ESkillCool = false;
            ani.SetBool("isESkill", true);
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
            {
                SetDir(hit.point);
                transform.forward = destination;

                if (m_ESkillCoroutine != null)
                {
                    StopCoroutine(m_ESkillCoroutine);
                    m_ESkillCoroutine = null;
                }
                m_ESkillCoroutine = StartCoroutine(ESkillCoroutine(hit.point));

                StartCoroutine(SkillCooltime(3));
            }
            else
            {
                return;
            }
        }
        
    }

    Coroutine m_ESkillCoroutine = null;
    IEnumerator ESkillCoroutine(Vector3 p_dir)
    {
        yield return new WaitForSeconds(0.2f);
        
        GameObject cpyobj = Instantiate(ESkillObj, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        cpyobj.GetComponent<Shot2>().SetShotInfo(p_dir, 12, 10);
        cpyobj.SetActive(true);
        ani.SetBool("isESkill", false);
        moveStop = false;
    }

    // R ��ų
    private void RSkill()
    {
        if(Input.GetKeyDown(KeyCode.R) && !moveStop && RSkillCool)
        {
            Debug.Log("�� ���");
            RSkillCool = false;
            if(m_RSkillCoroutine != null)
            {
                StopCoroutine(m_RSkillCoroutine);
                m_RSkillCoroutine = null;
            }
            m_RSkillCoroutine = StartCoroutine(RSkillCoroutine());

            StartCoroutine(SkillCooltime(4));
        }
    }

    Coroutine m_RSkillCoroutine = null;
    IEnumerator RSkillCoroutine()
    {
        yield return new WaitForSeconds(1f);
        PSkillCool = true;
        RSkillCheck = true;
        state.Speed += 200;

        if (m_RSkillEndCoroutine != null)
        {
            StopCoroutine(m_RSkillEndCoroutine);
            m_RSkillEndCoroutine = null;
        }
        m_RSkillEndCoroutine = StartCoroutine(RSkillEndCoroutine());
    }

    Coroutine m_RSkillEndCoroutine = null;
    IEnumerator RSkillEndCoroutine()
    {
        yield return new WaitForSeconds(10f);
        if (RSkillCheck)
        {
            state.Speed -= 200;
            RSkillCheck = false;
            PSkillCool = false;
            hide = false;
        }
    }

    private void PassiveAttack()
    {
        lastAttackTarget = attackTarget;

        PSkillCool = false;
        // �̵� ����
        ani.SetInteger("isMove", 0);
        moveStop = true;
        // �� ���� �Ĵٺ���
        transform.forward = attackTarget.transform.position - transform.position;

        // ���� ��� ����
        ani.SetInteger("AttackMotion", 0);

        // ���� ���� ���� �϶�
        if (attackCheck)
        {
            ani.SetBool("isPassive", true);

            if (m_AttackDelayCoroutine != null)
            {
                StopCoroutine(m_AttackDelayCoroutine);
                m_AttackDelayCoroutine = null;
            }

            ani.SetBool("isAttack", true);

            m_AttackDelayCoroutine = StartCoroutine(AttackDelay());

            attackCheck = false;
        }

        if (m_JumpCoroutine != null)
        {
            StopCoroutine(m_JumpCoroutine);
            m_JumpCoroutine = null;
        }
        m_JumpCoroutine = StartCoroutine(Jump());
    }

    Coroutine m_JumpCoroutine = null;
    IEnumerator Jump()
    {
        Vector3 p_mypos = transform.position;
        Vector3 p_targetpos = attackTarget.transform.position;

        p_targetpos -= (p_targetpos - p_mypos).normalized;

        float i = Vector3.Distance(p_mypos, p_targetpos);
        float divval = 1 / (i * 0.06f);
        float t = 0;
        ani.SetFloat("JumpSpeed", 4f - i * 0.25f);

        while (true)
        {
            yield return null;
            t += divval * Time.deltaTime;

            transform.position = Bezier(p_mypos, p_mypos + new Vector3(0, i * 0.2f, 0) + (transform.forward * i * 0.4f)
                , p_targetpos + new Vector3(0, i * 0.2f, 0) - (transform.forward * i * 0.4f), p_targetpos, t);

            if (t >= 1)
            {
                break;
            }
        }
        ani.SetBool("isPassive", false);
        ani.speed = 1;
        moveStop = false;

        m_JumpCoroutine = null;
    }

    // ������
    private Vector3 Bezier(Vector3 p_1,Vector3 p_2,
        Vector3 p_3,Vector3 p_4, float p_value)
    {
        Vector3 A = Vector3.Lerp(p_1, p_2, p_value);
        Vector3 B = Vector3.Lerp(p_2, p_3, p_value);
        Vector3 C = Vector3.Lerp(p_3, p_4, p_value);

        Vector3 D = Vector3.Lerp(A, B, p_value);
        Vector3 E = Vector3.Lerp(B, C, p_value);

        Vector3 F = Vector3.Lerp(D, E, p_value);

        return F;
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

    IEnumerator SkillCooltime(int p_skill)
    {
        switch (p_skill)
        {
            case 0:
                yield return new WaitForSeconds(PCoolTime);
                PSkillCool = true;
                break;
            case 1:
                yield return new WaitForSeconds(QCoolTime);
                QSkillCool = true;
                break;
            case 2:
                yield return new WaitForSeconds(WCoolTime);
                WSkillCool = true;
                break;
            case 3:
                yield return new WaitForSeconds(ECoolTime);
                ESkillCool = true;
                break;
            case 4:
                yield return new WaitForSeconds(RCoolTime);
                RSkillCool = true;
                break;
        }
    }
}
