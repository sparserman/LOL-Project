using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarBack : MonoBehaviour
{
    public GameObject hp;

    // 실드 상태 체크
    public ChampController champ;

    void Update()
    {
        if (champ.shieldHP > 0)
        {
            transform.GetComponent<Scrollbar>().size = (champ.state.HP + champ.shieldHP) / champ.state.MaxHP;
        }
        else
        {
            if (transform.GetComponent<Scrollbar>().size >= hp.GetComponent<Scrollbar>().size)
            {
                transform.GetComponent<Scrollbar>().size -= 0.005f;
            }
            else if (transform.GetComponent<Scrollbar>().size < hp.GetComponent<Scrollbar>().size)
            {
                transform.GetComponent<Scrollbar>().size = hp.GetComponent<Scrollbar>().size;
            }
        }
    }
}
