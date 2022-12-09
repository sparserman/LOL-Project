using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum ChampNum
{
    POPPY = 0, RENGAR
}

public class ChampController : MonoBehaviour
{
    GameManager gm;

    public ChampNum champNum;       // è�Ǿ� ��ȣ

    private Animator ani;
    private ChamState state;        // è�Ǿ� ����
    private NavMeshAgent agent;

    private Vector3 destination;    // ������ ��ǥ

    private bool clickCheck = true;        // �� ������ õõ�� �ݺ��ϱ� ����
    private float clickTime = 0;            // Ŭ�� ������

    public GameObject attackTarget;    // ���ݸ�ǥ

    private GameObject lastAttackTarget;    // ���� �ֱ� ���ݸ�ǥ

    private int attackCount = 0;     // ���� ��� ��ȣ
    private bool attackCheck = true;     // ���� ���� ����
    private bool hitCheck = false;      // ���� �Ǿ����� üũ

    [SerializeField]
    private GameObject eSkillTarget;    // E ��ų ��ǥ

    private bool RSkillCheck = false;       // R ��ų�� ��������� üũ
    private float RSkillTime = 0;           // R ��ų ���� �ð�

    [SerializeField]
    private bool PSkillCool = true;           // �нú� ��� ���� ����

    private bool QSkillCool = true;          // Q ��ų ��� ���� ���� (��Ÿ��)
    private bool WSkillCool = true;          // W ��ų ��� ���� ���� (��Ÿ��)
    private bool ESkillCool = true;          // E ��ų ��� ���� ���� (��Ÿ��)
    private bool RSkillCool = true;          // R ��ų ��� ���� ���� (��Ÿ��)

    public GameObject shield;

    [SerializeField]
    private LayerMask wallMask;        // �� ����
    [SerializeField]
    private LayerMask enemyMask;        // �� ����
    [SerializeField]
    private LayerMask mapMask;        // �� ����


    public float deltaRotation;     // ȸ�� �ӵ�

    [SerializeField]
    private bool moveStop = false;      // �̵� ����

    public bool hide = false;      // ���� ����

    // ��Ÿ�� �ð�
    public float PCoolTime;
    public float QCoolTime;
    public float WCoolTime;
    public float ECoolTime;
    public float RCoolTime;

    // ���� ���� ����
    public bool inOperation = false;

    // ü�� ��
    public GameObject hpBar;

    private bool die = false;

    void Start()
    {
        gm = GameManager.GetInstance;
        gm.allList.Add(gameObject);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        ani = GetComponent<Animator>();

        InitSetting();
    }

    void Update()
    {
        if (!die)
        {
            if (inOperation)
            {
                Click();
                QSkill();
                WSkill();
                ESkill();
                RSkill();
            }
            Attack();
            Stop();
            Move();
            HpControl();

            Test();

            // ����
            ani.SetFloat("AttackSpeed", 1.5f / state.AttackSpeed);
        }
    }

    private void InitSetting()
    {
        state.MaxHP = 600;
        state.HP = 600;
        state.Speed = 340;
        state.AttackSpeed = 1.5f;
        state.Attack = 50;

        // HPBar ���� �� �ο�
        hpBar = Instantiate(Resources.Load("Prefabs/" + "HPBar") as GameObject);
        hpBar.transform.parent = GameObject.Find("GUI").transform;
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
        if (Input.GetMouseButtonUp(1))
        {
            clickCheck = true;
        }
    }

    // HP ���������� ����
    private void HpControl()
    {
        // ü�� ����
        hpBar.GetComponent<Scrollbar>().size = state.HP / state.MaxHP;
    }

    private void Stop()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // ���� ����
            ani.SetBool("isAttack", false);
            attackTarget = null;

            // �̵� ����
            ani.SetInteger("isMove", 0);
            agent.speed = 0;

            // E ��ų ����
            if (!ani.GetBool("isESkill"))
            {
                eSkillTarget = null;
            }
        }

        if (clickTime >= 0.1f)
        {
            // Ŭ�� ���� ���·� �ٲٱ�
            clickCheck = true;
            clickTime = 0;
        }

        if (!clickCheck)
        {
            clickTime += Time.deltaTime;
        }
    }

    // Ŭ�� ��ġ �Ǵ� ��� ����
    private void Click()
    {
        // ��Ŷ ���� ��
        if (clickCheck)
        {
            // ���콺 ��Ŭ�� ��
            if (Input.GetMouseButton(1) && !moveStop)
            {
                // Ŭ�� Ȯ��
                clickCheck = false;
                // ���콺 �̵��� ��ġ ã��
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
                {
                    if(gm.serverConnected)
                    {
                        int protocol = 0;
                        switch (champNum)
                        {
                            case ChampNum.POPPY:
                                protocol = Sever.Manager_Protocol.Instance.Packing_prot(Pro.GAME_Poppy, Pro.MOVE);
                                break;
                            case ChampNum.RENGAR:
                                protocol = Sever.Manager_Protocol.Instance.Packing_prot(Pro.GAME_Rengar, Pro.MOVE);
                                break;
                        }
                        Sever.Instance.MovePack(protocol, hit.point);


                        // ��Ŷ���� ��ġ ���� hit.point
                    }
                    else
                    {
                        SetMovePos(hit.point);
                    }
                }

                // ���ݿ� ��ġ ã��
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
                {
                    // ����Ʈ���� �� ã��
                    int num = -1;
                    for (int i = 0; i < gm.allList.Count; i++)
                    {
                        if (gm.allList[i] == hit.collider.gameObject)
                        {
                            num = i;
                        }
                    }

                    // ��ǥ�� ��Ŷ���� ���ڷ� ġȯ�ؼ� ������
                    if (gm.serverConnected)
                    {
                        int protocol = 0;
                        switch (champNum)
                        {
                            case ChampNum.POPPY:
                                protocol = Sever.Manager_Protocol.Instance.Packing_prot(Pro.GAME_Poppy, Pro.ATTACK);
                                break;
                            case ChampNum.RENGAR:
                                protocol = Sever.Manager_Protocol.Instance.Packing_prot(Pro.GAME_Rengar, Pro.ATTACK);
                                break;
                        }
                        Sever.Instance.AttackPack(protocol, (int)champNum);

                        // num ������
                    }
                    else
                    {
                        SetAttackTarget(num);
                    }
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

    // Ŭ�� ��ġ�� �̵�
    private void Move()
    {
        if (!moveStop)
        {
            // ������ȯ�� �̵�
            if (ani.GetInteger("isMove") == 2)
            {
                // �̵�
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
    }

    // �̵� ��ġ �� ���� ��� ����
    public void SetMovePos(Vector3 p_pos)
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
                Debug.Log("��ž3");
                StopCoroutine(m_AttackDelayCoroutine);
                m_AttackDelayCoroutine = null;
            }
        }

        // ��ǥ�� ����
        ChampSetDestination(p_pos);
        //deInfo = hit.transform;   // �̰� ��¼��
    }

    public void SetAttackTarget(int p_champNum)
    {
        // ��ǥ ����
        if (!RSkillCheck)
        {
            attackTarget = gm.allList[p_champNum].gameObject;
        }
    }

    // ����
    private void Attack()
    {
        if (attackTarget != null && !moveStop)
        {
            lastAttackTarget = attackTarget;

            // ��밡 �׾��� �� üũ
            if(attackTarget.GetComponent<ChampController>().die)
            {
                // �⺻ �������� ����
                attackTarget = null;
                ani.Play("model|Idle1_Base_model");
                return;
            }

            // �нú�
            ChampPassive((int)champNum);

            // �⺻ ���� ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 2.5f && !PSkillCool)
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

    public void ChampPassive(int p_champNum)
    {
        switch(p_champNum)
        {
            case (int)ChampNum.POPPY:
                // �нú� ���� ��ǥ�� ���� ��
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 5f && PSkillCool)
                {
                    // �̵� ����
                    ani.SetInteger("isMove", 0);
                    agent.speed = 0;
                    // �� ���� �Ĵٺ���
                    transform.forward = attackTarget.transform.position - transform.position;

                    // ���� ��� ����
                    ani.SetInteger("AttackMotion", 0);

                    // ���� ���� ���� �϶�
                    if (attackCheck)
                    {
                        moveStop = true;
                        PSkillCool = false;
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
                break;
            case (int)ChampNum.RENGAR:
                // �нú� ���� ��ǥ�� ���� ��
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 8f && PSkillCool)
                {
                    PassiveAttack();
                }
                break;
        }
    }

    // ���� ����
    public void AttackEvent()
    {
        Aggro();    // ��׷�
        hitCheck = true;
        if (attackTarget.layer.Equals(9))
        {
            attackTarget.GetComponent<ChampController>().Damaged(state.Attack);
        }
    }

    public void Damaged(float p_damage)
    {
        state.HP -= p_damage;
        if(state.HP < 0)
        {
            // ���
            die = true;
            ani.Play("Die");
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

        m_AttackDelayCoroutine = null;
    }

    // �нú�
    public void PassiveEvent(int num)
    {
        switch (num)
        {
            case 0:
                GameObject cpyObj;
                cpyObj = Instantiate<GameObject>(Resources.Load("Prefabs/" + "PoppyPassiveEffect") as GameObject
                    , new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
                cpyObj.transform.forward = attackTarget.transform.position;
                cpyObj.transform.rotation = new Quaternion(0, 0, 0, 90);
                cpyObj.gameObject.SetActive(true);
                cpyObj.GetComponent<PoppyPassiveSkill>().Init(attackTarget, this);
                // ���� ����
                shield.SetActive(false);
                break;
            case 1:
                ani.SetBool("isPassive", false);
                moveStop = false;

                StartCoroutine(SkillCooltime(0));
                break;
        }
    }

    // Q ��ų
    private void QSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !moveStop && QSkillCool)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
            {
                if(gm.serverConnected)
                {
                    // ���� ���Ѱ� �� ������
                }
                else
                {
                    QSkillPlay(hit.point);
                }
            }
        }
    }

    // Q ��ų �����ϱ�
    public void QSkillPlay(Vector3 p_pos)
    {
        SetDir(p_pos);

        moveStop = true;
        ani.SetBool("isQSkill", true);
        ani.SetInteger("isMove", 0);
        ani.SetInteger("AttackMotion", 0);
        ani.SetBool("isAttack", false);
        //attackTarget = null;
        agent.speed = 0;
        QSkillCool = false;

        // �÷��̾� ����
        transform.forward = destination;

        StartCoroutine(SkillCooltime(1));
    }

    // Q ��ų ����Ʈ ����
    public void QSkillEvent(int num)
    {
        switch (num)
        {
            case 0:
                GameObject cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyQEffectPos") as GameObject);
                cpyObj.transform.position = transform.position;
                cpyObj.transform.forward = new Vector3(destination.x, -180, destination.z);
                // 1.5�ʵ� ����
                Destroy(cpyObj, 1.5f);
                break;
            case 1:
                ani.SetBool("isQSkill", false);
                moveStop = false;
                break;
        }
    }

    // W ��ų
    private void WSkill()
    {
        if (Input.GetKeyDown(KeyCode.W) && !moveStop && WSkillCool)
        {
            WSkillCool = false;
            Debug.Log("W");
            GameObject cpyObj;
            cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyWEffect") as GameObject
                , transform.position, Quaternion.identity);
            cpyObj.transform.forward = new Vector3(0, 90, 0);

            if (m_WSkillEffectCoroutine != null)
            {
                StopCoroutine(m_WSkillEffectCoroutine);
                m_WSkillEffectCoroutine = null;
            }
            m_WSkillEffectCoroutine = StartCoroutine(WSkillEffect(cpyObj));

            StartCoroutine(SkillCooltime(2));
        }
    }

    // W ��ų ����
    Coroutine m_WSkillEffectCoroutine = null;
    IEnumerator WSkillEffect(GameObject obj)
    {
        ani.SetBool("isWSkill", true);
        state.Speed += 300;
        float t = 0;
        while (true)
        {
            yield return null;
            t += Time.deltaTime;

            if (t > 0.5f)
            {
                ani.SetBool("isWSkill", false);
            }

            obj.transform.position = transform.position;
            if (t >= 2)
            {
                break;
            }
        }
        state.Speed -= 300;
        Destroy(obj);

        m_WSkillEffectCoroutine = null;
    }

    // E ��ų
    private void ESkill()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && !moveStop && ESkillCool)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
            {
                SetDir(hit.point);
                eSkillTarget = hit.collider.gameObject;
            }
            else
            {
                return;
            }
        }

        if (eSkillTarget != null)
        {
            if (moveStop || !ESkillCool)
            {
                return;
            }
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

                moveStop = true;
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
                if (Physics.Raycast(eSkillTarget.transform.position, transform.forward, out MapHit, 5f, wallMask))
                {
                    // ������ �̵�
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(MapHit.point,
                        Vector3.Distance(transform.position, MapHit.transform.position) * 0.03f));

                    Debug.Log("�ڿ� �� ����");
                }
                else
                {
                    // ������� �̵�
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(eSkillTarget.transform.position + (transform.forward * 4),
                        Vector3.Distance(transform.position, eSkillTarget.transform.position + (transform.forward * 4)) * 0.04f));

                    Debug.Log("�ڿ� �� ����");
                }
            }
        }
    }

    // ��ǥ���� ������ �ð��� �°� �̵�
    Coroutine m_MoveCoroutine = null;
    IEnumerator SetDirectionMove(Vector3 p_targetwpos, float p_sec)
    {
        Debug.Log("�ڷ�ƾ ����");
        Vector3 Currpos = transform.position;
        Vector3 targetpos = p_targetwpos;
        float t = 0;
        float divval = 1 / p_sec;
        Vector3 temppos;

        bool flag = false;

        // ��Ÿ��
        ESkillCool = false;
        moveStop = true;

        while (true)
        {
            yield return null;
            Debug.Log("�ڷ�ƾ �����");
            t += divval * Time.deltaTime;
            temppos = Vector3.Lerp(Currpos, targetpos, t);
            transform.position = temppos - transform.forward;

            RaycastHit hit;
            // �浹 üũ
            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.1f))
            {
                if (!hit.collider.CompareTag("Blue"))
                {
                    flag = true;
                }

            }

            // �浹�ߴٸ�
            if (flag)
            {
                eSkillTarget.transform.position = temppos;
            }
            if ((targetpos - temppos).sqrMagnitude <= 0.5f)
            {
                break;
            }
        }

        // ����ó���κ�
        transform.position = targetpos - transform.forward * 2;
        eSkillTarget.transform.position = temppos;

        moveStop = false;
        eSkillTarget = null;

        ani.SetBool("isESkill", false);

        StartCoroutine(SkillCooltime(3));
    }


    // R ��ų
    private void RSkill()
    {
        if (Input.GetKeyDown(KeyCode.R) && !moveStop && !RSkillCheck && RSkillCool)
        {
            RSkillCool = false;
            if (m_RSkillCoroutine != null)
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
        cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyRChargeEffect") as GameObject
            , temppos, Quaternion.identity);
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
            temppos.y = transform.position.y + 3;
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

            if (t > 5)
            {
                ani.SetBool("isRSkill", false);
                ani.SetBool("RSkillCancel", true);
                Destroy(cpyObj);

                float temp = RCoolTime;
                RCoolTime = RCoolTime * 0.3f;
                StartCoroutine(SkillCooltime(4));
                RCoolTime = temp;
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

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
        {
            SetDir(hit.point);
        }

        // �÷��̾� ����
        transform.forward = new Vector3(destination.x, -90, destination.z);


    }

    public void RSkillEvent(int num)
    {
        switch (num)
        {
            case 0: // �������� R ��ų
                RSkillCool = false;
                Debug.Log("R");
                GameObject cpyObj;
                cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyREffectPos") as GameObject
                    , transform.position, Quaternion.identity);
                cpyObj.transform.forward = new Vector3(destination.x, 0, destination.z);
                cpyObj.SetActive(true);
                // ����
                Destroy(cpyObj, 0.1f);
                break;
            case 1: // R ��ų ���� �� �̵� ���� ���·� �ٲٱ�
                RSkillCheck = false;
                moveStop = false;
                StartCoroutine(SkillCooltime(4));
                break;
            case 2: // ������ R ��ų
                Debug.Log("Charge R");
                GameObject cpyObj2;
                cpyObj2 = Instantiate<GameObject>(Resources.Load("Prefabs/" + "PoppyREffectChargePos") as GameObject
                    , transform.position, Quaternion.identity);
                cpyObj2.transform.forward = new Vector3(destination.x, 0, destination.z);

                // R ��ų �ִ� ����
                if (RSkillTime > 1.5f)
                {
                    RSkillTime = 1.5f;
                }

                cpyObj2.GetComponent<Shot>().Fire(RSkillTime);

                cpyObj2.gameObject.SetActive(true);
                RSkillTime = 0;
                break;
            case 3:
                Debug.Log("Ʈ��");
                moveStop = true;
                break;
        }

        ani.SetBool("isCharge", false);
        StartCoroutine(SkillCooltime(4));
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
                if (champNum == ChampNum.POPPY)
                {
                    shield.SetActive(true);
                }
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
    private Vector3 Bezier(Vector3 p_1, Vector3 p_2,
        Vector3 p_3, Vector3 p_4, float p_value)
    {
        Vector3 A = Vector3.Lerp(p_1, p_2, p_value);
        Vector3 B = Vector3.Lerp(p_2, p_3, p_value);
        Vector3 C = Vector3.Lerp(p_3, p_4, p_value);

        Vector3 D = Vector3.Lerp(A, B, p_value);
        Vector3 E = Vector3.Lerp(B, C, p_value);

        Vector3 F = Vector3.Lerp(D, E, p_value);

        return F;
    }
}
