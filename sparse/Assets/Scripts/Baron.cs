using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baron : MonoBehaviour
{
    public GameObject attackTarget;    // 공격 목표

    public bool hit = false;    // 공격 받은 상태
    private bool attackCheck = true;     // 공격 장전 상태
    
    [SerializeField]
    private BaronShotObj AttackEffect = null;     // 공격 이펙트

    private float time = 0;
    [SerializeField]
    private int patience = 100;     // 인내심

    private Animator ani;

    [SerializeField]
    private int attackCount = 0;    // 공격 횟수

    [SerializeField]
    private Transform ThroatPos;    // 공격 나오는 위치

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (attackTarget != null && patience != 0)
        {
            // 기본 공격 목표에 도착 시
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 7f)
            {
                // 공격 가능 상태 일때
                if (attackCheck)
                {
                    ani.SetTrigger("Attack");

                    if (m_AttackDelayCoroutine != null)
                    {
                        StopCoroutine(m_AttackDelayCoroutine);
                        m_AttackDelayCoroutine = null;
                    }

                    m_AttackDelayCoroutine = StartCoroutine(AttackDelay());

                    attackCheck = false;

                    // 3번 공격후 스킬 사용
                    if (attackCount == 3)
                    {
                        ani.Play("model|sru_baron_spell2_channel_model", 0);
                        ani.SetTrigger("Skill1");
                        attackCount = 0;
                    }
                    else
                    {
                        attackCount++;
                    }
                    
                }
            }

            // 사거리 밖으로 나갔을 때
            if (Vector3.Distance(transform.position, attackTarget.transform.position) >= 7f)
            {
                time += Time.deltaTime;
                if (time >= 0.5f)
                {
                    patience -= 10;
                    time = 0;
                }
            }
            else
            {
                time = 0;
            }
        }


        if (patience == 0)
        {
            attackTarget = null;
            patience = 100;
        }
    }

    // 공격 딜레이 (공격속도)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(3);
        attackCheck = true;

        m_AttackDelayCoroutine = null;
    }
}
