
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Destroy(gameObject);
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
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

