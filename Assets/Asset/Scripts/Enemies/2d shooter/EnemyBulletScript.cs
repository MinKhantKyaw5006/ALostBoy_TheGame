using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] private float damage = 20f; // Serialized field for bullet damage
    private Rigidbody2D rb;
    public bool canBeDeflected = true; // Add this line to enable deflection control

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetDirectionTowardsPlayer();
    }

    void Update()
    {
        Destroy(gameObject, 10f); // Optionally, destroy the bullet after some time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage); // Use the serialized damage variable
            }
            Destroy(gameObject);
        }
        else if (canBeDeflected && collision.gameObject.CompareTag("PlayerAttack")) // Assuming "PlayerAttack" is the tag for player's attack
        {
            DeflectBullet();
        }
    }

    void SetDirectionTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
    }

    void DeflectBullet()
    {
        // Invert bullet direction towards the source enemy
        rb.velocity = -rb.velocity;
        // Optionally adjust the rotation of the bullet to match the new direction
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + 180);
    }

}
