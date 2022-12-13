using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoppyPassiveSkill : MonoBehaviour
{

    public GameObject target;       // 타겟
    public ChampController champ;   // 사용자

    private bool hit = false;   // 공격 됐는지
    private bool end = false;   // 도착 했는지

    private float time = 0;     // 시간 체크

    private Vector3 droppos;    // 떨어질 위치

    private float startTime;

    public float num = 0;

    public float power = 100;       // 데미지
    public float shieldHP = 100;    // 보호막

    private bool dieCheck = false;  // 대상이 죽었는 지 체크

    private void Start()
    {
        startTime = Time.time;
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
        if (!hit)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                // 히트
                champ.Aggro();
                if (target.layer.Equals(9))
                {
                    target.GetComponent<ChampController>().Damaged(power);
                }
                else
                {
                    target.GetComponent<Minion>().Damaged(power);
                }

                hit = true;
                // 중간 지점
                droppos = (target.transform.position + champ.transform.position) * 0.5f;
                droppos.y = transform.position.y + 0.5f;

                Debug.DrawLine((target.transform.position + champ.transform.position) * 0.5f
                    , droppos + transform.right * 5f, Color.green, 3f);

                // 벽 체크
                if (Physics.Raycast((target.transform.position + champ.transform.position) * 0.5f
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
    }

    private bool hitCheck = false;
    private void Drop()
    {
        if (hit && !end)
        {
            if (!hitCheck)
            {
                hitCheck = true;
                if (target.layer.Equals(9))
                {
                    if (target.GetComponent<ChampController>().die)
                    {
                        dieCheck = true;
                    }
                }
                else if (target.layer.Equals(10))
                {
                    if(target.GetComponent<Minion>().die)
                    {
                        dieCheck = true;
                    }
                }
            }
            // 죽으면 바로 자기에게 날아오기
            if (dieCheck)
            {
                transform.position +=
                    (new Vector3(champ.transform.position.x,
                    champ.transform.position.y + 1,
                    champ.transform.position.z) - transform.position).normalized * Time.deltaTime * 25f;

                if (Vector3.Distance(transform.position, new Vector3(champ.transform.position.x,
                    champ.transform.position.y + 1,
                    champ.transform.position.z)) < 0.2f)
                {
                    CreateShield();
                }
            }
            // 아니면 바닥으로 떨어지기
            else
            {
                SlerpFn(transform.position, droppos, 60f);
                transform.Rotate(0, 0, Time.deltaTime * num);
                if (Vector3.Distance(transform.position, droppos) < 0.2f)
                {
                    end = true;
                    transform.position = droppos;
                }
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
            // 주웠을 때
            if(other.gameObject == champ.gameObject)
            {
                CreateShield();
            }
            // 상대가 밟았을 때
            else if(other.gameObject.layer.Equals(9))
            {
                if (other.GetComponent<ChampController>().team != champ.team)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void CreateShield()
    {
        GameObject cpyObj;
        cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyPassiveShield") as GameObject
            , champ.transform.position, Quaternion.identity);
        cpyObj.SetActive(true);
        champ.GetComponent<ChampController>().shieldHP = shieldHP;
        // 정보 전달
        cpyObj.GetComponent<Shield>().champ = champ;
        cpyObj.GetComponent<Shield>().shieldHP = shieldHP;

        Destroy(gameObject);
    }
}
