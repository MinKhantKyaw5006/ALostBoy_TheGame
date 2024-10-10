using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    Rigidbody2D rb; 
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>(); 
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TargetPlayerPosition(animator);

        if (BossFight.Instance.attackCountdown <= 0)
        {
            BossFight.Instance.AttackHandler();
            BossFight.Instance.attackCountdown = BossFight.Instance.attackTimer;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Run", false);
        Debug.Log("Enemy is not Running");
    }

    private void TargetPlayerPosition(Animator animator)
    {
        if (BossFight.Instance.Grounded())
        {
            BossFight.Instance.Flip();
            Vector2 _target = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y);
            Vector2 _newPosition = Vector2.MoveTowards(rb.position, _target, BossFight.Instance.runSpeed * Time.fixedDeltaTime);
            BossFight.Instance.runSpeed = BossFight.Instance.speed;
            rb.MovePosition(_newPosition);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
            Debug.Log("Fallilng to the Grounded");
        }

        if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= BossFight.Instance.attackRange)
        {
            animator.SetBool("Run", false);
            Debug.Log("Enemy is not running");
        }

        else
        {
            return; 
        }
    }


}
