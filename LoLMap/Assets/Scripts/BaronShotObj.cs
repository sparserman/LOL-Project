using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronShotObj : MonoBehaviour
{
    [SerializeField]
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
        if (attackTarget != null)
        {
            Vector3 temppos = (attackTarget.transform.position - transform.position + new Vector3(0, 0.5f, 0));
            transform.position += temppos.normalized * Time.deltaTime * 10;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("일반 충돌");
        if (other.gameObject == attackTarget)
        {
            Debug.Log("대상과 충돌");
            Destroy(gameObject);
        }
    }
}
