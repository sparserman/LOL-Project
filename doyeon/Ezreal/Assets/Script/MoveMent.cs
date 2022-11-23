using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveMent : MonoBehaviour
{
    ChamState Cstate;                        // è�Ǿ� ����
    ItemState Istate;                        // ������ ����

    public Camera _camera;                   // ī�޶�
    private Animator _animator;              // �ִϸ��̼�

    [SerializeField]
    private float _MoveSpeed = 2.0f;         // �̵��ӵ�
    private float _rotateSpeed = 10.0f;      // ȸ���ӵ�
    private Vector3 _movePos = Vector3.zero; // �̵���ǥ
    private Vector3 _moveDir = Vector3.zero; // ȸ����ǥ

    private bool _isClick = true;            // Ŭ���� üũ 
    private float _Clilckdelay = 0;          // Ŭ�� ������

    public GameObject _target;               // ���ݰ��� ���
    private bool _ismove = true;             // �̵� ����
    private bool hide = false;               // ���� ���� - �ν�
    private bool skillhit = false;           // ��ų ���� Ȯ�� - �нú��


    [SerializeField]
    private LayerMask wall, enemy ,ground;  // layerMask

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
        Cstate.Attack = 60f +  ((Cstate.LV - 1) * 2.5f);          // ���ݷ�  
        Cstate.DefPen = 0;                                        // �������

        Cstate.Mag = 0;                                           // �ֹ���
        Cstate.MagPen = 0;                                        // ���� �����

        Cstate.SkillAcc = 0;                                      // ��ų����(��Ÿ�� ����)
        Cstate.Speed = 325f;                                      // �̵��ӵ�
        Cstate.AttackRange = 550f;                                // ��Ÿ�
        Cstate.AttackSpeed = 1f  + ((Cstate.LV - 1) * 0.025f);    // ���ݼӵ� 
        Cstate.Critical = 0;                                      // ġ��Ÿ

        Cstate.Def = 24f + ((Cstate.LV - 1) * 4.7f);              // ����
        Cstate.MagDef = 30f + ((Cstate.LV - 1) * 1.3f);           // �������׷�
        Cstate.HP = 600f + ((Cstate.LV - 1) * 102f);              // ü��
        Cstate.HPRegen = 4f + ((Cstate.LV - 1) * 0.65f);          // ü�����
        Cstate.MP = 375f + ((Cstate.LV - 1) * 70f);               // ����
        Cstate.MPRegen = 8.5f + ((Cstate.LV - 1) * 0.65f);        // �������
        Cstate.Ten = 0;                                           // ������(���׷�)

    }

    // ������ �ʿ� ����ġ
    void LvUp()
    {
        if (Cstate.Xp >= (280 + (Cstate.LV -1) * 110))  // �������� �ʿ� ����ġ  ((lv -1) * 110) + 280  --/ ���� 1 : 280
        {
            if (Cstate.LV == 18)                        // �ְ��� 18���� �޼��� ������ ����
            {
                return;
            }
            Cstate.LV += 1;
            Cstate.Xp = 0;
            Ezreal_NomalState();
        }
    }// end LvUp()
    
    // �̵�
    private void Move()
    {
        if(_isClick)
        {
            if(Input.GetMouseButton(1) && !_ismove) // ���콺 ��Ŭ��
            {
              
                _isClick = false;
                RaycastHit _hit;

                if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out _hit,100.0f, ground)) // �� Ŭ��
                {
                    _movePos = _hit.point;
                    _movePos.y = 0;     // ���� �� ���� -> y���̵� ���� 
                    _moveDir = _movePos - transform.position;
                    Debug.Log("�̵���ǥ :" + _movePos);
                } // end raycast
            } // end ��Ŭ��
        } // Ŭ�� üũ

        // ȸ���ϴ� ���� ���ϱ� 
        Vector3 newDir = Vector3.RotateTowards(transform.forward, _moveDir, _rotateSpeed * Time.deltaTime, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position = Vector3.MoveTowards(transform.position, _movePos, _MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _movePos) < 0.1f)
        {
            _animator.SetBool("isMove", false);  //-------- "isMove" ���� ��� --------------------------------------------------------//
        }


        // ���콺 ��Ŭ�� ���� ���� Ŭ�� �����ϰ� ��� -- Ŭ���� �̵� ����
        if (Input.GetMouseButtonUp(1))
        {
            _isClick = true;
            _Clilckdelay = 0;
        }
    } //end move()

    // ���� ����Ű - S
    private void Stop()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // ���� ����
            _animator.SetBool("isAttack", false);
            _target = null;

            // �̵� ����
            _animator.SetBool("isMove", false);
            _MoveSpeed = 0;

            // + �ʿ��  ��ų ���� �߰� 
        }


        // ���콺 Ŭ�� ���� ���� ��ȯ
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

    // �⺻ ����
    private void Attack()
    {

    }// end Attack()


    // �нú�
    private void Passive()      
    {
        int i = 0;               // �нú� ��ø Ƚ�� Ȯ��

        if (skillhit == true)
        { 
            if( i > 5)          // �нú� ��ø �ִ� 5ȸ
            {
                return;
            }
            Cstate.AttackSpeed *= 1.1f;     // ��ø�� ���ݼӵ� 0.1 ���
            i++;
        }
    }// end Passive()      

    // ��ų Q
    private void Qskill()
    {

    }// end Qskill()

    // ��ų W
    private void Wskill()
    {

    }// end Wskill()

    // ��ų E
    private void Eskill()
    {

    }// end Eskill()

    // ��ų R
    private void Rskill()
    {

    }// end Rskill()



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


} // end class
