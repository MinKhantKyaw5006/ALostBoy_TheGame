/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    // Define a delegate and event for the enemy's death
    // Event to notify when the enemy dies
    public event Action<GameObject> OnEnemyDeath;

    // Static reference to the instance
    private PlayerController playerController;

    // Existing enemy states
    protected enum EnemyStates
    {   //crawler
        Idle, // Added Idle state
        Crawler_Idle,
        Crawler_Flip,
        Following, // Added Following state for when the zombie detects the player

        
        //bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

    }

    protected EnemyStates currentEnemyState = EnemyStates.Idle;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = PlayerController.Instance;

        playerController = FindObjectOfType<PlayerController>(); // Find the player controller instance in the scene
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }
        else
        {
            Debug.Log("Player Found by Enemy");
        }
    }

    protected virtual void Update()
    {
        UpdateEnemyStates();

        CheckHealth(); // Check health for destruction

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    protected void CheckHealth()
    {
        if (health <= 0)
        {
            OnEnemyDeath?.Invoke(gameObject); // Invoke the death event
            Destroy(gameObject);
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);

            // Trigger screen shake
            ScreenShaker screenShaker = FindObjectOfType<ScreenShaker>(); // Consider using a more efficient way to reference this
            if (screenShaker != null)
            {
                screenShaker.Shake(_hitDirection.normalized); // You might adjust the force
            }
            else
            {
                Debug.LogWarning("ScreenShaker not found in the scene.");
            }

        }
    }
 

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the collided object has a PlayerController component
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Call the TakeDamage method of the PlayerController to damage the player
                playerController.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on the collided object.");
            }
        }
    }

    protected virtual void UpdateEnemyStates() { }

    protected void ChangeState(EnemyStates _newState)
    {
        currentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}

*/
//original above <<<<



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] public float speed;
    [SerializeField] public float damage;
    // Define maxDamagePerHit
    [SerializeField] protected float maxDamagePerHit = 200f; // Set your desired maximum damage here

    protected float recoilTimer;
    public Rigidbody2D rb;
    protected SpriteRenderer sr;
    public Animator anim;

    // Define a delegate and event for the enemy's death
    public event Action<GameObject> OnEnemyDeath;

    // Static reference to the instance
    private PlayerController playerController;

    // Existing enemy states
    protected enum EnemyStates
    {
        // crawler
        Idle,
        Crawler_Idle,
        Crawler_Flip,
        Following,

        // bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        // boss1
        BossFight_Stage1,
        BossFight_Stage2,
        BossFight_Stage3,
        BossFight_Stage4
    }

    protected EnemyStates currentEnemyState = EnemyStates.Idle;

    // Property to get and set the current enemy state
    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerController.Instance;

        playerController = FindObjectOfType<PlayerController>(); // Find the player controller instance in the scene
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
        }
        else
        {
            Debug.Log("Player Found by Enemy");
        }
    }

    protected virtual void Update()
    {
        UpdateEnemyStates();
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    protected virtual void CheckHealth()
    {
        if (health <= 0)
        {
            health = 0; // Ensure health doesn't go below zero

            OnEnemyDeath?.Invoke(gameObject); // Invoke the death event
            Death(0f); // Call the virtual Death method
        }
    }

    protected virtual void Death(float destroyTime)
    {
        Destroy(gameObject, destroyTime); // Example implementation: destroy the enemy after a delay
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);

            // Trigger screen shake
            ScreenShaker screenShaker = FindObjectOfType<ScreenShaker>(); // Consider using a more efficient way to reference this
            if (screenShaker != null)
            {
                screenShaker.Shake(_hitDirection.normalized); // You might adjust the force
            }
            else
            {
                Debug.LogWarning("ScreenShaker not found in the scene.");
            }
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on the collided object.");
            }
        }
    }

    protected virtual void UpdateEnemyStates() { }

    protected virtual void ChangeCurrentAnimation()
    {
        // Override this method to implement animation changes based on the enemy's state
    }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState; // Use the property to change state
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
