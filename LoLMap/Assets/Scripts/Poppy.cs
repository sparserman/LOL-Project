using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Poppy : MonoBehaviour
{
    ChampController p;

    void Start()
    {
        // 뽀삐
        p = GetComponent<ChampController>();
    }

    void Update()
    {
        
    }

    public void PassiveAttack()
    {
        // 패시브 공격 목표에 도착 시
        if (Vector3.Distance(transform.position, p.attackTarget.transform.position) <= 7f && p.PSkillCool)
        {
            // 이동 중지
            p.ani.SetInteger("isMove", 0);
            p.agent.speed = 0;
            // 적 방향 쳐다보기
            transform.forward = p.attackTarget.transform.position - transform.position;

            // 어택 모션 끄기
            p.ani.SetInteger("AttackMotion", 0);

            // 공격 가능 상태 일때
            if (p.attackCheck)
            {
                p.moveStop = true;
                p.PSkillCool = false;
                p.ani.SetBool("isPassive", true);

                if (p.m_AttackDelayCoroutine != null)
                {
                    p.StopCoroutine(p.m_AttackDelayCoroutine);
                    p.m_AttackDelayCoroutine = null;
                }

                p.ani.SetBool("isAttack", true);

                p.m_AttackDelayCoroutine = p.StartCoroutine(p.AttackDelay());

                p.attackCheck = false;
            }

        }
    }

    public void PassiveEffect(int num)
    {
        switch (num)
        {
            case 0:
                GameObject cpyObj;
                cpyObj = Instantiate<GameObject>(Resources.Load("Prefabs/" + "PoppyPassiveEffect") as GameObject
                    , new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
                cpyObj.transform.forward = p.attackTarget.transform.position;
                cpyObj.transform.rotation = new Quaternion(0, 0, 0, 90);
                cpyObj.gameObject.SetActive(true);
                // 정보 설정
                cpyObj.GetComponent<PoppyPassiveSkill>().target = p.attackTarget;
                cpyObj.GetComponent<PoppyPassiveSkill>().champ = p;
                cpyObj.GetComponent<PoppyPassiveSkill>().power = 80 + p.state.Attack * 1.1f;
                // 방패 제거
                p.shield.SetActive(false);
                break;
            case 1:
                p.ani.SetBool("isPassive", false);
                p.moveStop = false;

                StartCoroutine(SkillCooltime(0));
                break;
        }
    }

    public void QSkill(Vector3 p_pos)
    {
        // 방향전환
        p.SetDir(p_pos);

        p.moveStop = true;
        p.ani.SetBool("isQSkill", true);
        p.ani.SetInteger("isMove", 0);
        p.ani.SetInteger("AttackMotion", 0);
        p.ani.SetBool("isAttack", false);
        p.agent.speed = 0;
        p.QSkillCool = false;

        // 플레이어 방향
        transform.forward = p.destination;

        StartCoroutine(SkillCooltime(1));
    }

    // Q 스킬 이펙트 생성
    public void QSkillEffect(int num)
    {
        switch (num)
        {
            case 0:
                GameObject cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyQEffectPos") as GameObject);
                cpyObj.transform.position = transform.position;
                cpyObj.transform.forward = new Vector3(p.destination.x, -180, p.destination.z);

                // 이펙트 정보 설정
                cpyObj.transform.GetChild(1).GetComponent<NonTargetDamage>().user = gameObject;
                cpyObj.transform.GetChild(1).GetComponent<NonTargetDamage>().power = 100 + p.state.Attack * 1.5f;
                // 1.5초뒤 삭제
                Destroy(cpyObj, 1.5f);
                break;
            case 1:
                p.ani.SetBool("isQSkill", false);
                p.moveStop = false;
                break;
        }
    }

    // W 스킬 이펙트 생성
    public void WSkill()
    {
        p.WSkillCool = false;
        Debug.Log("W");
        GameObject cpyObj;
        cpyObj = Instantiate(Resources.Load("Prefabs/" + "PoppyWEffect") as GameObject
            , transform.position, Quaternion.identity);
        cpyObj.transform.forward = new Vector3(0, 90, 0);

        if (m_WSkillEffectCoroutine != null)
        {
            StopCoroutine(m_WSkillEffectCoroutine);
            m_WSkillEffectCoroutine = null;
        }
        m_WSkillEffectCoroutine = StartCoroutine(WSkillEffect(cpyObj));

        StartCoroutine(SkillCooltime(2));
    }

    // W 스킬 지속
    Coroutine m_WSkillEffectCoroutine = null;
    IEnumerator WSkillEffect(GameObject obj)
    {
        p.ani.SetBool("isWSkill", true);
        p.state.Speed += 300;
        float t = 0;
        while (true)
        {
            yield return null;
            t += Time.deltaTime;

            if (t > 0.5f)
            {
                p.ani.SetBool("isWSkill", false);
            }

            obj.transform.position = transform.position;
            if (t >= 2)
            {
                break;
            }
        }
        p.state.Speed -= 300;
        Destroy(obj);

        m_WSkillEffectCoroutine = null;
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
