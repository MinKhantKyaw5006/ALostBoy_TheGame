using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTrap : MonoBehaviour
{
    [SerializeField] private float shootingRange;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletPos;

    private Transform player;
    private float nextFireTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return; // If player is null, exit early

        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer <= shootingRange && Time.time >= nextFireTime)
        {
            // Shoot at the player
            Instantiate(bullet, bulletPos.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
