using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppyPassiveSkill : MonoBehaviour
{

    protected GameObject target;
    protected ChampController Champ;

    private bool hit = false;   // ���� �ƴ���
    private bool end = false;   // ���� �ߴ���

    private float time = 0;     // �ð� üũ

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
            // ��Ʈ
            Champ.Aggro();

            hit = true;
            // �߰� ����
            droppos = (target.transform.position + Champ.transform.position) * 0.5f;
            droppos.y = transform.position.y + 0.5f;

            Debug.DrawLine((target.transform.position + Champ.transform.position) * 0.5f
                , droppos + transform.right * 5f, Color.green, 3f);

            // �� üũ
            if(Physics.Raycast((target.transform.position + Champ.transform.position) * 0.5f
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
            Debug.Log("���� ����!");
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
