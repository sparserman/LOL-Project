using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSkill : MonoBehaviour
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
            Chara.GetComponent<PlayerSkill>().StartWC2();
            Chara.GetComponent<PlayerSkill>().Wtarget = other.gameObject;
            Chara.GetComponent<PlayerSkill>().WBack = other.transform.position;
            
            transform.gameObject.SetActive(false);
        }
    }
}
