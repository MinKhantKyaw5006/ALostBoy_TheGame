/*
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float moveUpDistance = 5f; // Distance to move up
    public float moveSpeed = 2f; // Speed of the move
    [SerializeField] private AudioSource unlockSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool hasBeenOpened = false; // Flag to track if the door has been opened

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, moveUpDistance, 0);
        Debug.Log("Door initialized at position: " + closedPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpening && !hasBeenOpened)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;

        if (unlockSound != null)
        {
            unlockSound.Play();
        }
        else
        {
            Debug.LogWarning("Unlock sound is not assigned");
        }

        Debug.Log("Opening door");

        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition; // Snap to the exact position
        isOpening = false;
        hasBeenOpened = true; // Set the flag to true after opening once
        Debug.Log("Door opened");
    }
}
*/

using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float moveUpDistance = 5f; // Distance to move up
    public float moveSpeed = 2f; // Speed of the move
    [SerializeField] private AudioSource unlockSound;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool hasBeenOpened = false; // Flag to track if the door has been opened

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, moveUpDistance, 0);
        //Debug.Log("Door initialized at position: " + closedPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpening && !hasBeenOpened)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;

        if (unlockSound != null)
        {
            unlockSound.Play();
        }
        else
        {
            Debug.LogWarning("Unlock sound is not assigned");
        }

        Debug.Log("Opening door");

        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPosition; // Snap to the exact position
        isOpening = false;
        hasBeenOpened = true; // Set the flag to true after opening once
        Debug.Log("Door opened");
    }

    public void OpenWithoutCondition()
    {
        if (!isOpening && !hasBeenOpened)
        {
            StartCoroutine(OpenDoor());
        }
    }
}

