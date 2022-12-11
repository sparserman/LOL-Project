using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Poppy : MonoBehaviour
{
    ChampController p;

    void Start()
    {
        // �ǻ�
        p = GetComponent<ChampController>();
    }

    void Update()
    {
        
    }

    public void PassiveAttack(GameObject p_player)
    {

    }

    public void QSkill(Vector3 p_pos)
    {
        // ������ȯ
        p.SetDir(p_pos);

        p.moveStop = true;
        p.ani.SetBool("isQSkill", true);
        p.ani.SetInteger("isMove", 0);
        p.ani.SetInteger("AttackMotion", 0);
        p.ani.SetBool("isAttack", false);
        p.agent.speed = 0;
        p.QSkillCool = false;

        // �÷��̾� ����
        transform.forward = p.destination;

        StartCoroutine(SkillCooltime(1));
    }

    // Q ��ų ����Ʈ ����
    public void QSkillEffect(int num)
    {
        switch (num)
        {
            case 0:
                GameObject cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyQEffectPos") as GameObject);
                cpyObj.transform.position = transform.position;
                cpyObj.transform.forward = new Vector3(p.destination.x, -180, p.destination.z);

                // ����Ʈ ���� ����
                cpyObj.transform.GetChild(1).GetComponent<NonTargetDamage>().user = gameObject;
                cpyObj.transform.GetChild(1).GetComponent<NonTargetDamage>().power = 200;
                // 1.5�ʵ� ����
                Destroy(cpyObj, 1.5f);
                break;
            case 1:
                p.ani.SetBool("isQSkill", false);
                p.moveStop = false;
                break;
        }
    }

    IEnumerator SkillCooltime(int p_skill)
    {
        switch (p_skill)
        {
            case 0:
                yield return new WaitForSeconds(p.PCoolTime);
                p.PSkillCool = true;
                p.shield.SetActive(true);
                break;
            case 1:
                yield return new WaitForSeconds(p.QCoolTime);
                p.QSkillCool = true;
                break;
            case 2:
                yield return new WaitForSeconds(p.WCoolTime);
                p.WSkillCool = true;
                break;
            case 3:
                yield return new WaitForSeconds(p.ECoolTime);
                p.ESkillCool = true;
                break;
            case 4:
                yield return new WaitForSeconds(p.RCoolTime);
                p.RSkillCool = true;
                break;
        }
    }
}
