using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true; // Prevent the checkpoint from being activated again
            // Call your game manager's save method
            FindObjectOfType<GameManager>().SaveGame();
            // Provide visual feedback or play a sound to indicate checkpoint activation
            Debug.Log("Checkpoint reached!");
        }
    }
}
