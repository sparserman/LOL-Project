using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject ch;                    // 캐릭터 오브젝트
    private Vector3 ClickPos = Vector3.zero; // 클릭 좌표
    private bool _isClick = true;            // 클릭중 체크 

    bool _fixed = true;                      // 공 따라다니기

    bool SkillR_range = false;              
    public GameObject[] enermy = null;

    //오리아나 구체 
    [SerializeField]
    public GameObject obj;                      // 오리아나 구체 
    public float objSpeed = 5f;                 // 구체 속도
    private Vector3 objPos = Vector3.zero;      // 구체 좌표
    private Vector3 objDir = Vector3.zero;      // 구체 방향
    Coroutine m_MoveCortouine = null;           // 구체 코르틴

    [SerializeField]
    private LayerMask wall, enemy, ground;  // layerMask

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


 

    void Update()
    {
        Qskill();
        Wskill();
        Eskill();
        Rskill();
        fixedball();
    }

    IEnumerator UpdateTargetMove(Vector3 p_targetpos)
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, p_targetpos, Time.deltaTime * objSpeed);
            yield return null;


            if ((transform.position - p_targetpos).magnitude <= 0.1f)
            {
                transform.position = p_targetpos;
                _isClick = false;
                break;
            } // end if
        }// end while

        m_MoveCortouine = null;
    } // end UpdateTargetMove()

    //구체 이동
    void ObjMove()
    {
        if (m_MoveCortouine != null)
        {
            StopCoroutine(m_MoveCortouine);
            m_MoveCortouine = null;
        }

        StartCoroutine(UpdateTargetMove(ClickPos));
    } // end ObjMove()

    // 공이 따라다닐 때 좌표
    void fixedball()
    {
        if(_fixed == false)
        {
            return;
        }
        transform.position = ch.transform.position + new Vector3(0.5f, 0.5f, 0.5f);
    }// end fixedball

    // 패시브
    private void Passive()
    {

    }// end Passive()      

    // 스킬 Q
    private void Qskill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            objPos = Input.mousePosition;
            ObjMove();
            _fixed = false;
        }
    }// end Qskill()

    // 스킬 W
    private void Wskill()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _effctW.transform.position = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            _effctW.SetActive(true);
            StartCoroutine(isW());
        }
    }// end Wskill()

    IEnumerator isW()
    {
        yield return new WaitForSeconds(2f);
        _effctW.SetActive(false);
    }// end isW()

    // 스킬 E
    private void Eskill()
    {
        // 마우스 좌표 아군에게 실드
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(this.gameObject.layer == LayerMask.NameToLayer("Friendly"))
            { 

            }
        }
        // 자기 자신에게 실드
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            fixedball();
        }
    }// end Eskill()

    // 스킬 R
    private void Rskill()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SkillR_range == true) // layermask "Enermy"만 공격대상
            { 
                for (int i = 0; i < 2; i++)
                {   // 적 좌표 구체 쪽으로 당기기
                    enermy[i].transform.position = Vector3.MoveTowards(enermy[i].transform.position, objDir, Time.deltaTime * objSpeed);    
                }
            }
        }
    }// end Rskill()

    // 구체 범위 내 적 확인
    private void OnCollisionStay(Collision collision)
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Enermy"))
        {
            SkillR_range = true; //범위내 적이 있음
        }
    }



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
}
