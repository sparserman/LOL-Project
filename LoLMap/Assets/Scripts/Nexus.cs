using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour
{
    public Team team;

    public float time = 1;
    public float deley = 1;

    private int count = 0;

    void Start()
    {
        
    }

    void Update()
    {
        SpawnTime();
    }

    private float currentTime = 0;
    public void SpawnTime()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= time)
        {
            currentTime = 0;
            count++;
            StartCoroutine(SpawnCoroutine());
            time = 10;
        }
    }

    public void SpawnMinion()
    {
        // �̴Ͼ� ����
        GameObject obj = Instantiate(Resources.Load("Prefabs/" + "BlueWarrierMinion") as GameObject);
        obj.transform.position = transform.position;
        if(team == Team.BLUE)
        {
            obj.transform.position = new Vector3(-18.77f, 5.74f, -44.57f);
        }

        // �̴Ͼ� ����
        obj.GetComponent<Minion>().MaxHP += count * 10;
        obj.GetComponent<Minion>().HP += count * 10;
        obj.GetComponent<Minion>().ATK += count;
        obj.GetComponent<Minion>().team = team;
        obj.GetComponent<Minion>().redNexus = GameManager.GetInstance.redNexus;
        obj.GetComponent<Minion>().blueNexus = GameManager.GetInstance.blueNexus;
        // ù Ÿ�� (�� �ؼ���)
        if (obj.GetComponent<Minion>().team == Team.RED)
        {
            obj.GetComponent<Minion>().attackTarget = GameManager.GetInstance.blueNexus;
        }
        else if (obj.GetComponent<Minion>().team == Team.BLUE)
        {
            obj.GetComponent<Minion>().attackTarget = GameManager.GetInstance.redNexus;
        }
    }
    
    IEnumerator SpawnCoroutine()
    {
        int cnt = 0;
        while (true)
        {
            cnt++;

            yield return new WaitForSeconds(deley);
            SpawnMinion();

            if(cnt == 3)
            {
                break;
            }
        }
    }
}
