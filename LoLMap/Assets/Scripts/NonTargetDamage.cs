using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NonTargetDamage : MonoBehaviour
{
    public List<GameObject> targets;

    public float power = 200;

    public GameObject user;         // 스킬 사용자

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

    // 적 챔피언 및 미니언에게 데미지
    private void BoomEvent()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {

                // 챔프면
                if (targets[i].layer.Equals(9))
                {
                    if (user.GetComponent<ChampController>().team != targets[i].GetComponent<ChampController>().team)
                    {
                        targets[i].GetComponent<ChampController>().Damaged(power);
                    }
                }
                // 미니언이면
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
