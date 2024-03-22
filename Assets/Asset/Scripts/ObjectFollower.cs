using UnityEngine;
using UnityEngine.AI;

public class ObjectFollower : MonoBehaviour
{
    public Transform destination;
    public float followRange = 10f;
    public Transform idlePosition;

    private NavMeshAgent agent;
    private GameObject player;
    private bool isFollowingPlayer = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (isFollowingPlayer && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > followRange)
            {
                isFollowingPlayer = false;
                agent.SetDestination(idlePosition.position);
            }
            else
            {
                agent.SetDestination(player.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered by: " + other.name); // Log the name of the object that entered the trigger

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected in range. Starting to follow.");
            isFollowingPlayer = true;
            player = other.gameObject;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isFollowingPlayer = false;
            agent.SetDestination(idlePosition.position);
        }
    }

    // This function is called to draw Gizmos in the Editor
    void OnDrawGizmosSelected()
    {
        // Draw a yellow wireframe sphere representing the follow range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
