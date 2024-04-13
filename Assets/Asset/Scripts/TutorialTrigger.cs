using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public int tutorialID; // Unique ID for each tutorial trigger
    public TutorialManager tutorialManager; // Reference to the TutorialManager component

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var gameData = DataPersistenceManager.instance.GameData;

            Debug.Log($"Checking Tutorial ID: {tutorialID}. Completed IDs: {string.Join(", ", gameData.completedTutorialIDs)}");

            if (!gameData.completedTutorialIDs.Contains(tutorialID))
            {
                gameData.completedTutorialIDs.Add(tutorialID);
                DataPersistenceManager.instance.SaveGame();
                tutorialManager.BeginTutorial(tutorialID); // Adjusted to pass tutorialID
                Debug.Log($"Tutorial {tutorialID} started.");
            }
            else
            {
                Debug.Log($"Tutorial {tutorialID} already completed. Skipping.");
            }
        }
    }
}
