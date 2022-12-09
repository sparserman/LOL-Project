using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        // ī�޶� ���� �� è�Ǿ� ������ �ο�
        Camera.main.GetComponent<CameraControl>().m_Champ = playerList[(int)p_champNum];
        playerList[(int)p_champNum].GetComponent<ChampController>().inOperation = true;

        // �ڱ� ü�� �� �� ����
        if (playerList[(int)p_champNum].GetComponent<ChampController>().inOperation)
        {
            playerList[(int)p_champNum].GetComponent<ChampController>().hpBar.transform.GetChild(1).
                transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
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
