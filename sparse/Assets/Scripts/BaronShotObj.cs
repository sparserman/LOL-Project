using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronShotObj : MonoBehaviour
{
    private GameObject attackTarget = null;

    public void SetTarget(GameObject m_Target)
    {
        attackTarget = m_Target;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 temppos = (attackTarget.transform.position - transform.position + new Vector3(0, 0.5f, 0));
        transform.position += temppos.normalized * Time.deltaTime * 10;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == attackTarget)
        {
            Destroy(gameObject);
        }
    }
}
