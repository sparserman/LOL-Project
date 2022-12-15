using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum CC
{
    RESTRICTION = 0, STUN, SLOW, STOP
}
// STOP은 뽀삐 벽

public class ChampController : MonoBehaviour
{
    GameManager gm;

    public ChampNum champNum;       // 챔피언 번호

    public Team team;

    public Animator ani;
    public ChamState state;        // 챔피언 스탯
    public NavMeshAgent agent;

    public Vector3 destination;    // 목적지 좌표

    private bool clickCheck = true;        // 꾹 누를때 천천히 반복하기 위함
    private float clickTime = 0;            // 클릭 딜레이

    public GameObject attackTarget;    // 공격목표

    private GameObject lastAttackTarget;    // 가장 최근 공격목표

    private int attackCount = 0;     // 공격 모션 번호
    public bool attackCheck = true;     // 공격 장전 상태
    private bool hitCheck = false;      // 공격 되었는지 체크

    [SerializeField]
    private GameObject eSkillTarget;    // E 스킬 목표

    private bool RSkillCheck = false;       // R 스킬을 사용중인지 체크
    private float RSkillTime = 0;           // R 스킬 차지 시간

    [SerializeField]
    public bool PSkillCool = true;           // 패시브 사용 가능 상태

    public bool QSkillCool = true;          // Q 스킬 사용 가능 상태 (쿨타임)
    public bool WSkillCool = true;          // W 스킬 사용 가능 상태 (쿨타임)
    public bool ESkillCool = true;          // E 스킬 사용 가능 상태 (쿨타임)
    public bool RSkillCool = true;          // R 스킬 사용 가능 상태 (쿨타임)

    public GameObject shield;

    [SerializeField]
    private LayerMask wallMask;        // 벽 전용
    [SerializeField]
    private LayerMask enemyMask;        // 적 전용
    [SerializeField]
    private LayerMask mapMask;        // 맵 전용


    public float deltaRotation;     // 회전 속도

    public bool moveStop = false;      // 이동 중지

    public bool hide = false;      // 은신 상태

    // 쿨타임 시간
    public float PCoolTime;
    public float QCoolTime;
    public float WCoolTime;
    public float ECoolTime;
    public float RCoolTime;

    // 조작 가능 여부
    public bool inOperation = false;

    // 체력 바
    public GameObject hpBar;

    public bool die = false;        // 죽었는 지 체크
    public List<CC> cc;        // CC기 걸렸는 지 체크
    public float shieldHP = 100;        // 보호막 크기

    // 챔피언 스킬
    private Poppy poppy;

    public float dieTime = 5f;      // 부활 시간

    void Start()
    {
        gm = GameManager.GetInstance;

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

            // 공속
            ani.SetFloat("AttackSpeed", 1.5f / state.AttackSpeed);
        }
    }

    private void OnMouseEnter()
    {
        if (!inOperation)
        {
            ChampController champ = null;
            for (int i = 0; i < gm.playerList.Count; i++)
            {
                if (gm.playerList[i].GetComponent<ChampController>().inOperation)
                {
                    champ = gm.playerList[i].GetComponent<ChampController>();
                    break;
                }
            }
            // 적이면 빨간색 outline 쉐이더 키기
            if (champ.team != team)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
            // 팀이면 하늘색 outline 쉐이더 키기
            else
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void OnMouseExit()
    {
        if (!inOperation)
        {
            ChampController champ = null;
            for (int i = 0; i < gm.playerList.Count; i++)
            {
                if (gm.playerList[i].GetComponent<ChampController>().inOperation)
                {
                    champ = gm.playerList[i].GetComponent<ChampController>();
                    break;
                }
            }
            // 타겟이 자신이 아니라면
            if (champ.attackTarget != gameObject)
            {
                Debug.Log("같지않음");
                // 적이면 빨간색 outline 쉐이더 끄기
                if (champ.team != team)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
                // 팀이면 하늘색 outline 쉐이더 끄기
                else
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    private void InitSetting()
    {
        state.MaxHP = 600;
        state.HP = 600;
        state.Speed = 340;
        state.AttackSpeed = 1.5f;
        state.Attack = 70;
        state.AttackRange = 2.5f;

        // HPBar 생성 및 부여
        hpBar = Instantiate(Resources.Load("Prefabs/" + "HPBar") as GameObject);
        hpBar.transform.parent = GameObject.Find("GUI").transform;
        hpBar.transform.GetChild(0).GetComponent<HPBarBack>().champ = this;

        if (champNum == ChampNum.POPPY)
        {
            poppy = gameObject.AddComponent<Poppy>();
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
        if (Input.GetMouseButtonUp(1))
        {
            clickCheck = true;
        }
    }

    // HP 지속적으로 변경
    private void HpControl()
    {
        // 체력 변경
        if((state.HP + shieldHP) / state.MaxHP > 1)
        {
            hpBar.GetComponent<Scrollbar>().size = (state.HP + (state.MaxHP - (state.HP + shieldHP))) / state.MaxHP;
            Debug.Log(state.MaxHP - (state.HP + shieldHP));
        }
        else
        {
            hpBar.GetComponent<Scrollbar>().size = state.HP / state.MaxHP;
        }

        if(shieldHP > 0)
        {
            hpBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else
        {
            if(inOperation)
            {
                hpBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.red;
            }
            else
            {
                hpBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
            }
        }
    }

    private void Stop()
    {
        if (Input.GetKeyDown(KeyCode.S) || die)
        {
            // 공격 중지
            ani.SetBool("isAttack", false);
            attackTarget = null;

            // 이동 중지
            ani.SetInteger("isMove", 0);
            agent.speed = 0;

            // E 스킬 중지
            if (!ani.GetBool("isESkill"))
            {
                eSkillTarget = null;
            }
        }

        if (clickTime >= 0.1f)
        {
            // 클릭 가능 상태로 바꾸기
            clickCheck = true;
            clickTime = 0;
        }

        if (!clickCheck)
        {
            clickTime += Time.deltaTime;
        }
    }

    // 클릭 위치 또는 대상 전송
    private void Click()
    {
        // 패킷 보낼 곳
        if (clickCheck)
        {

            // 마우스 우클릭 시
            if (Input.GetMouseButton(1) && !moveStop)
            {
                // 어택 타겟의 outline을 끄기
                if (attackTarget != null)
                {
                    attackTarget.transform.GetChild(0).gameObject.SetActive(false);
                    attackTarget.transform.GetChild(1).gameObject.SetActive(false);
                }

                // 클릭 확인
                clickCheck = false;

                // 공격 할 적 찾기
                FindAttackTarget();

                FindMovePos();
            }
            if (Input.GetMouseButton(1) && moveStop)
            {
                lastAttackTarget = null;
                ani.SetInteger("isMove", 2);
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
                {
                    agent.SetDestination(hit.point);
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

    private void FindAttackTarget()
    {
        RaycastHit hit;

        // 공격용 위치 찾기
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
        {
            // 팀은 제외
            // 챔피언
            if (hit.transform.gameObject.layer.Equals(9))
            {
                if (hit.transform.gameObject.GetComponent<ChampController>().team == team)
                {
                    attackTarget = null;
                    return;
                }
            }
            // 미니언
            else if (hit.transform.gameObject.layer.Equals(10))
            {
                if (hit.transform.gameObject.GetComponent<Minion>().team == team)
                {
                    attackTarget = null;
                    return;
                }
            }

            // 리스트에서 적 찾기
            int num = -1;
            for (int i = 0; i < gm.allList.Count; i++)
            {
                if (gm.allList[i] == hit.collider.gameObject)
                {
                    num = i;
                }
            }

            // 목표를 패킷으로 숫자로 치환해서 보내기
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
                Sever.Instance.AttackPack(protocol, num);

                // num 보내기
            }
            else
            {
                SetAttackTarget(num);
            }

        }
        else
        {
            attackTarget = null;
        }
    }

    private void FindMovePos()
    {
        RaycastHit hit;

        // 이동용 위치 찾기
        if (attackTarget == null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
        {
            if (gm.serverConnected)
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


                // 패킷으로 위치 전송 hit.point
            }
            else
            {
                SetMovePos(hit.point);
            }
        }
    }

    // 클릭 위치로 이동
    private void Move()
    {
        if (!moveStop)
        {
            // 방향전환과 이동
            if (ani.GetInteger("isMove") == 2)
            {
                // 이동
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
    }

    // 이동 위치 및 공격 대상 설정
    public void SetMovePos(Vector3 p_pos)
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
                Debug.Log("스탑3");
                StopCoroutine(m_AttackDelayCoroutine);
                m_AttackDelayCoroutine = null;
            }
        }

        // 목표지 설정
        ChampSetDestination(p_pos);
    }

    public void SetAttackTarget(int p_listNum)
    {
        // 목표 설정
        if (!RSkillCheck)
        {
            attackTarget = gm.allList[p_listNum].gameObject;
            // 적이니까 빨간색 outline 쉐이더 키기
            attackTarget.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private bool attackEnd = false;
    // 공격
    private void Attack()
    {
        if (attackTarget != null && !moveStop)
        {
            lastAttackTarget = attackTarget;

            // 챔프가 죽었는 지 체크
            if (attackTarget.layer.Equals(9))
            {
                if (attackTarget.GetComponent<ChampController>().die)
                {
                    // 타겟 제거
                    attackTarget = null;
                    attackEnd = true;
                    return;
                }
            }
            // 미니언이 죽었는 지 체크
            else if (attackTarget.layer.Equals(10))
            {
                if (attackTarget.GetComponent<Minion>().die)
                {
                    // 타겟 제거
                    attackTarget = null;
                    attackEnd = true;
                    return;
                }
            }

            // 패시브 공격
            ChampPassive();

            // 기본 공격 목표에 도착 시
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= state.AttackRange && !PSkillCool)
            {
                // 사거리 증가
                state.AttackRange = 3.5f;
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

                }
            }
            else if (!ani.GetBool("isPassive"))
            {
                state.AttackRange = 2.5f;
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
        else
        {
            if(attackEnd && attackCheck)
            {
                attackEnd = false;
                ani.SetBool("isAttack", false);
            }
        }
    }

    // 패시브 스킬
    public void ChampPassive()
    {
        switch(champNum)
        {
            case ChampNum.POPPY:
                poppy.PassiveAttack();
                break;
            case ChampNum.RENGAR:
                // 패시브 공격 목표에 도착 시
                if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 8f && PSkillCool)
                {
                    PassiveAttack();
                }
                break;
        }
    }

    // 공격 성공
    public void AttackEvent()
    {
        if (attackTarget != null)
        {
            Aggro();    // 어그로
            hitCheck = true;
            if (attackTarget.layer.Equals(9))
            {
                attackTarget.GetComponent<ChampController>().Damaged(state.Attack);
                minionAggro();
            }
            else if (attackTarget.layer.Equals(10))
            {
                attackTarget.GetComponent<Minion>().Damaged(state.Attack);
            }
        }
    }

    // 상대 미니언의 어그로를 전부 때린 상대로
    public void minionAggro()
    {
        for (int i = 0; i < gm.allList.Count; i++)
        {
            if (Vector3.Distance(gm.allList[i].transform.position, transform.position) <= 5f)
            {
                if (gm.allList[i].transform.name != transform.name)
                {
                    if (gm.allList[i].layer.Equals(10))
                    {
                        if (team != gm.allList[i].GetComponent<Minion>().team)
                        {
                            if (!gm.allList[i].GetComponent<Minion>().aggro)
                            {
                                // 타겟을 자신으로 변경
                                gm.allList[i].GetComponent<Minion>().attackTarget = gameObject;
                                gm.allList[i].GetComponent<Minion>().aggro = true;

                                // 미니언 스크립트에서 코루틴 돌리기
                                gm.allList[i].GetComponent<Minion>().StartCoroutine(AggroRelease(gm.allList[i].GetComponent<Minion>()));
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator AggroRelease(Minion p_obj)
    {
        yield return new WaitForSeconds(2f);

        p_obj.aggro = false;
    }


    public void Damaged(float p_damage)
    {
        // 보호막이 있으면
        if(shieldHP > 0)
        {
            // 근데 보호막이 데미지보다 적다면
            if(shieldHP < p_damage)
            {
                shieldHP = 0;
                state.HP -= p_damage - shieldHP;
            }
            // 보호막이 더 크면
            else
            {
                shieldHP -= p_damage;
            }
        }
        // 보호막이 없으면
        else
        {
            state.HP -= p_damage;
        }

        if (state.HP <= 0 && !die)
        {
            // 사망
            die = true;
            ani.Play("Die");
            hpBar.SetActive(false);
            // outline 끄기
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            // 정지
            Stop();
            GetComponent<Collider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            StartCoroutine(ResurrectionCorouine());
        }
    }

    IEnumerator ResurrectionCorouine()
    {
        yield return new WaitForSeconds(dieTime);
        Resurrection();
    }

    // 부활
    public void Resurrection()
    {
        // 위치도 넣어야함

        // 스탯 초기화
        state.HP = state.MaxHP;
        state.MP = state.MaxMP;

        // 정보 수정
        die = false;
        GetComponent<Collider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        hpBar.SetActive(true);

        ani.Play("Idle");
    }

    public void Aggro()
    {
        // 어그로 끌기
        if (lastAttackTarget != null)
        {
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
    }

    // 공격 딜레이 (공격속도)
    public Coroutine m_AttackDelayCoroutine = null;
    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(state.AttackSpeed);
        if (!attackCheck)
        {
            attackCheck = true;
            hitCheck = false;
        }

        m_AttackDelayCoroutine = null;
    }

    // 패시브
    public void PassiveEvent(int num)
    {
        switch(champNum)
        {
            case ChampNum.POPPY:
                poppy.PassiveEffect(num);
                break;
            case ChampNum.RENGAR:

                break;
        }
    }

    // Q 스킬
    private void QSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !moveStop && QSkillCool)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, mapMask))
            {
                if(gm.serverConnected)
                {
                    // 방향 구한거 값 보내기
                }
                else
                {
                    QSkillPlay(hit.point);
                }
            }
        }
    }

    // Q 스킬 실행하기
    public void QSkillPlay(Vector3 p_pos)
    {
        switch(champNum)
        {
            case (int)ChampNum.POPPY:
                poppy.QSkill(p_pos);
                break;
        }
    }

    // Q 스킬 이펙트 생성
    public void QSkillEvent(int num)
    {
        switch (champNum)
        {
            case ChampNum.POPPY:
                poppy.QSkillEffect(num);
                break;
            case ChampNum.RENGAR:

                break;
        }
    }

    // W 스킬
    private void WSkill()
    {
        if (Input.GetKeyDown(KeyCode.W) && !moveStop && WSkillCool)
        {
            WSkillPlay();
        }
    }

    private void WSkillPlay()
    {
        switch (champNum)
        {
            case ChampNum.POPPY:
                poppy.WSkill();
                break;
            case ChampNum.RENGAR:

                break;
        }
    }

    // E 스킬
    private void ESkill()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && !moveStop && ESkillCool)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, enemyMask))
            {
                // 팀은 제외
                // 챔피언
                if (hit.transform.gameObject.layer.Equals(9))
                {
                    if (hit.transform.gameObject.GetComponent<ChampController>().team == team)
                    {
                        return;
                    }
                }
                // 미니언
                else if (hit.transform.gameObject.layer.Equals(10))
                {
                    if (hit.transform.gameObject.GetComponent<Minion>().team == team)
                    {
                        return;
                    }
                }

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

                moveStop = true;
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
                if (Physics.Raycast(eSkillTarget.transform.position, transform.forward, out MapHit, 5f, wallMask))
                {
                    // 벽으로 이동
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(MapHit.point,
                        Vector3.Distance(transform.position, MapHit.transform.position) * 0.03f));

                    Debug.Log("뒤에 벽 있음");
                }
                else
                {
                    // 빈곳으로 이동
                    m_MoveCoroutine = StartCoroutine(SetDirectionMove(eSkillTarget.transform.position + (transform.forward * 4),
                        Vector3.Distance(transform.position, eSkillTarget.transform.position + (transform.forward * 4)) * 0.04f));

                    Debug.Log("뒤에 벽 없음");
                }
            }
        }
    }

    // 목표까지 정해진 시간에 맞게 이동
    Coroutine m_MoveCoroutine = null;
    IEnumerator SetDirectionMove(Vector3 p_targetwpos, float p_sec)
    {
        Debug.Log("코루틴 시작");
        Vector3 Currpos = transform.position;
        Vector3 targetpos = p_targetwpos;
        float t = 0;
        float divval = 1 / p_sec;
        Vector3 temppos;

        bool flag = false;

        // 쿨타임
        ESkillCool = false;
        moveStop = true;

        while (true)
        {
            yield return null;
            Debug.Log("코루틴 재생중");
            t += divval * Time.deltaTime;
            temppos = Vector3.Lerp(Currpos, targetpos, t);
            transform.position = temppos - transform.forward;

            RaycastHit hit;
            // 충돌 체크
            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.1f))
            {
                if (!hit.collider.CompareTag("Blue"))
                {
                    flag = true;
                }

            }

            // 충돌했다면
            if (flag)
            {
                eSkillTarget.transform.position = temppos;
            }
            if ((targetpos - temppos).sqrMagnitude <= 0.5f)
            {
                break;
            }
        }

        // 추후처리부분
        transform.position = targetpos - transform.forward * 2;
        eSkillTarget.transform.position = temppos;

        moveStop = false;
        eSkillTarget = null;

        ani.SetBool("isESkill", false);

        StartCoroutine(SkillCooltime(3));
    }


    // R 스킬
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

    // R 스킬
    Coroutine m_RSkillCoroutine = null;
    IEnumerator ChargeCheck()
    {
        // 회전 이펙트 생성
        GameObject cpyObj;
        Vector3 temppos;
        temppos = transform.position;
        temppos.y = 3;
        cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyRChargeEffect") as GameObject
            , temppos, Quaternion.identity);
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
            temppos.y = transform.position.y + 3;
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

    // R 스킬
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

        // 플레이어 방향
        transform.forward = new Vector3(destination.x, -90, destination.z);


    }

    public void RSkillEvent(int num)
    {
        switch (num)
        {
            case 0: // 차지안한 R 스킬
                RSkillCool = false;
                Debug.Log("R");
                GameObject cpyObj;
                cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyREffectPos") as GameObject
                    , transform.position, Quaternion.identity);
                cpyObj.transform.forward = new Vector3(destination.x, 0, destination.z);
                cpyObj.SetActive(true);
                // 삭제
                Destroy(cpyObj, 0.1f);
                break;
            case 1: // R 스킬 끝난 후 이동 가능 상태로 바꾸기
                RSkillCheck = false;
                moveStop = false;
                StartCoroutine(SkillCooltime(4));
                break;
            case 2: // 차지한 R 스킬
                Debug.Log("Charge R");
                GameObject cpyObj2;
                cpyObj2 = Instantiate<GameObject>(Resources.Load("Prefabs/" + "PoppyREffectChargePos") as GameObject
                    , transform.position, Quaternion.identity);
                cpyObj2.transform.forward = new Vector3(destination.x, 0, destination.z);

                // R 스킬 최대 범위
                if (RSkillTime > 1.5f)
                {
                    RSkillTime = 1.5f;
                }

                cpyObj2.GetComponent<Shot>().Fire(RSkillTime);

                cpyObj2.gameObject.SetActive(true);
                RSkillTime = 0;
                break;
            case 3:
                Debug.Log("트루");
                moveStop = true;
                break;
        }

        ani.SetBool("isCharge", false);
        StartCoroutine(SkillCooltime(4));
    }


    // 이동 목적지 설정
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        ani.SetInteger("isMove", 2);
    }

    // 목적지만 설정 (방향만 바꾸기 위함)
    public void SetDir(Vector3 dir)
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
