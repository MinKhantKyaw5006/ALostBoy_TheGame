using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Include this for the new Input System

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public GameObject continueButton;
    public PlayerController playerController;
    private InputAction advanceDialogueAction; // Define an InputAction for advancing dialogue
    public bool isRepeatable = true; // True if the dialogue can trigger multiple times, false if only once
    private bool hasTriggered = false; // Tracks if the dialogue has already been triggered



    private void Awake() // Use Awake for initialization
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Initialize the InputAction
        advanceDialogueAction = new InputAction(binding: "<Gamepad>/buttonSouth");
        advanceDialogueAction.Enable();
        advanceDialogueAction.performed += ctx => AdvanceDialogue(); // Subscribe to the performed event
    }

    private void Update()
    {
        if (textDisplay.text == sentences[index])
        {
            continueButton.SetActive(true);
        }
    }

    IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        AdvanceDialogue();
    }

    private void AdvanceDialogue() // Refactored method to advance dialogue
    {
        continueButton.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            continueButton.SetActive(false);
            playerController.isInDialogue = false; // Re-enable player actions
                                                   // Here, disable jumping for a short duration after the dialogue ends
            if (!isRepeatable)
            {
                this.enabled = false; // Optionally disable this script if the dialogue should only occur once
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && (isRepeatable || !hasTriggered))
        {
            StartDialogue();
            hasTriggered = true; // Mark as triggered to prevent re-triggering if not repeatable
        }
    }

    /*
    public void ResetDialogue()
    {
        hasTriggered = false; // Allow the dialogue to be triggered again
    }
    */



    public void StartDialogue()
    {
        playerController.isInDialogue = true;
        index = 0;
        textDisplay.text = "";
        StartCoroutine(Type());
    }

    private void OnEnable()
    {
        advanceDialogueAction.Enable();
    }

    private void OnDisable()
    {
        advanceDialogueAction.Disable();
    }
}
