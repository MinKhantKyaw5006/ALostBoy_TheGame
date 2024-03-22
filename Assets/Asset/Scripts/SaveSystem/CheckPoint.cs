using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private AudioSource checkpointSound;
    [SerializeField] private TextMeshProUGUI checkpointSavedText;


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
                Debug.Log("Game Saved at Checkpoint" + checkpointID);
                // Now show the checkpoint saved message
                StartCoroutine(ShowCheckpointSavedMessage());
                checkpointSound.Play();
            }
            else
            {
                Debug.Log("Current checkpoint already exists in the list. Not saving.");
            }
        }
    }


    private IEnumerator ShowCheckpointSavedMessage()
    {
        checkpointSavedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3); // Display the message for 3 seconds
        checkpointSavedText.gameObject.SetActive(false);
    }
}
