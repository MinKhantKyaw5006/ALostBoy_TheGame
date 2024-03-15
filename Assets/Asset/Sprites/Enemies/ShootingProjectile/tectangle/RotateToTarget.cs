using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : Enemy
{
    /*
    public float rotationSpeed;
    private Vector2 direction;
    public float moveSpeed;

    private void Update()
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.MoveTowards(transform.position, cursorPos , moveSpeed * Time.deltaTime);
    }
    */
    public float moveSpeed;
    public Collider2D movementBounds; // Box collider defining the movement bounds

    private Vector3 targetPosition; // Target position within the movement bounds

    protected override void Start()
    {
        base.Start();
        UpdateTargetPosition();
    }

    protected override void Update()
    {
        base.Update();

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate towards the direction of movement
        Vector2 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // If the enemy reaches the target position, update the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            UpdateTargetPosition();
        }
    }

    // Method to update the target position within the movement bounds
    protected virtual void UpdateTargetPosition()
    {
        // Generate a random position within the movement bounds
        Vector3 minBounds = movementBounds.bounds.min;
        Vector3 maxBounds = movementBounds.bounds.max;

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);

        targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }
}
