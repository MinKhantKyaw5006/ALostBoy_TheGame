using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; // Add this line
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Add this line for scene management

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    private SpriteRenderer sr;
    Animator anim;
    public static PlayerController Instance;
    private bool canDash = true;
    private bool Dashed;
    private float gravity;
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;
    int stepXRecoiled, stepYRecoiled;

    //camera zoomout
    public delegate void PlayerJumpDelegate(int jumpCount);
    public event PlayerJumpDelegate OnPlayerJump;
    public event Action OnPlayerLanded;

    [Header("Health Setting")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;
    float healTimer;
    [SerializeField] float timetoHeal;
    [Space(5)]

    [Header("Mana Setting")]
    [SerializeField] Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [Space(5)]

    [Header("Spell Setting")]
    //spell stats       
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;

    [SerializeField] float spellDamage; //upspellexplosion downspellfireball    
    [SerializeField] float downSpellForce; //dive desolate
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    float timeSinceCast;
    float castOrHealTimer;
    [Space(5)]

    [Header("Horizontal Movement Setting")]
    [SerializeField] private float walkspeed = 1;
    [Space(5)]

    [Header("Vertical Movement Setting")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpbufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int MaxAirJumps;
    [Space(5)]

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask WhatIsGround;
    [Space(5)]

    [Header("Dash Variable Setting")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Attack Setting")]
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Transform UpAttackTransform;
    [SerializeField] Transform DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil Setting")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;

    public HeartController heartControllerInstance;



    // Start is called before the first frame update
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

        DontDestroyOnLoad(gameObject);

        // Checks if the current instance of the player should be destroyed
        CheckForDuplicatePlayerInstances();


    }
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        sr = GetComponent<SpriteRenderer>();
        Mana = mana;
        manaStorage.fillAmount = mana;

        Health = maxHealth;

        // Find the HeartController in the scene and keep a reference to it
        heartControllerInstance = FindObjectOfType<HeartController>();
    }

    void Update()
    {
        
        if (PauseMenuController.IsGamePaused())
        {
            // The game is paused, so do something or don't do anything
            return; // Stop the Update method here if the game is paused.
        }
        
        

        if (pState.cutscene) return;
        GetInput();
        UpdateJumpVariable();
        RestoreTimeScale();
        if (pState.dashing) return;

        if (Grounded() && pState.jumping)
        {
            pState.jumping = false;
            OnPlayerLanded?.Invoke(); // Invoke the event when the player lands
        }

        Flip();
        Move();
        Jump1();
        StartDash();
        Attack();
        FlashWhileInvincible();
        Heal();
        CastSpell();



        // Set the 'Idle' parameter
        anim.SetBool("Idle", xAxis == 0 && Grounded() && !pState.dashing);

        // Update the Animator's Speed parameter every frame
        anim.SetFloat("Speed", Mathf.Abs(xAxis));

        // Check for scene change to Main Menu at the end of Update method or in a separate method called from Update
        //CheckCurrentScene();

        // Add a Debug.Log statement to check the time scale each frame
        //Debug.Log("Player Current Time.timeScale: " + Time.timeScale);

    }

    private void CheckForDuplicatePlayerInstances()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
     

        // If we load the main menu, remove this player instance
        if (scene.name == "MainMenu")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);

            //return; // Add return here so that we don't attempt to reinitialize after destruction
        }
        /*else
        {
            // Re-establish reference to HeartController if needed
            HeartController heartControllerInstance = FindObjectOfType<HeartController>();
            if (heartControllerInstance != null)
            {
                heartControllerInstance.ReinitializeHeartContainers();
            }
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;
        if (pState.dashing) return;
        Recoil();
    }

    /*void CheckCurrentScene()
    {
        // Check if the current scene is the Main Menu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        { // Assume "MainMenu" is your scene's name
            Destroy(gameObject); // Destroy the player object
                                 // Alternatively, deactivate the player object if you don't want to destroy it
                                 // gameObject.SetActive(false);
        }
    }
    */

    void StopRecoilX()
    {
        stepXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        //Debug.Log("TakeDamage called. Damage: " + _damage);
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ?
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) :
            Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //if exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //if exit direciton is horizontal movements
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
    }


    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.05f && health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.Healing = true;
            anim.SetBool("Healing", true);

            // Drain mana outside of the Mana property to avoid recursion
            Mana -= Time.deltaTime * manaDrainSpeed;

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timetoHeal)
            {
                health++;
                healTimer = 0;
                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
        else
        {
            pState.Healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            float clampedValue = Mathf.Clamp(value, 0, 1); // Assuming mana is a value between 0 and 1.
            if (mana != clampedValue)
            {
                //Debug.Log($"Updating mana UI to {clampedValue}"); // This will print the new mana value.
                mana = clampedValue;
                manaStorage.fillAmount = mana;
            }
        }
    }



    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        else
        {
            castOrHealTimer = 0;
        }
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;

        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !Dashed)
        {
            //Debug.Log($"Dash input: {Input.GetButtonDown("Dash")}, canDash: {canDash}, Dashed: {Dashed}");
            StartCoroutine(Dash());
            Dashed = true;
        }
        if (Grounded())
        {
            Dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;

    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;

        anim.SetTrigger("TakeDamage");

        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void Attack()
    {
        // Check if the mouse is currently over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // If true, return early and do not proceed with the attack
        }
        

        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            // Update the Animator's Speed parameter even during the attack
            anim.SetFloat("Speed", Mathf.Abs(xAxis)); // Add this line

            // Perform the attack based on the vertical axis and grounded state
            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
                //Debug.Log("Side Attack!");
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
                //Debug.Log("Up Attack!");
            }
            else if (yAxis < 0 || !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
                //Debug.Log("Down Attack!");
            }
        }

        // Set the Idle parameter based on the xAxis value and whether the player is grounded
        anim.SetBool("Idle", xAxis == 0 && Grounded() && !pState.dashing);
    }

    void Attacknew()
    {
        // Check if the mouse is currently over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // If true, return early and do not proceed with the attack
        }

        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            // Perform the attack based on the vertical axis and grounded state
            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform.position, Quaternion.identity);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform.position);
            }
            else if (yAxis < 0 || !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform.position);
            }
        }

        // Update the Animator's parameters
        anim.SetBool("Idle", xAxis == 0 && Grounded() && !pState.dashing);
        anim.SetFloat("Speed", Mathf.Abs(xAxis));
    }

    // Helper method to instantiate slash effects at a given angle
    void SlashEffectAtAngle(GameObject effect, float angle, Vector3 position)
    {
        var instantiatedEffect = Instantiate(effect, position, Quaternion.Euler(0f, 0f, angle));
        instantiatedEffect.transform.localScale = transform.localScale; // Ensure effect scales with player
    }



    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true; // Set recoil direction to true if any object is hit
            //Debug.Log("Recoil triggered");
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                // Log a message when the player hits an enemy
                //Debug.Log("Player hit enemy: " + objectsToHit[i].name);

                // Call the EnemyHit method on the enemy
                enemy.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }


    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            if (yAxis < 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //Stopping the recoiling 
        if (pState.recoilingX && stepXRecoiled < recoilXSteps)
        {
            stepXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingX && stepYRecoiled < recoilYSteps)
        {
            stepYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkspeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            //disable downspell on ground
            downSpellFireball.SetActive(false);
        }
        // if down spell is active, force player down until ground
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine()
    {
        anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f); //based on casting animtion, two parts, prep and cast state, take on frame on time 00:15 , frame time can change varies

        //side cast
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _fireball = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);

            //fireball
            if (pState.lookingRight)
            {
                _fireball.transform.eulerAngles = Vector3.zero; //if facing right, fireball continues as per normal
            }
            else
            {
                _fireball.transform.eulerAngles = new Vector2(_fireball.transform.eulerAngles.x, 180);
                //if not facing right, rotate the effect 180 degree
            }

            pState.recoilingX = true;
        }

        //up cast
        else if (yAxis > 0)
        {
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }

        //down cast
        else if (yAxis < 0 && !Grounded())
        {
            downSpellFireball.SetActive(true);
        }

        Mana = manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("Casting", false);
        pState.casting = false;

    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, WhatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jumpsmooth()
    {
        bool isJumping = Input.GetButton("Jump");
        bool jumpButtonReleased = Input.GetButtonUp("Jump");
        bool canJump = Grounded();

        // Start jump
        if (isJumping && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("Jumping", true);
        }

        // If jump button is released and player is still ascending, reduce the jump force
        if (jumpButtonReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Check if grounded to update animation
        anim.SetBool("Jumping", !Grounded());
    }

    void Jump1()
    {
        if (jumpbufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }
        if (!Grounded() && airJumpCounter < MaxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        anim.SetBool("Jumping", !Grounded());

    }

    void Jump()
    {
        // Check for ground jump
        if (Grounded() && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            pState.jumping = true;
            anim.SetBool("Jumping", true);
            OnPlayerJump?.Invoke(1); // Trigger the jump event with a jump count of 1
        }

        // Check for air jumps
        if (!Grounded() && airJumpCounter < MaxAirJumps && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            pState.jumping = true;
            airJumpCounter++;
            anim.SetBool("Jumping", true);
            OnPlayerJump?.Invoke(airJumpCounter + 1); // Trigger the jump event with the current air jump count
        }

        // Allow variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Reset jumping state when grounded
        if (Grounded())
        {
            pState.jumping = false;
            airJumpCounter = 0;
            anim.SetBool("Jumping", false);
        }
    }

    void Jump12()
    {
        bool isGrounded = Grounded();
        bool jumpButtonDown = Input.GetButtonDown("Jump");

        // Check for ground jump
        if (isGrounded && jumpButtonDown)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            pState.jumping = true;
            anim.SetBool("Jumping", true); // Trigger the jump animation
            OnPlayerJump?.Invoke(1); // Notify about the first jump
        }

        // Check for air jump (double jump)
        if (!isGrounded && jumpButtonDown && airJumpCounter < MaxAirJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            pState.jumping = true;
            airJumpCounter++;
            anim.SetBool("Jumping", true); // Trigger the jump animation
            OnPlayerJump?.Invoke(airJumpCounter + 1); // Notify about the second jump
        }

        // Allow variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Reset jumping state when grounded
        if (isGrounded && pState.jumping)
        {
            pState.jumping = false;
            airJumpCounter = 0;
            anim.SetBool("Jumping", false);
        }
    }




    void UpdateJumpVariable()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpbufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpbufferCounter--;
        }
    }
}