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
    public float health; // Now a float
    public float maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;
    float healTimer;
    [SerializeField] float timetoHeal;
    [SerializeField] private RectTransform heartFillRectTransform; // Assign this in the Inspector
    [SerializeField] private float maxWidth = 100f; // Adjust based on your full heart UI width

    //public HeartController heartController;

    [Space(5)]

    [Header("Mana Setting")]
    [SerializeField] Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [SerializeField] private RectTransform manaBarRectTransform; // Assign in the inspector
    [SerializeField] private float maxManaHeight = 160f; // The y-position when mana is empty
    [SerializeField] private float manaIncrementPerAttack = 1f; // How much the bar should move up per attack
    private float manaMaxHeight = 100f; // Adjust based on your UI setup
    [SerializeField] private float manaChangeSpeed = 2f; // Speed of the mana bar animation
    private float targetManaY; // Target y position of the mana bar based on current mana
                               // These should be serialized or public if you want to set them in the Unity editor.
    [SerializeField] private float maxMana = 1f; // The maximum value that 'mana' can reach.
    [SerializeField] private float fullManaY = 160f; // The RectTransform's y position when the mana bar is full.
    [SerializeField] private float emptyManaY = -100f; // The RectTransform's y position when the mana bar is empty.

    // This can be private as we calculate it internally based on the current mana.




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
        }

        DontDestroyOnLoad(gameObject);

        // Checks if the current instance of the player should be destroyed
        CheckForDuplicatePlayerInstances();
        Mana = 0f; // Initialize mana to 0 and update UI


    }
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        sr = GetComponent<SpriteRenderer>();
        Mana = mana;


       

        InitializeManaStorage();
        Mana = 0f; // Set initial mana value and update UI.

        Health = maxHealth;
        InitializeHeartUI();
        



        // Find the HeartController in the scene and keep a reference to it
        //heartControllerInstance = FindObjectOfType<HeartController>();
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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



    /*
    void InitializeManaStorage()
    {
        if (manaStorage == null)
        {
            Debug.Log("Attempting to find ManaStorage by tag.");
            manaStorage = GameObject.FindGameObjectWithTag("ManaStorageTag").GetComponent<Image>();
            if (manaStorage != null)
            {
                Debug.Log("ManaStorage found successfully by tag.");
                manaStorage.fillAmount = mana;
            }
            else
            {
                Debug.LogWarning("Failed to find ManaStorage by tag.");
            }
        }
        else
        {
            Debug.Log("ManaStorage was pre-assigned.");
        }
    }
    */

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



    public void ResetPlayerState()
    {
        // Reset player position
        Transform spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            // Reset other state variables like velocity if needed
            rb.velocity = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("Spawn point not found. Make sure your spawn point is tagged correctly.");
        }

        // Reset other parts of the player state if necessary, e.g., Mana, Health
        Mana = 0f; // Assuming full mana is 1
        Health = maxHealth;

        // Reset logic for player state...

        UpdateManaUI(mana); // Make sure UI reflects the reset state
    }

    /*
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
        
    }
    */

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

    /*
    public void TakeDamage(float _damage)
    {
        //Debug.Log("TakeDamage called. Damage: " + _damage);
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }*/

    /*
    public void TakeDamage(float damage)
    {
        if (!pState.invincible) // Only take damage if not already invincible
        {
            Debug.Log($"Before taking damage: Health = {health}");
            Health -= damage; // Directly subtract the floating-point damage value
            Debug.Log($"After taking damage: Health = {health}");

            anim.SetTrigger("TakeDamage");
            StartCoroutine(MakeInvincible(1.0f)); // Start invincibility and flashing
            heartControllerInstance?.UpdateHeartBar(); // Update health UI
        }
    }*/

    public void TakeDamage(float damage)
    {
        if (!pState.invincible) // Only take damage if not already invincible
        {
            Debug.Log($"Before taking damage: Health = {health}");
            health -= damage; // Directly subtract the floating-point damage value
            health = Mathf.Clamp(health, 0, maxHealth); // Ensure health does not go below 0 or above maxHealth
            UpdateHeartUI(); // Update heart UI based on new health
            Debug.Log($"After taking damage: Health = {health}");

            anim.SetTrigger("TakeDamage");
            StartCoroutine(MakeInvincible(1.0f)); // Start invincibility and flashing

            // Calculate the health percentage
            float healthPercentage = health / maxHealth;
            // Update the heart UI with the current health percentage
            //heartControllerInstance?.UpdateHeartBar(healthPercentage);
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


    /* public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
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
    */

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

    /*
    public float Mana
    {
        get { return mana; }
        set
        {
            Debug.Log($"Updating Mana. New Value: {value}");
            mana = Mathf.Clamp(value, 0, 1);
            if (manaStorage != null)
            {
                manaStorage.fillAmount = mana;
                Debug.Log($"Mana UI Updated. Fill Amount: {manaStorage.fillAmount}");
            }
            else
            {
                Debug.LogWarning("Mana storage image is missing or not assigned.");
            }
        }
    }
    */


    public float Mana
    {
        get { return mana; }
        set
        {
            mana = Mathf.Clamp(value, 0, 1);
            UpdateManaUI(mana); // Update the UI with the current mana percentage
        }
    }







    /*
    public void UpdateManaUI()
    {
        if (manaStorage != null)
        {
            manaStorage.fillAmount = Mana; // Ensure this uses the property to maintain any associated logic.
        }
        else
        {
            Debug.LogWarning("Attempting to update mana UI but manaStorage is null.");
        }
    }
    */

    /*
    private void UpdateManaUI()
    {
        if (manaBarRectTransform != null)
        {
            // Calculate the new height based on the current mana.
            float newHeight = manaMaxHeight * Mana; // Mana is a percentage of the max height.
            manaBarRectTransform.sizeDelta = new Vector2(manaBarRectTransform.sizeDelta.x, newHeight);

            // Optionally, adjust the position if needed (e.g., if the anchor is at the top).
            // manaBarRectTransform.anchoredPosition = new Vector2(manaBarRectTransform.anchoredPosition.x, startingYPosition - (manaMaxHeight - newHeight));
        }
        else
        {
            Debug.LogWarning("ManaBar RectTransform is not assigned.");
        }
    }
    */
    /*
    public void UpdateManaUI(float manaPercentage)
    {
        if (manaBarRectTransform != null)
        {
            // Assuming manaPercentage is a value between 0 and 1
            float fullHeight = 160f; // The maximum height of the mana bar when it's full. You need to define this based on your UI setup.

            // Calculate the new height based on the mana percentage
            float newHeight = fullHeight * manaPercentage;

            // Set the size of the mana bar
            manaBarRectTransform.sizeDelta = new Vector2(manaBarRectTransform.sizeDelta.x, newHeight);

            // Optionally, if you need to adjust the position as well (if it's not anchored at the bottom), you might need to adjust its position too.
            // This is not needed if your RectTransform is anchored at the bottom.
        }
        else
        {
            Debug.LogWarning("ManaBar RectTransform is not assigned.");
        }
    }
    */


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