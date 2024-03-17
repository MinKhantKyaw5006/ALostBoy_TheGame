using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Enemy
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;

    private Transform currentTarget;

    protected override void Start()
    {
        base.Start();
        currentTarget = pointA;
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
}
