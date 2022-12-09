using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> allList;
    public List<GameObject> playerList;

    public bool serverConnected = false;

    void Start()
    {

    }

    void Update()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetComponent<ChampController>().hpBar != null)
            {
                playerList[i].GetComponent<ChampController>().hpBar.transform.position =
                    Camera.main.WorldToScreenPoint(new Vector3(
                        playerList[i].transform.position.x,
                        playerList[i].transform.position.y + 3,
                        playerList[i].transform.position.z));
            }
        }
    }

    public void ChampSelect(int p_champNum)
    {
        // 카메라 고정 및 챔피언 조종권 부여
        Camera.main.GetComponent<CameraControl>().m_Champ = playerList[(int)p_champNum];
        playerList[(int)p_champNum].GetComponent<ChampController>().inOperation = true;
    }


    private static GameManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager GetInstance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
}
