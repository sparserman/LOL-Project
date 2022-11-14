using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject poppy;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 temppos;
        temppos = transform.position;
        temppos = poppy.transform.position;
        temppos.y = poppy.transform.position.y + 1;

        transform.position = temppos;
    }
}
