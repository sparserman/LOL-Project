using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    void Update()
    {
        transform.position += transform.forward * 0.2f;
    }

    public void Fire(float t)
    {
        Debug.Log("�ϳ�");
        Destroy(gameObject, t * 0.15f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && other.name != "Poppy")
        {
            Debug.Log("Ʈ����");
            Destroy(gameObject);
        }
    }
}
