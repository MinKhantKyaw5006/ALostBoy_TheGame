using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingEnemy : Enemy
{

    public float moveSpeed;
    public GameObject[] wayPoints;
    public float yOffset = 0.5f; // Offset for the enemy's height above the lines
    public float xOffset = 0.5f; // Offset for the enemy's position along the x-axis relative to waypoints
    public float xscale = 0.5f; // Scaling factor for the x-axis
    public float yscale = 0.5f; // Scaling factor for the y-axis

    private int nextWaypoint = 1;
    private float disToPoint; // This will store the remaining distance between player and next waypoint

    private void OnDrawGizmos()
    {
        // Draw lines between waypoints
        Gizmos.color = Color.yellow;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            int nextIndex = (i + 1) % wayPoints.Length;
            Vector3 startPoint = GetWaypointPosition(wayPoints[i]);
            Vector3 endPoint = GetWaypointPosition(wayPoints[nextIndex]);
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }

    private Vector3 GetWaypointPosition(GameObject waypoint)
    {
        // Adjust position based on waypoint scale and offsets
        Vector3 position = waypoint.transform.position;
        position += Vector3.up * yOffset * waypoint.transform.localScale.y * yscale; // Adjust for y offset and waypoint scale
        position += Vector3.right * xOffset * waypoint.transform.localScale.x * xscale; // Adjust for x offset and waypoint scale
        return position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateEnemyStates();
        CheckHealth(); // Check health for destruction
    }

    void Move()
    {
        Vector3 targetPosition = GetWaypointPosition(wayPoints[nextWaypoint]);
        disToPoint = Vector2.Distance(transform.position, targetPosition);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (disToPoint < 0.2f)
        {
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        Vector3 currRot = transform.eulerAngles;
        currRot.z += wayPoints[nextWaypoint].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
        ChooseNextWaypoint();
    }

    void ChooseNextWaypoint()
    {
        nextWaypoint++;
        if (nextWaypoint == wayPoints.Length)
        {
            nextWaypoint = 0;
        }
    }
}
