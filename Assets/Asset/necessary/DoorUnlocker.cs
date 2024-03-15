using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlocker : MonoBehaviour
{
    public CollectibleType requiredItemType;
    public int requiredItemCount = 1;
    public float moveUpDistance = 5f; // Distance to move up
    public float moveSpeed = 2f; // Speed of the move

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCollect playerCollect = collision.gameObject.GetComponent<PlayerCollect>();
            if (playerCollect != null && playerCollect.HasItems(requiredItemType, requiredItemCount))
            {
                playerCollect.UseItems(requiredItemType, requiredItemCount);
                StartCoroutine(UnlockDoor()); // Use coroutine to unlock the door
            }
        }
    }

    private IEnumerator UnlockDoor()
    {
        // Implement the logic to unlock the door here
        Debug.Log("Door Unlocked!");

        Vector3 endPosition = transform.position + new Vector3(0, moveUpDistance, 0); // Calculate the end position
        while (Vector3.Distance(transform.position, endPosition) > 0.01f)
        {
            // Move the door upwards until it reaches the end position
            transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for a frame before continuing the loop
        }

        // Optionally, disable the door collider to prevent further interaction
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}
