using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BossFight : Enemy
{
    public static BossFight Instance;
    private BossFight_Event bossFightEvent; // Reference to BossFight_Event
    [SerializeField] private float knockbackForce = 5f; // Adjust this value in the inspector
    [SerializeField] private float bossMoveDistance = 1f; // Distance to move the boss when colliding with the player

    //point at which wall check happens

    [SerializeField] GameObject slashEffect;
    public Transform sideAttack; //the middle of the side attack area
    public Vector2 sideAttackArea; //how large the area of side attack is

    public Transform upAttack; //the middle of the up attack area
    public Vector2 upAttackArea; //how large the area of side attack is

    public Transform downAttack; //the middle of the down attack area
    public Vector2 downAttackArea; //how large the area of down attack is

    public float attackRange;
    public float attackTimer;
    public LayerMask playerLayer; // Assign this in the inspector to the layer the player is on.


    [HideInInspector] public bool facingRight;

    // Serialized fields to set health thresholds in the Inspector
    [SerializeField] private float stage1HealthThreshold = 170f;
    [SerializeField] private float stage2HealthThreshold = 125f;
    [SerializeField] private float stage3HealthThreshold = 80f;

    [Header("Wall Check Settings")]
    public Transform wallCheckPoint;
    [SerializeField] private float wallCheckY = 0.2f;
    [SerializeField] private float wallChekcX = 0.5f;
    [SerializeField] private LayerMask whatIsWall; 

    [Header("Ground Check Settings:")]
    [SerializeField] public Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer

    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool damagedPlayer = false;
    [HideInInspector] public bool parrying;

    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject pillar;

    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireball;
    [HideInInspector] public bool outbreakAttack;

    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;


    private int hitCounter;
    private bool stunned, canStun;
    private bool alive;


    [HideInInspector] public float runSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
        bossFightEvent = GetComponent<BossFight_Event>(); // Get the BossFight_Event component
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.BossFight_Stage1);
        alive = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (stunned)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public bool TounchedWall()
    {
        if (Physics2D.Raycast(wallCheckPoint.position, Vector2.down, wallCheckY, whatIsWall)
            || Physics2D.Raycast(wallCheckPoint.position + new Vector3(wallChekcX, 0, 0), Vector2.down, wallCheckY, whatIsWall)
            || Physics2D.Raycast(wallCheckPoint.position + new Vector3(-wallChekcX, 0, 0), Vector2.down, whatIsWall))
        {
            Debug.Log("Enemy touched the wall"); 
            return true;
        }

        else 
        {
            Debug.Log("Enemy did not touched the wall"); 
            return false; 
        }
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            Debug.Log("Enemy is Grounded");
            return true;
        }
        else
        {
            Debug.Log("Enemy is not Grounded");
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttack.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttack.position, upAttackArea);
        Gizmos.DrawWireCube(downAttack.position, downAttackArea);
    }

    public void Flip()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
            Debug.Log("Enemy is not facing right");
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
            Debug.Log("Enemy is facing right");
        }
    }

    protected override void UpdateEnemyStates()
    {
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.BossFight_Stage1:
                canStun = true;
                attackTimer = 6;
                runSpeed = speed; // Normal speed
                break;

            case EnemyStates.BossFight_Stage2:
                canStun = true;
                attackTimer = 5; // Faster attacks
                runSpeed = speed; // Same speed
                break;

            case EnemyStates.BossFight_Stage3:
                canStun = true;
                attackTimer = 8; // Slightly slower, longer attacks
                break;

            default:
                Debug.LogError("Invalid boss state!");
                break;

        
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {

    }

    public void AttackHandler()
    {
        // Stage 1
        if (currentEnemyState == EnemyStates.BossFight_Stage1)
        {
            Debug.Log("Instantiating attacks in Boss Fight Stage 1");

            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
                //StartCoroutine(Outbreak());
                Debug.Log("Triple Slash Coroutine Started");

                //DiveAttackJump();
                //Debug.Log("Instantiate Dive Jump Attack");

                //OutbreakBendDown();
                //Debug.Log("outbreak attack");
            }
            else
            {
                //StartCoroutine(Outbreak());
                StartCoroutine(Lunge());
                Debug.Log("Lunge Coroutine Started");
                //StartCoroutine(Outbreak());

                //DiveAttackJump();
                //Debug.Log("Instantiate Dive Jump Attack");
                //OutbreakBendDown();
                //Debug.Log("outbreak attack");
            }
        }

        // Stage 2
        else if (currentEnemyState == EnemyStates.BossFight_Stage2)
        {
            Debug.Log("Instantiating attacks in Boss Fight Stage 2");

            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
                Debug.Log("Triple Slash Coroutine Started");
            }
            else
            {
                int _attackChoosen = Random.Range(1, 4);

                if (_attackChoosen == 1)
                {
                    StartCoroutine(Lunge());
                    Debug.Log("Lunge Coroutine Started");
                }
                else if (_attackChoosen == 2)
                {
                    DiveAttackJump();
                    Debug.Log("Instantiate Dive Jump Attack");
                }
                else if (_attackChoosen == 3)
                {
                    BarrageBendDown();
                    Debug.Log("Instantiate Barrage Benddown Attack");
                }
                else
                {
                    Debug.LogError("Error in choosing attack for Stage 2");
                }
            }
        }

        // Stage 3
        else if (currentEnemyState == EnemyStates.BossFight_Stage3)
        {
            Debug.Log("Instantiating attacks in Boss Fight Stage 3");

            int _attackChoosen = Random.Range(1, 5);

            if (_attackChoosen == 1)
            {
                //OutbreakBendDown();
                BarrageBendDown();
                Debug.Log("Instantiate Outbreak Benddown Attack");
            }
            else if (_attackChoosen == 2)
            {
                DiveAttackJump();
                Debug.Log("Instantiate Dive Jump Attack");
            }
            else if (_attackChoosen == 3)
            {
                BarrageBendDown();
                Debug.Log("Instantiate Barrage Benddown Attack");
            }
            else if (_attackChoosen == 4)
            {
                StartCoroutine(TripleSlash());
                Debug.Log("Triple Slash Coroutine Started");
            }
            else
            {
                Debug.LogError("Error in choosing attack for Stage 3");
            }
        }

        // Error handling if no state matches
        else
        {
            Debug.LogError("Can not instantiate any of the attacks.");
        }
    }



    public void ResetAttacks()
    {
        attacking = false;

        StopCoroutine(TripleSlash());
        StopCoroutine(Lunge());
        StopCoroutine(Parry());
        StopCoroutine(Slash());
        Debug.LogWarning("All coroutine has been stopped.");

        diveAttack = false;
        barrageAttack = false;
        Debug.LogWarning("All attack has been reset");
    }

    /*
    private IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        // First Slash
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.3f);
        ApplyDamageToPlayer(); // Check and apply damage
        anim.ResetTrigger("Slash");

        // Second Slash
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.5f);
        ApplyDamageToPlayer(); // Check and apply damage
        anim.ResetTrigger("Slash");

        // Third Slash
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        ApplyDamageToPlayer(); // Check and apply damage
        anim.ResetTrigger("Slash");

        ResetAttacks();
    }
    */
    private IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        // First Slash
        anim.SetTrigger("Slash");
        SlashAngle(); // Assuming this method sets the correct attack angle/position
        yield return new WaitForSeconds(0.3f);
        bossFightEvent.Hit(sideAttack, sideAttackArea); // Call Hit method from BossFight_Event
        anim.ResetTrigger("Slash");

        // Second Slash
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.5f);
        bossFightEvent.Hit(upAttack, upAttackArea);
        anim.ResetTrigger("Slash");

        // Third Slash
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        bossFightEvent.Hit(downAttack, downAttackArea);
        anim.ResetTrigger("Slash");

        ResetAttacks();
    }



    private void SlashAngle()
    {
        if (PlayerController.Instance.transform.position.x - transform.position.x != 0)
        {
            Instantiate(slashEffect, sideAttack);
        }

        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, 80, upAttack);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, -90, upAttack);
        }
    }

    private void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    private IEnumerator Lunge()
    {
        Flip();
        attacking = true;

        anim.SetBool("Lunge", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Lunge", false);
        damagedPlayer = false;
        ResetAttacks();
    }

    private IEnumerator Parry()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("Parry", true);
        yield return new WaitForSeconds(0.8f);
       
        anim.SetBool("Parry", false);

        parrying = false;
        ResetAttacks();
    }

    private IEnumerator Slash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        bossFightEvent.Hit(sideAttack, sideAttackArea);
        anim.ResetTrigger("Slash");

        ResetAttacks();
    }

    //public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    //{
    //    if (!stunned)
    //    {
    //        if (!parrying)
    //        {
    //            if (canStun)
    //            {
    //                hitCounter++;
    //                if(hitCounter >= 3)
    //                {
    //                    ResetAttacks();
    //                    StartCoroutine(Stunned());
    //                }
    //            }

    //            ResetAttacks();
    //            base.EnemyHit(_damageDone, _hitDirection, _hitForce);

    //            if(currentEnemyState != EnemyStates.BossFight_Stage3)
    //            {
    //                ResetAttacks() ;
    //                StartCoroutine(Parry());
    //            }
    //        }

    //        else
    //        {
    //            StopCoroutine(Parry());
    //            parrying = false;
    //            ResetAttacks();
    //            StartCoroutine(Slash());
    //        }

    //    }
    //    else
    //    {
    //        StopCoroutine(Stunned());
    //        anim.SetBool("Stunned", false);
    //        stunned = false;
    //    }
    //    CheckHealth();
    //}

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (!stunned)
        {
            if (!parrying)
            {
                if (canStun)
                {
                    hitCounter++;
                    if (hitCounter >= 3)
                    {
                        ResetAttacks();
                        StartCoroutine(Stunned());
                    }
                }

                ResetAttacks();

                // Cap the damage at the maxDamagePerHit
                float finalDamage = Mathf.Min(_damageDone, maxDamagePerHit); // Assuming maxDamagePerHit is defined in the parent class
                health -= finalDamage; // Subtract the capped damage from current health

                rb.AddForce(-_hitForce * recoilFactor * _hitDirection);

                // Trigger screen shake
                ScreenShaker screenShaker = FindObjectOfType<ScreenShaker>();
                if (screenShaker != null)
                {
                    screenShaker.Shake(_hitDirection.normalized);
                }
                else
                {
                    Debug.LogWarning("ScreenShaker not found in the scene.");
                }

                // Check health after taking damage
                CheckHealth();

                if (currentEnemyState != EnemyStates.BossFight_Stage3)
                {
                    ResetAttacks();
                    StartCoroutine(Parry());
                }
            }
            else
            {
                StopCoroutine(Parry());
                parrying = false;
                ResetAttacks();
                StartCoroutine(Slash());
            }
        }
        else
        {
            StopCoroutine(Stunned());
            anim.SetBool("Stunned", false);
            stunned = false;
        }

        CheckHealth();
    }


    protected override void CheckHealth()
    {
        if (health > stage1HealthThreshold)
        {
            ChangeState(EnemyStates.BossFight_Stage1);
        }
        else if (health <= stage2HealthThreshold && health > stage3HealthThreshold)
        {
            ChangeState(EnemyStates.BossFight_Stage2);
        }
        else if (health <= stage3HealthThreshold && health > 0)
        {
            ChangeState(EnemyStates.BossFight_Stage3);
        }
        else if (health <= 10 && alive)
        {
            Death(0);
        }
        else
        {
            Debug.LogError("Cannot check for enemy health");
        }
    }
    private void DiveAttackJump()
    {
        attacking = true;
        moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
        diveAttack = true;
        anim.SetBool("Jump", true);
    }

    public void Dive()
    {
        anim.SetBool("Dive", true);
        anim.SetBool("Jump", false);
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage * 2);
            PlayerController.Instance.pState.recoilingX = true;
        }
    }
    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the attack is a dive attack or bounce attack
        if (diveAttack || bounceAttack)
        {
            // Check if the collided object is on the Player layer
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                // Get the PlayerController component
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if (player != null)
                {
                    // Apply damage to the player
                    player.TakeDamage(BossFight.Instance.damage);
                    PlayerController.Instance.pState.recoilingX = true;
                    Debug.Log("Player taking damage from dive/bounce attack!");

                    // Calculate knockback direction and apply knockback to the player
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized; // Direction to move the player
                    Vector2 knockback = knockbackDirection * knockbackForce; // Scale by knockback force value
                    player.ApplyKnockback(knockback); // Apply knockback

                    // Move the boss away (optional)
                    Vector2 bossKnockbackDirection = (transform.position - collision.transform.position).normalized; // Direction away from player
                    Vector2 newPosition = (Vector2)transform.position + (bossKnockbackDirection * bossMoveDistance);
                    transform.position = newPosition;
                    //transform.position = newPosition; // Move the boss to the new position

                    // Optional: Add some vertical offset if needed
                    transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f); // Adjust Y position

                }
                else
                {
                    Debug.LogWarning("Collider detected does not have a PlayerController component!");
                }
            }
            else
            {
                Debug.Log($"Collider detected with tag: {collision.gameObject.tag}. Not a Player.");
            }
        }
        else
        {
            Debug.Log("Attack is not a dive or bounce attack. No damage will be dealt.");
        }
    }



    public void DivingPillars()
    {
        Vector2 _impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5f;

        for (int i = 0; i < 30; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));

            _spawnDistance += 7f;
        }

        ResetAttacks();
    }

    private void BarrageBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;
        barrageAttack = true;
        anim.SetTrigger("BendDown");
    }

    //public IEnumerator Barrage()
    //{
    //    rb.velocity = Vector2.zero;

    //    float _currentAngle = 10f;
    //    for (int i = 0; i < 50; i++)
    //    {
    //        GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));

    //        if (facingRight)
    //        {
    //            _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle); ;
    //        }

    //        else
    //        {
    //            _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
    //        }

    //        _currentAngle += 5f;
    //        yield return new WaitForSeconds(0.15f);
    //    }

    //    yield return new WaitForSeconds(0.1f);
    //    anim.SetBool("Cast", false);
    //    ResetAttacks();
    //}

    public IEnumerator Barrage()
    {
        rb.velocity = Vector2.zero;

        float _currentAngle = 0f; // Start angle
        bool increasing = true; // Track whether we're increasing or decreasing the angle
        int totalProjectiles = 50; // Number of fireballs
        float maxAngle = 90f; // Maximum angle (0 to 90 degrees)

        for (int i = 0; i < totalProjectiles; i++)
        {
            // Instantiate the projectile at the current angle
            GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));

            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
            }

            // Alternate the angle back and forth between 0 and 90 degrees
            if (increasing)
            {
                _currentAngle += 10f; // Adjust this value for how much you want to increment per fireball
                if (_currentAngle >= maxAngle)
                {
                    _currentAngle = maxAngle;
                    increasing = false; // Start decreasing after hitting max
                }
            }
            else
            {
                _currentAngle -= 10f; // Adjust this value for how much you want to decrement per fireball
                if (_currentAngle <= 0f)
                {
                    _currentAngle = 0f;
                    increasing = true; // Start increasing again after hitting min
                }
            }

            yield return new WaitForSeconds(0.1f); // Time between fireballs
        }

        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAttacks();
    }


    //private void OutbreakBendDown()
    //{
    //    attacking = true;
    //    rb.velocity = Vector2.zero;

    //    // Disable the boss's collider to prevent unwanted collisions during the attack
    //    Collider2D bossCollider = GetComponent<Collider2D>();
    //    bossCollider.enabled = false;  // Disable the collider

    //    // Freeze rotation to prevent unwanted rotation during the attack
    //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    //    moveToPosition = new Vector2(transform.position.x, rb.position.y + 7);
    //    outbreakAttack = true;
    //    anim.SetTrigger("BendDown");
    //}

    //public IEnumerator Outbreak()
    //{
    //    // Disable the boss's collider to prevent unwanted collisions during the attack
    //    Collider2D bossCollider = GetComponent<Collider2D>();
    //    //bossCollider.enabled = false;  // Disable the collider

    //    yield return new WaitForSeconds(1f);
    //    anim.SetBool("Cast", true);

    //    rb.velocity = Vector2.zero;

    //    // Define the number of directions and the rotation speed
    //    int numDirections = 8;
    //    float rotationSpeed = 45f; // degrees per second
    //    float angleStep = 360f / numDirections; // angle between each fireball direction

    //    // Cast fireballs in a rotating manner
    //    for (int i = 0; i < 30; i++)
    //    {
    //        // Calculate the current angle for rotation
    //        float currentAngle = (i * rotationSpeed * Time.deltaTime) % 360;

    //        // Cast fireballs at each direction
    //        for (int j = 0; j < numDirections; j++)
    //        {
    //            // Calculate the final angle for the current direction
    //            float angle = currentAngle + (j * angleStep);
    //            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, angle));
    //        }

    //        yield return new WaitForSeconds(0.2f);
    //    }

    //    //for (int i = 0; i < 30; i++)
    //    //{
    //    //    Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130)));
    //    //    Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70)));
    //    //    Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280)));

    //    //    yield return new WaitForSeconds(0.2f);
    //    //}

    //    // Small pause before dropping down
    //    yield return new WaitForSeconds(0.1f);

    //    // Apply downward velocity to simulate falling
    //    rb.velocity = new Vector2(rb.velocity.x, -10);

    //    // Unfreeze the Rigidbody constraints to allow falling
    //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // Only freeze rotation, not position

    //    // Ensure gravity is enabled
    //    rb.gravityScale = 1f;  // Ensure gravity is enabled


    //    // Wait a short time during the fall
    //    yield return new WaitForSeconds(0.1f);

    //    // Unset casting state after the fall
    //    anim.SetBool("Cast", false);

    //    // Re-enable the boss's collider after the attack is finished
    //    bossCollider.enabled = true;  // Re-enable the collider

    //    // Reset attack state
    //    ResetAttacks();
    //}


    private void OutbreakBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        // Apply upward force to make the boss jump
        rb.velocity = new Vector2(rb.velocity.x, 10f); // Apply an upward force (adjust the value if needed)

        // Temporarily disable gravity and freeze vertical movement after the jump
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        moveToPosition = new Vector2(transform.position.x, rb.position.y + 7);
        outbreakAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Outbreak()
    {
        // Wait for the boss to jump into the air
        yield return new WaitForSeconds(0.5f);  // Adjust the time for how long it takes to jump

        anim.SetBool("Cast", true);

        rb.velocity = Vector2.zero;

        // Define the number of directions and the rotation speed
        int numDirections = 8;
        float rotationSpeed = 45f; // degrees per second
        float angleStep = 360f / numDirections; // angle between each fireball direction

        // Cast fireballs in a rotating manner
        for (int i = 0; i < 30; i++)
        {
            // Calculate the current angle for rotation
            float currentAngle = (i * rotationSpeed * Time.deltaTime) % 360;

            // Cast fireballs at each direction
            for (int j = 0; j < numDirections; j++)
            {
                // Calculate the final angle for the current direction
                float angle = currentAngle + (j * angleStep);
                Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, angle));
            }

            yield return new WaitForSeconds(0.2f);
        }

        // Small pause before dropping down
        yield return new WaitForSeconds(0.1f);

        // Apply downward velocity to simulate falling
        rb.velocity = new Vector2(rb.velocity.x, -10);

        // Re-enable gravity and unfreeze Y position to allow falling
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // Only freeze rotation, not position

        // Wait a short time during the fall
        yield return new WaitForSeconds(0.1f);

        // Unset casting state after the fall
        anim.SetBool("Cast", false);

        // Reset attack state
        ResetAttacks();
    }



    private void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 5);
        BounceBendDown();
    }

    int _bounces = 0;
    public void CheckBounce()
    {
        if (_bounces < bounceCount - 1)
        {
            _bounces++;
            BounceBendDown();
        }

        else
        {
            _bounces = 0;
            anim.Play("Boss_Run");
        }
    }

    public void BounceBendDown()
    {
        rb.velocity = Vector2.zero;
        moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
        bounceAttack = true;
        anim.SetTrigger("BendDown");
    }

    public void CalculateTargetAngle()
    {
        Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;

        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionToTarget = _angleOfTarget;
    }

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        anim.SetBool("Stunned", true);
        yield return new WaitForSeconds(6f);
        anim.SetBool("Stunned", false);
        stunned = false;
    }

    protected override void Death(float _destroyTime)
    {
        ResetAttacks();
        alive = false;
        rb.velocity = new Vector2(rb.velocity.x, -25);
        anim.SetTrigger("Die");
    }

    public void DestroyAfterDeath()
    {
        // Find the child object (the light) by name or tag
        Transform lightObject = transform.Find("Light 2D"); // Replace "LightObjectName" with the actual name of your child object

        if (lightObject != null)
        {
            Destroy(lightObject.gameObject); // Destroy the child light object
        }
        else
        {
            Debug.LogWarning("Light object not found!");
        }

        Destroy(gameObject); // Destroy the boss object
    }
}
