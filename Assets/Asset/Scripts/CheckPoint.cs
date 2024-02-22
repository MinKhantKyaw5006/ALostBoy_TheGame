/*using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkpointID; // Assign unique ID to each checkpoint in the inspector
    private GameMaster gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        // Disable collider if this checkpoint has already been activated
        if (gm.activatedCheckpoints.Contains(checkpointID))
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !gm.activatedCheckpoints.Contains(checkpointID))
        {
            gm.lastCheckpointPos = transform.position;
            gm.lastCheckpointID = checkpointID;
            gm.activatedCheckpoints.Add(checkpointID);
            GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent reactivation
            Debug.Log($"Activated Checkpoint: {checkpointID}");
        }
    }
}
*


public class CheckPoint : MonoBehaviour
{
    public int checkpointID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !GameMaster.activatedCheckpoints.Contains(checkpointID))
        {
            GameMaster.ActivateCheckpoint(checkpointID, transform.position);
            GameMaster.Instance.SaveGame(); // Save the game state after activating a checkpoint
        }
    }
}
*/


