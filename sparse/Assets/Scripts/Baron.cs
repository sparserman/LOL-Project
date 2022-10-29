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
    [SerializeField]
    private GameObject BaronSkillEffect1 = null;    // ��ų ����
    [SerializeField]
    private GameObject BaronSkillTarget = null;    // ��ų Ÿ��

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
            if (Vector3.Distance(transform.position, attackTarget.transform.position) <= 8f)
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

                        BaronSkill1();  // �ٷ� ��ų ����

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

    public void AttackEvent(int n)
    {
        switch (n)
        {
            case 0:
                Debug.Log("shot");
                BaronShotObj cpyObj;
                cpyObj = Instantiate<BaronShotObj>(AttackEffect, ThroatPos.position, Quaternion.identity);
                cpyObj.gameObject.SetActive(true);
                cpyObj.SetTarget(attackTarget);
                break;
            case 1:
                Debug.Log("shot");
                BaronShotObj cpyObj2;
                BaronSkillTarget.gameObject.SetActive(true);
                cpyObj2 = Instantiate<BaronShotObj>(AttackEffect, ThroatPos.position, Quaternion.identity);
                cpyObj2.gameObject.SetActive(true);
                cpyObj2.SetTarget(BaronSkillTarget);
                break;
        }
    }

    public void BaronSkill1()
    {
        BaronSkillEffect1.transform.GetChild(1).gameObject.SetActive(false);
        BaronSkillEffect1.gameObject.SetActive(true);
        BaronSkillEffect1.transform.forward = attackTarget.transform.position - transform.position;
        BaronSkillEffect1.transform.position = transform.position;
    }
}
