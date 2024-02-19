
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField] private float detectionRadius = 5f; // Distance within which the zombie starts following the player.
    private SpriteRenderer spriteRenderer; // To flip the sprite based on direction
    private bool facingRight;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the zombie.");
        }

        // Determine the initial direction based on the player's position
        facingRight = (player.transform.position.x > transform.position.x);
        spriteRenderer.flipX = facingRight; // If the player is to the right, flip the sprite to face right
    }

    protected override void Update()
    {
        base.Update();

        // Player detection and state transition
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= detectionRadius && currentEnemyState != EnemyStates.Following)
        {
            ChangeState(EnemyStates.Following);
        }
        else if (distanceToPlayer > detectionRadius && currentEnemyState != EnemyStates.Idle)
        {
            ChangeState(EnemyStates.Idle);
        }

        // Handle movement based on state
        if (currentEnemyState == EnemyStates.Following && !isRecoiling)
        {
            // Move towards the player's x position
            rb.velocity = new Vector2(speed * (facingRight ? 1 : -1), rb.velocity.y);

            // Check if the zombie needs to flip based on the player's position relative to the zombie
            if ((player.transform.position.x < transform.position.x && facingRight) ||
                (player.transform.position.x > transform.position.x && !facingRight))
            {
                Flip();
            }
        }
        else if (currentEnemyState == EnemyStates.Idle)
        {
            rb.velocity = Vector2.zero; // Zombie stops moving when idle
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = facingRight; // Flip the sprite to face the opposite direction
    }
}








