using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField]
    private Vector3 pos;

    void Start()
    {
        StartCoroutine(Move());
    }

    void Update()
    {
        
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (Vector3.Distance(transform.position, pos) < 0.5f)
            {
                Debug.Log("도착");
                break;
            }
            // 방향
            transform.forward = pos - transform.position;

            // 이동
            transform.position += transform.forward * 0.1f;
        }
    }
}
