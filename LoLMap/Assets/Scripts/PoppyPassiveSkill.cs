using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppyPassiveSkill : MonoBehaviour
{

    protected GameObject target;
    protected ChampController Champ;

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

    public void Init( GameObject p_target, ChampController p_poppy )
    {
        Champ = p_poppy;
        target = p_target;

        target = Champ.attackTarget;
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
            // 히트
            Champ.Aggro();

            hit = true;
            // 중간 지점
            droppos = (target.transform.position + Champ.transform.position) * 0.5f;
            droppos.y = transform.position.y + 0.5f;

            Debug.DrawLine((target.transform.position + Champ.transform.position) * 0.5f
                , droppos + transform.right * 5f, Color.green, 3f);

            // 벽 체크
            if(Physics.Raycast((target.transform.position + Champ.transform.position) * 0.5f
                , transform.right, 5f))
            {
                Debug.Log("오른쪽 벽 있음");
                droppos -= transform.right * 3;
            }
            else
            {
                Debug.Log("없음");
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
                cpyObj = Instantiate(ShieldObj, Champ.transform.position, Quaternion.identity);
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
