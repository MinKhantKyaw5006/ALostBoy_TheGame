using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss_Idle : StateMachineBehaviour
{
    Rigidbody2D rb;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;
        RunToPlayer(animator);

        if(BossFight.Instance.attackCountdown <= 0)
        {
            BossFight.Instance.AttackHandler();
            BossFight.Instance.attackCountdown = BossFight.Instance.attackTimer;
        }

        if (!BossFight.Instance.Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    private void RunToPlayer(Animator animator)
    {
        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) >= BossFight.Instance.attackRange)
        {
            animator.SetBool("Run", true);
            Debug.Log("Boss Running");
        }

        else
        {
 
            return;
        }
    }
}