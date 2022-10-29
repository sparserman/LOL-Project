using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objPos : MonoBehaviour
{
    [SerializeField] float objspeed = 10f;
    public GameObject ch;
    public GameObject efc_w;
    public GameObject efc_e;


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
    protected Camera m_InGameCamera = null;
    [SerializeField]
    protected LayerMask m_LandHitMask;

    [SerializeField]
    protected bool ballfixedch = true;

    bool m_ISQClicked = false;
    bool m_ISEClicked = false;

    public GameObject Chara;

    bool SkillR_range = false;
    public GameObject[] ch2 = null;




    void Start()
    {
        fixedball();
    }


    void Update()
    {
        
       CalMousePos(); // 실시간 마우스 좌표
        inputkeycode(); // 키 입력 
       

        if (ballfixedch)
        {
            fixedball();
        }

    }

    void inputkeycode()
    {
        // 키 입력
        //Q
        if (Input.GetKeyDown(KeyCode.Q))
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

    // 공이 캐릭터 따라다닐 때 
    void fixedball()
    {
        transform.position = ch.transform.position + new Vector3(0.5f, 0.5f, 0.5f);
    }

    //마우스 좌표, 월드 좌표 
    void CalMousePos()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitdata))
        {
            Worldpos = hitdata.point;
        }

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
                m_ISQClicked = false;
                break;
            }
        }

        m_MoveCortouine = null;
    }

 
    // 오브젝트 이동
    void ObjMove()
    {
        if(m_MoveCortouine != null)
        {
            StopCoroutine(m_MoveCortouine);
            m_MoveCortouine = null;
        }
        StartCoroutine( UpdateTargetMove(ClickWPos) );
    }

    void skill_Q()
    {
            ClickWPos = Worldpos;
            ballfixedch = false;
            ObjMove();
        

        
    }

    void skill_W()
    {
        efc_w.transform.position = transform.position + new Vector3(0.0f,0.5f,0.0f);
        efc_w.SetActive(true);
        StartCoroutine(isW());
    }

    void skill_E()
    {

        if(Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.E))
        {
            if(gameObject.tag == "Friendly")
            ObjMove();
            ballfixedch = true;
        }
         
    }

    void skill_R()
    {
               
    }

    IEnumerator isW()    
    {
        yield return new WaitForSeconds(2f);
        efc_w.SetActive(false);
    }

    //콜라이더 충돌중
    private void OnCollisionStay(Collision collision)
    {
        if(gameObject.tag == "Enermy")
        {
            SkillR_range = true; //범위내 적이 있음
            for(int i = 0;i<2;i++)
            {
                ch2[i].transform.position = Vector3.MoveTowards(ch2[i].transform.position, objDir, Time.deltaTime * objspeed);
            }
        }
    }



}
