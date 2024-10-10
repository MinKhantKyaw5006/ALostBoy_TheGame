using System.Collections;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform heightCheck;
    [SerializeField] private Transform platformCheck;
    [SerializeField] private float forwardCheckDistance = 1f;
    [SerializeField] private float maxJumpHeight = 1f;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private Animator animator;

    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private float groundCheckDistance = 0.1f;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the sprite to face left initially if that's the initial direction.
        spriteRenderer.flipX = !isFacingLeft;
        UpdateCheckPositions();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ensure the animator is assigned
    }

    protected override void Update()
    {
        base.Update();

        // Move and check for platform edge only when grounded
        if (IsGrounded())
        {
            Move();
            CheckForPlatformEdge();
        }
        else
        {
            // Stop moving if not grounded
            rb.velocity = Vector2.zero;
        }
    }

    private void Move()
    {
        float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void CheckForPlatformEdge()
    {
        // Cast a ray downward to check for ground ahead
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        // If no ground ahead, flip direction
        if (!isGroundAhead)
        {
            Debug.Log("No ground ahead, flipping direction.");
            Flip();
        }
    }

    private bool IsGrounded()
    {
        // Check for ground beneath the enemy
        float checkRadius = 0.2f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(platformCheck.position, checkRadius, platformLayerMask);
        bool isGrounded = colliders.Length > 0;
        Debug.Log(isGrounded ? "Enemy is grounded." : "Enemy is not grounded.");
        return isGrounded;
    }

    private void Flip()
    {
        if (!IsGrounded()) return; // Don't flip if in the air

        isFacingLeft = !isFacingLeft;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        UpdateCheckPositions();
        Debug.Log(isFacingLeft ? "Now facing left." : "Now facing right.");
    }

    private void UpdateCheckPositions()
    {
        // Adjust groundCheck and heightCheck positions based on facing direction
        float localXPosition = Mathf.Abs(groundCheck.localPosition.x) * (isFacingLeft ? -1 : 1);
        groundCheck.localPosition = new Vector3(localXPosition, groundCheck.localPosition.y, groundCheck.localPosition.z);
        heightCheck.localPosition = new Vector3(localXPosition, heightCheck.localPosition.y, heightCheck.localPosition.z);
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        // Ensure the enemy's health does not go below zero
        health -= _damageDone;
        health = Mathf.Max(health, 0); // Clamp health to 0

        Debug.Log($"Enemy took {_damageDone} damage. Current health: {health}");

        // Trigger squash animation and damage flash effect
        if (animator != null)
        {
            animator.SetTrigger("Squash");
        }

        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        // Check if the enemy's health has reached zero
        if (health <= 0)
        {
            Debug.Log("Enemy health is 0. Destroying the enemy.");
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        // Any destruction effects, such as playing a death animation, can be added here.
        Destroy(gameObject);
        Debug.Log("Enemy destroyed.");
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float maxJumpHeight = 1f;
    [SerializeField] private float forwardCheckDistance = 1f;
    [SerializeField] private float jumpForce = 300f; // Adjusted for jump height
    [SerializeField] private float jumpForwardSpeed = 2f; // Control forward movement during jump
    [SerializeField] private float groundCheckDistance = 0.1f; // Distance to check for the ground

    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isJumping = false;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.flipX = isFacingLeft;
    }

    protected override void Update()
    {
        base.Update();

        if (!isJumping)
        {
            Move();
            CheckForPlatformEdge();
            CheckForHeightChange();
        }
    }

    private void Move()
    {
        if (!isJumping)
        {
            float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    private void CheckForPlatformEdge()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (!isGroundAhead && !isJumping)
        {
            Flip();
        }
    }

    private void CheckForHeightChange()
    {
        Vector2 forwardDirection = isFacingLeft ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, forwardDirection, forwardCheckDistance, platformLayerMask);

        if (hit.collider != null && !isJumping)
        {
            float heightDifference = hit.point.y - groundCheck.position.y;
            if (heightDifference <= maxJumpHeight)
            {
                StartCoroutine(JumpAndMoveForward());
            }
        }
    }

    private IEnumerator JumpAndMoveForward()
    {
        isJumping = true;

        // Apply jump force
        rb.AddForce(Vector2.up * jumpForce);

        // Apply forward movement for a duration
        float duration = 0.5f; // Adjust this duration based on the needs of your game
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            rb.velocity = new Vector2(jumpForwardSpeed * (isFacingLeft ? -1 : 1), rb.velocity.y);
            yield return null;
        }

        isJumping = false;
    }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        spriteRenderer.flipX = !isFacingLeft;
        groundCheck.localPosition = new Vector3(-groundCheck.localPosition.x, groundCheck.localPosition.y, groundCheck.localPosition.z);
    }
}
*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float maxJumpHeight = 1f;
    [SerializeField] private float forwardCheckDistance = 1f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private float jumpForwardSpeed = 2f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool allowFlip = true;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.flipX = isFacingLeft;
        Debug.Log("PatrolEnemy started facing " + (isFacingLeft ? "left" : "right"));
    }

    protected override void Update()
    {
        base.Update();

        if (!isJumping)
        {
            Move();
            if (allowFlip)
            {
                CheckForPlatformEdge();
                CheckForHeightChange();
            }
        }
        if (isJumping && IsGrounded())
        {
            Debug.Log("Landed, allowing flip.");
            allowFlip = true;
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        bool grounded = hit.collider != null;
        Debug.Log("IsGrounded: " + grounded);
        return grounded;
    }

    private void Move()
    {
        if (!isJumping)
        {
            float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            Debug.Log("Moving " + (isFacingLeft ? "left" : "right"));
        }
    }

    private void CheckForPlatformEdge()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        if (!isGroundAhead && !isJumping)
        {
            Debug.Log("No ground ahead, flipping.");
            Flip();
        }
    }

    private void CheckForHeightChange()
    {
        if (!isJumping)
        {
            Vector2 forwardDirection = isFacingLeft ? Vector2.left : Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, forwardDirection, forwardCheckDistance, platformLayerMask);

            if (hit.collider != null)
            {
                float heightDifference = hit.point.y - groundCheck.position.y;
                if (heightDifference <= maxJumpHeight)
                {
                    Debug.Log("Height change detected, preparing to jump.");
                    StartCoroutine(JumpAndMoveForward());
                    allowFlip = false;
                }
            }
        }
    }

    private IEnumerator JumpAndMoveForward()
    {
        Debug.Log("Jumping and moving forward.");
        isJumping = true;
        rb.AddForce(Vector2.up * jumpForce);
        float endTime = Time.time + 0.5f;
        while (Time.time < endTime)
        {
            rb.velocity = new Vector2(jumpForwardSpeed * (isFacingLeft ? -1 : 1), rb.velocity.y);
            yield return null;
        }
    }

    private void Flip()
    {
        if (allowFlip)
        {
            isFacingLeft = !isFacingLeft;
            spriteRenderer.flipX = !isFacingLeft;
            groundCheck.localPosition = new Vector3(-groundCheck.localPosition.x, groundCheck.localPosition.y, groundCheck.localPosition.z);
            Debug.Log("Flipped, now facing " + (isFacingLeft ? "left" : "right"));
        }
    }
}
*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    // Serialized fields remain unchanged...
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float maxJumpHeight = 0.95f;
    [SerializeField] private float forwardCheckDistance = 1f;
    [SerializeField] private float jumpForce = 300f; // Adjusted for jump height
    [SerializeField] private float jumpForwardSpeed = 2f; // Control forward movement during jump
    [SerializeField] private float groundCheckDistance = 0.1f; // Distance to check for the ground

    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool allowFlip = true; // Add a flag to control when flipping is allowed

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.flipX = isFacingLeft;
    }

    protected override void Update()
    {
        base.Update();

        if (!isJumping)
        {
            Move();
            if (allowFlip) // Only check for platform edge and height change if flipping is allowed
            {
                CheckForPlatformEdge();
                CheckForHeightChange();
            }
        }
        // After jumping, ensure the enemy is grounded before allowing flipping again
        if (isJumping && IsGrounded())
        {
            allowFlip = true;
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        // Check if the enemy is touching the ground
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        return hit.collider != null;
    }

    // Move method remains unchanged...
    private void Move()
    {
        if (!isJumping)
        {
            float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    // CheckForPlatformEdge method remains unchanged...
    private void CheckForPlatformEdge()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (!isGroundAhead && !isJumping)
        {
            Flip();
        }
    }

    private void CheckForHeightChange()
    {
        if (!isJumping) // Only execute if not currently jumping
        {
            Vector2 forwardDirection = isFacingLeft ? Vector2.left : Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, forwardDirection, forwardCheckDistance, platformLayerMask);

            if (hit.collider != null)
            {
                float heightDifference = hit.point.y - groundCheck.position.y;
                if (heightDifference <= maxJumpHeight)
                {
                    StartCoroutine(JumpAndMoveForward());
                    allowFlip = false; // Disable flipping when starting the jump
                }
            }
        }
    }

    private IEnumerator JumpAndMoveForward()
    {
        isJumping = true;

        // Apply jump force and forward movement
        rb.AddForce(Vector2.up * jumpForce);
        // Keep moving forward for the duration of the jump
        float duration = 0.5f; // This duration can be adjusted
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            rb.velocity = new Vector2(jumpForwardSpeed * (isFacingLeft ? -1 : 1), rb.velocity.y);
            yield return null;
        }
        // allowFlip will be set back to true once the enemy is grounded, in the Update method
    }

    private void Flip()
    {
        if (allowFlip) // Only flip if allowed
        {
            isFacingLeft = !isFacingLeft;
            spriteRenderer.flipX = !isFacingLeft;
            groundCheck.localPosition = new Vector3(-groundCheck.localPosition.x, groundCheck.localPosition.y, groundCheck.localPosition.z);
        }
    }
}

*/








