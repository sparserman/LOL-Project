using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppyPassiveSkill : MonoBehaviour
{
    protected GameObject target;
    protected ChampMove Poppy;

    private bool hit = false;   // 공격 됐는지
    private bool end = false;   // 도착 했는지

    private float time = 0;     // 시간 체크

    private Vector3 droppos;

    private float startTime;

    public GameObject ShieldObj;

    public float num = 0;

    private void Start()
    {
        startTime = Time.time;
    }

    public void Init( GameObject p_target, ChampMove p_poppy )
    {
        Poppy = p_poppy;
        target = p_target;

        target = Poppy.attackTarget;
    }

    void Update()
    {
        Move();
        Attack();
        Drop();
    }

    private void Move()
    {
        if (!hit)
        {
            var dir = target.transform.position - transform.position;
            transform.position += dir.normalized * Time.deltaTime * 25f;
            transform.forward = dir;
        }
    }

    private void Attack()
    {
        if(Vector3.Distance(transform.position, target.transform.position) < 0.1f && !hit)
        {
            hit = true;
            // 중간 지점
            droppos = (target.transform.position + Poppy.transform.position) * 0.5f;
            droppos.y = 0.5f;

            RaycastHit rayhit;
            if(Physics.Raycast((target.transform.position + Poppy.transform.position) * 0.5f
                , droppos + transform.right, out rayhit, 5f))
            {
                droppos -= transform.right * 3;
            }
            else
            {
                droppos += transform.right * 3;
            }
        }
    }

    private void Drop()
    {
        if(hit && !end)
        {
            SlerpFn(transform.position, droppos, 60f);
            transform.Rotate(0, 0, Time.deltaTime * num);
            if(Vector3.Distance(transform.position, droppos) < 0.2f)
            {
                end = true;
                transform.position = droppos;
            }
        }

        if(end)
        {
            time += Time.deltaTime;
            if(time >= 5)
            {
                Destroy(gameObject);
            }
        }
    }

    private void SlerpFn(Vector3 sunrise, Vector3 sunset, float journeyTime)
    {
        Vector3 center = (sunrise + sunset) * 0.5f;

        center -= new Vector3(0, 1, 0);

        Vector3 riseRelCenter = sunrise - center;
        Vector3 setRelCenter = sunset - center;

        float fracComplete = (Time.time - startTime) / journeyTime;

        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
    }

    private void OnTriggerStay(Collider other)
    {
        if(end)
        {
            Debug.Log("방패 습득!");
            if(other.name == "Poppy")
            {
                GameObject cpyObj;
                cpyObj = Instantiate(ShieldObj, Poppy.transform.position, Quaternion.identity);
                cpyObj.SetActive(true);

                Destroy(gameObject);
            }
            else if(other.tag == "Player")
            {
                Destroy(gameObject);
            }
        }
    }
}
