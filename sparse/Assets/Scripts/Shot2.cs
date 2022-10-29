using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot2 : MonoBehaviour
{
    Vector3 dir;
    float speed;
    float range;

    Vector3 firstPos;

    void Update()
    {
        ShotMove();   
    }

    public void SetShotInfo(Vector3 p_dir, float p_speed, float p_range)
    {
        firstPos = transform.position;
        p_dir.y = 0.5f;
        dir = p_dir - transform.position;
        speed = p_speed;
        range = p_range;
    }

    private void ShotMove()
    {
        transform.position += dir.normalized * speed * Time.deltaTime;
        if(Vector3.Distance(firstPos, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Mob") || other.CompareTag("Red"))
        {
            Destroy(gameObject);
        }

    }
}
