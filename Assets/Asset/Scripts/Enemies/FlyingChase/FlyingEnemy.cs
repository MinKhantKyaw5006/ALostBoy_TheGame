using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float speed;
    public Transform startingPoint;
    private GameObject player;
    private bool chase = false; // Start with chasing disabled

    // Property to get or set the chase field
    public bool Chase
    {
        get { return chase; }
        set { chase = value; }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        if (chase)
        {
            ChasePlayer();
        }
        else
        {
            ReturnToStartPoint();
        }

        FlipTowardsPlayer();
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void ReturnToStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
    }

    private void FlipTowardsPlayer()
    {
        if (player != null)
        {
            if (transform.position.x > player.transform.position.x)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
    // Optionally override any Enemy methods with specific behaviors for the flying enemy
