using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; // Add this line
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Add this line for scene management

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    private SpriteRenderer sr;
    private Animator anim;
    public static PlayerController Instance;
    private bool canDash = true;
    private bool Dashed;
    private float gravity;
    private bool attack = false;
    private float timeBetweenAttack, timeSinceAttack;
    private int stepXRecoiled, stepYRecoiled;

    [Header("Health Setting")]
    public float health;
    public float maxHealth;
    [SerializeField] private GameObject bloodSpurt;
    [SerializeField] private float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;
    private float healTimer;
    [SerializeField] private float timetoHeal;
    [SerializeField] private RectTransform heartFillRectTransform;
    [SerializeField] private float maxWidth = 100f;

    [Header("Mana Setting")]
    [SerializeField] private Image manaStorage;
    [SerializeField] private float mana;
    [SerializeField] private float manaDrainSpeed;
    [SerializeField] private float manaGain;
    [SerializeField] private RectTransform manaBarRectTransform;
    [SerializeField] private float maxManaHeight = 160f;
    private float manaMaxHeight = 100f;
    [SerializeField] private float manaChangeSpeed = 2f;
    [SerializeField] private float maxMana = 1f;
    [SerializeField] private float fullManaY = 160f;
    [SerializeField] private float emptyManaY = -100f;

    [Header("Spell Setting")]
    [SerializeField] private float manaSpellCost = 0.3f;
    [SerializeField] private float timeBetweenCast = 0.5f;
    [SerializeField] private float spellDamage;
    [SerializeField] private float downSpellForce;
    [SerializeField] private GameObject sideSpellFireball;
    [SerializeField] private GameObject upSpellExplosion;
    [SerializeField] private GameObject downSpellFireball;
    private float timeSinceCast;
    private float castOrHealTimer;

    [Header("Horizontal Movement Setting")]
    [SerializeField] private float walkspeed = 1;

    [Header("Vertical Movement Setting")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpbufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int MaxAirJumps;

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask WhatIsGround;

    [Header("Dash Variable Setting")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private GameObject dashEffect;

    [Header("Attack Setting")]
    [SerializeField] private Transform SideAttackTransform;
    [SerializeField] private Transform UpAttackTransform;
    [SerializeField] private Transform DownAttackTransform;
    [SerializeField] private Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage;
    [SerializeField] private GameObject slashEffect;

    [Header("Recoil Setting")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;
    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;


    [Header("Jumpzoom Setting")]
    [SerializeField] private float firstJumpZoomFactor = 1.2f; // Adjust for the first jump
    [SerializeField] private float secondJumpZoomFactor = 1.5f; // Adjust for the second jump
    private int currentJumpCount = 0;


    //camera zoomout
    public delegate void PlayerJumpDelegate(int jumpCount);
    public event PlayerJumpDelegate OnPlayerJump;
    public event Action OnPlayerLanded;
    //public HeartController heartControllerInstance;



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
            DontDestroyOnLoad(gameObject);
        }


    }
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        pState.alive = true; // Ensure the player is marked as alive at the start

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        sr = GetComponent<SpriteRenderer>();
        Mana = mana;
        Health = maxHealth;

    }

    public void LoadData(GameData data)
    {
        // Load the player's position from the saved data
        transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        // Save the current player's position to the data
        data.playerPosition = transform.position;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    void Update()
    {

        if (pState.cutscene) return;
        GetInput();
        UpdateJumpVariable();
        if (pState.dashing) return;

        if (Grounded() && pState.jumping)
        {
            pState.jumping = false;
        }

        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        FlashWhileInvincible();
        Heal();
        CastSpell();

    }


    void InitializeManaStorage()
    {
        GameObject manaStorageObject = GameObject.FindGameObjectWithTag("ManaStorageTag");
        if (manaStorageObject != null)
        {
            manaBarRectTransform = manaStorageObject.GetComponent<RectTransform>();
            if (manaBarRectTransform != null)
            {
                Debug.Log("ManaStorage found and assigned.");
                Mana = 0f; // Reset mana and update UI.
            }
            else
            {
                Debug.LogWarning("Mana storage image is missing or not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Failed to find ManaStorage by tag.");
        }
    }

    public void UpdateHeartUI()
    {
        if (heartFillRectTransform != null)
        {
            float healthPercentage = health / maxHealth;
            float newWidth = maxWidth * healthPercentage;
            heartFillRectTransform.sizeDelta = new Vector2(newWidth, heartFillRectTransform.sizeDelta.y);
        }
        else
        {
            Debug.LogError("HeartFill RectTransform not found.");
        }
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeManaStorage();
        InitializeHeartUI(); // Call initialization explicitly here // Ensure heart UI reflects current health
        //UpdateHeartUI(); // Ensure this updates UI based on current health.
        if (scene.name == "MainMenu")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);


            //return; // Add return here so that we don't attempt to reinitialize after destruction
        }
        // Perform the same initialization as in Start().
        Debug.Log("Scene loaded, attempting to reinitialize.");
        //InitializeManaStorage();
        // Other necessary reinitializations.
    }

    private void InitializeHeartUI()
    {
        // Attempt to find the heartFill if not manually assigned
        if (heartFillRectTransform == null)
        {
            GameObject heartFillObject = GameObject.FindGameObjectWithTag("HeartFillTag");
            if (heartFillObject)
            {
                heartFillRectTransform = heartFillObject.GetComponent<RectTransform>();
                if (heartFillRectTransform != null)
                {
                    Debug.Log("HeartFill found and assigned dynamically.");
                    Debug.Log("HeartFill width after found: " + heartFillRectTransform.rect.width);
                    maxWidth = heartFillRectTransform.rect.width;
                    UpdateHeartUI(); // Update immediately to ensure UI is correct
                    Debug.Log("HeartFill width after UpdateHeartUI call: " + heartFillRectTransform.rect.width);
                }
                else
                {
                    Debug.LogError("HeartFill GameObject found, but it does not have a RectTransform component.");
                }
            }
            else
            {
                Debug.LogError("Failed to find HeartFill GameObject. Check if the tag is correctly applied.");
            }
        }
        else
        {
            Debug.Log("HeartFill already assigned through Inspector with width: " + heartFillRectTransform.rect.width);
            UpdateHeartUI(); // Update again to be certain UI is correct
        }
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


    public void TakeDamage(float damage)
    {
        if (!pState.invincible && pState.alive) // Only take damage if not already invincible and alive
        {
            Debug.Log($"Before taking damage: Health = {health}");
            health -= damage; // Directly subtract the floating-point damage value
            health = Mathf.Clamp(health, 0, maxHealth); // Ensure health does not go below 0 or above maxHealth
            UpdateHeartUI(); // Update heart UI based on new health
            Debug.Log($"After taking damage: Health = {health}");

            if (health <= 0)
            {
                StartCoroutine(Death()); // Trigger death sequence
                return; // Exit the function to not proceed further after death
            }

            anim.SetTrigger("TakeDamage");
            StartCoroutine(MakeInvincible(1.0f)); // Start invincibility and flashing
        }
    }



    // Assuming this is somewhere within the PlayerController class where you handle health changes
    public void HandleHealthChanged()
    {
        float healthPercentage = (float)health / maxHealth; // Ensure health and maxHealth are float for accurate calculation
        //heartControllerInstance.UpdateHeartBar(healthPercentage);
    }



    IEnumerator MakeInvincible(float duration)
    {
        pState.invincible = true;
        StartCoroutine(FlashEffect()); // Start flashing
        yield return new WaitForSeconds(duration);
        pState.invincible = false;
        StopCoroutine(FlashEffect()); // Ensure to stop the flashing effect
        sr.material.color = Color.white; // Reset sprite color back to normal
    }

    IEnumerator FlashEffect()
    {
        float endTime = Time.time + 1.0f; // Adjust duration accordingly
        while (pState.invincible) // Use invincibility check to control flashing duration
        {
            sr.material.color = sr.material.color == Color.white ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        sr.material.color = Color.white; // Ensure color is reset after the loop
    }


    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ?
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) :
            Color.white;
        StartCoroutine(FlashEffect());
    }

    IEnumerator Death()
    {
        
        pState.alive = false;
        Time.timeScale = 1f;
        //GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        //Destroy(_bloodspurtParticles, 1, 5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);

    }


    public float Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0f, maxHealth);
                UpdateHeartUI(); // Update the heart UI to reflect the change.
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

    


    public float Mana
    {
        get { return mana; }
        set
        {
            mana = Mathf.Clamp(value, 0, 1);
            UpdateManaUI(mana); // Update the UI with the current mana percentage
            Debug.Log($"Mana updated to: {mana}");
        }
    }


    public void UpdateManaUI(float manaPercentage)
    {
        if (manaBarRectTransform != null)
        {
            // Assuming the mana bar's anchor is at the bottom center of the bar.
            // Calculate the Y position based on the mana percentage
            float minY = -50f; // The starting Y position when mana is empty
            float maxY = 50f; // The Y position when mana is full

            // Calculate the new Y position within the range based on the current mana percentage
            float newY = Mathf.Lerp(minY, maxY, manaPercentage);

            // Set the new position of the mana bar
            manaBarRectTransform.anchoredPosition = new Vector2(manaBarRectTransform.anchoredPosition.x, newY);
        }
        else
        {
            Debug.LogWarning("ManaBar RectTransform is not assigned.");
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

    /* original 
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
    */

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            Debug.Log($"Mana before cast: {Mana}");
            Debug.Log($"Mana cost for casting: {manaSpellCost}");

            Mana -= manaSpellCost; // Deduct the mana spell cost from the current mana

            Debug.Log($"Mana after cast: {Mana}");

            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        // Disable down spell on ground
        if (Grounded())
        {
            downSpellFireball.SetActive(false);
        }

        // If down spell is active, force player down until ground
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


    void Jump()
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