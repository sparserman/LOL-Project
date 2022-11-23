using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveMent : MonoBehaviour
{
    ChamState Cstate;                        // 챔피언 정보
    ItemState Istate;                        // 아이템 정보

    public Camera _camera;                   // 카메라
    private Animator _animator;              // 애니메이션

    [SerializeField]
    private float _MoveSpeed = 2.0f;         // 이동속도
    private float _rotateSpeed = 10.0f;      // 회전속도
    private Vector3 _movePos = Vector3.zero; // 이동좌표
    private Vector3 _moveDir = Vector3.zero; // 회전좌표

    private bool _isClick = true;            // 클릭중 체크 
    private float _Clilckdelay = 0;          // 클릭 딜레이

    public GameObject _target;               // 공격가능 대상
    private bool _ismove = true;             // 이동 상태
    private bool hide = false;               // 은신 상태 - 부시
    private bool skillhit = false;           // 스킬 적중 확인 - 패시브용


    [SerializeField]
    private LayerMask wall, enemy ,ground;  // layerMask

    [SerializeField]
    protected Vector3 HitInfoVec = new Vector3();   // laycastHit Vector Pos

    // 스킬 이펙트 추가 
    public GameObject _effctQ;              // 스킬 Q 이펙트
    public GameObject _effctW;              // 스킬 W 이펙트
    public GameObject _effctE;              // 스킬 E 이펙트
    public GameObject _effctR;              // 스킬 R 이펙트

    // 스킬 쿨타임
    private float _PcoolTime;               // 패시브 스킬 쿨타임
    private float _QcoolTime;               // Q 스킬 쿨타임
    private float _WcoolTime;               // W 스킬 쿨타임
    private float _EcoolTime;               // E 스킬 쿨타임
    private float _RcoolTime;               // R 스킬 쿨타임

    private bool _checkP = false;            // 패시브 사용가능 체크
    private bool _checkQ = false;            // 스킬 Q 사용가능 체크
    private bool _checkW = false;            // 스킬 W 사용가능 체크
    private bool _checkE = false;            // 스킬 E 사용가능 체크
    private bool _checkR = false;            // 스킬 R 사용가능 체크


    void Start()
    {
        Cstate.Xp = 0;
        Cstate.LV = 1;
        _animator = GetComponent<Animator>();
    } // end Start()

    void Update()
    {
        Move();
        Stop();
        Attack();
        Qskill();
        Wskill();
        Eskill();
        Rskill();
	Passive();
        LvUp();
    } //end update()

    void Ezreal_NomalState()
    {
        Cstate.Attack = 60f +  ((Cstate.LV - 1) * 2.5f);          // 공격력  
        Cstate.DefPen = 0;                                        // 방어관통력

        Cstate.Mag = 0;                                           // 주문력
        Cstate.MagPen = 0;                                        // 마법 관통력

        Cstate.SkillAcc = 0;                                      // 스킬가속(쿨타임 감소)
        Cstate.Speed = 325f;                                      // 이동속도
        Cstate.AttackRange = 550f;                                // 사거리
        Cstate.AttackSpeed = 1f  + ((Cstate.LV - 1) * 0.025f);    // 공격속도 
        Cstate.Critical = 0;                                      // 치명타

        Cstate.Def = 24f + ((Cstate.LV - 1) * 4.7f);              // 방어력
        Cstate.MagDef = 30f + ((Cstate.LV - 1) * 1.3f);           // 마법저항력
        Cstate.HP = 600f + ((Cstate.LV - 1) * 102f);              // 체력
        Cstate.HPRegen = 4f + ((Cstate.LV - 1) * 0.65f);          // 체력재생
        Cstate.MP = 375f + ((Cstate.LV - 1) * 70f);               // 마나
        Cstate.MPRegen = 8.5f + ((Cstate.LV - 1) * 0.65f);        // 마나재생
        Cstate.Ten = 0;                                           // 강인함(저항력)

    }

    // 레벨업 필요 경험치
    void LvUp()
    {
        if (Cstate.Xp >= (280 + (Cstate.LV -1) * 110))  // 레벨업당 필요 경험치  ((lv -1) * 110) + 280  --/ 레벨 1 : 280
        {
            if (Cstate.LV == 18)                        // 최고레벨 18레벨 달성시 레벨업 종료
            {
                return;
            }
            Cstate.LV += 1;
            Cstate.Xp = 0;
            Ezreal_NomalState();
        }
    }// end LvUp()
    
    // 이동
    private void Move()
    {
        if(_isClick)
        {
            if(Input.GetMouseButton(1) && !_ismove) // 마우스 우클릭
            {
              
                _isClick = false;
                RaycastHit _hit;

                if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out _hit,100.0f, ground)) // 땅 클릭
                {
                    _movePos = _hit.point;
                    _movePos.y = 0;     // 게임 상 평지 -> y축이동 고정 
                    _moveDir = _movePos - transform.position;
                    Debug.Log("이동좌표 :" + _movePos);
                } // end raycast
            } // end 우클릭
        } // 클릭 체크

        // 회전하는 방향 구하기 
        Vector3 newDir = Vector3.RotateTowards(transform.forward, _moveDir, _rotateSpeed * Time.deltaTime, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position = Vector3.MoveTowards(transform.position, _movePos, _MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _movePos) < 0.1f)
        {
            _animator.SetBool("isMove", false);  //-------- "isMove" 수정 사용 --------------------------------------------------------//
        }


        // 마우스 우클릭 떼는 순간 클릭 가능하게 사용 -- 클릭중 이동 이유
        if (Input.GetMouseButtonUp(1))
        {
            _isClick = true;
            _Clilckdelay = 0;
        }
    } //end move()

    // 중지 단축키 - S
    private void Stop()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // 공격 중지
            _animator.SetBool("isAttack", false);
            _target = null;

            // 이동 중지
            _animator.SetBool("isMove", false);
            _MoveSpeed = 0;

            // + 필요시  스킬 중지 추가 
        }


        // 마우스 클릭 가능 상태 변환
        if(_Clilckdelay >= 0.2f)
        {
            _isClick = true;
            _Clilckdelay = 0;
        }

        if (!_isClick)
        {
            _Clilckdelay += Time.deltaTime;
        }
    } // end Stop()

    // 기본 공격
    private void Attack()
    {

    }// end Attack()


    // 패시브
    private void Passive()      
    {
        int i = 0;               // 패시브 중첩 횟수 확인

        if (skillhit == true)
        { 
            if( i > 5)          // 패시브 중첩 최대 5회
            {
                return;
            }
            Cstate.AttackSpeed *= 1.1f;     // 중첩당 공격속도 0.1 상승
            i++;
        }
    }// end Passive()      

    // 스킬 Q
    private void Qskill()
    {

    }// end Qskill()

    // 스킬 W
    private void Wskill()
    {

    }// end Wskill()

    // 스킬 E
    private void Eskill()
    {

    }// end Eskill()

    // 스킬 R
    private void Rskill()
    {

    }// end Rskill()



    //  스킬 쿨타임 관리
    IEnumerator SkillCooltime(int cooltime_)
    {
        switch (cooltime_)
        {
            case 0:
                yield return new WaitForSeconds(_PcoolTime);
                _checkP = true;
                break;
            case 1:
                yield return new WaitForSeconds(_QcoolTime);
                _checkQ = true;
                break;
            case 2:
                yield return new WaitForSeconds(_WcoolTime);
                _checkW = true;
                break;
            case 3:
                yield return new WaitForSeconds(_EcoolTime);
                _checkE = true;
                break;
            case 4:
                yield return new WaitForSeconds(_RcoolTime);
                _checkR = true;
                break;
        }


    }// end SkillCooltime


} // end class
