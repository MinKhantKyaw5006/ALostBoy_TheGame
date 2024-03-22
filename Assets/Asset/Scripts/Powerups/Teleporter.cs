using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private AudioClip teleportSound; // The sound effect for teleporting
    private AudioSource audioSource;

    private void Awake()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Optionally add an AudioSource if one is not already attached
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public Transform GetDestination()
    {
        return destination;
    }

    public void PlayTeleportSound()
    {
        if (teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }
    }
}
