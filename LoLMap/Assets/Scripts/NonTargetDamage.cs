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

                // è����
                if (targets[i].layer.Equals(9))
                {
                    if (user.GetComponent<ChampController>().team != targets[i].GetComponent<ChampController>().team)
                    {
                        targets[i].GetComponent<ChampController>().Damaged(power);
                    }
                }
                // �̴Ͼ��̸�
                else if (targets[i].layer.Equals(10))
                {
                    if (user.GetComponent<ChampController>().team != targets[i].GetComponent<Minion>().team)
                    {
                        targets[i].GetComponent<Minion>().Damaged(power);
                    }
                }

            }
        }
    }
}
