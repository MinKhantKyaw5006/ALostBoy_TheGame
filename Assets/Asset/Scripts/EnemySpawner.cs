/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnLocations; // Array of spawn locations
    [SerializeField] private int maxEnemies = 10; // Maximum number of enemies that can be spawned
    [SerializeField] private float cooldownTimeMinutes = 5f; // Cooldown time in minutes
    [SerializeField] private bool randomSpawn = true; // Flag to enable random spawning
    [SerializeField] private bool respawnAfterCooldown = true; // Flag to enable respawning after cooldown
    [SerializeField] private AttackableDoor door; // Reference to the door

    private bool canSpawn = false;
    private int currentEnemyCount = 0;
    private bool cooldownActive = false;
    private int currentSpawnIndex = 0;
    private float CooldownTimeSeconds => cooldownTimeMinutes * 60f; // Cooldown time in seconds

    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List to track spawned enemies

    public event System.Action OnAllEnemiesDefeated; // Event to notify when all enemies are defeated

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !cooldownActive)
        {
            Debug.Log("Player entered spawner trigger.");
            canSpawn = true;
            StartCoroutine(Spawner());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited spawner trigger.");
            canSpawn = false;
            StopCoroutine(Spawner());
        }
    }

    private IEnumerator Spawner()
    {
        Debug.Log("Spawner coroutine started.");
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn && currentEnemyCount < maxEnemies)
        {
            yield return wait;

            int randPrefabIndex = randomSpawn ? UnityEngine.Random.Range(0, enemyPrefabs.Length) : 0;

            GameObject enemyToSpawn = enemyPrefabs[randPrefabIndex];
            Transform spawnLocation = GetNextSpawnLocation();

            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity);
            spawnedEnemies.Add(spawnedEnemy); // Track spawned enemy
            spawnedEnemy.GetComponent<Enemy>().OnEnemyDeath += OnEnemyDeath; // Subscribe to the enemy's death event

            currentEnemyCount++;
            Debug.Log("Enemy spawned. Current enemy count: " + currentEnemyCount);
        }

        if (currentEnemyCount >= maxEnemies && respawnAfterCooldown)
        {
            Debug.Log("Max enemy count reached. Cooling down.");
            cooldownActive = true;
            yield return new WaitForSeconds(CooldownTimeSeconds);
            cooldownActive = false;
            currentEnemyCount = 0; // Reset enemy count after cooldown
            currentSpawnIndex = 0; // Reset spawn index after cooldown
            Debug.Log("Cooldown finished. Resuming spawning.");
            StartCoroutine(Spawner()); // Restart the spawner after cooldown
        }
        else if (!respawnAfterCooldown)
        {
            Debug.Log("Max enemy count reached. Spawner will not respawn.");
        }
    }

    private Transform GetNextSpawnLocation()
    {
        Transform spawnLocation = spawnLocations[currentSpawnIndex];
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnLocations.Length;
        return spawnLocation;
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        currentEnemyCount--;
        Debug.Log("Enemy died. Remaining enemy count: " + currentEnemyCount);

        if (spawnedEnemies.Count == 0 && !respawnAfterCooldown)
        {
            OnAllEnemiesDefeated?.Invoke(); // Notify that all enemies are defeated
            Destroy(gameObject); // Destroy the spawner
            Debug.Log("All enemies defeated. Destroying spawner.");
        }
    }

    // Function to check if random spawning is enabled
    public bool IsRandomSpawnEnabled()
    {
        return randomSpawn;
    }

    // Function to stop spawning enemies
    public void StopSpawning()
    {
        StopAllCoroutines();
        canSpawn = false;
        cooldownActive = false;
        Debug.Log("Spawning stopped.");
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float cooldownTimeMinutes = 5f;
    [SerializeField] private bool randomSpawn = true;
    [SerializeField] private bool respawnAfterCooldown = true;
    [SerializeField] private bool isForAttackableDoor = false;

    private bool canSpawn = false;
    private int currentEnemyCount = 0;
    private bool cooldownActive = false;
    private int currentSpawnIndex = 0;
    private float CooldownTimeSeconds => cooldownTimeMinutes * 60f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public event System.Action OnAllEnemiesDefeated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !cooldownActive && currentEnemyCount == 0)
        {
            canSpawn = true;
            StartCoroutine(Spawner());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = false;
            StopCoroutine(Spawner());
        }
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn && (isForAttackableDoor || currentEnemyCount < maxEnemies))
        {
            if (currentEnemyCount >= maxEnemies && !isForAttackableDoor)
            {
                yield break;
            }

            yield return wait;

            int randPrefabIndex = randomSpawn ? UnityEngine.Random.Range(0, enemyPrefabs.Length) : 0;
            GameObject enemyToSpawn = enemyPrefabs[randPrefabIndex];
            Transform spawnLocation = GetNextSpawnLocation();
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity);
            spawnedEnemies.Add(spawnedEnemy);
            spawnedEnemy.GetComponent<Enemy>().OnEnemyDeath += OnEnemyDeath;
            currentEnemyCount++;
        }

        if (isForAttackableDoor && respawnAfterCooldown)
        {
            cooldownActive = true;
            yield return new WaitForSeconds(CooldownTimeSeconds);
            cooldownActive = false;
            StartCoroutine(Spawner());
        }
    }

    private Transform GetNextSpawnLocation()
    {
        Transform spawnLocation = spawnLocations[currentSpawnIndex];
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnLocations.Length;
        return spawnLocation;
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        currentEnemyCount--;

        if (spawnedEnemies.Count == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    public void StopSpawning()
    {
        canSpawn = false;
        StopAllCoroutines();
    }
}
