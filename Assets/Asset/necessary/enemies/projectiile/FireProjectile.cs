using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private float fireSpeed;
    [SerializeField] private float damageAmount; // Amount of damage the projectile inflicts
    private GameObject target;
    private Rigidbody2D bulletRb;
    [SerializeField] private float lifespan = 5f; // Lifespan of the projectile in seconds


    // Start is called before the first frame update
    void Start()
    {
        bulletRb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDir = (target.transform.position - transform.position).normalized * fireSpeed;
        bulletRb.velocity = new Vector2(moveDir.x, moveDir.y);

        Destroy(gameObject, lifespan); // Destroy the projectile after 'lifespan' seconds
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collided with the player
        if (collision.CompareTag("Player"))
        {
            // Get the player's PlayerController component
            PlayerController playerController = collision.GetComponent<PlayerController>();

            // If playerController is not null, apply damage to the player
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }

            // Destroy the projectile on collision with the player
            Destroy(gameObject);
        }
    }


   
}
   
