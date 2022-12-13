using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoppyPassiveSkill : MonoBehaviour
{

    public GameObject target;       // Ÿ��
    public ChampController champ;   // �����

    private bool hit = false;   // ���� �ƴ���
    private bool end = false;   // ���� �ߴ���

    private float time = 0;     // �ð� üũ

    private Vector3 droppos;    // ������ ��ġ

    private float startTime;

    public float num = 0;

    public float power = 100;       // ������
    public float shieldHP = 100;    // ��ȣ��

    private bool dieCheck = false;  // ����� �׾��� �� üũ

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
                // ��Ʈ
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
                // �߰� ����
                droppos = (target.transform.position + champ.transform.position) * 0.5f;
                droppos.y = transform.position.y + 0.5f;

                Debug.DrawLine((target.transform.position + champ.transform.position) * 0.5f
                    , droppos + transform.right * 5f, Color.green, 3f);

                // �� üũ
                if (Physics.Raycast((target.transform.position + champ.transform.position) * 0.5f
                    , transform.right, 5f))
                {
                    Debug.Log("������ �� ����");
                    droppos -= transform.right * 3;
                }
                else
                {
                    Debug.Log("����");
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
            // ������ �ٷ� �ڱ⿡�� ���ƿ���
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
            // �ƴϸ� �ٴ����� ��������
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
            // �ֿ��� ��
            if(other.gameObject == champ.gameObject)
            {
                CreateShield();
            }
            // ��밡 ����� ��
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
        // ���� ����
        cpyObj.GetComponent<Shield>().champ = champ;
        cpyObj.GetComponent<Shield>().shieldHP = shieldHP;

        Destroy(gameObject);
    }
}
