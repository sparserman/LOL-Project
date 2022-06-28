using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChampMove : MonoBehaviour
{
    private Camera cam;
    private Animator ani;
    private ChamState state;
    private NavMeshAgent agent;

    private bool isMove;
    private Vector3 destination;

    private bool isQSkill;

    public Transform QSkillPos;
    public GameObject QSkillObj;

    void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        state.Speed = 325;
    }
    void Update()
    {
        Move();
        QSkill();
    }

    private void Move()
    {
        // 마우스 우클릭
        if(Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                ChampSetDestination(hit.point);
            }
        }

        // 방향전환과 이동
        if(isMove)
        {
            agent.speed = state.Speed * 0.01f;
            var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
            transform.forward = dir;

            // 목적지 도착 시
            if (agent.angularSpeed <= 0.1f)
            {
                isMove = false;
            }
        }
        
    }

    private void QSkill()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            RaycastHit hit;
            
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                SetDir(hit.point);
            }
            

            GameObject cpyObj;
            cpyObj = Instantiate(QSkillObj, QSkillPos.position, Quaternion.identity);
            cpyObj.transform.forward = new Vector3(destination.x, -90, destination.z);
            Destroy(cpyObj, 1);
        }
    }
     
    private void ChampSetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
        destination = dest;
        isMove = true;
    }

    private void SetDir(Vector3 dir)
    {
        destination = dir - transform.position;
        transform.forward = dir;
        isQSkill = true;
    }
}
