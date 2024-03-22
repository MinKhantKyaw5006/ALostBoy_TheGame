using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Enemy
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private DamageFlash damageFlash;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Transform currentTarget;

    protected override void Start()
    {
        base.Start();
        currentTarget = pointA;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Initialize the animator reference
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (isRecoiling)
        {
            // Handle recoiling in the base update
            return;
        }

        PatrolBetweenPoints();
    }

    private void PatrolBetweenPoints()
    {
        // Move towards the current target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);

        // Check if the enemy has reached the current target
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            // If the current target is pointA, switch to pointB, and vice versa
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }

        // Determine the direction of movement to flip the enemy accordingly
        bool movingRight = (currentTarget.position.x > transform.position.x);
        FlipObject(movingRight);
    }

    private void FlipObject(bool shouldFlip)
    {
        if (shouldFlip != (transform.localScale.x > 0))
        {
            // Flip the object's scale based on the direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce); // Call the base method to handle health reduction and recoil

        // Check if health has dropped to zero or below, then handle death
        if (health <= 0)
        {
            Die(); // Handle the enemy's death here
        }

        // Trigger animations or effects specific to RotatingEnemy
        if (animator != null)
        {
            animator.SetTrigger("Hit"); // Assuming there's a "Hit" trigger in your animator
        }

        if (damageFlash != null)
        {
            damageFlash.Flash(); // Trigger the flash effect
        }

        // No need to handle the screen shake here if it's done in the base class
    }

    private void Die()
    {
        // Logic for handling the enemy's death specific to RotatingEnemy
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
        // Optionally, trigger any death animations or effects before destroying the object
    }




}
