using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if the boss is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // Move and flip logic
        if (playerTransform != null)
        {
            MoveAndFlip();
        }
    }

    private void MoveAndFlip()
    {
        if (isGrounded)
        {
            Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
            Vector2 bossPosition = rb.position;
            float distanceFromPlayer = Vector2.Distance(playerPosition, bossPosition);

            if (distanceFromPlayer > attackRange)
            {
                Vector2 direction = (playerPosition - bossPosition).normalized;
                rb.velocity = direction * moveSpeed;
                FlipBasedOnDirection(direction);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FlipBasedOnDirection(Vector2 direction)
    {
        if (direction.x > 0 && !facingRight || direction.x < 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}



/*
  private void FollowPlayer()
{
    Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
    Vector2 bossPosition = new Vector2(transform.position.x, transform.position.y);

    float distanceFromPlayer = Vector2.Distance(playerPosition, bossPosition);

    if (distanceFromPlayer > attackRange)
    {
        Vector2 direction = (playerPosition - bossPosition).normalized;
        rb.velocity = direction * moveSpeed;

        // Flip the boss to face the player
        Flip(direction.x);
    }
    else
    {
        rb.velocity = Vector2.zero;
        // Ensure the boss is facing the player
        Flip(playerTransform.position.x);
    }
}
*/