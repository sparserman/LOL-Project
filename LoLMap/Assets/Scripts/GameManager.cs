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
        HPBarPos();
    }

    private void HPBarPos()
    {
        for (int i = 0; i < allList.Count; i++)
        {
            // è����
            if (allList[i].layer.Equals(9))
            {
                if (allList[i].GetComponent<ChampController>().hpBar != null)
                {
                    allList[i].GetComponent<ChampController>().hpBar.transform.position =
                        Camera.main.WorldToScreenPoint(new Vector3(
                            allList[i].transform.position.x,
                            allList[i].transform.position.y,
                            allList[i].transform.position.z + 2));
                }
            }
            // �̴Ͼ��̸�
            else if (allList[i].layer.Equals(10))
            {
                if (allList[i].GetComponent<Minion>().hpBar != null)
                {
                    allList[i].GetComponent<Minion>().hpBar.transform.position =
                        Camera.main.WorldToScreenPoint(new Vector3(
                            allList[i].transform.position.x,
                            allList[i].transform.position.y,
                            allList[i].transform.position.z + 2));
                }
            }
        }
    }

    public void ChampSelect(int p_champNum)
    {
        // ī�޶� ���� �� è�Ǿ� ������ �ο�
        Camera.main.GetComponent<CameraControl>().m_Champ = playerList[p_champNum];
        playerList[p_champNum].GetComponent<ChampController>().inOperation = true;

        // �ڱ� ü�� �� �� ����
        if (serverConnected)
        {
            playerList[p_champNum].GetComponent<ChampController>().hpBar.transform.GetChild(1).
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
