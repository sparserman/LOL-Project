using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public Camera cam;
    public Unit unit;
    public float QCoolTime;
    public float WCoolTime;
    public float ECoolTime;
    public float RCoolTime;

    public GameObject Q11;
    public GameObject Q12;
    public GameObject Q21;
    public GameObject Q22;
    public GameObject Q31;
    public GameObject Q32;

    public int QCount;

    public List<GameObject> colliders = new List<GameObject>();
    public List<GameObject> colliders2 = new List<GameObject>();

    public float Q11Range;
    public float Q12Range;
    public float Q21Range;
    public float Q22Range;
    public float Q31Range;
    public float Q32Range;

    public GameObject WObj;
    public float WRange;
    public float WSpeed;
    public GameObject Wtarget;
    public Vector3 WBack;
    public float WbackSpeed;

    public float ERange;
    public float ESpeed;

    public Vector3 MousePosi;
    public Vector3 MouseWorldPosi;
    public Vector3 MovePoint;

    public float RDamage;

    public Animator ani;

    public bool AkeyOn;
    public bool AttackOn;
    public float Attackend;
    public GameObject AttackTarget;
    public bool Attack;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(QCoolTime >= 0)
        {
            QCoolTime -= Time.deltaTime;
        }
        if(WCoolTime >= 0)
        {
            WCoolTime -= Time.deltaTime;
        }
        if (ECoolTime >= 0)
        {
            ECoolTime -= Time.deltaTime;
        }
        if (RCoolTime >= 0)
        {
            RCoolTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AttackOn = false;
            AttackTarget = null;
            Attack = false;

            MousePosi = Input.mousePosition;
            MousePosi.z = 10.0f;
            MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
            MovePoint = MouseWorldPosi;
            MovePoint.y = 0.0f;
            transform.LookAt(MovePoint);

            switch (QCount)
            {
                case 0:
                    Q11.transform.position = transform.position + transform.forward * Q11Range;
                    Q12.transform.position = transform.position + transform.forward * Q12Range;
                    Q11.transform.rotation = transform.rotation;
                    Q12.transform.rotation = transform.rotation;
                    Q11.SetActive(true);
                    Q12.SetActive(true);
                    StartCoroutine(Q1C());
                    colliders.Clear();
                    colliders2.Clear();
                    QCount++;
                    ani.SetTrigger("Q1");
                    break;
                case 1:
                    Q21.transform.position = transform.position + transform.forward * Q21Range;
                    Q22.transform.position = transform.position + transform.forward * Q22Range;
                    Q21.transform.rotation = transform.rotation;
                    Q22.transform.rotation = transform.rotation;
                    Q21.SetActive(true);
                    Q22.SetActive(true);
                    StartCoroutine(Q2C());
                    colliders.Clear();
                    colliders2.Clear();
                    QCount++;
                    ani.SetTrigger("Q2");
                    break;
                case 2:
                    Q31.transform.position = transform.position + transform.forward * Q31Range;
                    Q32.transform.position = transform.position + transform.forward * Q32Range;
                    Q31.transform.rotation = transform.rotation;
                    Q32.transform.rotation = transform.rotation;
                    Q31.SetActive(true);
                    Q32.SetActive(true);
                    StartCoroutine(Q3C());
                    colliders.Clear();
                    colliders2.Clear();
                    QCount = 0;
                    ani.SetTrigger("Q3");
                    break;
            }
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            MousePosi = Input.mousePosition;
            MousePosi.z = 10.0f;
            MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
            MovePoint = MouseWorldPosi;
            MovePoint.y = 0.0f;
            transform.LookAt(MovePoint);

            transform.GetComponent<PlayerMove>().MovePoint = transform.position;

            StartCoroutine(WC1());
            AttackOn = false;
            AttackTarget = null;
            Attack = false;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            MousePosi = Input.mousePosition;
            MousePosi.z = 10.0f;
            MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
            MovePoint = MouseWorldPosi;
            MovePoint.y = 0.0f;
            transform.LookAt(MovePoint);

            transform.GetComponent<PlayerMove>().MoveStop = true;
            ani.SetBool("E", true);
            StartCoroutine(EC());
            AttackOn = false;
            AttackTarget = null;
            Attack = false;
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            ani.SetBool("R", true);
            RDamage = unit.state.Attack * 0.5f;
            unit.state.Attack += RDamage;
            StartCoroutine(RC());
            AttackOn = false;
            AttackTarget = null;
            Attack = false;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            AkeyOn = true;
        }
        if(Input.GetMouseButtonDown(0))
        {
            if(AkeyOn)
            {
                MousePosi = Input.mousePosition;
                MousePosi.z = 10.0f;
                MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
                MovePoint = MouseWorldPosi;
                MovePoint.y = 0.0f;
                transform.GetComponent<PlayerMove>().MovePoint = MovePoint;
                AttackOn = true;
                AkeyOn = false;
                StartCoroutine(AttackC());
            }
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            AttackOn = false;
            AttackTarget = null;
            Attack = false;
            transform.GetComponent<PlayerMove>().MovePoint = transform.position;
        }
        if(Attack)
        {
            Collider[] coll = Physics.OverlapSphere(transform.position, 2.0f);
            for(int i = 0; i < coll.Length;i++)
            {
                if (coll[i].gameObject != transform.gameObject)
                {
                    Debug.Log("123");
                    if (coll[i].CompareTag("Unit") )
                    {
                        if(AttackTarget == null)
                        {
                           
                        }
                        else
                        {
                            if (coll[i] == AttackTarget)
                            {
                                Debug.Log("123");
                                coll[i].GetComponent<Unit>().state.HP -= 10;
                            }
                        }
                    }
                }
            }
        }
        
    }

    IEnumerator AttackC()
    {
        float time = 0.0f;
        while(true)
        {
            time += Time.deltaTime;
            yield return null;
            if (!AttackOn)
            {
                Debug.Log("123");
                break;
            }
            if(time >= 0.5f)
            {
                Debug.Log("456");
                Attack = true;
                break;
            }
        }
    }

    IEnumerator Q1C()
    {
        transform.GetComponent<PlayerMove>().MoveStop = true;
        transform.GetComponent<PlayerMove>().RatateStop = true;
        yield return new WaitForSeconds(1.0f);
        Q11.GetComponent<BoxCollider>().enabled = true;
        Q12.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<PlayerMove>().MoveStop = false;
        transform.GetComponent<PlayerMove>().RatateStop = false;
        yield return new WaitForSeconds(0.1f);
        Q11.SetActive(false);
        Q12.SetActive(false);
        Q11.GetComponent<BoxCollider>().enabled = false;
        Q12.GetComponent<BoxCollider>().enabled = false;
        for (int i = 0; i < colliders2.Count; i++)
        {
            if (colliders2[i].CompareTag("Unit"))
            {
                colliders2[i].GetComponent<Unit>().state.HP -= unit.state.Attack * 1.6f;
                Debug.Log($"{colliders2[i].name} {unit.state.Attack * 1.6f}대미지");
            }
        }
        for (int i = 0;i<colliders.Count;i++)
        {
            if (colliders[i].CompareTag("Unit"))
            {
                colliders[i].GetComponent<Unit>().state.HP -= unit.state.Attack;
                Debug.Log($"{colliders[i].name} {unit.state.Attack}대미지");
            }
        }
    }

    IEnumerator Q2C()
    {
        transform.GetComponent<PlayerMove>().MoveStop = true;
        transform.GetComponent<PlayerMove>().RatateStop = true;
        yield return new WaitForSeconds(1.0f);
        Q21.GetComponent<BoxCollider>().enabled = true;
        Q22.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<PlayerMove>().MoveStop = false;
        transform.GetComponent<PlayerMove>().RatateStop = false;
        yield return new WaitForSeconds(0.1f);
        Q21.SetActive(false);
        Q22.SetActive(false);
        Q21.GetComponent<BoxCollider>().enabled = false;
        Q22.GetComponent<BoxCollider>().enabled = false;
        for (int i = 0; i < colliders2.Count; i++)
        {
            if (colliders2[i].CompareTag("Unit"))
            {
                colliders2[i].GetComponent<Unit>().state.HP -= unit.state.Attack * 1.25f * 1.6f;
                Debug.Log($"{colliders2[i].name} {unit.state.Attack * 1.25f * 1.6f}대미지");
            }
        }
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].CompareTag("Unit"))
            {
                colliders[i].GetComponent<Unit>().state.HP -= unit.state.Attack * 1.25f;
                Debug.Log($"{colliders[i].name} {unit.state.Attack * 1.25f}대미지");
            }
        }
    }

    IEnumerator Q3C()
    {
        transform.GetComponent<PlayerMove>().MoveStop = true;
        transform.GetComponent<PlayerMove>().RatateStop = true;
        yield return new WaitForSeconds(1.0f);
        Q31.GetComponent<BoxCollider>().enabled = true;
        Q32.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<PlayerMove>().MoveStop = false;
        transform.GetComponent<PlayerMove>().RatateStop = false;
        yield return new WaitForSeconds(0.1f);
        Q31.SetActive(false);
        Q32.SetActive(false);
        Q31.GetComponent<BoxCollider>().enabled = false;
        Q32.GetComponent<BoxCollider>().enabled = false;
        for (int i = 0; i < colliders2.Count; i++)
        {
            if (colliders2[i].CompareTag("Unit"))
            {
                colliders2[i].GetComponent<Unit>().state.HP -= unit.state.Attack * 1.5f * 1.6f;
                Debug.Log($"{colliders2[i].name} {unit.state.Attack * 1.5f * 1.6f}대미지");
            }
        }
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].CompareTag("Unit"))
            {
                colliders[i].GetComponent<Unit>().state.HP -= unit.state.Attack * 1.5f;
                Debug.Log($"{colliders[i].name} {unit.state.Attack * 1.5f}대미지");
            }
        }
    }
    IEnumerator WC1()
    {
        WObj.transform.position = transform.position;
        WObj.transform.rotation = transform.rotation;
        Vector3 vec = WObj.transform.position + WObj.transform.forward * WRange;
        WObj.SetActive(true);
        while (true)
        {
            WObj.transform.position =  Vector3.MoveTowards(WObj.transform.position, vec, WSpeed * Time.deltaTime);
            yield return null;
            if (Vector3.Distance(WObj.transform.position,vec) <= 0.1f)
            {
                WObj.SetActive(false);
                break;
            }
        }
    }

    public void StartWC2()
    {
        StartCoroutine(WC2());
    }

    IEnumerator WC2()
    {
        yield return new WaitForSeconds(1.5f);
        while(true)
        {
            Wtarget.transform.position = Vector3.MoveTowards(Wtarget.transform.position, WBack, WbackSpeed * Time.deltaTime);
            yield return null;
            if(Vector3.Distance(Wtarget.transform.position, WBack) <= 0.1f)
            {
                break;
            }
        }
    }

    IEnumerator EC()
    {
        float time = 0;
        MousePosi = Input.mousePosition;
        MousePosi.z = 10.0f;
        MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
        MovePoint = MouseWorldPosi;
        MovePoint.y = 0.0f;
        Vector3 vec1 = MovePoint - transform.position;
        Vector3 vec2 = vec1.normalized;


        Vector3 vec = transform.position + vec2 * ERange;

        transform.GetComponent<PlayerMove>().LookPosi(vec);
        while (true)
        {
            time += Time.deltaTime;
            yield return null;
            Vector3 Minus = Vector3.MoveTowards(transform.position, vec, ESpeed * Time.deltaTime);
            Vector3 plus = Minus - transform.position;
            transform.position += plus;
            Q11.transform.position += plus;
            Q12.transform.position += plus;
            Q21.transform.position += plus;
            Q22.transform.position += plus;
            Q31.transform.position += plus;
            Q32.transform.position += plus;
            if (time >= 0.5f || Vector3.Distance(transform.position, vec) <= 0.1f)
            {
                transform.GetComponent<PlayerMove>().MoveStop = false;
                ani.SetBool("E", false);
                break;
            }
        }
        transform.GetComponent<PlayerMove>().MovePoint = transform.position;
    }
    IEnumerator RC()
    {
        yield return new WaitForSeconds(10.0f);
        unit.state.Attack -= RDamage;
        ani.SetBool("R", false);
    }
    private void OnDrawGizmos()
    {

    }
}
