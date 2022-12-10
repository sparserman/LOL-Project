using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDamage : MonoBehaviour
{
    private List<GameObject> targets;

    public float power = 200;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 대상 찾기
    private void FindTarget(GameObject p_target)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                if (!targets[i].Equals(p_target))
                {
                    break;
                }
            }
            targets.Add(p_target);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        FindTarget(other.gameObject);
    }

    private void OnDestroy()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                // 챔프면
                if (targets[i].layer.Equals(9))
                {
                    targets[i].GetComponent<ChampController>().Damaged(power);
                }
                // 미니언이면
                else if (targets[i].layer.Equals(10))
                {
                    targets[i].GetComponent<Minion>().Damaged(power);
                }
            }
        }
        else
        {

        }
    }
}
