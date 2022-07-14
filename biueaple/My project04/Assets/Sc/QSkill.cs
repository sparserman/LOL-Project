using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSkill : MonoBehaviour
{
    public float Damage;
    public GameObject Chara;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Chara)
        {
            return;
        }
        if (other.transform.CompareTag("Unit"))
        {
            for(int i = 0; i< Chara.GetComponent<PlayerSkill>().colliders.Count;i++)
            {
                if(Chara.GetComponent<PlayerSkill>().colliders[i] == other.gameObject)
                {
                    return;
                }
            }
            for (int i = 0; i < Chara.GetComponent<PlayerSkill>().colliders2.Count; i++)
            {
                if (Chara.GetComponent<PlayerSkill>().colliders2[i] == other.gameObject)
                {
                    return;
                }
            }
            Chara.GetComponent<PlayerSkill>().colliders.Add(other.gameObject);
        }
    }


}
