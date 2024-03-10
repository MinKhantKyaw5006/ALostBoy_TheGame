using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Set the flag to reset game data on the next level
            if (DataPersistenceManager.instance != null)
            {
                DataPersistenceManager.instance.shouldReset = true;
                // Optionally call NewGame() if you want to reset data immediately
                // DataPersistenceManager.instance.NewGame();
            }
            else
            {
                Debug.LogError("DataPersistenceManager instance not found.");
            }

            // Go to next level
            SceneController.instance.NextLevel();
        }
    }
}
