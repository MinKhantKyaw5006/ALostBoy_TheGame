using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorUnlocker : MonoBehaviour
{
    public CollectibleType requiredItemType;
    public int requiredItemCount = 1;
    public float moveUpDistance = 5f; // Distance to move up
    public float moveSpeed = 2f; // Speed of the move
    [SerializeField] private AudioSource unlockSound;
    public GameObject requirementPanel; // Assign this in the Inspector
    public TextMeshProUGUI messageUI; // Use Text if you're not using TextMeshPro
    [SerializeField] private GameObject collectiblePrefab; // The prefab of the collectible to spawn
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points for the collectibles
    private bool hasSpawnedCollectibles = false; // To ensure collectibles are spawned only once
    private bool isDoorUnlocked = false; // Add this line to declare the variable
    private bool isUnlocking = false; // Indicates that the unlocking process has started



    /*
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
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCollect playerCollect = collision.gameObject.GetComponent<PlayerCollect>();
            if (playerCollect != null)
            {
                // Check if the door is already unlocked
                if (isDoorUnlocked || isUnlocking)
                {
                    // If the door is unlocked, do nothing or perform another action
                    return; // Exit the method early
                }

                if (playerCollect.HasItems(requiredItemType, requiredItemCount))
                {
                    playerCollect.UseItems(requiredItemType, requiredItemCount);
                    StartCoroutine(UnlockDoor());
                    isUnlocking = true; // Add this line to indicate that unlocking has started
                }
                else
                {
                    int currentItemCount = playerCollect.GetItemCount(requiredItemType);
                    if (currentItemCount == 0 && !hasSpawnedCollectibles)
                    {
                        SpawnCollectibles();
                        hasSpawnedCollectibles = true;
                    }

                    int itemsNeeded = requiredItemCount - currentItemCount;
                    messageUI.text = currentItemCount == 0 ?
                        $"You need {requiredItemCount} {requiredItemType}s to unlock this door." :
                        $"You have {currentItemCount}. You need {itemsNeeded} more {requiredItemType}s to unlock this door.";
                    requirementPanel.SetActive(true);
                    StartCoroutine(HideMessageAfterDelay(3f));
                }
            }
        }
    }

    private void SpawnCollectibles()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(collectiblePrefab, spawnPoint.position, Quaternion.identity);
        }
    }



    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        requirementPanel.SetActive(false);
    }




    private IEnumerator UnlockDoor()
    {
        unlockSound.Play();
        // Implement the logic to unlock the door here
        Debug.Log("Door Unlocked!");

        Vector3 endPosition = transform.position + new Vector3(0, moveUpDistance, 0); // Calculate the end position
        while (Vector3.Distance(transform.position, endPosition) > 0.01f)
        {
            // Move the door upwards until it reaches the end position
            transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for a frame before continuing the loop
        }

        // Uncomment and correctly position this line
        isDoorUnlocked = true; // Set the door as unlocked 
        isUnlocking = true; // Add this line to indicate that unlocking has started
        // Optionally, disable the door collider to prevent further interaction
        /*
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        isDoorUnlocked = true; // Set the door as unlocked
        */
    }
}
