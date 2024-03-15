using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    public int Speed;
    Vector2 targetPos;
    private Vector3 playerOffset;

    void Start()
    {
        targetPos = posB.position;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, posA.position) < 0.1f)
        {
            targetPos = posB.position;
        }
        else if (Vector2.Distance(transform.position, posB.position) < 0.1f)
        {
            targetPos = posA.position;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);

        // Move the player smoothly along with the platform
        if (playerOffset.magnitude > 0.01f)
        {
            PlayerMoveWithPlatform();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Store the offset between player and platform
            playerOffset = other.transform.position - transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOffset = Vector3.zero;
        }
    }

    private void PlayerMoveWithPlatform()
    {
        // Move the player along with the platform
        Vector3 playerTargetPos = transform.position + playerOffset;
        GameObject.FindGameObjectWithTag("Player").transform.position = Vector3.MoveTowards(GameObject.FindGameObjectWithTag("Player").transform.position, playerTargetPos, Speed * Time.deltaTime);
    }
}
