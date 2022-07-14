using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove : BaseActor
{
    public Camera cam;              // ���� ī�޶�
    private Animator ani;
    private ChamState state;        // è�Ǿ� ����
    private NavMeshAgent agent;

    private Vector3 destination;    // ������ ��ǥ

    private bool clickCheck = true;        // �� ������ õõ�� �ݺ��ϱ� ����
    private float clickTime = 0;            // Ŭ�� ������

    public GameObject attackTarget;    // ���ݸ�ǥ

    public PoppyPassiveSkill PassiveSkillObj;        // Passive ��ų ����Ʈ
    public GameObject Shield;   // ����

    public GameObject QSkillObj;        // Q ��ų ����Ʈ
    public GameObject WSkillObj;        // W ��ų ����Ʈ
    public GameObject RSkillObj;        // R ��ų ���� ����Ʈ
    //public Shot RSkillScript;              // R ��ų ���� ��ũ��Ʈ
    public GameObject RSkillEffectObj;  // R ��ų ȸ�� ����Ʈ

    private int attackCount = 0;     // ���� ��� ��ȣ
    private bool attackCheck = true;     // ���� ���� ����
    private bool hitCheck = false;      // ���� �Ǿ����� üũ

    [SerializeField]
    private GameObject eSkillTarget;    // E ��ų ��ǥ
    private bool ESkillCheck = false;       // E ��ų�� ��������� üũ


    private bool RSkillCheck = false;       // R ��ų�� ��������� üũ
    private float RSkillTime = 0;           // R ��ų ���� �ð�

    [SerializeField]
    private bool PassiveOn = true;           // �нú� ��� ���� ����

    [SerializeField]
    private LayerMask layerMask;        // �� ����
    [SerializeField]
    private LayerMask enemyMask;        // �� ����
    [SerializeField]
    private LayerMask MapMask;        // �� ����


    void Start()
    {
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

            // E ��ų ����
            if(!ani.GetBool("isESkill"))
            {
                eSkillTarget = null;
                ESkillCheck = false;
            }
        }

        if (clickTime >= 0.3f)
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
            if (Input.GetMouseButton(1) && !ani.GetBool("isQSkill") & !ani.GetBool("isESkill")
                && !ani.GetBool("isPassive") && !RSkillCheck)
            {
                // Ŭ�� Ȯ��
                clickCheck = false;
                // ���콺 �̵��� ��ġ ã��
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
                {
                    // ��ǥ�� ����
                    ChampSetDestination(hit.point);
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
                    eSkillTarget = null;
                    ani.SetBool("isAttack", false);  // �̵� �� ���� ��� ����
                    ani.SetInteger("AttackMotion", 0);  // �̵� �� ���� ��� ����

                    // ���� ĵ�� ��
                    if (!hitCheck)
                    {
                        attackCheck = true;
                        if (m_AttackDelayCoroutine != null)
                        {
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
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

                // ������ȯ
                transform.forward = dir;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Wall")
        {
            ani.SetInteger("isMove", 0);
            agent.speed = 0;
        }
    }

    // ����
    private void Attack()
    {
        if (attackTarget != null && !ani.GetBool("isCharge") && !ani.GetBool("isRSkill") && !RSkillCheck)
        {
            // �нú� ���� ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 5f && PassiveOn)
            {
                // �̵� ����
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // �� ���� �Ĵٺ���
                transform.forward = attackTarget.transform.position - transform.position;

                // ���� ���� ���� �϶�
                if (attackCheck && !ani.GetBool("isRSkill"))
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

            }

            // �⺻ ���� ��ǥ�� ���� ��
            else if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 2.5f && !PassiveOn)
            {
                // �̵� ����
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // �� ���� �Ĵٺ���
                transform.forward = attackTarget.transform.position - transform.position;

                // ���� ���� ���� �϶�
                if (attackCheck && !ani.GetBool("isRSkill"))
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

                    if (m_AttackDelayCoroutine != null)
                    {
                        StopCoroutine(m_AttackDelayCoroutine);
                        m_AttackDelayCoroutine = null;
                    }

                    ani.SetBool("isAttack", true);

                    m_AttackDelayCoroutine = StartCoroutine(AttackDelay());

                    attackCheck = false;
                    ani.SetInteger("AttackMotion", attackCount);

                }
            }
            else
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

    // ���� ��Ʈ
    public void AttackEvent()
    {
        Debug.Log("��Ʈ!");
        hitCheck = true;
    }

    // ���� ������ (���ݼӵ�)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(state.AttackSpeed);
        if (!attackCheck)
        {
            attackCheck = true;
            hitCheck = false;
        }
    }

    // �нú�
    private void PassiveEvent(int num)
    {
        switch(num)
        {
            case 0:
                PoppyPassiveSkill cpyObj;
                cpyObj = Instantiate<PoppyPassiveSkill>(PassiveSkillObj
                    , new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity);
                cpyObj.transform.forward = attackTarget.transform.position;
                cpyObj.transform.rotation = new Quaternion(0,0,0,90);
                cpyObj.gameObject. SetActive(true);
                cpyObj.Init(attackTarget, this);
                // ���� ����
                Shield.SetActive(false);
                break;
            case 1:
                PassiveOn = false;
                ani.SetBool("isPassive", false);
                StartCoroutine(PassiveTime());
                break;
                
        }
    }

    // �нú� ��Ÿ��
    IEnumerator PassiveTime()
    {
        yield return new WaitForSeconds(3f);
        PassiveOn = true;
        Shield.SetActive(true);
    }

    // Q ��ų
    private void QSkill()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !ani.GetBool("isQSkill") && !ani.GetBool("isESkill")
            && !ani.GetBool("isRSkill") && !RSkillCheck && !ani.GetBool("isPassive"))
        {
            ani.SetBool("isQSkill", true);
            ani.SetInteger("isMove", 0);
            ani.SetInteger("AttackMotion", 0);
            ani.SetBool("isAttack", false);
            attackTarget = null;
            agent.speed = 0;

            RaycastHit hit;
            
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
            {
                SetDir(hit.point);
            }

            // �÷��̾� ����
            transform.forward = destination;
        }
    }

    // Q ��ų ����Ʈ ����
    public void QSkillEvent(int num)
    {
        if (num == 0)
        {
            GameObject cpyObj;
            cpyObj = Instantiate(QSkillObj, transform.position, Quaternion.identity);
            cpyObj.transform.forward = new Vector3(destination.x, -180, destination.z);
            cpyObj.SetActive(true);
            // 1�ʵ� ����
            Destroy(cpyObj, 1.5f);
        }
        if (num == 1)
        {
            ani.SetBool("isQSkill", false);
        }
    }

    // W ��ų
    private void WSkill()
    {
        if (Input.GetKeyDown(KeyCode.W) && !ani.GetBool("isPassive"))
        {
            Debug.Log("W");
            GameObject cpyObj;
            cpyObj = Instantiate(WSkillObj, transform.position, Quaternion.identity);
            cpyObj.transform.forward = new Vector3(destination.x, 90, destination.z);
            cpyObj.SetActive(true);

            if(m_WSkillEffectCoroutine != null)
            {
                StopCoroutine(m_WSkillEffectCoroutine);
                m_WSkillEffectCoroutine = null;
            }
            m_WSkillEffectCoroutine = StartCoroutine(WSkillEffect(cpyObj));
        }
    }

    // W ��ų ����
    Coroutine m_WSkillEffectCoroutine = null;
    IEnumerator WSkillEffect(GameObject obj)
    {
        ani.SetBool("isWSkill", true);
        state.Speed += 300;
        float t = 0;
        while(true)
        {
            yield return null;
            t += Time.deltaTime;

            if(t > 0.5f)
            {
                ani.SetBool("isWSkill", false);
            }

            obj.transform.position = transform.position;
            if(t >= 2)
            {
                break;
            }
        }
        state.Speed -= 300;
        Destroy(obj);
    }

    // E ��ų
    private void ESkill()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && !ESkillCheck && !ani.GetBool("isESkill")
            && !ani.GetBool("isPassive") && !ani.GetBool("isRSkill") && !RSkillCheck)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
            {
                ESkillCheck = true;
                SetDir(hit.point);
                eSkillTarget = hit.collider.gameObject;
            }
            else
            {
                return;
            }
        }

        if(ESkillCheck && eSkillTarget != null)
        {
            // ��Ÿ� �ȿ� ���� �� �̵�
            if (Vector3.Distance(transform.position, eSkillTarget.transform.position) > 5f)
            {
                ani.SetInteger("isMove", 2);
                ani.SetBool("isAttack", false);
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(eSkillTarget.transform.position.x, transform.position.y, eSkillTarget.transform.position.z) - transform.position;
                transform.forward = dir;

                ChampSetDestination(eSkillTarget.gameObject.transform.position);
            }

            // ��Ÿ� �ȿ� ������ ��
            else
            {
                Debug.Log("��Ÿ� ������ ����");
                ESkillCheck = false;

                ani.SetBool("isESkill", true);
                ani.SetInteger("isMove", 0);
                ani.SetInteger("AttackMotion", 0);
                ani.SetBool("isAttack", false);
                attackTarget = null;
                agent.speed = 0;

                // �÷��̾� ����
                var dir = new Vector3(eSkillTarget.transform.position.x, transform.position.y, eSkillTarget.transform.position.z) - transform.position;
                transform.forward = dir;
                


                if (m_MoveCoroutine != null)
                {
                    StopCoroutine(m_MoveCoroutine);
                    m_MoveCoroutine = null;
                }

                // �ڿ� �� Ȯ��
                RaycastHit MapHit;
                if (Physics.Raycast(eSkillTarget.transform.position, transform.forward, out MapHit, 5f, layerMask))
                {
                    // ������ �̵�
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(MapHit.point,
                        Vector3.Distance(transform.position, MapHit.transform.position) * 0.03f));
                }
                else
                {
                    // ������� �̵�
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(eSkillTarget.transform.position + (transform.forward * 4),
                        Vector3.Distance(transform.position, eSkillTarget.transform.position + (transform.forward * 4)) * 0.04f));
                }
            }
        }
    }

    // ��ǥ���� ������ �ð��� �°� �̵�
    Coroutine m_MoveCoroutine = null;
    IEnumerator SetDirectionMove( Vector3 p_targetwpos, float p_sec )
    {
        Vector3 Currpos = transform.position;
        Vector3 targetpos = p_targetwpos;
        float t = 0;
        float divval = 1 / p_sec;
        Vector3 temppos;

        bool flag = false;

        while (true)
        {
            yield return null;

            t += divval * Time.deltaTime;
            temppos = Vector3.Lerp(Currpos, targetpos, t);
            transform.position = temppos - transform.forward;

            RaycastHit hit;
            // �浹 üũ
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                if(hit.collider.name != "Poppy")
                {
                    flag = true;
                }
                
            }

            // �浹�ߴٸ�
            if (flag)
            {
                eSkillTarget.transform.position = temppos;
            }
            if( (targetpos - temppos).sqrMagnitude <= 0.5f )
            {
                break;
            }
        }

        // ����ó���κ�
        transform.position = targetpos - transform.forward * 2;
        eSkillTarget.transform.position = temppos;

        eSkillTarget = null;

        ani.SetBool("isESkill", false);
    }


    // R ��ų
    private void RSkill()
    {
        if(Input.GetKeyDown(KeyCode.R) && !ani.GetBool("isQSkill") && !ani.GetBool("isESkill")
            && !ani.GetBool("isRSkill") && !ani.GetBool("isCharge") && !RSkillCheck && !ani.GetBool("isPassive"))
        {
            if(m_RSkillCoroutine != null)
            {
                StopCoroutine(m_RSkillCoroutine);
                m_RSkillCoroutine = null;
            }
            ani.SetBool("isRSkill", true);
            m_RSkillCoroutine = StartCoroutine(ChargeCheck());
        }
    }

    // R ��ų
    Coroutine m_RSkillCoroutine = null;
    IEnumerator ChargeCheck()
    {
        // ȸ�� ����Ʈ ����
        GameObject cpyObj;
        Vector3 temppos;
        temppos = transform.position;
        temppos.y = 3;
        cpyObj = Instantiate(RSkillEffectObj, temppos, Quaternion.identity);
        cpyObj.SetActive(true);

        // ����
        ani.SetBool("RSkillCancel", false);
        ani.SetBool("isRSkill", true);
        ani.SetInteger("AttackMotion", 0);
        ani.SetBool("isAttack", false);
        attackTarget = null;

        float t = 0;
        while (true)
        {
            yield return null;

            // ȸ������Ʈ ���� �ٴϱ�
            temppos = transform.position;
            temppos.y = 3;
            cpyObj.transform.position = temppos;

            t += Time.deltaTime;
            // �� ������ �ִ��� üũ
            if (!Input.GetKey(KeyCode.R))
            {
                // ���� �����ߴٸ�
                if (t >= 0.5f)
                {
                    ani.SetBool("isCharge", true);
                    RSkillTime = t;
                }
                else
                {
                    ani.SetBool("isCharge", false);
                }
                Destroy(cpyObj);
                RSkillUse();
                break;
            }

            if(t > 5)
            {
                ani.SetBool("isRSkill", false);
                ani.SetBool("RSkillCancel", true);
                Destroy(cpyObj);
                break;
            }
        }
    }

    // R ��ų
    private void RSkillUse()
    {
        
        ani.SetBool("isRSkill", false);
        ani.SetInteger("isMove", 0);
        agent.speed = 0;

        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
        {
            SetDir(hit.point);
        }

        // �÷��̾� ����
        transform.forward = new Vector3(destination.x, -90, destination.z);

        
    }

    public void RSkillEvent(int num)
    {
        switch(num)
        {
            case 0: // �������� R ��ų
                Debug.Log("R");
                GameObject cpyObj;
                cpyObj = Instantiate(RSkillObj, transform.position, Quaternion.identity);
                cpyObj.transform.forward = new Vector3(destination.x, 0, destination.z);
                cpyObj.SetActive(true);
                // ����
                Destroy(cpyObj, 0.1f);
                break;
            case 1: // R ��ų ���� �� �̵� ���� ���·� �ٲٱ�
                RSkillCheck = false;
                break;
            case 2: // ������ R ��ų
                Debug.Log("Charge R");
                //Shot cpyObj2;
                //cpyObj2 = Instantiate<Shot>(RSkillScript, transform.position, Quaternion.identity);
                //cpyObj2.transform.forward = new Vector3(destination.x, 0, destination.z);

                // R ��ų �ִ� ����
                //if(RSkillTime > 1.5f)
                //{
                //    RSkillTime = 1.5f;
                //}

                //cpyObj2.Fire(RSkillTime);

                //cpyObj2.gameObject.SetActive(true);
                //RSkillTime = 0;
                break;
            case 3:
                RSkillCheck = true;
                break;
        }

        ani.SetBool("isCharge", false);
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


    protected void ClickEvent(KeyCode p_code)
    {
        switch(p_code)
        {
            case KeyCode.Q:
                Click_WSkillEvent();
                break;
        }
    }

    public override void Click_QSkillEvent()
    {
        
    }

    public override void Click_WSkillEvent()
    {
        
    }

    public override void Click_ESkillEvent()
    {
        
    }

    public override void Click_RSkillEvent()
    {
        
    }
}
