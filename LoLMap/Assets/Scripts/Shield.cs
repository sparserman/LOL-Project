using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
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
        temppos = GameManager.GetInstance.playerList[0].transform.position;
        temppos.y = GameManager.GetInstance.playerList[0].transform.position.y + 1;

        transform.position = temppos;
    }
}
