using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougueTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actors;

    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(messages, actors);
    }
}

[System.Serializable]
public class Message
{
    public int actorId;
    public string message;
    public float duration; // Duration to display this message
}


[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DialougueTrigger : MonoBehaviour
//{
//    public Message[] messages; // Dialogue messages for this trigger
//    public Actor[] actors; // Actors for this dialogue

//    // Method to start dialogue
//    public void StartDialogue()
//    {
//        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
//        if (dialogueManager != null)
//        {
//            dialogueManager.OpenDialogue(messages, actors); // Start the dialogue via DialogueManager
//        }
//        else
//        {
//            Debug.LogError("DialogueManager not found in the scene.");
//        }
//    }
//}

//[System.Serializable]
//public class Message
//{
//    public int actorId; // Actor ID to display
//    public string message; // The message text
//    public float duration; // Duration to display this message
//}

//[System.Serializable]
//public class Actor
//{
//    public string name; // Actor's name
//    public Sprite sprite; // Actor's sprite or image
//}
