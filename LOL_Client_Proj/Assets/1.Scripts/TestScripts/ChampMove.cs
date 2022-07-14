using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove : BaseActor
{
    public Camera cam;              // 메인 카메라
    private Animator ani;
    private ChamState state;        // 챔피언 스탯
    private NavMeshAgent agent;

    private Vector3 destination;    // 목적지 좌표

    private bool clickCheck = true;        // 꾹 누를때 천천히 반복하기 위함
    private float clickTime = 0;            // 클릭 딜레이

    public GameObject attackTarget;    // 공격목표

    public PoppyPassiveSkill PassiveSkillObj;        // Passive 스킬 이펙트
    public GameObject Shield;   // 방패

    public GameObject QSkillObj;        // Q 스킬 이펙트
    public GameObject WSkillObj;        // W 스킬 이펙트
    public GameObject RSkillObj;        // R 스킬 범위 이펙트
    //public Shot RSkillScript;              // R 스킬 차지 스크립트
    public GameObject RSkillEffectObj;  // R 스킬 회전 이펙트

    private int attackCount = 0;     // 공격 모션 번호
    private bool attackCheck = true;     // 공격 장전 상태
    private bool hitCheck = false;      // 공격 되었는지 체크

    [SerializeField]
    private GameObject eSkillTarget;    // E 스킬 목표
    private bool ESkillCheck = false;       // E 스킬을 사용중인지 체크


    private bool RSkillCheck = false;       // R 스킬을 사용중인지 체크
    private float RSkillTime = 0;           // R 스킬 차지 시간

    [SerializeField]
    private bool PassiveOn = true;           // 패시브 사용 가능 상태

    [SerializeField]
    private LayerMask layerMask;        // 벽 전용
    [SerializeField]
    private LayerMask enemyMask;        // 적 전용
    [SerializeField]
    private LayerMask MapMask;        // 맵 전용


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

            // E 스킬 중지
            if(!ani.GetBool("isESkill"))
            {
                eSkillTarget = null;
                ESkillCheck = false;
            }
        }

        if (clickTime >= 0.3f)
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
            if (Input.GetMouseButton(1) && !ani.GetBool("isQSkill") & !ani.GetBool("isESkill")
                && !ani.GetBool("isPassive") && !RSkillCheck)
            {
                // 클릭 확인
                clickCheck = false;
                // 마우스 이동용 위치 찾기
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, MapMask))
                {
                    // 목표지 설정
                    ChampSetDestination(hit.point);
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
                    eSkillTarget = null;
                    ani.SetBool("isAttack", false);  // 이동 시 공격 모션 끄기
                    ani.SetInteger("AttackMotion", 0);  // 이동 시 공격 모션 끄기

                    // 공격 캔슬 시
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
            // 방향전환과 이동
            if (ani.GetInteger("isMove") == 2)
            {
                // 이동
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;

                // 방향전환
                transform.forward = dir;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Wall")
        {
            ani.SetInteger("isMove", 0);
            agent.speed = 0;
        }
    }

    // 공격
    private void Attack()
    {
        if (attackTarget != null && !ani.GetBool("isCharge") && !ani.GetBool("isRSkill") && !RSkillCheck)
        {
            // 패시브 공격 목표에 도착 시
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 5f && PassiveOn)
            {
                // 이동 중지
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // 적 방향 쳐다보기
                transform.forward = attackTarget.transform.position - transform.position;

                // 공격 가능 상태 일때
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

            // 기본 공격 목표에 도착 시
            else if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 2.5f && !PassiveOn)
            {
                // 이동 중지
                ani.SetInteger("isMove", 0);
                agent.speed = 0;
                // 적 방향 쳐다보기
                transform.forward = attackTarget.transform.position - transform.position;

                // 공격 가능 상태 일때
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

    // 공격 히트
    public void AttackEvent()
    {
        Debug.Log("히트!");
        hitCheck = true;
    }

    // 공격 딜레이 (공격속도)
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

    // 패시브
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
                // 방패 제거
                Shield.SetActive(false);
                break;
            case 1:
                PassiveOn = false;
                ani.SetBool("isPassive", false);
                StartCoroutine(PassiveTime());
                break;
                
        }
    }

    // 패시브 쿨타임
    IEnumerator PassiveTime()
    {
        yield return new WaitForSeconds(3f);
        PassiveOn = true;
        Shield.SetActive(true);
    }

    // Q 스킬
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

            // 플레이어 방향
            transform.forward = destination;
        }
    }

    // Q 스킬 이펙트 생성
    public void QSkillEvent(int num)
    {
        if (num == 0)
        {
            GameObject cpyObj;
            cpyObj = Instantiate(QSkillObj, transform.position, Quaternion.identity);
            cpyObj.transform.forward = new Vector3(destination.x, -180, destination.z);
            cpyObj.SetActive(true);
            // 1초뒤 삭제
            Destroy(cpyObj, 1.5f);
        }
        if (num == 1)
        {
            ani.SetBool("isQSkill", false);
        }
    }

    // W 스킬
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

    // W 스킬 지속
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

    // E 스킬
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
            // 사거리 안에 없을 때 이동
            if (Vector3.Distance(transform.position, eSkillTarget.transform.position) > 5f)
            {
                ani.SetInteger("isMove", 2);
                ani.SetBool("isAttack", false);
                agent.speed = state.Speed * 0.01f;
                var dir = new Vector3(eSkillTarget.transform.position.x, transform.position.y, eSkillTarget.transform.position.z) - transform.position;
                transform.forward = dir;

                ChampSetDestination(eSkillTarget.gameObject.transform.position);
            }

            // 사거리 안에 들어왔을 때
            else
            {
                Debug.Log("사거리 안으로 들어옴");
                ESkillCheck = false;

                ani.SetBool("isESkill", true);
                ani.SetInteger("isMove", 0);
                ani.SetInteger("AttackMotion", 0);
                ani.SetBool("isAttack", false);
                attackTarget = null;
                agent.speed = 0;

                // 플레이어 방향
                var dir = new Vector3(eSkillTarget.transform.position.x, transform.position.y, eSkillTarget.transform.position.z) - transform.position;
                transform.forward = dir;
                


                if (m_MoveCoroutine != null)
                {
                    StopCoroutine(m_MoveCoroutine);
                    m_MoveCoroutine = null;
                }

                // 뒤에 벽 확인
                RaycastHit MapHit;
                if (Physics.Raycast(eSkillTarget.transform.position, transform.forward, out MapHit, 5f, layerMask))
                {
                    // 벽으로 이동
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(MapHit.point,
                        Vector3.Distance(transform.position, MapHit.transform.position) * 0.03f));
                }
                else
                {
                    // 빈곳으로 이동
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(eSkillTarget.transform.position + (transform.forward * 4),
                        Vector3.Distance(transform.position, eSkillTarget.transform.position + (transform.forward * 4)) * 0.04f));
                }
            }
        }
    }

    // 목표까지 정해진 시간에 맞게 이동
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
            // 충돌 체크
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                if(hit.collider.name != "Poppy")
                {
                    flag = true;
                }
                
            }

            // 충돌했다면
            if (flag)
            {
                eSkillTarget.transform.position = temppos;
            }
            if( (targetpos - temppos).sqrMagnitude <= 0.5f )
            {
                break;
            }
        }

        // 추후처리부분
        transform.position = targetpos - transform.forward * 2;
        eSkillTarget.transform.position = temppos;

        eSkillTarget = null;

        ani.SetBool("isESkill", false);
    }


    // R 스킬
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

    // R 스킬
    Coroutine m_RSkillCoroutine = null;
    IEnumerator ChargeCheck()
    {
        // 회전 이펙트 생성
        GameObject cpyObj;
        Vector3 temppos;
        temppos = transform.position;
        temppos.y = 3;
        cpyObj = Instantiate(RSkillEffectObj, temppos, Quaternion.identity);
        cpyObj.SetActive(true);

        // 정리
        ani.SetBool("RSkillCancel", false);
        ani.SetBool("isRSkill", true);
        ani.SetInteger("AttackMotion", 0);
        ani.SetBool("isAttack", false);
        attackTarget = null;

        float t = 0;
        while (true)
        {
            yield return null;

            // 회전이펙트 따라 다니기
            temppos = transform.position;
            temppos.y = 3;
            cpyObj.transform.position = temppos;

            t += Time.deltaTime;
            // 궁 누르고 있는지 체크
            if (!Input.GetKey(KeyCode.R))
            {
                // 궁을 차지했다면
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

    // R 스킬
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

        // 플레이어 방향
        transform.forward = new Vector3(destination.x, -90, destination.z);

        
    }

    public void RSkillEvent(int num)
    {
        switch(num)
        {
            case 0: // 차지안한 R 스킬
                Debug.Log("R");
                GameObject cpyObj;
                cpyObj = Instantiate(RSkillObj, transform.position, Quaternion.identity);
                cpyObj.transform.forward = new Vector3(destination.x, 0, destination.z);
                cpyObj.SetActive(true);
                // 삭제
                Destroy(cpyObj, 0.1f);
                break;
            case 1: // R 스킬 끝난 후 이동 가능 상태로 바꾸기
                RSkillCheck = false;
                break;
            case 2: // 차지한 R 스킬
                Debug.Log("Charge R");
                //Shot cpyObj2;
                //cpyObj2 = Instantiate<Shot>(RSkillScript, transform.position, Quaternion.identity);
                //cpyObj2.transform.forward = new Vector3(destination.x, 0, destination.z);

                // R 스킬 최대 범위
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
