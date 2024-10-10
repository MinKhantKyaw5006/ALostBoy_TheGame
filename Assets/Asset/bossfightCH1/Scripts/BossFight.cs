using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BossFight : Enemy
{
    public static BossFight Instance;

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

    [HideInInspector] public bool facingRight;

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
                runSpeed = speed; 
                break;

            case EnemyStates.BossFight_Stage2:
                canStun = true;
                attackTimer = 5;
                runSpeed = speed; 
                break;

            case EnemyStates.BossFight_Stage3:
                canStun = true;
                attackTimer = 8; 
                break;

            case EnemyStates.BossFight_Stage4:
                canStun = false;
                attackTimer = 10;
                runSpeed = speed / 2; 
                break;

            default:
                break;
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {

    }

    public void AttackHandler()
    {
        if (currentEnemyState == EnemyStates.BossFight_Stage1)
        {
            Debug.Log("Instantiating attacks in Boss Fight Stage 1");
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
                Debug.Log("Instantiate Bounce Attack");
            }

            else
            {
 
               
                StartCoroutine(Lunge());
                Debug.Log("Lunge Coroutine Started"); 
              
            }
        }

        
    if(currentEnemyState == EnemyStates.BossFight_Stage2)
    {
        Debug.Log("Instantiating attacks in Boss Fight Stage 2");
        if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
        {
            StartCoroutine(TripleSlash());
            Debug.Log("Triple Slash Coroutine Started");
        }

        else
        {
            int _attackChoosen = Random.Range(1, 3);
            if(_attackChoosen == 1)
            {
                StartCoroutine(Lunge());
                Debug.Log("Lunge Coroutine Started"); 
            }

            else if(_attackChoosen == 2)
            {
                DiveAttackJump();
                Debug.Log("Instantiate Dive Jump Attack"); 
            }

            else if(_attackChoosen == 3)
            {
                BarrageBendDown();
                Debug.Log("Instantiate Barrage Benddown Attack");
            }

            else
            {
                Debug.LogError("Can not instantiate any attack in Boss Fight Stage 2");
            }
        }
    }

    if (currentEnemyState == EnemyStates.BossFight_Stage3)
    {
        Debug.Log("Instantiating attacks in Boss Fight Stage 3"); 
        int _attackChoosen = Random.Range(1, 4);


        if(_attackChoosen == 1)
        {
            OutbreakBendDown();
            Debug.Log("Instatiate Outbreak Benddown Attack"); 
        }

        else if(_attackChoosen == 2)
        {
            DiveAttackJump();
            Debug.Log("Instatiate Dive Jump Attack");

        }

        else if(_attackChoosen == 3)
        {
            BarrageBendDown();
            Debug.Log("Instantiate Barrage Benddown Attack");
        }

        else if(_attackChoosen == 4)
        {
            BounceAttack();
            Debug.Log("Instantiate Bounce Attack");
        }

        else
        {
            Debug.LogError("Can not instantiate any attack in Boss Fight Stage 3"); 
        }
    }

    else if (currentEnemyState == Enemy.EnemyStates.BossFight_Stage4)
    {
        Debug.Log("Instantiating attacks in Boss Fight Stage 4");
        if(Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
        {
            StartCoroutine(Slash() );
            Debug.Log("Slash Coroutine Started");
        }

        else 
        {
            BounceAttack();
            Debug.Log("Instantiate Bounce Atttack");
        }
    }

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

    private IEnumerator TripleSlash()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
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
        anim.ResetTrigger("Slash");

        ResetAttacks();
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (!stunned)
        {
            if (!parrying)
            {
                if (canStun)
                {
                    hitCounter++;
                    if(hitCounter >= 3)
                    {
                        ResetAttacks();
                        StartCoroutine(Stunned());
                    }
                }

                ResetAttacks();
                base.EnemyHit(_damageDone, _hitDirection, _hitForce);

                if(currentEnemyState != EnemyStates.BossFight_Stage4)
                {
                    ResetAttacks() ;
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
        if (health > 170)
        {
            ChangeState(EnemyStates.BossFight_Stage1);
        }
        else if (health <= 125 && health > 80)
        {
            ChangeState(EnemyStates.BossFight_Stage2);
        }
        else if (health <= 80 && health > 40)
        {
            ChangeState(EnemyStates.BossFight_Stage3);
        }
        else if (health <= 40 && health > 0) 
        {
            ChangeState(EnemyStates.BossFight_Stage4);
        }
        else if (health <= 0 && alive) 
        {
            Death(0);
        }
        else
        {
            Debug.LogError("Can not check for enemy health");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage * 2);
            PlayerController.Instance.pState.recoilingX = true;
        }
    }

    public void DivingPillars()
    {
        Vector2 _impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5f;

        for (int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));

            _spawnDistance += 5f;
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

    public IEnumerator Barrage()
    {
        rb.velocity = Vector2.zero;

        float _currentAngle = 30f;
        for (int i = 0; i < 10; i++)
        {
            GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));

            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle); ;
            }

            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
            }

            _currentAngle += 5f;
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAttacks();
    }

    private void OutbreakBendDown()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
        outbreakAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Outbreak()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Cast", true);

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 30; i++)
        {
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130)));
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70)));
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280)));

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(rb.velocity.x, -10);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
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
        Destroy(gameObject);
    }
}
