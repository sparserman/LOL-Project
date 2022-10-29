using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveMents : MonoBehaviour
{

    [SerializeField] float MoveSpeed = 10.0f;
    [SerializeField] float rotateSpeed = 10.0f;

    [SerializeField]
    private Vector3 movePos = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;
    public Animator animator;

    void Update()
    {
        // 마우스 오른쪽 클릭
        if(Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isMove",true);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //충돌체 있는지 확인 (이동 불가지역)
            if(Physics.Raycast(ray,out RaycastHit raycastHit))
            {

                if(raycastHit.transform.tag == "ObstacleObj")
                {
                    return;
                }


                movePos = raycastHit.point;
                moveDir = movePos - transform.position;
                moveDir.y = transform.position.y;
            }
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
          
        }
        if(movePos != Vector3.zero)
        {
            Vector3 dir = movePos - transform.position;
        }

        // 회전하는 방향 구하기 
        Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDir, rotateSpeed * Time.deltaTime, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position = Vector3.MoveTowards(transform.position, movePos, MoveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,movePos) < 0.1f)
        {
            animator.SetBool("isMove", false);
        }
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

}
