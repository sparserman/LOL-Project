using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ready : MonoBehaviour
{
    public GameObject pickBackground;
    public bool ready = false;
    public bool pick = false;

    public void PlayerReady()
    {
        if (ready)
        {
            pickBackground.GetComponent<Image>().color = new Color32(24, 43, 55, 255);
            GetComponent<Image>().color = new Color32(120, 120, 120, 255);
            pick = true;
        }
    }
}
