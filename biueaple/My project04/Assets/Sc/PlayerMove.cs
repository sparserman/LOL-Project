using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Unit unit;
    public Vector3 MousePosi;
    public Vector3 MouseWorldPosi;
    public Camera cam;
    public Vector3 MovePoint;
    public bool MoveStop;
    public bool RatateStop;
    public bool walking;
    public Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        MovePoint.y = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(transform.GetComponent<PlayerSkill>().AttackOn)
            {
                transform.GetComponent<PlayerSkill>().AttackOn = false;
            }

            MousePosi = Input.mousePosition;
            MousePosi.z = 10.0f;
            MouseWorldPosi = cam.ScreenToWorldPoint(MousePosi);
            MovePoint = MouseWorldPosi;
            MovePoint.y = 0.0f;

            RaycastHit hit = new RaycastHit();

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray.origin,ray.direction,out hit))
            {
                if(hit.transform.CompareTag("Unit"))
                {
                    transform.GetComponent<PlayerSkill>().AttackOn = true;
                    transform.GetComponent<PlayerSkill>().AttackTarget = hit.transform.gameObject;
                    MovePoint = hit.transform.position;
                }
            }

            if (!RatateStop)
            {
                transform.LookAt(MovePoint);
            }
        }
        if (!MoveStop)
        {
            transform.position = Vector3.MoveTowards(transform.position, MovePoint, unit.state.Speed * Time.deltaTime);
        }
        if(Vector3.Distance(this.transform.position, MovePoint) > 0.01f)
        {
            walking = false;
            ani.SetBool("Walking", false);
        }
        else
        {
            ani.SetBool("Walking", true);
            walking = true;
        }
    }

    public void LookPosi(Vector3 vec)
    {
        if (!RatateStop)
        {
            transform.LookAt(vec);
        }
    }
}
