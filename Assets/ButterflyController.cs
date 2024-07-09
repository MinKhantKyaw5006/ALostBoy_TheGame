using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ButterflyController : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints; // Waypoints for the butterfly to follow
    public float waitDistance = 5.0f; // Distance at which the butterfly waits for the player
    public float resumeDistance = 2.0f; // Distance at which the butterfly resumes leading
    public float waitTime = 2.0f; // Time to wait at each waypoint

    private NavMeshAgent navMeshAgent;
    private int currentWaypointIndex = 0;
    private bool waitingForPlayer = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    void Update()
    {
        if (!waitingForPlayer && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > waitDistance)
        {
            WaitForPlayer();
        }
        else if (distanceToPlayer < resumeDistance && waitingForPlayer)
        {
            ResumeLeading();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0)
            return;

        navMeshAgent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(waitTime);
        GoToNextWaypoint();
    }

    void WaitForPlayer()
    {
        waitingForPlayer = true;
        navMeshAgent.isStopped = true;
    }

    void ResumeLeading()
    {
        waitingForPlayer = false;
        navMeshAgent.isStopped = false;
        GoToNextWaypoint();
    }
}
