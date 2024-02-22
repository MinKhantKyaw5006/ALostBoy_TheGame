/*using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerPos : MonoBehaviour
{
    private GameMaster gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        // Set player position to the last checkpoint position
        transform.position = gm.lastCheckpointPos;
    }

    void Update()
    {
        // Example logic for restarting at the checkpoint (if using a key press)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RestartAtLastCheckpoint();
        }
    }

    public void RestartAtLastCheckpoint()
    {
        // Reset player position to the last activated checkpoint
        transform.position = GameMaster.instance.lastCheckpointPos;
        // Optionally, if you're reloading the scene, ensure the GameMaster's lastCheckPointPos is used to set the player's start position
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log($"Restarting at Last Checkpoint: {GameMaster.instance.lastCheckpointID}");
    }
}
*/