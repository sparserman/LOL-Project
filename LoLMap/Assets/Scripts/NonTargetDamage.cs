using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NonTargetDamage : MonoBehaviour
{
    public List<GameObject> targets;

    public float power = 200;

    public GameObject user;         // ��ų �����

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        targets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.gameObject);
    }

    // �� è�Ǿ� �� �̴Ͼ𿡰� ������
    private void BoomEvent()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (!targets[i].Equals(user))
                {
                    // è����
                    if (targets[i].layer.Equals(9))
                    {
                        targets[i].GetComponent<ChampController>().Damaged(power);
                    }
                    // �̴Ͼ��̸�
                    else if (targets[i].layer.Equals(10))
                    {
                        targets[i].GetComponent<Minion>().Damaged(power);
                    }
                }
            }
        }
    }
}
