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
        BossFight.Instance.divingCollider.SetActive(true);
    }

    //// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    BossFight.Instance.divingCollider.SetActive(true);

    //    if (BossFight.Instance.Grounded())
    //    {
    //        BossFight.Instance.divingCollider.SetActive(false); 

    //        if(!callOnce)
    //        {
    //            BossFight.Instance.DivingPillars();
    //            animator.SetBool("Dive", false);
    //            BossFight.Instance.ResetAttacks();
    //            callOnce = true;
    //        }
    //    }
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    BossFight.Instance.divingCollider.SetActive(true);
    //    Debug.Log("dive onstateupdate is called");

    //    if (BossFight.Instance.Grounded())
    //    {
    //        BossFight.Instance.divingCollider.SetActive(false);

    //        // Check for collision with PlayerController when diving
    //        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(rb.position, 1f); // Adjust the radius as necessary
    //        foreach (var hitCollider in hitColliders)
    //        {
    //            if (hitCollider.CompareTag("Player"))
    //            {
    //                // Show message or log that the boss dived into the player
    //                Debug.Log("Boss dived into player!");
    //                hitCollider.GetComponent<PlayerController>().TakeDamage(BossFight.Instance.damage);
    //                break; // Exit loop after hitting the player
    //            }
    //        }

    //        if (!callOnce)
    //        {
    //            BossFight.Instance.DivingPillars();
    //            animator.SetBool("Dive", false);
    //            BossFight.Instance.ResetAttacks();
    //            callOnce = true;
    //        }
    //    }
    //}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BossFight.Instance.divingCollider.SetActive(true);
        Debug.Log("Dive OnStateUpdate is called");

        // Check for collision with PlayerController while diving
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(rb.position, 1f); // Adjust the radius as necessary
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Show message or log that the boss is diving into the player
                Debug.Log("Boss dived into player!");
                hitCollider.GetComponent<PlayerController>().TakeDamage(BossFight.Instance.damage);
                // You may want to break here if you only want to hit the first player detected
                break; // Exit loop after hitting the player
            }
        }

        // Continue with the diving logic
        if (BossFight.Instance.Grounded())
        {
            BossFight.Instance.divingCollider.SetActive(false);

            if (!callOnce)
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
