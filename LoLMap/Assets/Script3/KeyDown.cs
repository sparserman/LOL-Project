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
        storePopUpButton();
    }


    void storePopUpButton()
    {
       if(Input.GetKeyDown(KeyCode.I))
       {
            StorePopUp();
       }
    }

    public void StorePopUp()
    {
        if (st.activeSelf == true)
        {
            st.SetActive(false);
        }
        else if (st.activeSelf == false)
        {
            st.SetActive(true);
        }
    }
}
