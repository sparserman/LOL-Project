using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objPos : MonoBehaviour
{
    [SerializeField] float objspeed = 10f;
    public GameObject ch;
    private Vector3 objPOS;
    private Vector3 objDir;

    [SerializeField]
    private Vector3 ClickWPos;
    private bool Chsel = true;

    [SerializeField]
    private int skillcode = 0;

    [SerializeField]
    private Vector3 Worldpos;
    [SerializeField]
    private Vector3 Screenpos;

    [SerializeField]
    protected Camera m_InGameCamera = null;
    [SerializeField]
    protected LayerMask m_LandHitMask;

    [SerializeField]
    protected bool ballfixedch = true;

    bool m_ISQClicked = false;
    bool m_ISEClicked = false;


    void Start()
    {
        fixedball();
    }


    void Update()
    {
        
        CalMousePos(); // �ǽð� ���콺 ��ǥ
        inputkeycode(); // Ű �Է� 

        if (ballfixedch)
        {
            fixedball();
        }

    }

    void inputkeycode()
    {
        // Ű �Է�
        //Q
        if (Input.GetKey(KeyCode.Q))
        {
            m_ISQClicked = true;
            Debug.Log("Input key Q");
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            m_ISQClicked = false;
        }
        if (m_ISQClicked)
        {
            skill_Q();
            Debug.Log("skill Q");
        }

        //W
        if (Input.GetKeyDown(KeyCode.W))
        {
            skill_W();
            Debug.Log("skill W");
        }

        //E
        if (Input.GetKey(KeyCode.E))
        {
            m_ISEClicked = true;
            Debug.Log("Input key E");
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            m_ISEClicked = false;
        }
        if (m_ISEClicked)
        {
            skill_E();
            Debug.Log("Skill E");
        }

        // R
        if (Input.GetKeyDown(KeyCode.R))
        {
            skill_R();
            Debug.Log("Skill R");
        }
    }

    // ���� ĳ���� ����ٴ� �� 
    void fixedball()
    {
        objPOS = ch.transform.position + new Vector3(0.2f, 0.5f, 0.3f);
        transform.position = ch.transform.position + objPOS;
    }

    //���콺 ��ǥ, ���� ��ǥ 
    void CalMousePos()
    {
        Screenpos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Screenpos);

        if (Physics.Raycast(ray, out RaycastHit hitdata, 100, 1 << 8))
        {
            Worldpos = hitdata.point;
        }
        transform.position = Worldpos;
    }

   

    Coroutine m_MoveCortouine = null;
    IEnumerator UpdateTargetMove( Vector3 p_targetpos )
    {
        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, p_targetpos, Time.deltaTime * objspeed);
            yield return null;

            
            if ( (transform.position - p_targetpos).magnitude <= 0.1f )
            {
                transform.position = p_targetpos;
                break;
            }
        }

        m_MoveCortouine = null;
    }

 
    // ������Ʈ �̵�
    void ObjMove()
    {
        if(m_MoveCortouine != null)
        {
            StopCoroutine(m_MoveCortouine);
            m_MoveCortouine = null;
        }
        transform.position = Vector3.MoveTowards(transform.position, ClickWPos, Time.deltaTime * objspeed);
        StartCoroutine( UpdateTargetMove(ClickWPos) );
    }

    void skill_Q()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ballfixedch = false;
            ObjMove();
        }

        
    }

    void skill_W()
    {
      
       
    }

    void skill_E()
    {

    }

    void skill_R()
    {

    }

}
