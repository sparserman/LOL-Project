using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronSkillEffect1 : MonoBehaviour
{
    public float move;
    public float speed;
    public float max;

    void Update()
    {
        if(move > max)
        {
            gameObject.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
            move = 0;
            transform.localPosition = new Vector3(0, 0.1f, 0);
        }
        transform.position += transform.forward * (Time.deltaTime * speed);
        move += Time.deltaTime * speed;
    }
}
