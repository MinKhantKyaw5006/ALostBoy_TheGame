using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask; // Set this to the platform layer in the Inspector
    [SerializeField] private Transform groundCheck; // Assign the checkpoint GameObject in the Inspector

    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private float groundCheckDistance = 0.1f; // Distance to check for the ground, adjust as needed

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = isFacingLeft;
    }

    protected override void Update()
    {
        base.Update();

        Move();
        CheckForPlatformEdge();
    }

    private void Move()
    {
        // Always move left first when landing on the platform
        float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void CheckForPlatformEdge()
    {
        // Use the groundCheck to detect if the platform ends
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);

        // Debug the ground check
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        // If no ground detected, flip direction
        if (!isGroundAhead)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;

        // Ensure the sprite faces the correct direction when flipping.
        // flipX should be false when facing left because the sprite's initial direction is left.
        spriteRenderer.flipX = !isFacingLeft;

        // Update the ground check position to be in front of the sprite.
        // If the sprite is facing left, the ground check should be on its left side (negative local x).
        // If the sprite is facing right, the ground check should be on its right side (positive local x).
        groundCheck.localPosition = new Vector3(Mathf.Abs(groundCheck.localPosition.x) * (isFacingLeft ? -1 : 1), groundCheck.localPosition.y, groundCheck.localPosition.z);

        // Log the direction the enemy is facing for debugging purposes.
        Debug.Log(isFacingLeft ? "Now facing left." : "Now facing right.");
    }


    // ... Rest of the script remains unchanged
}