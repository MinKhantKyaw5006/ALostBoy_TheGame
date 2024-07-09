//-------------------------------------------------------------------------DEPEDENCIES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; // Add this line
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Add this line for scene management
using UnityEngine.InputSystem;
//-------------------------------------------------------------------------DEPEDENCIES

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
    public bool isInDialogue = false;
    public bool isControlDisabled;
    private Coroutine dashCoroutine;


    //camera follow settting---------------------------
    [Header("Camera Setting")]
    [SerializeField] private GameObject _cameraFollowGo;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangethresold;
    //camera follow settting---------------------------

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
    [SerializeField] private DamageFlash damageFlash;
    // Reference to SaveGroundCP script
    //[SerializeField] private SaveGroundCP saveGroundCP;


    [Header("Mana Setting")]
    //[SerializeField] private Image manaStorage;
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
    public float walkspeed = 1f;
    [SerializeField] private float PlayerScale = 1;

    [Header("Vertical Movement Setting")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpbufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    public int MaxAirJumps;

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
    public float damage;
    [SerializeField] private GameObject slashEffect;
    [SerializeField] private LayerMask attackableItemMask; // Layer mask for attackable items
    [SerializeField] private float attackRange = 1f; // Range of the player's attack

    [Header("Attack Cooldown")]
    [SerializeField] private float attackRate = 2f; // How many attacks per second
    [SerializeField]private float nextAttackTime = 0f;
    [SerializeField] private float attackCooldown = 1f; // Adjust this value as needed


    [Header("Recoil Setting")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;
    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;


    [Header("Jumpzoom Setting")]
    [SerializeField] private float firstJumpZoomFactor = 1.2f; // Adjust for the first jump
    [SerializeField] private float secondJumpZoomFactor = 1.5f; // Adjust for the second jump
    private int currentJumpCount = 0;

    [SerializeField] private SaveGroundCP saveGroundCP;


    //camera zoomout
    public delegate void PlayerJumpDelegate(int jumpCount);
    public event PlayerJumpDelegate OnPlayerJump;
    public event Action OnPlayerLanded;
    //public HeartController heartControllerInstance;

    private PlayerControls playerControls;
    private Vector2 currentMovementInput;

    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;
    private bool isHealing = false; // Add this to keep track of healing state
    public bool speedBoostActive = false;

    // Start is called before the first frame update

    //moving platform
    private GameObject currentPlatform = null;
    private Vector3 lastPlatformPosition;
    private Rigidbody2D playerRigidbody;
    private Vector2 platformVelocityLastFrame;// Inside PlayerController
    public event Action<bool> OnFlip;

    // Sound Effects
    [Header("SoundEffects")]
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource dashSoundEffect;
    [SerializeField] private AudioSource attackSoundEffect;
    [SerializeField] private AudioSource healSoundEffect;
    [SerializeField] private AudioSource spellSoundEffect;
    [SerializeField] private AudioSource onGroundSoundEffect;
    [SerializeField] private AudioSource WalkingEffect;
    [SerializeField] private AudioSource hitSound; // Drag your hit sound effect here in the Inspector

    //tutorial session
    public bool canMoveLeft = true;
    public bool canMoveRight = true;
    public bool canJump = true;

    public bool IsFacingRight
    {
        get { return pState.lookingRight; } // Assuming pState.lookingRight exists and indicates facing direction
    }



    private void Awake()
    {
   

        // Initialize the input controls
        playerControls = new PlayerControls();

        // Subscribe to the performed and canceled events of the Move action
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        // Initialize healing action callbacks
      


    }


    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        playerRigidbody = GetComponent<Rigidbody2D>();

        pState.alive = true; // Ensure the player is marked as alive at the start

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        sr = GetComponent<SpriteRenderer>();
        Mana = mana;
        Health = maxHealth;
        //saveGroundCP = FindObjectOfType<SaveGroundCP>(); // Find the SaveGroundCP component in the scene

    }

    //------------------------Save&Load---------------------------------------------------------
    //loading player data
    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;

    }

    //saving player data
    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
   
    }
    //------------------------Save&Load---------------------------------------------------------

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerControls.Enable();
        playerControls.Player.Jump.performed += OnJumpPerformed;
        playerControls.Player.Dash.performed += OnDashPerformed;
        playerControls.Player.Attack.performed += OnAttackPerformed;
        playerControls.Player.Heal.performed += OnHealPerformed;
        playerControls.Player.Heal.canceled += OnHealCanceled;
        //playerControls.Player.SpellCast.performed += OnSpellCastPerformed;
        playerControls.Enable();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        playerControls.Disable();
        playerControls.Player.Jump.performed -= OnJumpPerformed;
        playerControls.Player.Dash.performed -= OnDashPerformed;
        playerControls.Player.Attack.performed -= OnAttackPerformed;
        playerControls.Player.Heal.performed -= OnHealPerformed;
        playerControls.Player.Heal.canceled -= OnHealCanceled;
        //playerControls.Player.SpellCast.performed -= OnSpellCastPerformed;  
        playerControls.Disable();

    }
    // Callbacks from the new input system
    private void OnHealPerformed(InputAction.CallbackContext context)
    {
        isHealing = true; // Start healing
    }

    private void OnHealCanceled(InputAction.CallbackContext context)
    {
        isHealing = false; // Stop healing
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // Perform the attack logic
        Attack();
        // Play attack sound effect
        //attackSoundEffect.Play();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isInDialogue || isControlDisabled)
        {
            return; // Ignore jump input if in dialogue or controls are disabled
        }

        // If jump is performed and player is on the ground or has air jumps left, make the player jump
        if (Grounded() || (airJumpCounter < MaxAirJumps && !pState.jumping))
        {
            pState.jumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            if (!Grounded())
            {
                airJumpCounter++;
            }
            anim.SetBool("Jumping", true);

            jumpSoundEffect.Play(); // Play jump sound effect only when jump is performed
        }
    }


    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (isInDialogue || isControlDisabled)
        {
            return; // Ignore dash input if in dialogue or controls are disabled
        }

        // Check if dash is performed and the player can dash
        if (canDash && !Dashed)
        {
            StartCoroutine(Dash());
            Dashed = true;
        }
    }



    // Modify existing methods to check these flags
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!isInDialogue) // Only proceed if not in dialogue
        {
            float moveInput = context.ReadValue<Vector2>().x;
            // Implement movement restrictions based on flags
            if ((moveInput < 0 && canMoveLeft) || (moveInput > 0 && canMoveRight))
            {
                currentMovementInput = context.ReadValue<Vector2>();
                WalkingEffect.Play();
            }
            else
            {
                // If movement is restricted, stop the movement and walking sound effect
                rb.velocity = new Vector2(0, rb.velocity.y);
                WalkingEffect.Stop();
            }
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Reset the current movement input when the input is canceled
        currentMovementInput = Vector2.zero;

        // Stop the walking sound effect
        //WalkingEffect.Stop();
    }

    void HandleWalkingSound()
    {
        // Only play the walking sound if the player is moving and grounded
        if (Grounded() && Mathf.Abs(currentMovementInput.x) > 0.1f && !WalkingEffect.isPlaying)
        {
            WalkingEffect.Play();
        }
        else if (!Grounded() || Mathf.Abs(currentMovementInput.x) <= 0.1f)
        {
            // Optionally, stop the walking sound if the player is airborne or not moving
            WalkingEffect.Stop();
        }
    }




    void Update()
    {

        if (isInDialogue)
        {
            // Halt player movement and skip the rest of the update if in dialogue
            rb.velocity = Vector2.zero; // Explicitly reset the player's velocity
            WalkingEffect.Stop(); // Stop the walking sound effect if in dialogue
            anim.SetBool("Walking", false); // Ensure the walking animation is stopped
            return; // Skip further processing
        }

        // Get the movement input
        xAxis = currentMovementInput.x;

        if (pState.cutscene) return;
        GetInput();
        timeSinceCast += Time.deltaTime;
        UpdateJumpVariable();
        if (pState.dashing) return;

        if (Grounded() && pState.jumping)
        {
            pState.jumping = false;
        }
        /*
        if (Input.GetButtonDown("Cast"))
        {
            Debug.Log("Cast button pressed");
            CastSpellTest();
        }
        */

        HandleWalkingSound();
        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        FlashWhileInvincible();
        Heal();
       
        CastSpell();
        


        //if we are falling past a certain speed thresold
        if (rb.velocity.y < _fallSpeedYDampingChangethresold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        //if we are standing still or moving up
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            //reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        } 
        

    }
    public void DisableControls()
    {
        isControlDisabled = true;
        rb.velocity = Vector2.zero;
        WalkingEffect.Stop();
        anim.SetBool("Walking", false);
        anim.SetBool("Jumping", false);

        // Stop dashing if the player is dashing
        if (pState.dashing && dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            rb.gravityScale = gravity;
            pState.dashing = false;
            canDash = true; // Reset ability to dash

            // Stop the dash effect
            if (dashEffect != null && dashEffect.activeInHierarchy)
            {
                Destroy(dashEffect);
            }

            // Stop the dash sound effect
            if (dashSoundEffect.isPlaying)
            {
                dashSoundEffect.Stop();
            }
        }

        // Stop jumping sound effect
        if (jumpSoundEffect.isPlaying)
        {
            jumpSoundEffect.Stop();
        }
    }


    public void EnableControls()
    {
        isControlDisabled = false;
        if (Grounded())
        {
            Dashed = false;
            airJumpCounter = 0; // Reset air jump counter when grounded
        }
    }

    void CastSpellTest()
    {
        Debug.Log($"Attempting to cast spell. Current Mana: {Mana}");
        if (Mana >= manaSpellCost)
        {
            Debug.Log($"Casting spell. Mana before cast: {Mana}, Cost: {manaSpellCost}");
            Mana -= manaSpellCost;
            Debug.Log($"Mana after cast: {Mana}");
            StartCoroutine(CastCoroutine());
        }
        else
        {
            Debug.Log("Not enough mana to cast.");
        }
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
        /*
        // Check if we're loading into a gameplay scene where the player is needed.
        if (scene.name == "Chapter1" )
        {
            Debug.Log("Activating PlayerController.");
            // Make sure player is active when needed.
            gameObject.SetActive(true);
            InitializeManaStorage();
            InitializeHeartUI(); // Call initialization explicitly here.
                                 // Ensure heart UI reflects current health.
            Debug.Log("Gameplay scene loaded, player initialized.");
        }
        else
        {
            Debug.Log("Deactivating PlayerController.");
            // If we're not in a gameplay scene, disable the player instead of destroying it.
            gameObject.SetActive(false);
            Debug.Log("Non-gameplay scene loaded, player deactivated.");
        }
        */
    }

  

    void InitializeHealthUI()
    {
        // Find the heartFillRectTransform if not manually assigned
        if (heartFillRectTransform == null)
        {
            GameObject heartFillObject = GameObject.FindGameObjectWithTag("HeartFillTag");
            if (heartFillObject != null)
            {
                heartFillRectTransform = heartFillObject.GetComponent<RectTransform>();
                if (heartFillRectTransform != null)
                {
                    maxWidth = heartFillRectTransform.sizeDelta.x;
                }
            }
        }

        // Update health UI immediately to reflect current health
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (heartFillRectTransform != null)
        {
            float healthPercentage = health / maxHealth;
            heartFillRectTransform.sizeDelta = new Vector2(maxWidth * healthPercentage, heartFillRectTransform.sizeDelta.y);
        }
        else
        {
            Debug.LogWarning("HeartFill RectTransform is not assigned.");
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
        //if (pState.cutscene) return;
        if (pState.dashing) return;
        Recoil();

        if (currentPlatform != null)
        {
            Vector3 deltaPosition = currentPlatform.transform.position - lastPlatformPosition;
            transform.position += deltaPosition;
            lastPlatformPosition = currentPlatform.transform.position;
        }

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
            UpdateHealthUI(); // Update heart UI based on new health
            Debug.Log($"After taking damage: Health = {health}");

            if (health <= 0)
            {
                StartCoroutine(Death()); // Trigger death sequence
                return; // Exit the function to not proceed further after death
            }

            anim.SetTrigger("TakeDamage"); // Trigger the TakeDamage animation
            StartCoroutine(MakeInvincible(1.0f, true)); // Make player invincible for 1 second after taking damage with flash effect
        }
    }





    // Assuming this is somewhere within the PlayerController class where you handle health changes
    public void HandleHealthChanged()
    {
        float healthPercentage = (float)health / maxHealth; // Ensure health and maxHealth are float for accurate calculation
        //heartControllerInstance.UpdateHeartBar(healthPercentage);
    }


    IEnumerator MakeInvincible(float duration, bool shouldFlash)
    {
        pState.invincible = true;
        if (shouldFlash)
        {
            //StartCoroutine(FlashEffect());
            // Trigger the damage flash effect
            if (damageFlash != null)
            {
                damageFlash.Flash();
            }
        }
        yield return new WaitForSeconds(duration);
        pState.invincible = false;
        if (shouldFlash)
        {
            StopCoroutine(FlashEffect());
            sr.material.color = Color.white; // Reset sprite color back to normal
        }
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
        //Time.timeScale = 1f;
        // anim.SetBool("IsAlive", false); // Indicate death in the Animator
        anim.SetBool("IsDead", true); // Trigger death animation
        Debug.Log("Player Death Animation Triggered");

        yield return new WaitForSeconds(0f); // Wait time could be adjusted based on your death animation length

        // Additional wait time before warping
        //float warpUpDelay = 4f; // Adjust this value to match your needs
        //yield return new WaitForSeconds(warpUpDelay);

        

       
        if (saveGroundCP != null)
        {
            
            transform.position = saveGroundCP.safeGroundLocation;
        }
        else
        {
            Debug.LogError("SaveGroundCP reference not set in PlayerController.");
        }
       

       

        // Set the player to alive again for the Animator and state
        // Respawn logic here
        anim.SetBool("IsDead", false); // Exit death animation
                                       // Reset health to full and update UI
        Health = maxHealth;
        UpdateHeartUI();
        anim.SetBool("IsAlive", true);
        pState.alive = true;

        // Optionally, make the player invincible for a short duration after respawning
        StartCoroutine(MakeInvincible(2.0f,true));
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
                Debug.Log($"health updated to: {health}");
            }
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
        /*
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        */

        // Use the value from the new input system
        xAxis = currentMovementInput.x;
        yAxis = Input.GetAxisRaw("Vertical");

        /*
        // Check for attack input
        attack = Input.GetButtonDown("Attack");
        */

        // Only set attack to true if the cooldown has passed
        if (Input.GetButtonDown("Attack") && Time.time >= nextAttackTime)
        {
            attack = true;
            // Update the next allowed attack time based on current time and cooldown duration
            nextAttackTime = Time.time + attackCooldown; // Assume attackCooldown is a serialized field or constant
            //Debug.Log("can attack");
        }
        else
        {
            attack = false; // This line can be omitted if you handle attack elsewhere
            //Debug.Log("cannot attack");
        }


        if (Input.GetButton("Cast"))
        {
            castOrHealTimer += Time.deltaTime;
            
            //CastSpell();
            
        }
        else
        {
            castOrHealTimer = 0;
            
        }
    }

    private void Flip()
    {
        if (isControlDisabled) return;

        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-PlayerScale, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(PlayerScale, transform.localScale.y);
            pState.lookingRight = true;
        }

        OnFlip?.Invoke(pState.lookingRight); // Raise the event whenever the player flips
    }

    void StartDash()
    {
        // The dash input is now handled by OnDashPerformed.

        // Resetting Dashed if the player is grounded
        if (Grounded())
        {
            Dashed = false;
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        dashCoroutine = StartCoroutine(MakeInvincible(dashTime, false)); // Make the player invincible during dash without flashing effect
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);

        WalkingEffect.Stop();
        dashSoundEffect.Play();

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }



    /*
    void Attack()
    {
        if (isInDialogue || isControlDisabled) return; // Skip attack logic if in dialogue or controls are disabled

        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            // Update the Animator's Speed parameter even during the attack
            anim.SetFloat("Speed", Mathf.Abs(xAxis)); // Add this line

            Vector3 effectScale = new Vector3(1f, 1f, 1f); // Define the desired scale for the slash effect

            // Perform the attack based on the vertical axis and grounded state
            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                float effectAngle = pState.lookingRight ? 0f : 180f;
                SlashEffectAtAngle(slashEffect, effectAngle, SideAttackTransform.position, effectScale);
                Debug.Log("Side Attack!");
                PlayAttackSound();
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform.position, effectScale);
                Debug.Log("Up Attack!");
                PlayAttackSound();
            }
            else if (yAxis < 0 || !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform.position, effectScale);
                Debug.Log("Down Attack!");
                PlayAttackSound();
            }
        }

        // Set the Idle parameter based on the xAxis value and whether the player is grounded
        anim.SetBool("Idle", xAxis == 0 && Grounded() && !pState.dashing);
    }*/

    void Attack()
    {
        if (isInDialogue || isControlDisabled) return; // Skip attack logic if in dialogue or controls are disabled

        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            // Update the Animator's Speed parameter even during the attack
            anim.SetFloat("Speed", Mathf.Abs(xAxis)); // Add this line

            Vector3 effectScale = new Vector3(1f, 1f, 1f); // Define the desired scale for the slash effect

            // Perform the attack based on the vertical axis and grounded state
            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                float effectAngle = pState.lookingRight ? 0f : 180f;
                SlashEffectAtAngle(slashEffect, effectAngle, SideAttackTransform.position, effectScale);
                Debug.Log("Side Attack!");
                PlayAttackSound();
                CheckForAttackableItems(SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform.position, effectScale);
                Debug.Log("Up Attack!");
                PlayAttackSound();
                CheckForAttackableItems(UpAttackTransform);
            }
            else if (yAxis < 0 || !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform.position, effectScale);
                Debug.Log("Down Attack!");
                PlayAttackSound();
                CheckForAttackableItems(DownAttackTransform);
            }
        }

        // Set the Idle parameter based on the xAxis value and whether the player is grounded
        anim.SetBool("Idle", xAxis == 0 && Grounded() && !pState.dashing);
    }

    private void CheckForAttackableItems(Transform attackTransform)
    {
        Collider2D[] hitItems = Physics2D.OverlapCircleAll(attackTransform.position, attackRange, attackableItemMask);

        foreach (Collider2D hit in hitItems)
        {
            // Check if the item has an AttackableItem component
            AttackableItem attackableItem = hit.GetComponent<AttackableItem>();
            if (attackableItem != null)
            {
                attackableItem.OnHit(); // Trigger the item's hit behavior
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SideAttackTransform.position, attackRange);
        Gizmos.DrawWireSphere(UpAttackTransform.position, attackRange);
        Gizmos.DrawWireSphere(DownAttackTransform.position, attackRange);
    }

    void PlayAttackSound()
    {
        // Check if attack sound effect is already playing to avoid overlapping
        if (!attackSoundEffect.isPlaying)
        {
            attackSoundEffect.Play();
        }
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
                    hitSound.Play();
                    Mana += manaGain;
                }
            }
        }
    }


    void SlashEffectAtAngle(GameObject effectPrefab, float angle, Vector3 position, Vector3 effectScale)
    {
        GameObject effectInstance = Instantiate(effectPrefab, position, Quaternion.Euler(0f, 0f, angle));
        effectInstance.transform.localScale = effectScale; // Apply the fixed scale

        // Parent the effect instance to the player
        effectInstance.transform.parent = transform;

        // Optionally, adjust the position for up/down attacks if needed
        if (angle == 80 || angle == -90) // Assuming 80 is for up and -90 is for down
        {
            effectInstance.transform.localPosition += new Vector3(0, 0.1f, 0); // Example adjustment
        }

        // Set up a coroutine to unparent the effect after a short duration
        StartCoroutine(UnparentEffectAfterDuration(effectInstance, 0.5f)); // Adjust the duration as needed
    }

    IEnumerator UnparentEffectAfterDuration(GameObject effectInstance, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Unparent the effect and optionally adjust its settings
        effectInstance.transform.parent = null;

        // Here you can adjust the effect if needed, for example, start fading it out or destroy it
        // Destroy(effectInstance, additionalDuration); // Destroy the effect after an additional duration if needed
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
        if (isInDialogue || isControlDisabled)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement
            anim.SetBool("Walking", false); // Ensure the walking animation is not playing
            return; // Skip the rest of the method
        }

        // Use currentMovementInput to set the velocity
        float moveX = currentMovementInput.x * walkspeed;
        float moveY = rb.velocity.y;
        rb.velocity = new Vector2(moveX, moveY);

        // Update the animator with the walking state
        anim.SetBool("Walking", moveX != 0 && Grounded());
    }


   
    // Updated Heal method without direct input checks
    void Heal()
    {
        if (isHealing && health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.Healing = true;
            anim.SetBool("Healing", true);
            Debug.Log("Player healing");    

            // Play heal sound effect if not already playing
            //if (!healSoundEffect.isPlaying) healSoundEffect.Play();

            // Drain mana outside of the Mana property to avoid recursion
            Mana -= Time.deltaTime * manaDrainSpeed;

            // Healing
            healTimer += Time.deltaTime;
            if (healTimer >= timetoHeal)
            {
                //health++;
                Health++; // Increment health using the Health property
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

   
    
    void CastSpell()
    {
        // Check if mana is full and other conditions are met before casting
        if (Input.GetButtonDown("Cast") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost )
        {
            Debug.Log($"Mana before cast: {Mana}");
            Debug.Log($"Mana cost for casting: {manaSpellCost}");

            // Deduct the mana spell cost from the current mana
            Mana -= manaSpellCost;

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
        Debug.Log("Spell casting coroutine started.");
        anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f); //based on casting animtion, two parts, prep and cast state, take on frame on time 00:15 , frame time can change varies

        // Play spell sound effect
        spellSoundEffect.Play();

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

        //Mana = manaSpellCost;
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
        if (isInDialogue || isControlDisabled) return; // Skip if in dialogue or controls are disabled
                                                       // Jump logic is now handled by OnJumpPerformed.
                                                       // The following is to handle the "letting go" of the jump button for variable jump height

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            Debug.Log("PlayerJumped");
            // jumpSoundEffect.Play(); // Commented out to prevent jump sound effect when releasing jump button
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
            //WalkingEffect.Stop();
            //jumpSoundEffect.Play();

            //WalkingEffect.Stop();
            
        }
        else
        {
            jumpbufferCounter--;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.gameObject;
            lastPlatformPosition = currentPlatform.transform.position;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }




}