using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : Enemy
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float shootingDistance = 4f;
    public float shootingCooldown = 2f;
    private float shootingTimer;
    public Transform relocationPoint; // Assign this in the Inspector
    private bool shouldReturnToStart = false;

    protected override void Update()
    {
        base.Update(); // Call the base enemy update

        HandleShooting();
        FlipBasedOnPlayerPosition();
    }

    void HandleShooting()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float verticalDifference = Mathf.Abs(transform.position.y - player.transform.position.y);

        // Define a maximum allowed vertical difference
        float maxVerticalDifference = 1f; // Adjust this value as needed

        if (distanceToPlayer < shootingDistance && verticalDifference <= maxVerticalDifference)
        {
            shootingTimer += Time.deltaTime;

            if (shootingTimer >= shootingCooldown)
            {
                shootingTimer = 0;
                Shoot();
            }
        }
    }


    void Shoot()
    {
        // Calculate direction from enemy to player
        Vector3 directionToPlayer = (player.transform.position - bulletSpawnPoint.position).normalized;

        // Calculate rotation towards the player
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Instantiate the bullet and rotate it towards the player
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0, 0, angle - 90)); // Adjust the angle offset if necessary

        // If you want to flip the bullet sprite based on the direction, you might need additional logic here.
    }


    void FlipBasedOnPlayerPosition()
    {
        // Assuming the player is not null and has been defined in the Enemy base class
        if (player != null)
        {
            // Determine if the player is on the left or right side of the enemy
            bool isPlayerToLeft = player.transform.position.x < transform.position.x;

            // If the player is to the left, then localScale should be flipped, otherwise, it should be normal.
            // This assumes that the enemy's original facing direction is to the right.
            Vector3 flipped = transform.localScale;
            flipped.x = isPlayerToLeft ? -Mathf.Abs(flipped.x) : Mathf.Abs(flipped.x);
            transform.localScale = flipped;
        }
    }
}
