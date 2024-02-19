/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [SerializeField] private float chaseDistance;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (currentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chase);
                }
                break;

            case EnemyStates.Bat_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                FlipBat();
                break;

            case EnemyStates.Bat_Stunned:
                break;

            case EnemyStates.Bat_Death:
                break;
        }
    }

    void FlipBat()
    {
      
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    // Define the BatStates enum within the Bat class
    protected enum BatStates
    {
        Flying_Idle,
        Flying_Follow,
        // Additional states like Attack, Hurt, Die will be added later
    }

    // Declare currentBatState using the BatStates enum
    protected BatStates currentBatState = BatStates.Flying_Idle;
    private SpriteRenderer spriteRenderer; // To flip the sprite based on direction
    [SerializeField] private float detectionRadius = 5f; // Distance within which the bat starts following the player.

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the bat.");
        }
        // Additional initialization for the bat, if necessary
    }

    protected override void Update()
    {
        base.Update(); // Call the base class Update method

        // Handle bat-specific behavior based on states
        switch (currentBatState)
        {
            case BatStates.Flying_Idle:
                CheckForPlayer(); // Check if the player is in range to follow
                // Here you might want to implement idling behavior, such as hovering
                break;
            case BatStates.Flying_Follow:
                FollowPlayer(); // Follow the player
                break;
                // Additional case statements for other states will be added later
        }
    }

    private void CheckForPlayer()
    {
        // Check for player within detection radius
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            currentBatState = BatStates.Flying_Follow;
        }
        else
        {
            currentBatState = BatStates.Flying_Idle;
        }
    }

    private void FollowPlayer()
    {
        // Calculate the direction vector from the bat to the player
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Set the velocity in the direction to the player, multiplied by speed
        rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * speed;

        // Check if the bat needs to flip based on the player's position relative to the bat
        if ((player.transform.position.x < transform.position.x && !spriteRenderer.flipX) ||
            (player.transform.position.x > transform.position.x && spriteRenderer.flipX))
        {
            Flip();
        }
    }


    private void Flip()
    {
        // Flip the sprite to face the player
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // Additional methods for Attack, Hurt, Die will be added later
}
