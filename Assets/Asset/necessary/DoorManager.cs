using System.Collections;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject door;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;

    private bool isDoorOpen = false;

    private void Start()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
        }
        else
        {
            Debug.LogWarning("EnemySpawner not assigned in DoorManager.");
        }
    }

    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnAllEnemiesDefeated -= HandleAllEnemiesDefeated;  
        }
    }

    private void HandleAllEnemiesDefeated()
    {
        if (!isDoorOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isDoorOpen = true;
        Vector3 startPos = door.transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        float elapsedTime = 0f;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            door.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / (moveDistance / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        door.transform.position = endPos;
        Debug.Log("Door opened.");
    }
}
