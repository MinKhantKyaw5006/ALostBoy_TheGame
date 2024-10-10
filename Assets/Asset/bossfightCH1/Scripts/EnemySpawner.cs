//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemySpawner : MonoBehaviour
//{
//    public static EnemySpawner Instance; 
//    [SerializeField] private float spawnRate = 1f;
//    [SerializeField] private GameObject[] enemyPrefabs;
//    [SerializeField] private Transform[] spawnLocations; // Array of spawn locations
//    [SerializeField] private int maxEnemies = 10; // Maximum number of enemies that can be spawned
//    [SerializeField] private float cooldownTimeMinutes = 5f; // Cooldown time in minutes
//    [SerializeField] private bool randomSpawn = true; // Flag to enable random spawning
//    private bool canSpawn = false;
//    private int currentEnemyCount = 0;
//    private bool cooldownActive = false;
//    private int currentSpawnIndex = 0;

//    private BoxCollider2D col; 

//    private float CooldownTimeSeconds => cooldownTimeMinutes * 60f; // Cooldown time in seconds

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//        }
//        else
//        {
//            Instance = this;
//        }
//    }

//    private void Start()
//    {
//        col = GetComponent<BoxCollider2D>();
//    }
//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") && !cooldownActive)
//        {
//            Debug.Log("Player entered spawner trigger.");
//            canSpawn = true;
//            StartCoroutine(Spawner());
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            Debug.Log("Player exited spawner trigger.");
//            canSpawn = false;
//            StopCoroutine(Spawner());
//        }
//    }

//    private IEnumerator Spawner()
//    {
//        Debug.Log("Spawner coroutine started.");
//        WaitForSeconds wait = new WaitForSeconds(spawnRate);

//        while (canSpawn && currentEnemyCount < maxEnemies)
//        {
//            yield return wait;

//            int randPrefabIndex = randomSpawn ? UnityEngine.Random.Range(0, enemyPrefabs.Length) : 0;

//            GameObject enemyToSpawn = enemyPrefabs[randPrefabIndex];
//            Transform spawnLocation = GetNextSpawnLocation();

//            Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity);
//            currentEnemyCount++;
//            Debug.Log("Enemy spawned. Current enemy count: " + currentEnemyCount);
//        }

//        // Activate cooldown
//        if (currentEnemyCount >= maxEnemies)
//        {
//            Debug.Log("Max enemy count reached. Cooling down.");
//            cooldownActive = true;
//            yield return new WaitForSeconds(CooldownTimeSeconds);
//            cooldownActive = false;
//            currentEnemyCount = 0; // Reset enemy count after cooldown
//            currentSpawnIndex = 0; // Reset spawn index after cooldown
//            Debug.Log("Cooldown finished. Resuming spawning.");
//        }
//    }

//    private Transform GetNextSpawnLocation()
//    {
//        Transform spawnLocation = spawnLocations[currentSpawnIndex];
//        currentSpawnIndex = (currentSpawnIndex + 1) % spawnLocations.Length;
//        return spawnLocation;
//    }

//    // Function to check if random spawning is enabled
//    public bool IsRandomSpawnEnabled()
//    {
//        return randomSpawn;
//    }

//    public void IsNotTrigger()
//    {
//        col.isTrigger = true; 
//    }
//}
