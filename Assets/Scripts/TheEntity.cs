using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TheEntity : MonoBehaviour
{
    public Transform[] waypoints; // Set waypoints in the editor
    private int currentWaypointIndex = 0;

    private NavMeshAgent agent;

    [SerializeField] private float chaseDistance = 5f; // Distance at which Granny starts chasing
    [SerializeField] private float wanderRadius = 10f; // Radius for random wandering
    [SerializeField] private float loseChaseDistance = 10f; // Distance at which Granny loses sight of the player
    private Transform player;

    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip chaseSfx;
    private AudioSource audioSource;

    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GoToNextWaypoint();
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }

        CheckForPlayer();
    }

    void GoToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.destination = waypoints[currentWaypointIndex].position;

        if (Random.value > 0.7f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        PlayFootstepAudio();
    }

    void CheckForPlayer()
    {
        // Get the horizontal position of the player
        Vector3 horizontalDistanceToPlayer = new Vector3(player.position.x, transform.position.y, player.position.z);

        // Calculate the distance ignoring vertical differences
        float horizontalDistance = Vector3.Distance(transform.position, horizontalDistanceToPlayer);

        // Calculate the vertical distance
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

        // Check if the player is within chase distance and on the same floor (or close enough)
        if (horizontalDistance < chaseDistance && verticalDistance < 1.0f) // Adjust 1.0f as needed for floor height
        {
            isChasing = true;
            PlayChaseAudio();
        }
        else if (isChasing && (horizontalDistance > loseChaseDistance || verticalDistance >= 1.0f))
        {
            isChasing = false;
            GoToNextWaypoint();
        }
    }


    void ChasePlayer()
    {
        agent.destination = player.position;
        if (!audioSource.isPlaying)
        {
            PlayChaseAudio();
        }
    }

    void PlayFootstepAudio()
    {
        if (footstepSfx && !isChasing)
        {
            if (audioSource.clip != footstepSfx)
            {
                audioSource.clip = footstepSfx;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    void PlayChaseAudio()
    {
        if (chaseSfx && isChasing)
        {
            if (audioSource.clip != chaseSfx)
            {
                audioSource.clip = chaseSfx;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
