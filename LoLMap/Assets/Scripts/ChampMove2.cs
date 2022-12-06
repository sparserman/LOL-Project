using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove2 : MonoBehaviour
{
    GameManager gm;

    public Camera cam;              // 메인 카메라
    private Animator ani;
    private ChamState state;        // 챔피언 스탯
    private NavMeshAgent agent;

    private Vector3 destination;    // 목적지 좌표
    private Transform deInfo;         // 목적지 정보

    private bool clickCheck = true;        // 꾹 누를때 천천히 반복하기 위함
    private float clickTime = 0;            // 클릭 딜레이

    public GameObject attackTarget;    // 공격목표

    private GameObject lastAttackTarget;    // 가장 최근 공격목표

    
    public GameObject QSkillObj;        // Q 스킬 이펙트
    public GameObject WSkillObj;        // W 스킬 이펙트
    public GameObject ESkillObj;        // E 스킬 이펙트

    private int attackCount = 0;     // 공격 모션 번호
    [SerializeField]
    private bool attackCheck = true;     // 공격 장전 상태
    private bool hitCheck = false;      // 공격 되었는지 체크

    private bool RSkillCheck = false;       // R 스킬을 사용중인지 체크

    [SerializeField]
    private bool PSkillCool = true;           // 패시브 사용 가능 상태 (쿨타임)
    private bool QSkillOn = false;           // Q 스킬 사용 가능 상태

    private bool QSkillCool = true;          // Q 스킬 사용 가능 상태 (쿨타임)
    private bool WSkillCool = true;          // W 스킬 사용 가능 상태 (쿨타임)
    private bool ESkillCool = true;          // E 스킬 사용 가능 상태 (쿨타임)
    private bool RSkillCool = true;          // R 스킬 사용 가능 상태 (쿨타임)

    [SerializeField]
    private LayerMask layerMask;        // 벽 전용
    [SerializeField]
    private LayerMask enemyMask;        // 적 전용
    [SerializeField]
    private LayerMask MapMask;        // 맵 전용


    public float deltaRotation;     // 회전 속도

    [SerializeField]
    private bool moveStop = false;      // 이동 중지

    private bool hide = false;      // 은신 상태

    // 쿨타임 시간
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
        // 공속 증감
        if (Input.GetKeyDown(KeyCode.T))
        {
            state.AttackSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            state.AttackSpeed -= 0.1f;
        }

        // 마우스 업
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
            // 공격 중지
            ani.SetBool("isAttack", false);
            attackTarget = null;

            // 이동 중지
            ani.SetInteger("isMove", 0);
            agent.speed = 0;
        }

        if (clickTime >= 0.2f)
        {
            // 클릭 가능 상태로 바꾸기
            clickCheck = true;
            clickTime = 0;
        }
        
        if(!clickCheck)
        {
            clickTime += Time.deltaTime;
        }
    }

    // 이동
    private void Move()
    {
        if (clickCheck)
        {
            ani.SetFloat("AttackSpeed", 1.5f / state.AttackSpeed);
            // 마우스 우클릭 시
            if (Input.GetMouseButton(1) && !moveStop)
            {
                // 클릭 확인
                clickCheck = false;
                // 마우스 이동용 위치 찾기
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
                {
                    // 목표지 설정
                    ChampSetDestination(hit.point);
                    deInfo = hit.transform;
                    
                }

                // 공격용 위치 찾기
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
                {
                    // 목표 설정
                    attackTarget = hit.collider.gameObject;
                }
                else
                {
                    // 이동 시 목표 제거
                    attackTarget = null;
                    ani.SetBool("isAttack", false);  // 이동 시 공격 모션 끄기
                    ani.SetInteger("AttackMotion", 0);  // 이동 시 공격 모션 끄기

                    // 공격 캔슬 시
                    if (!hitCheck)
                    {
                        attackCheck = true;
                        if (m_AttackDelayCoroutine != null)
                        {
                            Debug.Log("스탑3");
                            StopCoroutine(m_AttackDelayCoroutine);
                            m_AttackDelayCoroutine = null;
                        }
                    }
                }
            }
        }

        if (!ani.GetBool("isQSkill"))
        {
            // 방향전환과 이동
            if (ani.GetInteger("isMove") == 2)
            {
                // 이동
                moveStop = false;
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

                // 방향전환
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), deltaRotation);

                // 목적지 도착 시
                if (Vector3.Distance(transform.position, destination) <= 0.5f)
                {
                    ani.SetInteger("isMove", 0);
                    agent.speed = 0;
                }
            }
        }

        // 마우스를 떼면 다시 클릭가능상태로 바꾸기
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

    // 공격
    private void Attack()
    {
        if (attackTarget != null && !moveStop)
        {
            lastAttackTarget = attackTarget;

            // 패시브 공격 목표에 도착 시
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 8f && PSkillCool)
            {
                PassiveAttack();
            }

            // 기본 공격 목표에 도착 시
            else if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 2.5f && !PSkillCool)
            {
                // 이동 중지
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // 적 방향 쳐다보기
                transform.forward = attackTarget.transform.position - transform.position;

                // 공격 가능 상태 일때
                if (attackCheck)
                {
                    // Q 스킬이 적용된 상태라면
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

    // 공격 성공
    public void AttackEvent()
    {
        Debug.Log("히트!");
        Aggro();    // 어그로
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
        // 어그로 끌기
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

    // 공격 딜레이 (공격속도)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(state.AttackSpeed);
        Debug.Log("딜레이");
        if (!attackCheck)
        {
            attackCheck = true;
            hitCheck = false;
        }

        m_AttackDelayCoroutine = null;
    }

    // Q 스킬
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

    // W 스킬
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

    // 모션 끝
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


    // E 스킬
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

    // R 스킬
    private void RSkill()
    {
        if(Input.GetKeyDown(KeyCode.R) && !moveStop && RSkillCool)
        {
            Debug.Log("궁 사용");
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
        // 이동 중지
        ani.SetInteger("isMove", 0);
        moveStop = true;
        // 적 방향 쳐다보기
        transform.forward = attackTarget.transform.position - transform.position;

        // 어택 모션 끄기
        ani.SetInteger("AttackMotion", 0);

        // 공격 가능 상태 일때
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

    // 베지어곡선
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

    // 이동 목적지 설정
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        ani.SetInteger("isMove", 2);
    }

    // 목적지만 설정 (방향만 바꾸기 위함)
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
