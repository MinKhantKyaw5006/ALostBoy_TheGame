//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Boss_Jump : StateMachineBehaviour
//{
//    private Rigidbody2D rb;
//    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
//    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        rb = animator.GetComponentInParent<Rigidbody2D>();     
//    }

//    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
//    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        DiveAttack(); 
//    }

//    private void DiveAttack()
//    {
//        if(BossFight.Instance.diveAttack)
//        {
//            BossFight.Instance.Flip();
//            Vector2 _newPos = Vector2.MoveTowards(rb.position, BossFight.Instance.moveToPosition, BossFight.Instance.speed * 3 * Time.fixedDeltaTime);
//            rb.MovePosition(_newPos);

//            if (BossFight.Instance.TounchedWall())
//            {
//                BossFight.Instance.moveToPosition.x = rb.position.x;
//                _newPos = Vector2.MoveTowards(rb.position, BossFight.Instance.moveToPosition, BossFight.Instance.speed * 1.5f * Time.fixedDeltaTime);
//            }
//            float _distance = Vector2.Distance(rb.position, _newPos);
//            if(_distance < 0.1f)
//            {
//                BossFight.Instance.Dive();
//            }
//        } 
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Jump : StateMachineBehaviour
{
    private Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
        // Set jumping state to true
        animator.SetBool("isJumping", true);

        // Ensure Z rotation is frozen during the jump
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DiveAttack(animator);
    }

    private void DiveAttack(Animator animator)
    {
        if (BossFight.Instance.diveAttack)
        {
            BossFight.Instance.Flip();
            Vector2 targetPosition = BossFight.Instance.moveToPosition;
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, BossFight.Instance.speed * 3 * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            // Debugging line to see current position and target position
            Debug.Log($"Boss Position: {rb.position}, Target Position: {targetPosition}");

            if (BossFight.Instance.TounchedWall())
            {
                BossFight.Instance.moveToPosition.x = rb.position.x; // Prevent boss from moving through walls
                newPos = Vector2.MoveTowards(rb.position, BossFight.Instance.moveToPosition, BossFight.Instance.speed * 1.5f * Time.fixedDeltaTime);
            }

            float distance = Vector2.Distance(rb.position, newPos);
            if (distance < 0.1f)
            {
                BossFight.Instance.Dive(); // Execute dive attack
                // Reset jumping state when dive attack is executed
                animator.SetBool("isJumping", false);
            }
        }
    }

    // Called when the state machine exits this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset jumping state to false when exiting the jump state
        animator.SetBool("isJumping", false);

        // Ensure Z rotation stays frozen after the jump
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
