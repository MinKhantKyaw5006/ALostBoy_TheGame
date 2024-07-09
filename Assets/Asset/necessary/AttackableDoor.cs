using System;
using System.Collections;
using UnityEngine;

public class AttackableDoor : AttackableItem
{
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private EnemySpawner enemySpawner; // Reference to the enemy spawner

    private float currentMoveDistance = 0f;
    private Vector3 initialPosition;
    private Coroutine closingCoroutine;
    private bool isFullyOpened = false;

    public event Action OnDoorFullyOpened;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (!isFullyOpened && closingCoroutine == null && currentMoveDistance > 0)
        {
            closingCoroutine = StartCoroutine(CloseDoor());
        }
    }

    public override void OnHit()
    {
        base.OnHit();
        if (!isFullyOpened)
        {
            MoveDoor();
        }
    }

    private void MoveDoor()
    {
        if (currentMoveDistance < moveDistance)
        {
            transform.position += Vector3.up * moveSpeed;
            currentMoveDistance += moveSpeed;
            Debug.Log("Door moved. Current move distance: " + currentMoveDistance);

            if (closingCoroutine != null)
            {
                StopCoroutine(closingCoroutine);
                closingCoroutine = null;
            }

            if (currentMoveDistance >= moveDistance)
            {
                isFullyOpened = true;
                OnDoorFullyOpened?.Invoke(); // Trigger the event
                if (enemySpawner != null)
                {
                    enemySpawner.StopSpawning();
                }
                Debug.Log("Door fully opened.");
            }
        }
    }

    private IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(closeDelay);

        float closeSpeed = moveSpeed / 2;

        while (currentMoveDistance > 0 && !isFullyOpened)
        {
            transform.position -= Vector3.up * closeSpeed;
            currentMoveDistance -= closeSpeed;
            yield return new WaitForSeconds(0.02f);
        }

        if (!isFullyOpened)
        {
            transform.position = initialPosition;
            currentMoveDistance = 0;
            Debug.Log("Door closed.");
        }
    }

    public bool IsFullyOpened()
    {
        return isFullyOpened;
    }
}

