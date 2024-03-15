using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Amount of damage to deal on fall
    private SaveGroundCP safeGroundCheckPointSaver;


    private void Start()
    {
        safeGroundCheckPointSaver = GameObject.FindGameObjectWithTag("Player").GetComponent<SaveGroundCP>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try to get the PlayerController component on the collided object
            PlayerController playerHealth = collision.GetComponent<PlayerController>();

            // If the component is found, call the TakeDamage method with isFallDamage set to true
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); // Specify true for isFallDamage parameter

                //warp the player to the saveground checkpoint
                safeGroundCheckPointSaver.WarpPlayerToSafeGround();
            }
        }
    }

}
