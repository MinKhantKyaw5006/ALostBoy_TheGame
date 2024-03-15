using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingShootingEnemy : Enemy // Derive from the Enemy class
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lineOfSite;
    [SerializeField] private float shootingRange;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletPos;
    [SerializeField] private float enemyScale;

    private Transform player;
    private Transform enemyTransform;
    private float nextFireTime;

    protected override void Start()
    {
        base.Start(); // Call the base Start method from the Enemy class
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyTransform = transform;
    }

    protected override void Update()
    {
        base.Update(); // Call the base Update method from the Enemy class
        FollowingEnemy();
        CheckHealth(); // Check health for destruction
    }

    private void FollowingEnemy()
    {
        if (player == null) return; // If player is null, exit early

        float distanceFromPlayer = Vector2.Distance(player.position, enemyTransform.position);

        if (distanceFromPlayer < lineOfSite && distanceFromPlayer > shootingRange)
        {
            // Move towards the player
            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, player.position, moveSpeed * Time.deltaTime);

            // Determine the direction of movement
            float xAxis = player.position.x - enemyTransform.position.x;
            bool movingRight = xAxis > 0;

            // Flip the enemy object based on movement direction
            FlipObject(movingRight);
        }
        else if (distanceFromPlayer <= shootingRange && Time.time >= nextFireTime)
        {
            // Shoot at the player
            Instantiate(bullet, bulletPos.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FlipObject(bool shouldFlip)
    {
        // Flip the object's scale based on the direction
        float newScale = shouldFlip ? -enemyScale : enemyScale;
        enemyTransform.localScale = new Vector3(newScale, enemyTransform.localScale.y, enemyTransform.localScale.z);
    }



    private void OnDrawGizmosSelected()
    {
        if (enemyTransform == null) return; // Ensure enemyTransform is not null

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(enemyTransform.position, lineOfSite);
        Gizmos.DrawWireSphere(enemyTransform.position, shootingRange);
    }
}
