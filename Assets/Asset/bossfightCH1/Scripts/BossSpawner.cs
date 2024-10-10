using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // Reference to the Boss prefab
    public GameObject bossPrefab;

    // Spawn location for the boss
    public Transform spawnPoint;

    // Flag to check if the boss has been spawned
    private bool bossSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player enters the trigger zone
        if (collision.CompareTag("Player") && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    // Method to spawn the boss
    void SpawnBoss()
    {
        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        bossSpawned = true; // Ensure the boss is spawned only once
    }
}
