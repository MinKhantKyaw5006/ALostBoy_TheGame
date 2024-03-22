using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform heightCheck; // Ensure you assign this in the Inspector
    [SerializeField] private Transform platformCheck; // Assign this in the Inspector
    [SerializeField] private float forwardCheckDistance = 1f; // Adjust this value based on your needs
    [SerializeField] private float maxJumpHeight = 1f; // Adjust this value as needed
    [SerializeField] private DamageFlash damageFlash;


    [SerializeField] private Animator animator;





    private bool isFacingLeft = true;
    private SpriteRenderer spriteRenderer;
    private float groundCheckDistance = 0.1f;


    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the sprite to face left initially if that's the initial direction.
        spriteRenderer.flipX = !isFacingLeft;
        UpdateCheckPositions();

        rb = GetComponent<Rigidbody2D>();
        // Initialize the animator reference
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        Move();
        CheckForPlatformEdge();

        if (IsGrounded())
        {
            Move();
            CheckForPlatformEdge();
            

        }
    }

    private void Move()
    {
        float moveSpeed = isFacingLeft ? -patrolSpeed : patrolSpeed;
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }







    private void CheckForPlatformEdge()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, platformLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
        if (!isGroundAhead)
        {
            Flip();
        }
    }

    private bool IsGrounded()
    {
        float checkRadius = 0.2f; // Adjust as necessary for your game
        Collider2D[] colliders = Physics2D.OverlapCircleAll(platformCheck.position, checkRadius, platformLayerMask);
        return colliders.Length > 0;
    }


    private void Flip()
    {
        if (!IsGrounded()) return; // Prevent flipping in mid-air

        isFacingLeft = !isFacingLeft;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        UpdateCheckPositions();

        //Debug.Log(isFacingLeft ? "Now facing left." : "Now facing right.");
    }

    private void UpdateCheckPositions()
    {
        // Update the local position of the groundCheck and heightCheck to be in front of the sprite
        float localXPosition = Mathf.Abs(groundCheck.localPosition.x) * (isFacingLeft ? -1 : 1);
        groundCheck.localPosition = new Vector3(localXPosition, groundCheck.localPosition.y, groundCheck.localPosition.z);
        heightCheck.localPosition = new Vector3(localXPosition, heightCheck.localPosition.y, heightCheck.localPosition.z);
    }
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        // Call the base version to ensure base functionality is maintained
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        // Trigger the squash animation
        if (animator != null)
        {
            animator.SetTrigger("Squash");
        }

        // Trigger the damage flash effect
        if (damageFlash != null)
        {
            damageFlash.Flash();
        }
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








