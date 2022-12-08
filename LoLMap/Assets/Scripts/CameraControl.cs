using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject m_Champ;

    public float my;
    public float mz;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 temppos;
        temppos = m_Champ.transform.position;
        temppos.y = my;
        temppos.z = m_Champ.transform.position.z + mz;
        transform.position = temppos;
    }
}
