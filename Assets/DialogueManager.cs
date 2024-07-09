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
}
