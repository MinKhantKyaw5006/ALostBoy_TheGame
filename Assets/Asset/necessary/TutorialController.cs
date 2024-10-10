using System.Collections;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class TutorialController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;  // Reference to PlayerController (optional)
    [SerializeField] private TextMeshProUGUI tutorialText;       // TextMeshProUGUI element for tutorial messages
    [SerializeField] private string[] tutorialSteps;             // Array of tutorial messages
    [SerializeField] private GameObject tutorialPanel;           // Panel that contains the tutorial UI (optional)

    private int currentStep = 0;   // To track the current step in the tutorial

    private void Awake()
    {
        // Ensure the tutorial panel and text are active
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        ShowCurrentStep();
    }

    private void Update()
    {
        // Check for specific input depending on the current step
        switch (currentStep)
        {
            case 0:  // Move left (A key or joystick left)
                if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Horizontal") < -0.1f)
                {
                    NextStep();
                }
                break;

            case 1:  // Move right (D key or joystick right)
                if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Horizontal") > 0)
                {
                    NextStep();
                }
                break;

            case 2:  // Jump (Space or joystick button down, usually "X")
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
                {
                    NextStep();
                }
                break;

            case 3:  // Dash (F key or joystick button right)
                if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Dash")) // Make sure "Dash" is defined in Input Manager
                {
                    NextStep();
                }
                break;

                // Add more cases for additional tutorial steps if needed
        }
    }

    // Show the current tutorial step's message
    private void ShowCurrentStep()
    {
        if (currentStep < tutorialSteps.Length)
        {
            tutorialText.text = tutorialSteps[currentStep];
        }
        else
        {
            EndTutorial();
        }
    }

    // Go to the next step in the tutorial
    private void NextStep()
    {
        currentStep++;
        ShowCurrentStep();
    }

    // End the tutorial and enable player controls
    private void EndTutorial()
    {
        tutorialText.text = "";  // Clear the tutorial message
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);  // Hide the tutorial UI
        }

        playerController.EnableControls();  // Re-enable player controls
    }
}
