using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampPick : MonoBehaviour
{
    public GameObject myPick;

    public Ready readyButton;

    public void Pick()
    {
        if (!readyButton.pick)
        {
            myPick.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
            myPick.GetComponent<Image>().color = Color.white;
            readyButton.ready = true;
            readyButton.GetComponent<Image>().color = new Color32(255, 212, 102, 255);
        }
    }
}
