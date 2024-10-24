//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class DialogueManager : MonoBehaviour
//{
//    public Image ActorImage;
//    public TextMeshProUGUI actorName;
//    public TextMeshProUGUI messageText;
//    public RectTransform backgroundBox;

//    private Message[] currentMessages;
//    private Actor[] currentActors;
//    private int activeMessage = 0;
//    public static bool isActive = false;

//    public float typingSpeed = 0.05f; // Speed of typing effect in seconds per character
//    private Coroutine typingCoroutine; // Store the reference to the typing coroutine

//    // New variable to enable or disable skipping
//    public bool canSkipDialogue = true;

//    void Update()
//    {
//        // Check for input if skipping is allowed and dialogue is active
//        if (canSkipDialogue && isActive && Input.anyKeyDown)
//        {
//            SkipCurrentMessage();
//        }
//    }

//    public void OpenDialogue(Message[] messages, Actor[] actors)
//    {
//        currentMessages = messages;
//        currentActors = actors;
//        activeMessage = 0;
//        isActive = true;
//        Debug.Log("Started conversation! Loaded messages: " + messages.Length);
//        backgroundBox.gameObject.SetActive(true); // Show the dialogue UI
//        StartCoroutine(DisplayMessageWithTypingEffect());
//    }

//    IEnumerator DisplayMessageWithTypingEffect()
//    {
//        while (isActive)
//        {
//            DisplayMessage();
//            yield return new WaitForSeconds(currentMessages[activeMessage].duration);
//            NextMessage();
//        }
//    }

//    void DisplayMessage()
//    {
//        if (activeMessage < currentMessages.Length)
//        {
//            Message messageToDisplay = currentMessages[activeMessage];
//            Actor actorToDisplay = currentActors[messageToDisplay.actorId];
//            actorName.text = actorToDisplay.name;
//            ActorImage.sprite = actorToDisplay.sprite;

//            // Stop any previous typing coroutine before starting a new one
//            if (typingCoroutine != null)
//            {
//                StopCoroutine(typingCoroutine);
//            }

//            typingCoroutine = StartCoroutine(TypeMessage(messageToDisplay.message));
//        }
//    }

//    IEnumerator TypeMessage(string message)
//    {
//        messageText.text = "";
//        foreach (char letter in message.ToCharArray())
//        {
//            messageText.text += letter;
//            yield return new WaitForSeconds(typingSpeed);
//        }
//    }

//    public void NextMessage()
//    {
//        activeMessage++;
//        if (activeMessage < currentMessages.Length)
//        {
//            StopAllCoroutines(); // Stop any running coroutines before starting a new message
//            StartCoroutine(DisplayMessageWithTypingEffect());
//        }
//        else
//        {
//            Debug.Log("Conversation ended");
//            isActive = false;
//            backgroundBox.gameObject.SetActive(false); // Hide the dialogue UI
//        }
//    }

//    // Updated function to skip the current message and go to the next one
//    public void SkipCurrentMessage()
//    {
//        // Stop the current typing coroutine
//        if (typingCoroutine != null)
//        {
//            StopCoroutine(typingCoroutine);
//        }

//        // Immediately jump to the next message
//        NextMessage();
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Image ActorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;

    private Message[] currentMessages;
    private Actor[] currentActors;
    private int activeMessage = 0;
    public static bool isActive = false;

    public float typingSpeed = 0.05f; // Speed of typing effect in seconds per character
    private Coroutine typingCoroutine; // Store the reference to the typing coroutine

    // New variable to enable or disable skipping
    public bool canSkipDialogue = true;

    void Update()
    {
        // Check for input if skipping is allowed and dialogue is active
        if (canSkipDialogue && isActive && Input.anyKeyDown)
        {
            SkipCurrentMessage();
        }
    }

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        Debug.Log("Started conversation! Loaded messages: " + messages.Length);
        backgroundBox.gameObject.SetActive(true); // Show the dialogue UI
        StartCoroutine(DisplayMessageWithTypingEffect());
    }

    IEnumerator DisplayMessageWithTypingEffect()
    {
        while (isActive)
        {
            DisplayMessage();
            yield return new WaitForSeconds(currentMessages[activeMessage].duration);
            NextMessage();
        }
    }

    void DisplayMessage()
    {
        if (activeMessage < currentMessages.Length)
        {
            Message messageToDisplay = currentMessages[activeMessage];
            Actor actorToDisplay = currentActors[messageToDisplay.actorId];
            actorName.text = actorToDisplay.name;
            ActorImage.sprite = actorToDisplay.sprite;

            // Stop any previous typing coroutine before starting a new one
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeMessage(messageToDisplay.message));
        }
    }

    IEnumerator TypeMessage(string message)
    {
        messageText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            StopAllCoroutines(); // Stop any running coroutines before starting a new message
            StartCoroutine(DisplayMessageWithTypingEffect());
        }
        else
        {
            Debug.Log("Conversation ended");
            isActive = false;
            backgroundBox.gameObject.SetActive(false); // Hide the dialogue UI
        }
    }

    // Updated function to skip the current message and go to the next one
    public void SkipCurrentMessage()
    {
        // Stop the current typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Immediately jump to the next message
        NextMessage();
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class DialogueManager : MonoBehaviour
//{
//    public Image ActorImage;
//    public TextMeshProUGUI actorName;
//    public TextMeshProUGUI messageText;
//    public RectTransform backgroundBox;

//    private Message[] currentMessages;
//    private Actor[] currentActors;
//    private int activeMessage = 0;
//    public static bool isActive = false;

//    public float typingSpeed = 0.05f; // Speed of typing effect in seconds per character
//    private Coroutine typingCoroutine; // Store the reference to the typing coroutine

//    public bool canSkipDialogue = true; // Variable to enable or disable skipping

//    void Update()
//    {
//        // Check for input if skipping is allowed and dialogue is active
//        if (canSkipDialogue && isActive && Input.anyKeyDown)
//        {
//            SkipCurrentMessage();
//        }
//    }

//    // Method to start the dialogue
//    public void OpenDialogue(Message[] messages, Actor[] actors)
//    {
//        currentMessages = messages;
//        currentActors = actors;
//        activeMessage = 0;
//        isActive = true;
//        Debug.Log("Started conversation! Loaded messages: " + messages.Length);
//        backgroundBox.gameObject.SetActive(true); // Show the dialogue UI
//        StartCoroutine(DisplayMessageWithTypingEffect());
//    }

//    // Coroutine to handle typing effect and message display
//    IEnumerator DisplayMessageWithTypingEffect()
//    {
//        while (isActive)
//        {
//            DisplayMessage();
//            yield return new WaitForSeconds(currentMessages[activeMessage].duration);
//            NextMessage();
//        }
//    }

//    // Displays the current message and actor
//    void DisplayMessage()
//    {
//        if (activeMessage < currentMessages.Length)
//        {
//            Message messageToDisplay = currentMessages[activeMessage];
//            Actor actorToDisplay = currentActors[messageToDisplay.actorId];
//            actorName.text = actorToDisplay.name;
//            ActorImage.sprite = actorToDisplay.sprite;

//            if (typingCoroutine != null)
//            {
//                StopCoroutine(typingCoroutine); // Stop previous typing coroutine
//            }

//            typingCoroutine = StartCoroutine(TypeMessage(messageToDisplay.message));
//        }
//    }

//    // Coroutine to type out each character of the message
//    IEnumerator TypeMessage(string message)
//    {
//        messageText.text = "";
//        foreach (char letter in message.ToCharArray())
//        {
//            messageText.text += letter;
//            yield return new WaitForSeconds(typingSpeed);
//        }
//    }

//    // Move to the next message
//    public void NextMessage()
//    {
//        activeMessage++;
//        if (activeMessage < currentMessages.Length)
//        {
//            StopAllCoroutines(); // Stop all coroutines before starting a new message
//            StartCoroutine(DisplayMessageWithTypingEffect());
//        }
//        else
//        {
//            EndDialogue(); // Call EndDialogue when the conversation is finished
//        }
//    }

//    // Updated function to skip to the next message
//    public void SkipCurrentMessage()
//    {
//        if (typingCoroutine != null)
//        {
//            StopCoroutine(typingCoroutine); // Stop the typing effect
//        }
//        NextMessage(); // Jump to the next message
//    }

//    // New method to stop the dialogue mid-way, useful for cutscene ending or skipping
//    public void StopDialogue()
//    {
//        StopAllCoroutines(); // Stop all running coroutines
//        isActive = false; // Mark the dialogue as inactive
//        backgroundBox.gameObject.SetActive(false); // Hide the dialogue UI
//        Debug.Log("Dialogue interrupted and stopped.");
//    }

//    // Method to end the dialogue when all messages are finished
//    private void EndDialogue()
//    {
//        Debug.Log("Conversation ended.");
//        isActive = false; // Mark the dialogue as inactive
//        backgroundBox.gameObject.SetActive(false); // Hide the dialogue UI
//    }
//}
