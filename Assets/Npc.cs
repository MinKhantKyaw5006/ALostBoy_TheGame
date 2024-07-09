using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public DialougueTrigger trigger;
    private bool hasInteracted = false; // Flag to check if interaction has happened

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasInteracted)
        {
            hasInteracted = true; // Set the flag to true after interaction
            trigger.StartDialogue();
        }
    }
}
