using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baron : MonoBehaviour
{
    public GameObject attackTarget;    // ���� ��ǥ

    public bool hit = false;    // ���� ���� ����
    private bool attackCheck = true;     // ���� ���� ����
    
    [SerializeField]
    private BaronShotObj AttackEffect = null;     // ���� ����Ʈ

    private float time = 0;
    [SerializeField]
    private int patience = 100;     // �γ���

    private Animator ani;

    [SerializeField]
    private int attackCount = 0;    // ���� Ƚ��

    [SerializeField]
    private Transform ThroatPos;    // ���� ������ ��ġ

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
            // �⺻ ���� ��ǥ�� ���� ��
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 7f)
            {
                // ���� ���� ���� �϶�
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

                    // 3�� ������ ��ų ���
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

            // ��Ÿ� ������ ������ ��
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

    // ���� ������ (���ݼӵ�)
    Coroutine m_AttackDelayCoroutine = null;
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(3);
        attackCheck = true;

        m_AttackDelayCoroutine = null;
    }
}
