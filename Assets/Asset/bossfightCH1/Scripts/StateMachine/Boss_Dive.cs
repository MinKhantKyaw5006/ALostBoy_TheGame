using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Dive : StateMachineBehaviour
{
    private Rigidbody2D rb;
    bool callOnce; 
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();  
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BossFight.Instance.divingCollider.SetActive(true);

        if (BossFight.Instance.Grounded())
        {
            BossFight.Instance.divingCollider.SetActive(false); 
            
            if(!callOnce)
            {
                BossFight.Instance.DivingPillars();
                animator.SetBool("Dive", false);
                BossFight.Instance.ResetAttacks();
                callOnce = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;    
    }
}
