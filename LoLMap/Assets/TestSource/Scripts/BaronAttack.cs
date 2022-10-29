using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BaronAttack : BaronBase
{

    //[SerializeField]
    //protected GameObject m_AttackTarget;


    //[SerializeField]
    //protected GameObject m_LookTarget;

    //public void SetAttackTarget(GameObject p_target)
    //{
    //    m_AttackTarget = p_target;
    //}


    public void SetAttackTarget(GameObject p_AttackTarget)
    {
        m_AttackTarget = p_AttackTarget;
        m_LinkMob.Attack();

        AttackDelay();
    }

    async void AttackDelay()
    {
        await Task.Delay(1000);

        m_LinkMob.m_ISAttackCheck = true;
        m_ThisAnimator.Play("Chease", 0);
        m_LinkMob.m_ActAnimator.Play("model|Idle1_Base_model");
    }

    Animator m_ThisAnimator = null;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_LookTarget = m_TargetBaron.attackTarget;


    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(m_LinkMob.attackTarget == null )
        {
            animator.Play("Chease", 0);
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
