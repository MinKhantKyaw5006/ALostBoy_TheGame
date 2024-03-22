using UnityEngine;

public class BatFollow : Enemy
{
    [SerializeField] private float lineOfSight = 5f;
    [SerializeField] private float moveSpeed = 2f;
    // Size for the trigger collider that detects the player above
    [SerializeField] private float aboveDetectionWidth = 2f;
    [SerializeField] private float aboveDetectionHeight = 1f;
    [SerializeField] private float enemyScale;
    [SerializeField] private Animator animator;
    [SerializeField] private DamageFlash damageFlash;
    private SpriteRenderer spriteRenderer;

    private Transform player;
    // References to the enemy's colliders
    private BoxCollider2D mainCollider;
    private BoxCollider2D aboveDetectionCollider;


    [SerializeField] private float recoilDistance = 1f; // How far the enemy recoils
    [SerializeField] private float recoilSpeed = 5f; // Speed of the recoil
    private bool isRecoiling = false;
    private Vector2 recoilDirection;
    private float recoilStartTime;


    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Initialize the animator reference
        animator = GetComponent<Animator>();
        // Assume the main Collider is the first BoxCollider2D component
        mainCollider = GetComponent<BoxCollider2D>();
        // Add or get the second BoxCollider2D component for detecting the player above
        aboveDetectionCollider = gameObject.AddComponent<BoxCollider2D>();
        aboveDetectionCollider.isTrigger = true;
        aboveDetectionCollider.size = new Vector2(aboveDetectionWidth, aboveDetectionHeight);
        aboveDetectionCollider.offset = new Vector2(0, aboveDetectionHeight / 2); // Adjust based on enemy size
    }

    protected override void Update()
    {
        base.Update();
        FollowPlayer();

        if (!isRecoiling && player != null)
        {
            FollowPlayer();
            FlipTowardsPlayer();
        }

        if (isRecoiling)
        {
            float recoilDuration = 0.2f; // Duration of the recoil effect in seconds
            if (Time.time - recoilStartTime < recoilDuration)
            {
                // Move the enemy in the recoil direction
                transform.position += (Vector3)recoilDirection * recoilSpeed * Time.deltaTime;
            }
            else
            {
                // Stop recoiling after the duration ends
                isRecoiling = false;
            }
        }

    }

    private void FollowPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < lineOfSight)
        {
            Vector2 targetPosition = new Vector2(player.position.x, player.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
    private void FlipTowardsPlayer()
    {
        // Flipping based on the player's position
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    private void FlipObject(bool shouldFlip)
    {
        float newScaleX = shouldFlip ? Mathf.Abs(enemyScale) : -Mathf.Abs(enemyScale);
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Optionally disable the main collider when the player is directly above
             mainCollider.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Optionally re-enable the main collider when the player exits the detection area
             mainCollider.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        // Line of sight representation
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);

        // Above detection area representation
        Gizmos.color = Color.red;
        Vector3 aboveAreaPosition = transform.position + new Vector3(0, aboveDetectionHeight / 2, 0);
        Vector3 aboveAreaSize = new Vector3(aboveDetectionWidth, aboveDetectionHeight, 1);
        Gizmos.DrawWireCube(aboveAreaPosition, aboveAreaSize);
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce); // Call base method if it exists

        // Initiate recoil
        isRecoiling = true;
        recoilDirection = -_hitDirection.normalized; // Recoil in the opposite direction of the hit
        recoilStartTime = Time.time;

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
