using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    
    public int checkpointID; // Unique identifier for the checkpoint

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Checkpoint with ID " + checkpointID + " triggered.");

            var checkpointIDs = DataPersistenceManager.instance.GameData.checkpointIDs;

            Debug.Log("Checking from the list: " + string.Join(", ", checkpointIDs));

            if (!checkpointIDs.Contains(checkpointID))
            {
                Debug.Log("No current checkpoint in the list. Adding checkpoint ID " + checkpointID);
                checkpointIDs.Add(checkpointID);
                DataPersistenceManager.instance.SaveGame();
            }
            else
            {
                Debug.Log("Current checkpoint already exists in the list. Not saving.");
            }
        }
    }
}
