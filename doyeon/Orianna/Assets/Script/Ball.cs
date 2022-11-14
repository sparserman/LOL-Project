using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject ch;                    // ĳ���� ������Ʈ
    private Vector3 ClickPos = Vector3.zero; // Ŭ�� ��ǥ
    private bool _isClick = true;            // Ŭ���� üũ 

    bool _fixed = true;                      // �� ����ٴϱ�

    bool SkillR_range = false;              
    public GameObject[] enermy = null;

    //�����Ƴ� ��ü 
    [SerializeField]
    public GameObject obj;                      // �����Ƴ� ��ü 
    public float objSpeed = 5f;                 // ��ü �ӵ�
    private Vector3 objPos = Vector3.zero;      // ��ü ��ǥ
    private Vector3 objDir = Vector3.zero;      // ��ü ����
    Coroutine m_MoveCortouine = null;           // ��ü �ڸ�ƾ

    [SerializeField]
    private LayerMask wall, enemy, ground;  // layerMask

    [SerializeField]
    protected Vector3 HitInfoVec = new Vector3();   // laycastHit Vector Pos

    // ��ų ����Ʈ �߰� 
    public GameObject _effctQ;              // ��ų Q ����Ʈ
    public GameObject _effctW;              // ��ų W ����Ʈ
    public GameObject _effctE;              // ��ų E ����Ʈ
    public GameObject _effctR;              // ��ų R ����Ʈ

    // ��ų ��Ÿ��
    private float _PcoolTime;               // �нú� ��ų ��Ÿ��
    private float _QcoolTime;               // Q ��ų ��Ÿ��
    private float _WcoolTime;               // W ��ų ��Ÿ��
    private float _EcoolTime;               // E ��ų ��Ÿ��
    private float _RcoolTime;               // R ��ų ��Ÿ��

    private bool _checkP = false;            // �нú� ��밡�� üũ
    private bool _checkQ = false;            // ��ų Q ��밡�� üũ
    private bool _checkW = false;            // ��ų W ��밡�� üũ
    private bool _checkE = false;            // ��ų E ��밡�� üũ
    private bool _checkR = false;            // ��ų R ��밡�� üũ


 

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

    //��ü �̵�
    void ObjMove()
    {
        if (m_MoveCortouine != null)
        {
            StopCoroutine(m_MoveCortouine);
            m_MoveCortouine = null;
        }

        StartCoroutine(UpdateTargetMove(ClickPos));
    } // end ObjMove()

    // ���� ����ٴ� �� ��ǥ
    void fixedball()
    {
        if(_fixed == false)
        {
            return;
        }
        transform.position = ch.transform.position + new Vector3(0.5f, 0.5f, 0.5f);
    }// end fixedball

    // �нú�
    private void Passive()
    {

    }// end Passive()      

    // ��ų Q
    private void Qskill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            objPos = Input.mousePosition;
            ObjMove();
            _fixed = false;
        }
    }// end Qskill()

    // ��ų W
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

    // ��ų E
    private void Eskill()
    {
        // ���콺 ��ǥ �Ʊ����� �ǵ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(this.gameObject.layer == LayerMask.NameToLayer("Friendly"))
            { 

            }
        }
        // �ڱ� �ڽſ��� �ǵ�
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            fixedball();
        }
    }// end Eskill()

    // ��ų R
    private void Rskill()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SkillR_range == true) // layermask "Enermy"�� ���ݴ��
            { 
                for (int i = 0; i < 2; i++)
                {   // �� ��ǥ ��ü ������ ����
                    enermy[i].transform.position = Vector3.MoveTowards(enermy[i].transform.position, objDir, Time.deltaTime * objSpeed);    
                }
            }
        }
    }// end Rskill()

    // ��ü ���� �� �� Ȯ��
    private void OnCollisionStay(Collision collision)
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Enermy"))
        {
            SkillR_range = true; //������ ���� ����
        }
    }



    //  ��ų ��Ÿ�� ����
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
