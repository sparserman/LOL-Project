using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour
{
    public Dummy dummy;
    private void OnTriggerStay(Collider other)
    {
        if(other.name == "dummy")
        {
            dummy.MoveStop();
        }
    }
}
