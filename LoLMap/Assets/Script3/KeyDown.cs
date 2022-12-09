using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDown : MonoBehaviour
{
    bool store = false;

    [SerializeField]
    public GameObject st;
   
    void Update()
    {
        storePopUp();
    }


    void storePopUp()
    {
       if(Input.GetKeyDown(KeyCode.I))
        {
            if(st.activeSelf == true)
            {
                st.SetActive(false);
            }
            if(st.activeSelf == false)
            {
                st.SetActive(true);
            }
        }
    }
}
