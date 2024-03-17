using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoost : MonoBehaviour
{
    public enum BoostType
    {
        Vertical,
        Horizontal
    }

    [SerializeField] private BoostType boostType = BoostType.Vertical;
    [SerializeField] private float boostStrength = 10f;
    [SerializeField] private float boostDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                StartCoroutine(ApplyBoost(playerRb));
            }
        }
    }


    private IEnumerator ApplyBoost(Rigidbody2D playerRb)
    {
        Vector2 originalGravity = Physics2D.gravity;
        Physics2D.gravity = Vector2.zero; // Temporarily disable gravity
        Vector2 boostDirection = boostType == BoostType.Vertical ? Vector2.up : Vector2.right;

        playerRb.velocity = boostDirection * boostStrength; // Apply initial boost

        yield return new WaitForSeconds(boostDuration);

        Physics2D.gravity = originalGravity; // Re-enable gravity
        // Optionally, you can smoothly decrease the player's velocity here instead of a sudden stop
    }
}
