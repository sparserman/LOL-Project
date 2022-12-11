using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarBack : MonoBehaviour
{
    public GameObject hp;

    void Update()
    {
        if(transform.GetComponent<Scrollbar>().size >= hp.GetComponent<Scrollbar>().size)
        {
            transform.GetComponent<Scrollbar>().size -= 0.005f;
        }
        else if (transform.GetComponent<Scrollbar>().size < hp.GetComponent<Scrollbar>().size)
        {
            transform.GetComponent<Scrollbar>().size = hp.GetComponent<Scrollbar>().size;
        }
    }
}
