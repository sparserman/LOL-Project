using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BaronChease : BaronBase
{

    //[SerializeField]
    //protected GameObject m_AttackTarget;


    //[SerializeField]
    //protected GameObject m_LookTarget;

    ////public void SetAttackTarget(GameObject p_target)
    ////{
    ////    m_AttackTarget = p_target;
    ////}



    public void InitChease(Baron p_LinkBaron)
    {
        //m_TargetBaron = p_LinkBaron;

    }


    public void SetAttackTarget(GameObject p_AttackTarget)
    {
        m_AttackTarget = p_AttackTarget;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_LookTarget = m_TargetBaron.attackTarget;
        m_ThisAnimator = animator;


    }

    Animator m_ThisAnimator;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if( m_LinkMob.attackTarget != null)
        {
            animator.SetTrigger("Attack");
            m_LinkMob.m_ActAnimator.Play("model|sru_baron_attack1_model");
        }
        Attack();

    }

    private float time = 0;
    private void Attack()
    {
        if (m_LinkMob.attackTarget != null && m_LinkMob.patience != 0)
        {
            // 기본 공격 목표에 도착 시
            if (Vector3.Distance(m_LinkMob.transform.position, m_LinkMob.attackTarget.transform.position) <= 7f)
            {
                // 공격 가능 상태 일때
                if (m_LinkMob.m_ISAttackCheck)
                {
                    m_LinkMob.m_ISAttackCheck = false;

                    // 3번 공격후 스킬 사용
                    if (m_LinkMob.attackCount == 3)
                    {
                        m_ThisAnimator.SetTrigger("Skill1");
                        m_LinkMob.attackCount = 0;
                    }
                    else
                    {
                        m_LinkMob.attackCount++;
                    }

                }
            }

            // 사거리 밖으로 나갔을 때
            if (Vector3.Distance(m_LinkMob.transform.position, m_LinkMob.attackTarget.transform.position) >= 7f)
            {
                time += Time.deltaTime;
                if (time >= 0.5f)
                {
                    m_LinkMob.patience -= 10;
                    time = 0;
                }
            }
            else
            {
                time = 0;
            }
        }


        if (m_LinkMob.patience == 0)
        {
            m_AttackTarget = null;
            m_LinkMob.patience = 100;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
