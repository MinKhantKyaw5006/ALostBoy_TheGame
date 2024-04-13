using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex = 0;
    private PlayerInput playerInput;
    public int tutorialID; // Assign this in the inspector or through script when initializing the tutorial

    void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (ShouldShowPopUps())
        {
            switch (popUpIndex)
            {
                case 0:
                    if (Keyboard.current.aKey.wasPressedThisFrame || LeftStickMovedLeft())
                    {
                        Debug.Log("Moved Left, showing next tutorial.");
                        AdvanceTutorial();
                    }
                    break;
                case 1:
                    if (Keyboard.current.dKey.wasPressedThisFrame || LeftStickMovedRight())
                    {
                        Debug.Log("Moved Right, showing next tutorial.");
                        AdvanceTutorial();
                    }
                    break;
                case 2:
                    if (Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        Debug.Log("Jumped, showing next tutorial.");
                        AdvanceTutorial();
                    }
                    break;
                    // Additional cases as needed
            }
            // After switch case
            Debug.Log($"Current Tutorial Step: {popUpIndex}");
        }
    }

  
    private bool LeftStickMovedLeft()
    {
        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        return move.x < 0;
    }

    private bool LeftStickMovedRight()
    {
        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        return move.x > 0;
    }

    private void AdvanceTutorial()
    {
        popUpIndex++;
        ShowPopUp(popUpIndex);
    }



    private void ShowPopUp(int index)
    {
        Debug.Log($"Showing Pop Up for index: {index}");
        foreach (GameObject popUp in popUps)
        {
            popUp.SetActive(false);
        }
        if (index < popUps.Length)
        {
            popUps[index].SetActive(true);
        }
        else
        {
            Debug.Log("Tutorial Complete!");
            MarkTutorialAsCompleted();
        }
    }



    public void BeginTutorial(int id)
    {
        tutorialID = id;
        if (!DataPersistenceManager.instance.GameData.completedTutorialIDs.Contains(tutorialID))
        {
            popUpIndex = 0; // Ensure we start from the first tutorial message
            ShowPopUp(popUpIndex); // Show the first tutorial pop-up
            Debug.Log($"BeginTutorial called with ID: {id}. Tutorial started.");
        }
        else
        {
            Debug.Log($"Tutorial {id} has already been completed. Skipping.");
        }
    }


    private bool ShouldShowPopUps()
    {
        bool shouldShow = !DataPersistenceManager.instance.GameData.completedTutorialIDs.Contains(tutorialID);
        Debug.Log($"ShouldShowPopUps: {shouldShow} for tutorial ID: {tutorialID}");
        return shouldShow;
    }


    private void MarkTutorialAsCompleted()
    {
        // This method should be called only when the tutorial is fully completed.
        if (!DataPersistenceManager.instance.GameData.completedTutorialIDs.Contains(tutorialID))
        {
            DataPersistenceManager.instance.GameData.completedTutorialIDs.Add(tutorialID);
            DataPersistenceManager.instance.SaveGame(); // Ensure to save after marking as completed
            Debug.Log($"Tutorial {tutorialID} marked as completed.");
        }
    }
}
