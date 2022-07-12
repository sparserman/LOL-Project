using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public GameObject Poppy;
    public float a = 0.5f;
    private bool isMove = false;

    public void MoveStop()
    {
        isMove = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            isMove = true;
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        while (true)
        {
            if(!isMove)
            {
                break;
            }
            yield return null;
            float t = 0f;
            t += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Poppy.transform.position, a);
            transform.forward = Poppy.transform.position;

            if (Vector3.Distance(transform.position, Poppy.transform.position) <= 1f)
            {
                Debug.Log("Á¤»óµµÂø");
                break;
            }
        }

        if(isMove)
        {
            transform.position = Poppy.transform.position - transform.forward;
            isMove = false;
        }
        else
        {
            
        }
    }
}
