using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float delayBeforeFall = 0.5f; // Time in seconds before the platform starts to fall
    [SerializeField] private float fallSpeed = 2f; // Speed of the fall

    private Rigidbody2D rb;
    private Vector3 originalPosition;
    private bool isFalling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position; // Store the original position of the platform
        rb.isKinematic = true; // Initially, the platform doesn't fall
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has walked on the platform
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(FallAfterDelay());
        }
    }

    private IEnumerator FallAfterDelay()
    {
        isFalling = true;
        yield return new WaitForSeconds(delayBeforeFall);

        rb.isKinematic = false; // Platform starts falling
        rb.velocity = new Vector2(0, -fallSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the platform has collided with the designated trigger for resetting
        if (other.CompareTag("ResetTrigger") && isFalling)
        {
            ResetPlatform();
        }
    }

    private void ResetPlatform()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.position = originalPosition;
        isFalling = false; // The platform can now fall again if the player touches it
    }
}
